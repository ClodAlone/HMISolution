// <copyright file="TabLayoutPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;



namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// Class which represents the New tab layout panel logic
    /// </summary>
    public class NewTabLayout : Panel
    {
        /// <summary>
        /// When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement"/>-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// The size that this element determines it needs during layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size size = new Size();
            if (InternalChildren.Count > 0)
            {
                ContentPresenter toolbar = InternalChildren[0] as ContentPresenter;
                FrameworkElement newtab = InternalChildren[1] as FrameworkElement;
                if ((newtab!=null && newtab.Visibility == Visibility.Visible) || (toolbar!=null && toolbar.Visibility==Visibility.Visible))
                {
                    var tabparent=TemplatedParent; 
                    if((TemplatedParent is TabPanelAdv))
                    {
                        tabparent = (TemplatedParent as TabPanelAdv).TemplatedParent;
                    }
                    if (tabparent != null)
                    {
                        if (tabparent is TabControlExt)
                        {                            
                           if ((tabparent as TabControlExt).IsNewButtonEnabled || (tabparent as TabControlExt).ToolBarTray!=null)
                           {
                                if ((tabparent as TabControlExt).IsNewButtonEnabled)
                                {
                                    Visibility = Visibility.Visible;
                                    newtab.Measure(availableSize);
                                    size = newtab.DesiredSize;
                                    size.Width += size.Width * .10;
                                }
                                if ((tabparent as TabControlExt).ToolBarTray != null)
                                {
                                    Visibility = Visibility.Visible;                                    
                                    toolbar.Measure(availableSize);
                                    size.Width += toolbar.DesiredSize.Width;                                   
                                                                            
                                }
                            }
                           else if (tabparent is DocumentTabControl)
                           {
                               DocumentContainer container = VisualUtils.FindAncestor(this, typeof(DocumentContainer)) as DocumentContainer;
                               if(container!=null)
                               {
                                if((container.TDIToolBarTray!=null) || ((container.DockingManager!=null)&&(container.DockingManager.TDIToolBarTray!=null)))                              
                                {
                                    Visibility = Visibility.Visible;                                    
                                    toolbar.Measure(availableSize);
                                    size.Width += toolbar.DesiredSize.Width;
                                }
                                else
                                {
                                     Visibility = Visibility.Collapsed;
                                     return base.MeasureOverride(availableSize); 
                                }
                               }
                               else
                               {
                                    Visibility = Visibility.Collapsed;
                                    return base.MeasureOverride(availableSize);    
                               }
                            
                           }
                            else
                            {
                                    Visibility = Visibility.Collapsed;
                                    return base.MeasureOverride(availableSize);                                
                            }
                        }
                    }                    
                }        

            }
            return size;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count > 0)
            {
                ContentPresenter toolbar = InternalChildren[0] as ContentPresenter;
                FrameworkElement newtab = InternalChildren[1] as FrameworkElement;
                var tabparent=TemplatedParent; 
                    if((TemplatedParent is TabPanelAdv))
                    {
                        tabparent = (TemplatedParent as TabPanelAdv).TemplatedParent;
                    }
                    if (tabparent != null)
                    {
                        if (tabparent is TabControlExt)
                        {
                            if ((tabparent as TabControlExt).IsNewButtonEnabled || (tabparent as TabControlExt).ToolBarTray != null)
                            {
                                if ((tabparent as TabControlExt).ToolBarTray != null)
                                toolbar.Arrange(new Rect(0, 0, toolbar.DesiredSize.Width, toolbar.DesiredSize.Height));
                                if ((tabparent as TabControlExt).IsNewButtonEnabled)
                                {
                                    if ((tabparent as TabControlExt).ToolBarTray == null)
                                        newtab.Arrange(new Rect(0, 0, newtab.DesiredSize.Width, newtab.DesiredSize.Height));
                                    else
                                        newtab.Arrange(new Rect(toolbar.DesiredSize.Width, 0, newtab.DesiredSize.Width, newtab.DesiredSize.Height));                                    
                                }
                            }                            
                        }                        
                    }
            }

            return base.ArrangeOverride(finalSize);
        }
    }
    /// <summary>
    /// Panel that is responsible for tab layout logic.
    /// </summary>
    public class TabLayoutPanel : Panel
    {
        #region Constants
        /// <summary>
        /// Stores the tab intersection factor.
        /// </summary>
        internal const double TAB_INTERSECTION_FACTOR = 1.5;

        /// <summary>
        /// Stores the tab shift value.
        /// </summary>
        private const int VS2008_TAB_SHIFT = 12;

        /// <summary>
        /// Presents name of EditableHeader text box.
        /// </summary>
        private const string C_EDITABLE_HEADER_NAME = "EditableHeader";

        /// <summary>
        /// Presents name of content.
        /// </summary>
        private const string C_CONTENT_NAME = "PART_EditHeader";
        #endregion

        #region Structs
        /// <summary>
        /// The structure for DragInfo.
        /// </summary>
        internal struct DragInfo
        {
            #region Private members
            /// <summary>
            /// Stores the drag marker.
            /// </summary>
            private DragMarkerAdorner dragMarker;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets the dragged item.
            /// </summary>
            /// <value>The dragged item.</value>
            public TabItemExt DragedItem
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the drag over item.
            /// </summary>
            /// <value>The drag over item.</value>
            public TabItemExt DragOverItem
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the adorner alignment.
            /// </summary>
            /// <value>The adorner alignment.</value>
            public AdornerAlignment AdornerAlignment
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether [rotate text when vertical].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [rotate text when vertical]; otherwise, <c>false</c>.
            /// </value>
            public bool RotateTextWhenVertical
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether [skip drag].
            /// </summary>
            /// <value><c>true</c> if [skip drag]; otherwise, <c>false</c>.</value>
            public bool SkipDrag
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the owner.
            /// </summary>
            /// <value>The owner.</value>
            public TabControlExt Owner
            {
                get;
                set;
            }

            /// <summary>
            /// Gets a value indicating whether this instance is initialized.
            /// </summary>
            /// <value>
            ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
            /// </value>
            private bool IsInitialized
            {
                get
                {
                    return DragOverItem != null && dragMarker != null;
                }
            }

            #endregion

            #region Public methods
            /// <summary>
            /// Determines whether this instance can drag.
            /// </summary>
            /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
            /// <returns>
            ///    <c>true</c> if this instance can drag the specified e; otherwise, <c>false</c>.
            /// </returns>
            public bool CanDrag(DragEventArgs e)
            {
                TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));
                Point position = e.GetPosition(item);
                double distance = RotateTextWhenVertical
                       ? (IsLessThanMiddle(position, item) ? position.Y : item.ActualHeight - position.Y)
                       : (IsLessThanMiddle(position, item) ? position.X : item.ActualWidth - position.X);
                return distance > 2;
            }

            /// <summary>
            /// Determines whether is less than middle.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="item">Value of the item.</param>
            /// <returns>
            ///      <c>true</c> if [is less than middle] [the specified position]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsLessThanMiddle(Point position, FrameworkElement item)
            {
                return RotateTextWhenVertical
                    ? position.Y < item.ActualHeight / 2
                    : position.X < item.ActualWidth / 2;
            }

            /// <summary>
            /// Refreshes the marker.
            /// </summary>
            /// <param name="newItem">The new item.</param>
            public void RefreshMarker(TabItemExt newItem)
            {
                RemoveAdorner();
                DragOverItem = newItem;
                AddAdorner();
            }

            /// <summary>
            /// Clears the marker.
            /// </summary>
            public void ClearMarker()
            {
                if (IsInitialized)
                {
                    RemoveAdorner();
                    DragOverItem = null;
                }
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Adds the adorner.
            /// </summary>
            private void AddAdorner()
            {
                if (DragOverItem != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(DragOverItem);

                    if (adornerLayer != null)
                    {
                        TabControlExt parent = (DragOverItem as TabItemExt).TabControlParent as TabControlExt;
                        dragMarker = new DragMarkerAdorner(DragOverItem);

                        if (parent != null)
                        {
                            dragMarker.TabStripPlacement = parent.TabStripPlacement;
                            dragMarker.SetStyle(parent.DragMarkerStyle);
                            dragMarker.MarkerColor = parent.DragMarkerColor;
                        }

                        dragMarker.AdornerAlignment = AdornerAlignment;
                        dragMarker.RotateTextWhenVertical = RotateTextWhenVertical;
                        dragMarker.CoerceOffset();
                        adornerLayer.Add(dragMarker);
                    }
                }
            }

            /// <summary>
            /// Removes the adorner.
            /// </summary>
            private void RemoveAdorner()
            {
                if (DragOverItem != null)
                {
                    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(DragOverItem);
                    if (adornerLayer != null)
                    {
                        if (dragMarker == null)
                        {
                            dragMarker = new DragMarkerAdorner(DragOverItem);
                        }

                        adornerLayer.Remove(dragMarker);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Structure for ScrollInfo.
        /// </summary>
        internal struct ScrollInfo
        {
            /// <summary>
            /// Stores bool value based on whether the Scroll button should be displayed
            /// </summary>
            public bool NeedScrollButtonsShow;

            /// <summary>
            /// Stores the first trimmed tab index.
            /// </summary>
            public int FirstTrimmedTabIndex;

            /// <summary>
            /// Stores the last trimmed tab index.
            /// </summary>
            public int LastTrimmedTabIndex;

            /// <summary>
            /// Stores the desired width.
            /// </summary>
            public double DesiredWidth;

            /// <summary>
            /// Stores the last tab trimmed width.
            /// </summary>
            public double LastTabTrimmedWidth;

            /// <summary>
            /// Stores the first tab trimmed width.
            /// </summary>
            public double FirstTabTrimmedWidth;

            /// <summary>
            /// Stores the page width.
            /// </summary>
            public double PageWidth;

            /// <summary>
            /// Stores the AllTrimmed width.
            /// </summary>
            public double AllTrimmedWidth;

            /// <summary>
            /// Stores the offset.
            /// </summary>
            public double Offset;
        }

        /// <summary>
        /// Structure for ClickInfo.
        /// </summary>
        private struct ClickInfo
        {
            /// <summary>
            /// Time when the last click on title bar was performed.
            /// </summary>
            public DateTime LastTabItemClick;

            /// <summary>
            /// Point where the last click on title bar was performed.
            /// </summary>
            public Point LastTabItemPoint;

            /// <summary>
            /// Determines whether [is double click] [the specified position].
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>
            ///     <c>true</c> if [is double click] [the specified position]; otherwise, <c>false</c>.
            /// </returns>
            public bool IsDoubleClick(Point position)
            {
                if (((DateTime.Now.Subtract(LastTabItemClick).TotalMilliseconds < 500)
                    && (Math.Abs((LastTabItemPoint.X - position.X)) <= 2))
                    && (Math.Abs((LastTabItemPoint.Y - position.Y)) <= 2))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Determines whether is drag started.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <returns>
            ///     <c>true</c> if is drag started; otherwise, <c>false</c>.
            /// </returns>
            public bool IsDragStarted(Point position)
            {
                if (Math.Abs(position.X - LastTabItemPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - LastTabItemPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Private members

        /// <summary>
        /// Stores the story board.
        /// </summary>
        private Storyboard m_scrollStoryboard;

        /// <summary>
        /// Stores the double animation.
        /// </summary>
        private DoubleAnimation m_scrollAnimation;

        /// <summary>
        /// Stores the Tab item.
        /// </summary>
        private TabItemExt m_editingItem;

        /// <summary>
        /// Stores the Editable header.
        /// </summary>
        private TextBox m_editableHeader;

        /// <summary>
        /// Stores the average width.
        /// </summary>
        internal double AverageWidth;

        /// <summary>
        /// Stores the row height.
        /// </summary>
        internal double m_RowHeight;

        /// <summary>
        /// Stores the number of rows.
        /// </summary>
        internal int m_NumRows;

        /// <summary>
        /// Stores the Tab control.
        /// </summary>
        internal TabControlExt m_ParentTabControl;

        /// <summary>
        /// Stores the parent tab control.
        /// </summary>
        private TabPanelAdv m_parentTabPanel;

        internal bool m_AllowDrag = false;

        /// <summary>
        /// Stores the scroll information.
        /// </summary>
        internal ScrollInfo m_scrollInfo;

        /// <summary>
        /// Stores the click information
        /// </summary>
        private ClickInfo m_clickInfo;

        /// <summary>
        /// Stores the drag information.
        /// </summary>
        internal DragInfo m_dragInfo;

        /// <summary>
        /// Stores the bool value denoting whether the needle shrinks.
        /// </summary>
        private bool m_needShrink;

        /// <summary>
        /// Represents the Scrolled Offset
        /// </summary>
        private double scrolledOffset = 0.0;

        /// <summary>
        /// Represents the count
        /// </summary>
        private int count = 0;

        /// <summary>
        /// Represents the Add last value
        /// </summary>
        internal static bool AddAtLast = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the visible items count.
        /// </summary>
        /// <value>The visible items count.</value>
        internal int VisibleItemsCount
        {
            get
            {
                int count = 0;

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility == Visibility.Visible)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [need shrink].
        /// </summary>
        /// <value><c>true</c> if [need shrink]; otherwise, <c>false</c>.</value>
        internal bool NeedShrink
        {
            get
            {
                return m_needShrink;
            }
        }

        /// <summary>
        /// Gets the tab scroll style.
        /// </summary>
        /// <value>The tab scroll style.</value>
        internal TabScrollStyle TabScrollStyle
        {
            get
            {
                TabScrollStyle style = TabScrollStyle.Normal;
                if (m_ParentTabControl != null)
                {
                    style = m_ParentTabControl.TabScrollStyle;
                }

                return style;
            }
        }

        /// <summary>
        /// Gets or sets the scroll offset.
        /// </summary>
        /// <value>The scroll offset.</value>
        protected double ScrollOffset
        {
            get
            {
                return (double)GetValue(ScrollOffsetProperty);
            }

            set
            {
                SetValue(ScrollOffsetProperty, value);
            }
        }

        /// <summary>
        /// Local variable which represents tab
        /// </summary>
        internal TabItemExt tab = null;

		/// <summary>
		/// Gets the collection used to generate the tab items.
		/// </summary>
		internal IList SourceItems
		{
			get 
			{
				return m_ParentTabControl.ItemsSource as IList;
			}
		}

        internal IList TabItemExtCollection
        {
            get
            {
                return m_ParentTabControl.ItemsSource as IList;
            }
        }

        internal TabPanelAdv TabPanelAdv
        {
            get
            {
                if (Parent is ScrollViewer)
                    return (Parent as ScrollViewer).Parent as TabPanelAdv;
                return Parent as TabPanelAdv;
            }
        }

        internal TabControlExt GetTabControl()
        {
            return VisualUtils.FindAncestor(this, typeof(TabControlExt)) as TabControlExt;
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabStripPlacement property.
        /// </summary>
        /// <value>The tab strip placement.</value>
        private Dock TabStripPlacement
        {
            get
            {
                Dock top = Dock.Top;

                if (m_ParentTabControl != null)
                {
                    top = m_ParentTabControl.TabStripPlacement;
                }

                return top;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Duplicate TabControlExt KeepTabInFront.
        /// </summary>
        /// <value><c>true</c> if [keep tab in front]; otherwise, <c>false</c>.</value>
        private bool KeepTabInFront
        {
            get
            {
                bool result = true;
                if (m_ParentTabControl != null)
                {
                    result = m_ParentTabControl.KeepTabInFront;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabItemLayout property.
        /// </summary>
        /// <value>The tab item layout.</value>
        private TabItemLayoutType TabItemLayout
        {
            get
            {
                TabItemLayoutType result = TabItemLayoutType.SingleLine;
                if (m_ParentTabControl != null)
                {
                    result = m_ParentTabControl.TabItemLayout;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabItemSize property.
        /// </summary>
        /// <value>The size of the tab item.</value>
        private TabItemSizeMode TabItemSize
        {
            get
            {
                TabItemSizeMode result = TabItemSizeMode.Normal;
                if (m_ParentTabControl != null)
                {
                    result = m_ParentTabControl.TabItemSize;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the duplicate TabControlExt TabScrollButtonVisibility property.
        /// </summary>
        /// <value>The tab scroll button visibility.</value>
        private TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get
            {
                TabScrollButtonVisibility visibility = TabScrollButtonVisibility.Auto;
                if (m_ParentTabControl != null)
                {
                    visibility = m_ParentTabControl.TabScrollButtonVisibility;
                }

                return visibility;
            }
        }

        /// <summary>
        /// Gets the tab items.
        /// </summary>
        /// <value>The tab items.</value>
        private ItemCollection TabItems
        {
            get
            {
                return m_ParentTabControl != null
                    ? m_ParentTabControl.Items : null;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TabLayoutPanel"/> class.
        /// </summary>
        static TabLayoutPanel()
        {
            EnvironmentTest.ValidateLicense(typeof(TabLayoutPanel));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(TabLayoutPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(TabLayoutPanel), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabLayoutPanel"/> class.
        /// </summary>
        public TabLayoutPanel()
        {
            ClipToBounds = true;
            m_scrollInfo = new ScrollInfo();
            m_clickInfo = new ClickInfo();
            m_dragInfo = new DragInfo();
#if !SyncfusionFramework3_5
            //IsManipulationEnabled = true;
#endif
            this.Loaded += new RoutedEventHandler(TabLayoutPanel_Loaded);
        }

        /// <summary>
		/// Handles the Loaded event of the TabLayoutPanel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		void TabLayoutPanel_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= TabLayoutPanel_Loaded;
		}
        #endregion

        #region Public methods
        

        /// <summary>
        /// Launch scroll int to the previous page.
        /// </summary>
        public void ScrollToPrevPage()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
                TabPanelAdv.ChildScrollViewer.ScrollToHorizontalOffset(TabPanelAdv.ChildScrollViewer.HorizontalOffset - TabPanelAdv.PageScrollWidth);
        }

        /// <summary>
        /// Adds the new tab.
        /// </summary>
        internal void AddNewTab()
        {
            if (m_ParentTabControl.IsNewButtonEnabled)
            {
                tab = new TabItemExt
                {
                    // tabParent=m_ParentTabControl,
                    IsNewTab = true,
                    newtabParent = m_ParentTabControl,
                };

                if (m_ParentTabControl.NewButtonTemplate != null)
                {
                    tab.Template = m_ParentTabControl.NewButtonTemplate;
                }

                if (m_ParentTabControl.NewButtonStyle != null)
                {
                    tab.Style = m_ParentTabControl.NewButtonStyle;
                }

                AddLogicalChild(tab);
                AddVisualChild(tab);

            }
        }

        internal void RemoveNewTab()
        {
            if (tab != null)
            {
                RemoveLogicalChild(tab);
                RemoveVisualChild(tab);
                tab = null;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Process scrolling mechanism.
        /// </summary>
        /// <param name="scrollDirection">The direction of scroll</param>
        internal void ProcessScrollInternal(ScrollDirection scrollDirection)
        {
            m_IsScrollButtonClicked = true;
            switch (scrollDirection)
            {
                case ScrollDirection.NextTab:
                    ScrollToNextTab();
                    break;
                case ScrollDirection.PrevTab:
                    ScrollToPrevTab();
                    break;
                case ScrollDirection.NextPage:
                    ScrollToNextPage();
                    break;
                case ScrollDirection.PrevPage:
                    ScrollToPrevPage();
                    break;
                case ScrollDirection.FirstTab:
                    ScrollToFirstTab();
                    break;
                case ScrollDirection.LastTab:
                    ScrollToLastTab();
                    break;
                default:
                    break;
            }
        }

        internal bool m_NeedScrolling = true;
        internal bool m_IsRemoving = false;
        internal double m_CheckingSize = 0.0;
        internal bool m_SelectLastItem = false;
        internal TabItemExt m_ScrollToSelectedItem;
        internal bool m_IsScrollButtonClicked;
        /// <summary>
        /// Selects the item internal.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        internal void SelectItemInternal(TabItemExt item)
        {
            if (item != null)
            {
                m_ScrollToSelectedItem = item;
               int itemIndex = m_ParentTabControl.ItemContainerGenerator.IndexFromContainer(m_ParentTabControl.GetTabItem(item));
                bool needScroll = false;
                int sind = m_ParentTabControl.SelectedIndex;

                if (AddAtLast)
                {
                    needScroll = (itemIndex >= 0 && itemIndex <= m_scrollInfo.FirstTrimmedTabIndex)
                        || (itemIndex < InternalChildren.Count && itemIndex >= m_scrollInfo.LastTrimmedTabIndex - 1);
                }
                else
                {
                    needScroll = (itemIndex >= 0 && itemIndex <= m_scrollInfo.FirstTrimmedTabIndex)
                        || (itemIndex < InternalChildren.Count && itemIndex >= m_scrollInfo.LastTrimmedTabIndex);
                }

                if (!item.IsVisible)
                {
                    needScroll = true;
                    //  itemIndex++;
                }

                if (m_NeedScrolling)
                {
                    if (needScroll && TabItemLayout == TabItemLayoutType.SingleLine && TabItemSize == TabItemSizeMode.Normal)
                    {
                        double endOffset = itemIndex <= m_scrollInfo.FirstTrimmedTabIndex
                            ? PrepareScrollInfo(item, itemIndex, m_scrollInfo.FirstTrimmedTabIndex, 1, m_scrollInfo.FirstTabTrimmedWidth)
                            : PrepareScrollInfo(item, m_scrollInfo.LastTrimmedTabIndex + 1, itemIndex + 1, -1, m_scrollInfo.LastTabTrimmedWidth);

                        if (!TabControlExt.GetIsEditing(item))
                        {
                            if (itemIndex == 0)
                                m_scrollInfo.Offset = 0.0;
                            else if (itemIndex == InternalChildren.Count - 1)
                            {
                                item.Loaded += item_Loaded;
                                item.Unloaded += item_Unloaded;
                            }
                            else if (m_scrollInfo.Offset != endOffset)
                                StartScrolling(m_scrollInfo.Offset, endOffset);
                        }
                    }
                }
                else
                {
                    if (!m_IsRemoving)
                    {
                        if (!TabControlExt.GetIsEditing(item))
                        {
                            m_scrollInfo.Offset = m_CheckingSize;
                        }
                    }
                }

                if (m_ParentTabControl.SelectedIndex != itemIndex && itemIndex!=-1) 
                {
                    m_ParentTabControl.SelectedIndex = itemIndex;
                }
            }
        }

        void item_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as TabItemExt).Loaded -= item_Loaded;
        }

        void item_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateArrange();
            m_SelectLastItem = true;
        }

        /// <summary>
        /// Gets an enumerator that can iterate the logical child elements of this <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/>. This property has no default value.
        /// </returns>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                return base.LogicalChildren;
            }
        }

        /// <summary>
        /// Prepares the scroll info.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <param name="start">Value of the start.</param>
        /// <param name="end">Value of the end.</param>
        /// <param name="inc">Value of the inc.</param>
        /// <param name="boundaryWidth">Width of the boundary.</param>
        /// <returns>Returns a double value</returns>
        internal double PrepareScrollInfo(DependencyObject item, int start, int end, int inc, double boundaryWidth)
        {
            double scrollWidth = 0.0;

            if (AddAtLast)
            {
                if (m_ParentTabControl.Items.Count == count)
                {
                    if (start == end)
                    {
                        double endOffset1 = m_scrollInfo.Offset + (inc * (scrollWidth + boundaryWidth));
                        if (TabControlExt.GetIsEditing(item))
                        {
                            m_scrollInfo.Offset += inc * (scrollWidth + boundaryWidth);
                            InvalidateArrange();
                        }
                        return endOffset1;
                    }
                    else
                    {
                        if (start > -1)
                        {

                            for (int i = start; i < end; ++i)
                            {
                                if (i < m_ParentTabControl.Items.Count)
                                {
                                    scrollWidth += ((TabItemExt)m_ParentTabControl.ItemContainerGenerator.ContainerFromItem(TabItems[i])).ActualWidth;
                                }
                            }
                        }

                        double endOffset2 = m_scrollInfo.Offset + (inc * (scrollWidth + boundaryWidth));

                        if (TabControlExt.GetIsEditing(item))
                        {
                            m_scrollInfo.Offset += inc * (scrollWidth + boundaryWidth);
                            InvalidateArrange();
                        }

                        return endOffset2;
                       
                    }
                }
                else
                {
                    count = 0;
                    foreach (UIElement element in InternalChildren)
                    {
                        double tmp = 0.0;
                        Size desiredSize = GetDesiredSize(element);
                        tmp = desiredSize.Width;
                        if (m_scrollInfo.Offset != 0.0)
                        {
                            scrolledOffset = m_scrollInfo.Offset;
                        }

                        if (tmp != 0.0)
                        {
                            scrollWidth += tmp;
                        }

                        count++;
                    }

                    if (m_scrollInfo.Offset == 0.0)
                    {
                        m_scrollInfo.Offset = scrolledOffset - scrollWidth;
                    }

                    if (m_scrollInfo.Offset == (-scrollWidth))
                    {
                        m_scrollInfo.Offset = 0.0;
                    }
                }
            }
            else
            {
                if (start > -1)
                {

                    for (int i = start; i < end; ++i)
                    {
                        if (i < m_ParentTabControl.Items.Count)
                        {
                            scrollWidth += ((TabItemExt)m_ParentTabControl.ItemContainerGenerator.ContainerFromItem(TabItems[i])).ActualWidth;
                        }
                    }
                }
            }
        

            double endOffset = m_scrollInfo.Offset + (inc * (scrollWidth + boundaryWidth));

            if (TabControlExt.GetIsEditing(item))
            {
                m_scrollInfo.Offset += inc * (scrollWidth + boundaryWidth);
                InvalidateArrange();
            }

            return endOffset;
        }

        /// <summary>
        /// Validates the scrolling panel.
        /// </summary>
        internal void ValidateScrollingPanel()
        {
            if (m_ParentTabControl != null)
            {
                //m_scrollingPanel.CoerceValue(FrameworkElement.FlowDirectionProperty);
                // m_scrollingPanel1.CoerceValue(FrameworkElement.FlowDirectionProperty);
                //tab.CoerceValue(FrameworkElement.FlowDirectionProperty);
                SelectItemInternal(m_ParentTabControl.SelectedItem as TabItemExt);
            }
        }

        /// <summary>
        /// Complete editing process on the specifies TabItemExt.
        /// </summary>
        /// <param name="editableItem">TabItemExt which is editing in the current moment.</param>
        /// <param name="applyChanges">Specifies whether editing changes should be applied or no.</param>
        internal void CompleteHeaderEditInternal(TabItemExt editableItem, bool applyChanges)
        {
            if (!TabControlExt.GetUseCustomEditableTemplate(editableItem)
                && m_editableHeader != null)
            {
                if (applyChanges)
                {
                    UpdateBinding(m_editableHeader);
                }

                RemoveDelegates(m_editableHeader);
                m_editableHeader = null;
            }
            else if (!applyChanges)
            {
                editableItem.Header = editableItem.Tag;
                editableItem.Tag = null;
            }

            RemoveDelegates(editableItem);
            FocusManager.SetIsFocusScope(editableItem, false);
            TabControlExt.SetIsEditing(editableItem, false);
            m_editingItem = null;
            m_ParentTabControl.FireAfterLabelEdit(editableItem);
        }

        /// <summary>
        /// Launch editing process on the specified TabItemExt.
        /// </summary>
        /// <param name="item">TabItemExt which should be edited.</param>
        internal void LabelEditStartInternal(TabItemExt item)
        {
            if (item != null)
            {
                m_ParentTabControl.FireBeforeLabelEdit(item);
                TabControlExt.SetIsEditing(item, true);
                item.UpdateLayout();
                m_editingItem = item;
                item.Focus();
                FocusManager.SetIsFocusScope(item, true);

                foreach (ContentPresenter cPresenter in VisualUtils.EnumChildrenOfType(item, typeof(ContentPresenter)))
                {
                    if (C_CONTENT_NAME == cPresenter.Name)
                    {
                        PrepareEditableHeaderEx(item, cPresenter);
                        break;
                    }
                }

                SelectItemInternal(item);
            }
        }

        /// <summary>
        /// Prepares the editable header ex.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <param name="presenter">Value of the presenter.</param>
        private void PrepareEditableHeaderEx(HeaderedContentControl item, Visual presenter)
        {
            if (null != presenter)
            {
                item.LostFocus += new RoutedEventHandler(OnTabItem_LostFocus);
                item.KeyDown += new KeyEventHandler(OnTabItemKeyDown);

                if (!TabControlExt.GetUseCustomEditableTemplate(item))
                {
                    foreach (TextBox textBox in VisualUtils.EnumChildrenOfType(presenter, typeof(TextBox)))
                    {
                        if (C_EDITABLE_HEADER_NAME == textBox.Name)
                        {
                            PrepareEditableHeader(item, textBox);
                            break;
                        }
                    }
                }
                else
                {
                    item.Tag = item.Header;
                }
            }
        }

        /// <summary>
        /// Prepares the editable header.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <param name="editableHeader">The editable header.</param>
        private void PrepareEditableHeader(DependencyObject item, TextBox editableHeader)
        {
            m_editableHeader = editableHeader;
            m_editableHeader.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditableTexBox_LostKeyboardFocus);
            m_editableHeader.KeyDown += new KeyEventHandler(OnTabItemKeyDown);
            FocusManager.SetFocusedElement(item, m_editableHeader);
            m_editableHeader.SelectAll();
        }

        /// <summary>
        /// Returns row number that contains selected tab. 
        /// </summary>
        /// <param name="rowDistribution">Tabs distribution in rows.</param>
        /// <returns>Row number</returns>
        private int GetActiveRow(ICollection<List<int>> rowDistribution)
        {
            if (rowDistribution.Count > 1)
            {
                int rowNumber = 0;

                foreach (List<int> row in rowDistribution)
                {
                    foreach (int tabIndex in row)
                    {
                        UIElement element = InternalChildren[tabIndex];

                        if ((bool)element.GetValue(Selector.IsSelectedProperty))
                        {
                            return rowNumber;
                        }
                    }

                    rowNumber++;
                }
            }

            return 0;
        }

        /// <summary>
        /// Calculates tab distribution for each row.
        /// </summary>
        /// <param name="arrangeWidth">Available width</param>
        /// <returns>List of tabs index for each row.</returns>
        private List<List<int>> CalculateRowDistribution(double arrangeWidth)
        {
            double[] headersSize = GetHeadersSize();
            List<List<int>> rowDistribution = new List<List<int>>(m_NumRows);

            for (int i = 0; i < m_NumRows; i++)
            {
                rowDistribution.Add(new List<int>());
            }

            int rowNumber = 0;
            Point startPoint = new Point(0, 0);

            for (int i = 0, cnt = headersSize.Length; i < cnt; ++i)
            {
                double width = headersSize[i];

                if (startPoint.X + width <= arrangeWidth)
                {
                    startPoint.X += width;
                    rowDistribution[rowNumber].Add(i);
                }
                else
                {
                    rowNumber++;
                    startPoint = new Point(0, m_RowHeight * rowNumber);
                    rowDistribution[rowNumber].Add(i);
                    startPoint.X += width;
                }
            }

            if (KeepTabInFront)
            {
                int frontRowIndex = GetActiveRow(rowDistribution);
                List<int> frontRow = rowDistribution[frontRowIndex];
                rowDistribution[frontRowIndex] = rowDistribution[0];
                rowDistribution[0] = frontRow;
            }

            return rowDistribution;
        }

        /// <summary>
        /// Calculates width scaling for all tabs in specified row, when TabControl layout is switched to MultiLineFullWidth.
        /// </summary>
        /// <param name="arrangeWidth">Available width.</param>
        /// <param name="rowDistribution">Tabs distribution in rows.</param>
        /// <returns>return an array of double value</returns>
        private double[] CalculateHeaderScaling(double arrangeWidth, IEnumerable<List<int>> rowDistribution)
        {
            double[] scalingSizes = new double[m_NumRows];
            double[] headersSize = GetHeadersSize();
            double rowSum = 0.0;
            int tabsInRow = 0, currentRow = 0;

            foreach (List<int> row in rowDistribution)
            {
                foreach (int tabIndex in row)
                {
                    double width = headersSize[tabIndex];

                    if (width == 0)
                    {
                        continue;
                    }

                    rowSum += width;
                    tabsInRow++;
                }

                scalingSizes[currentRow] = (arrangeWidth - rowSum) / tabsInRow;
                currentRow++;
                rowSum = 0;
                tabsInRow = 0;
            }

            return scalingSizes;
        }

        /// <summary>
        /// Returns an array with headers size of each tab.
        /// </summary>
        /// <returns>returns an array of double value</returns>
        internal double[] GetHeadersSize()
        {
            double[] numArray = new double[InternalChildren.Count];
            int index = 0;

            foreach (UIElement element in InternalChildren)
            {
                if (element != null)
                {
                    Size desiredSize = GetDesiredSize(element);
                    numArray[index] = element.Visibility == Visibility.Collapsed ? 0.0 : desiredSize.Width;
                    index++;
                }
            }

            return numArray;
        }
        double m_rowsizewidth = 0;
        /// <summary>
        /// Positions tabs in MultiLine mode. 
        /// </summary>
        /// <param name="arrangeSize">Value of the arrangeSize</param>
        private void ArrangeMultiLine(Size arrangeSize)
        {
            if (arrangeSize.Width > 0 && InternalChildren.Count>0)
            {
                int rowNumber = 0;
                Point startPoint;
                double[] scalingSizes = new double[m_NumRows];
                double[] headersSize = GetHeadersSize();
                List<List<int>> rowDistribution = CalculateRowDistribution(arrangeSize.Width);
                m_ParentTabControl.VerifyZIndex();

                if (TabItemLayout == TabItemLayoutType.MultiLineWithFullWidth)
                {
                    scalingSizes = CalculateHeaderScaling(arrangeSize.Width, rowDistribution);
                }

                // VS2008 depended code
                bool isProperStyle = SkinStorage.GetVisualStyle(this) == "Default" || SkinStorage.GetVisualStyle(this) == "OneNote";
                Dock tabPlacement = TabStripPlacement;
                m_rowsizewidth = 0;
                if (rowDistribution.Count > 0 && rowDistribution[rowDistribution.Count - 1].Count > 0)
                {
                    foreach (int row in rowDistribution[rowDistribution.Count - 1])
                    {
                        m_rowsizewidth += InternalChildren[row].DesiredSize.Width;
                    }
                }
                foreach (List<int> row in rowDistribution)
                {
                    startPoint = new Point(0, m_RowHeight * (m_NumRows - rowNumber - 1));

                    if (isProperStyle)
                    {
                        UIElement fElement = InternalChildren[0];

                        if (Dock.Top == tabPlacement || Dock.Right == tabPlacement)
                        {
                            Panel.SetZIndex(fElement, 9999);
                        }

                        UIElement eElement = InternalChildren[row.Count - 1];

                        if (Dock.Bottom == tabPlacement || Dock.Left == tabPlacement)
                        {
                            Panel.SetZIndex(eElement, 9999);
                        }
                    }

                    foreach (int tabIndex in row)
                    {
                        UIElement element = InternalChildren[tabIndex];

                        if (element.Visibility != Visibility.Visible)
                        {
                            continue;
                        }

                        double width = headersSize[tabIndex];
                        element.Arrange(new Rect(startPoint.X, startPoint.Y, width + scalingSizes[rowNumber] + TAB_INTERSECTION_FACTOR, m_RowHeight));
                        startPoint.X += width + scalingSizes[rowNumber];
                    }

                    rowNumber++;
                }
            }
            else
            {
                foreach (UIElement element in InternalChildren)
                {
                    element.Arrange(new Rect(0, 0, 0, 0));
                }
            }
        }

        public bool CheckToUpdatePosition(Size arrangesize,TabItemExt tabitem)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = GetHeadersSize();
            //double availiableWidth = arrangesize.Width - ScrollingPanel.DesiredSize.Width;
            double availiableWidth = arrangesize.Width;
            startPoint.X = m_scrollInfo.Offset;
            

            foreach (UIElement element in InternalChildren)
            {
                width = headersSize[index];
                if (element != null)
                {
                    if (startPoint.X + width <= availiableWidth)
                    {
                        if (startPoint.X + width > 0 && startPoint.X < 0)
                        {
                            if ((element as TabItemExt) == tabitem && Math.Abs(width - (availiableWidth - startPoint.X)) > 0.01)
                                return false;
                        }
                        index++;
                        startPoint.X += width;
                    }
                    else
                    {
                        if ((element as TabItemExt) == tabitem)
                            return false;
                    }
                }
            }
            return true;
        }

        internal void CheckScrollBehavior(ScrollViewer sender, Size panelSize)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = GetHeadersSize();
            double availiableWidth = sender.ViewportWidth + sender.HorizontalOffset;

            foreach (UIElement element in InternalChildren)
            {
                width = headersSize[index];
                if (element != null)
                {
                    if (sender.HorizontalOffset == 0)
                    {
                        m_scrollInfo.FirstTabTrimmedWidth = 0;
                        m_scrollInfo.FirstTrimmedTabIndex = index;
                        break;
                    }
                    else if (sender.HorizontalOffset > 0)
                    {
                        if (startPoint.X + width > sender.HorizontalOffset)
                        {
                            if (sender.HorizontalOffset - startPoint.X < (width / 3))
                            {
                                int addedindex = (index != 0) ? 1 : 0;
                                m_scrollInfo.FirstTabTrimmedWidth = (addedindex == 0) ?
                                    sender.HorizontalOffset - startPoint.X :
                                    sender.HorizontalOffset - startPoint.X + headersSize[index - 1];
                            }
                            else
                            {
                                m_scrollInfo.FirstTabTrimmedWidth = sender.HorizontalOffset - startPoint.X;
                            }
                            m_scrollInfo.FirstTrimmedTabIndex = index;
                            break;
                        }
                        else if (startPoint.X + width == sender.HorizontalOffset)
                        {
                            m_scrollInfo.FirstTabTrimmedWidth = width;
                            m_scrollInfo.FirstTrimmedTabIndex = index;
                            break;
                        }
                    }
                    startPoint.X += width;
                    index++;
                }
            }
            startPoint = new Point(0, 0);
            index = 0;
            foreach (UIElement element in InternalChildren)
            {
                width = headersSize[index];
                if (element != null)
                {
                    if (startPoint.X + width >= availiableWidth)
                    {
                        if (startPoint.X + width == availiableWidth)
                        {
                            m_scrollInfo.LastTabTrimmedWidth = width;
                            m_scrollInfo.LastTrimmedTabIndex = index;
                            break;
                        }
                        else
                        {
                            if ((startPoint.X + width) - availiableWidth < (width / 3))
                            {
                                int addedindex = (index < headersSize.Length - 1) ? 1 : 0;
                                m_scrollInfo.LastTabTrimmedWidth = (addedindex == 0) ?
                                    (startPoint.X + width) - availiableWidth :
                                    ((startPoint.X + width) - availiableWidth) + headersSize[index + 1];
                            }
                            else
                            {
                                m_scrollInfo.LastTabTrimmedWidth = (startPoint.X + width) - availiableWidth;
                            }
                            m_scrollInfo.LastTrimmedTabIndex = index;
                            break;
                        }
                    }
                    startPoint.X += width;
                    index++;
                }
            }
            if (!m_IsScrollButtonClicked)
                ScrollToSelectedItem(sender, panelSize);
        }

        internal void ScrollToSelectedItem(ScrollViewer sender, Size panelSize)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = GetHeadersSize();
            double availiableWidth = sender.ViewportWidth + sender.HorizontalOffset;

            foreach (UIElement element in InternalChildren)
            {
                width = headersSize[index];
                if (element != null)
                {
                    if (startPoint.X + width > availiableWidth)
                    {
                        if ((element as TabItemExt).IsSelected && (element as TabItemExt) == m_ScrollToSelectedItem)
                        {
                            sender.ScrollToHorizontalOffset(sender.HorizontalOffset + ((startPoint.X + width) - availiableWidth));
                            break;
                        }
                    }
                    startPoint.X += width;
                    index++;
                }
            }
        }
        
        /// <summary>
        /// Positions tabs in SingleLine mode. 
        /// </summary>
        /// <param name="availiableWidth">The final area within the TabControlExt that this element should use to arrange itself and its children.</param>
        private void ArrangeSingleLine(double availiableWidth)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = GetHeadersSize();


            if (TabItemSize == TabItemSizeMode.ShrinkToFit && availiableWidth < m_scrollInfo.DesiredWidth)
            {
                width = availiableWidth / VisibleItemsCount;
                if (width < 0)
                {
                    width *= -1;
                }

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    element.Arrange(new Rect(startPoint.X, startPoint.Y, width + TAB_INTERSECTION_FACTOR, m_RowHeight));
                    startPoint.X += width;
                }
            }
            else
            {
                foreach (UIElement element in InternalChildren)
                {
                    width = headersSize[index];
                    if (element != null)
                    {
                        element.Arrange(new Rect(startPoint.X, startPoint.Y, width + TAB_INTERSECTION_FACTOR,
                                                     m_RowHeight));
                        startPoint.X += width;

                        index++;
                    }
                }
            }

        }



        /// <summary>
        /// Positions Tabs in Single Line mode for Excel Like TabControl Style
        /// </summary>
        /// <param name="availiableWidth"></param>
        /// <remarks></remarks>
        private void ArrangeSingleLine(double availiableWidth,double initialPoint)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(initialPoint, 0);
            double[] headersSize = GetHeadersSize();
            m_scrollInfo.LastTrimmedTabIndex = InternalChildren.Count+1;
            m_scrollInfo.FirstTrimmedTabIndex = -1;
           
            //if (m_scrollInfo.Offset > 0)
            //{
            //    m_scrollInfo.Offset = 0;
            //}
            //if ((availiableWidth - ScrollingPanel.DesiredSize.Width) < m_scrollInfo.DesiredWidth)
            if (availiableWidth < m_scrollInfo.DesiredWidth)
            {
                //if (Math.Abs(m_scrollInfo.Offset) + (availiableWidth - ScrollingPanel.DesiredSize.Width) > m_scrollInfo.DesiredWidth)
                if (Math.Abs(m_scrollInfo.Offset) + availiableWidth  > m_scrollInfo.DesiredWidth)
                {
                    m_scrollInfo.Offset += Math.Abs(m_scrollInfo.Offset) + availiableWidth - m_scrollInfo.DesiredWidth;
                    //m_scrollingPanel.DisableNextPart();
                    TabPanelAdv.DisableNextPart();
                }
                else
                {
                    //m_scrollingPanel.EnableNextPart();
                    TabPanelAdv.EnableNextPart();
                }
            }
            else
            {
                m_scrollInfo.Offset = 0;
            }

            ChangePrevPart();

            if (TabItemSize == TabItemSizeMode.ShrinkToFit && availiableWidth < m_scrollInfo.DesiredWidth)
            {
                width = availiableWidth / VisibleItemsCount;
                if (width < 0)
                {
                    width *= -1;
                }

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    element.Arrange(new Rect(startPoint.X, startPoint.Y, width + TAB_INTERSECTION_FACTOR, m_RowHeight));
                    startPoint.X += width;
                }
            }
            else
            {
                startPoint.X = initialPoint;

                foreach (UIElement element in InternalChildren)
                {
                    width = headersSize[index];
                    if (element != null)
                    {
                        if (startPoint.X + width < availiableWidth)
                        {
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, width + TAB_INTERSECTION_FACTOR,
                                                     m_RowHeight));

                            if ((startPoint.X < 0 && ((startPoint.X + width) >= 0)) ||
                                Math.Abs(startPoint.X + width) < 0.01)
                            {
                                m_scrollInfo.FirstTabTrimmedWidth = Math.Abs(startPoint.X);
                                m_scrollInfo.FirstTrimmedTabIndex = index;

                                if (m_scrollInfo.FirstTabTrimmedWidth < 0.01)
                                {
                                    m_scrollInfo.FirstTabTrimmedWidth = width;
                                }
                            }

                            startPoint.X += width;

                            if (Math.Abs(startPoint.X) < 0.01 && Math.Abs(startPoint.X) > 0)
                            {
                                startPoint.X = 0;
                            }
                        }
                        else if (startPoint.X < availiableWidth)
                        {
                            int addedHeight = ((TabItemExt)element).IsSelected ? 1 : 0;
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, availiableWidth - startPoint.X,
                                                     m_RowHeight + addedHeight));

                            if (Math.Abs(width - (availiableWidth - startPoint.X)) > 0.01)
                            {
                                m_scrollInfo.LastTabTrimmedWidth = width - (availiableWidth - startPoint.X);
                                m_scrollInfo.LastTrimmedTabIndex = index;
                            }
                            else
                            {
                                m_scrollInfo.LastTabTrimmedWidth = 0;
                            }

                            startPoint.X += availiableWidth - startPoint.X;
                        }
                        else
                        {
                            if (m_scrollInfo.LastTabTrimmedWidth == 0)
                            {
                                m_scrollInfo.LastTabTrimmedWidth = width;
                                m_scrollInfo.LastTrimmedTabIndex = index;
                            }

                            element.Arrange(new Rect(startPoint.X, startPoint.Y, 0, 0));
                        }

                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Arranges the single line X.
        /// </summary>
        /// <param name="availiableWidth">Width of the availiable.</param>
        /// <param name="taboff">The taboff.</param>
        private void ArrangeSingleLineX(double availiableWidth, double taboff)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = GetHeadersSize();
            m_scrollInfo.LastTrimmedTabIndex = InternalChildren.Count + 1;
            m_scrollInfo.FirstTrimmedTabIndex = -1;

            if (m_scrollInfo.Offset > 0)
            {
                m_scrollInfo.Offset = 0;
            }

            if (availiableWidth < m_scrollInfo.DesiredWidth)
            {
                if (Math.Abs(m_scrollInfo.Offset) + availiableWidth > m_scrollInfo.DesiredWidth)
                {
                    m_scrollInfo.Offset += Math.Abs(m_scrollInfo.Offset) + availiableWidth - m_scrollInfo.DesiredWidth;
                    //m_scrollingPanel.DisableNextPart();
                    TabPanelAdv.DisableNextPart();
                }
                else
                {
                    //m_scrollingPanel.EnableNextPart();
                    TabPanelAdv.EnableNextPart();
                }
            }
            else
            {
                m_scrollInfo.Offset = 0;
            }

            ChangePrevPart();

            if (TabItemSize == TabItemSizeMode.ShrinkToFit && availiableWidth < m_scrollInfo.DesiredWidth)
            {
                width = availiableWidth / VisibleItemsCount;

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    element.Arrange(new Rect(startPoint.X + taboff, startPoint.Y, width + TAB_INTERSECTION_FACTOR, m_RowHeight));
                    startPoint.X += width;
                }
            }
            else
            {
                startPoint.X = m_scrollInfo.Offset;

                foreach (UIElement element in InternalChildren)
                {
                    width = headersSize[index];

                    if (startPoint.X + width < availiableWidth)
                    {
                        element.Arrange(new Rect(startPoint.X + taboff, startPoint.Y, width + TAB_INTERSECTION_FACTOR, m_RowHeight));

                        if ((startPoint.X < 0 && ((startPoint.X + width) >= 0)) || Math.Abs(startPoint.X + width) < 0.01)
                        {
                            m_scrollInfo.FirstTabTrimmedWidth = Math.Abs(startPoint.X);
                            m_scrollInfo.FirstTrimmedTabIndex = index;

                            if (m_scrollInfo.FirstTabTrimmedWidth < 0.01)
                            {
                                m_scrollInfo.FirstTabTrimmedWidth = width;
                            }
                        }

                        startPoint.X += width;

                        if (Math.Abs(startPoint.X) < 0.01 && Math.Abs(startPoint.X) > 0)
                        {
                            startPoint.X = 0;
                        }
                    }
                    else if (startPoint.X < availiableWidth)
                    {
                        int addedHeight = ((TabItemExt)element).IsSelected ? 1 : 0;
                        element.Arrange(new Rect(startPoint.X + taboff, startPoint.Y, availiableWidth - startPoint.X, m_RowHeight + addedHeight));

                        if (Math.Abs(width - (availiableWidth - startPoint.X)) > 0.01)
                        {
                            m_scrollInfo.LastTabTrimmedWidth = width - (availiableWidth - startPoint.X);
                            m_scrollInfo.LastTrimmedTabIndex = index;
                        }
                        else
                        {
                            m_scrollInfo.LastTabTrimmedWidth = 0;
                        }

                        startPoint.X += availiableWidth - startPoint.X;
                    }
                    else
                    {
                        if (m_scrollInfo.LastTabTrimmedWidth == 0)
                        {
                            m_scrollInfo.LastTabTrimmedWidth = width;
                            m_scrollInfo.LastTrimmedTabIndex = index;
                        }

                        element.Arrange(new Rect(startPoint.X + taboff, startPoint.Y, 0, 0));
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Changes the prev part.
        /// </summary>
        private void ChangePrevPart()
        {
            if (m_scrollInfo.Offset == 0)
            {
                //m_scrollingPanel.DisablePrevPart();
                TabPanelAdv.DisablePrevPart();
            }
            else
            {
                //m_scrollingPanel.EnablePrevPart();
                TabPanelAdv.EnablePrevPart();
            }
        }

        /// <summary>
        /// Positions tabs in SingleLine mode. 
        /// </summary>
        /// <param name="arrangeSize">The final area within the TabControlExt that this element should use to arrange itself and its children.</param>
        internal void ArrangeElements(Size arrangeSize)
        {
            if (tab == null)
                return;

            double REDU_FACTOR = 5;
            //double availiableWidth = arrangeSize.Width - ScrollingPanel.DesiredSize.Width;
            double availiableWidth = arrangeSize.Width;
            double availiableWidthNew = availiableWidth - tab.DesiredSize.Width - 5;
            m_scrollInfo.PageWidth = availiableWidth;
            m_scrollInfo.AllTrimmedWidth = m_scrollInfo.DesiredWidth > availiableWidth
                ? m_scrollInfo.DesiredWidth - availiableWidth : 0;

            double childswidth = 0;
            childswidth = ChildWidths();

            if (TabItemLayout == TabItemLayoutType.SingleLine)
            {

                if (TabItemSize != TabItemSizeMode.ShrinkToFit)
                {
                    if (availiableWidth > childswidth)
                    {
                        if (this.DesiredSize.Width > childswidth + tab.DesiredSize.Width + REDU_FACTOR)
                        {
                            ArrangeSingleLine(availiableWidth);
                        }
                        else
                        {
                            availiableWidth = availiableWidth - tab.DesiredSize.Width - REDU_FACTOR;
                            ArrangeSingleLine(availiableWidth);
                        }
                    }
                    else
                    {
                        ArrangeSingleLine(availiableWidthNew);
                    }
                }
                else if (availiableWidth - 50 < m_scrollInfo.DesiredWidth)
                {
                    ArrangeSingleLine(availiableWidthNew);
                    tab.Arrange(new Rect(arrangeSize.Width - tab.DesiredSize.Width, 0, tab.DesiredSize.Width, m_RowHeight));
                    return;
                }
                else
                {
                    ArrangeSingleLine(availiableWidth);
                    tab.Arrange(new Rect(childswidth + REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                    return;
                }

            }
            else
            {

                if (childswidth + tab.DesiredSize.Width > arrangeSize.Width && m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.Last)
                {
                    Size temparragesize = arrangeSize;
                    temparragesize.Width -= tab.DesiredSize.Width;

                    ArrangeMultiLine(temparragesize);
                    if (m_rowsizewidth <= 0)
                    {
                        tab.Arrange(new Rect(temparragesize.Width + REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                    }
                    else
                    {
                        tab.Arrange(new Rect(m_rowsizewidth, 0, tab.DesiredSize.Width, m_RowHeight));
                    }
                }
                else
                {

                    if (m_ParentTabControl.IsNewButtonEnabled)
                    {
                        if (m_ParentTabControl.TabItemLayout == TabItemLayoutType.MultiLine)
                        {
                            ArrangeMultiLine(arrangeSize);
                            tab.Arrange(new Rect(childswidth + REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                        }
                        else if (m_ParentTabControl.TabItemLayout == TabItemLayoutType.MultiLineWithFullWidth)
                        {
                            Size tempsize = arrangeSize;
                            tempsize.Width -= tab.DesiredSize.Width;
                            ArrangeMultiLine(tempsize);
                            tab.Arrange(new Rect(tempsize.Width, 0, tab.DesiredSize.Width, m_RowHeight));
                        }
                    }
                    else
                    {
                        ArrangeMultiLine(arrangeSize);
                    }
                }
                return;
            }

            if (m_ParentTabControl.NewButtonStyle != null)
            {
                tab.Style = m_ParentTabControl.NewButtonStyle;
            }

            //childswidth *= 0.75;
            if (availiableWidth > childswidth)
            {
                childswidth += REDU_FACTOR;
                if (this.DesiredSize.Width < childswidth + tab.DesiredSize.Width)
                {
                    childswidth = 2 * (childswidth + REDU_FACTOR) - this.DesiredSize.Width;
                    tab.Arrange(new Rect(childswidth, 0, tab.DesiredSize.Width, m_RowHeight));
                }
                else
                {
                    m_RowHeight = m_RowHeight == 0 ? 20 : m_RowHeight;
                    tab.Arrange(new Rect(childswidth, 0, tab.DesiredSize.Width, m_RowHeight));
                }
                //ScrollingPanel.Arrange(new Rect(availiableWidth, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));
            }
            else
            {
                if (this.DesiredSize.Width > childswidth + tab.DesiredSize.Width + REDU_FACTOR)
                {
                    tab.Arrange(new Rect(availiableWidth - tab.DesiredSize.Width - REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                    //ScrollingPanel.Arrange(new Rect(this.DesiredSize.Width - ScrollingPanel.DesiredSize.Width - REDU_FACTOR, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));
                }
                else
                {
                    //if (!ScrollingPanel.Showing)
                    //{
                        tab.Arrange(new Rect(availiableWidth - REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                    //}
                    //else
                    //{
                    //    tab.Arrange(new Rect(availiableWidth - tab.DesiredSize.Width - REDU_FACTOR, 0, tab.DesiredSize.Width, m_RowHeight));
                    //}
                    //ScrollingPanel.Arrange(new Rect(this.DesiredSize.Width - REDU_FACTOR - ScrollingPanel.DesiredSize.Width, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));
                }
            }


            //double availwidth = availiableWidth - tab.DesiredSize.Width;
            //tab.Arrange(new Rect(availiableWidth, 0, tab.DesiredSize.Width, m_RowHeight));
        }

        /// <summary>
        /// Arranges the elements X.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        //private void ArrangeElementsX(Size arrangeSize)
        //{
        //    //double availiableWidth = arrangeSize.Width - ScrollingPanel.DesiredSize.Width - tab.DesiredSize.Width;
        //    double availiableWidth = arrangeSize.Width - tab.DesiredSize.Width;
        //    m_scrollInfo.PageWidth = availiableWidth;
        //    m_scrollInfo.AllTrimmedWidth = m_scrollInfo.DesiredWidth > availiableWidth
        //        ? m_scrollInfo.DesiredWidth - availiableWidth : 0;

        //    if (TabItemLayout == TabItemLayoutType.SingleLine)
        //    {
        //        ArrangeSingleLineX(availiableWidth, tab.DesiredSize.Width);
        //    }
        //    else
        //    {
        //        ArrangeMultiLine(arrangeSize);
        //    }
        //    tab.Arrange(new Rect(0, 0, tab.DesiredSize.Width, m_RowHeight));
        //    //ScrollingPanel.Arrange(new Rect(availiableWidth + tab.DesiredSize.Width, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));
        //}

        /// <summary>
        /// Arranges the elements1.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        internal void ArrangeElements1(Size arrangeSize)
        {
            if (m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
            {
              
                //double availiableWidth = arrangeSize.Width - ScrollingPanel.DesiredSize.Width;
                double availiableWidth = arrangeSize.Width;
                m_scrollInfo.PageWidth = availiableWidth;
                m_scrollInfo.AllTrimmedWidth = m_scrollInfo.DesiredWidth > availiableWidth
                    ? m_scrollInfo.DesiredWidth - availiableWidth : 0;                              

                if (TabItemLayout == TabItemLayoutType.SingleLine)
                {                   
                    ArrangeSingleLine(availiableWidth);
                }
                else
                {
                    ArrangeMultiLine(arrangeSize);
                }

                //ScrollingPanel.Arrange(new Rect(availiableWidth, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));
            }
            else /* Its for Excel Like TabControl Style */
            {
                double availiableWidth = arrangeSize.Width;               
                m_scrollInfo.PageWidth = availiableWidth;
                m_scrollInfo.AllTrimmedWidth = m_scrollInfo.DesiredWidth > availiableWidth
                    ? m_scrollInfo.DesiredWidth - availiableWidth : 0;

                //m_scrollInfo.AllTrimmedWidth = availiableWidth > m_scrollInfo.DesiredWidth ? availiableWidth - m_scrollInfo.DesiredWidth : 0;
               
                
                //ScrollingPanel.Arrange(new Rect(-4, 0, ScrollingPanel.DesiredSize.Width, m_RowHeight));

                //if (m_scrollInfo.Offset == 0 || m_scrollInfo.Offset > ScrollingPanel.DesiredSize.Width)
                //{
                //    m_scrollInfo.Offset = ScrollingPanel.DesiredSize.Width;
                //}
                //else if (ScrollingPanel.DesiredSize.Width == 1)
                //{
                //    m_scrollInfo.Offset = 0.0;
                //}

                if (TabItemLayout == TabItemLayoutType.SingleLine)
                {                   
                   ArrangeSingleLine(availiableWidth, m_scrollInfo.Offset);                                     
                }               
            }

           
        }

        /// <summary>
        /// Childs the widths.
        /// </summary>
        /// <returns></returns>
        private double ChildWidths()
        {
            double childswidth = 0;
            foreach (TabItemExt item in Children)
            {
                childswidth += item.DesiredSize.Width;
            }
            return childswidth;
        }
        /// <summary>
        /// Measure all elements.
        /// </summary>
        /// <param name="availableSize">Value of the availableSize</param>
        /// <returns>returns a double value</returns>
        internal double MeasureElements(Size availableSize)
        {
            Size size;
            int index = 0;
            double totalWidth = 0d;
            m_NumRows = 1;
            foreach (UIElement element in InternalChildren)
            {
                if (element != null)
                {
                    if (element.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    element.Measure(availableSize);
                    size = GetDesiredSize(element);

                    if (m_RowHeight < size.Height)
                    {
                        m_RowHeight = size.Height;
                    }

                    if (TabItemLayout == TabItemLayoutType.SingleLine)
                    {
                        totalWidth += size.Width;
                    }
                    else
                    {
                        if (m_ParentTabControl.IsNewButtonEnabled && tab != null &&
                            m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.Last)
                        {
                            if (((totalWidth + size.Width + tab.DesiredSize.Width) > availableSize.Width) && (index > 0))
                            {
                                totalWidth = size.Width;
                                index = 1;
                                m_NumRows++;
                                continue;
                            }
                        }
                        else
                        {
                            if (((totalWidth + size.Width) > availableSize.Width) && (index > 0))
                            {
                                totalWidth = size.Width;
                                index = 1;
                                m_NumRows++;
                                continue;
                            }
                        }

                        totalWidth += size.Width;
                        index++;
                    }
                }
            }

            m_scrollInfo.DesiredWidth = totalWidth;
            return totalWidth;
        }

        /// <summary>
        /// Launch scroll int to the next tab.
        /// </summary>
        private void ScrollToNextTab()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
            {
                TabPanelAdv.ChildScrollViewer.ScrollToHorizontalOffset(TabPanelAdv.ChildScrollViewer.HorizontalOffset + m_scrollInfo.LastTabTrimmedWidth);
                //if (TabPanelAdv.ChildScrollViewer.ViewportWidth + TabPanelAdv.ChildScrollViewer.HorizontalOffset + m_scrollInfo.LastTabTrimmedWidth >= TabPanelAdv.ChildScrollViewer.ExtentWidth)
                //    TabPanelAdv.DisableNextPart();
            }
        }

        /// <summary>
        /// Launch scroll int to the previous tab.
        /// </summary>
        private void ScrollToPrevTab()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
            {
                TabPanelAdv.ChildScrollViewer.ScrollToHorizontalOffset(TabPanelAdv.ChildScrollViewer.HorizontalOffset - m_scrollInfo.FirstTabTrimmedWidth);
            }
        }

        /// <summary>
        /// Launch scroll int to the first tab.
        /// </summary>
        private void ScrollToFirstTab()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
                TabPanelAdv.ChildScrollViewer.ScrollToLeftEnd();
        }

        /// <summary>
        /// Launch scroll int to the last tab.
        /// </summary>
        internal void ScrollToLastTab()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
            {
                TabPanelAdv.ChildScrollViewer.ScrollToRightEnd();
            }
        }

        /// <summary>
        /// Scrolls to last tab1.
        /// </summary>
        internal void ScrollToLastTab1()
        {
            StartScrolling(m_scrollInfo.Offset, -m_scrollInfo.AllTrimmedWidth);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }



        /// <summary>
        /// Launch scroll int to the next page.
        /// </summary>
        private void ScrollToNextPage()
        {
            if (TabPanelAdv.ChildScrollViewer != null)
                TabPanelAdv.ChildScrollViewer.ScrollToHorizontalOffset(TabPanelAdv.PageScrollWidth + TabPanelAdv.ChildScrollViewer.HorizontalOffset);
        }

        /// <summary>
        /// Removes delegates on the specified target.
        /// </summary>
        /// <param name="target">Delegates owner.</param>
        private void RemoveDelegates(Control target)
        {
            if (target is TextBox)
            {
                TextBox textBox = (TextBox)target;
                textBox.LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(OnEditableTexBox_LostKeyboardFocus);
                textBox.KeyDown -= new KeyEventHandler(OnTabItemKeyDown);
            }

            if (target is TabItemExt)
            {
                TabItemExt item = (TabItemExt)target;
                item.LostFocus -= new RoutedEventHandler(OnTabItem_LostFocus);
                item.KeyDown -= new KeyEventHandler(OnTabItemKeyDown);
            }
        }

        /// <summary>
        /// Calculates the height of the max row.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void CalculateMaxRowHeight(Size availableSize)
        {
            m_RowHeight = 0;
            if (m_ParentTabControl != null)
            {
                m_ParentTabControl.m_SelectionFlag = false;
            }
            foreach (UIElement element in InternalChildren)
            {
                if (element != null)
                {
                    if (element.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    if (m_NumRows > 1 || (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
                        || m_ParentTabControl.RotateTextWhenVertical == false
                        || ((TabStripPlacement == Dock.Left || TabStripPlacement == Dock.Right)
                        && m_ParentTabControl.RotateTextWhenVertical == true))
                    {
                        element.Measure(availableSize);
                    }

                    double currentHeight = GetDesiredSize(element).Height;

                    if (m_RowHeight < currentHeight)
                    {
                        m_RowHeight = currentHeight;
                    }
                }
            }
            if (VisibleItemsCount == 0 && m_ParentTabControl != null && m_ParentTabControl.IsNewButtonEnabled && m_RowHeight == 0 && tab != null)
            {
                if (m_NumRows > 1 || (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Bottom)
                       || m_ParentTabControl.RotateTextWhenVertical == false
                       || ((TabStripPlacement == Dock.Left || TabStripPlacement == Dock.Right)
                       && m_ParentTabControl.RotateTextWhenVertical == true))
                {
                    tab.Measure(availableSize);
                }

                double currentHeight = GetDesiredSize(tab).Height;

                if (m_RowHeight < currentHeight)
                {
                    m_RowHeight = currentHeight - 0.1;
                }
            }
            if (m_ParentTabControl != null)
            {
                m_ParentTabControl.m_SelectionFlag = true;
            }
        }

        /// <summary>
        /// Initializes the animation.
        /// </summary>
        private void InitializeAnimation()
        {
            m_scrollStoryboard = new Storyboard();
            m_scrollAnimation = new DoubleAnimation();
            Storyboard.SetTargetProperty(m_scrollAnimation, new PropertyPath(ScrollOffsetProperty));
            m_scrollStoryboard.Children.Add(m_scrollAnimation);
            m_scrollStoryboard.CurrentTimeInvalidated += new EventHandler(ScrollOffsetChanged);
            m_scrollStoryboard.Completed += new EventHandler(M_scrollStoryboard_Completed);
        }

        /// <summary>
        /// Starts the scrolling.
        /// </summary>
        /// <param name="from">Value of the From.</param>
        /// <param name="to">Value of the To.</param>
        internal void StartScrolling(double from, double to)
        {
            if (m_scrollStoryboard == null || m_scrollAnimation == null)
            {
                InitializeAnimation();
            }

            m_scrollAnimation.From = from;
            m_scrollAnimation.To = to;
            m_scrollAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(m_ParentTabControl.ScrollingTime));
            m_scrollStoryboard.Begin(this, true);
        }

        /// <summary>
        /// Returns desired size for specified element.
        /// </summary>
        /// <param name="element">Value of the element</param>
        /// <returns>required size</returns>
        private static Size GetDesiredSize(UIElement element)
        {
            return new Size(element.DesiredSize.Width, element.DesiredSize.Height);
        }

        /// <summary>
        /// Updates  binding explicit.
        /// </summary>
        /// <param name="target">Binding target.</param>
        private static void UpdateBinding(FrameworkElement target)
        {
            BindingExpression expression = target.GetBindingExpression(TextBox.TextProperty);
            if (expression != null)
            {
                expression.UpdateSource();
            }
        }
        #endregion

        #region Override
        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <param name="availableSize">Measurement constraints, a control cannot return a size larger than the constraint.</param>
        /// <returns>The size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double REDUCTION_FACTOR = 20;
            m_needShrink = false;

            if (m_parentTabPanel != null
                && m_parentTabPanel.Visibility != Visibility.Visible
                && VisibleItemsCount > 0)
            {
                m_parentTabPanel.Visibility = Visibility.Visible;
                m_ParentTabControl.IsAllTabsClosed = false;

            }


            CalculateMaxRowHeight(availableSize);
            double totalWidth = MeasureElements(new Size(availableSize.Width, m_RowHeight));

            double rowWidth = 0;
            if (rowWidth < totalWidth)
            {
                rowWidth = totalWidth;
            }

            m_scrollInfo.NeedScrollButtonsShow = availableSize.Width - REDUCTION_FACTOR < totalWidth;
            //m_scrollInfo.NeedScrollButtonsShow = m_scrollInfo.FirstTabTrimmedWidth != 0 || m_scrollInfo.LastTabTrimmedWidth!=0;
            m_parentTabPanel.IsAllItemsVisible = !m_scrollInfo.NeedScrollButtonsShow ||
                                                 TabItemLayout != TabItemLayoutType.SingleLine ||
                                                 TabItemSize != TabItemSizeMode.Normal;

            //switch (TabScrollButtonVisibility)
            //{
            //    case TabScrollButtonVisibility.Auto:
            //        if (!m_parentTabPanel.IsAllItemsVisible)
            //        {
            //            //ScrollingPanel.Show();
            //            TabPanelAdv.ShowScrollingButtons();
            //        }
            //        else
            //        {
            //            //ScrollingPanel.Hide();
            //            TabPanelAdv.HideScrollingButtons();
            //        }

            //        break;

            //    case TabScrollButtonVisibility.Hidden:
            //        //ScrollingPanel.Hide();
            //        TabPanelAdv.HideScrollingButtons();
            //        break;

            //    case TabScrollButtonVisibility.Visible:
            //        if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSize == TabItemSizeMode.Normal)
            //        {
            //            //ScrollingPanel.Show();
            //            TabPanelAdv.ShowScrollingButtons();
            //        }
            //        else
            //        {
            //            //ScrollingPanel.Hide();
            //            TabPanelAdv.HideScrollingButtons();
            //        }

            //        break;
            //    default:
            //        break;
            //}

            if (TabItemLayout != TabItemLayoutType.SingleLine && m_NumRows == VisibleItemsCount)
            {
                AverageWidth = availableSize.Width - VS2008_TAB_SHIFT;

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    InvalidateMeasure();
                }
            }

            if (TabItemSize == TabItemSizeMode.ShrinkToFit && totalWidth + 1.0 > availableSize.Width)
            {
                m_needShrink = true;
                AverageWidth = availableSize.Width / VisibleItemsCount;

                foreach (UIElement element in InternalChildren)
                {
                    if (element.Visibility == Visibility.Collapsed)
                    {
                        continue;
                    }

                    element.Measure(new Size(AverageWidth, m_RowHeight));
                }
            }

            Size size = new Size(availableSize.Width, m_RowHeight * m_NumRows);

            // VS2008 depended code            
            //if (TabStripPlacement == Dock.Left || TabStripPlacement == Dock.Bottom)
            //{
            //    if (m_ParentTabControl.TabVisualStyle == TabVisualStyle.None)
            //    {
            //        ScrollingPanel.Margin = new Thickness((m_RowHeight / (VS2008_TAB_SHIFT + 8)) + VS2008_TAB_SHIFT, 0, 0, 0);
            //    }
            //    else
            //    {
            //        ScrollingPanel.Margin = new Thickness(1, 0, 0, 0);
            //    }
            //}
            //else
            //{
            //    ScrollingPanel.Margin = new Thickness(1, 0, 0, 0);
            //}

            //ScrollingPanel.Measure(size);

            Size sizetab = new Size();
            sizetab.Width = 40;
            sizetab.Height = m_RowHeight * m_NumRows;

            if (m_ParentTabControl.IsNewButtonEnabled)
            {
                if (tab == null)
                    AddNewTab();
                if (m_ParentTabControl != null && m_ParentTabControl.NewButtonTemplate != null)
                {
                    tab.Template = m_ParentTabControl.NewButtonTemplate;
                }
                if (m_ParentTabControl.NewButtonStyle != null)
                {
                    if (m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.Last)
                    {
                        tab.Style = m_ParentTabControl.NewButtonStyle;
                    }
                    else if(m_parentTabPanel.tab !=null)
                    {
                        m_parentTabPanel.tab.Style = m_ParentTabControl.NewButtonStyle;
                    }
                }
                if (m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.Last)
                {
                    if (m_ParentTabControl.NewButtonTemplate == null)
                    {
                        if (m_ParentTabControl.Items.Count > 0)
                        {
                            if (m_ParentTabControl.Items[0] is TabItemExt)
                            {
                                tab.Template = (m_ParentTabControl.Items[0] as TabItemExt).Template;
                            }
                            else
                            {
                                tab.Template = (Children[0] as TabItemExt).Template;
                            }

                        }
                    }
                    else
                    {
                        tab.Template = m_ParentTabControl.NewButtonTemplate;
                    }
                    if (m_parentTabPanel.tab != null)
                    {
                        m_parentTabPanel.tab.Visibility = Visibility.Collapsed;
                    }
                    if (tab != null)
                    {
                        ApplyNewTabProperties(tab);
                    }
                }
                else
                {
                    if (m_parentTabPanel.tab != null)
                    {
                        if (m_ParentTabControl.NewButtonTemplate == null)
                        {

                            if (m_ParentTabControl.Items.Count > 0)
                            {
                                if (m_ParentTabControl.Items[0] is TabItemExt)
                                {
                                    m_parentTabPanel.tab.Template = (m_ParentTabControl.Items[0] as TabItemExt).Template;
                                }
                                else
                                {
                                    m_parentTabPanel.tab.Template = (this.Children[0] as TabItemExt).Template;
                                }
                            }
                        }
                        else
                        {
                            m_parentTabPanel.tab.Template = m_ParentTabControl.NewButtonTemplate;
                        }
                        m_parentTabPanel.tab.Visibility = Visibility.Visible;
                        m_parentTabPanel.tab.Measure(new Size(availableSize.Width, m_RowHeight));
                        ApplyNewTabProperties(m_parentTabPanel.tab);
                    }
                }
                if (tab != null)
                {
                    tab.Measure(new Size(availableSize.Width, m_RowHeight));
                }
            }
            else
            {
                if (m_parentTabPanel.tab != null)
                {
                    m_parentTabPanel.tab.Visibility = Visibility.Collapsed;
                }
            }


            if (m_ParentTabControl.IsNewButtonEnabled)
            {
                if (m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.First && m_parentTabPanel.tab != null)
                {
                    m_parentTabPanel.tab.Width = tab.DesiredSize.Width;
                    m_parentTabPanel.tab.Height = m_RowHeight;
                }
            }


            if (double.IsInfinity(size.Width) || DoubleUtil.IsNaN(size.Width))
            {
                size.Width = rowWidth;
                return size;
            }


            return size;
        }

        /// <summary>
        /// Gets the R factor.
        /// </summary>
        /// <returns></returns>
        private double GetRFactor()
        {
            double width = 20;
            if (m_ParentTabControl.Items.Count > 0)
            {
                width = (m_ParentTabControl.Items[m_ParentTabControl.Items.Count - 1] as TabItemExt).DesiredSize.Width / 2;
            }

            return width;
        }

        /// <summary>
        /// Applies the new tab properties.
        /// </summary>
        /// <param name="item">The item.</param>
        private void ApplyNewTabProperties(TabItemExt item)
        {
            if (m_ParentTabControl != null)
            {
                if (m_ParentTabControl.NewButtonBackground != null)
                {
                    item.Background = m_ParentTabControl.NewButtonBackground;
                }
                if (m_ParentTabControl.NewButtonBorderThickness != null)
                {
                    item.BorderThickness = m_ParentTabControl.NewButtonBorderThickness;
                }
            }
        }

        internal Size m_finalsize;
        /// <summary>
        /// Called to arrange and size tabs of a TabControlExt object. 
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange tabs.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            //SU I78477
            //double REDU_FACTOR = 5;
            //EU I78477
            HeaderPanel perent = (HeaderPanel)m_parentTabPanel.Parent;
            perent.InvalidateMeasure();
            if (m_ParentTabControl.IsNewButtonEnabled && m_ParentTabControl.NewButtonAlignment == NewButtonAlignment.Last)
            {
                ArrangeElements(finalSize);
            }
            else
            {
                //if (!m_ParentTabControl.m_loadstate)
                //{
                //    ArrangeElements1(finalSize);
                //}
                //else
                //{
                //    finalSize.Width += (m_parentTabPanel.DesiredSize.Width - finalSize.Width-REDU_FACTOR) / 2;
                //    ArrangeElements1(finalSize);
                //}
                m_finalsize = finalSize;
                ArrangeElements1(finalSize);
            }

            return finalSize;
        }

        /// <summary>
        /// Invoked when the <see cref="T:System.Windows.Media.VisualCollection"/> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="T:System.Windows.Media.Visual"/> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="T:System.Windows.Media.Visual"/> that was removed from the collection.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// Returns an alternative clipping geometry that represents the region that would be clipped if ClipToBounds were set to true. 
        /// </summary>
        /// <param name="layoutSlotSize">The available size provided by the element.</param>
        /// <returns>The potential clipping geometry.</returns>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            if (ClipToBounds)
            {
                if (m_parentTabPanel != null)
                {
                    if (m_scrollInfo.Offset != 0)
                    {
                        // 2 - TabItemExt margin.
                        return new RectangleGeometry(new Rect(-2, -2, 2 * RenderSize.Width, 2 * RenderSize.Height));
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the number of child elements for the control.
        /// </summary>
        /// <returns>An Int32 value that represents the number of child elements.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                int count = base.VisualChildrenCount;
                if (m_ParentTabControl != null)
                {
                    if (m_ParentTabControl.IsNewButtonEnabled)
                    {
                        if (count == 0 && !m_ParentTabControl.IsNewButtonClosedonNoChild)
                        {
                            return count + 1;
                        }
                        return (count >= 1) ? (count + 1) : count;
                    }
                    else
                    {
                        return count;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements. 
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>The requested child element. This should not return null; if the provided index is out of range, an exception is raised.</returns>
        protected override Visual GetVisualChild(int index)
        {
            Visual retvisual = null;
            if (m_ParentTabControl.IsNewButtonEnabled)
            {
                if (index == VisualChildrenCount - 1)
                {
                    retvisual = tab;
                }
                //else if (index == VisualChildrenCount - 2)
                //{
                //    retvisual = tab;
                //}
                else
                {
                    retvisual = base.GetVisualChild(index);
                }
            }
            else
            {
                retvisual = base.GetVisualChild(index);
            }

            return retvisual;
        }

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            m_ParentTabControl = TemplatedParent as TabControlExt;
            if (Parent is ScrollViewer)
                m_parentTabPanel = (Parent as ScrollViewer).Parent as TabPanelAdv;
            else
                m_parentTabPanel = Parent as TabPanelAdv;

            //tab = new TabItemExt
            //{
            //    Content = new Grid(),
            //    Header = "New 1",
            //};
            //Children.Add(tab);
            List<TabItemExt> childList = new List<TabItemExt>();
            //InternalChildren.Add(tab);
            //foreach (TabItemExt item in m_ParentTabControl.Items)
            //{
            //    childList.Add(item);
            //}
            //foreach (TabItemExt item in childList)
            //{
            //    m_ParentTabControl.Items.Remove(item);
            //}
            //foreach (TabItemExt item in childList)
            //{
            //    Children.Add(item);
            //}

        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseRightButtonUp"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was released.</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));

            if (item != null)
            {

            }
        }
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseRightButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the right mouse button was pressed.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnPreviewMouseRightButtonDown(e);

                TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));

                if (item != null)
                {
                    if (m_ParentTabControl != null && item != m_ParentTabControl.SelectedItem)
                    {
                        int itemIndex = m_ParentTabControl.ItemContainerGenerator.IndexFromContainer(m_ParentTabControl.GetTabItem(item));
                        if (m_ParentTabControl.SelectedIndex != itemIndex && itemIndex != -1)
                        {
                            m_ParentTabControl.SelectedIndex = itemIndex;
                        }
                    }
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnPreviewMouseLeftButtonUp(e);
                m_ParentTabControl.allowdrop = true;
                m_dragInfo.DragedItem = null;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/>�routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnPreviewMouseLeftButtonDown(e);
                m_dragInfo.DragedItem = null;
                m_dragInfo.Owner = null;
                TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));

                if (m_ParentTabControl.EnableLabelEdit)
                {
                    Point position = e.GetPosition(this);

                    if (m_clickInfo.IsDoubleClick(position))
                    {
                        if (item != null)
                        {
                            if (m_ParentTabControl.SelectedItem != null)
                            {
                                PreviewSelectedItemChangedEventArgs args = new PreviewSelectedItemChangedEventArgs(m_ParentTabControl.GetTabItem(m_ParentTabControl.SelectedItem), item);
                                m_ParentTabControl.FirePreviewSelectedItemChangedEvent(args);
                                if (!args.Cancel)
                                    LabelEditStartInternal(item);
                            }
                        }
                    }
                    else
                    {
                        m_clickInfo.LastTabItemPoint = e.GetPosition(this);
                    }

                    m_clickInfo.LastTabItemClick = DateTime.Now;
                }

                if (m_ParentTabControl.AllowDragDrop && item != null)
                {
                    m_dragInfo.DragedItem = item;
                    TabControlExt.DragSourceObject = m_ParentTabControl;
                    TabControlExt.DraggedItem = item;
                    m_dragInfo.Owner = m_ParentTabControl;
                }

                //if (item.CheckNewTabItem())
                //{
                //    //UIElement element = null;
                //    //NewTabItemArgs args = new NewTabItemArgs(element);
                //    //Container.InvokeNewTabItem(args);
                //    //if (args.Content != null)
                //    //{
                //    //    ActiveTabControl = (DocumentTabControl)d;
                //    //    Container.Items.Add(args.Content);
                //    //}
                //    //else
                //    //{
                //    //    //int index = ((DocumentTabControl)d).Items.IndexOf(e.NewValue as TabItemAdv);
                //    //    //if (index > 1)
                //    //    //{
                //    //    //    ((DocumentTabControl)d).SelectedIndex = index - 1;
                //    //    //}
                //    //    ((DocumentTabControl)d).SelectedItem = e.OldValue as TabItemAdv;
                //    //}

            }
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Called when [tab item_ key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnTabItemKeyDown(object sender, KeyEventArgs e)
        {
            if (null != m_editingItem
                && (e.Key == Key.Enter || e.Key == Key.Escape))
            {
                bool applyChanges = Key.Enter == e.Key;
                CompleteHeaderEditInternal(m_editingItem, applyChanges);
            }
        }

        /// <summary>
        /// Called when [tab item_ lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTabItem_LostFocus(object sender, RoutedEventArgs e)
        {
            TabItemExt editingItem = (TabItemExt)sender;
            CompleteHeaderEditInternal(editingItem, true);
        }

        /// <summary>
        /// Called when [editable text box_ lost keyboard focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void OnEditableTexBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (m_editingItem != null && !(e.NewFocus is ContextMenu))
            {
                CompleteHeaderEditInternal(m_editingItem, true);
            }
        }

        /// <summary>
        /// Handles the Completed event of the m_ScrollStoryboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void M_scrollStoryboard_Completed(object sender, EventArgs e)
        {
            InvalidateArrange();
            ScrollOffset = 0;
            m_scrollInfo.Offset = (double)m_scrollAnimation.To;
        }

        /// <summary>
        /// Scrolls the offset changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ScrollOffsetChanged(object sender, EventArgs e)
        {            
            m_scrollInfo.Offset = ScrollOffset;
        }

       
        #endregion

        #region Dependency properties
        /// <summary>
        /// Represents the ScrollOffset Dependency Property
        /// </summary>
        protected static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register("ScrollOffset", typeof(double), typeof(TabLayoutPanel), new UIPropertyMetadata(0d));
        #endregion

        /// <summary>
        /// Processes the drag.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ProcessDrag(DragEventArgs e)
        {
            TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));
            if(item.Parent is TabControlExt)
            m_dragInfo.Owner = (TabControlExt)item.Parent;
            Point position = e.GetPosition(item);
            m_dragInfo.ClearMarker();

            if (item != null && item != m_dragInfo.DragedItem)
            {
                // m_ParentTabControl.FireDragStart();
                m_dragInfo.DragOverItem = item;
                m_dragInfo.RotateTextWhenVertical = m_ParentTabControl.RotateTextWhenVertical;
                m_dragInfo.AdornerAlignment = m_dragInfo.IsLessThanMiddle(position, m_dragInfo.DragOverItem)
                    ? AdornerAlignment.FirstSide : AdornerAlignment.SecondSide;

				int index = m_ParentTabControl.GetTabItemIndex(item);
				if (index < 0) return;

                TabItemExt nextItem = GetItem(index, TabItems.Count - 1, 1);
                TabItemExt prewItem = GetItem(index, 0, -1);
                Dock tabPlacement = TabStripPlacement;
                bool rotate = m_ParentTabControl.RotateTextWhenVertical;
                if (m_dragInfo.AdornerAlignment == AdornerAlignment.FirstSide)
                {
                    if (Dock.Top == tabPlacement || Dock.Right == tabPlacement)
                    {
                        m_dragInfo.AdornerAlignment = AdornerAlignment.SecondSide;
                        if (index != 0)
                        {
                            TabItemExt previousitem = TabItems[index - 1] as TabItemExt;
                            m_dragInfo.RefreshMarker(previousitem);
                        }
                        else
                        {
                            m_dragInfo.AdornerAlignment = AdornerAlignment.FirstSide;
                            m_dragInfo.RefreshMarker(item);
                        }
                    }
                    if (Dock.Bottom == tabPlacement || Dock.Left == tabPlacement)
                    {
                        m_dragInfo.AdornerAlignment = AdornerAlignment.SecondSide;
                        if (index != TabItems.Count-1)
                        {
                            TabItemExt nexttabitem = TabItems[index + 1] as TabItemExt;
                            m_dragInfo.RefreshMarker(nexttabitem);
                        }
                        else
                        {
                            m_dragInfo.AdornerAlignment = AdornerAlignment.FirstSide;
                            m_dragInfo.RefreshMarker(item);
                        }
                    }
                }
                else
                {

                    if (Dock.Top == tabPlacement || Dock.Right == tabPlacement || rotate)
                    {
                        ValidateDragInfo(nextItem, prewItem, item, index, AdornerAlignment.FirstSide, AdornerAlignment.SecondSide);
                    }

                    if ((Dock.Bottom == tabPlacement || Dock.Left == tabPlacement) && !rotate)
                    {
                        ValidateDragInfo(prewItem, nextItem, item, index, AdornerAlignment.SecondSide, AdornerAlignment.FirstSide);
                    }
                }
                
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="cnt">Value of the CNT.</param>
        /// <param name="inc">Value of the inc.</param>
        /// <returns>returns the TabItemExt</returns>
        private TabItemExt GetItem(int itemIndex, int cnt, int inc)
        {
            return itemIndex < cnt ? TabItems[itemIndex + inc] as TabItemExt : null;
        }

        /// <summary>
        /// Validates the drag info.
        /// </summary>
        /// <param name="firstItem">The first item.</param>
        /// <param name="secondItem">The second item.</param>
        /// <param name="item">Value of the item.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="firstAlignment">The first alignment.</param>
        /// <param name="secondAlignment">The second alignment.</param>
        private void ValidateDragInfo(TabItemExt firstItem, TabItemExt secondItem, TabItemExt item, int itemIndex, AdornerAlignment firstAlignment, AdornerAlignment secondAlignment)
        {
            ValidateDragInfo(firstItem, AdornerAlignment.SecondSide);
            ValidateDragInfo(secondItem, AdornerAlignment.SecondSide);

            if (!((itemIndex == m_scrollInfo.LastTrimmedTabIndex && m_dragInfo.AdornerAlignment == secondAlignment)
                || (itemIndex == m_scrollInfo.FirstTrimmedTabIndex && m_dragInfo.AdornerAlignment == firstAlignment)))
            {
                m_dragInfo.RefreshMarker(item);
            }
        }

        /// <summary>
        /// Validates the drag info.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <param name="alignment">The alignment.</param>
        private void ValidateDragInfo(TabItemExt item, AdornerAlignment alignment)
        {
            if (item != null && item == m_dragInfo.DragedItem)
            {
                m_dragInfo.AdornerAlignment = alignment;
            }
        }

		/// <summary>
		/// Moves the specified item to the new index.
		/// </summary>
		/// <param name="item">the TabItem to move.</param>
		/// <param name="newIndex">the new index.</param>
		private void MoveItem(object item, int newIndex)
		{
			if (item == null || newIndex >= this.TabItems.Count || newIndex < 0) 
				return;

			object objDragged = m_ParentTabControl.GetSourceObject(item);

			if (objDragged == null) return;

			if (SourceItems == null)
			{
				(TabControlExt.DragSourceObject as TabControlExt).Items.Remove(TabControlExt.DraggedItem);
				TabItems.Remove(objDragged);
				TabItems.Insert(newIndex, objDragged);
			}
			else
			{
				SourceItems.Remove(objDragged);
				SourceItems.Insert(newIndex, objDragged);
			}
		}

        /// <summary>
        /// Processes the drop.
        /// </summary>
        private void ProcessDrop()
        {
            if (m_dragInfo.DragOverItem != null)
            {

				int oldIndex = m_ParentTabControl.GetTabItemIndex(TabControlExt.DraggedItem);
				int newIndex = m_ParentTabControl.GetTabItemIndex(m_dragInfo.DragOverItem);

				newIndex = ValidateIndex(oldIndex, newIndex);

                if (oldIndex < 0 || newIndex < 0)
                {
                    newIndex++;
                }
                if (newIndex >= TabItems.Count)
                    newIndex--;

				MoveItem(m_dragInfo.DragedItem, newIndex);

                TabControlExt.DraggedItem = null;
                TabControlExt.DragSourceObject = null;
                m_ParentTabControl.SelectedItem = TabItems[newIndex];
                m_dragInfo.ClearMarker();
            }
        }

        /// <summary>
        /// Validates the index.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <returns>returns an integer value</returns>
        private int ValidateIndex(int oldIndex, int newIndex)
        {
            return (TabStripPlacement == Dock.Top
                            || TabStripPlacement == Dock.Right
                            || m_ParentTabControl.RotateTextWhenVertical)
                            ? ValidateIndex(oldIndex, newIndex, AdornerAlignment.SecondSide, AdornerAlignment.FirstSide)
                            : ValidateIndex(oldIndex, newIndex, AdornerAlignment.FirstSide, AdornerAlignment.SecondSide);
        }

        /// <summary>
        /// Validates the index.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <param name="firstAlignment">The first alignment.</param>
        /// <param name="secondAlignment">The second alignment.</param>
        /// <returns>returns an integer value</returns>
        private int ValidateIndex(int oldIndex, int newIndex, AdornerAlignment firstAlignment, AdornerAlignment secondAlignment)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                if (newIndex < oldIndex)
                {
                    if (m_dragInfo.AdornerAlignment == firstAlignment)
                    {
                        newIndex++;
                    }
                }
                else if (m_dragInfo.AdornerAlignment == secondAlignment)
                {
                    newIndex--;
                }
            }

            else if (FlowDirection == FlowDirection.RightToLeft)
            {
                if (newIndex > oldIndex)
                {
                    if (m_dragInfo.AdornerAlignment == firstAlignment)
                    {
                        newIndex++;
                    }
                }
                else if (m_dragInfo.AdornerAlignment == secondAlignment)
                {
                    newIndex--;
                }
            }

            return newIndex;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                    m_dragInfo.DragedItem = null;
                base.OnMouseEnter(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                base.OnMouseMove(e);
                if (m_ParentTabControl != null && m_ParentTabControl.DependencyObjectType.Name == "TabControlExt")
                {
                    if (e.LeftButton == MouseButtonState.Pressed
                        && m_ParentTabControl.AllowDragDrop
                        && PermissionHelper.HasUnmanagedCodePermission && m_ParentTabControl.allowdrop)
                    {
                        Point currentPoint = e.GetPosition(this);

                        if (m_clickInfo.IsDragStarted(currentPoint)
                            && !m_dragInfo.SkipDrag
                            && m_dragInfo.DragedItem != null
                            && !TabControlExt.GetIsEditing(m_dragInfo.DragedItem))
                        {
                            TabControlExtDragEventArgs args = m_ParentTabControl.FireDragStart(m_dragInfo.DragedItem);
                            if (!args.Cancel && args.DragSource != null)
                            {
                                m_ParentTabControl.IsDragging = true;
                                UIElement content = m_dragInfo.DragedItem.Content as UIElement;

                                if (null != content)
                                {
                                    content.AllowDrop = false;
                                }
                                if (!m_dragInfo.DragedItem.IsNewTab)
                                {
                                    DragDrop.DoDragDrop(this, m_dragInfo.DragedItem, DragDropEffects.Move);
                                }
                            }
                            e.Handled = true;
                        }

                        m_dragInfo.SkipDrag = false;
                    }

                }
                base.OnMouseMove(e);
            }
        }

        private SystemGesture m_SystemGesture;
        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_SystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }
        #region TouchEvents
#if !SyncfusionFramework3_5

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    if (m_ParentTabControl != null && m_ParentTabControl.IsTouchEnabled && m_ParentTabControl.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (m_ParentTabControl.m_tabControlExtSystemGesture != SystemGesture.Drag)
        //            m_dragInfo.DragedItem = null;

        //    }
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchMove(TouchEventArgs e)
        //{
        //    if (m_ParentTabControl != null && m_ParentTabControl.IsTouchEnabled && m_ParentTabControl.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (m_ParentTabControl != null && m_ParentTabControl.DependencyObjectType.Name == "TabControlExt")
        //        {
        //            if (m_ParentTabControl.m_tabControlExtSystemGesture == SystemGesture.Drag
        //                && m_ParentTabControl.AllowDragDrop
        //                && PermissionHelper.HasUnmanagedCodePermission && m_ParentTabControl.allowdrop)
        //            {
        //                Point currentPoint = e.GetTouchPoint(this).Position;

        //                if (m_clickInfo.IsDragStarted(currentPoint)
        //                    && !m_dragInfo.SkipDrag
        //                    && m_dragInfo.DragedItem != null
        //                    && !TabControlExt.GetIsEditing(m_dragInfo.DragedItem))
        //                {
        //                    TabControlExtDragEventArgs args = m_ParentTabControl.FireDragStart(m_dragInfo.DragedItem);
        //                    if (!args.Cancel && args.DragSource != null)
        //                    {
        //                        m_ParentTabControl.IsDragging = true;
        //                        UIElement content = m_dragInfo.DragedItem.Content as UIElement;

        //                        if (null != content)
        //                        {
        //                            content.AllowDrop = false;
        //                        }
        //                        if (!m_dragInfo.DragedItem.IsNewTab)
        //                        {
        //                            if (m_AllowDrag)
        //                                DragDrop.DoDragDrop(this, m_dragInfo.DragedItem, DragDropEffects.Move);
        //                        }
        //                    }
        //                    e.Handled = true;
        //                }

        //                m_dragInfo.SkipDrag = false;
        //            }

        //        }
        //    }
        //    base.OnTouchMove(e);
        //}

        //protected override void OnTouchDown(TouchEventArgs e)
        //{
        //    base.OnTouchDown(e);
        //}

        //protected override void OnTouchUp(TouchEventArgs e)
        //{
        //    base.OnTouchUp(e);
        //}

        //protected override void OnPreviewTouchMove(TouchEventArgs e)
        //{
        //    if (m_ParentTabControl != null && m_ParentTabControl.IsTouchEnabled && m_ParentTabControl.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region TouchRightFingerDown
        //        if (m_ParentTabControl.m_tabControlExtSystemGesture == SystemGesture.HoldEnter)
        //        {
        //            OnPreviewTouchRightFingerDown(e);
        //        }
        //        #endregion
        //        base.OnPreviewTouchMove(e);
        //    }
        //}

        //protected override void OnPreviewTouchDown(TouchEventArgs e)
        //{
        //    if (m_ParentTabControl != null && m_ParentTabControl.IsTouchEnabled && m_ParentTabControl.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region TouchLeftFingerDown

        //        OnPreviewTouchLeftFingerDown(e);

        //        #endregion
                
        //        //#region TouchRightFingerDown
        //        //if (m_ParentTabControl.m_tabControlExtSystemGesture == SystemGesture.RightTap)
        //        //{
        //        //    OnPreviewTouchRightFingerDown(e);
        //        //}
        //        //#endregion
        //        base.OnPreviewTouchDown(e);
        //    }
        //}

        //private void OnPreviewTouchLeftFingerDown(TouchEventArgs e)
        //{
        //    m_dragInfo.DragedItem = null;
        //    m_dragInfo.Owner = null;
        //    TabItemExt item = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));

        //    if (m_ParentTabControl.EnableLabelEdit)
        //    {
        //        Point position = e.GetTouchPoint(this).Position;

        //        if (m_clickInfo.IsDoubleClick(position))
        //        {
        //            if (item != null)
        //            {
        //                if (m_ParentTabControl.SelectedItem != null)
        //                {
        //                    PreviewSelectedItemChangedEventArgs args = new PreviewSelectedItemChangedEventArgs(m_ParentTabControl.GetTabItem(m_ParentTabControl.SelectedItem), item);
        //                    m_ParentTabControl.FirePreviewSelectedItemChangedEvent(args);
        //                    if (!args.Cancel)
        //                        LabelEditStartInternal(item);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            m_clickInfo.LastTabItemPoint = e.GetTouchPoint(this).Position;
        //        }

        //        m_clickInfo.LastTabItemClick = DateTime.Now;
        //    }

        //    if (m_ParentTabControl.AllowDragDrop && item != null)
        //    {
        //        m_dragInfo.DragedItem = item;
        //        TabControlExt.DragSourceObject = m_ParentTabControl;
        //        TabControlExt.DraggedItem = item;
        //        m_dragInfo.Owner = m_ParentTabControl;
        //    }
        //}

        //private void OnPreviewTouchRightFingerDown(TouchEventArgs e)
        //{
        //    TabItemExt item1 = (TabItemExt)VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt));

        //    if (item1 != null)
        //    {
        //        if (m_ParentTabControl != null && item1 != m_ParentTabControl.SelectedItem)
        //        {
        //            int itemIndex = m_ParentTabControl.ItemContainerGenerator.IndexFromContainer(m_ParentTabControl.GetTabItem(item1));
        //            if (m_ParentTabControl.SelectedIndex != itemIndex && itemIndex != -1)
        //            {
        //                m_ParentTabControl.SelectedIndex = itemIndex;
        //            }
        //        }
        //    }
        //}

        //private void OnPreviewTouchLeftFingerUp(TouchEventArgs e)
        //{
        //    m_ParentTabControl.allowdrop = true;
        //    m_dragInfo.DragedItem = null;
        //}

        //private void OnPreviewTouchRightFingerUp(TouchEventArgs e)
        //{
        //}

        //protected override void OnPreviewTouchUp(TouchEventArgs e)
        //{
        //    if (m_ParentTabControl != null && m_ParentTabControl.IsTouchEnabled && m_ParentTabControl.m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        #region TouchLeftFingerUp
        //        if (m_ParentTabControl.m_tabControlExtSystemGesture == SystemGesture.Tap)
        //        {
        //            OnPreviewTouchLeftFingerUp(e);
        //        }
        //        #endregion

        //        #region TouchRightFingerUp
        //        else if (m_ParentTabControl.m_tabControlExtSystemGesture == SystemGesture.RightTap)
        //        {
        //            OnPreviewTouchRightFingerUp(e);
        //        }
        //        #endregion
        //    }
        //    base.OnPreviewTouchUp(e);
        //}
#endif
        #endregion
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragEnter"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            ProcessDragEx(e);
        }


        protected override void OnPreviewQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                m_ParentTabControl.allowdrop = false;
            }
            base.OnPreviewQueryContinueDrag(e);
        }

        /// <summary>
        /// Processes the drag ex.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void ProcessDragEx(DragEventArgs e)
        {
            if (m_ParentTabControl != null && m_ParentTabControl.DependencyObjectType.Name == "TabControlExt" && e.Effects != DragDropEffects.None && m_dragInfo.CanDrag(e))
            {
                ProcessDrag(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragOver"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            ProcessDragEx(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragEnter"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            if (m_ParentTabControl != null && m_ParentTabControl.DependencyObjectType.Name == "TabControlExt")
            {
                base.OnDrop(e);
                TabControlExtDragEventArgs args=null;
                
                if (TabControlExt.DraggedItem is TabItemExt)
                {
                    args = m_ParentTabControl.FireDragEnd(TabControlExt.DraggedItem as TabItemExt);
                    if (!args.Cancel)
                    {
                        ProcessDrop();
                        m_ParentTabControl.IsDragging = false;
                    }
                }
               
            }
            m_dragInfo.ClearMarker();
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.GiveFeedback"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.GiveFeedbackEventArgs"/> that contains the event data.</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            if (e.Effects == DragDropEffects.None)
            {
                m_dragInfo.ClearMarker();
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.DragDrop.DragLeave"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            m_dragInfo.ClearMarker();
        }
    }
}