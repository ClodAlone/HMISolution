#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
#if !WP
using System.ComponentModel.DataAnnotations;
#endif
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Linq;
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Collections.Generic;
#if WinRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class DataRow : GridDataRow
    {
        internal int GroupRecordIndex { get; set; }

        #region Ctor

        public DataRow()
        {

        }

        #endregion

        #region override methods
        
        protected override VirtualizingCellsControl OnCreateRowElement()
        {
            if (this.IsAddNewRow)
            {
                var row = new AddNewRowControl
                {
                    DataContext = this.RowData,
                    Visibility = this.RowVisibility
                };
                row.GetVisibleLineOrigin = GetVisibleLineOrigin;
                row.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                SetSelectionBorderBindings(row);
                return row;
            }
            else if (this.RowRegion == Grid.RowRegion.Header)
            {
                var row = new HeaderRowControl
                    {
                        DataContext = this.RowData,
                        Visibility = this.RowVisibility
                    };
                row.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                return row;
            }
            else if (this.RowRegion == Grid.RowRegion.Footer)
            {
                var row = new TableSummaryRowControl
                    {
                        DataContext = this.RowData,
                        Visibility = this.RowVisibility
                    };
                UpdateRowStyles(row);
                row.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                return row;
            }
            else
            {
                var row = new VirtualizingCellsControl
                    {
                        DataContext = this.RowData,
                        Visibility = this.RowVisibility
                    };
                UpdateRowStyles(row);
                row.GetVisibleLineOrigin = GetVisibleLineOrigin;
                row.AllowRowHoverHighlighting = AllowRowHoverHighlighting;
                row.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                SetSelectionBorderBindings(row);
                return row;
            }
        }

        protected override void OnGenerateVisibleColumns(VisibleLinesCollection visibleColumnLines)
        {
            this.VisibleColumns.Clear();
            for (int i = 0; i < 3; i++)
            {
                int StartColumnIndex;
                int EndColumnIndex;
                if (i == 0)
                {
                    if (visibleColumnLines.FirstBodyVisibleIndex <= 0)
                        continue;
                    StartColumnIndex = 0;
                    EndColumnIndex = visibleColumnLines[visibleColumnLines.FirstBodyVisibleIndex - 1].LineIndex;
                }
                else if (i == 1)
                {
                    if (visibleColumnLines.FirstBodyVisibleIndex <= 0 && visibleColumnLines.LastBodyVisibleIndex < 0)
                        continue;
                    if (visibleColumnLines.Count > visibleColumnLines.firstBodyVisibleIndex)
                        StartColumnIndex = visibleColumnLines[visibleColumnLines.FirstBodyVisibleIndex].LineIndex;
                    else
                        continue;
                    EndColumnIndex = visibleColumnLines[visibleColumnLines.LastBodyVisibleIndex].LineIndex;
                    
                }
                else
                {
                    if (visibleColumnLines.FirstFooterVisibleIndex >= visibleColumnLines.Count)
                        continue;
                    StartColumnIndex = visibleColumnLines[visibleColumnLines.FirstFooterVisibleIndex].LineIndex;
                    EndColumnIndex = visibleColumnLines[visibleColumnLines.Count - 1].LineIndex;
                }

                for (int index = StartColumnIndex; index <= EndColumnIndex; index++)
                {
                    if (DataGrid.ShowRowHeader && index == 0)
                    {
                        if (!this.VisibleColumns.Any(col => col.ColumnIndex == index))
                        {
                            CreateRowHeaderColumn(index);
                        }
                        continue;
                    }
                    else if (this.DataGrid.View != null && ((index < this.DataGrid.View.GroupDescriptions.Count) || (index <= this.DataGrid.View.GroupDescriptions.Count && this.DataGrid.ShowRowHeader)))
                    {
                        if (!this.VisibleColumns.Any(col => col.ColumnIndex == index))
                        {
                            var isLastRow = this.DataGrid.RowGenerator.IsLastRow(this.DataGrid.ResolveToRecordIndex(this.RowIndex + 1));
                            var isLastGroupRow = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(this.RowIndex + 1)] is Group;
                            var indentColumn = CreateIndentColumn(index);
                            indentColumn.IsEnsured = true;

                            if (isLastRow)
                                indentColumn.IndentColumnType = IndentColumnType.InLastGroupRow;
                            else if (isLastGroupRow)
                            {
                                if (index == 0)
                                    indentColumn.IndentColumnType = this.RowRegion == RowRegion.Header ? IndentColumnType.InHeader : IndentColumnType.InDataRow;
                                else
                                    indentColumn.IndentColumnType = IndentColumnType.InLastGroupRow;
                            }
                            else
                                indentColumn.IndentColumnType = this.RowRegion == RowRegion.Header ? IndentColumnType.InHeader : IndentColumnType.InDataRow;
                           
                            this.VisibleColumns.Add(indentColumn);
                        }
                        continue;
                    }
#if !WP
                    var expanderindex = (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count + 1 : 1) + (this.DataGrid.ShowRowHeader ? 1 : 0);
                    if (this.DataGrid.DetailsViewManager.HasDetailsView && (index < expanderindex))
                    {
                        var expanderColumn = CreateDetailsViewExpanderColumn(index);
                        VisibleColumns.Add(expanderColumn);
                        continue;
                    }
#endif
                    int heightIncrementation = 0;
                    if (this.RowRegion == RowRegion.Header && this.DataGrid.StackedHeaderRows.Count > 0 && !this.IsAddNewRow)
                    {
                        heightIncrementation = this.DataGrid.GetHeightIncremeantationLimit(new CoveredCellInfo(index, index), this.RowIndex - 1);
                    }
                    var dc = CreateColumn(index, heightIncrementation);
                    var columnIndex =this.DataGrid.ResolveToGridVisibleColumnIndex(index);
                    if (this.DataGrid.Columns[columnIndex].IsHidden)
                        dc.ColumnVisibility = Visibility.Collapsed;
                    this.VisibleColumns.Add(dc);
                }
            }
        }

        internal override void EnsureColumns(VisibleLinesCollection visibleColumnLines)
        {
            this.VisibleColumns.ForEach(column => column.IsEnsured = false);
            var StartColumnIndex = visibleColumnLines[0].LineIndex;
            var EndColumnIndex = visibleColumnLines[visibleColumnLines.LastBodyVisibleIndex].LineIndex;
            
            for (int index = StartColumnIndex; index <= EndColumnIndex; index++)
            {
                if (DataGrid.ShowRowHeader && index == 0)
                {
                    var rhc = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);
                    if (rhc != null)
                    {
                        if (rhc.ColumnVisibility == Visibility.Collapsed)
                            rhc.ColumnVisibility = Visibility.Visible;
                        rhc.IsEnsured = true;
                    }
                    else
                        CreateRowHeaderColumn(index);
                    continue;
                }
                else if (this.DataGrid. View != null && ((index < this.DataGrid.View.GroupDescriptions.Count) || (index <= this.DataGrid.View.GroupDescriptions.Count && this.DataGrid.ShowRowHeader)))
                {
                    if (!this.VisibleColumns.Any(col => col.ColumnIndex == index))
                    {
                        var indentColumn = CreateIndentColumn(index);
                        indentColumn.IsEnsured = true;
                        indentColumn.IndentColumnType = this.RowRegion == Grid.RowRegion.Header
                                                            ? IndentColumnType.InHeader
                                                            : IndentColumnType.InDataRow;
                        this.VisibleColumns.Add(indentColumn);
                    }
                    var ic = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);
                    if (ic != null)
                    {
                        if (ic.ColumnVisibility == Visibility.Collapsed)
                            ic.ColumnVisibility = Visibility.Visible;
                        ic.IsEnsured = true;
                    }
                    continue;
                }
