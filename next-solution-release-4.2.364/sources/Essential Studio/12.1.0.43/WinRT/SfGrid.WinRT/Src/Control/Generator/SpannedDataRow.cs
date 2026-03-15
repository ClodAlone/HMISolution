#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data.Extensions;
using Syncfusion.Data;
using System.Linq;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
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
    public class SpannedDataRow : GridDataRow
    {
        #region Fields

        List<CoveredCellInfo> coveredCells;
        internal Func<int,int,double> GetCoveredColumnSize;

        #endregion

        #region Property

        internal List<CoveredCellInfo> CoveredCells
        {
            get { return coveredCells; }
        }

        #endregion

        #region Ctor

        public SpannedDataRow()
        {
            coveredCells = new List<CoveredCellInfo>();
        }

        #endregion

        #region override methods

        protected bool ShowRowHeader()
        {
            return this.DataGrid.ShowRowHeader;
        }

        protected override void OnRowIndexChanged()
        {
            base.OnRowIndexChanged();

            if (DataGrid != null && DataGrid.ShowRowHeader)
            {
                var dc = this.VisibleColumns.FirstOrDefault();
                if (dc == null || this.RowIndex <= 0) return;
                var rowHeaderCell = (dc.ColumnElement as GridRowHeaderCell);
                if (rowHeaderCell != null)
                    rowHeaderCell.RowIndex = this.RowIndex;
            }
        }

        protected override VirtualizingCellsControl OnCreateRowElement()
        {
            if (this.RowType== RowType.CaptionRow || this.RowType== RowType.CaptionCoveredRow)
            {
                var captionRow = new CaptionSummaryRowControl
                    {
                        DataContext = this.RowData,
                        Visibility = this.RowVisibility
                    };
                this.UpdateRowStyles(captionRow);
                captionRow.GetVisibleLineOrigin = GetVisibleLineOrigin;
                captionRow.UpdateVisibleColumn(GetVisibleColumns,this.ShowRowHeader, this.GetColumnVisibleLineInfo, GetColumnSize);
                captionRow.IsExpandedChanged = IsExpandedChanged;
                captionRow.CheckForValidation = CheckForValidation;
                SetSelectionBorderBindings(captionRow);
                return captionRow;
            }
            else if (this.RowType == RowType.SummaryCoveredRow || this.RowType == RowType.SummaryRow)
            {
                var summaryRow = new GroupSummaryRowControl
                    {
                        DataContext = this.RowData,
                    };
                this.UpdateRowStyles(summaryRow);
                summaryRow.GetVisibleLineOrigin = GetVisibleLineOrigin;
                summaryRow.UpdateVisibleColumns(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                SetSelectionBorderBindings(summaryRow);
                return summaryRow;
            }
            else if (RowType == Grid.RowType.TableSummaryCoveredRow || RowType == Grid.RowType.TableSummaryRow)
            {
                var footerRow = new TableSummaryRowControl
                {
                    DataContext = this.RowData
                };
                this.UpdateRowStyles(footerRow);
                footerRow.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                SetSelectionBorderBindings(footerRow);
                return footerRow;
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
            else
            {
                var footerRow = new TableSummaryRowControl
                    {
                        DataContext = this.RowData
                    };
                this.UpdateRowStyles(footerRow);
                footerRow.InitializeVirtualizingRowControl(GetVisibleColumns, this.GetColumnVisibleLineInfo, GetColumnSize);
                SetSelectionBorderBindings(footerRow);
                return footerRow;
            }
        }

        protected override double GetColumnSize(int index, bool lineNull)
        {
            if (!lineNull)
            {
                foreach (var item in this.CoveredCells)
                {
                    if (index >= item.Left && index <= item.Right)
                    {
                        return GetCoveredColumnSize(item.Left, item.Right);
                    }
                }
            }
            return base.GetColumnSize(index,lineNull);
        }

        protected override void OnGenerateVisibleColumns(VisibleLinesCollection visibleColumnLines)
        {
            if (visibleColumnLines.Count <= 0)
                return;
            int startColumnIndex;
            int endColumnIndex;
            
            this.VisibleColumns.Clear();

            if (this.CoveredCells.Count > 0)
            {
                foreach (var coveredCell in this.CoveredCells)
                {
                    int columnHeightIncrementation = 0;
                    columnHeightIncrementation = coveredCell.RowSpan;
                    var dc = CreateColumn(coveredCell,coveredCell.Left, columnHeightIncrementation,coveredCell.Right - coveredCell.Left);
                    this.VisibleColumns.Add(dc);
                }
                int startIndex = 0;

                if (this.DataGrid.ShowRowHeader)
                {
                    this.CreateRowHeaderColumn(startIndex);
                    startIndex++;
                }

                for (int index = startIndex; index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) + startIndex; index++)
                {
                    this.CreateIndentInCoveredRow(index);
                }
#if !WP
                if (this.DataGrid.DetailsViewManager.HasDetailsView && this.DataGrid.View != null)
                {
                    if (this.RowIndex < this.DataGrid.StackedHeaderRows.Count)
                    {
                        this.VisibleColumns.Add(this.CreateIndentColumn(startIndex));
                    }                    
                    else if (this.RowType == RowType.SummaryRow || this.RowType == RowType.CaptionRow || this.RowType == RowType.TableSummaryRow)
                    {
                        this.VisibleColumns.Add(CreateDetailsViewIndentColumn(this.DataGrid.View.GroupDescriptions.Count + startIndex));
                    }
                }
#endif
                this.ResetLastColumnBorderThickness(this.VisibleColumns.LastOrDefault(col => col.ColumnElement is GridGroupSummaryCell || col.ColumnElement is GridTableSummaryCell || col.ColumnElement is GridCaptionSummaryCell), true);
            }
            else
            {
                startColumnIndex = visibleColumnLines[visibleColumnLines.FirstBodyVisibleIndex - this.DataGrid.VisualContainer.FrozenColumns].LineIndex;
                endColumnIndex = visibleColumnLines[visibleColumnLines.LastBodyVisibleIndex].LineIndex;
                for (int index = startColumnIndex; index <= endColumnIndex; index++)
                {
                    if (index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0))
                    {
                        this.CreateIndentInCoveredRow(index);
                        continue;
                    }
                    //var dc = CreateColumn(null, index, 0, 0);
                    //dc.IsFixedColumn = false;
                    //this.VisibleColumns.Add(dc);
                    //this.ResetLastColumnBorderThickness(dc, index == endColumnIndex);
                }
            }
        }

        internal override void EnsureColumns(VisibleLinesCollection visibleColumnLines)
        {
            if (this.RowIndex == -1)
            {
                this.RowLevel = -1;
                return;
            }
            int startColumnIndex;
            int endColumnIndex;
            this.VisibleColumns.ForEach(column => column.IsEnsured = false);
            startColumnIndex = visibleColumnLines[0].LineIndex;
            endColumnIndex = visibleColumnLines[visibleColumnLines.LastBodyVisibleIndex].LineIndex;
            if (this.CoveredCells.Count > 0 || this.RowIndex < this.DataGrid.HeaderLineCount)
            {
                for (int index = startColumnIndex; index <= endColumnIndex; index++)
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
#if !WP
                    if (this.DataGrid.View != null && this.DataGrid.DetailsViewManager.HasDetailsView && index == this.DataGrid.View.GroupDescriptions.Count + 1)
                    {
                        var dvc = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);
                        if (dvc != null)
                        {
                            if (dvc.ColumnVisibility == Visibility.Collapsed)
                                dvc.ColumnVisibility = Visibility.Visible;
                            dvc.IsEnsured = true;
                            continue;
                        }

                        if (this.RowIndex < this.DataGrid.StackedHeaderRows.Count)
                        {
                            this.VisibleColumns.Add(this.CreateIndentColumn(index));
                        }
                        else if (this.RowType == RowType.SummaryRow || this.RowType == RowType.CaptionRow ||
                                 this.RowType == RowType.TableSummaryRow)
                        {
                            this.VisibleColumns.Add(CreateDetailsViewIndentColumn(index));
                        }
                        continue;
                    }
#endif

                    if ((index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0)) || (index <= (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) && this.DataGrid.ShowRowHeader))
                    {
                        this.EnsureIndentColumns(visibleColumnLines, index);
                        this.CheckAvailablity(index, true);
                        continue;
                    }
                    else
                    {
                        CoveredCellInfo coveredCellItem = null;
                        if (this.RowType == RowType.CaptionCoveredRow || this.RowType == RowType.SummaryCoveredRow || this.RowType == RowType.TableSummaryCoveredRow)
                        {
                            coveredCellItem = this.CoveredCells.FirstOrDefault();
                        }
                        else
                            coveredCellItem = this.CoveredCells.FirstOrDefault(item => item.Left <= index && item.Right >= index);
                        var actualIndex = coveredCellItem != null ? coveredCellItem.Left : index;
                        if ((this.RowRegion == RowRegion.Body || RowType == RowType.TableSummaryCoveredRow || RowType == RowType.TableSummaryRow) && this.VisibleColumns.All(column => column.ColumnIndex != actualIndex))
                        {
                            if (this.VisibleColumns.Any(column => ((column.ColumnIndex < startColumnIndex || column.ColumnIndex > endColumnIndex) && !column.IsEnsured && !column.IsIndentColumn)))
                            {
                                var datacolumn = this.VisibleColumns.FirstOrDefault(column => ((column.ColumnIndex < startColumnIndex || column.ColumnIndex > endColumnIndex) && !column.IsEnsured && !column.IsIndentColumn && column.Renderer != null && column.Renderer != this.DataGrid.CellRenderers["RowHeader"]));
                                if (datacolumn != null)
                                {
                                    UpdateColumn(datacolumn, index);
                                }
                            }
                        }
                        this.CheckAvailablity(actualIndex, false);
                    }
                }
                var orderedColumns = this.VisibleColumns.OrderBy(item => item.ColumnIndex);
                this.ResetLastColumnBorderThickness(orderedColumns.LastOrDefault(col => col.ColumnElement is GridGroupSummaryCell || col.ColumnElement is GridTableSummaryCell || col.ColumnElement is GridCaptionSummaryCell), true);
            }

            if (this.IsSelectedRow && (this.RowType != Grid.RowType.TableSummaryRow || this.RowType != Grid.RowType.TableSummaryCoveredRow))
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
            this.InvalidateMeasure();
        }

        internal override void UpdateRowStyles(ContentControl row)
        {
            if (row != null && this.RowRegion != Grid.RowRegion.Header)
            {
                if (row is CaptionSummaryRowControl)
                {
                    this.ApplyCaptionSummaryRowStyles(row);
                }
                else if (row is GroupSummaryRowControl)
                {
                    this.ApplyGroupSummaryRowStyles(row);
                }
                else
                {
                    this.ApplyTableSummaryRowStyles(row);
                }
            }
        }

        protected override DataColumnBase CreateIndentColumn(int index)
        {
            DataColumnBase dc = new SpannedDataColumn();
            dc.IsIndentColumn = true;
            dc.IsEnsured = true;
            dc.IsEditing = false;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            if (this.RowRegion == Grid.RowRegion.Header)
                dc.ColumnElement = new GridHeaderIndentCell();
            else
                dc.InitializeColumnElement(this.RowData, false);
            if (this.RowType == RowType.TableSummaryRow || this.RowType == RowType.TableSummaryCoveredRow)
            {
                (dc.ColumnElement as GridIndentCell).ColumnType = IndentColumnType.InTableSummaryRow;
                dc.IndentColumnType = IndentColumnType.InTableSummaryRow;
            }
            return dc;
        }

        #endregion

        #region Private Methods

        private void EnsureIndentColumns(VisibleLinesCollection visibleColumns, int index)
        {
            var startColumnIndex = visibleColumns[0].LineIndex + (this.DataGrid.ShowRowHeader ? 1 : 0);
            var endColumnIndex = visibleColumns[visibleColumns.LastBodyVisibleIndex].LineIndex;
            if ((index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0)) || (index <= (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) && this.DataGrid.ShowRowHeader))
            {
                if ((index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0)) || (this.DataGrid.ShowRowHeader && index <= (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0)))
                {
                    if (index > this.RowLevel && (this.RowType == RowType.CaptionCoveredRow || this.RowType == RowType.SummaryCoveredRow || this.RowType == RowType.TableSummaryCoveredRow))
                    {
                        return;
                    }
                }
                if (!this.VisibleColumns.Any(column => column.ColumnIndex == index && column.IsIndentColumn))
                {
                    var dataColumn = this.VisibleColumns.FirstOrDefault(column => ((column.ColumnIndex < startColumnIndex || column.ColumnIndex > endColumnIndex) && !column.IsEnsured && column.IsIndentColumn));
                    if (dataColumn != null)
                    {
                        if (index < 0)
                        {
                            dataColumn.ColumnVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            dataColumn.ColumnIndex = index;
                            if (dataColumn.ColumnVisibility == Visibility.Collapsed)
                                dataColumn.ColumnVisibility = Visibility.Visible;
                        }
                    }
                    else
                        this.CreateIndentInCoveredRow(index);
                }
                else
                {
                    var indentColumn = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index && column.IsIndentColumn);
                    if (indentColumn != null)
                        indentColumn.IsEnsured = true;
                }
            }
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
                var columnElement = dc.ColumnElement as GridCell;
                if (columnElement!=null && columnElement.IsLastCell)
                    columnElement.IsLastCell = false;
                dc.GridColumn = this.DataGrid.Columns[ResolveToGridColumnIndex(index)];
                if (dc.ColumnVisibility == Visibility.Collapsed)
                    dc.ColumnVisibility = Visibility.Visible;
                dc.ColumnElement.DataContext = this.RowData;
                dc.UpdateBinding(this.RowData);
            }
        }

        private SpannedDataColumn CreateColumn(CoveredCellInfo cc, int index,int heightIncrementation, int widthIncrementation)
        {
            var dc = new SpannedDataColumn
                {
                    ColumnIndex = index,
                    RowSpan = heightIncrementation,
                    ColumnSpan = widthIncrementation,
                    GridColumn = this.DataGrid.Columns[ResolveToGridColumnIndex(index)],
                    RowIndex = this.RowIndex,
                    SelectionController = this.DataGrid.SelectionController
                };
            if (this.RowType == RowType.TableSummaryRow || this.RowType == RowType.TableSummaryCoveredRow)
                dc.Renderer = this.DataGrid.CellRenderers["TableSummary"];
            else if (this.RowType == RowType.CaptionRow || this.RowType == RowType.CaptionCoveredRow)
                dc.Renderer = this.DataGrid.CellRenderers["CaptionSummary"];
            else if (this.RowRegion == Grid.RowRegion.Header)
            {
                if (this.DataGrid.HeaderLineCount - 1 > dc.RowIndex)
                {
                    dc.GridColumn = null;
                    dc.Renderer = this.DataGrid.CellRenderers["StackedHeader"];
                    this.RowData = this.DataGrid.StackedHeaderRows[this.RowIndex].StackedColumns.FirstOrDefault(col => col.HeaderText == cc.Name);
                }
                else
                    dc.Renderer = this.DataGrid.CellRenderers["Header"];
            }
            else //(this.RowType == RowType.SummaryRow || this.RowType == RowType.SummaryCoveredRow)
                dc.Renderer = this.DataGrid.CellRenderers["GroupSummary"];

            dc.InitializeColumnElement(this.RowData, false);
            return dc;
        }

        private void InvalidateMeasure()
        {
            Panel panel = this.WholeRowElement.ItemsPanel;
            if (this.WholeRowElement is CaptionSummaryRowControl)
                this.WholeRowElement.InvalidateMeasure();
            if (panel != null)
                panel.InvalidateMeasure();
        }

        private void CreateIndentInCoveredRow(int index)
        {
            var indentColumn = CreateIndentColumn(index);
            if (this.RowData is Group)
            {
                var group = this.RowData as Group;
                int lastGroupLevel = -1;
                if (!group.IsExpanded)
                {
                    bool isLastRow = this.DataGrid.RowGenerator.IsLastRow(group, this.RowIndex, ref lastGroupLevel);
                    bool isLastGroup = this.DataGrid.RowGenerator.IsLastGroup(group);
                    if (indentColumn.ColumnIndex == ((this.Level + (this.DataGrid.ShowRowHeader ? 1 : 0)) - 1))
                    {
                        indentColumn.IndentColumnType = IndentColumnType.InExpanderCollapsed;
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Collapsed)
                            indentColumn.ColumnElement.Visibility = Visibility.Visible;
                    }
                    else if (indentColumn.ColumnIndex < (this.Level + (this.DataGrid.ShowRowHeader ? 1 : 0) - 1))
                    {
                        indentColumn.IndentColumnType = indentColumn.ColumnIndex < lastGroupLevel ? (isLastRow ? IndentColumnType.InLastGroupRow : IndentColumnType.BeforeExpander) : (isLastGroup ? IndentColumnType.InLastGroupRow : IndentColumnType.BeforeExpander);
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Collapsed)
                            indentColumn.ColumnElement.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        indentColumn.IndentColumnType = IndentColumnType.AfterExpander;
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Visible)
                            indentColumn.ColumnElement.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (indentColumn.ColumnIndex == (this.Level + (this.DataGrid.ShowRowHeader ? 1 : 0) - 1))
                    {
                        indentColumn.IndentColumnType = IndentColumnType.InExpanderExpanded;
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Collapsed)
                            indentColumn.ColumnElement.Visibility = Visibility.Visible;
                    }
                    else if (indentColumn.ColumnIndex < (this.Level + (this.DataGrid.ShowRowHeader ? 1 : 0) - 1))
                    {
                        indentColumn.IndentColumnType = IndentColumnType.BeforeExpander;
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Collapsed)
                            indentColumn.ColumnElement.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        indentColumn.IndentColumnType = IndentColumnType.AfterExpander;
                        if (this.RowType == RowType.CaptionCoveredRow && indentColumn.ColumnElement.Visibility == Visibility.Visible)
                            indentColumn.ColumnElement.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (this.RowType == RowType.SummaryRow || this.RowType == RowType.SummaryCoveredRow)
            {
                var isLastRow = this.DataGrid.RowGenerator.IsLastRow(this.DataGrid.ResolveToRecordIndex(this.RowIndex + 1));
                var isLastGroupRow = this.DataGrid.View.TopLevelGroup.DisplayElements[this.DataGrid.ResolveToRecordIndex(this.RowIndex + 1)] is Group;

                if (isLastRow)
                    indentColumn.IndentColumnType = IndentColumnType.InLastGroupRow;
                else if (isLastGroupRow)
                {
                    indentColumn.IndentColumnType = index == 0 ? IndentColumnType.InSummaryRow : IndentColumnType.InLastGroupRow;
                }
                else
                    indentColumn.IndentColumnType = IndentColumnType.InSummaryRow;
            }
            else if (this.RowType == RowType.TableSummaryRow || this.RowType == RowType.TableSummaryCoveredRow)
                indentColumn.IndentColumnType = IndentColumnType.InTableSummaryRow;
            else
                indentColumn.IndentColumnType = IndentColumnType.InHeader;

            indentColumn.IsEnsured = true;
            this.VisibleColumns.Add(indentColumn);
        }

        private bool CheckAvailablity(int index, bool forIndentColumn)
        {
            if (index >= this.DataGrid.VisualContainer.ColumnCount)
                return false;

            if (forIndentColumn && ((index < (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0)) || (this.DataGrid.ShowRowHeader && index <= (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0))))
            {
                if (index > this.RowLevel && (this.RowType == RowType.CaptionCoveredRow || this.RowType == RowType.SummaryCoveredRow || this.RowType == RowType.TableSummaryCoveredRow))
                {
                    return false;
                }
            }

            var cc = this.CoveredCells.FirstOrDefault(cell => cell.Left == index);

            DataColumnBase dataColumn = this.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == index);

            if (dataColumn != null)
            {
                if (dataColumn.ColumnVisibility == Visibility.Collapsed)
                    dataColumn.ColumnVisibility = Visibility.Visible;
                dataColumn.IsEnsured = true;
                var columnElement = dataColumn.ColumnElement as GridCell;
                if (columnElement != null && columnElement.IsLastCell)
                    columnElement.IsLastCell = false;
                if (this.RowType!= Grid.RowType.CaptionRow && this.RowType!= Grid.RowType.SummaryRow && (dataColumn.ColumnElement is GridIndentCell) && dataColumn.IndentColumnType == IndentColumnType.AfterExpander)
                    dataColumn.ColumnVisibility = Visibility.Collapsed;
                return true;
            }
            else if (cc!=null && this.DataGrid.Columns.Count > 0)
            {
                var dc = CreateColumn(cc, index, cc.RowSpan, cc.Right - cc.Left);
                dc.IsEnsured = true;
                this.VisibleColumns.Add(dc);
            }
            else if (this.DataGrid.Columns.Count > 0 && this.RowRegion!= Grid.RowRegion.Header)
            {
                var dc = CreateColumn(null, index, 0, 0);
                dc.IsEnsured = true;
                this.VisibleColumns.Add(dc);
            }
            return false;
        }

        private int ResolveToGridColumnIndex(int index)
        {
            int resolvedIndex = 0;
            if (this.RowType == RowType.SummaryCoveredRow || this.RowType == RowType.CaptionCoveredRow || this.RowType == RowType.TableSummaryCoveredRow)
            {
                var group = this.RowData as Group;
                if (group != null)
                    resolvedIndex = index - group.Level - (this.DataGrid.ShowRowHeader ? 1 : 0);
            }
            else
#if !WP
                resolvedIndex = index - (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) - (this.DataGrid.DetailsViewDefinition.Count > 0 ? 1 : 0) - (this.DataGrid.ShowRowHeader ? 1 : 0);
#else
                resolvedIndex = index - (this.DataGrid.View != null ? this.DataGrid.View.GroupDescriptions.Count : 0) - (this.DataGrid.ShowRowHeader ? 1 : 0);
#endif
            return resolvedIndex;
        }

        private void ResetLastColumnBorderThickness(DataColumnBase column, bool isLast)
        {
            if (column != null)
            {
                if (isLast)
                {
                    var cell = column.ColumnElement as GridCell;
                    cell.IsLastCell = true;
                }
                else
                {
                    var cell = column.ColumnElement as GridCell;
                    cell.IsLastCell = false;
                }
            }
        }

        private void IsExpandedChanged(bool isExpanded)
        {
            if (isExpanded)
                this.DataGrid.GridModel.ExpandGroup(this.RowData as Group);
            else
                this.DataGrid.GridModel.CollapseGroup(this.RowData as Group);

            //this.dataGrid.UpdateRowCountAndScrollBars();
            this.DataGrid.GridModel.RefreshDataRow(this.RowIndex, true);
#if !WP
            if (this.DataGrid.NotifyListener != null)
            {
                var parentDataGrid = this.DataGrid.NotifyListener.GetParentDataGrid();
                parentDataGrid.VisualContainer.ScrollRows.MarkDirty();
                parentDataGrid.VisualContainer.InvalidateMeasureInfo();
            }
#endif
        }
