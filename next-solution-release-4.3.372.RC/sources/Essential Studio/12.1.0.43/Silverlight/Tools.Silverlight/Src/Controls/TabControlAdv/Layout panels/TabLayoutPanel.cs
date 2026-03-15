#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Syncfusion.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents panel responsible for tab item's layout logic. 
    /// </summary>
    public class TabLayoutPanel : Panel
    {
        #region Constants
        /// <summary>
        /// Presents name of EditableHeader text box.
        /// </summary>
        private const string EDITABLEHEADERNAME = "EditableHeader";

        /// <summary>
        /// Name of the content.
        /// </summary>
        private const string CONTENTNAME = "Content";

        /// <summary>
        /// Minimum distance for dragging.
        /// </summary>
        private const int MinimumDragDistance = 4;

        /// <summary>
        /// Margin of tab items when style is classic.
        /// </summary>
        private const int ClassicStyleMargin = 3;
        #endregion

        #region Structs
        /// <summary>
        /// Represents the dragging data.
        /// </summary>
        private struct DragInfo
        {
            #region Properties
            /// <summary>
            /// Gets or sets dragged tab item.
            /// </summary>
            public TabItemAdv DraggedItem
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the tab item over which dragging occurs.
            /// </summary>
            public TabItemAdv DragOverItem
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether text is rotated 
            /// when tab strip placemnt is left or right.
            /// </summary>
            public bool RotateTextWhenVertical
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets marker alignment.
            /// </summary>
            public DragMarkerAlignment DragMarkerAlignment
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether to skip the dragging.
            /// </summary>
            public bool SkipDrag
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the tabcontrol parent.
            /// </summary>
            public TabControlAdv TabControlParent
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the X offset of the marker.
            /// </summary>
            public double OffsetX
            {
                get;
                set;
            }
            #endregion

            #region Implementation
            /// <summary>
            /// Coerces the offset.
            /// </summary>
            internal void CoerceOffset()
            {
                if (this.DragOverItem != null)
                {
                    double leftMargin = this.DragOverItem.Margin.Left;
                    double rightMargin = this.DragOverItem.Margin.Right;



                    RotateTransform transfrom = new RotateTransform();

                    if (this.TabControlParent != null)
                    {
                        switch (this.TabControlParent.TabStripPlacement)
                        {
                            case TabStripPlacement.Top:
                                transfrom.Angle = 0;
                                DragOverItem.DragMarker.RenderTransform = transfrom;

                                OffsetX = DragMarkerAlignment == DragMarkerAlignment.RightSide
                                    ? this.DragOverItem.ActualWidth + rightMargin : -leftMargin;
                                break;

                            case TabStripPlacement.Left:
                                if (this.RotateTextWhenVertical)
                                {
                                    transfrom.Angle = 90;

                                    OffsetX = DragMarkerAlignment == DragMarkerAlignment.RightSide
                                        ? -leftMargin : this.DragOverItem.ActualWidth + rightMargin;
                                }
                                else
                                {
                                    transfrom.Angle = 0;

                                    OffsetX = DragMarkerAlignment == DragMarkerAlignment.RightSide
                                    ? this.DragOverItem.ActualWidth + rightMargin : -leftMargin;
                                }

                                DragOverItem.DragMarker.RenderTransform = transfrom;
                                break;

                            case TabStripPlacement.Right:
                                if (this.RotateTextWhenVertical)
                                {
                                    transfrom.Angle = 90;
                                }
                                else
                                {
                                    transfrom.Angle = 0;
                                }

                                DragOverItem.DragMarker.RenderTransform = transfrom;
                                OffsetX = DragMarkerAlignment == DragMarkerAlignment.RightSide
                                     ? this.DragOverItem.ActualWidth + rightMargin : -leftMargin;
                                break;

                            case TabStripPlacement.Bottom:
                                transfrom.Angle = 0;
                                DragOverItem.DragMarker.RenderTransform = transfrom;

                                OffsetX = DragMarkerAlignment == DragMarkerAlignment.RightSide
                                    ? -leftMargin : this.DragOverItem.ActualWidth + rightMargin;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            /// <summary>
            /// Indicates whether drag marker crosses the middle of the tab item.
            /// </summary>
            /// <param name="position">Position of the tab item.</param>
            /// <param name="item">The tab item.</param>
            /// <returns>A value indicating whether drag marker crosses the middle of the tab item.</returns>
            public bool IsLessThanMiddle(Point position, FrameworkElement item)
            {
                return position.X < item.ActualWidth / 2;
            }

            /// <summary>
            /// Refreshes the drag marker.
            /// </summary>
            /// <param name="newItem">The tab item.</param>
            public void RefreshMarker(TabItemAdv newItem)
            {
                this.HideMarker();
                this.DragOverItem = newItem;
                this.ShowMarker();
            }

            /// <summary>
            /// Clears the drag marker.
            /// </summary>
            public void ClearMarker()
            {
                if (this.DragOverItem != null)
                {
                    HideMarker();
                    DragOverItem = null;
                }
            }

            /// <summary>
            /// Adds the drag marker to the tab item.
            /// </summary>
            private void ShowMarker()
            {
                if (DragOverItem != null && DragOverItem.DragMarker != null)
                {
                    this.CoerceOffset();
                    DragOverItem.DragMarker.HorizontalOffset = OffsetX - this.DragOverItem.DragMarker.ActualWidth;
                    DragOverItem.DragMarker.VerticalOffset = -DragOverItem.ActualHeight + (this.DragOverItem.DragMarker.ActualHeight / 2);
                    DragOverItem.DragMarker.Show();
                }
            }

            /// <summary>
            /// Removes the drag marker from the tab item.
            /// </summary>
            private void HideMarker()
            {
                if (DragOverItem != null && DragOverItem.DragMarker != null)
                {
                    DragOverItem.DragMarker.Close();
                }
            }
            #endregion
        }

        /// <summary>
        /// Represents the scrolling data.
        /// </summary>
        private struct ScrollInfo
        {
            /// <summary>
            /// Indicates whether scroll buttons must be shown.
            /// </summary>
            public bool NeedScrollButtonsShow;

            /// <summary>
            /// Index of the first trimmed tab item.
            /// </summary>
            public int FirstTrimmedTabIndex;

            /// <summary>
            /// Index of the last trimmed tab item.
            /// </summary>
            public int LastTrimmedTabIndex;

            /// <summary>
            /// Tab panel's desired width.
            /// </summary>
            public double DesiredWidth;

            /// <summary>
            /// Width of the last trimmed tab item.
            /// </summary>
            public double LastTabTrimmedWidth;

            /// <summary>
            /// Width of the first trimmed tab item.
            /// </summary>
            public double FirstTabTrimmedWidth;

            /// <summary>
            /// Sroll page width.
            /// </summary>
            public double PageWidth;

            /// <summary>
            /// Tab panel's width.
            /// </summary>
            public double AllTrimmedWidth;

            /// <summary>
            /// Value used for scrolling tab items.
            /// </summary>
            public double Offset;
        }

        /// <summary>
        /// Represents the tab item's click data.
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
            /// Indicates whether double click is occured.
            /// </summary>
            /// <param name="position">Mouse position.</param>
            /// <returns>A value indicating whether double click is occured.</returns>
            public bool IsDoubleClick(Point position)
            {
                if (((DateTime.Now.Subtract(LastTabItemClick).TotalMilliseconds < 450)
                    && (Math.Abs((LastTabItemPoint.X - position.X)) <= 2))
                    && (Math.Abs((LastTabItemPoint.Y - position.Y)) <= 2))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Indicates whether dragging has occured.
            /// </summary>
            /// <param name="position">Mouse position.</param>
            /// <returns>A value indicating whether dragging has occured.</returns>
            public bool IsDragStarted(Point position)
            {
                if (Math.Abs(position.X - LastTabItemPoint.X) > MinimumDragDistance ||
                    Math.Abs(position.Y - LastTabItemPoint.Y) > MinimumDragDistance)
                {
                    return true;
                }

                return false;
            }
        }
        #endregion

        #region Private members
        /// <summary>
        /// Tab item's intersection length.
        /// </summary>
        internal double tabIntersectionFactor = 0;

        /// <summary>
        /// Used for scroll animating.
        /// </summary>
        private Storyboard scrollStoryboard;

        /// <summary>
        /// Used for scroll animating.
        /// </summary>
        private DoubleAnimation scrollAnimation;

        /// <summary>
        /// Edited tab item.
        /// </summary>
        private TabItemAdv editingItem;

        /// <summary>
        /// Average tab item's width used shrinkToFit layout.
        /// </summary>
        private double averageWidth;

        /// <summary>
        /// Tab item's row height.
        /// </summary>
        private double rowHeight;

        /// <summary>
        /// Row count.
        /// </summary>
        private int numRows;

        /// <summary>
        /// Tabcontrol parent.
        /// </summary>
        private TabControlAdv tabControlParent;

        /// <summary>
        /// Tabpanel parent.
        /// </summary>
        private TabPanelAdv tabPanelParent;

        /// <summary>
        /// Click data.
        /// </summary>
        private ClickInfo clickInfo;

        /// <summary>
        /// Scroll data.
        /// </summary>
        private ScrollInfo scrollInfo;

        /// <summary>
        /// Indicates whether shrink is needed.
        /// </summary>
        private bool needShrink;

        /// <summary>
        /// Indicates whether layout is rightToLeft.
        /// </summary>
        private bool isRightToLeft = false;

        /// <summary>
        /// Drag data.
        /// </summary>
        private DragInfo dragInfo;

        /// <summary>
        /// Indicates whether tab item is captured by mouse.
        /// </summary>
        private bool isItemMouseCaptured = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the average tab item's width used shrinkToFit layout.
        /// </summary>
        internal double AverageWidth
        {
            get
            {
                return this.averageWidth;
            }
        }

        /// <summary>
        /// Gets the tab item's row height.
        /// </summary>
        internal double RowHeight
        {
            get
            {
                return this.rowHeight;
            }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        internal int NumRows
        {
            get
            {
                return this.numRows;
            }
        }

        /// <summary>
        /// Gets the visible items count.
        /// </summary>
        internal int VisibleItemsCount
        {
            get
            {
                int count = 0;

                foreach (UIElement element in Children)
                {
                    if (element.Visibility == Visibility.Visible
                        && !(element is ScrollingPanel))
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether shrink is needed.
        /// </summary>
        internal bool NeedShrink
        {
            get
            {
                return needShrink;
            }
        }

        /// <summary>
        /// Gets the tab scroll style.
        /// </summary>
        internal TabScrollStyle TabScrollStyle
        {
            get
            {
                TabScrollStyle style = TabScrollStyle.Normal;
                if (TabControlParent != null)
                {
                    style = TabControlParent.TabScrollStyle;
                }

                return style;
            }
        }

        /// <summary>
        /// Gets or sets the scroll offset.
        /// </summary>
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
        /// Gets the scrolling panel.
        /// </summary>
        internal ScrollingPanel ScrollingPanel
        {
            get
            {
                ScrollingPanel panel = null;
                if (this.TabPanelParent != null)
                {
                    panel = this.TabPanelParent.ScrollingPanel;
                    if (panel != null && panel.LayoutPanel == null)
                    {
                        panel.LayoutPanel = this;
                    }
                }

                return panel;
            }
        }

        /// <summary>
        /// Gets the close button.
        /// </summary>
        private CloseButton CloseButton
        {
            get
            {
                CloseButton button = null;
                if (this.TabPanelParent != null)
                {
                    button = this.TabPanelParent.CloseButton;
                }

                return button;
            }
        }

        /// <summary>
        /// Gets the close button.
        /// </summary>
        private Border ButtonsBorder
        {
            get
            {
                Border buttonsBorder = null;
                if (this.TabPanelParent != null)
                {
                    buttonsBorder = this.TabPanelParent.ButtonsBorder;
                }

                return buttonsBorder;
            }
        }

        /// <summary>
        /// Gets the menu panel.
        /// </summary>
        private StackPanel MenuPanel
        {
            get
            {
                StackPanel panel = null;
                if (this.TabPanelParent != null)
                {
                    panel = this.TabPanelParent.MenuPanel;
                }

                return panel;
            }
        }

        /// <summary>
        /// Gets the tab strip placement.
        /// </summary>
        private TabStripPlacement TabStripPlacement
        {
            get
            {
                TabStripPlacement top = TabStripPlacement.Top;

                if (TabControlParent != null)
                {
                    top = TabControlParent.TabStripPlacement;
                }

                return top;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to keep tab in first row.
        /// </summary>
        private bool KeepTabInFront
        {
            get
            {
                bool result = true;
                if (TabControlParent != null)
                {
                    result = TabControlParent.KeepTabInFront;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets tab item's layout type.
        /// </summary>
        private TabItemLayoutType TabItemLayout
        {
            get
            {
                TabItemLayoutType result = TabItemLayoutType.SingleLine;
                if (TabControlParent != null)
                {
                    result = TabControlParent.TabItemLayout;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets tab item's size mode.
        /// </summary>
        private TabItemSizeMode TabItemSizeMode
        {
            get
            {
                TabItemSizeMode result = TabItemSizeMode.Normal;
                if (TabControlParent != null)
                {
                    result = TabControlParent.TabItemSizeMode;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets scroll buttons visibility.
        /// </summary>
        private TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get
            {
                TabScrollButtonVisibility visibility = TabScrollButtonVisibility.Auto;
                if (TabControlParent != null)
                {
                    visibility = TabControlParent.TabScrollButtonVisibility;
                }

                return visibility;
            }
        }

        /// <summary>
        /// Gets the tab items.
        /// </summary>
        private TabHeaderCollection TabHeaders
        {
            get
            {
                return TabControlParent != null
                    ? TabControlParent.TabHeaders : null;
            }
        }

        /// <summary>
        /// Gets or sets the tabcontrol parent.
        /// </summary>
        internal TabControlAdv TabControlParent
        {
            get
            {
                return tabControlParent;
            }

            set
            {
                if (value != tabControlParent)
                {
                    if (this.tabControlParent != null)
                    {
                        this.tabControlParent.RotateTextWhenVerticalChanged -= new PropertyChangedCallback(TabIntersectionFactorUpdate);
                        this.tabControlParent.TabStripPlacementChanged -= new PropertyChangedCallback(TabIntersectionFactorUpdate);
                    }

                    tabControlParent = value;

                    if (this.tabControlParent != null)
                    {
                        this.tabControlParent.RotateTextWhenVerticalChanged += new PropertyChangedCallback(TabIntersectionFactorUpdate);
                        this.tabControlParent.TabStripPlacementChanged += new PropertyChangedCallback(TabIntersectionFactorUpdate);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets tabpanel parent.
        /// </summary>
        internal TabPanelAdv TabPanelParent
        {
            get
            {
                return tabPanelParent;
            }

            set
            {
                tabPanelParent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tab item's layout is rightToLeft.
        /// </summary>
        internal bool IsRightToLeft
        {
            get
            {
                return isRightToLeft;
            }

            set
            {
                this.isRightToLeft = value;
                this.UpdateDockPositions();
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsRotatingNeeded
        /// </summary>
        private bool IsRotatingNeeded
        {
            get
            {
                bool isNeeded = false;
                if (this.TabControlParent != null && this.TabControlParent.RotateTextWhenVertical &&
                    (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left || this.TabControlParent.TabStripPlacement == TabStripPlacement.Right))
                {
                    isNeeded = true;
                }

                return isNeeded;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize a new instance of TabLayoutPanel class.
        /// </summary>
        public TabLayoutPanel()
        {
            clickInfo = new ClickInfo();
            scrollInfo = new ScrollInfo();

            this.Loaded += new RoutedEventHandler(TabLayoutPanelLoaded);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(TabLayoutPanelMouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(TabLayoutPanelMouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(TabLayoutPanelMouseLeftButtonUp);
        }
        #endregion

        #region Overrides
        
        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <param name="availableSize">Measurement constraints, a control cannot return a size larger than the constraint.</param>
        /// <returns>The size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            needShrink = false;
            Size size = Size.Empty;
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                this.CalculateTabsSize(availableSize);
                double totalWidth = MeasureElements(new Size(availableSize.Width, rowHeight));

                double rowWidth = 0;
                if (rowWidth < totalWidth)
                {
                    rowWidth = totalWidth;
                }
                scrollInfo.NeedScrollButtonsShow = availableSize.Width < totalWidth;

                if (this.ScrollingPanel != null)
                {
                    switch (TabScrollButtonVisibility)
                    {
                        case TabScrollButtonVisibility.Auto:
                            if (!TabPanelParent.IsAllItemsVisible)
                            {
                                this.ScrollingPanel.Show();
                            }
                            else
                            {
                                this.ScrollingPanel.Hide();
                            }

                            break;

                        case TabScrollButtonVisibility.Hidden:
                            ScrollingPanel.Hide();
                            break;

                        case TabScrollButtonVisibility.Visible:
                            if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSizeMode == TabItemSizeMode.Normal)
                            {
                                ScrollingPanel.Show();
                            }
                            else
                            {
                                ScrollingPanel.Hide();
                            }

                            break;

                        default:
                            break;
                    }
                }

                if (TabItemLayout != TabItemLayoutType.SingleLine && numRows == VisibleItemsCount)
                {
                    this.averageWidth = availableSize.Width;

                    foreach (UIElement element in Children)
                    {
                        if (element.Visibility == Visibility.Collapsed)
                        {
                            continue;
                        }

                        InvalidateMeasure();
                    }
                }

                if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSizeMode == TabItemSizeMode.ShrinkToFit && totalWidth > availableSize.Width)
                {
                    needShrink = true;
                    this.averageWidth = availableSize.Width / VisibleItemsCount;
                    foreach (UIElement element in Children)
                    {
                        if (element.Visibility == Visibility.Collapsed)
                        {
                            continue;
                        }
                        element.Measure(new Size(averageWidth, rowHeight));
                    }
                }

                size = new Size(availableSize.Width, rowHeight * numRows);
                if (ScrollingPanel != null)
                {
                    ScrollingPanel.Margin = new Thickness(1, 0, 0, 0);
                    ScrollingPanel.Measure(size);
                }

                if (double.IsInfinity(size.Width) || double.IsNaN(size.Width))
                {
                    size.Width = rowWidth;
                }

            }
            else
            {
                if (this.TabPanelParent != null)
                {
                    if (this.TabPanelParent.Visibility != Visibility.Visible && VisibleItemsCount > 0)
                    {
                        TabPanelParent.Visibility = Visibility.Visible;
                        TabControlParent.IsAllTabsClosed = false;
                    }

                    this.CalculateTabsSize(availableSize);
                    double totalWidth = MeasureElements(new Size(availableSize.Width, rowHeight));

                    double rowWidth = 0;
                    if (rowWidth < totalWidth)
                    {
                        rowWidth = totalWidth;
                    }

                    scrollInfo.NeedScrollButtonsShow = availableSize.Width < totalWidth;
                    TabPanelParent.IsAllItemsVisible = !scrollInfo.NeedScrollButtonsShow ||
                                                         TabItemLayout != TabItemLayoutType.SingleLine ||
                                                         TabItemSizeMode != TabItemSizeMode.Normal;
                    if (this.ScrollingPanel != null)
                    {
                        switch (TabScrollButtonVisibility)
                        {
                            case TabScrollButtonVisibility.Auto:
                                if (!TabPanelParent.IsAllItemsVisible)
                                {
                                    this.ScrollingPanel.Show();
                                }
                                else
                                {
                                    this.ScrollingPanel.Hide();
                                }

                                break;

                            case TabScrollButtonVisibility.Hidden:
                                ScrollingPanel.Hide();
                                break;

                            case TabScrollButtonVisibility.Visible:
                                if (TabItemLayout == TabItemLayoutType.SingleLine && TabItemSizeMode == TabItemSizeMode.Normal)
                                {
                                    ScrollingPanel.Show();
                                }
                                else
                                {
                                    ScrollingPanel.Hide();
                                }

                                break;

                            default:
                                break;
                        }
                    }

                    if (TabItemLayout != TabItemLayoutType.SingleLine && numRows == VisibleItemsCount)
                    {
                        this.averageWidth = availableSize.Width;

                        foreach (UIElement element in Children)
                        {
                            if (element.Visibility == Visibility.Collapsed)
                            {
                                continue;
                            }

                            InvalidateMeasure();
                        }
                    }

                    if (TabItemLayout == TabItemLayoutType.SingleLine &&
                        TabItemSizeMode == TabItemSizeMode.ShrinkToFit && totalWidth > availableSize.Width)
                    {
                        needShrink = true;
                        this.averageWidth = availableSize.Width / VisibleItemsCount;
                        foreach (UIElement element in Children)
                        {
                            if (element.Visibility == Visibility.Collapsed)
                            {
                                continue;
                            }

                            element.Measure(new Size(averageWidth, rowHeight));
                        }
                    }

                    size = new Size(availableSize.Width, rowHeight * numRows);
                    if (ScrollingPanel != null)
                    {
                        ScrollingPanel.Margin = new Thickness(1, 0, 0, 0);
                        ScrollingPanel.Measure(size);
                    }

                    if (double.IsInfinity(size.Width) || double.IsNaN(size.Width))
                    {
                        size.Width = rowWidth;
                    }
                }
            }

            if (double.IsInfinity(size.Width))
            {
                size.Width = 0;
            }

            if (double.IsInfinity(size.Height))
            {
                size.Height = 0;
            }

            RectangleGeometry rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 0, size.Width, size.Height);
            this.Clip = rect;
            return size;            
        }

        /// <summary>
        /// Called to arrange and size tabs of a TabControlAdv object. 
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange tabs.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeElements(finalSize);
            return finalSize;
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// Occurs when the left mouse button is pressed 
        /// while the mouse pointer is over a panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabLayoutPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragInfo.DraggedItem = null;
            dragInfo.TabControlParent = null;

            if (e.OriginalSource is UIElement)
            {
                TabItemAdv item = null;
                item = this.FindAncestor(e.OriginalSource as UIElement, typeof(TabItemAdv)) as TabItemAdv;
                if (item == null && TabControlParent != null)
                {
                    item = TabControlParent.ItemContainerGenerator.ContainerFromIndex(TabControlParent.SelectedIndex) as TabItemAdv;
                }

                this.SelectItemInternal();

                if (this.TabControlParent != null && TabControlParent.EnableLabelEdit)
                {
                    Point position = e.GetPosition(this);

                    if (clickInfo.IsDoubleClick(position))
                    {
                        this.LabelEditStartInternal(item);
                    }
                    else
                    {
                        clickInfo.LastTabItemPoint = e.GetPosition(this);
                    }

                    clickInfo.LastTabItemClick = DateTime.Now;
                }

                if (TabControlParent.AllowDragDrop && item != null)
                {
                    dragInfo.DraggedItem = item;
                    dragInfo.TabControlParent = TabControlParent;
                    item.CaptureMouse();
                    isItemMouseCaptured = true;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Occurs when the mouse pointer hovers over a panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabLayoutPanelMouseMove(object sender, MouseEventArgs e)
        {
            if (TabControlParent.AllowDragDrop && isItemMouseCaptured)
            {
                Point currentPoint = e.GetPosition(this);
                if (dragInfo.DraggedItem != null)
                {
                    dragInfo.DraggedItem.Cursor = Cursors.Hand;
                }

                if (clickInfo.IsDragStarted(currentPoint) && !dragInfo.SkipDrag && dragInfo.DraggedItem != null)
                {
                    this.ProcessDrag(e);
                }
                dragInfo.SkipDrag = false;
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is released 
        /// while the mouse pointer is over a panel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabLayoutPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isItemMouseCaptured = false;
            this.ProcessDrop();
        }

        /// <summary>
        ///  Occurs when a panel has completed layout passes,
        ///  has rendered, and is ready for interaction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabLayoutPanelLoaded(object sender, RoutedEventArgs e)
        {
            if (this.TabItemLayout == TabItemLayoutType.MultiLine || this.TabItemLayout == TabItemLayoutType.MultiLineWithFullWidth
                || this.TabItemSizeMode == TabItemSizeMode.ShrinkToFit)
            {
                this.ScrollingPanel.Visibility = Visibility.Collapsed;
            }

            if (this.TabControlParent != null)
            {

                {
                    this.tabIntersectionFactor = 0;
                }

                this.InvalidateMeasure();
                this.InvalidateArrange();
            }
        }

        /// <summary>
        /// Occurs when visual style of tab control is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabIntersectionFactorUpdate(object sender, DependencyPropertyChangedEventArgs e)
        {

            {
                this.tabIntersectionFactor = 0;
            }

            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        /// <summary>
        /// Occurs when textbox text is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Occurs when a keyboard key is pressed while the System.Windows.UIElement
        /// has focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnTabItemKeyDown(object sender, KeyEventArgs e)
        {
            if (null != editingItem && (e.Key == Key.Enter || e.Key == Key.Escape))
            {
                bool applyChanges = Key.Enter == e.Key;
                this.CompleteHeaderEditInternal(editingItem, applyChanges);
            }
        }

        /// <summary>
        /// Called when panel losts focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnTabItemLostFocus(object sender, RoutedEventArgs e)
        {
            ScrollViewer viewer = Syncfusion.Silverlight.Shared.VisualUtils.FindAncestor(e.OriginalSource as DependencyObject, typeof(ScrollViewer)) as ScrollViewer;
            this.CompleteHeaderEditInternal(editingItem, true);
        }

        /// <summary>
        /// Handles the completed event of the m_ScrollStoryboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ScrollStoryboardCompleted(object sender, EventArgs e)
        {
            scrollInfo.Offset = (double)scrollAnimation.To;
            this.InvalidateArrange();
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the ScrollOffset dependency property.
        /// </summary>
        protected static readonly DependencyProperty ScrollOffsetProperty = DependencyProperty.Register("ScrollOffset", typeof(double), typeof(TabLayoutPanel), new PropertyMetadata(0d, OnScrollOffsetChanged));
        #endregion

        #region Implementation
        /// <summary>
        /// Calls OnScrollOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabLayoutPanel instance = (TabLayoutPanel)d;
            instance.OnScrollOffsetChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ScrollOffsetChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScrollOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            scrollInfo.Offset = this.ScrollOffset;
            this.ArrangeElements(new Size(this.ActualWidth, this.ActualHeight));
        }

        /// <summary>
        /// Updates child element's positions.
        /// </summary>
        private void UpdateDockPositions()
        {
            if (this.isRightToLeft)
            {
                if (this.TabControlParent.TabVisualStyle != TabVisualStyle.None)
                {
                    if (this.ButtonsBorder != null)
                    {
                        DockPanel.SetDock(this.ButtonsBorder, Dock.Right);
                    }
                }
                else
                {
                    if (this.ButtonsBorder != null)
                    {
                        DockPanel.SetDock(this.ButtonsBorder, Dock.Left);
                    }
                }

                if (this.CloseButton != null)
                {
                    DockPanel.SetDock(this.CloseButton, Dock.Left);
                }

                if (this.MenuPanel != null)
                {
                    DockPanel.SetDock(this.MenuPanel, Dock.Left);
                }

                if (this.TabPanelParent != null && this.TabPanelParent.ContentElement != null)
                {
                    DockPanel.SetDock(this.TabPanelParent.ContentElement, Dock.Right);
                }
            }
            else
            {
                if (this.ButtonsBorder != null)
                {
                    DockPanel.SetDock(this.ButtonsBorder, Dock.Right);
                }

                if (this.CloseButton != null)
                {
                    DockPanel.SetDock(this.CloseButton, Dock.Right);
                }

                if (this.MenuPanel != null)
                {
                    DockPanel.SetDock(this.MenuPanel, Dock.Right);
                }

                if (this.TabPanelParent != null && this.TabPanelParent.ContentElement != null)
                {
                    DockPanel.SetDock(this.TabPanelParent.ContentElement, Dock.Left);
                }
            }
        }


        /// <summary>
        /// Processes the drag event.
        /// </summary>
        /// <param name="e">The needed data.</param>
        private void ProcessDrag(MouseEventArgs e)
        {
            if (e.OriginalSource is UIElement)
            {
                TabItemAdv item = this.FindAncestor(e.OriginalSource as UIElement, typeof(TabItemAdv)) as TabItemAdv;
                Point position = e.GetPosition(item);

                if (item == dragInfo.DraggedItem)
                {
                    dragInfo.ClearMarker();
                }

                if (item != null && item != dragInfo.DraggedItem)
                {
                    dragInfo.ClearMarker();
                    TabControlParent.FireDragStart();
                    dragInfo.DragOverItem = item;
                    dragInfo.RotateTextWhenVertical = TabControlParent.RotateTextWhenVertical;
                    dragInfo.DragMarkerAlignment = dragInfo.IsLessThanMiddle(position, dragInfo.DragOverItem)
                        ? DragMarkerAlignment.LeftSide : DragMarkerAlignment.RightSide;

                    int itemIndex = TabHeaders.IndexOf(item);
                    TabItemAdv nextItem = itemIndex < TabHeaders.Count - 1 ? TabHeaders[itemIndex + 1] as TabItemAdv : null;

                    TabStripPlacement tabPlacement = TabStripPlacement;

                    if (TabStripPlacement.Top == tabPlacement || TabStripPlacement.Right == tabPlacement)
                    {
                        this.ValidateDragInfo(item, itemIndex, DragMarkerAlignment.LeftSide, DragMarkerAlignment.RightSide);
                    }
                    else if (TabStripPlacement.Bottom == tabPlacement || TabStripPlacement.Left == tabPlacement)
                    {
                        this.ValidateDragInfo(item, itemIndex, DragMarkerAlignment.RightSide, DragMarkerAlignment.LeftSide);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the drop event.
        /// </summary>
        private void ProcessDrop()
        {
            if (this.TabControlParent != null && dragInfo.DragOverItem != null && dragInfo.DraggedItem != null
                && dragInfo.DraggedItem.TabItemParent != null && dragInfo.DragOverItem.TabItemParent != null)
            {
                TabControlParent.FireDragEnd();
                int oldIndex = TabControlParent.GetIndexOfTabItemAdv(dragInfo.DraggedItem.TabItemParent);
                int newIndex = TabControlParent.GetIndexOfTabItemAdv(dragInfo.DragOverItem.TabItemParent);
                newIndex = ValidateIndex(oldIndex, newIndex);
                if (TabControlParent.ItemsSource != null)
                {
                    IList collview = this.TabControlParent.ItemsSource as IList;
                    int removeindex = TabControlParent.GetIndexOfTabItemAdv(dragInfo.DraggedItem.TabItemParent);
                    object o = collview[removeindex];
                    collview.RemoveAt(removeindex);
                    collview.Insert(newIndex, o);
                }
                else
                {
                    TabControlParent.Items.Remove(dragInfo.DraggedItem.TabItemParent);
                    TabControlParent.Items.Insert(newIndex, dragInfo.DraggedItem.TabItemParent);
                }

                TabControlParent.SelectedIndex = TabControlParent.GetIndexOfTabItemAdv(dragInfo.DraggedItem.TabItemParent);
                dragInfo.DraggedItem.ReleaseMouseCapture();
                dragInfo.ClearMarker();
            }

            if (dragInfo.DraggedItem != null)
            {
                dragInfo.DraggedItem.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the view control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void view_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }





        /// <summary>
        /// Validates dragging event.
        /// </summary>
        /// <param name="oldIndex">Old tab item's index.</param>
        /// <param name="newIndex">New tab item's index.</param>
        /// <returns>Value indicating whether dragging is validated.</returns>
        private int ValidateIndex(int oldIndex, int newIndex)
        {
            return (TabStripPlacement == TabStripPlacement.Top || TabStripPlacement == TabStripPlacement.Right)
                            ? ValidateIndex(oldIndex, newIndex, DragMarkerAlignment.RightSide, DragMarkerAlignment.LeftSide)
                            : ValidateIndex(oldIndex, newIndex, DragMarkerAlignment.LeftSide, DragMarkerAlignment.RightSide);
        }

        /// <summary>
        /// Validates dragging event.
        /// </summary>
        /// <param name="oldIndex">Old tab item's index.</param>
        /// <param name="newIndex">New tab item's index.</param>
        /// <param name="firstAlignment">Drag marker first alignment.</param>
        /// <param name="secondAlignment">Drag marker second alignment.</param>
        /// <returns>Value indicating whether dragging is validated.</returns>
        private int ValidateIndex(int oldIndex, int newIndex, DragMarkerAlignment firstAlignment, DragMarkerAlignment secondAlignment)
        {
            if (newIndex < oldIndex)
            {
                if (dragInfo.DragMarkerAlignment == firstAlignment)
                {
                    newIndex++;
                }
            }
            else if (dragInfo.DragMarkerAlignment == secondAlignment)
            {
                newIndex--;
            }

            return newIndex;
        }

        /// <summary>
        /// Validates dragging event.
        /// </summary>
        /// <param name="item">The tab item.</param>
        /// <param name="itemIndex">Tab item's index.</param>
        /// <param name="firstAlignment">Drag marker first alignment.</param>
        /// <param name="secondAlignment">Drag marker second alignment.</param>
        private void ValidateDragInfo(TabItemAdv item, int itemIndex, DragMarkerAlignment firstAlignment, DragMarkerAlignment secondAlignment)
        {
            if (!((itemIndex == scrollInfo.LastTrimmedTabIndex
                && dragInfo.DragMarkerAlignment == secondAlignment)
                || (itemIndex == scrollInfo.FirstTrimmedTabIndex
                && dragInfo.DragMarkerAlignment == firstAlignment)))
            {
                dragInfo.RefreshMarker(item);
            }
        }

        /// <summary>
        /// Looks for the visual ancestor of the specified type.
        /// </summary>        
        /// <param name="startingFrom">Element the search is started from.</param>
        /// <param name="typeAncestor">Desired type of the ancestor.</param>
        /// <returns>Element of the specified type, or null if no ancestors of the specified type were found.</returns>
        public UIElement FindAncestor(UIElement startingFrom, Type typeAncestor)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(startingFrom);

            while (parent != null && !typeAncestor.IsInstanceOfType(parent))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as UIElement;
        }

        /// <summary>
        /// Process scrolling mechanism.
        /// </summary>
        /// <param name="scrollDirection">Scroll direction.</param>
        internal void ProcessScrollInternal(ScrollDirection scrollDirection)
        {
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

        /// <summary>
        /// Selects the tab item.
        /// </summary>
        internal void SelectItemInternal()
        {
            int itemIndex = TabControlParent.SelectedIndex;
            bool needScroll = (itemIndex >= 0 && itemIndex <= scrollInfo.FirstTrimmedTabIndex && scrollInfo.FirstTrimmedTabIndex+1 < this.Children.Count)
                || (itemIndex < this.Children.Count && scrollInfo.LastTrimmedTabIndex-1 < this.Children.Count && itemIndex >= scrollInfo.LastTrimmedTabIndex);

            if (needScroll && TabItemLayout == TabItemLayoutType.SingleLine && TabItemSizeMode == TabItemSizeMode.Normal)
            {
                double endOffset = itemIndex <= scrollInfo.FirstTrimmedTabIndex
                    ? PrepareScrollInfo(itemIndex, scrollInfo.FirstTrimmedTabIndex, 1, scrollInfo.FirstTabTrimmedWidth)
                    : PrepareScrollInfo(scrollInfo.LastTrimmedTabIndex + 1, itemIndex + 1, -1, scrollInfo.LastTabTrimmedWidth);

                this.StartScrolling(scrollInfo.Offset, endOffset);
            }

            if (TabControlParent.SelectedIndex != itemIndex)
            {
                TabControlParent.SelectedIndex = itemIndex;
            }

            this.InvalidateArrange();
        }

        /// <summary>
        /// Prepares the scrolling operation.
        /// </summary>
        /// <param name="start">Start index.</param>
        /// <param name="end">End index.</param>
        /// <param name="inc">Scroll direction.</param>
        /// <param name="boundaryWidth">Tab trimmed width.</param>
        /// <returns>Offset to scroll.</returns>
        private double PrepareScrollInfo(int start, int end, int inc, double boundaryWidth)
        {
            double scrollWidth = 0;

            for (int i = start; i < end; ++i)
            {
                scrollWidth += TabHeaders[i].ActualWidth;
            }

            double endOffset = scrollInfo.Offset + (inc * (scrollWidth + boundaryWidth));
            return endOffset;
        }

        /// <summary>
        /// Complete editing process on the specifies TabHeader.
        /// </summary>
        /// <param name="editableItem">TabHeader which is editing in the current moment.</param>
        /// <param name="applyChanges">Specifies whether editing changes should be applied or no.</param>
        internal void CompleteHeaderEditInternal(TabItemAdv editableItem, bool applyChanges)
        {
            if (editableItem != null && editableItem.TabContent != null)
            {
                if (applyChanges)
                {
                    this.RemoveDelegates(editableItem.TabTextBox);
                    if (editableItem.TabItemParent != null)
                    {
                        editableItem.TabItemParent.Header = editableItem.TabTextBox.Text;
                    }
                }
                else if (!applyChanges)
                {
                    if (editableItem.TabItemParent != null)
                    {
                        editableItem.TabItemParent.Header = editableItem.Tag;
                    }

                    editableItem.Tag = null;
                }

                editableItem.TabTextBox.Visibility = Visibility.Collapsed;
                editableItem.TabContent.Visibility = Visibility.Visible;

                this.RemoveDelegates(editableItem);
                TabControlParent.FireAfterLabelEdit(editableItem.TabContent.Content);
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Launch editing process on the specified TabHeader.
        /// </summary>
        /// <param name="item">TabHeader which should be edited.</param>
        internal void LabelEditStartInternal(TabItemAdv item)
        {
            if (item != null && item.TabContent != null)
            {
                TabControlParent.FireBeforeLabelEdit(item.TabContent.Content);
                item.UpdateLayout();
                editingItem = item;
                this.PrepareEditableHeader(item);
                this.SelectItemInternal();
            }
        }

        /// <summary>
        /// Prepares editing operation.
        /// </summary>
        /// <param name="item">Tab item to be edited.</param>
        private void PrepareEditableHeader(TabItemAdv item)
        {
            item.TabContent.Visibility = Visibility.Collapsed;
            item.TabTextBox.Visibility = Visibility.Visible;
            item.TabTextBox.MinWidth = 15d;
            item.TabTextBox.Width = item.TabContent.ActualWidth;
            item.TabTextBox.KeyDown += new KeyEventHandler(OnTabItemKeyDown);
            item.TabTextBox.LostFocus += new RoutedEventHandler(OnTabItemLostFocus);
            item.TabTextBox.TextChanged += new TextChangedEventHandler(TabTextBoxTextChanged);
            if (item.TabContent != null)
            {
                if (item.TabContent.Content is TextBlock)
                {
                    item.TabTextBox.Text = (item.TabContent.Content as TextBlock).Text;
                }
                else
                {
                    if (item.TabContent.Content != null)
                    {
                        item.TabTextBox.Text = item.TabContent.Content.ToString();
                    }
                }

                item.Tag = item.TabContent.Content;
            }

            item.TabTextBox.SelectAll();
            item.TabTextBox.Focus();
            this.InvalidateMeasure();
        }

        /// <summary>
        /// Returns row nuber that contains selected tab. 
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
                        TabItemAdv element = Children[tabIndex] as TabItemAdv;

                        if (element != null && element.IsSelected)
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
        /// <param name="arrangeWidth">Availiavle width</param>
        /// <returns>List of tabs index for each row.</returns>
        private List<List<int>> CalculateRowDistribution(double arrangeWidth)
        {
            double[] headersSize = GetHeadersSize();
            List<List<int>> rowDistribution = new List<List<int>>(numRows);

            for (int i = 0; i < numRows; i++)
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
                    if (width > 0)
                    {
                        startPoint.X -= this.tabIntersectionFactor;
                    }

                    rowDistribution[rowNumber].Add(i);
                }
                else
                {
                    rowNumber++;
                    startPoint = new Point(0, rowHeight * rowNumber);
                    if(rowDistribution.Count>rowNumber && rowNumber!=-1)
                    rowDistribution[rowNumber].Add(i);

                    startPoint.X += width;
                    if (width > 0)
                    {
                        startPoint.X -= this.tabIntersectionFactor;
                    }
                }
            }

            if (this.KeepTabInFront)
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
        /// <param name="arrangeWidth">Availiable width.</param>
        /// <param name="rowDistribution">Tabs distribution in rows.</param>
        /// <returns>Calculated width.</returns>
        private double[] CalculateHeaderScaling(double arrangeWidth, IEnumerable<List<int>> rowDistribution)
        {
            double[] scalingSizes = new double[numRows];
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

                    rowSum += width - this.tabIntersectionFactor;
                    tabsInRow++;
                }

                scalingSizes[currentRow] = (arrangeWidth - rowSum - this.tabIntersectionFactor) / tabsInRow;
                currentRow++;
                rowSum = 0;
                tabsInRow = 0;
            }

            return scalingSizes;
        }

        /// <summary>
        /// Returns an array with headers size of each tab.
        /// </summary>
        /// <returns>Header size.</returns>
        private double[] GetHeadersSize()
        {
            double[] numArray = new double[Children.Count];
            int index = 0;

            foreach (UIElement element in Children)
            {
                Size desiredSize = GetDesiredSize(element);
                numArray[index] = element.Visibility == Visibility.Collapsed
                    ? 0.0 : desiredSize.Width;
                index++;
            }

            return numArray;
        }

        /// <summary>
        /// Positions tabs in MultiLine mode. 
        /// </summary>
        /// <param name="arrangeSize">Available size to arrange the elements.</param>
        private void ArrangeMultiLine(Size arrangeSize)
        {
            if (arrangeSize.Width > 0)
            {
                double availableWidth = arrangeSize.Width - (ClassicStyleMargin * 2);

                int rowNumber = 0;
                Point startPoint;
                double[] scalingSizes = new double[numRows];
                double[] headersSize = GetHeadersSize();
                List<List<int>> rowDistribution = CalculateRowDistribution(availableWidth);

                if (TabItemLayout == TabItemLayoutType.MultiLineWithFullWidth)
                {
                    scalingSizes = CalculateHeaderScaling(availableWidth, rowDistribution);
                }

                foreach (List<int> row in rowDistribution)
                {
                    startPoint = new Point(0, rowHeight * (numRows - rowNumber - 1));
                    startPoint.X += ClassicStyleMargin;

                    foreach (int tabIndex in row)
                    {
                        FrameworkElement element = Children[tabIndex] as FrameworkElement;

                        startPoint.X += element.Margin.Left;
                        if (element.Visibility != Visibility.Visible)
                        {
                            continue;
                        }

                        double width = headersSize[tabIndex];
                        if (this.IsRightToLeft)
                        {
                            element.Arrange(new Rect(arrangeSize.Width - width - scalingSizes[rowNumber] - startPoint.X, startPoint.Y, width + scalingSizes[rowNumber], rowHeight));
                        }
                        else
                        {
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, width + scalingSizes[rowNumber], rowHeight));
                        }

                        startPoint.X += element.ActualWidth - tabIntersectionFactor + element.Margin.Right;
                    }

                    rowNumber++;
                }
            }
            else
            {
                foreach (UIElement element in Children)
                {
                    element.Arrange(new Rect(0, 0, 0, 0));
                }
            }
        }

        /// <summary>
        /// Positions tabs in MultiLine mode. 
        /// </summary>
        /// <param name="availableWidth">The final area within the TabControlAdv that this element should use to arrange itself and its children.</param>
        private void ArrangeSingleLine(double availableWidth)
        {
            int index = 0;
            double width;
            Point startPoint = new Point(0, 0);
            double[] headersSize = this.GetHeadersSize();
            scrollInfo.LastTrimmedTabIndex = Children.Count + 1;
            scrollInfo.FirstTrimmedTabIndex = -1;

            if (scrollInfo.Offset > 0)
            {
                scrollInfo.Offset = 0;
            }

            if (this.TabControlParent != null)
            {
                this.TabControlParent.UpdateTabStripPlacementAppearance();
            }

            if (availableWidth < scrollInfo.DesiredWidth)
            {
                if (Math.Abs(scrollInfo.Offset) + availableWidth >= scrollInfo.DesiredWidth)
                {
                    scrollInfo.Offset += Math.Abs(scrollInfo.Offset) + availableWidth - scrollInfo.DesiredWidth;
                    if (this.ScrollingPanel != null)
                    {
                        ScrollingPanel.DisableNextPart();
                    }
                }
                else
                {
                    if (this.ScrollingPanel != null)
                    {
                        ScrollingPanel.EnableNextPart();
                    }
                }
            }
            else
            {
                scrollInfo.Offset = 0;
            }

            this.ChangePrevPart();

            if (TabItemSizeMode == TabItemSizeMode.ShrinkToFit
                && availableWidth <= scrollInfo.DesiredWidth)
            {
                width = (availableWidth - (ClassicStyleMargin * 2) + (this.tabIntersectionFactor * (VisibleItemsCount - 1))) / VisibleItemsCount;
                startPoint.X += ClassicStyleMargin;

                foreach (FrameworkElement element in Children)
                {
                    startPoint.X += element.Margin.Left;
                    if (element.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    if (this.IsRightToLeft)
                    {
                        element.Arrange(new Rect(availableWidth - width - startPoint.X, startPoint.Y, width, rowHeight));
                    }
                    else
                    {
                        element.Arrange(new Rect(startPoint.X, startPoint.Y, width, rowHeight));
                    }

                    if (element.Visibility == Visibility.Visible)
                    {
                        startPoint.X += element.ActualWidth - tabIntersectionFactor + element.Margin.Right;
                    }
                }
            }
            else
            {
                startPoint.X = scrollInfo.Offset + ClassicStyleMargin;
                foreach (FrameworkElement element in Children)
                {
                    width = headersSize[index];

                    if (startPoint.X + width < availableWidth)
                    {
                        if (this.IsRightToLeft)
                        {
                            element.Arrange(new Rect(availableWidth - width - startPoint.X, startPoint.Y, width, rowHeight));
                        }
                        else
                        {
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, width, rowHeight));
                        }

                        if ((startPoint.X < 0 && (startPoint.X + width > 0)) || Math.Abs(startPoint.X + width) < 0.01)
                        {
                            scrollInfo.FirstTabTrimmedWidth = Math.Abs(startPoint.X) + ClassicStyleMargin;
                            scrollInfo.FirstTrimmedTabIndex = index;

                            if (scrollInfo.FirstTabTrimmedWidth < 0.01)
                            {
                                double tempWidth = width;
                                int tempIndex = index;
                                while (tempWidth == 0 && tempIndex >= 0)
                                {
                                    tempWidth = headersSize[tempIndex];
                                    tempIndex--;
                                }

                                scrollInfo.FirstTabTrimmedWidth = tempWidth + ClassicStyleMargin;
                            }
                        }

                        startPoint.X += width;
                        if (width > 0)
                        {
                            startPoint.X -= this.tabIntersectionFactor;
                        }

                        if (Math.Abs(startPoint.X) < 0.01 && Math.Abs(startPoint.X) > 0)
                        {
                            startPoint.X = 0;
                        }
                    }
                    else if (startPoint.X < availableWidth)
                    {
                        int addedHeight = 0;
                        if (element is TabItemAdv)
                        {
                            addedHeight = (element as TabItemAdv).IsSelected ? 1 : 0;
                        }

                        if (this.IsRightToLeft)
                        {
                            element.Arrange(new Rect(availableWidth - width - startPoint.X, startPoint.Y, width, rowHeight));
                        }
                        else
                        {
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, availableWidth - startPoint.X, rowHeight + addedHeight));
                        }

                        if (Math.Abs(width - (availableWidth - startPoint.X)) > 0.01)
                        {
                            scrollInfo.LastTabTrimmedWidth = width - (availableWidth - startPoint.X) + ClassicStyleMargin;
                            scrollInfo.LastTrimmedTabIndex = index;
                        }
                        else
                        {
                            scrollInfo.LastTabTrimmedWidth = 0;
                        }

                        startPoint.X += availableWidth - startPoint.X;
                    }
                    else
                    {
                        if (this.IsRightToLeft)
                        {
                            element.Arrange(new Rect(availableWidth - width - startPoint.X, startPoint.Y, 0, 0));
                        }
                        else
                        {
                            element.Arrange(new Rect(startPoint.X, startPoint.Y, 0, 0));
                        }

                        if (scrollInfo.LastTabTrimmedWidth == 0)
                        {
                            scrollInfo.LastTabTrimmedWidth = width + ClassicStyleMargin;
                            scrollInfo.LastTrimmedTabIndex = index;
                        }
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
            if (this.ScrollingPanel != null)
            {
                if (scrollInfo.Offset == 0)
                {
                    ScrollingPanel.DisablePrevPart();
                }
                else
                {
                    ScrollingPanel.EnablePrevPart();
                }
            }
        }

        /// <summary>
        /// Positions tabs in SingleLine mode. 
        /// </summary>
        /// <param name="arrangeSize">The final area within the TabControlAdv that this element should use to arrange itself and its children.</param>
        internal void ArrangeElements(Size arrangeSize)
        {
            double availiableWidth = arrangeSize.Width;

            scrollInfo.PageWidth = availiableWidth;
            scrollInfo.AllTrimmedWidth = scrollInfo.DesiredWidth > availiableWidth ? scrollInfo.DesiredWidth - availiableWidth : 0;
            if (TabItemLayout == TabItemLayoutType.SingleLine)
            {
                if (availiableWidth > 0)
                {
                    ArrangeSingleLine(availiableWidth);
                }
            }
            else
            {
                if (!arrangeSize.IsEmpty)
                {
                    ArrangeMultiLine(arrangeSize);
                }
            }
        }

        /// <summary>
        /// Measures all elements.
        /// </summary>
        /// <param name="availableSize">Available width.</param>
        /// <returns>Total width.</returns>
        internal double MeasureElements(Size availableSize)
        {
            Size size;
            int index = 0;
            double totalWidth = 0d;
            numRows = 1;
            if (availableSize.Width > ClassicStyleMargin * 2)
            {
                availableSize.Width -= ClassicStyleMargin * 2;
            }

            if (TabItemLayout == TabItemLayoutType.SingleLine)
            {
                availableSize.Width += this.tabIntersectionFactor * (this.VisibleItemsCount - 1);
            }

            foreach (UIElement element in Children)
            {
                if (this.TabItemLayout == TabItemLayoutType.SingleLine && this.TabItemSizeMode == TabItemSizeMode.Normal)
                {
                    availableSize.Width = double.PositiveInfinity;
                }

                element.Measure(availableSize);
                size = GetDesiredSize(element);

                if (rowHeight < size.Height)
                {
                    rowHeight = size.Height;
                }

                if (TabItemLayout == TabItemLayoutType.SingleLine)
                {
                    totalWidth += size.Width;
                }
                else
                {
                    if (index == 0)
                    {
                        totalWidth = this.tabIntersectionFactor;
                    }

                    if (((totalWidth + size.Width - this.tabIntersectionFactor) > availableSize.Width) && (index > 0))
                    {
                        totalWidth = size.Width;
                        index = 1;
                        numRows++;
                        continue;
                    }

                    totalWidth += size.Width - this.tabIntersectionFactor;
                    index++;
                }
            }

            if (TabItemLayout == TabItemLayoutType.SingleLine)
            {
                totalWidth -= this.tabIntersectionFactor * (this.VisibleItemsCount - 1);
            }

            totalWidth += ClassicStyleMargin * 2;
            scrollInfo.DesiredWidth = totalWidth;
            return totalWidth;
        }

        /// <summary>
        /// Launch scrollint to the next tab.
        /// </summary>
        private void ScrollToNextTab()
        {
            if (this.IsRightToLeft)
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset + scrollInfo.FirstTabTrimmedWidth);
            }
            else
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.LastTabTrimmedWidth);
            }
        }

        /// <summary>
        /// Launch scrollint to the previous tab.
        /// </summary>
        private void ScrollToPrevTab()
        {
            if (this.IsRightToLeft)
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.LastTabTrimmedWidth);
            }
            else
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset + scrollInfo.FirstTabTrimmedWidth);
            }
        }

        /// <summary>
        /// Launch scrollint to the first tab.
        /// </summary>
        private void ScrollToFirstTab()
        {
            if (this.IsRightToLeft)
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.AllTrimmedWidth);
            }
            else
            {
                StartScrolling(scrollInfo.Offset, 0);
            }
        }

        /// <summary>
        /// Launch scrollint to the last tab.
        /// </summary>
        private void ScrollToLastTab()
        {
            if (scrollInfo.AllTrimmedWidth > 0)
            {
                if (this.IsRightToLeft)
                {
                    StartScrolling(scrollInfo.Offset, 0);
                }
                else
                {
                    StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.AllTrimmedWidth);
                }
            }
        }

        /// <summary>
        /// Launch scrollint to the next page.
        /// </summary>
        private void ScrollToNextPage()
        {
            if (this.IsRightToLeft)
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset + scrollInfo.PageWidth);
            }
            else
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.PageWidth);
            }
        }

        /// <summary>
        /// Launch scrolling to the previous page.
        /// </summary>
        private void ScrollToPrevPage()
        {
            if (this.IsRightToLeft)
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset - scrollInfo.PageWidth);
            }
            else
            {
                StartScrolling(scrollInfo.Offset, scrollInfo.Offset + scrollInfo.PageWidth);
            }
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
                textBox.KeyDown -= new KeyEventHandler(OnTabItemKeyDown);
                textBox.LostFocus -= new RoutedEventHandler(OnTabItemLostFocus);
                textBox.TextChanged -= new TextChangedEventHandler(TabTextBoxTextChanged);
            }
        }

        /// <summary>
        /// Calculates the height of the max row.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        private void CalculateTabsSize(Size availableSize)
        {
            rowHeight = 0;

            foreach (UIElement element in Children)
            {
                if (element.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (this.TabItemLayout != TabItemLayoutType.SingleLine || this.TabItemSizeMode != TabItemSizeMode.ShrinkToFit
                    || this.IsRotatingNeeded)
                {
                    availableSize.Width = double.PositiveInfinity;
                }

                element.Measure(availableSize);
                double currentHeight = GetDesiredSize(element).Height;

                if (rowHeight < currentHeight)
                {
                    rowHeight = currentHeight;
                }
            }
        }

        /// <summary>
        /// Initializes the animation.
        /// </summary>
        private void InitializeAnimation()
        {
            scrollStoryboard = new Storyboard();
            scrollAnimation = new DoubleAnimation();
            Storyboard.SetTarget(scrollAnimation, this);
            Storyboard.SetTargetProperty(scrollAnimation, new PropertyPath("TabLayoutPanel.ScrollOffset"));
            scrollStoryboard.Children.Add(scrollAnimation);
            scrollStoryboard.Completed += new EventHandler(ScrollStoryboardCompleted);
        }

        /// <summary>
        /// Starts the scrolling.
        /// </summary>
        /// <param name="from">Start scrolling position.</param>
        /// <param name="to">End scrolling position.</param>
        private void StartScrolling(double from, double to)
        {
            if (scrollStoryboard == null || scrollAnimation == null)
            {
                InitializeAnimation();
            }

            scrollAnimation.From = from;
            scrollAnimation.To = to;
            scrollAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(this.TabControlParent.ScrollingTime));
            scrollStoryboard.Begin();
        }

        /// <summary>
        /// Returns desired size of the specified element.
        /// </summary>
        /// <param name="element">Element to get the desired size.</param>
        /// <returns>Desired size of the specified element</returns>
        private static Size GetDesiredSize(UIElement element)
        {
            return new Size(element.DesiredSize.Width, element.DesiredSize.Height);
        }
        #endregion
    }

    //internal class CollectionView : QueryableCollectionView
    //{
    //    public CollectionView() : base(IEnumerable source)
    //    { 

    //    }

    //} 
}