#if !WP
                if (this.DataGrid.DetailsViewManager.HasDetailsView && (index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) + 1 + (this.DataGrid.ShowRowHeader ? 1 : 0)))
                {
                    DataColumnBase expanderColumn;
                    if (this.RowRegion == RowRegion.Header)
                        expanderColumn = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index && column.IsIndentColumn);
                    else
                        expanderColumn = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index && column.IsExpanderColumn);
                    
                    if (expanderColumn == null)
                    {
                        expanderColumn = CreateDetailsViewExpanderColumn(index);
                        expanderColumn.IsEnsured = true;
                        VisibleColumns.Add(expanderColumn);
                    }
                    else
                    {
                        expanderColumn.IsEnsured = true;
                        if (expanderColumn.ColumnVisibility == Visibility.Collapsed)
                            expanderColumn.ColumnVisibility = Visibility.Visible;
                    }
                    continue;
                }
#endif
                if (this.VisibleColumns.All(column => column.ColumnIndex != index))
                {
                    var datacolumn = this.VisibleColumns.FirstOrDefault(
                        column => ((column.ColumnIndex < StartColumnIndex || column.ColumnIndex > EndColumnIndex) &&
                                   !column.IsEnsured && !column.IsIndentColumn && column.Renderer != null && column.Renderer != this.DataGrid.CellRenderers["RowHeader"] &&
#if !WP
                                   !column.IsExpanderColumn &&
#endif
                                   !column.IsEditing && !column.IsSelectedColumn));
                    if (datacolumn != null)
                    {
                        UpdateColumn(datacolumn, index);
                    }
                }
                var dc = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);
                GridColumn gc = null;
                var columnIndex = this.DataGrid.View == null ? index : this.DataGrid.ResolveToGridVisibleColumnIndex(index);
                if (this.DataGrid.Columns.Count > columnIndex)
                    gc = this.DataGrid.Columns[columnIndex];

                if (dc != null)
                {
                    if (dc.ColumnVisibility == Visibility.Collapsed && gc != null && !gc.IsHidden)
                    {
                        dc.ColumnVisibility = Visibility.Visible;
                        if (dc.IsEditing && dc.Renderer != null && dc.Renderer.HasCurrentCellState)
                            dc.Renderer.SetFocus(true);
                    }
                    if (this.RowRegion == RowRegion.Header&& !this.IsAddNewRow && this.DataGrid.StackedHeaderRows.Count>0)
                        dc.RowSpan =
                            this.DataGrid.GetHeightIncremeantationLimit(new CoveredCellInfo(index, index),
                                                                                     this.RowIndex - 1);
#if !WP
                    if (this.RowIndex!=-1 && this.DataGrid.NavigationMode == NavigationMode.Cell && this.RowIndex == this.DataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex && !this.DataGrid.SelectionController.CurrentCellManager.HasCurrentCell)
                        this.UpdateCurrentCellSelection(dc);
#endif
                    dc.IsEnsured = true;
                }
                else
                {
                    if (index >= this.DataGrid.VisualContainer.ColumnCount)
                        continue;
                    int heightIncrementation = 0;
                    if (this.RowRegion == RowRegion.Header && !this.IsAddNewRow)
                        heightIncrementation =
                            this.DataGrid.GetHeightIncremeantationLimit(new CoveredCellInfo(index, index),
                                                                                     this.RowIndex - 1);
                    var datacolumn = CreateColumn(index, heightIncrementation);
#if !WP
                    if (this.RowIndex != -1 && this.RowIndex == this.DataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex && this.DataGrid.NavigationMode == NavigationMode.Cell)
                        this.UpdateCurrentCellSelection(datacolumn);
#endif
                    datacolumn.IsEnsured = true;
                    this.VisibleColumns.Add(datacolumn);
                    if (gc != null && gc.IsHidden)
                        datacolumn.ColumnVisibility = Visibility.Collapsed;
                }
            }
            //if (this.IsSelectedRow && i == 1 && this.dataGrid.GridModel.HasGroup)
            if (this.IsSelectedRow && this.DataGrid.GridModel.HasGroup)
            {
                this.WholeRowElement.UpdateSelectionBorderClip();
            }
            
            this.VisibleColumns.ForEach(column =>
                {
                    if (!column.IsEnsured)
                    {
                        CollapseColumn(column);
                    }
                });
            Panel panel = this.WholeRowElement.ItemsPanel;
            if (panel != null)
                panel.InvalidateMeasure();
        }

        internal override void UpdateCurrentCellSelection()
        {
            if (this.DataGrid.SelectionMode != GridSelectionMode.None && DataGrid.SelectionController.CurrentCellManager.CurrentCell == null)
            {
                var dataColumn =
                    this.VisibleColumns.FirstOrDefault(
                        item => item.ColumnIndex == this.DataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex);
                if (dataColumn != null)
                {
                    if (!dataColumn.IsSelectedColumn)
                    {
                        dataColumn.IsSelectedColumn = true;
                        DataGrid.SelectionController.CurrentCellManager.SetCurrentColumnBase(dataColumn, true);
                    }
                }
            }
        }