#if !SILVERLIGHT && !WP
        private void ApplyCaptionSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasCaptionSummaryRowStyle = DataGrid.hasCaptionSummaryRowStyle;
            bool hasCaptionSummaryRowStyleSelector = DataGrid.hasCaptionSummaryRowStyleSelector;

            if (!hasCaptionSummaryRowStyleSelector && !hasCaptionSummaryRowStyle)
                return;

            if (hasCaptionSummaryRowStyleSelector && hasCaptionSummaryRowStyle)
            {
                newStyle = DataGrid.CaptionSummaryRowStyleSelector.SelectStyle(this, row);
                newStyle = newStyle ?? DataGrid.CaptionSummaryRowStyle;
            }
            else if (hasCaptionSummaryRowStyleSelector)
            {
                newStyle = DataGrid.CaptionSummaryRowStyleSelector.SelectStyle(this, row);
            }
            else if (hasCaptionSummaryRowStyle)
            {
                newStyle = DataGrid.CaptionSummaryRowStyle;
            }
            row.Style = newStyle;
        }

        private void ApplyGroupSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasGroupSummaryRowStyleSelector = DataGrid.hasGroupSummaryRowStyleSelector;
            bool hasGroupSummaryRowStyle = DataGrid.hasGroupSummaryRowStyle;

            if (!hasGroupSummaryRowStyleSelector && !hasGroupSummaryRowStyle)
                return;

            if (hasGroupSummaryRowStyleSelector && hasGroupSummaryRowStyle)
            {
                newStyle = DataGrid.GroupSummaryRowStyleSelector.SelectStyle(this, row);
                newStyle = newStyle ?? DataGrid.GroupSummaryRowStyle;
            }
            else if (hasGroupSummaryRowStyleSelector)
            {
                newStyle = DataGrid.GroupSummaryRowStyleSelector.SelectStyle(this, row);
            }
            else if (hasGroupSummaryRowStyle)
            {
                newStyle = DataGrid.GroupSummaryRowStyle;
            }
            row.Style = newStyle;
        }

        private void ApplyTableSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasTableSummaryRowStyleSelector = DataGrid.hasTableSummaryRowStyleSelector;
            bool hasTableSummaryRowStyle = DataGrid.hasTableSummaryRowStyle;
            if (!hasTableSummaryRowStyle && !hasTableSummaryRowStyleSelector)
                return;

            if (hasTableSummaryRowStyleSelector && hasTableSummaryRowStyle)
            {
                newStyle = DataGrid.TableSummaryRowStyleSelector.SelectStyle(this, row);
                newStyle = newStyle ?? DataGrid.TableSummaryRowStyle;
            }
            else if (hasTableSummaryRowStyleSelector)
            {
                newStyle = DataGrid.TableSummaryRowStyleSelector.SelectStyle(this, row);
            }
            else if (DataGrid.TableSummaryRowStyle != null)
            {
                newStyle = DataGrid.TableSummaryRowStyle;
            }
            row.Style = newStyle;
        }
