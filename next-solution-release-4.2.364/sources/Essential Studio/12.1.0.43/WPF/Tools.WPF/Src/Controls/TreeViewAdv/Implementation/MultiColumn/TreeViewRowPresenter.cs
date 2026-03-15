// <copyright file="TreeViewRowPresenter.cs" company="Syncfusion">
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
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class Represents the TreeViewRowPresenter
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewRowPresenter : TreeViewRowPresenterBase
    {
        #region Constants

        /// <summary>
        /// Presents ActualWidth
        /// </summary>
        private const string C_nameActualWidth = "ActualWidth";

        /// <summary>
        /// Presents DisplayMemberBinding
        /// </summary>
        private const string C_nameDisplayMemberBinding = "DisplayMemberBinding";

        /// <summary>
        /// Presents Header
        /// </summary>
        private const string C_nameContentSource = "Header";

        private const string RowPresenter = "PART_RowPresenter";

        internal double presenterHeight = 0;

        internal double FinalWidth = 0;

        internal double actualWidth = 0;

        private bool isresize = true;

        internal Dictionary<int, double> tempWidth = new Dictionary<int, double>();

        internal Size arrangesize;

        #endregion Constants

        #region Dependency property

        /// <summary>
        /// Represents the content Dependency property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            ContentControl.ContentProperty.AddOwner(typeof(TreeViewRowPresenter), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(TreeViewRowPresenter.OnContentChanged)));

        #endregion Dependency property

        #region Properties

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get
            {
                return base.GetValue(ContentProperty);
            }

            set
            {
                base.SetValue(ContentProperty, value);
            }
        }

        internal HorizontalAlignment ContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(ContentAlignmentProperty); }
            set { SetValue(ContentAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentAlignment.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ContentAlignmentProperty =
            DependencyProperty.Register("ContentAlignment", typeof(HorizontalAlignment), typeof(TreeViewRowPresenter), new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(TreeViewRowPresenter.OnContentAlignmentChanged)));

        /// <summary>
        ///
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(TreeViewRowPresenter), new PropertyMetadata(null));

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewRowPresenter"/> class.
        /// </summary>
        public TreeViewRowPresenter()
        {
            Loaded += new RoutedEventHandler(TreeViewRowPresenter_Loaded);
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Handles the Loaded event of the TreeViewRowPresenter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TreeViewRowPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            if (ParentTreeView != null && !ParentTreeView.rowPresenterCollection.Contains(this))
                ParentTreeView.rowPresenterCollection.Add(this);
            InvalidateMeasure();
            Loaded -= new RoutedEventHandler(TreeViewRowPresenter_Loaded);
        }

        internal TreeViewAdv ParentTreeView
        {
            get
            {
                return TreeViewAdv.GetTreeViewFromChildren(this);
            }
        }

        /// <summary>
        /// Measures the override.
        /// </summary>
        /// <param name="constraint">The constraint.</param>
        /// <returns> Size arrange Size </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (Columns == null)
            {
                return new Size();
            }

            UIElementCollection internalChildren = InternalChildren;
            double heightElement = constraint.Height;
            double widthElement = 0.0;
            double width = 0.0;
            double height = 0.0;

            foreach (TreeViewColumn column in Columns)
            {
                UIElement element = internalChildren[column.ActualIndex];

                if (element != null)
                {
                    if (column.ActualIndex == 0)
                    {
                        double offset = GetOffset(element) + Margin.Left + Margin.Right;
                        widthElement = Math.Max(0, column.Width.Value - offset);
                        widthElement = Math.Max(widthElement, column.MinWidth - offset);
                        switch (column.State)
                        {
                            case ColumnMeasureState.Auto:
                                widthElement = (widthElement <= 0) ? double.PositiveInfinity : widthElement;
                                break;

                            case ColumnMeasureState.Star:

                                break;
                        }

                        if (!isresize)
                        {
                            widthElement = (widthElement <= 0) ? double.PositiveInfinity : widthElement;
                            isresize = true;
                        }
                        else
                            widthElement = (widthElement < 0) ? double.PositiveInfinity : widthElement;
                        element.Measure(new Size(widthElement, heightElement));
                        width += (widthElement <= 0 || widthElement == double.PositiveInfinity) ? element.DesiredSize.Width : widthElement;
                        height = Math.Max(height, element.DesiredSize.Height);
                    }
                    else
                    {
                        widthElement = Math.Max(column.Width.Value, column.MinWidth);
                        switch (column.State)
                        {
                            case ColumnMeasureState.Auto:
                                widthElement = (widthElement <= 0) ? double.PositiveInfinity : widthElement;
                                break;

                            case ColumnMeasureState.Star:

                                break;
                        }
                        widthElement = (widthElement < 0) ? double.PositiveInfinity : widthElement;
                        element.Measure(new Size(widthElement, heightElement));
                        width += (widthElement <= 0 || widthElement == double.PositiveInfinity) ? element.DesiredSize.Width : widthElement;
                        height = Math.Max(height, element.DesiredSize.Height);
                    }
                }
            }

            constraint = new Size(width, height);
            return constraint;
        }

        internal Size ArrangeRow(Size availablesize)
        {
            Size finalsize = new Size(availablesize.Width, presenterHeight);
            this.ArrangeOverride(finalsize);
            return availablesize;
        }

        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        /// <returns> Size arrange Size </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            if (arrangeSize.Height > 0)
                presenterHeight = arrangeSize.Height;
            TreeViewColumnCollection columns = Columns;
            double width = 0.0;
            double height = 0.0;
            double resizeWidth = 0;
            double columnWidth = 0;

            if (columns != null && ParentTreeView != null)
            {
                foreach (TreeViewColumn treecolumn in columns)
                {
                    if (treecolumn.State != ColumnMeasureState.Star)
                    {
                        resizeWidth = resizeWidth + treecolumn.Width.Value;
                    }
                    treecolumn.RowPresenter = this;
                    if (treecolumn.Width.Value < treecolumn.MinWidth)
                    {
                        treecolumn.Width = new GridLength(treecolumn.MinWidth);
                        columnWidth = columnWidth + treecolumn.Width.Value;
                    }
                }
                if (ParentTreeView != null && ParentTreeView.columnsState.Count > 0 && ParentTreeView.m_virtualizingpanel != null &&
                    ParentTreeView.m_virtualizingpanel.ActualWidth > 0)
                {
                    ParentTreeView.ResizingWidth = (ParentTreeView.m_virtualizingpanel.ActualWidth - 10 - resizeWidth) / ParentTreeView.columnsState.Count;
                }
                if (ParentTreeView != null)
                    ParentTreeView.rowPresenter = this;
                UIElementCollection internalChildren = InternalChildren;
                double heightElement = arrangeSize.Height;
                double widthElement = 0.0;
                double x = 0.0;
                foreach (TreeViewColumn column in columns)
                {
                    UIElement element = internalChildren[column.ActualIndex];
                    if (element != null)
                    {
                        if ((element as ContentPresenter) != null && ParentTreeView != null)
                        {
                            if (ParentTreeView.MultiColumnEnable)
                                (element as ContentPresenter).HorizontalAlignment = column.ContentAlignment;
                        }
                        if (column.ActualIndex == 0)
                        {
                            double offset = GetOffset(element);

                            widthElement = Math.Max(0, column.Width.Value - offset);

                            widthElement = Math.Max(widthElement, column.MinWidth - offset);

                            widthElement = (widthElement <= 0) ? element.DesiredSize.Width : widthElement;
                            if (ParentTreeView.allowArrange && column.State == ColumnMeasureState.Star)
                            {
                                if (ParentTreeView.ResizingWidth > column.MinWidth)
                                    widthElement = ParentTreeView.ResizingWidth - offset;
                                else
                                    widthElement = Math.Max(widthElement, column.MinWidth - offset);

                                isresize = false;
                            }
                            if (ParentTreeView.allowArrange)
                            {
                                if (ParentTreeView.m_virtualizingpanel != null && ParentTreeView.m_virtualizingpanel.ActualWidth > 0
                                    && this.Width != (ParentTreeView.m_virtualizingpanel.ActualWidth) && ParentTreeView.ResizingWidth > column.Width.Value)
                                {
                                    this.Width = ParentTreeView.m_virtualizingpanel.ActualWidth - offset - 10;
                                    ParentTreeView.AllowUpdate = true;
                                }
                                else if (ParentTreeView.rowPresenterCollection.Count > 0)
                                    ParentTreeView.AllowUpdate = false;
                            }
                            else
                            {
                                if (columnWidth > 0)
                                    this.Width = columnWidth - offset;
                            }
                            element.Arrange(new Rect(x, 0.0, widthElement, arrangeSize.Height));
                            if (ParentTreeView.AllowDynamicResizing && ParentTreeView.MultiColumnEnable)
                            {
                                if (element != null && !ParentTreeView.Flag_Width)
                                {
                                    if (VisualTreeHelper.GetChildrenCount(element) > 0)
                                    {
                                        if (VisualTreeHelper.GetChild(element, 0) as FrameworkElement != null)
                                        {
                                            FrameworkElement elem = VisualTreeHelper.GetChild(element, 0) as FrameworkElement;
                                            int count = (VisualTreeHelper.GetChildrenCount(elem));
                                            if (count > 0)
                                            {
                                                for (int i = 0; i < count; i++)
                                                {
                                                    actualWidth = (VisualTreeHelper.GetChild(elem, i) as FrameworkElement).ActualWidth + actualWidth;
                                                }
                                            }
                                            else
                                                actualWidth = elem.ActualWidth;

                                            FinalWidth = actualWidth + GetOffset(element) + Margin.Left + Margin.Right;
                                            if (ParentTreeView.row.ContainsKey(FinalWidth))
                                            {
                                                if ((this.TemplatedParent as TreeViewItemAdv).ParentTreeViewItem != null && !(this.TemplatedParent as TreeViewItemAdv).ParentTreeViewItem.IsExpanded)
                                                {
                                                    FinalWidth = 0;
                                                }
                                            }
                                            actualWidth = 0;
                                            if (this.TemplatedParent is TreeViewItemAdv && this.TemplatedParent as TreeViewItemAdv != null)
                                            {
                                                TreeViewRowPresenter rowPresenter = null;
                                                TreeViewItemAdv treeitem = null;

                                                if (!(this.TemplatedParent as TreeViewItemAdv).IsExpanded)
                                                {
                                                    double temp = 0;
                                                    for (int i = ParentTreeView.row.Count; i > 0; i--)
                                                    {
                                                        if (ParentTreeView.cell_width.ContainsKey(column.ActualIndex))
                                                            temp = ParentTreeView.cell_width[column.ActualIndex];
                                                        if ((this.TemplatedParent as TreeViewItemAdv).Items.Count > 0)
                                                        {
                                                            if (ParentTreeView.row.ContainsKey(temp))
                                                            {
                                                                if ((((ParentTreeView.row[temp] as TreeViewRowPresenter).TemplatedParent)) != null && ((((ParentTreeView.row[temp] as TreeViewRowPresenter).TemplatedParent) as TreeViewItemAdv).ParentItemsControl as TreeViewItemAdv) == (this.TemplatedParent as TreeViewItemAdv))
                                                                {
                                                                    if (ParentTreeView.cell_width.ContainsKey(column.ActualIndex))
                                                                    {
                                                                        ParentTreeView.cell_width.Remove(column.ActualIndex);
                                                                        rowPresenter = ParentTreeView.row[temp];
                                                                        ParentTreeView.row.Remove(temp);
                                                                        temp = 0;

                                                                        for (int j = ParentTreeView.rowWidth.Count; j > 0; j--)
                                                                        {
                                                                            if ((ParentTreeView.row.ContainsKey(ParentTreeView.rowWidth[j])) == true)
                                                                            {
                                                                                if (ParentTreeView.rowWidth[j] > temp)
                                                                                {
                                                                                    if (ParentTreeView.row.ContainsKey(ParentTreeView.rowWidth[j]))
                                                                                    {
                                                                                        if (((ParentTreeView.row[ParentTreeView.rowWidth[j]] as TreeViewRowPresenter).TemplatedParent) != null && ((ParentTreeView.row[ParentTreeView.rowWidth[j]] as TreeViewRowPresenter).TemplatedParent as TreeViewItemAdv).ParentTreeViewItem != null)
                                                                                        {
                                                                                            if (((ParentTreeView.row[ParentTreeView.rowWidth[j]] as TreeViewRowPresenter).TemplatedParent as TreeViewItemAdv).ParentTreeViewItem.IsExpanded)
                                                                                            {
                                                                                                temp = ParentTreeView.rowWidth[j];
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                ParentTreeView.row.Add(ParentTreeView.rowWidth[j], rowPresenter);
                                                                            }
                                                                        }
                                                                        if (temp > FinalWidth)
                                                                        {
                                                                            FinalWidth = temp;
                                                                            ParentTreeView.cell_width.Add(column.ActualIndex, temp);
                                                                        }
                                                                        else
                                                                            ParentTreeView.cell_width.Add(column.ActualIndex, FinalWidth);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    TreeViewRowPresenter RowPresenter;
                                                    for (int k = 0; k < ((this.TemplatedParent as TreeViewItemAdv).Items.Count); k++)
                                                    {
                                                        treeitem = ((this.TemplatedParent as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(k)) as TreeViewItemAdv;
                                                        if (treeitem != null && (treeitem as TreeViewItemAdv != null)
                                                            && (((this.TemplatedParent as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(k)) as TreeViewItemAdv).HeaderElement != null
                                                            && ((((this.TemplatedParent as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(k)) as TreeViewItemAdv).HeaderElement as ContentControl) != null)
                                                        {
                                                            RowPresenter = (((treeitem.HeaderElement as ContentControl).Content) as TreeViewRowPresenter);
                                                            if (RowPresenter != null)
                                                            {
                                                                RowPresenter.InvalidateArrange();
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            if (FinalWidth > ParentTreeView.cell_width[column.ActualIndex])
                                            {
                                                if (tempWidth.ContainsKey(column.ActualIndex))
                                                {
                                                    tempWidth.Remove(column.ActualIndex);
                                                    tempWidth.Add(column.ActualIndex, FinalWidth);
                                                }
                                                else
                                                    tempWidth.Add(column.ActualIndex, FinalWidth);
                                                if (ParentTreeView.cell_width.ContainsKey(column.ActualIndex))
                                                {
                                                    ParentTreeView.cell_width.Remove(column.ActualIndex);
                                                    ParentTreeView.cell_width.Add(column.ActualIndex, FinalWidth);
                                                }
                                                else
                                                    ParentTreeView.cell_width.Add(column.ActualIndex, FinalWidth);
                                                if (ParentTreeView.row.ContainsKey(FinalWidth))
                                                {
                                                    if (ParentTreeView.row[FinalWidth] == this)
                                                    {
                                                    }
                                                    else
                                                    {
                                                        ParentTreeView.row.Remove(FinalWidth);
                                                        ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                                    }
                                                }
                                                else
                                                    ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                                ParentTreeView.rowWidth.Add(ParentTreeView.rowWidth.Count + 1, FinalWidth);

                                            }
                                            if (ParentTreeView.row.ContainsKey(FinalWidth))
                                            {
                                                if (ParentTreeView.row[FinalWidth] == this)
                                                {
                                                }
                                                else
                                                {
                                                    ParentTreeView.row.Remove(FinalWidth);
                                                    ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                                }
                                            }
                                            else
                                                ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                            ParentTreeView.rowWidth.Add(ParentTreeView.rowWidth.Count + 1, FinalWidth);
                                        }
                                    }
                                }
                            }
                            x += widthElement;
                            width += widthElement;
                            height = Math.Max(height, element.DesiredSize.Height);
                        }
                        else
                        {
                            widthElement = Math.Max(column.Width.Value, column.MinWidth);

                            widthElement = (widthElement <= 0) ? element.DesiredSize.Width : widthElement;

                            if (ParentTreeView.allowArrange && column.State == ColumnMeasureState.Star)
                            {
                                if (ParentTreeView.ResizingWidth > column.MinWidth)
                                    widthElement = ParentTreeView.ResizingWidth;
                                else
                                    widthElement = column.MinWidth;
                            }
                            element.Arrange(new Rect(x, 0.0, widthElement, arrangeSize.Height));
                            if (ParentTreeView.AllowDynamicResizing && ParentTreeView.MultiColumnEnable)
                            {
                                if (element != null && !ParentTreeView.Flag_Width)
                                {
                                    if (VisualTreeHelper.GetChildrenCount(element) > 0)
                                    {
                                        if (VisualTreeHelper.GetChild(element, 0) as FrameworkElement != null)
                                        {
                                            FrameworkElement elem = VisualTreeHelper.GetChild(element, 0) as FrameworkElement;
                                            int count = (VisualTreeHelper.GetChildrenCount(elem));
                                            if (count > 0)
                                            {
                                                for (int i = 0; i < count; i++)
                                                {
                                                    actualWidth = (VisualTreeHelper.GetChild(elem, i) as FrameworkElement).ActualWidth + actualWidth;
                                                }
                                            }
                                            else
                                                actualWidth = elem.ActualWidth;
                                            FinalWidth = actualWidth + Margin.Left + Margin.Right;
                                            actualWidth = 0;

                                            if (FinalWidth > ParentTreeView.cell_width[column.ActualIndex])
                                            {
                                                if (tempWidth.ContainsKey(column.ActualIndex))
                                                {
                                                    tempWidth.Remove(column.ActualIndex);
                                                    tempWidth.Add(column.ActualIndex, FinalWidth);
                                                }
                                                else
                                                    tempWidth.Add(column.ActualIndex, FinalWidth);
                                                if (ParentTreeView.cell_width.ContainsKey(column.ActualIndex))
                                                {
                                                    ParentTreeView.cell_width.Remove(column.ActualIndex);
                                                    ParentTreeView.cell_width.Add(column.ActualIndex, FinalWidth);
                                                }
                                                else
                                                    ParentTreeView.cell_width.Add(column.ActualIndex, FinalWidth);
                                                if (ParentTreeView.row.ContainsKey(FinalWidth))
                                                {
                                                    if (ParentTreeView.row[FinalWidth] == this)
                                                    {
                                                    }
                                                    else
                                                    {
                                                        ParentTreeView.row.Remove(FinalWidth);
                                                        ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                                    }
                                                }
                                                else
                                                    ParentTreeView.row.Add(FinalWidth, this as TreeViewRowPresenter);
                                                ParentTreeView.rowWidth.Add(ParentTreeView.rowWidth.Count + 1, FinalWidth);
                                            }
                                        }
                                    }
                                }
                            }
                            x += widthElement;
                            width += widthElement;
                        }
                    }
                }
            }

            arrangesize = arrangeSize;
            return arrangeSize;
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>double value type</returns>
        private double GetOffset(UIElement element)
        {
            double offset = 0;

            if (element != null)
            {
                TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(element as FrameworkElement);

                if (item != null)
                {
                    offset += item.GetWidthUnheader(true);
                }
            }

            return offset;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (NeedUpdateVisualTree)
            {
                InternalChildren.Clear();
                TreeViewColumnCollection columns = Columns;
                if (columns != null)
                {
                    for (int i = 0; i < columns.ColumnCollection.Count; i++)
                    {
                        InternalChildren.Add(CreateCell(columns.ColumnCollection[i]));
                    }
                }

                NeedUpdateVisualTree = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ColumnCollectionChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.TreeViewColumnCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal override void OnColumnCollectionChanged(TreeViewColumnCollectionChangedEventArgs e)
        {
            OnColumnCollectionChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        InternalChildren.Add(CreateCell((TreeViewColumn)e.NewItems[0]));
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        InternalChildren.RemoveAt(e.ActualIndex);
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        InternalChildren.RemoveAt(e.ActualIndex);
                        InternalChildren.Add(CreateCell((TreeViewColumn)e.NewItems[0]));
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        InternalChildren.Clear();
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        InvalidateArrange();
                        return;
                    }
            }

            InvalidateMeasure();
        }

        /// <summary>
        /// Called when [column property changed].
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="propertyName">Name of the property.</param>
        internal override void OnColumnPropertyChanged(TreeViewColumn column, string propertyName)
        {
            int num;

            if (!C_nameActualWidth.Equals(propertyName)
                && (((num = column.ActualIndex) >= 0)
                && (num < InternalChildren.Count)))
            {
                if (TreeViewColumn.WidthProperty.Name.Equals(propertyName))
                {
                    InvalidateMeasure();
                }
                else if (C_nameDisplayMemberBinding.Equals(propertyName))
                {
                    FrameworkElement element = InternalChildren[num] as FrameworkElement;

                    if (element != null)
                    {
                        BindingBase displayMemberBinding = column.DisplayMemberBinding;

                        if ((displayMemberBinding != null) && (element is TextBlock))
                        {
                            element.SetBinding(TextBlock.TextProperty, displayMemberBinding);
                        }
                        else
                        {
                            RenewCell(num, column);
                        }
                    }
                }
                else
                {
                    ContentPresenter presenter = InternalChildren[num] as ContentPresenter;

                    if (presenter != null)
                    {
                        if (TreeViewColumn.CellTemplateProperty.Name.Equals(propertyName))
                        {
                            DataTemplate cellTemplate = column.CellTemplate;

                            if (cellTemplate == null)
                            {
                                presenter.ClearValue(ContentControl.ContentTemplateProperty);
                            }
                            else
                            {
                                presenter.ContentTemplate = cellTemplate;
                            }
                        }
                        else if (TreeViewColumn.CellTemplateSelectorProperty.Name.Equals(propertyName))
                        {
                            DataTemplateSelector cellTemplateSelector = column.CellTemplateSelector;

                            if (cellTemplateSelector == null)
                            {
                                presenter.ClearValue(ContentControl.ContentTemplateSelectorProperty);
                            }
                            else
                            {
                                presenter.ContentTemplateSelector = cellTemplateSelector;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the cell.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns> Content Presenter of column</returns>
        private ContentPresenter CreateCell(TreeViewColumn column)
        {
            BindingBase displayMemberBinding = column.DisplayMemberBinding;
            ContentPresenter presenter = new ContentPresenter();
            column.RowPresenter = this;
            presenter.VerticalAlignment = VerticalAlignment.Center;
            presenter.HorizontalAlignment = column.ContentAlignment;
            presenter.ContentSource = C_nameContentSource;
            presenter.Content = Content;
            presenter.DataContext = Content;

            if (displayMemberBinding != null)
            {
                presenter.SetBinding(ContentPresenter.ContentProperty, CreateBinding(displayMemberBinding));
            }

            if (ContentTemplate != null)
            {
                presenter.ContentTemplate = ContentTemplate;
            }

            DataTemplate cellTemplate = column.CellTemplate;

            if (cellTemplate != null)
            {
                presenter.ContentTemplate = cellTemplate;
            }

            DataTemplateSelector cellTemplateSelector = column.CellTemplateSelector;

            if (cellTemplateSelector != null)
            {
                presenter.ContentTemplateSelector = cellTemplateSelector;
            }

            ContentControl templatedParent = base.TemplatedParent as ContentControl;

            if (templatedParent != null)
            {
                presenter.VerticalAlignment = templatedParent.VerticalContentAlignment;
                presenter.HorizontalAlignment = templatedParent.HorizontalContentAlignment;
            }

            return presenter;
        }

        /// <summary>
        /// Creates the binding.
        /// </summary>
        /// <param name="baseBinding">The base binding.</param>
        /// <returns> binding of base</returns>
        private Binding CreateBinding(BindingBase baseBinding)
        {
            Binding binding = null;

            if (baseBinding != null && baseBinding is Binding)
            {
                Binding b = baseBinding as Binding;
                binding = new Binding();
                binding.Path = b.Path;
                binding.Mode = b.Mode;
                binding.Converter = b.Converter;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                if (b.RelativeSource != null)
                {
                    binding.RelativeSource = b.RelativeSource;
                }
                else
                {
                    binding.Source = Content;
                }

                if (b.ElementName != null)
                {
                    binding.ElementName = b.ElementName;
                }
            }

            return binding;
        }

        /// <summary>
        /// Called when [contentAlignment changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContentAlignmentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            TreeViewRowPresenter c = (TreeViewRowPresenter)obj;
            if (c != null)
                c.OnContentAlignmentChanged(args);
        }

        protected void OnContentAlignmentChanged(DependencyPropertyChangedEventArgs args)
        {
            foreach (TreeViewRowPresenter row in ParentTreeView.rowPresenterCollection)
            {
                row.ArrangeOverride(arrangesize);
            }
        }

        /// <summary>
        /// Called when [content changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Type type = (e.OldValue != null) ? e.OldValue.GetType() : null;
            Type type2 = (e.NewValue != null) ? e.NewValue.GetType() : null;

            if (type != type2)
            {
                ((TreeViewRowPresenter)d).NeedUpdateVisualTree = true;
            }
            else
            {
                ((TreeViewRowPresenter)d).UpdateCells();
            }
        }

        /// <summary>
        /// Renews the cell.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="column">The column.</param>
        private void RenewCell(int index, TreeViewColumn column)
        {
            InternalChildren.RemoveAt(index);
            InternalChildren.Insert(index, CreateCell(column));
        }

        /// <summary>
        /// Updates the cells.
        /// </summary>
        private void UpdateCells()
        {
            UIElementCollection internalChildren = InternalChildren;
            ContentControl templatedParent = TemplatedParent as ContentControl;

            for (int i = 0; i < internalChildren.Count; i++)
            {
                FrameworkElement element = (FrameworkElement)internalChildren[i];
                ContentPresenter presenter = element as ContentPresenter;

                if (presenter != null)
                {
                    presenter.Content = Content;
                }
                else
                {
                    element.DataContext = Content;
                }

                if (templatedParent != null)
                {
                    element.VerticalAlignment = templatedParent.VerticalContentAlignment;
                    element.HorizontalAlignment = templatedParent.HorizontalContentAlignment;
                }
            }
        }

        #endregion Implementation
    }
}