#if !WP
        protected override void OnRowIndexChanged()
        {
            base.OnRowIndexChanged();
            if (suspendUpdateStyle && (DataGrid.hasAlternatingRowStyle || DataGrid.hasAlternatingRowStyleSelector))
                this.ApplyRowStyles(this.WholeRowElement);

            if (DataGrid != null && DataGrid.ShowRowHeader)
            {
                var visiblecolumn = this.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == 0);
                if (visiblecolumn == null || this.RowIndex < 0) return;
                var rowHeaderCell = (visiblecolumn.ColumnElement as GridRowHeaderCell);
                if (rowHeaderCell != null)
                    rowHeaderCell.RowIndex = this.RowIndex;
            }

            var dc = this.VisibleColumns.FirstOrDefault(column => column.IsExpanderColumn);
            if (dc == null || this.RowIndex <= 0) return;
            var expander = (dc.ColumnElement as GridDetailsViewExpanderCell);
            if (expander != null)
                expander.RowColumnIndex = new RowColumnIndex(this.RowIndex, dc.ColumnIndex);
        }
#endif

        #endregion

        #region private method

        private DataColumnBase CreateColumn(int index, int columnHeightIncrementation)
        {
            DataColumnBase dc = new DataColumn();
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.RowSpan = columnHeightIncrementation;
            var columnIndex = this.DataGrid.ResolveToGridVisibleColumnIndex(index);
            dc.GridColumn = this.DataGrid.Columns[columnIndex];

            if (this.RowIndex <= this.DataGrid.GetHeaderIndex() && this.RowIndex >= 0)
            {
                dc.Renderer = this.DataGrid.CellRenderers["Header"];
                this.RowData = this.DataGrid.Columns[columnIndex];
            }
            else if (dc.GridColumn.IsUnbound)
                dc.Renderer = this.DataGrid.CellRenderers["UnBoundColumn"];

            else
                dc.Renderer = dc.GridColumn.CellType != string.Empty
                                  ? this.DataGrid.CellRenderers[dc.GridColumn.CellType]
                                  : this.DataGrid.CellRenderers["Static"];

            dc.IsEditing = false;
            dc.InitializeColumnElement(this.RowData, false);
            dc.SelectionController = this.DataGrid.SelectionController;
#if !WP
            SetCurrentCellBorderBinding(dc.ColumnElement);
#endif

            if (this.DataGrid.GridValidationMode != GridValidationMode.None && this.RowIndex > this.DataGrid.HeaderLineCount - 1 && this.RowIndex >= 0)
            {
#if !WP
                this.DataGrid.Validations.ValidateColumn(this.RowData, dc.GridColumn.MappingName, dc.ColumnElement as GridCell, new RowColumnIndex(dc.RowIndex, dc.ColumnIndex));
#endif
            }

            return dc;
        }

        private void UpdateColumn(DataColumnBase dc, int index)
        {
            if (index < 0 || index >= this.DataGrid.VisualContainer.ColumnCount)
            {
                dc.ColumnVisibility = Visibility.Collapsed;
            }
            else
            {
                dc.ColumnIndex = index;
                if (this.RowRegion == RowRegion.Header && !this.IsAddNewRow)
                    dc.RowSpan = this.DataGrid.GetHeightIncremeantationLimit(new CoveredCellInfo(index, index), this.RowIndex - 1);
                else
                    dc.RowSpan = 0;
                dc.GridColumn = this.DataGrid.Columns[this.DataGrid.ResolveToGridVisibleColumnIndex(index)];
                bool isElementUnloaded = this.UpdateRenderer(dc);
                if (dc.ColumnVisibility == Visibility.Collapsed)
                    dc.ColumnVisibility = Visibility.Visible;
                if (isElementUnloaded)
                {
                    dc.ColumnElement.ClearValue(FrameworkElement.DataContextProperty);
                    dc.InitializeColumnElement(this.RowData, dc.IsEditing);
                    dc.UpdateCellStyle();
                }
                else
                    dc.UpdateBinding(this.RowData);
#if !WP
                if (this.DataGrid.GridValidationMode != GridValidationMode.None && this.RowIndex > this.DataGrid.HeaderLineCount - 1 && this.RowIndex >= 0)
                    this.DataGrid.Validations.ValidateColumn(this.RowData, dc.GridColumn.MappingName, dc.ColumnElement as GridCell, new RowColumnIndex(dc.RowIndex, dc.ColumnIndex));
#endif
            }
        }
        /// <summary>
        /// Update Renderer and UnloadUIElement if needed
        /// </summary>
        /// <param name="dataColumn"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool UpdateRenderer(DataColumnBase dataColumn)
        {
            IGridCellRenderer newRenderer = null;
            if (this.RowRegion == RowRegion.Header && !IsAddNewRow)
                newRenderer = this.DataGrid.CellRenderers["Header"];
            else
            {
                if (dataColumn.GridColumn.IsUnbound)
                    newRenderer = this.DataGrid.CellRenderers["UnBoundColumn"];
                else
                    newRenderer = dataColumn.GridColumn.CellType != string.Empty
                                  ? this.DataGrid.CellRenderers[dataColumn.GridColumn.CellType]
                                  : this.DataGrid.CellRenderers["Static"];
            }
            if (dataColumn.Renderer != null && dataColumn.Renderer != newRenderer)
            {
		        dataColumn.Renderer.UnloadUIElements(new RowColumnIndex(dataColumn.RowIndex, dataColumn.ColumnIndex), dataColumn.ColumnElement);
                dataColumn.Renderer = newRenderer;
                 return true;
             }
             return false;
        }

        private void UpdateCurrentCellSelection(DataColumnBase column)
        {
            if (this.DataGrid.SelectionMode != GridSelectionMode.None)
            {
                if (this.DataGrid.SelectionController.CurrentCellManager.CurrentCellIndex.ColumnIndex == column.ColumnIndex)
                {
                    column.IsSelectedColumn = true;
                    if (DataGrid.SelectionController.CurrentCellManager.CurrentCell == null)
                        DataGrid.SelectionController.CurrentCellManager.SetCurrentColumnBase(column, true);
                }
                else
                {
                    column.IsSelectedColumn = false;
                }
            }
        }