#else

        private void ApplyCaptionSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasCaptionSummaryRowStyle = DataGrid.hasCaptionSummaryRowStyle;

            if (!hasCaptionSummaryRowStyle)
                return;

            if (hasCaptionSummaryRowStyle)
            {
                newStyle = DataGrid.CaptionSummaryRowStyle;
            }
            row.Style = newStyle;
        }

        private void ApplyGroupSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasGroupSummaryRowStyle = DataGrid.hasGroupSummaryRowStyle;

            if (!hasGroupSummaryRowStyle)
                return;

            if (hasGroupSummaryRowStyle)
            {
                newStyle = DataGrid.GroupSummaryRowStyle;
            }
            row.Style = newStyle;
        }

        private void ApplyTableSummaryRowStyles(ContentControl row)
        {
            Style newStyle = null;

            if (DataGrid == null || row == null)
                return;

            bool hasTableSummaryRowStyle = DataGrid.hasTableSummaryRowStyle;
            if (!hasTableSummaryRowStyle)
                return;

            if (DataGrid.TableSummaryRowStyle != null)
            {
                newStyle = DataGrid.TableSummaryRowStyle;
            }
            row.Style = newStyle;
        }
#endif
        private bool CheckForValidation()
        {
            return this.DataGrid.Validations.CheckForValidation(false);
        }

        #endregion

        #region Internal Methods

        internal void ApplyFixedRowVisualState(bool isfixed)
        {
            if (isfixed)
            {
                foreach (var cell in VisibleColumns.Select(column => column.ColumnElement as GridCell))
                {
                    if (!cell.IsLastCell)
                    {
                        if (cell is GridIndentCell)
                        {
                            if ((cell as GridIndentCell).ColumnType == IndentColumnType.AfterExpander)
                                VisualStateManager.GoToState(cell, "Fixed_NormalCell", false);
                        }
                        else
                            VisualStateManager.GoToState(cell, "Fixed_NormalCell", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(cell, "Fixed_LastCell", false);
                    }
                }
            }
            else
            {
                foreach (var cell in VisibleColumns.Select(column => column.ColumnElement as GridCell))
                {
                    if (cell is GridIndentCell)
                    {
                        var indentCell = cell as GridIndentCell;
                        indentCell.ApplyIndentVisualState(indentCell.ColumnType);
                    }
                    else
                    {
                        cell.ApplyVisualState(cell.IsLastCell);
                    }
                }
            }
        }
#if !WP
        private DataColumnBase CreateDetailsViewIndentColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.SelectionController = this.DataGrid.SelectionController;
            var intentcell = new GridDetailsViewIndentCell();
            if (this.RowType != Grid.RowType.TableSummaryCoveredRow && this.RowType != Grid.RowType.TableSummaryRow)
                intentcell.ApplyVisualState("LastCell");
            dc.ColumnElement = intentcell;
            return dc;
        }
#endif

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            if (this.coveredCells != null)
            {
                this.coveredCells.Clear();
                this.coveredCells = null;
            }
            this.GetCoveredColumnSize = null;
            base.Dispose();
        }

        #endregion
    }
}
