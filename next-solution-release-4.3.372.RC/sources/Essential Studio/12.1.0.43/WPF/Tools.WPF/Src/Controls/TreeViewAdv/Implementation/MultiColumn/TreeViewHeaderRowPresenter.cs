// <copyright file="TreeViewHeaderRowPresenter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the TreeView Header Row presenter
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewHeaderRowPresenter : TreeViewRowPresenterBase
    {
        #region Constants

        /// <summary>
        /// Presents actual width
        /// </summary>
        private const string C_nameActualWidth = "ActualWidth";

        #endregion Constants

        #region Members

        /// <summary>
        /// Presents m_headerScrollViewer
        /// </summary>
        private ScrollViewer m_headerScrollViewer;

        /// <summary>
        /// Presents m_mainScrollViewer
        /// </summary>
        private ScrollViewer m_mainScrollViewer;

        /// <summary>
        /// Presents m_parentItemsControl
        /// </summary>
        private ItemsControl m_parentItemsControl;

        /// <summary>
        /// Presents m_isColumnChangedOrCreated
        /// </summary>
        private bool m_isColumnChangedOrCreated;

        /// <summary>
        /// Presents m_paddingHeader
        /// </summary>
        private TreeViewColumnHeader m_paddingHeader;

        /// <summary>
        /// Presents m_dragMarkerAdorner
        /// </summary>
        private TreeViewRowDragMarkerAdorner m_dragMarkerAdorner;

        /// <summary>
        /// Presents m_columnHeaderAdorner
        /// </summary>
        private TreeViewColumnHeaderAdorner m_columnHeaderAdorner;

        /// <summary>
        /// Presents m_draggingSourceHeader
        /// </summary>
        private TreeViewColumnHeader m_draggingSourceHeader = null;

        /// <summary>
        /// Presents m_startPos
        /// </summary>
        private Point m_startPos;

        /// <summary>
        /// Presents m_relativeStartPos
        /// </summary>
        private Point m_relativeStartPos;

        /// <summary>
        /// Presents m_headersPositionList
        /// </summary>
        private List<Rect> m_headersPositionList;

        /// <summary>
        /// Presents m_isHeaderDragging
        /// </summary>
        private bool m_isHeaderDragging = false;

        /// <summary>
        /// Presents m_startColumnIndex
        /// </summary>
        private int m_startColumnIndex = -1;

        /// <summary>
        /// Presents m_desColumnIndex
        /// </summary>
        private int m_desColumnIndex = -1;

        /// <summary>
        /// Presents m_prepareDragging
        /// </summary>
        private bool m_prepareDragging;

        /// <summary>
        /// Presents m_currentPos
        /// </summary>
        private Point m_currentPos;

        internal double headerHeight = 0;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [allows column reorder].
        /// </summary>
        /// <value><c>true</c> if [allows column reorder]; otherwise, <c>false</c>.</value>
        public bool AllowsColumnReorder
        {
            get
            {
                return (bool)GetValue(AllowsColumnReorderProperty);
            }

            set
            {
                SetValue(AllowsColumnReorderProperty, value);
            }
        }

        /// <summary>
        /// Gets the parent tree view.
        /// </summary>
        /// <value>The parent tree view.</value>
        private TreeViewAdv ParentTreeView
        {
            get
            {
                return TreeViewAdv.GetTreeViewFromChildren(this);
            }
        }

        /// <summary>
        /// Gets the drag marker adorner.
        /// </summary>
        /// <value>The drag marker adorner.</value>
        private TreeViewRowDragMarkerAdorner DragMarkerAdorner
        {
            get
            {
                if (m_dragMarkerAdorner == null)
                {
                    m_dragMarkerAdorner = new TreeViewRowDragMarkerAdorner(this);
                }

                return m_dragMarkerAdorner;
            }
        }

        /// <summary>
        /// Gets the column header adorner.
        /// </summary>
        /// <value>The column header adorner.</value>
        private TreeViewColumnHeaderAdorner ColumnHeaderAdorner
        {
            get
            {
                if (m_columnHeaderAdorner == null)
                {
                    m_columnHeaderAdorner = new TreeViewColumnHeaderAdorner(this);
                }

                return m_columnHeaderAdorner;
            }
        }

        /// <summary>
        /// Gets the headers position list.
        /// </summary>
        /// <value>The headers position list.</value>
        private List<Rect> HeadersPositionList
        {
            get
            {
                if (m_headersPositionList == null)
                {
                    m_headersPositionList = new List<Rect>();
                }

                return m_headersPositionList;
            }
        }

        #endregion Properties

        #region Dependency property

        /// <summary>
        /// Represents the Allow Column Reorder property of TreeViewHeader Row
        /// </summary>
        public static readonly DependencyProperty AllowsColumnReorderProperty = DependencyProperty.RegisterAttached("AllowsColumnReorder", typeof(bool), typeof(TreeViewHeaderRowPresenter), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        #endregion Dependency property

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewHeaderRowPresenter"/> class.
        /// </summary>
        public TreeViewHeaderRowPresenter()
        {
            Loaded += new RoutedEventHandler(TreeViewHeaderRowPresenter_Loaded);
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Raises the <see cref="E:ColumnCollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.TreeViewColumnCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal override void OnColumnCollectionChanged(TreeViewColumnCollectionChangedEventArgs e)
        {
            base.OnColumnCollectionChanged(e);
            int visualIndex;
            TreeViewColumn column;
            UIElementCollection internalChildren = base.InternalChildren;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        visualIndex = GetVisualIndex(e.NewStartingIndex);
                        column = (TreeViewColumn)e.NewItems[0];
                        CreateAndInsertHeader(column, visualIndex + 1);
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        RemoveHeader(null, GetVisualIndex(e.OldStartingIndex));
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        visualIndex = this.GetVisualIndex(e.OldStartingIndex);
                        RemoveHeader(null, visualIndex);
                        column = (TreeViewColumn)e.NewItems[0];
                        CreateAndInsertHeader(column, visualIndex);
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        int index = GetVisualIndex(e.OldStartingIndex);
                        int num3 = GetVisualIndex(e.NewStartingIndex);
                        TreeViewColumnHeader element = (TreeViewColumnHeader)internalChildren[index];
                        internalChildren.RemoveAt(index);
                        internalChildren.Insert(num3, element);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        for (int i = 0; i < e.ClearedColumns.Count; i++)
                        {
                            RemoveHeader(null, 1);
                        }

                        break;
                    }
            }

            BuildHeaderLinks();
            m_isColumnChangedOrCreated = true;
        }

        /// <summary>
        /// Called when [column property changed].
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="propertyName">Name of the property.</param>
        internal override void OnColumnPropertyChanged(TreeViewColumn column, string propertyName)
        {
            if (column.ActualIndex >= 0)
            {
                TreeViewColumnHeader element = FindHeaderByColumn(column);

                if (element != null)
                {
                    if (TreeViewColumn.WidthProperty.Name.Equals(propertyName)
                        || C_nameActualWidth.Equals(propertyName))
                    {
                        base.InvalidateMeasure();
                    }
                    else if (TreeViewColumn.HeaderProperty.Name.Equals(propertyName))
                    {
                        UpdateHeaderContent(element);
                    }
                }
            }
        }

        /// <summary>
        /// Makes the parent items control got focus.
        /// </summary>
        internal void MakeParentItemsControlGotFocus()
        {
            if ((m_parentItemsControl != null) && !m_parentItemsControl.IsKeyboardFocusWithin)
            {
                m_parentItemsControl.Focus();
            }
        }

        /// <summary>
        /// Measures the override.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns>Size constraint </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Initialize();
            TreeViewColumnCollection columns = Columns;
            UIElementCollection internalChildren = InternalChildren;
            double width = 0.0;
            double maxWidth = 0.0;
            double height = constraint.Height;
            double maxHeight = 0.0;

            if (columns != null)
            {
                UIElement element = null;
                TreeViewColumn column = null;

                for (int i = 0; i < columns.Count; i++)
                {
                    element = internalChildren[GetVisualIndex(i)];

                    if (element != null)
                    {
                        maxWidth = Math.Max((double)0.0, (double)(constraint.Width - width));
                        column = columns[i];
                        if (column.Width.GridUnitType == GridUnitType.Auto)
                        {
                            System.Drawing.Font fontstyle = new System.Drawing.Font((element as TreeViewColumnHeader).FontFamily.Source.ToString(), (float)((element as TreeViewColumnHeader).FontSize));
                            double textWidthSize = 0.0;
                            if (column.Header is string)
                                textWidthSize = (System.Windows.Forms.TextRenderer.MeasureText(column.Header.ToString(), fontstyle)).Width;

                            var minWidth = (element as TreeViewColumnHeader).Column.MinWidth;
                            if (minWidth == 0.0 && column.Header is string)
                                column.Width = new GridLength(textWidthSize);
                            else
                            {
                                if (column.ColumnHeader != null)
                                {
                                    if (minWidth > column.ColumnHeader.DesiredSize.Width)
                                        maxWidth = minWidth;
                                    else
                                        maxWidth = column.ColumnHeader.DesiredSize.Width;
                                }
                                else
                                    maxWidth = minWidth;
                                column.Width = new GridLength(maxWidth, GridUnitType.Auto);
                            }
                        }
                        else if (column.Width.GridUnitType == GridUnitType.Star)
                        {
                            if (column.ColumnHeader != null && column.ColumnHeader.ParentTreeView != null)
                            {
                                double sumWidth = column.ColumnHeader.ParentTreeView.CompleteRect.Width;
                                double val = sumWidth - columns.totalWidth;
                                double total = 0;
                                for (int j = 0; j < columns.indexs.Count; j++)
                                {
                                    total = total + columns.indexs[j];
                                }
                                double starVal = val / total;
                                maxWidth = starVal * column.Width.Value;
                                column.Width = new GridLength(maxWidth, GridUnitType.Star);
                            }
                        }
                        else
                        {
                            maxWidth = Math.Min(maxWidth, column.Width.Value);
                            maxWidth = Math.Max(maxWidth, column.MinWidth);
                        }
                        switch (column.State)
                        {
                            case ColumnMeasureState.Auto:
                                maxWidth = (maxWidth <= 0) ? double.PositiveInfinity : maxWidth;
                                break;

                            case ColumnMeasureState.Star:

                                break;
                        }
                        element.Measure(new Size(maxWidth, height));
                        if (element is TreeViewColumnHeader && (element as TreeViewColumnHeader).headerContent != null)
                        {
                            column.ColumnHeader = element as TreeViewColumnHeader;
                            (element as TreeViewColumnHeader).headerContent.HorizontalAlignment = column.HeaderContentAlignment;
                        }
                        if (column.Width.GridUnitType == GridUnitType.Auto)
                        {
                            System.Drawing.Font fontstyle = new System.Drawing.Font((element as TreeViewColumnHeader).FontFamily.Source.ToString(), (float)((element as TreeViewColumnHeader).FontSize));
                            double textWidthSize = 0.0;
                            if (column.Header is string)
                                textWidthSize = (System.Windows.Forms.TextRenderer.MeasureText(column.Header.ToString(), fontstyle)).Width;
                            var minWidth = (element as TreeViewColumnHeader).Column.MinWidth;
                            if (minWidth == 0.0 && column.Header is string)
                                width += textWidthSize;
                            else
                            {
                                if (column.ColumnHeader != null)
                                {
                                    if (minWidth > column.ColumnHeader.DesiredSize.Width)
                                        width += minWidth;
                                    else
                                        width += column.ColumnHeader.DesiredSize.Width;
                                }
                                else
                                    width += minWidth;
                            }
                        }
                        else if (column.Width.GridUnitType == GridUnitType.Star)
                        {
                            if (column.ColumnHeader != null && column.ColumnHeader.ParentTreeView != null)
                            {
                                double sumWidth = column.ColumnHeader.ParentTreeView.CompleteRect.Width;
                                double val = sumWidth - columns.totalWidth;
                                double total = 0;
                                for (int j = 0; j < columns.indexs.Count; j++)
                                {
                                    total = total + columns.indexs[j];
                                }
                                double starVal = val / total;

                                width += starVal * column.Width.Value;
                                column.Width = new GridLength(starVal * column.Width.Value, GridUnitType.Star);
                            }
                        }
                        else
                            width += (column.Width.Value == 0.0) ? element.DesiredSize.Width : column.Width.Value;
                        maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
                    }
                }
            }

            m_paddingHeader.Measure(new Size(0.0, height));
            maxHeight = Math.Max(maxHeight, m_paddingHeader.DesiredSize.Height);
            width += 2.0;
            return new Size(width, maxHeight);
        }

        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        /// <returns> Size arrangeSize</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (arrangeSize.Height > 0)
                headerHeight = arrangeSize.Height;
            Initialize();
            Rect rect;
            TreeViewColumnCollection columns = base.Columns;
            UIElementCollection internalChildren = base.InternalChildren;
            double x = 0.0;
            double maxWidth = 0.0;
            double width = arrangeSize.Width;
            HeadersPositionList.Clear();
            double resizeWidth = 0;
            if (columns != null)
            {
                foreach (TreeViewColumn treecolumn in columns)
                {
                    if (treecolumn.State != ColumnMeasureState.Star)
                    {
                        resizeWidth = resizeWidth + treecolumn.Width.Value;
                    }
                }
                if (ParentTreeView != null && ParentTreeView.columnsState.Count > 0 && ParentTreeView.m_virtualizingpanel != null &&
                    ParentTreeView.m_virtualizingpanel.ActualWidth > 0)
                {
                    ParentTreeView.ResizingWidth = (ParentTreeView.m_virtualizingpanel.ActualWidth - resizeWidth) / ParentTreeView.columnsState.Count;
                }
                if (this.m_paddingHeader != null && ParentTreeView.ResizingWidth > 0)
                {
                    (m_paddingHeader as TreeViewColumnHeader).UpdateColumnHeaderWidth(ParentTreeView.ResizingWidth);
                }
                for (int i = 0; i < columns.Count; i++)
                {
                    int index = InternalChildren.Count - 1 - i;
                    UIElement element = internalChildren[index];

                    TreeViewColumn column = columns[i];
                    if (column.Width.GridUnitType == GridUnitType.Auto)
                    {
                        System.Drawing.Font fontstyle = new System.Drawing.Font((element as TreeViewColumnHeader).FontFamily.Source.ToString(), (float)((element as TreeViewColumnHeader).FontSize));
                        double textWidthSize = 0.0;
                        if (column.Header is string)
                            textWidthSize = (System.Windows.Forms.TextRenderer.MeasureText(column.Header.ToString(), fontstyle)).Width;

                        var minWidth = (element as TreeViewColumnHeader).Column.MinWidth;
                        if (minWidth == 0.0 && column.Header is string)
                            column.Width = new GridLength(textWidthSize);
                        else
                        {
                            if (column.ColumnHeader != null)
                            {
                                if (minWidth > column.ColumnHeader.DesiredSize.Width)
                                    maxWidth = minWidth;
                                else
                                    maxWidth = column.ColumnHeader.DesiredSize.Width;
                            }
                            else
                                maxWidth = minWidth;
                            column.Width = new GridLength(maxWidth);
                        }
                    }
                    else if (column.Width.GridUnitType == GridUnitType.Star)
                    {
                        if (column.ColumnHeader != null && column.ColumnHeader.ParentTreeView != null)
                        {
                            double sumWidth = column.ColumnHeader.ParentTreeView.CompleteRect.Width;
                            double val = sumWidth - columns.totalWidth;
                            double total = 0;
                            for (int j = 0; j < columns.indexs.Count; j++)
                            {
                                total = total + columns.indexs[j];
                            }
                            double starVal = val / total;
                            maxWidth = starVal * column.Width.Value;
                            column.Width = new GridLength(maxWidth);
                        }
                    }

                    if (element != null)
                    {
                        maxWidth = Math.Min(width, column.Width.Value);
                        maxWidth = Math.Max(maxWidth, column.MinWidth);
                        if (ParentTreeView.allowArrange && ParentTreeView.AllowUpdate && column.State == ColumnMeasureState.Star && ParentTreeView.ResizingWidth > column.MinWidth)
                            maxWidth = ParentTreeView.ResizingWidth;
                        rect = new Rect(x, 0.0, maxWidth, arrangeSize.Height);
                        if (ParentTreeView.AllowUpdate || ParentTreeView.columnsState.Count <= 0)
                        {
                            element.Arrange(rect);
                            HeadersPositionList.Add(rect);
                        }
                        width -= maxWidth;
                        x += maxWidth;
                    }
                }

                if (m_isColumnChangedOrCreated)
                {
                    for (int j = 0; j < columns.Count; j++)
                    {
                        (internalChildren[InternalChildren.Count - 1 - j] as TreeViewColumnHeader).CheckWidthForPreviousHeaderGripper();
                    }

                    m_paddingHeader.CheckWidthForPreviousHeaderGripper();
                    m_isColumnChangedOrCreated = false;
                }
            }

            rect = new Rect(x, 0.0, Math.Max(width, 0.0), arrangeSize.Height);
            m_paddingHeader.Arrange(rect);
            HeadersPositionList.Add(rect);
            return arrangeSize;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (NeedUpdateVisualTree)
            {
                UIElementCollection internalChildren = InternalChildren;
                TreeViewColumnCollection columns = Columns;
                RenewEvents();

                if (internalChildren.Count == 0)
                {
                    AddPaddingColumnHeader();
                }

                if (columns != null)
                {
                    int i = 1;
                    TreeViewColumn column = null;

                    for (int j = columns.Count - 1; j >= 0; j--)
                    {
                        column = columns[j];
                        CreateAndInsertHeader(column, i++);
                    }
                }

                BuildHeaderLinks();
                NeedUpdateVisualTree = false;
                m_isColumnChangedOrCreated = true;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            TreeViewColumnHeader source = e.Source as TreeViewColumnHeader;

            if (source != null && AllowsColumnReorder)
            {
                HeaderDragPrepare(source, e.GetPosition(this), e.GetPosition(source));
            }

            e.Handled = true;
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                m_prepareDragging = false;

                if (m_isHeaderDragging)
                {
                    HeaderDragFinish(false);
                }
                else
                {
                    SortDirection direction;
                    TreeViewColumnHeader treeviewColumnHeader = null;
                    treeviewColumnHeader = e.Source as TreeViewColumnHeader;
                    if (treeviewColumnHeader != null)
                    {
                        TreeViewColumnCollection columns = base.Columns;
                        foreach (TreeViewColumn column in columns)
                        {
                            if (column != null && column.ColumnHeader != null && column.ColumnHeader.Arrowpath != null)
                            {
                                column.ColumnHeader.Arrowpath.Visibility = Visibility.Collapsed;
                            }
                        }
                        if (treeviewColumnHeader.Arrowpath != null && treeviewColumnHeader.Column != null && treeviewColumnHeader.Column.IsSortingEnabledOnHeaderClick)
                            treeviewColumnHeader.Arrowpath.Visibility = Visibility.Visible;
                        if (treeviewColumnHeader != null && treeviewColumnHeader.Column != null)
                        {
                            if (treeviewColumnHeader.Column.IsSortingEnabledOnHeaderClick)
                            {
                                if (treeviewColumnHeader.SortDirection == SortDirection.None)
                                {
                                    direction = SortDirection.Ascending;
                                }
                                else if (treeviewColumnHeader.SortDirection == SortDirection.Ascending)
                                {
                                    direction = SortDirection.Descending;
                                }
                                else
                                {
                                    direction = SortDirection.Ascending;
                                }
                                if (ParentTreeView != null)
                                {
                                    ParentTreeView.Sorting = direction;
                                    treeviewColumnHeader.SortDirection = direction;
                                    if (ParentTreeView.ItemsSource == null)
                                    {
                                        if (treeviewColumnHeader.Column.DisplayMemberBinding != null)
                                            ParentTreeView.SortingField = "Header." + ((Binding)treeviewColumnHeader.Column.DisplayMemberBinding).Path.Path;
                                    }
                                    else
                                    {
                                        if (treeviewColumnHeader.Column.DisplayMemberBinding != null && ((Binding)treeviewColumnHeader.Column.DisplayMemberBinding).Path != null)
                                            ParentTreeView.SortingField = ((Binding)treeviewColumnHeader.Column.DisplayMemberBinding).Path.Path;
                                        else if (treeviewColumnHeader.Column.SortBy != string.Empty)
                                        {
                                            ParentTreeView.SortingField = treeviewColumnHeader.Column.SortBy;
                                        }
                                        else
                                        {
                                            String columnheader = treeviewColumnHeader.Column.Header.ToString();
                                            for (int i = 0; i < columnheader.Length; i++)
                                            {
                                                if (columnheader[i] == ' ')
                                                {
                                                    columnheader = columnheader.Remove(i, 1);
                                                    i--;
                                                }
                                            }
                                            ParentTreeView.SortingField = columnheader;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                e.Handled = true;
                base.OnMouseLeftButtonUp(e);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((e.LeftButton == MouseButtonState.Pressed) && m_prepareDragging)
            {
                m_currentPos = e.GetPosition(this);
                m_desColumnIndex = FindIndexByPosition(m_currentPos, true);

                if (!m_isHeaderDragging)
                {
                    if (CheckStartHeaderDrag(m_currentPos, m_startPos))
                    {
                        HeaderDragStart(m_currentPos);
                    }
                }
                else
                {
                    ShowDragMarker();
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.LostMouseCapture"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            if ((e.LeftButton == MouseButtonState.Pressed) && m_isHeaderDragging)
            {
                HeaderDragFinish(true);
            }
        }

        /// <summary>
        /// Checks the start header drag.
        /// </summary>
        /// <param name="currentPos">The current pos.</param>
        /// <param name="originalPos">The original pos.</param>
        /// <returns>bool value type </returns>
        private bool CheckStartHeaderDrag(Point currentPos, Point originalPos)
        {
            return DoubleUtil.GreaterThan(Math.Abs((double)(currentPos.X - originalPos.X)), 4.0);
        }

        /// <summary>
        /// Called when [column headers presenter key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void OnColumnHeadersPresenterKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape) && m_isHeaderDragging)
            {
                HeaderDragFinish(true);
            }
        }

        /// <summary>
        /// Headers the drag prepare.
        /// </summary>
        /// <param name="header">The header tree view column header.</param>
        /// <param name="pos">The pos point.</param>
        /// <param name="relativePos">The relative pos.</param>
        private void HeaderDragPrepare(TreeViewColumnHeader header, Point pos, Point relativePos)
        {
            if (header.Role == TreeViewColumnHeaderRole.Normal)
            {
                m_prepareDragging = true;
                m_isHeaderDragging = false;
                m_draggingSourceHeader = header;
                m_columnHeaderAdorner = new TreeViewColumnHeaderAdorner(header);
                m_startPos = pos;
                m_relativeStartPos = relativePos;
                m_startColumnIndex = FindIndexByPosition(pos, false);
            }
        }

        /// <summary>
        /// Headers the drag start.
        /// </summary>
        /// <param name="point">The point.</param>
        private void HeaderDragStart(Point point)
        {
            m_startPos = m_currentPos;
            m_isHeaderDragging = true;

            if (base.Columns != null)
            {
                base.Columns.BlockWrite();
            }
        }

        /// <summary>
        /// Headers the drag finish.
        /// </summary>
        /// <param name="isCancel">if set to <c>true</c> [is cancel].</param>
        private void HeaderDragFinish(bool isCancel)
        {
            m_prepareDragging = false;
            m_isHeaderDragging = false;

            if (base.Columns != null)
            {
                base.Columns.UnblockWrite();
            }

            if (!isCancel)
            {
                int newIndex = (m_startColumnIndex >= m_desColumnIndex) ?
                m_desColumnIndex : (m_desColumnIndex - 1);
                base.Columns.Move(m_startColumnIndex, newIndex);
            }

            HideDragMarker();
        }

        /// <summary>
        /// Adds the padding column header.
        /// </summary>
        private void AddPaddingColumnHeader()
        {
            TreeViewColumnHeader element = new TreeViewColumnHeader();
            element.SetValue(TreeViewColumnHeader.RolePropertyKey, TreeViewColumnHeaderRole.Padding);
            InternalChildren.Add(element);
            m_paddingHeader = element;
        }

        /// <summary>
        /// Creates the and insert header.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="index">The index.</param>
        /// <returns>TreeView ColumnHeader value type</returns>
        private TreeViewColumnHeader CreateAndInsertHeader(TreeViewColumn column, int index)
        {
            TreeViewColumnHeader element = element = new TreeViewColumnHeader();
            element.SetValue(TreeViewColumnHeader.ColumnPropertyKey, column);
            InternalChildren.Insert(index, element);
            UpdateHeaderContent(element);
            return element;
        }

        /// <summary>
        /// Updates the content of the header.
        /// </summary>
        /// <param name="header">The header.</param>
        private void UpdateHeaderContent(TreeViewColumnHeader header)
        {
            if (header != null)
            {
                TreeViewColumn column = header.Column;

                if (column != null)
                {
                    if (column.Header == null)
                    {
                        header.ClearValue(ContentControl.ContentProperty);
                    }
                    else
                    {
                        if (column.Header is FrameworkElement
                            && (column.Header as FrameworkElement).Parent is TreeViewColumnHeader)
                            ((column.Header as FrameworkElement).Parent as TreeViewColumnHeader).Content = null;

                        header.Content = column.Header;
                        header.ContentTemplate = column.ColumnHeaderTemplate;
                    }
                }
            }
        }

        /// <summary>
        /// Builds the header links.
        /// </summary>
        private void BuildHeaderLinks()
        {
            TreeViewColumnHeader prevHeader = null;

            if (Columns != null)
            {
                TreeViewColumnHeader header;

                for (int i = Columns.Count; i >= 0; i--)
                {
                    header = (TreeViewColumnHeader)InternalChildren[i];
                    header.PreviousVisualHeader = prevHeader;
                    prevHeader = header;
                }
            }

            if (m_paddingHeader != null)
            {
                m_paddingHeader.PreviousVisualHeader = prevHeader;
            }
        }

        /// <summary>
        /// Called when [header scroll changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ScrollChangedEventArgs"/> instance containing the event data.</param>
        private void OnHeaderScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((m_mainScrollViewer != null) && (m_headerScrollViewer == e.OriginalSource))
            {
                m_mainScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        /// <summary>
        /// Called when [main scroll changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ScrollChangedEventArgs"/> instance containing the event data.</param>
        private void OnMainScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if ((m_headerScrollViewer != null) && (m_mainScrollViewer == e.OriginalSource))
            {
                m_headerScrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }

        /// <summary>
        /// Renews the events.
        /// </summary>
        private void RenewEvents()
        {
            ScrollViewer viewer = m_headerScrollViewer;
            m_headerScrollViewer = base.Parent as ScrollViewer;

            if (viewer != m_headerScrollViewer)
            {
                if (viewer != null)
                {
                    viewer.ScrollChanged -= new ScrollChangedEventHandler(OnHeaderScrollChanged);
                }

                if (m_headerScrollViewer != null)
                {
                    m_headerScrollViewer.ScrollChanged += new ScrollChangedEventHandler(OnHeaderScrollChanged);
                }
            }

            ScrollViewer viewer2 = m_mainScrollViewer;
            m_mainScrollViewer = base.TemplatedParent as ScrollViewer;

            if (viewer2 != m_mainScrollViewer)
            {
                if (viewer2 != null)
                {
                    viewer2.ScrollChanged -= new ScrollChangedEventHandler(OnMainScrollChanged);
                }

                if (m_mainScrollViewer != null)
                {
                    m_mainScrollViewer.ScrollChanged += new ScrollChangedEventHandler(OnMainScrollChanged);
                }
            }

            ItemsControl control = m_parentItemsControl;
            m_parentItemsControl = TreeViewAdv.GetItemsControlFromChildren(this);

            if (control != m_parentItemsControl)
            {
                if (control != null)
                {
                    control.KeyDown -= new KeyEventHandler(OnColumnHeadersPresenterKeyDown);
                }

                if (m_parentItemsControl != null)
                {
                    m_parentItemsControl.KeyDown += new KeyEventHandler(OnColumnHeadersPresenterKeyDown);
                }
            }
        }

        /// <summary>
        /// Removes the header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="index">The index.</param>
        private void RemoveHeader(TreeViewColumnHeader header, int index)
        {
            if (header != null)
            {
                base.InternalChildren.Remove(header);
            }
            else
            {
                header = (TreeViewColumnHeader)base.InternalChildren[index];
                base.InternalChildren.RemoveAt(index);
            }
        }

        /// <summary>
        /// Finds the header by column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>TreeView ColumnHeader</returns>
        private TreeViewColumnHeader FindHeaderByColumn(TreeViewColumn column)
        {
            TreeViewColumnCollection columns = base.Columns;
            UIElementCollection internalChildren = base.InternalChildren;

            if ((columns != null) && (internalChildren.Count > columns.Count))
            {
                int index = columns.IndexOf(column);

                if (index != -1)
                {
                    int visualIndex = GetVisualIndex(index);
                    TreeViewColumnHeader header = internalChildren[visualIndex] as TreeViewColumnHeader;

                    if (header.Column == column)
                    {
                        return header;
                    }

                    for (int i = 1; i < internalChildren.Count; i++)
                    {
                        header = internalChildren[i] as TreeViewColumnHeader;

                        if ((header != null) && (header.Column == column))
                        {
                            return header;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the index of the visual.
        /// </summary>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns>int value type</returns>
        private int GetVisualIndex(int columnIndex)
        {
            return (base.InternalChildren.Count - 1) - columnIndex;
        }

        /// <summary>
        /// Finds the index by position.
        /// </summary>
        /// <param name="startPos">The start pos.</param>
        /// <param name="findNearestColumn">if set to <c>true</c> [find nearest column].</param>
        /// <returns>int value type</returns>
        private int FindIndexByPosition(Point startPos, bool findNearestColumn)
        {
            int num = -1;
            if (startPos.X < 0.0)
            {
                return 0;
            }

            for (int i = 0; i < HeadersPositionList.Count; i++)
            {
                num++;
                Rect rect = HeadersPositionList[i];
                double x = rect.X;
                double num4 = x + rect.Width;

                if (DoubleUtil.GreaterThanOrClose(startPos.X, x)
                    && DoubleUtil.LessThanOrClose(startPos.X, num4))
                {
                    if (findNearestColumn)
                    {
                        double num5 = (x + num4) * 0.5;

                        if (DoubleUtil.GreaterThanOrClose(startPos.X, num5)
                            && (i != (HeadersPositionList.Count - 1)))
                        {
                            num++;
                        }
                    }

                    return num;
                }
            }

            return num;
        }

        /// <summary>
        /// Shows the drag marker.
        /// </summary>
        private void ShowDragMarker()
        {
            int index = m_desColumnIndex;

            if (DragMarkerAdorner != null && index > -1)
            {
                double offset = 0;

                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        offset += HeadersPositionList[i].Width;
                    }
                }

                DragMarkerAdorner.OffsetX = offset;
                ColumnHeaderAdorner.OffsetX = m_currentPos.X - m_startPos.X;
                TryAddAdorner(DragMarkerAdorner);
                TryAddAdorner(ColumnHeaderAdorner);
            }
        }

        /// <summary>
        /// Hides the drag marker.
        /// </summary>
        private void HideDragMarker()
        {
            if (DragMarkerAdorner != null)
            {
                TryRemoveAdorner(DragMarkerAdorner);
                TryRemoveAdorner(ColumnHeaderAdorner);
            }
        }

        /// <summary>
        /// Tries the add adorner.
        /// </summary>
        /// <param name="adorner">The adorner.</param>
        /// <returns>bool value type</returns>
        private bool TryAddAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null)
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(adorner.AdornedElement);

                if (layer != null && !IsContainsAdorner(layer, adorner))
                {
                    layer.Add(adorner);
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Determines whether [is contains adorner] [the specified layer].
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <param name="adorner">The adorner.</param>
        /// <returns>
        /// <c>true</c> if [is contains adorner] [the specified layer]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsContainsAdorner(AdornerLayer layer, Adorner adorner)
        {
            bool bContains = false;

            if (layer != null && adorner != null)
            {
                Adorner[] arr = layer.GetAdorners(adorner.AdornedElement);

                if (arr != null && arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i] == adorner)
                        {
                            bContains = true;
                            break;
                        }
                    }
                }
            }

            return bContains;
        }

        /// <summary>
        /// Tries the remove adorner.
        /// </summary>
        /// <param name="adorner">The adorner.</param>
        /// <returns>bool value type</returns>
        private bool TryRemoveAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adorner.AdornedElement);

                if (adornerLayer != null)
                {
                    if (adorner.IsVisible)
                    {
                        ret = true;
                    }

                    adornerLayer.Remove(adorner);
                }
            }

            return ret;
        }

        internal Size ArrangeHeader(Size availablesize)
        {
            Size finalsize = new Size(availablesize.Width, headerHeight);
            this.ArrangeOverride(finalsize);
            return availablesize;
        }

        /// <summary>
        /// Handles the Loaded event of the TreeViewHeaderRowPresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TreeViewHeaderRowPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewAdv parent = ParentTreeView;
            if (ParentTreeView != null && !ParentTreeView.rowHeaderPresenterCollection.Contains(this))
                ParentTreeView.rowHeaderPresenterCollection.Add(this);
            if (parent != null)
            {
                Binding binding = new Binding("AllowsColumnReorder");
                binding.Source = parent;
                SetBinding(AllowsColumnReorderProperty, binding);
            }
        }

        #endregion Implementation
    }
}