#if !SILVERLIGHT && !WP
        private void ApplyRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            var hasRowStyleSelector = DataGrid.hasRowStyleSelector;
            var hasRowStyle = DataGrid.hasRowStyle;
            var hasAlternatingRowStyle = DataGrid.hasAlternatingRowStyle;
            var hasAlternatingRowStyleSelector = DataGrid.hasAlternatingRowStyleSelector;

            if (!hasRowStyle && !hasRowStyleSelector && !hasAlternatingRowStyle && !hasAlternatingRowStyleSelector)
                return;

            if (hasAlternatingRowStyle || hasAlternatingRowStyleSelector)
            {
                int index;
                if (DataGrid.GridModel.HasGroup)
                    index = GroupRecordIndex;
                else
                {
                    index = RowIndex - DataGrid.HeaderLineCount;
                    if (this.DataGrid.DetailsViewManager.HasDetailsView)
                        index = index / (this.DataGrid.DetailsViewDefinition.Count + 1);
                }
                index += 1;
                var canApplyAlternatingRowStyle = (index % DataGrid.AlternationCount) == 0;
                if (canApplyAlternatingRowStyle)
                {
                    if (hasAlternatingRowStyle && hasAlternatingRowStyleSelector)
                    {
                        newStyle = DataGrid.AlternatingRowStyleSelector.SelectStyle(this, row);
                        newStyle = newStyle ?? DataGrid.AlternatingRowStyle;
                    }
                    else if (hasAlternatingRowStyleSelector)
                    {
                        newStyle = DataGrid.AlternatingRowStyleSelector.SelectStyle(this, row);
                    }
                    else if (hasAlternatingRowStyle)
                    {
                        newStyle = DataGrid.AlternatingRowStyle;
                    }
                    row.Style = newStyle;
                    return;
                }
            }

            if (!hasRowStyle && !hasRowStyleSelector)
            {
                if(row.ReadLocalValue(FrameworkElement.StyleProperty)!= DependencyProperty.UnsetValue)
                    row.ClearValue(FrameworkElement.StyleProperty);
                return;
            }

            if (hasRowStyleSelector && hasRowStyle)
            {
                newStyle = DataGrid.RowStyleSelector.SelectStyle(this, row);
                newStyle = newStyle ?? DataGrid.RowStyle;
            }
            else if (hasRowStyleSelector)
            {
                newStyle = DataGrid.RowStyleSelector.SelectStyle(this, row);
            }
            else if (hasRowStyle)
            {
                newStyle = DataGrid.RowStyle;
            }
            row.Style = newStyle;
        }
