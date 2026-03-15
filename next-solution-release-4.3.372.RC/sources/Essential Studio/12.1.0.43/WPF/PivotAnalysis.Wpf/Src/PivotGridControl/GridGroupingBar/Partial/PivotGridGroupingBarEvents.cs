#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media;
using Syncfusion.PivotAnalysis.Base;
using System.ComponentModel;
using System.Reflection;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Xml.Serialization;
using Syncfusion.Windows.Controls.Cells;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    public partial class PivotGridGroupingBar
    {

        #region [ Members ]

        private Dictionary<PivotItem, IComparer> _savedComparers;

        #endregion

        #region [ Events ]

        internal event EventHandler OnFilterItemChanged;

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsWindowLoaded)
            {
                this.IsWindowLoaded = true;
                if (this.GridControl != null && this.GridControl.ShowFieldList)
                    this.ShowFieldListExecuted(this, new RoutedEventArgs() as ExecutedRoutedEventArgs);
            }
        }

        /// <summary>
        /// Handles the ResizingColumns event of the InternalGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridResizingColumnsEventArgs"/> instance containing the event data.</param>
        void InternalGrid_ResizingColumns(object sender, Syncfusion.Windows.Controls.Grid.GridResizingColumnsEventArgs args)
        {
            if (this.RowHeaderArea != null)
            {
                for (int i = 0; i < this.Engine.PivotRows.Count; i++)
                {
                    ListBoxItem item = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if(item != null)
                    item.Width = (this.GridControl.InternalGrid.Model.ColumnWidths[i] >= 2) ? this.GridControl.InternalGrid.Model.ColumnWidths[i] - 2 : 0;
                }
            }
            this.CalculateComputationInfoWidth();
        }

        void PivotColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.ColumnHeaderArea.Items.Count > 0 && this.ColumnHeaderArea.Items[0] is TextBlock)
            {
                this.ColumnHeaderArea.Items.Clear();
                //this.ColumnHeaderArea.Items.Clear();
                this.ColumnHeaderArea.DataContext = this.GridControl.PivotColumns;
                this.ColumnHeaderArea.ItemsSource = this.GridControl.PivotColumns;

                ResourceDictionary resource = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };

                this.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;
            }
        }

        void GridControl_Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.FilterHeaderArea.Items.Count > 0 && this.FilterHeaderArea.Items[0] is TextBlock)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems.Count > 0)
                    {
                        System.Collections.ObjectModel.ObservableCollection<FilterItemsCollection> filtersCollection = new System.Collections.ObjectModel.ObservableCollection<FilterItemsCollection>();
                        foreach (var item in e.NewItems)
                        {
                            if ((item is FilterExpression) && (item as FilterExpression).DimensionHeader != null && (item as FilterExpression).Tag is FilterItemsCollection)
                            {
                                filtersCollection.Add((FilterItemsCollection)(item as FilterExpression).Tag);
                            }
                        }
                        this.Filters = filtersCollection;
                    }
                    
                }

                if (this.Filters.Count > 0)
                {
                    this.FilterHeaderArea.Items.Clear();
                    this.FilterHeaderArea.DataContext = this.Filters;
                    this.FilterHeaderArea.ItemsSource = this.Filters;

                    ResourceDictionary resource = new ResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                    };

                    this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                }
            }
        }


        void PivotGridGroupingBar_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this.FieldList != null && !this.FieldList.IsDrag)
            {
                this.FieldList.RemoveAdorner(AdornerLayer.GetAdornerLayer(this.FieldList.PivotItemPanel));
            }
        }

        void PivotGridGroupingBar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            this.Focus();
        }

        void PivotItemList_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PivotItemToggleButton = Common.GetParentElement<ToggleButton>(((DependencyObject)e.OriginalSource));

            if (PivotItemToggleButton != null)
            {
                CurrentItemsControl = sender as PivotGroupingItemsControl;
                int index = ((IList)CurrentItemsControl.ItemsSource).IndexOf(PivotItemToggleButton.Tag);

                if (index == 0 && CurrentItemsControl.Items.Count > 1)
                {
                    MenuItem item = PivotItemToggleButton.ContextMenu.Items[1] as MenuItem;
                    ((MenuItem)item.Items[0]).IsEnabled = false;
                    ((MenuItem)item.Items[1]).IsEnabled = false;
                    ((MenuItem)item.Items[2]).IsEnabled = true;
                    ((MenuItem)item.Items[3]).IsEnabled = true;
                    ((MenuItem)item.Items[4]).IsEnabled = true;
                    ((MenuItem)item.Items[5]).IsEnabled = true;
                }
                else if (index == CurrentItemsControl.Items.Count - 1 && CurrentItemsControl.Items.Count > 1)
                {
                    MenuItem item = PivotItemToggleButton.ContextMenu.Items[1] as MenuItem;
                    ((MenuItem)item.Items[0]).IsEnabled = true;
                    ((MenuItem)item.Items[1]).IsEnabled = true;
                    ((MenuItem)item.Items[2]).IsEnabled = false;
                    ((MenuItem)item.Items[3]).IsEnabled = false;
                    ((MenuItem)item.Items[4]).IsEnabled = true;
                    ((MenuItem)item.Items[5]).IsEnabled = true;
                }
                else if (index == 0 && CurrentItemsControl.Items.Count <= 1)
                {
                    MenuItem item = PivotItemToggleButton.ContextMenu.Items[1] as MenuItem;
                    ((MenuItem)item.Items[0]).IsEnabled = false;
                    ((MenuItem)item.Items[1]).IsEnabled = false;
                    ((MenuItem)item.Items[2]).IsEnabled = false;
                    ((MenuItem)item.Items[3]).IsEnabled = false;
                    ((MenuItem)item.Items[4]).IsEnabled = false;
                    ((MenuItem)item.Items[5]).IsEnabled = false;
                }
                else
                {
                    MenuItem item = PivotItemToggleButton.ContextMenu.Items[1] as MenuItem;
                    ((MenuItem)item.Items[0]).IsEnabled = true;
                    ((MenuItem)item.Items[1]).IsEnabled = true;
                    ((MenuItem)item.Items[2]).IsEnabled = true;
                    ((MenuItem)item.Items[3]).IsEnabled = true;
                    ((MenuItem)item.Items[4]).IsEnabled = true;
                    ((MenuItem)item.Items[5]).IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the PivotItemList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void PivotItemList_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBox source = (ListBox)sender;

            if (source.Name == this.DataHeaderArea.Name)
            {
                if (this.GridControl.PivotCalculations.Count == 1)
                {
                    return;
                }
            }

            DragSource = source;
            m_StartPoint = e.GetPosition(DragSource);
            if (!(e.OriginalSource is System.Windows.Shapes.Path || e.OriginalSource is Image))
                CanDrop = true;

            ToggleButton tgButton = Common.GetParentElement<ToggleButton>(e.OriginalSource as DependencyObject);

            if (tgButton != null)
            {
                DraggedItem = tgButton.Tag;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the PivotItemList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void PivotItemList_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ListBox source = (ListBox)sender;
            PivotGroupingItemsControl control = (PivotGroupingItemsControl)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition((IInputElement)source);
                if (Math.Abs(position.X - m_StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - m_StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    object _draggedData = GetDataFromListBox(DragSource, e.GetPosition(source));

                    if (_draggedData != null)
                    {

                        if (_draggedData != null && CanDrop)
                        {
                            CanDrop = false;
                            DragDrop.DoDragDrop(source, _draggedData, DragDropEffects.Move);
                        }
                    }
                }
            }
        }

        internal bool shouldShowBouncingArrows = true;
        /// <summary>
        /// Handles the PreviewDragOver event of the PivotItemList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void PivotItemList_PreviewDragOver(object sender, DragEventArgs e)
        {
            ListBox dropTarget = (ListBox)sender;

            string FormatText = null;
            TextBlock txtBlock = null;

            Point hitPoint = e.GetPosition(dropTarget);
            HitTestResult result = null; 
            if (VisualTreeHelper.HitTest(dropTarget, hitPoint).VisualHit is ScrollViewer)
                hitPoint.Y = hitPoint.Y + 20;
            result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ToggleButton button = result != null ? Common.GetParentElement<ToggleButton>(result.VisualHit) : null;
            PivotGroupingItemsControl control = result!= null ? Common.GetParentElement<PivotGroupingItemsControl>(result.VisualHit): null;

            if (DraggedItem is PivotComputationInfo)
            {
                PivotComputationInfo item = DraggedItem as PivotComputationInfo;
                if (!item.AllowRunTimeGroupByField)
                {
                    shouldShowBouncingArrows = false;
                }
            }
            else if (DraggedItem is PivotItem)
            {
                PivotItem item = DraggedItem as PivotItem;
                if (!item.AllowRunTimeGroupByField)
                {
                    shouldShowBouncingArrows = false;
                }
            }

            if (control != null)
            {
                if (control.Items.Count > 0 && control.Items[0] is TextBlock)
                {
                    txtBlock = control.Items[0] as TextBlock;
                }

                else if (button == null)
                {
                    button = GetButtonElement(control, dropTarget, hitPoint);
                }
            }

            if (txtBlock != null)
            {
                if (e.Data.GetDataPresent(typeof(PivotItem)) || e.Data.GetDataPresent(typeof(PivotComputationInfo)) || e.Data.GetDataPresent(typeof(FilterItemsCollection)))
                {
                    Point RelativePosition = e.GetPosition(txtBlock);
                    this.IndicatorPopup.PlacementTarget = txtBlock;
                    this.IndicatorPopup.PlacementRectangle = new Rect(6, -16, txtBlock.ActualWidth - 5, 0);
                    this.IndicatorPopup.Placement = PlacementMode.Left;
                    this.IndicatorPopup.PlacementTarget = txtBlock;
                    this.ShowPlacementIndicatorPopup();
                }
            }

            else if (button != null)
            {
                if (e.Data.GetDataPresent(typeof(PivotItem)) || e.Data.GetDataPresent(typeof(PivotComputationInfo)) || e.Data.GetDataPresent(typeof(FilterItemsCollection)))
                {
                    if (m_IndicatorPlacementSource != button)
                    {
                        m_IndicatorPlacementSource = button;
                        this.ClosePlacementIndicatorPopup();
                    }

                    Point buttonRelativePosition = e.GetPosition(button);
                    this.IndicatorPopup.PlacementTarget = button;
                    this.IndicatorPopup.PlacementRectangle = new Rect(6, -10, button.ActualWidth - 12, 0);
                    if (button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X)
                    {
                        if (!SystemParameters.IsMenuDropRightAligned)
                            this.IndicatorPopup.Placement = PlacementMode.Left;
                        else
                            this.IndicatorPopup.Placement = PlacementMode.Right;
                    }
                    else
                    {
                        if (!SystemParameters.IsMenuDropRightAligned)
                            this.IndicatorPopup.Placement = PlacementMode.Right;
                        else
                            this.IndicatorPopup.Placement = PlacementMode.Left;
                    }
                    this.IndicatorPopup.PlacementTarget = button;
                    this.ShowPlacementIndicatorPopup();
                }
            }

            //bool IsComputationInfo = false;

            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                FormatText = ((PivotItem)e.Data.GetData(typeof(PivotItem))).FieldHeader;
            }

            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                FormatText = ((PivotComputationInfo)e.Data.GetData(typeof(PivotComputationInfo))).FieldHeader;
            }

            else
            {
                if (e.Data.GetData(typeof(FilterItemsCollection)) != null)
                {
                    FormatText = ((FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection))).DisplayHeader;
                }
            }

            if (FormatText != null)
            {
                if (this.FieldList != null)
                {
                    this.FieldList.FormatText = FormatText;
                }

                // Point point = e.GetPosition(dropTarget);
                // point = dropTarget.TranslatePoint(point, this);
                // Point point =  e.GetPosition(this);
                this.AddAdorners(e.GetPosition(this), FormatText, DraggedItem);
            }
        }

        /// <summary>
        /// Handles the PreviewDrop event of the FilterBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void FilterBar_PreviewDrop(object sender, DragEventArgs e)
        {
            this.RemoveAdorners();
            shouldShowBouncingArrows = true;
            ListBox dropTarget = (ListBox)sender;

            Point hitPoint = e.GetPosition(dropTarget);
            HitTestResult result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ToggleButton button = Common.GetParentElement<ToggleButton>(result.VisualHit);
            this.ClosePlacementIndicatorPopup();

            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                PivotItem pivotItem = (PivotItem)e.Data.GetData(typeof(PivotItem));
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    e.Handled = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotItem.FieldHeader).FirstOrDefault() == null)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
                    if (descriptor != null)
                    {
                        //this.GridControl.IgnoreRefesh = true;

                        FilterItemsCollection filterList = null;
                        FilterExpression exp = null;

                        exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == descriptor.Name);
                        if (exp != null)
                        {
                            if (this.GridControl.EnableHyperlinkOnMouseOver)
                                filterList = exp.Tag as FilterItemsCollection;
                            else
                                filterList = exp.Tag as FilterItemsCollection;
                        }
                        else
                            filterList = GetFilterItem(descriptor);

                        filterList.DisplayHeader = pivotItem.FieldHeader;
                        filterList.Format = pivotItem.Format;
                        filterList.AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField;
                        filterList.ShowSubTotal = pivotItem.ShowSubTotal;
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                            {
                                insertIndex++;
                            }
                            if (!this.Filters.Any(x => x.Name == filterList.Name))
                            {
                                if (insertIndex >= 0)
                                {
                                    this.Filters.Insert(insertIndex, filterList);
                                    this.GridControl.FilterItems = this.Filters;
                                }
                                else if (insertIndex < 0)
                                {
                                    this.Filters.Insert(0, filterList);
                                    this.GridControl.FilterItems = this.Filters;
                                }

                            }
                        }
                        else
                        {
                            this.Filters.Add(filterList);
                            this.GridControl.FilterItems = this.Filters;
                        }

                        ((IList)DragSource.ItemsSource).Remove(pivotItem);
                        this.AddEmptyItem(DragSource);
                        this.ApplyTemplateToGroupingControl(this.FilterHeaderArea, null);
                        if (OnFilterItemChanged != null)
                            OnFilterItemChanged(this, e);
                        //this.FilterHeaderArea.ItemsSource = null;
                        //this.FilterHeaderArea.Items.Clear();
                        //this.FilterHeaderArea.ItemsSource = this.Filters;

                        //ResourceDictionary resource = new ResourceDictionary()
                        //{
                        //    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                        //};

                        //this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                PivotComputationInfo pivotComputationInfo = (PivotComputationInfo)e.Data.GetData(typeof(PivotComputationInfo));
                if (!pivotComputationInfo.AllowRunTimeGroupByField)
                {
                    e.Handled = true;
                    return;
                }
                if (this.Filters.Where(f => f.Name == pivotComputationInfo.FieldName).FirstOrDefault() == null)
                {
                    PropertyDescriptor descriptor = GetPropertyDescriptor(pivotComputationInfo.FieldName);
                    if (descriptor != null)
                    {
                        if (((IList)DragSource.ItemsSource).Count > 1)
                        {
                            FilterItemsCollection filterList = GetFilterItem(descriptor);
                            filterList.DisplayHeader = pivotComputationInfo.FieldHeader;
                            filterList.Format = pivotComputationInfo.Format;
                            filterList.AllowRunTimeGroupByField = pivotComputationInfo.AllowRunTimeGroupByField;
                            if (button != null)
                            {
                                int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                                Point buttonRelativePosition = e.GetPosition(button);
                                if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                                {
                                    insertIndex++;
                                }

                                if (insertIndex >= 0)
                                {
                                    this.Filters.Insert(insertIndex, filterList);
                                    this.GridControl.FilterItems = this.Filters;
                                }
                                else if (insertIndex < 0)
                                {
                                    this.Filters.Insert(0, filterList);
                                    this.GridControl.FilterItems = this.Filters;
                                }
                            }
                            else
                            {
                                this.Filters.Add(filterList);
                                this.GridControl.FilterItems = this.Filters;
                            }

                            ((IList)DragSource.ItemsSource).Remove(pivotComputationInfo);
                            this.ApplyTemplateToGroupingControl(this.FilterHeaderArea, null);
                            if (OnFilterItemChanged != null)
                                OnFilterItemChanged(this, e);
                            //this.FilterHeaderArea.ItemsSource = null;
                            //this.FilterHeaderArea.Items.Clear();
                            //this.FilterHeaderArea.ItemsSource = this.Filters;

                            //ResourceDictionary resource = new ResourceDictionary()
                            //{
                            //    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                            //};

                            //this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                        }
                    }
                }
            }

            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection));

                if (filterItem != null)
                {
                    if (DragSource == dropTarget)
                    {
                        if (button != null)
                        {
                            if (filterItem == button.Tag)
                            {
                                return;
                            }
                        }
                    }

                    ((IList)DragSource.ItemsSource).Remove(filterItem);

                    if (button != null)
                    {
                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                        {
                            insertIndex++;
                        }

                        if (insertIndex >= 0)
                        {
                            this.Filters.Insert(insertIndex, filterItem);
                        }
                        else if (insertIndex < 0)
                            this.Filters.Insert(0, filterItem);
                    }
                    else
                    {
                        this.Filters.Add(filterItem);
                    }

                    this.ApplyTemplateToGroupingControl(this.FilterHeaderArea, null);
                    if (OnFilterItemChanged != null)
                        OnFilterItemChanged(this, e);
                    //this.FilterHeaderArea.ItemsSource = null;
                    //this.FilterHeaderArea.Items.Clear();
                    //this.FilterHeaderArea.ItemsSource = this.Filters;

                    //ResourceDictionary resource = new ResourceDictionary()
                    //{
                    //    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                    //};

                    //this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                }
            }

            this.GridControl.GridScrollViewer.ScrollToHorizontalOffset(0);
        }

        /// <summary>
        /// Handles the PreviewDrop event of the PivotItemList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void PivotItemList_PreviewDrop(object sender, DragEventArgs e)
        {
            this.RemoveAdorners();
            shouldShowBouncingArrows = true;
            ListBox dropTarget = (ListBox)sender;
            IndicatorPopup.IsOpen = false;

            Point hitPoint = e.GetPosition(dropTarget);
            HitTestResult result = null;
            if (VisualTreeHelper.HitTest(dropTarget, hitPoint).VisualHit is ScrollViewer)
                hitPoint.Y = hitPoint.Y + 20;
            result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
            ToggleButton button = result != null ? Common.GetParentElement<ToggleButton>(result.VisualHit) : null;
            if (button == null)
            {
                PivotGroupingItemsControl control = result != null ? Common.GetParentElement<PivotGroupingItemsControl>(result.VisualHit) : null;
                if (control != null)
                {
                    button = GetButtonElement(control, dropTarget, hitPoint);
                }
            }

            if (e.Data.GetDataPresent(typeof(PivotItem)))
            {
                object data = e.Data.GetData(typeof(PivotItem));
                if (!((PivotItem)data).AllowRunTimeGroupByField)
                {
                    e.Handled = true;
                    return;
                }
                if (DragSource == dropTarget)
                {
                    if (this.BreakFunction(button, data, DragSource, dropTarget))
                    {
                        return;
                    }
                }

                this.GridControl.IgnoreRefesh = true;
                ((IList)DragSource.ItemsSource).Remove(data);

                this.AddEmptyItem(DragSource);

                if (dropTarget == this.DataHeaderArea)
                {
                    PivotItem pivotItem = (PivotItem)data;

                    if (button != null)
                    {
                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                        {
                            insertIndex++;
                        }
                        if (insertIndex >= 0)
                        {
                            this.InsertCalculationInfo(insertIndex, pivotItem);
                        }
                        else if (insertIndex < 0)
                            this.InsertCalculationInfo(0, pivotItem);
                    }
                    else
                    {
                        this.AddComputationInfo(pivotItem);
                    }
                }
                else
                {
                    //if (((IList)dropTarget.ItemsSource) == null)
                    {
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);
                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                            {
                                insertIndex++;
                            }
                            if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(insertIndex, data); //.Add(data);
                            }
                            else if (insertIndex < 0)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(0, data);
                            }
                        }
                        else
                        {
                            if (dropTarget.ItemsSource != null)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Add(data);
                            }
                            else
                            {
                                this.GridControl.IgnoreRefesh = false;
                                this.ApplyTemplateToGroupingControl(dropTarget, data);
                            }
                        }
                    }
                }
            }

            else if (e.Data.GetDataPresent(typeof(PivotComputationInfo)))
            {
                if (((IList)DragSource.ItemsSource).Count > 1)
                {
                    object data = e.Data.GetData(typeof(PivotComputationInfo));
                    PivotComputationInfo compInfo = (PivotComputationInfo)data;
                    if (!compInfo.AllowRunTimeGroupByField)
                    {
                        e.Handled = true;
                        return;
                    }
                    if (DragSource == dropTarget)
                    {
                        if (this.BreakFunction(button, data, DragSource, dropTarget))
                        {
                            return;
                        }
                    }

                    this.GridControl.IgnoreRefesh = true;
                    ((IList)DragSource.ItemsSource).Remove(data);

                    if (dropTarget == this.DataHeaderArea)
                    {
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);

                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                            {
                                insertIndex++;
                            }

                            if (insertIndex >= 0)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(insertIndex, data); //.Add(data);
                            }
                            else if (insertIndex < 0)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(0, data);
                            }
                        }
                        else
                        {
                            this.GridControl.IgnoreRefesh = false;
                            ((IList)dropTarget.ItemsSource).Add(data);
                        }
                    }
                    else
                    {
                        if (button != null)
                        {
                            int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                            Point buttonRelativePosition = e.GetPosition(button);

                            if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                            {
                                insertIndex++;
                            }

                            data = new PivotItem() { FieldHeader = compInfo.FieldHeader, SummaryType=compInfo.SummaryType,Summary=compInfo.Summary ,FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField };

                            if (insertIndex >= 0)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(insertIndex, data); //.Add(data);
                            }
                            else if (insertIndex < 0)
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Insert(0, data);
                            }
                        }
                        else
                        {
                            if (dropTarget.ItemsSource == null)
                            {
                                PivotItem pivotItem = new PivotItem() { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField };
                                this.GridControl.IgnoreRefesh = false;
                                this.ApplyTemplateToGroupingControl(dropTarget, pivotItem);
                            }
                            else
                            {
                                this.GridControl.IgnoreRefesh = false;
                                ((IList)dropTarget.ItemsSource).Add(new PivotItem { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, Format = compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField });
                            }
                        }
                    }
                }
            }

            else if (e.Data.GetDataPresent(typeof(FilterItemsCollection)))
            {
                FilterItemsCollection filterItem = (FilterItemsCollection)e.Data.GetData(typeof(FilterItemsCollection));
                //// Removing the filter item entry from the filters collection
                RemoveFilterItem(filterItem);
                //// Generating a PivotItem based on FilterItemsCollection
                PivotItem pivotItem = GetPivotItem(filterItem);
                pivotItem.ShowSubTotal = filterItem.ShowSubTotal;
                object data = e.Data.GetData(typeof(FilterItemsCollection));
                if (!pivotItem.AllowRunTimeGroupByField)
                {
                    e.Handled = true;
                    return;
                }
                if (DragSource == dropTarget)
                {
                    if (this.BreakFunction(button, data, DragSource, dropTarget))
                    {
                        return;
                    }
                }

                if (dropTarget == this.DataHeaderArea)
                {
                    if (button != null)
                    {
                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                        {
                            insertIndex++;
                        }
                        if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                        {
                            this.InsertCalculationInfo(insertIndex, pivotItem);
                        }
                        else if (insertIndex < 0)
                        {
                            this.InsertCalculationInfo(0, pivotItem);
                        }
                    }
                    else
                    {
                        this.AddComputationInfo(pivotItem);
                    }
                }
                else
                {
                    if (dropTarget.ItemsSource == null)
                    {
                        this.GridControl.IgnoreRefesh = false;
                        this.ApplyTemplateToGroupingControl(dropTarget, pivotItem);
                    }
                    else
                    {
                        //((IList)dropTarget.ItemsSource).Add(pivotItem);

                        int insertIndex = ((IList)dropTarget.ItemsSource).IndexOf(button.Tag);
                        Point buttonRelativePosition = e.GetPosition(button);
                        if (!(button.ActualWidth > 0 && (button.ActualWidth / 2) > buttonRelativePosition.X))
                        {
                            insertIndex++;
                        }
                        if (insertIndex >= 0)// && insertIndex < ((IList)dropTarget.ItemsSource).Count)
                        {
                            //this.InsertItemToValues(insertIndex, pivotItem);
                            this.GridControl.IgnoreRefesh = false;
                            ((IList)dropTarget.ItemsSource).Insert(insertIndex, pivotItem);
                        }
                        else if (insertIndex < 0)
                        {
                            this.GridControl.IgnoreRefesh = false;
                            ((IList)dropTarget.ItemsSource).Add(pivotItem);
                        }
                    }
                }
                this.AddEmptyItem(this.FilterHeaderArea);
                if (OnFilterItemChanged != null)
                    OnFilterItemChanged(this, e);
            }

            this.GridControl.GridScrollViewer.ScrollToHorizontalOffset(0);
        }

        /// <summary>
        /// Handles the PreviewDragLeave event of the PivotItemList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void PivotItemList_PreviewDragLeave(object sender, DragEventArgs e)
        {
            this.RemoveAdorners();
        }
        #endregion

        #region [ Command Routed Events ]
        /// <summary>
        /// Can Execute the FieldList visibility(show/hide) operation for the given command
        /// </summary>
        public void ShowFilterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Button filterbtn = e.OriginalSource as Button;
            if (this.AllowFiltering)
            {
                filterbtn.Visibility = System.Windows.Visibility.Visible;
                e.CanExecute = true;
                e.Handled = true;
            }
            else
            {
                filterbtn.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        class PivotFilterElementComparer : IComparer<FilterItemElement>
        {
            IComparer comparer = null;
            public PivotFilterElementComparer(IComparer comparer)
            {
                this.comparer = comparer;
            }
            public int Compare(FilterItemElement x, FilterItemElement y)
            {
                return comparer.Compare(x.Key, y.Key);
            }
        }
        /// <summary>
        /// Executes the Filter visibility(show/hide) operation for the given command
        /// </summary>
        public void ShowFilterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null && button.Tag != null)
            {
                if (button.Tag is PivotItem)
                {
                    PivotItem pivotItem = button.Tag as PivotItem;
                    PropertyDescriptor descriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
                    string format = pivotItem.Format == null || pivotItem.Format.Length == 0 ? null : pivotItem.Format;
                    string name = pivotItem.FieldMappingName;
                    if (pivotItem.Comparer == null && descriptor != null && descriptor.PropertyType != typeof(DateTime))
                    {
                        pivotItem.Comparer = (IComparer)GridControl.PivotEngine.AddComparers(descriptor.PropertyType);
                    }
                    FilterExpression fItem=null;
                    if (descriptor.PropertyType == typeof(DateTime) && pivotItem.Format != null)
                        fItem = this.GridControl.Filters.Where(f => f.Name == name && f.Format == format).FirstOrDefault();
                    else
                        fItem = this.GridControl.Filters.Where(f => f.Name == name).FirstOrDefault();
                    if (fItem != null && fItem.Tag != null)
                    {
                        FilterItemsCollection filterList = fItem.Tag as FilterItemsCollection;

                        if (filterList.FilteredValues.Count > 0)
                        {
                            foreach (var item in filterList)
                            {
                                //filterList.Select(i => i).Where(j => j.Key == item).FirstOrDefault().IsSelected = true;
                                if (item.Key != "(All)")
                                {
                                    if (filterList.FilteredValues.Contains(item.Key))
                                    {
                                        item.IsSelected = true;
                                    }
                                    else
                                        item.IsSelected = false;
                                }
                            }
                        }

                        this.FilterPopup.FilterList = filterList;
                    }
                    else
                    {
                        this.FilterPopup.FilterList = GetFilterItem(descriptor, format);
                        if (descriptor.PropertyType == typeof(DateTime) && format != null)
                        {
                            this.FilterPopup.FilterListForDateTime = GetFilterItemsForDateTime(descriptor, format);
                        }
                        if (format != null && pivotItem.FieldHeader != null && pivotItem.FieldHeader.Length > 0)
                        {
                            this.FilterPopup.FilterList.SetName(pivotItem.FieldHeader);
                        }
                        if (pivotItem.Comparer != null)
                        {
                            var temp = this.FilterPopup.FilterList[0];// remove (ALL) before sorting...
                            this.FilterPopup.FilterList.RemoveAt(0);
                            this.FilterPopup.FilterList.Sort(new PivotFilterElementComparer(pivotItem.Comparer));
                            this.FilterPopup.FilterList.Insert(0, temp); //Insert (ALL) back...
                        }
                    }

                    this.FilterPopup.PlacementTarget = button;
                    this.FilterPopup.IsOpen = true;
                }
                else if (button.Tag is FilterItemsCollection || button.Tag is FilterExpression)
                {
                    FilterItemsCollection filterList = null;
                    if (button.Tag is FilterExpression)
                    {
                        filterList = ((FilterExpression)button.Tag).Tag as FilterItemsCollection;
                    }
                    else
                    {
                        filterList = button.Tag as FilterItemsCollection;
                    }

                    if (filterList.FilteredValues.Count > 0)
                    {
                        foreach (var item in filterList)
                        {
                            //filterList.Select(i => i).Where(j => j.Key == item).FirstOrDefault().IsSelected = true;
                            if (filterList.FilteredValues.Contains(item.Key))
                            {
                                item.IsSelected = true;
                            }
                            else
                                item.IsSelected = false;
                        }
                    }

                    this.FilterPopup.FilterList = filterList;
                    this.FilterPopup.PlacementTarget = button;
                    this.FilterPopup.IsOpen = true;
                }
            }
            this.FilterPopup.GetButton(button);
        }
        /// <summary>
        /// Can Execute the PivotItem's sorting operation for the given command
        /// </summary>
        public void SortPivotItemCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void ReCalculateRowCoverRanges()
        {
            if (this.GridControl.PivotEngine.PivotRows.Count == 0)
                return;

            //cover upper left corner
            int lastColumn = this.GridControl.PivotEngine.PivotRows.Count;
            int row = this.GridControl.PivotEngine.PivotColumns.Count + ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? 1 : 0);

            CoveredCellRange range = new CoveredCellRange(0, 0, row - 1, lastColumn - 1);
            if (row >= 1)
            {
                this.GridControl.PivotEngine[0, 0].CellRange = range;
                this.GridControl.PivotEngine[0, 0].CellType = PivotCellType.TopLeftCell;
                this.GridControl.PivotEngine.CoveredRanges.Add(range);
            }

            int row0 = row;
            int lastCol = this.GridControl.PivotEngine.PivotRows.Count;
            int startCol = 1;

            while (startCol < lastCol)
            {
                row = row0;
                //cover the columns
                int startRow = ++row;
                ++row;
                int index = 1;
                while (row < this.GridControl.PivotEngine.RowCount && startCol < lastCol)
                {
                    int parentRow = row - 2;
                    int parentCol = startCol - 1;

                    string o = this.GridControl.PivotEngine[row - 2, startCol - 1].UniqueText;
                    while (row < this.GridControl.PivotEngine.RowCount && (this.GridControl.PivotEngine[row - 1, startCol - 1].UniqueText == null || this.GridControl.PivotEngine[row - 1, startCol - 1].UniqueText.Equals(o)))
                    {
                        row++;
                    }

                    int col = startCol + 1;
                    while (col <= lastCol && this.GridControl.PivotEngine[startRow - 1, col - 1].UniqueText == null)
                    {
                        col++;
                    }
                    col--;

                    if (row > startRow + 1 || col > startCol)
                    {
                        //mark all empty cells to easily spot successive covered cells...
                        for (int r = startRow; r <= row - 1; ++r)
                        {
                            for (int c = startCol; c <= Math.Min(lastCol, col); ++c)
                            {
                                if (this.GridControl.PivotEngine[r - 1, c - 1].UniqueText == null)
                                {
                                    this.GridControl.PivotEngine[r - 1, c - 1].Value = 'x';
                                    this.GridControl.PivotEngine[r - 1, c - 1].UniqueText = "x";
                                    this.GridControl.PivotEngine[r - 1, c - 1].ParentCell = this.GridControl.PivotEngine[parentRow, parentCol];
                                }
                            }
                        }

                        range = new CoveredCellRange(startRow - 1, startCol - 1, row - 2, Math.Min(col, lastCol) - 1);
                        this.GridControl.PivotEngine[startRow - 1, startCol - 1].CellRange = range;
                        this.GridControl.PivotEngine.CoveredRanges.RemoveAt(index);
                        this.GridControl.PivotEngine.CoveredRanges.Insert(index, range);
                        index++;

                    }
                    else if (this.GridControl.PivotEngine.PivotCalculations.Count > 0
                     && this.GridControl.PivotEngine[row - 1, startCol - 1] != null && this.GridControl.PivotEngine[row - 1, startCol - 1].UniqueText != null
                     && this.GridControl.PivotEngine[row - 1, startCol - 1].UniqueText.ToString() != "x")
                    {
                        range = new CoveredCellRange(startRow - 1, startCol - 1, startRow - 1, startCol - 1);
                        this.GridControl.PivotEngine[startRow - 1, startCol - 1].CellRange = range;
                        this.GridControl.PivotEngine.CoveredRanges.RemoveAt(index);
                        this.GridControl.PivotEngine.CoveredRanges.Insert(index, range);
                        index++;

                    }
                    startRow = row;
                    row++;
                }
                startCol++;
                this.GridControl.InternalGrid.CoveredCells.Clear();
                for (int i = 0; i < this.GridControl.PivotEngine.CoveredRanges.Count - 1; i++)
                {
                    var range1 = this.GridControl.PivotEngine.CoveredRanges[i];
                    if (!this.GridControl.InternalGrid.CoveredCells.Any(x => x.Top == range1.Top && x.Bottom == range1.Bottom && x.Left == range1.Left && x.Right == range1.Right))
                        this.GridControl.InternalGrid.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(range1.Top, range1.Left, range1.Bottom, range1.Right));
                }

            }
        }
        /// <summary>
        /// Executes the PivotItem's sorting operation for the given command
        /// </summary>
        public void SortPivotItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ToggleButton button = e.OriginalSource as ToggleButton;
            if (button != null && button.Tag != null && this.AllowSorting)
            {
                PivotItem pivotItem = button.Tag as PivotItem;                
                if (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.GridLayout != GridLayout.TopSummary && !this.GridControl.PivotEngine.UseIndexedEngine)
                {
                    int loc = this.GridControl.PivotRows.IndexOf(pivotItem);
                    bool isRowPivot = loc > -1;
                    if (!isRowPivot)
                    {
                        loc = this.GridControl.PivotColumns.IndexOf(pivotItem);
                    }

                    ListSortDirection dir = pivotItem.Comparer == null ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.GridControl.SortPivotItem(loc, dir, isRowPivot);
                    if (this.GridControl.Filters.Count > 0)
                    {
                         this.GridControl.InternalGrid.ReApplyFilters(false);
                    }

                    else
                    {
                        if (this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || !GridControl.ShowSubTotals)
                            foreach (PivotItem item in this.GridControl.PivotRows)
                            {
                                this.GridControl.InternalGrid.SubTotalVisibilityRenderer(item);
                            }
                        else if (this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false) || !GridControl.ShowSubTotals)
                            foreach (PivotItem item in this.GridControl.PivotColumns)
                            {
                                this.GridControl.InternalGrid.SubTotalVisibilityRenderer(item);
                            }
                    }
                }
                if (pivotItem.Comparer == null)
                {
                    pivotItem.Comparer = new ReverseOrderComparer();
                    if (!this.GridControl.PivotEngine.EnableOnDemandCalculations || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.GridLayout == GridLayout.TopSummary) || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.PivotEngine.UseIndexedEngine))
                    {
                        this.GridControl.UpdateGridLayout = true;
                        this.GridControl.InternalRefresh();
                    }
                }
                else if (pivotItem.Comparer is ReverseOrderComparer)
                {
                    pivotItem.Comparer = null;
                    if (!this.GridControl.PivotEngine.EnableOnDemandCalculations || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.GridLayout == GridLayout.TopSummary) || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.PivotEngine.UseIndexedEngine))
                    {
                        this.GridControl.UpdateGridLayout = true;
                        this.GridControl.InternalRefresh();
                    }
                }
                else //custom comparer specified through the PivotItem
                {
                    if (_savedComparers == null)
                    {
                        _savedComparers = new Dictionary<PivotItem, IComparer>();
                    }
                    if (_savedComparers.ContainsKey(pivotItem))
                    {
                        pivotItem.Comparer = _savedComparers[pivotItem];
                        _savedComparers.Remove(pivotItem);
                        if (!this.GridControl.PivotEngine.EnableOnDemandCalculations || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.PivotEngine.UseIndexedEngine))
                        {
                            this.GridControl.UpdateGridLayout = true;
                            this.GridControl.InternalRefresh();
                        }
                    }
                    else
                    {
                        _savedComparers.Add(pivotItem, pivotItem.Comparer);
                        pivotItem.Comparer = new ReverseCustomComparer(pivotItem.Comparer);
                        if (!this.GridControl.PivotEngine.EnableOnDemandCalculations || (this.GridControl.PivotEngine.EnableOnDemandCalculations && this.GridControl.PivotEngine.UseIndexedEngine))
                        {
                            this.GridControl.UpdateGridLayout = true;
                            this.GridControl.InternalRefresh();
                        }
                    }
                }

            }
        }
        /// <summary>
        /// Can execute the data reloading operation for the given command
        /// </summary>
        public void ReloadDataCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        /// <summary>
        /// Executes the data reloading operation for the given command
        /// </summary>
        public void ReloadDataExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //this.GridControl.PivotEngine.Populate();
            this.GridControl.InternalRefresh();
        }
        /// <summary>
        /// Can execute the order for the given command
        /// </summary>
        public void OrderCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        /// <summary>
        /// Executes the order for the given command
        /// </summary>
        public void OrderExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            bool _isValidSorting = false;

            if (this.CurrentItemsControl == this.ColumnHeaderArea)
            {
                if (e.Parameter.ToString() == "Smallest to largest")
                {
                    var orderedCollection = this.GridControl.PivotColumns.OrderBy(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotColumns[i].FieldHeader)
                            {
                                this.GridControl.PivotColumns[i] = this.GridControl.PivotEngine.PivotColumns[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else if (e.Parameter.ToString() == "Largest to smallest")
                {
                    var orderedCollection = this.GridControl.PivotColumns.OrderByDescending(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotColumns[i].FieldHeader)
                            {
                                this.GridControl.PivotColumns[i] = this.GridControl.PivotEngine.PivotColumns[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else
                {
                    int[] val = MoveItem(e.Parameter.ToString(), this.ColumnHeaderArea.ItemsSource, this.PivotItemToggleButton.Tag);
                    this.GridControl.PivotColumns.Move(val[0], val[1]);
                }
            }

            else if (this.CurrentItemsControl == this.RowHeaderArea)
            {
                if (e.Parameter.ToString() == "Smallest to largest")
                {
                    var orderedCollection = this.GridControl.PivotRows.OrderBy(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotRows[i].FieldHeader)
                            {
                                this.GridControl.PivotRows[i] = this.GridControl.PivotEngine.PivotRows[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else if (e.Parameter.ToString() == "Largest to smallest")
                {
                    var orderedCollection = this.GridControl.PivotRows.OrderByDescending(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotRows[i].FieldHeader)
                            {
                                this.GridControl.PivotRows[i] = this.GridControl.PivotEngine.PivotRows[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else
                {
                    int[] val = MoveItem(e.Parameter.ToString(), this.RowHeaderArea.ItemsSource, this.PivotItemToggleButton.Tag);
                    this.GridControl.PivotRows.Move(val[0], val[1]);
                }
            }

            else if (this.CurrentItemsControl == this.DataHeaderArea)
            {
                if (e.Parameter.ToString() == "Smallest to largest")
                {
                    var orderedCollection = this.GridControl.PivotCalculations.OrderBy(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotCalculations[i].FieldHeader)
                            {
                                this.GridControl.PivotCalculations[i] = this.GridControl.PivotEngine.PivotCalculations[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else if (e.Parameter.ToString() == "Largest to smallest")
                {
                    var orderedCollection = this.GridControl.PivotCalculations.OrderByDescending(m => m.FieldHeader).ToList();
                    if (orderedCollection != null)
                    {
                        for (int i = 0; i < orderedCollection.Count; i++)
                        {
                            if (orderedCollection[i].FieldHeader != this.GridControl.PivotCalculations[i].FieldHeader)
                            {
                                this.GridControl.PivotCalculations[i] = this.GridControl.PivotEngine.PivotCalculations[i] = orderedCollection[i];
                                _isValidSorting = true;
                            }
                        }
                    }
                }
                else
                {
                    int[] val = MoveItem(e.Parameter.ToString(), this.DataHeaderArea.ItemsSource, this.PivotItemToggleButton.Tag);
                    this.GridControl.PivotCalculations.Move(val[0], val[1]);
                }
            }

            if (_isValidSorting)
                this.GridControl.InternalRefresh();
        }
        /// <summary>
        /// Can execute the delete operation of the PivotItems for the given command
        /// </summary>
        public void DeleteItemCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Button deletebtn = e.OriginalSource as Button;
            if (this.AllowRemove)
            {
                deletebtn.Visibility = System.Windows.Visibility.Visible;
                e.CanExecute = true;
                e.Handled = true;
            }
            else
            {
                deletebtn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Executes the delete operation of the PivotItems for the given command
        /// </summary>
        public void DeleteItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;

            if (button != null && button.Tag != null)
            {
                ListBox parentListBox = Utils.GetParentItem<ListBox>(button);
                //this.FieldList = new PivotGridFieldList(this.GridControl, this.DragSource);
                if (parentListBox != null)
                {
                    if (button.Tag is PivotItem)
                    {
                        PivotItem pivotItem = (PivotItem)button.Tag;
                        this.GridControl.PivotFields.Add(this.GetPivotItemData(pivotItem));
                        ((IList)parentListBox.ItemsSource).Remove(pivotItem);
                    }
                    else if (button.Tag is PivotComputationInfo)
                    {
                        PivotComputationInfo computationInfo = (PivotComputationInfo)button.Tag;
                        this.GridControl.PivotFields.Add(this.GetPivotItemData(computationInfo));
                        ((IList)parentListBox.ItemsSource).Remove(computationInfo);
                    }
                    else if (button.Tag is FilterItemsCollection || button.Tag is FilterExpression)
                    {
                        FilterItemsCollection filterList = null;
                        if (button.Tag is FilterExpression)
                        {
                            filterList = ((FilterExpression)button.Tag).Tag as FilterItemsCollection;
                        }
                        else
                        {
                            filterList = button.Tag as FilterItemsCollection;
                        }
                        this.GridControl.FilterItems.Remove(filterList);
                        this.GridControl.PivotFields.Add(this.GetPivotItemData(filterList));
                        ((IList)parentListBox.ItemsSource).Remove(filterList);
                    }
                }
                this.AddEmptyItem(parentListBox);
                this.CanDrop = false;
                if (this.GridControl.ShowFieldList)
                {
                    if (this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemTemplate == null)
                    {
                        this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemTemplate = this.GridControl.GroupingBar.FieldList.Resources["PivotItemTemplate"] as DataTemplate;
                    }

                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemsSource = null;//.Clear();
                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.DataContext = this.GridControl.PivotFields;
                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemsSource = this.GridControl.PivotFields;
                }
            }
        }
        /// <summary>
        /// Cam  execute the FieldList visibility(show/hide) operation for the given command
        /// </summary>
        public void ShowFieldListCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        /// <summary>
        /// Executes the FieldList visibility(show/hide) operation for the given command
        /// </summary>
        public void ShowFieldListExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (IsWindowLoaded)
            {
                this.ContextMenu.IsOpen = false;

                if (this.RowHeaderArea != null)
                {
                    this.RowHeaderArea.ContextMenu.IsOpen = false;
                }

                if (this.PivotItemToggleButton != null)
                {
                    this.PivotItemToggleButton.ContextMenu.IsOpen = false;
                }

                if (this.FieldList != null && this.FieldList.IsLoaded)
                {
                    return;
                }

                Window parentWindow = Utils.GetParentItem<Window>(this);
                this.FieldList = new PivotGridFieldList(this.GridControl, this.DragSource);
                if (parentWindow != null)
                {
                    FieldList.Owner = parentWindow;
                }
                else if (Application.Current != null && Application.Current.MainWindow != null)
                {
                    FieldList.Owner = Application.Current.MainWindow;
                }
                GeneralTransform transformation = this.ColumnHeaderArea.TransformToVisual(this);
                Point windowPosition = transformation.Transform(new Point(0, 0));
                windowPosition = this.PointToScreen(windowPosition);

                //Console.WriteLine(Canvas.GetLeft(this.ColumnHeaderArea));
                FieldList.Left = windowPosition.X + 10;
                FieldList.Top = windowPosition.Y + 20;
                FieldList.Show();
            }
        }
        #endregion

        #region [Methods]

        private PivotItem GetPivotItemData(object item)
        {
            if (item is PivotItem)
            {
                return item as PivotItem;
            }
            else if (item is FilterItemsCollection)
            {
                PivotItem pivotitem = this.GridControl.GroupingBar.GetPivotItem(item as FilterItemsCollection);
                return pivotitem;
            }
            else if (item is PivotComputationInfo)
            {
                PivotComputationInfo compInfo = item as PivotComputationInfo;
                PivotItem pivotitem = new PivotItem() { FieldHeader = compInfo.FieldHeader, FieldMappingName = compInfo.FieldName, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField };
                return pivotitem;
            }
            return null;
        }

        #endregion
    }
}