#else
        private void ApplyRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            var hasRowStyle = DataGrid.hasRowStyle;
            var hasAlternatingRowStyle = DataGrid.hasAlternatingRowStyle;

            if (!hasRowStyle && !hasAlternatingRowStyle)
                return;

            if (hasAlternatingRowStyle)
            {
                 int index;
                if (DataGrid.GridModel.HasGroup)
                    index = GroupRecordIndex + 1;
                else
                    index = ((RowIndex - DataGrid.HeaderLineCount) + 1);
                var canApplyAlternatingRowStyle = (index % DataGrid.AlternationCount) == 0;
                if (canApplyAlternatingRowStyle)
                {
                    newStyle = DataGrid.AlternatingRowStyle;
                    row.Style = newStyle;
                    return;
                }
            }

            if (hasRowStyle)
            {
                newStyle = DataGrid.RowStyle;
            }
            else
            {
                if (row.ReadLocalValue(FrameworkElement.StyleProperty) != DependencyProperty.UnsetValue)
                    row.ClearValue(FrameworkElement.StyleProperty);
                return;
            }
            row.Style = newStyle;
        }
#endif


        internal override void UpdateRowStyles(ContentControl row)
        {
            if (row != null && this.RowRegion != RowRegion.Header)
            {
                this.ApplyRowStyles(row);
            }
        }

        protected override DataColumnBase CreateIndentColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsIndentColumn = true;
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            if (this.RowRegion == Grid.RowRegion.Header && !this.IsAddNewRow)
                dc.ColumnElement = new GridHeaderIndentCell();
            else
                dc.InitializeColumnElement(this.RowData, false);
            return dc;
        }
#if !WP
        private DataColumnBase CreateDetailsViewExpanderColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.GridColumn = null;
            dc.IsEditing = false;
            dc.SelectionController = this.DataGrid.SelectionController;
            if (this.RowRegion == Grid.RowRegion.Header)
            {
                dc.ColumnElement = new GridHeaderIndentCell();
                dc.IsIndentColumn = true;
            }
            else
            {
                dc.IsExpanderColumn = true;
                dc.Renderer = this.DataGrid.CellRenderers["DetailsViewExpander"];
                dc.InitializeColumnElement(this.RowData, false);
                var expander = (dc.ColumnElement as GridDetailsViewExpanderCell);
                if (expander != null)
                {
                    expander.columnBase = dc;
                    if (expander.IsExpanded != this.IsExpanded)
                        expander.IsExpanded = this.IsExpanded;
                }
                SetCurrentCellBorderBinding(dc.ColumnElement);
            }
            return dc;
        }

        internal void CheckForDetailsViewExpanderVisibilty()
        {
            var dc = this.VisibleColumns.FirstOrDefault(column => column.IsExpanderColumn);
            if (dc == null) return;
            var expander = (dc.ColumnElement as GridDetailsViewExpanderCell);
            if (expander == null) return;
            if (IsAddNewRow)
            {
                expander.ExpanderIconVisibility = Visibility.Collapsed;
                return;
            }
            var record = this.DataGrid.DetailsViewManager.GetDetailsViewRecord(this.RowIndex);
            if (!this.DataGrid.HideEmptyGridViewDefinition)
            {
                expander.ExpanderIconVisibility = Visibility.Visible;
                if (record != null && expander.IsExpanded != record.IsExpanded)
                {
                    expander.SuspendChangedAction = true;
                    expander.IsExpanded = record.IsExpanded;
                    expander.SuspendChangedAction = false;
                }
                if (expander.IsExpanded)
                {
                    for (int i = 1; i <= this.DataGrid.DetailsViewDefinition.Count; i++)
                    {
                        this.DataGrid.VisualContainer.RowHeights.SetHidden(this.RowIndex + i, this.RowIndex + i, false);                         
                        var row = this.DataGrid.RowGenerator.Items.OfType<DetailsViewDataRow>().FirstOrDefault(datarow => datarow.RowIndex == i + this.RowIndex);
                        if (row != null)
                        {
                            if (i == this.DataGrid.DetailsViewDefinition.Count)
                                row.ApplyContentVisualState("LastCell");
                            else
                                row.ApplyContentVisualState("NormalCell");
                        }
                    }
                }
                return;
            }
            var provider = this.DataGrid.View.GetPropertyAccessProvider();
            if (provider == null) return;
            int count = 0;
            bool IsExpanderVisible = false;
            int detailsrowIndex = -1;
            foreach (var gridViewDefinition in this.DataGrid.DetailsViewDefinition.OfType<GridViewDefinition>())
            {
                var childSource = provider.GetValue(this.RowData, gridViewDefinition.RelationalColumn) as IEnumerable;
                //if (childSource != null && childSource.AsQueryable().Count() > 0)
                count++;
                if (childSource != null && childSource.Cast<object>().Any())
                {
                    expander.ExpanderIconVisibility = Visibility.Visible;
                    IsExpanderVisible = true;
                    detailsrowIndex = count;
                }
                else
                    this.DataGrid.VisualContainer.RowHeights.SetHidden(this.RowIndex + count, this.RowIndex + count, true);
            }
            if (!IsExpanderVisible)
            {
                expander.ExpanderIconVisibility = Visibility.Collapsed;
                if (expander.IsExpanded)
                    expander.IsExpanded = false;
            }
            else
            {
                var row = this.DataGrid.RowGenerator.Items.OfType<DetailsViewDataRow>().FirstOrDefault(datarow => datarow.RowIndex == detailsrowIndex + this.RowIndex);
                if (row != null)
                    row.ApplyContentVisualState("LastCell");
            }
        }
#endif
        #endregion

        #region internal methods
        /// <summary>
        /// Update the value of UnBoundColumn
        /// </summary>
        /// <param name="dr"></param>
        /// <remarks></remarks>
        internal void UpdateUnBoundColumn()
        {
            if (VisibleColumns.Any(col => col.GridColumn != null && col.GridColumn.IsUnbound))
            {
                VisibleColumns.ForEach(col =>
                {
                    if (col.GridColumn != null && col.GridColumn.IsUnbound)
                    {
                        col.ColumnElement.DataContext = this.RowData;
                         col.UpdateBinding(this.RowData);
                    }
                });
            }
        }

        #endregion
    }
}
