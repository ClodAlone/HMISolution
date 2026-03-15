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
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.Data.Helper;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Linq;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows;
using System.Windows.Media;
#if !WP
using System.ComponentModel.DataAnnotations;
#endif

using System.Windows.Data;
using System.Threading;
using System.Diagnostics;


#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class RowGenerator : IRowGenerator, IDisposable
    {
        #region Fields

        internal SfDataGrid Owner;
        internal bool ForceUpdateBinding;
        private List<DataRowBase> _Items = new List<DataRowBase>();
        private int lastFetcheSize = -1;

        #endregion

        #region Property

        public ICollectionViewAdv View
        {
            get { return this.Owner.View; }
        }

        public List<DataRowBase> Items
        {
            get { return _Items; }
            set { _Items = value; }
        }

        #endregion

        #region Ctor

        public RowGenerator(SfDataGrid owner)
        {
            this.Owner = owner;
        }

        #endregion

        #region Virtual methods


        #endregion

        #region Internal Methods
        /// <summary>
        /// Updates the Binding Information for the DataColumn, when Editor APIs are Changed
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        internal void UpdateBinding(GridColumn gridColumn)
        {
            foreach (var item in Items)
            {
                if (item.RowVisibility != Visibility.Visible)
                    continue;
                var dataColumn = item.VisibleColumns.FirstOrDefault(datacolumn => datacolumn.GridColumn == gridColumn);
                if (dataColumn != null)
                    dataColumn.UpdateBinding(item.RowData, false);
            }
        }
        #endregion

        #region Public methods

        public void OnItemSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null)
                this.UnWireViewEvents();
            if (this.Items.Count > 0)
            {
                foreach (var item in Items)
                    item.Dispose();
                this.Items.Clear();
                foreach (var cellRenderer in this.Owner.CellRenderers.Values)
                {
                    var renderer = cellRenderer as IGridCellRenderer;
                    if (renderer != null)
                        renderer.ClearRecycleBin();
                }
            }
            if (args.NewValue != null)
                this.WireViewEvents();
        }

        /// <summary>
        /// Refreshing StackedHeaders after the changes in GridColumn collection
        /// </summary>
        /// <remarks></remarks>
        public void RefreshStackedHeaders()
        {
            if (this.Owner.StackedHeaderRows != null && this.Owner.StackedHeaderRows.Count > 0)
            {
                foreach (var header in this.Owner.StackedHeaderRows)
                {
                    var sdr = (this.Items.FirstOrDefault(row => row.RowIndex == this.Owner.StackedHeaderRows.IndexOf(header)) as SpannedDataRow);
                    if (sdr != null)
                    {
                        sdr.CoveredCells.Clear();
                        sdr.VisibleColumns.ForEach(col =>
                        {
                            if (this.Owner.View != null && col.ColumnIndex >= this.Owner.View.GroupDescriptions.Count)
                                this.UnloadUIElements(sdr, col);
                        });
#if !SILVERLIGHT && !WP7
                        sdr.VisibleColumns.RemoveAll(col => this.Owner.View != null && col.ColumnIndex >= this.Owner.View.GroupDescriptions.Count);
#else
                        for (int i = sdr.VisibleColumns.Count -1 ; i >= 0; i--)
                        {
                            if (sdr.VisibleColumns[i].ColumnIndex >= this.Owner.View.GroupDescriptions.Count)
                                sdr.VisibleColumns.RemoveAt(i);
                        }
#endif
                        CreateStackedCoveredCells(sdr, header, this.Owner.StackedHeaderRows.IndexOf(header));
                    }
                }
            }
        }

        #endregion

        #region internal Properties

        internal VisualContainer Container
        {
            get { return this.Owner.VisualContainer; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Create CoveredCells for the StackedHeaders
        /// </summary>
        /// <param name="sdr"></param>
        /// <param name="header"></param>
        /// <param name="rowIndex"></param>
        /// <remarks></remarks>
        private void CreateStackedCoveredCells(SpannedDataRow sdr, StackedHeaderRow header, int rowIndex)
        {
            foreach (var column in header.StackedColumns)
            {
                int colIndex = header.StackedColumns.IndexOf(column);
                List<int> childSequence = this.Owner.GetChildSequence(column, rowIndex);
                childSequence.Sort();

                childSequence = childSequence.Except(this.Owner.IntersectedChildColumn(childSequence, header, column)).ToList();
                if (rowIndex - 1 >= 0)
                {
                    var newSequence = this.Owner.CheckChildSequence(childSequence, this.Owner.StackedHeaderRows[rowIndex - 1], column);
                    childSequence = newSequence.Intersect(childSequence).ToList();
                }

                column.ChildColumnsIndex = childSequence;

                var sequence = childSequence.GroupBy(num => childSequence.Where(candidate => candidate >= num)
                                      .OrderBy(candidate => candidate)
                                      .TakeWhile((candidate, index) => candidate == num + index)
                                      .Last())
                 .Select(seq => seq.OrderBy(num => num));
                foreach (var item in sequence)
                {
                    var columnList = item.ToList();
                    int right = columnList.Max() + this.Owner.ResolveToScrollColumnIndex(0);
                    int left = columnList.Min() + this.Owner.ResolveToScrollColumnIndex(0);
                    sdr.CoveredCells.Add(new CoveredCellInfo(column.HeaderText, left, right));
                }
            }
            if (rowIndex >= 0)
            {
                foreach (var currCell in sdr.CoveredCells)
                {
                    currCell.RowSpan = this.Owner.GetHeightIncremeantationLimit(currCell, rowIndex - 1);
                }
            }
        }

        private void RemoveColumnbyIndex(DataRowBase row, int columnindex)
        {
            var datacolumn = row.VisibleColumns.FirstOrDefault(column => column.ColumnIndex == columnindex);
            row.VisibleColumns.Remove(datacolumn);
        }

        internal void UnloadUIElements(DataRowBase row, DataColumnBase col)
        {
            if (col.Renderer != null)
                col.Renderer.UnloadUIElements(new RowColumnIndex(col.RowIndex, col.ColumnIndex), col.ColumnElement);
            if (col.ColumnElement is GridHeaderCellControl)
                (col.ColumnElement as GridHeaderCellControl).Content = null;
            else if (col.ColumnElement is GridCell)
                (col.ColumnElement as GridCell).Content = null;
            row.WholeRowElement.ItemsPanel.Children.Remove(col.ColumnElement);
        }

        private DataRowBase CreateHeaderRow(int rowIndex, VisibleLinesCollection visibleColumns)
        {
            if (Owner.AddNewRowPosition == AddNewRowPosition.Top && rowIndex == this.Owner.HeaderLineCount - 1)
            {
                return CreateAddNewRow(rowIndex, visibleColumns);
            }
            else if (rowIndex == this.Owner.GetHeaderIndex())
            {
                var dr = new DataRow();
                if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
                {
                    dr.RowVisibility = Visibility.Collapsed;
                }
                dr.RowIndex = rowIndex;
                dr.DataGrid = this.Owner;
                dr.RowRegion = RowRegion.Header;
                dr.InitializeDataRow(visibleColumns);
                return dr;
            }
            else if (rowIndex < this.Owner.StackedHeaderRows.Count)
            {
                var sdr = new SpannedDataRow() { RowIndex = rowIndex, GetCoveredColumnSize = GetCoveredColumnSize };
                var header = this.Owner.StackedHeaderRows[rowIndex];

                CreateStackedCoveredCells(sdr, header, rowIndex);

                sdr.RowData = header;
                sdr.RowRegion = RowRegion.Header;
                sdr.DataGrid = this.Owner;
                sdr.InitializeDataRow(visibleColumns);
                return sdr;
            }
            else
            {
                DataRowBase dr = null;
                if (this.View != null)
                {
                    dr = MakeTableSummaryRow(dr, rowIndex, TableSummaryRowPosition.Top);
                    dr.RowRegion = RowRegion.Header;
                    dr.RowIndex = rowIndex;
                    dr.InitializeDataRow(visibleColumns);
                    if (dr.WholeRowElement is TableSummaryRowControl)
                    {
                        (dr.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.HeaderSummaryRow;
                        dr.VisibleColumns.ForEach(col =>
                            {
                                if (col.ColumnElement is GridIndentCell)
                                    (col.ColumnElement as GridIndentCell).ColumnType = IndentColumnType.InTableSummaryRow;
                            });
                    }
                }
                return dr;
            }
        }

        private DataRowBase CreateDataRow(int rowIndex, VisibleLinesCollection visibleColumns)
        {
            if (Owner.IsAddNewIndex(rowIndex))
            {
                return CreateAddNewRow(rowIndex, visibleColumns);
            }
            else
            {

                if (this.View.GroupDescriptions.Count == 0)
                {
#if !WP
                    if (this.Owner.IsInDetailsViewIndex(rowIndex))
                    {
                        RecordEntry record = null;
                        record = this.Owner.DetailsViewManager.GetDetailsViewRecord(rowIndex);
                        var datarow = this.Items.FirstOrDefault(row => row.RowData == record.Data);
                        if (datarow != null && !datarow.IsExpanded)
                            datarow.IsExpanded = true;
                        return this.Owner.DetailsViewManager.CreateDetailsViewDataRow(rowIndex);
                    }
#endif
                    var dr = new DataRow();
                    if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
                    {
                        dr.RowVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        var record = this.View.Records[this.Owner.ResolveToRecordIndex(rowIndex)];
                        dr.RowData = record.Data;
#if !WP
                        dr.IsExpanded = record.IsExpanded;
#endif
                        dr.RowIndex = rowIndex;
                        dr.DataGrid = this.Owner;
                        dr.RowRegion = RowRegion.Body;
                        dr.RowType = RowType.DefaultRow;
                        dr.RowLevel = 0;
                        dr.IsSelectedRow = this.CheckForSelection(dr);
                        dr.InitializeDataRow(visibleColumns);
                    }
                    return dr;
                }
                else
                {
                    var groupelement = this.View.TopLevelGroup.DisplayElements[this.Owner.ResolveToRecordIndex(rowIndex)];
                    if (groupelement is RecordEntry)
                    {
                        var dr = new DataRow();
                        var record = groupelement as RecordEntry;
                        dr.RowData = record.Data;
                        var group = record.Parent as Group;
                        if (group != null) dr.GroupRecordIndex = group.Records.IndexOf(record);
                        dr.RowIndex = rowIndex;
#if !WP
                        dr.IsExpanded = record.IsExpanded;
#endif
                        dr.RowType = RowType.DefaultRow;
                        dr.DataGrid = this.Owner;
                        dr.RowLevel = (record.Parent as Group).Level;
                        dr.IsSelectedRow = this.CheckForSelection(dr);
                        dr.InitializeDataRow(visibleColumns);
                        return dr;
                    }
                    else if (groupelement is SummaryRecordEntry)
                    {
                        var record = groupelement as SummaryRecordEntry;
                        var sdr = new SpannedDataRow { RowIndex = rowIndex, GetCoveredColumnSize = GetCoveredColumnSize };

                        if ((groupelement as SummaryRecordEntry).SummaryRow.ShowSummaryInRow)
                        {
                            var cc = new CoveredCellInfo(record.Level + (this.Owner.ShowRowHeader ? 1 : 0), this.Owner.VisualContainer.ColumnCount);
                            sdr.CoveredCells.Add(cc);
                            sdr.RowType = RowType.SummaryCoveredRow;
                        }
                        else
                        {
#if !WP
                            var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.DetailsViewDefinition.Count > 0 ? 1 : 0) + (this.Owner.ShowRowHeader ? 1 : 0);
#else
                            var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.ShowRowHeader ? 1 : 0);
#endif
                            for (int i = startIndex; i < this.Owner.VisualContainer.ColumnCount; i++)
                            {
                                var cc = new CoveredCellInfo(i, i);
                                sdr.CoveredCells.Add(cc);
                            }
                            sdr.RowType = RowType.SummaryRow;
                        }
                        sdr.RowData = groupelement;
                        sdr.RowLevel = record.Level;
                        sdr.RowIndex = rowIndex;
                        sdr.DataGrid = this.Owner;
                        sdr.IsSelectedRow = this.CheckForSelection(sdr);
                        sdr.InitializeDataRow(visibleColumns);
                        return sdr;
                    }
                    else
                    {
                        var group = groupelement as Group;
                        var sdr = new SpannedDataRow
                        {
                            RowLevel = @group.Level,
                            RowIndex = rowIndex,
                            GetCoveredColumnSize = GetCoveredColumnSize
                        };
                        if (this.Owner.CaptionSummaryRow == null || this.Owner.CaptionSummaryRow.ShowSummaryInRow)
                        {
                            var cc = new CoveredCellInfo(group.Level + (this.Owner.ShowRowHeader ? 1 : 0), this.Owner.VisualContainer.ColumnCount);
                            sdr.CoveredCells.Add(cc);
                            sdr.RowType = RowType.CaptionCoveredRow;
                        }
                        else
                        {
#if !WP
                            var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.DetailsViewDefinition.Count > 0 ? 1 : 0) + (this.Owner.ShowRowHeader ? 1 : 0);
#else
                            var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.ShowRowHeader ? 1 : 0);
#endif
                            for (int i = startIndex; i < this.Owner.VisualContainer.ColumnCount; i++)
                            {
                                var cc = new CoveredCellInfo(i, i);
                                sdr.CoveredCells.Add(cc);
                            }
                            sdr.RowType = RowType.CaptionRow;
                        }
                        sdr.RowData = groupelement;
                        sdr.RowIndex = rowIndex;
                        sdr.IsSelectedRow = this.CheckForSelection(sdr);
                        sdr.DataGrid = this.Owner;
                        sdr.InitializeDataRow(visibleColumns);
                        return sdr;
                    }
                }
            }
        }

        private DataRowBase CreateFooterRow(int rowIndex, VisibleLinesCollection visibleColumns)
        {
            DataRowBase dr = null;
            if (this.Owner.GetTableSummaryCount(TableSummaryRowPosition.Bottom) == 0)
            {
                dr = new DataRow() { DataGrid = this.Owner };
                if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
                {
                    dr.RowVisibility = Visibility.Collapsed;
                }
#if WinRT
                var record = this.View[this.Owner.ResolveToRecordIndex(rowIndex)] as RecordEntry;
#else
                var record = this.View.Records[this.Owner.ResolveToRecordIndex(rowIndex)] as RecordEntry;
#endif
                dr.RowData = record.Data;
                dr.RowIndex = rowIndex;
                dr.RowRegion = RowRegion.Footer;
                dr.InitializeDataRow(visibleColumns);
            }
            else
            {
                dr = MakeTableSummaryRow(dr, rowIndex, TableSummaryRowPosition.Bottom);
                dr.RowIndex = rowIndex;
                dr.RowRegion = RowRegion.Footer;
                dr.InitializeDataRow(visibleColumns);

                if (dr.WholeRowElement is TableSummaryRowControl)
                {
                    (dr.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.FooterSummaryRow;
                    dr.VisibleColumns.ForEach(col =>
                    {
                        if (col.ColumnElement is GridIndentCell)
                            (col.ColumnElement as GridIndentCell).ColumnType = IndentColumnType.InTableSummaryRow;
                    });
                }
            }

            return dr;
        }

        private DataRowBase MakeTableSummaryRow(DataRowBase dr, int rowIndex, TableSummaryRowPosition position)
        {
            if (dr == null)
                dr = new SpannedDataRow { RowIndex = rowIndex, DataGrid = this.Owner };

            (dr as SpannedDataRow).GetCoveredColumnSize = GetCoveredColumnSize;

            var record = this.View.Records.TableSummaries.FirstOrDefault(rec => rec.SummaryRow.Equals(GetSummaryRow(rowIndex, position)));
            if (record.SummaryRow.ShowSummaryInRow)
            {
                var cc = new CoveredCellInfo(this.Owner.ResolveToScrollColumnIndex(0), this.Owner.VisualContainer.ColumnCount);
                (dr as SpannedDataRow).CoveredCells.Add(cc);
                dr.RowData = record;
                dr.RowType = RowType.TableSummaryCoveredRow;
            }
            else
            {
#if !WP
                var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.DetailsViewDefinition.Count > 0 ? 1 : 0) + (this.Owner.ShowRowHeader ? 1 : 0);
#else
                    var startIndex = this.Owner.View.GroupDescriptions.Count + (this.Owner.ShowRowHeader ? 1 : 0);
#endif
                for (int i = startIndex; i < this.Owner.VisualContainer.ColumnCount; i++)
                {
                    var cc = new CoveredCellInfo(i, i);
                    (dr as SpannedDataRow).CoveredCells.Add(cc);
                }
                dr.RowData = record;
                dr.RowType = RowType.TableSummaryRow;
            }
            return dr;
        }

        private DataRowBase CreateAddNewRow(int rowIndex, VisibleLinesCollection visibleColumns)
        {
            var dr = new DataRow() { DataGrid = this.Owner };
            if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
            {
                dr.RowVisibility = Visibility.Collapsed;
            }
            if (View != null && View.IsAddingNew)
                dr.RowData = View.CurrentAddItem;
            dr.RowIndex = rowIndex;
            if (rowIndex == Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex)
                dr.IsSelectedRow = true;
            dr.RowLevel = 0;
            dr.RowRegion = this.Owner.AddNewRowPosition == AddNewRowPosition.Top ? RowRegion.Header : RowRegion.Body;
            dr.IsAddNewRow = true;
            dr.InitializeDataRow(visibleColumns);
            return dr;
        }

        internal void RemoveStackedHeader()
        {
            for (int rowindex = 0; rowindex < this.Owner.HeaderLineCount; rowindex++)
            {
                SpannedDataRow sdr = this.Items.FirstOrDefault(row => row.RowIndex == rowindex) as SpannedDataRow;
                if (sdr != null)
                {
                    RemoveSpannedRow(sdr);
                    this.Items.Remove(sdr);
                }
            }
        }

        private void RemoveSpannedRow(SpannedDataRow sdr)
        {
            if (this.Owner.View == null)
                return;
            sdr.CoveredCells.Clear();
            sdr.VisibleColumns.ForEach(col =>
            {
                if (col.ColumnIndex >= this.Owner.View.GroupDescriptions.Count)
                    this.UnloadUIElements(sdr, col);
            });
#if !SILVERLIGHT && !WP7
            sdr.VisibleColumns.RemoveAll(col => col.ColumnIndex >= this.Owner.View.GroupDescriptions.Count);
#else
                        for (int i = sdr.VisibleColumns.Count -1 ; i >= 0; i--)
                        {
                            if (sdr.VisibleColumns[i].ColumnIndex >= this.Owner.View.GroupDescriptions.Count)
                                sdr.VisibleColumns.RemoveAt(i);
                        }
#endif
            sdr.WholeRowElement.Dispose();
            this.Container.Children.Remove(sdr.WholeRowElement);
        }

        private void Updatebinding(DataRow dr)
        {
            dr.VisibleColumns.ForEach(col =>
            {
                if (col.GridColumn != null)
                {
                    col.UpdateBinding(dr.RowData, false);
                }
            });
        }
        private void UpdateRow(IEnumerable<DataRowBase> rows, int rowIndex, RowRegion region)
        {
            if (region == RowRegion.Header)
            {
                if (rowIndex == this.Owner.GetHeaderIndex())
                {
                    DataRowBase dr = this.Items.FirstOrDefault(r => r.RowRegion == RowRegion.Header && !(r is SpannedDataRow));
                    if (dr != null)
                    {
                        if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
                            dr.RowVisibility = Visibility.Collapsed;

                        dr.RowIndex = rowIndex;
                        (dr as GridDataRow).DataGrid = this.Owner;
                        dr.RowRegion = RowRegion.Header;
                        if (this.Owner.HeaderLineCount > 1)
                        {
                            foreach (var column in this.Owner.Columns)
                            {
                                var index = this.Owner.ResolveToScrollColumnIndex(this.Owner.Columns.IndexOf(column));
                                var visibleColumn = dr.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == index);
                                if (visibleColumn != null)
                                    visibleColumn.RowSpan = this.Owner.GetHeightIncremeantationLimit(new CoveredCellInfo(index, index), rowIndex - 1);
                            }
                        }
                        else
                            dr.VisibleColumns.ForEach(column => column.RowSpan = 0);
                        dr.WholeRowElement.UpdateRowBackgroundClip();
                        dr.WholeRowElement.ItemsPanel.InvalidateMeasure();
                    }
                    else
                    {
                        var row = CreateHeaderRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                        this.Items.Add(row);
                    }
                }
                else if (rowIndex < this.Owner.StackedHeaderRows.Count)
                {
                    var sdr = this.Items.FirstOrDefault(r => r.RowRegion == RowRegion.Header) as SpannedDataRow;
                    var header = this.Owner.StackedHeaderRows[rowIndex];
                    if (sdr != null)
                    {
                        RemoveSpannedRow(sdr);
                        CreateStackedCoveredCells(sdr, header, rowIndex);
                        sdr.RowIndex = rowIndex;
                        sdr.RowData = header;
                        sdr.RowRegion = RowRegion.Header;
                        sdr.DataGrid = this.Owner;
                        sdr.InitializeDataRow(Container.ScrollColumns.GetVisibleLines());
                        sdr.WholeRowElement.UpdateRowBackgroundClip();
                    }
                    else
                    {
                        sdr = CreateHeaderRow(rowIndex, Container.ScrollColumns.GetVisibleLines()) as SpannedDataRow;
                        this.Items.Add(sdr);
                    }
                }
                else if (!Owner.IsAddNewIndex(rowIndex))
                {
                    var dr = rows.FirstOrDefault(row => row.RowType == RowType.TableSummaryCoveredRow || row.RowType == RowType.TableSummaryRow) as SpannedDataRow;
                    if (dr == null)
                    {
                        CreateFooterRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                        this.Items.Add(dr);
                        return;
                    }
                    MakeTableSummaryRow(dr, rowIndex, TableSummaryRowPosition.Top);
                    dr.RowIndex = rowIndex;
                }
            }

            else if (region != RowRegion.Footer)
            {
                if (Owner.IsAddNewIndex(rowIndex))
                {
                    if (Items.Any(row => row.IsAddNewRow))
                    {
                        var dr = Items.FirstOrDefault(row => row.IsAddNewRow) as DataRow;
                        dr.RowIndex = rowIndex;
                    }
                    else
                    {
                        var dr = CreateAddNewRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                        this.Items.Add(dr);
                    }
                }
                else
                {
                    if (this.View.GroupDescriptions.Count == 0)
                    {
                        if (rows.Any(row => row is DataRow))
                        {
                            var dr = rows.FirstOrDefault(row => row is DataRow) as DataRow;
                            if (rowIndex < 0 || rowIndex >= this.Owner.VisualContainer.ScrollRows.LineCount)
                            {
                                dr.RowVisibility = Visibility.Collapsed;
                            }
                            else
                            {
                                var record = this.View.Records[this.Owner.ResolveToRecordIndex(rowIndex)];
                                dr.RowData = record.Data;
                                if (this.ForceUpdateBinding)
                                    this.Updatebinding(dr);
                                dr.RowLevel = 0;
                                dr.RowIndex = rowIndex;
#if !WP
                                dr.IsExpanded = record.IsExpanded;
#endif
                                if (dr.RowVisibility == Visibility.Collapsed)
                                    dr.RowVisibility = Visibility.Visible;
                                dr.IsSelectedRow = this.CheckForSelection(dr);
                                dr.UpdateUnBoundColumn();
                                if (this.Owner.GridValidationMode != GridValidationMode.None)
                                {
#if !WP
                                    this.Owner.Validations.ValidateColumns(dr);
#endif
                                }
                                dr.ApplyRowHeaderVisualState();
                                dr.WholeRowElement.UpdateRowBackgroundClip();
                            }
                        }
                        else
                        {
                            var dr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                            this.Items.Add(dr);
                        }
                    }
                    else
                    {
                        var groupelement = this.View.TopLevelGroup.DisplayElements[this.Owner.ResolveToRecordIndex(rowIndex)];

                        if (groupelement is RecordEntry)
                        {
                            if (rows.Any(row => row is DataRow))
                            {
                                var dr = rows.First(row => row is DataRow) as DataRow;
                                var record = groupelement as RecordEntry;
                                dr.RowData = record.Data;
                                if (this.ForceUpdateBinding)
                                    this.Updatebinding(dr);
                                var group = record.Parent as Group;
                                if (group != null) dr.GroupRecordIndex = group.Records.IndexOf(record);
                                dr.RowIndex = rowIndex;
#if !WP
                                dr.IsExpanded = record.IsExpanded;
#endif
                                if (dr.RowVisibility == Visibility.Collapsed)
                                    dr.RowVisibility = Visibility.Visible;
                                dr.RowLevel = (record.Parent as Group).Level;
                                dr.IsSelectedRow = this.CheckForSelection(dr);
                                UpdateIndentCells(dr, record);

                                dr.UpdateUnBoundColumn();
                                if (this.Owner.GridValidationMode != GridValidationMode.None)
                                {
#if !WP
                                    this.Owner.Validations.ValidateColumns(dr);
#endif
                                }
                                dr.ApplyRowHeaderVisualState();
                                dr.WholeRowElement.UpdateRowBackgroundClip();
                            }
                            else
                            {
                                var dr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                                this.Items.Add(dr);
                            }
                        }
                        else if (groupelement is SummaryRecordEntry)
                        {
                            if (rows.Any(row => row is SpannedDataRow))
                            {
                                var record = groupelement as SummaryRecordEntry;
                                SpannedDataRow dr;

                                if (record.SummaryRow.ShowSummaryInRow)
                                {
                                    dr = rows.FirstOrDefault(row => row.RowType == RowType.SummaryCoveredRow) as SpannedDataRow;
                                    this.UpdateCoveredRow(dr, record);
                                }
                                else
                                    dr = rows.FirstOrDefault(row => row.RowType == RowType.SummaryRow) as SpannedDataRow;

                                if (dr == null)
                                {
                                    dr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines()) as SpannedDataRow;
                                    this.UpdateIndentCells(dr, record);
                                    this.Items.Add(dr);
                                    return;
                                }

                                dr.RowData = groupelement;
                                dr.RowIndex = rowIndex;
                                dr.RowLevel = (record.Parent as Group).Level;
                                if (dr.RowVisibility == Visibility.Collapsed)
                                    dr.RowVisibility = Visibility.Visible;
                                dr.IsSelectedRow = this.CheckForSelection(dr);
                                this.UpdateIndentCells(dr, record);
                                dr.ApplyRowHeaderVisualState();
                                dr.WholeRowElement.UpdateRowBackgroundClip();
                            }
                            else
                            {
                                var dr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                                this.Items.Add(dr);
                            }
                        }
                        else
                        {
                            if (rows.Any(row => row is SpannedDataRow))
                            {
                                var group = groupelement as Group;
                                SpannedDataRow dr;
                                if (this.Owner.CaptionSummaryRow == null || this.Owner.CaptionSummaryRow.ShowSummaryInRow)
                                {
                                    dr = rows.FirstOrDefault(row => row.RowType == RowType.CaptionCoveredRow) as SpannedDataRow;
                                    this.UpdateCoveredRow(dr, group);
                                }
                                else
                                    dr = rows.FirstOrDefault(row => row.RowType == RowType.CaptionRow) as SpannedDataRow;

                                if (dr == null)
                                {
                                    DataRowBase sdr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                                    this.Items.Add(sdr);
                                    return;
                                }
                                var needToEnsure = dr.RowLevel != 0 && dr.RowLevel != group.Level;
                                dr.RowLevel = group.Level;
                                dr.RowData = groupelement;
                                dr.RowIndex = rowIndex;
                                if(needToEnsure)
                                    dr.EnsureColumns(this.Owner.VisualContainer.ScrollColumns.GetVisibleLines());
                                this.UpdateGroupExpander(dr, group);
                                this.UpdateIndentCells(dr, group);
                                if (dr.RowVisibility == Visibility.Collapsed)
                                    dr.RowVisibility = Visibility.Visible;
                                dr.IsSelectedRow = this.CheckForSelection(dr);
                                dr.WholeRowElement.Clip = null;

                                dr.ApplyRowHeaderVisualState();
                                dr.WholeRowElement.UpdateRowBackgroundClip();
                            }
                            else
                            {
                                var dr = CreateDataRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                                this.Items.Add(dr);
                            }
                        }
                    }
                }
            }
            else
            {
                var record = this.View.Records.TableSummaries.FirstOrDefault(rec => rec.SummaryRow.Equals(GetSummaryRow(rowIndex, TableSummaryRowPosition.Bottom)));
                SpannedDataRow dr;
                if (record.SummaryRow.ShowSummaryInRow)
                {
                    dr = rows.FirstOrDefault(row => row.RowType == RowType.TableSummaryCoveredRow) as SpannedDataRow;
                }
                else
                {
                    dr = rows.FirstOrDefault(row => row.RowType == RowType.TableSummaryRow) as SpannedDataRow;
                }

                if (dr == null)
                {
                    var datarow = CreateFooterRow(rowIndex, this.Container.ScrollColumns.GetVisibleLines());
                    this.Items.Add(datarow);
                    return;
                }
                else
                    dr = MakeTableSummaryRow(dr, rowIndex, TableSummaryRowPosition.Bottom) as SpannedDataRow;
                dr.RowData = record;
                dr.RowIndex = rowIndex;
            }
        }

        /// <summary>
        /// Method which helps to update the expander position and expanded state
        /// </summary>
        /// <param name="row"></param>
        /// <param name="group"></param>
        /// <remarks></remarks>
        private void UpdateGroupExpander(SpannedDataRow row, Group group)
        {
            var captionRow = row.WholeRowElement as CaptionSummaryRowControl;
            captionRow.IsExpanded = group.IsExpanded;
            captionRow.InvalidateMeasure();
            captionRow.ItemsPanel.InvalidateMeasure();
        }

        private void UpdateIndentCells(DataRowBase row, object dataContext)
        {
#if !SILVERLIGHT && !WP7
            var indentCells = row.VisibleColumns.FindAll(col => col.IsIndentColumn);
#else
            var indentCells = row.VisibleColumns.Where(col => col.IsIndentColumn);
#endif

            if (Owner.IsAddNewIndex(row.RowIndex))
            {
                indentCells.ForEach(item => item.IndentColumnType = IndentColumnType.InLastGroupRow);
                return;
            }

            if (row.RowData is Group)
            {
                var group = row.RowData as Group;
                int lastGroupLevel = -1;
                if (!group.IsExpanded)
                {
                    bool isLastRow = this.IsLastRow(group, row.RowIndex, ref lastGroupLevel);
                    bool isLastGroup = IsLastGroup(group);
                    indentCells.ForEach(cell =>
                    {
                        if (cell.ColumnIndex == (row.Level + (this.Owner.ShowRowHeader ? 1 : 0) - 1))
                        {
                            cell.IndentColumnType = IndentColumnType.InExpanderCollapsed;
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Collapsed)
                                cell.ColumnElement.Visibility = Visibility.Visible;
                        }
                        else if (cell.ColumnIndex < (row.Level + (this.Owner.ShowRowHeader ? 1 : 0) - 1))
                        {
                            cell.IndentColumnType = cell.ColumnIndex < lastGroupLevel ? (isLastRow ? IndentColumnType.InLastGroupRow : IndentColumnType.BeforeExpander) : (isLastGroup ? IndentColumnType.InLastGroupRow : IndentColumnType.BeforeExpander);
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Collapsed)
                                cell.ColumnElement.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            cell.IndentColumnType = IndentColumnType.AfterExpander;
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Visible)
                                cell.ColumnElement.Visibility = Visibility.Collapsed;
                        }
                    });
                }
                else
                {
                    indentCells.ForEach(cell =>
                    {
                        if (cell.ColumnIndex == (row.Level + (this.Owner.ShowRowHeader ? 1 : 0) - 1))
                        {
                            cell.IndentColumnType = IndentColumnType.InExpanderExpanded;
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Collapsed)
                                cell.ColumnElement.Visibility = Visibility.Visible;
                        }
                        else if (cell.ColumnIndex < (row.Level + (this.Owner.ShowRowHeader ? 1 : 0) - 1))
                        {
                            cell.IndentColumnType = IndentColumnType.BeforeExpander;
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Collapsed)
                                cell.ColumnElement.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            cell.IndentColumnType = IndentColumnType.AfterExpander;
                            if (row.RowType == RowType.CaptionCoveredRow && cell.ColumnElement.Visibility == Visibility.Visible)
                                cell.ColumnElement.Visibility = Visibility.Collapsed;
                        }
                    });
                }
            }
            else if (row.RowRegion == RowRegion.Header)
            {
                indentCells.ForEach(cell => { cell.IndentColumnType = IndentColumnType.InHeader; });
            }
            else if (row.RowData is SummaryRecordEntry)
            {
                bool isLastGroupRow;
                int recordIndex = this.Owner.ResolveToRecordIndex(row.RowIndex);
                bool isLastRow = this.IsLastRow(recordIndex + 1);
                isLastGroupRow = this.View.TopLevelGroup.DisplayElements[recordIndex + 1] is Group;
                if (isLastRow)
                {
                    indentCells.ForEach(cell => { cell.IndentColumnType = IndentColumnType.InLastGroupRow; });
                }
                else if (isLastGroupRow)
                {
                    indentCells.ForEach(cell =>
                    {
                        cell.IndentColumnType = cell.ColumnIndex == 0 ? IndentColumnType.InDataRow : IndentColumnType.InLastGroupRow;
                    });
                }
                else
                    indentCells.ForEach(cell => { cell.IndentColumnType = IndentColumnType.InSummaryRow; });
            }
            else
            {
                var record = dataContext as RecordEntry;
                var group = record.Parent as Group;
                int lastGroupLevel = -1;
                NodeEntry lastRecord = group.Records[group.Records.Count - 1];
                bool isLastRow = false;
                bool isLastGroupRow = lastRecord.Equals(record);

                bool isLastGroup = (group.Parent != null && group.Parent is Group) ? group.Equals((group.Parent as Group).Groups.LastOrDefault()) : false;
                if (isLastGroupRow)
                    isLastRow = this.IsLastRow(record, row.RowIndex, ref lastGroupLevel);
                if (isLastRow)
                {
                    indentCells.ForEach(cell => { cell.IndentColumnType = IndentColumnType.InLastGroupRow; });
                }
                else if (isLastGroupRow && this.Owner.GroupSummaryRows.Count == 0)
                {
                    indentCells.ForEach(cell =>
                    {
                        if (cell.ColumnIndex < lastGroupLevel + (this.Owner.ShowRowHeader ? 1 : 0))
                        {
                            cell.IndentColumnType = isLastRow ? IndentColumnType.InLastGroupRow : IndentColumnType.InDataRow;
                        }
                        else if (cell.ColumnIndex < group.Level + (this.Owner.ShowRowHeader ? 1 : 0) - 1)
                        {
                            cell.IndentColumnType = isLastGroup ? IndentColumnType.InLastGroupRow : IndentColumnType.InDataRow;
                        }
                        else
                        {
                            cell.IndentColumnType = IndentColumnType.InLastGroupRow;
                        }
                    });
                }
                else
                    indentCells.ForEach(cell => { cell.IndentColumnType = IndentColumnType.InDataRow; });
            }
        }

        internal bool IsLastRow(int nextRecordIndex)
        {
            object nextRecord = nextRecordIndex < this.View.TopLevelGroup.DisplayElements.Count ? this.View.TopLevelGroup.DisplayElements[nextRecordIndex] : null;
            if (nextRecord is Group)
            {
                return (nextRecord as Group).Parent is TopLevelGroup;
            }
            return false;
        }

        internal bool IsLastRow(NodeEntry record, int rowIndex, ref int level)
        {
            bool isLast = false;
            //var recordIndex = this.View.TopLevelGroup.DisplayElements.IndexOf(record);
            if (this.Owner.GroupSummaryRows.Count > 0)
            {
                int originalRowIndex = rowIndex;
                if (record is RecordEntry)
                {
                    if (!(record as RecordEntry).IsExpanded)
                    {
                        originalRowIndex = rowIndex + (record.Parent as Group).GetRelationsCount();
                    }
                }
                var rec = this.View.TopLevelGroup.DisplayElements[this.Owner.ResolveToRecordIndex(originalRowIndex + 1)];
                if (rec is SummaryRecordEntry)
                    return isLast;
            }
            var parentRecord = record.Parent as Group;
            if (parentRecord != null)
            {
                if (record is RecordEntry)
                {
                    var lastIndex = parentRecord.GetRecordCount();
                    if (!(record as RecordEntry).IsExpanded)
                    {
                        isLast = parentRecord.Records[parentRecord.Records.Count - 1].Equals(record);
                    }
                    else
                    {
                        var recordIndex = this.View.TopLevelGroup.DisplayElements.IndexOf(record);
                        var rowInx = this.Owner.ResolveToRowIndex(recordIndex) + parentRecord.GetRelationsCount();
                        if (rowInx == rowIndex)
                            isLast = true;
                    }
                }
                else if (record is Group)
                {
                    if (parentRecord.Groups.Count() != 0)
                        isLast = parentRecord.Groups.LastOrDefault().Equals(record);
                }

                if (!parentRecord.IsTopLevelGroup && !(parentRecord.Parent is TopLevelGroup))
                {
                    if (isLast)
                    {
                        isLast = this.IsLastRow(parentRecord, rowIndex, ref level);
                    }
                }
                if (level < 0)
                    level = parentRecord.Level;
            }
            return isLast;
        }

        internal bool IsLastGroup(Group group)
        {
            var parentGroup = group.Parent as Group;
            var lastGroup = parentGroup.Groups.LastOrDefault();
            bool isLast = false;
            if (lastGroup != null)
                isLast = parentGroup.Groups.LastOrDefault().Equals(group);
            return isLast;
        }

        /// <summary>
        /// Method which helps to update the GroupCaption column index
        /// </summary>
        /// <param name="row"></param>
        /// <param name="group"></param>
        /// <remarks></remarks>
        private void UpdateCoveredRow(SpannedDataRow row, NodeEntry dataContext)
        {
            if (row != null)
            {
                if (row.RowType != RowType.TableSummaryCoveredRow && row.RowType != RowType.TableSummaryRow)
                {
                    var cellInfo = new CoveredCellInfo(dataContext.Level + (this.Owner.ShowRowHeader ? 1 : 0), this.Container.ScrollColumns.LineCount);
                    row.CoveredCells.Clear();
                    row.CoveredCells.Add(cellInfo);
                    row.VisibleColumns.ForEach(col =>
                    {
                        if (col.ColumnElement is GridGroupSummaryCell)
                            col.ColumnIndex = dataContext.Level + (this.Owner.ShowRowHeader ? 1 : 0);
                        else if (col.ColumnElement is GridCaptionSummaryCell)
                            col.ColumnIndex = dataContext.Level + (this.Owner.ShowRowHeader ? 1 : 0);
                        col.ColumnSpan = cellInfo.Right - cellInfo.Left;
                    });
                }
                else
                {
                    var cellInfo = new CoveredCellInfo(Owner.ResolveToScrollColumnIndex(0), this.Container.ScrollColumns.LineCount);
                    row.CoveredCells.Clear();
                    row.CoveredCells.Add(cellInfo);
                    row.VisibleColumns.ForEach(col =>
                    {
                        if (col.ColumnElement is GridTableSummaryCell)
                            col.ColumnIndex = Owner.ResolveToScrollColumnIndex(0);
                        col.ColumnSpan = cellInfo.Right - cellInfo.Left;
                    });
                }
            }
        }

        /// <summary>
        /// Update StackedHeader Covered Row after Grouping and UnGrouping
        /// </summary>
        /// <param name="row"></param>
        /// <param name="increasedIndexValue"></param>
        /// <remarks></remarks>
        internal void UpdateStackedheaderCoveredRow(SpannedDataRow row, int increasedIndexValue)
        {
            if (row != null)
            {
                List<CoveredCellInfo> coveredCells = new List<CoveredCellInfo>();
                row.CoveredCells.ForEach(cell => coveredCells.Add(cell));
                row.CoveredCells.Clear();
                coveredCells.ForEach(cell =>
                {
                    var cc = new CoveredCellInfo(cell.Row, cell.Left + increasedIndexValue, cell.Right + increasedIndexValue);
                    row.CoveredCells.Add(cc);
                });
            }
        }

        private void CollapseRow(DataRowBase row)
        {
            //row.IsEnsured = true;
            row.RowVisibility = Visibility.Collapsed;
#if !WP
            if (row is DetailsViewDataRow)
                this.Owner.DetailsViewManager.CollapsingDetailsViewDataRow((row as DetailsViewDataRow));
#endif
        }

        /// <summary>
        /// Method which will ensure whether the row is selected or not.
        /// </summary>
        /// <param name="rowIndex">Corresponding Row Index</param>
        /// <returns>Whether row is selected or not</returns>
        /// <remarks></remarks>
        private bool CheckForSelection(DataRowBase row)
        {
            var isRowSelected = this.Owner.SelectionController.SelectedRows.Contains(row.RowIndex);
#if !WP
            if (row.RowType == RowType.DefaultRow && this.Owner.NavigationMode == NavigationMode.Cell)
                return isRowSelected;
            row.IsFocusedRow = false;

            if (!isRowSelected && this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex == row.RowIndex)
                row.IsFocusedRow = true;
#endif

            return isRowSelected;
        }

        private double GetCoveredColumnSize(int start, int end)
        {
            DoubleSpan[] CurrentPos = this.Container.ScrollColumns.RangeToRegionPoints(start, end, true);
            return CurrentPos[1].Length;
        }

        private void WireViewEvents()
        {

        }

        private void UnWireViewEvents()
        {

        }

        private DataRowBase EnsureGroupCaption(int rowIndex, int actualStartIndex, int actualEndIndex)
        {
            if (rowIndex > -1)
            {
                var datarow = this.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
                if (datarow != null)
                {
                    datarow.IsEnsured = true;
                    datarow.IsFixedRow = true;
                    datarow.WholeRowElement.Clip = null;
                    if (datarow.RowVisibility == Visibility.Collapsed)
                        datarow.RowVisibility = Visibility.Visible;
                    return datarow;
                }
                else
                {
                    var rows = this.Items.Where(row => ((row.RowIndex < 0 || row.RowIndex < actualStartIndex || row.RowIndex > actualEndIndex) && row.RowRegion == RowRegion.Body && !row.IsEnsured)).ToList();
                    UpdateRow(rows, rowIndex, RowRegion.Body);
                    var newrow = this.Items.FirstOrDefault(row => row.RowIndex == rowIndex);
                    newrow.IsEnsured = true;
                    newrow.IsFixedRow = true;
                    newrow.WholeRowElement.Clip = null;
                    return newrow;
                }
            }
            return null;
        }

#if WinRT
        private async void InitializeIncrementalSource(int visibleRowCount)
#else
        private void InitializeIncrementalSource(int visibleRowCount)
#endif
        {
            if (View.HasMoreItems)
            {
                var initialLoadAmount = visibleRowCount * 2;
                var loadSize = this.Owner.DataFetchSize;
                do
                {
#if WinRT
                    await View.LoadMoreItemsAsync((uint)loadSize);
#else
                    View.LoadMoreItemsAsync((uint)loadSize);
#endif
                    loadSize *= 10;
                } while (loadSize <= initialLoadAmount);
                lastFetcheSize = loadSize;
            }
        }

#if WinRT
        private async void UpdateIncrementalSource()
#else
        private void UpdateIncrementalSource()
#endif
        {
            if (View.HasMoreItems)
            {
#if WinRT
                await View.LoadMoreItemsAsync((uint)lastFetcheSize);
#else
                View.LoadMoreItemsAsync((uint)lastFetcheSize);
#endif

            }
        }

        private bool CanUpdateSource(int endIndex)
        {
            var pivotIndex = (this.View as CollectionViewAdv).Count > this.Owner.DataFetchSize ? (this.View as CollectionViewAdv).Count - this.Owner.DataFetchSize : (this.View as CollectionViewAdv).Count;
            return endIndex >= pivotIndex && this.View.HasMoreItems;
        }

        private GridSummaryRow GetSummaryRow(int rowIndex, TableSummaryRowPosition position)
        {
            if (position == TableSummaryRowPosition.Bottom)
            {
                var startTableSummaryIndex = Owner.VisualContainer.RowCount - Owner.GetTableSummaryCount(position);
                var indexInCollection = rowIndex - startTableSummaryIndex;
                var bottomSummaries = Owner.TableSummaryRows.Where(row => (row is GridSummaryRow && !(row is GridTableSummaryRow)) || (row is GridTableSummaryRow && (row as GridTableSummaryRow).Position != TableSummaryRowPosition.Top));
                var summarRow = bottomSummaries.ElementAt(indexInCollection) as GridSummaryRow;
                return summarRow;
            }
            else
            {
                var startTableSummaryIndex = Owner.GetHeaderIndex() + 1;
                var indexInCollection = rowIndex - startTableSummaryIndex;
                var topSummaries = Owner.TableSummaryRows.Where(row => (row is GridTableSummaryRow && (row as GridTableSummaryRow).Position == TableSummaryRowPosition.Top));
                var summaryRow = topSummaries.ElementAt(indexInCollection) as GridSummaryRow;
                return summaryRow;
            }
        }

        private void EnsureFrozenGroupHeaders(VisibleLinesCollection visibleRows)
        {
            var ActualStartIndex = visibleRows[visibleRows.FirstBodyVisibleIndex].LineIndex;
            var ActualEndIndex = visibleRows[visibleRows.LastBodyVisibleIndex].LineIndex;

            var startindex = ActualStartIndex;
            var fixedRows = new List<DataRowBase>();
            while (true)
            {
                if (startindex > ActualEndIndex)
                    break;
                var recordindex = this.Owner.ResolveToRecordIndex(startindex);
                var startingRecord = this.View.TopLevelGroup.DisplayElements[recordindex];
                if (startingRecord != null)
                {
                    var record = startingRecord;
                    if (!record.IsGroups)
                        record = record.Parent;
                    if (record.IsGroups)
                    {
                        var index = this.View.TopLevelGroup.DisplayElements.IndexOf(record);
                        while (index > -1)
                        {
                            int rowIndex = index + this.Owner.HeaderLineCount;
                            if (fixedRows.All(frow => frow.RowIndex != rowIndex))
                            {
                                var group = record as Group;
                                if (!group.IsBottomLevel || (group.IsBottomLevel && rowIndex + this.Owner.GroupSummaryRows.Count + group.GetRecordCount() >= (ActualStartIndex + (group.Level - 1))))
                                {
                                    var row = EnsureGroupCaption(rowIndex, ActualStartIndex, ActualEndIndex);
                                    fixedRows.Add(row);
                                }
                            }
                            record = record.Parent;
                            if (record != null && record.IsGroups)
                                index = this.View.TopLevelGroup.DisplayElements.IndexOf(record);
                            else
                                index = -1;
                        }
                    }

                    if (startingRecord.Parent != null && startingRecord.Parent.IsGroups)
                    {
                        if (startingRecord.IsRecords)
                        {
                            var fixedrowscount = fixedRows.GroupBy(frow => frow.Level).Count();
                            if (ActualStartIndex + fixedrowscount <= startindex)
                                break;
                            else
                                startindex++;
                        }
                        else
                            startindex++;
                    }
                }
                else
                    break;
            }

            var fixeditems = this.Items.Where(item => item.IsFixedRow);
            foreach (var item in fixeditems)
            {
                if (item.RowData is Group)
                {
                    if (!(item.RowData as Group).IsExpanded)
                    {
                        item.IsFixedRow = false;
                        item.IsEnsured = false;
                    }
                }
            }

            RectangleGeometry previousClip = null;
            foreach (var captionRow in Items.Where(item => item.RowType == RowType.CaptionRow || item.RowType == RowType.CaptionCoveredRow))
            {
                if (captionRow.RowVisibility == Visibility.Collapsed)
                    continue;
                if (previousClip == null || previousClip.Rect.IsEmpty)
                {
                    VisualStateManager.GoToState(captionRow.WholeRowElement, "Normal", false);
                    (captionRow as SpannedDataRow).ApplyFixedRowVisualState(false);
                }
                previousClip = captionRow.WholeRowElement.Clip as RectangleGeometry;
            }
        }

        #endregion

        #region IRowGenerator

        IList<IRowElement> IRowGenerator.Items
        {
#if !SILVERLIGHT && !WP7
            get { return this.Items.ToList<IRowElement>(); }
#else
            get { return this.Tolist(this.Items); }
#endif
        }
#if SILVERLIGHT || WP7
        IList<IRowElement> Tolist(List<DataRowBase> items)
        {
            IList<IRowElement> rowElement = new List<IRowElement>();
            foreach (var item in items)
            {
                rowElement.Add(item);
            }
            return rowElement;
        }
#endif

        public void PregenerateRows(VisibleLinesCollection visibleRows, VisibleLinesCollection visibleColumns)
        {
            if (this.Items.Count != 0 || this.Owner.VisualContainer.RowCount <= 0) return;
            this.Owner.GridModel.InitializeGrouping();
#if !WP
            this.Owner.GridModel.InitialFiltering();
#endif

            if (this.View != null && this.View.HasMoreItems)
            {
                this.InitializeIncrementalSource(visibleRows.Count);
            }

            for (var i = 0; i < visibleRows.Count; i++)
            {
                var line = visibleRows[i];
                DataRowBase dr = null;
                switch (line.Region)
                {
                    case ScrollAxisRegion.Header:
                        dr = CreateHeaderRow(line.LineIndex, visibleColumns);
                        break;
                    case ScrollAxisRegion.Body:
                        dr = CreateDataRow(line.LineIndex, visibleColumns);
                        break;
                    case ScrollAxisRegion.Footer:
                        dr = CreateFooterRow(line.LineIndex, visibleColumns);
                        break;
                }
                if (dr != null)
                    this.Items.Add(dr);
            }
        }

        public void EnsureRows(VisibleLinesCollection visibleRows)
        {
            var hasCurrentCell = this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex != RowColumnIndex.Empty &&
                     this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex != -1;

            if (visibleRows.Count > 0 && visibleRows.FirstBodyVisibleIndex < visibleRows.Count)
            {
                var ActualStartIndex = visibleRows[visibleRows.FirstBodyVisibleIndex].LineIndex;
                var ActualEndIndex = visibleRows[visibleRows.LastBodyVisibleIndex].LineIndex;

                this.Items.ForEach(row => { row.IsEnsured = false; row.IsFixedRow = false; });

                if (CanUpdateSource(ActualEndIndex))
                {
                    this.UpdateIncrementalSource();
                }

                if (this.Owner.AllowFrozenGroupHeaders && this.View.GroupDescriptions.Count > 0)
                {
                    EnsureFrozenGroupHeaders(visibleRows);
                }

                var region = RowRegion.Header;
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                    {
                        if (visibleRows.firstBodyVisibleIndex > 0)
                        {
                            ActualStartIndex = 0;
                            ActualEndIndex = visibleRows[visibleRows.FirstBodyVisibleIndex - 1].LineIndex;
                        }
                        else
                        {
                            ActualStartIndex = 0;
                            ActualEndIndex = -1;
                        }
                        region = RowRegion.Header;
                    }
                    else if (i == 1)
                    {
                        ActualStartIndex = visibleRows[visibleRows.FirstBodyVisibleIndex].LineIndex;
                        ActualEndIndex = visibleRows[visibleRows.LastBodyVisibleIndex].LineIndex;
                        region = RowRegion.Body;
                    }
                    else
                    {

                        if (visibleRows.firstFooterVisibleIndex < visibleRows.Count)
                        {
                            ActualStartIndex = visibleRows[visibleRows.firstFooterVisibleIndex].LineIndex;
                            ActualEndIndex = visibleRows[visibleRows.Count - 1].LineIndex;
                        }
                        else
                        {
                            ActualStartIndex = 0;
                            ActualEndIndex = -1;
                        }
                        region = RowRegion.Footer;
                    }

                    for (int index = ActualStartIndex; index <= ActualEndIndex; index++)
                    {
                        if (visibleRows.All(row => row.LineIndex != index))
                            continue;
                        if (this.Items.All(row => row.RowIndex != index))
                        {
                            if (this.Items.Any(row => (row.RowIndex < 0 || row.RowIndex < ActualStartIndex || row.RowIndex > ActualEndIndex) && !row.IsEnsured))
                            {
                                IEnumerable<DataRowBase> rows;
                                if (this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex >= 0 && this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex == index && !Owner.IsAddNewIndex(index))
                                    rows = this.Items.Where(row => row.IsCurrentRow);
                                else
                                {
                                    rows = this.Items.Where(row => ((row.RowIndex < 0 || row.RowIndex < ActualStartIndex ||
                                              row.RowIndex > ActualEndIndex) && row.RowRegion == region &&
                                             !row.IsEnsured && !row.IsEditing) && !row.IsCurrentRow && !row.IsAddNewRow);
                                }
                                if (rows != null && rows.Any())
                                {
#if !WP
                                    if (region != RowRegion.Footer && this.Owner.DetailsViewManager.HasDetailsView && this.Owner.IsInDetailsViewIndex(index))
                                        this.Owner.DetailsViewManager.UpdateDetailsViewDataRow(rows, index);
                                    else
#endif
                                        UpdateRow(rows, index, region);
                                }
                            }
                        }

                        var dr = this.Items.FirstOrDefault(row => row.RowIndex == index && row.RowRegion == region);
#if !WP
                        if (dr != null && this.Owner.GridModel.HasGroup && this.Owner.IsInDetailsViewIndex(index))
                        {
                            var inx = this.Owner.ResolveToGroupRecordIndexForDetailsView(index);
                            var nodeEntry = this.View.TopLevelGroup.DisplayElements[inx];
                            UpdateIndentCells(dr, nodeEntry);
                        }
#endif
                        if (Owner.IsAddNewIndex(index))
                        {
                            if (dr != null && dr.IsAddNewRow)
                            {
                                if (dr.RowVisibility == Visibility.Collapsed)
                                {
                                    dr.RowVisibility = Visibility.Visible;
                                    dr.WholeRowElement.UpdateRowBackgroundClip();
                                }
                            }
                            else
                            {
                                dr = this.Items.FirstOrDefault(row => row.IsAddNewRow);
                                if (dr != null)
                                {
                                    if (View.IsAddingNew)
                                        dr.RowData = View.CurrentAddItem;
                                    dr.RowIndex = index;
                                    if (index == Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex && !dr.IsSelectedRow)
                                        dr.IsSelectedRow = true;
                                    dr.RowLevel = 0;
                                    dr.RowRegion = this.Owner.AddNewRowPosition == AddNewRowPosition.Top ? RowRegion.Header : RowRegion.Body;
                                    dr.IsAddNewRow = true;
                                }
                            }
                        }

                        if (dr != null)
                        {
                            if (dr.RowVisibility == Visibility.Collapsed)
                            {
                                dr.RowVisibility = Visibility.Visible;
                                dr.WholeRowElement.UpdateRowBackgroundClip();
                            }
                        }
                        else
                        {
                            if (region == RowRegion.Header)
                                dr = CreateHeaderRow(index, this.Container.ScrollColumns.GetVisibleLines());
                            else if (region == RowRegion.Footer)
                                dr = CreateFooterRow(index, this.Container.ScrollColumns.GetVisibleLines());
                            else
                                dr = CreateDataRow(index, this.Container.ScrollColumns.GetVisibleLines());

                            this.Items.Add(dr);
                        }
                        if (dr.IsAddNewRow)
                        {
                            dr.IsSelectedRow = CheckForSelection(dr);
                            if (Owner.View != null && Owner.View.GroupDescriptions.Count > 0)
                                UpdateIndentCells(dr, null);
                            (dr.WholeRowElement as AddNewRowControl).UpdateTextBorder();
                        }
                        if (dr.IsCurrentRow)
                        {
                            dr.IsCurrentRow = false;
                            (dr as GridDataRow).ApplyRowHeaderVisualState();
                        }
                        dr.IsEnsured = true;
                        if (dr.IsSelectedRow) dr.WholeRowElement.UpdateSelectionBorderClip();
                        if (dr.RowRegion != region) dr.RowRegion = region;
#if !WP
                        if (dr.RowIndex == this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex && (this.Owner.NavigationMode == NavigationMode.Cell || Owner.IsAddNewIndex(dr.RowIndex)) && (dr.IsSelectedRow || (this.Owner.SelectionMode == GridSelectionMode.Multiple && this.Owner.CurrentItem != null)))
                        {
                            dr.UpdateCurrentCellSelection();
                        }
                        if (this.Owner.DetailsViewManager.HasDetailsView && dr is DataRow) (dr as DataRow).CheckForDetailsViewExpanderVisibilty();
#endif

                    }
                }
            }
            else
            {
                this.Items.ForEach(row =>
                {
                    if (row.RowRegion == RowRegion.Header || (row.RowRegion == RowRegion.Footer && this.Owner.GetTableSummaryCount(TableSummaryRowPosition.Bottom) > 0))
                        row.IsEnsured = true;
                    else
                        row.IsEnsured = false;
                });
            }

            var footerRecords = this.Items.LastOrDefault(item => (item.RowRegion == RowRegion.Footer && item.RowIndex == Owner.VisualContainer.RowCount - 1));
            if (footerRecords != null)
                (footerRecords.WholeRowElement as TableSummaryRowControl).TableSummaryRowType = TableSummaryRowType.LastFooterSummaryRow;
#if !WP
            if (this.Owner.DetailsViewManager.HasDetailsView && this.View != null)
            {
                var expandedRecordCount = this.View.Records.Count(record => record.IsExpanded);
                if (expandedRecordCount == 0)
                {
                    var lastIndex = this.Owner.VisualContainer.ScrollColumns.LineCount - 1;
                    var lastColumn = this.Owner.Columns.LastOrDefault();
                    if (lastColumn != null && !double.IsNaN(lastColumn.ExtendedWidth))
                    {
                        var extraWidth = lastColumn.ExtendedWidth;
                        lastColumn.ExtendedWidth = double.NaN;
                        this.Owner.VisualContainer.ColumnWidths[lastIndex] -= extraWidth;
                        lastColumn.ActualWidth = this.Owner.VisualContainer.ColumnWidths[lastIndex];
                    }
                }
            }
#endif
            if (this.Owner.SelectionController.CurrentCellManager.HasCurrentCell)
            {
                var currentRow = this.Items.FirstOrDefault(row => row.RowIndex == this.Owner.SelectionController.CurrentCellManager.CurrentCellIndex.RowIndex);
                if (currentRow != null)
                {
                    currentRow.IsCurrentRow = true;
                    (currentRow as GridDataRow).ApplyRowHeaderVisualState();
                }
            }
            this.ForceUpdateBinding = false;
            this.Items.ForEach(row => { if (!row.IsEnsured) { CollapseRow(row); } });
        }

        public void EnsureColumns(VisibleLinesCollection visibleColumns)
        {
            foreach (var gridRow in this.Items)
            {
                gridRow.EnsureColumns(visibleColumns);
                gridRow.WholeRowElement.UpdateRowBackgroundClip();
            }
        }

        public void ApplyFixedRowVisualState(int index, bool canapply)
        {
            var row = Items.FirstOrDefault(item => item.RowIndex == index);
            if (row is SpannedDataRow)
            {
                if (canapply)
                {
                    VisualStateManager.GoToState(row.WholeRowElement, "FixedCaption", false);
                    (row as SpannedDataRow).ApplyFixedRowVisualState(true);
                }
                else
                {
                    VisualStateManager.GoToState(row.WholeRowElement, "Normal", false);
                    (row as SpannedDataRow).ApplyFixedRowVisualState(false);
                }
            }
        }

        public void ColumnHiddenChanged(HiddenRangeChangedEventArgs args)
        {
            this.Items.ForEach(row => row.VisibleColumns.ForEach(column =>
            {
#if !WP
                if (!(row is DetailsViewDataRow))
#endif
                    if (column.ColumnIndex >= args.From && column.ColumnIndex <= args.To)
                    {
                        column.ColumnVisibility = args.Hide ? Visibility.Collapsed : Visibility.Visible;
                    }
            }));
        }

        public void RowHiddenChanged(HiddenRangeChangedEventArgs args)
        {
            this.Items.ForEach(row =>
            {
                if (row.RowIndex >= args.From && row.RowIndex <= args.To)
                {
                    row.RowVisibility = args.Hide ? Visibility.Collapsed : Visibility.Visible;
                }
            });
        }

        public void ColumnInserted(int index, int count)
        {
            if (this.Items.Count > 0)
            {
                int visiblecolumnIndex = this.Owner.ResolveToGridVisibleColumnIndex(index);
                this.Items.ForEach(row =>
                {
#if !WP
                    if (!(row is DetailsViewDataRow) || (row is DetailsViewDataRow && visiblecolumnIndex < 0))
                    {
                        row.VisibleColumns.ForEach(col =>
                        {
                            if (index <= col.ColumnIndex)
                            {
                                col.ColumnIndex += count;
                            }
                        });
                    }
#else
                    row.VisibleColumns.ForEach(col =>
                                    {
                                        if (index <= col.ColumnIndex)
                                            col.ColumnIndex += count;
                                    });
#endif


                    if (row.RowType == RowType.SummaryCoveredRow || row.RowType == RowType.CaptionCoveredRow
                        || row.RowType == RowType.TableSummaryCoveredRow)
                    {
                        var prevCoverdCells = new List<CoveredCellInfo>();
                        foreach (var item in (row as SpannedDataRow).CoveredCells)
                        {
                            prevCoverdCells.Add(item);
                        }
                        (row as SpannedDataRow).CoveredCells.Clear();
                        prevCoverdCells.ForEach(cell =>
                            {
                                if (index < cell.Left)
                                    (row as SpannedDataRow).CoveredCells.Add(new CoveredCellInfo(cell.Left + count, cell.Right + count));
                                else if (index < cell.Right)
                                    (row as SpannedDataRow).CoveredCells.Add(new CoveredCellInfo(cell.Left, cell.Right + count));
                            });
                    }
                });
            }
        }

        public void ColumnRemoved(int index, int count)
        {
            int endIndex = index + count - 1;
            int visiblecolumnIndex = this.Owner.ResolveToGridVisibleColumnIndex(index);

            Func<DataRowBase, bool> canAdjust = row =>
            {

                if ((this.Owner.inRowHeaderChange && index == 0) || (row.RowType != RowType.SummaryCoveredRow
                    && row.RowType != RowType.CaptionCoveredRow && row.RowType != RowType.TableSummaryCoveredRow))
                    return true;
                return false;
            };

            this.Items.ForEach(row =>
            {
                if (canAdjust(row))
                {
#if !WP
                    if (row is DetailsViewDataRow)
                    {
                        row.VisibleColumns.ForEach(col =>
                        {
                            if (col.ColumnIndex >= index && col.ColumnIndex <= endIndex && !(col.ColumnElement is DetailsViewContentPresenter))
                                this.UnloadUIElements(row, col);
                        });
                    }
                    else
#endif
                        row.VisibleColumns.ForEach(col =>
                        {
                            if (col.ColumnIndex >= index && col.ColumnIndex <= endIndex)
                                this.UnloadUIElements(row, col);
                        });
                }
            });

            for (int columnindex = index; columnindex <= endIndex; columnindex++)
            {
                this.Items.ForEach(row =>
                {
                    if (canAdjust(row))
                    {
#if !WP
                        if (row is DetailsViewDataRow)
                        {
                            var isIndentcolumn = row.VisibleColumns.FirstOrDefault(col => col.ColumnIndex == columnindex && !(col.ColumnElement is DetailsViewContentPresenter));
                            if (isIndentcolumn != null)
                                row.VisibleColumns.Remove(isIndentcolumn);
                        }
                        else
#endif
                            RemoveColumnbyIndex(row, columnindex);
                    }

                });
            }

            this.Items.ForEach(row =>
            {
                if (row.RowType == RowType.SummaryCoveredRow || row.RowType == RowType.CaptionCoveredRow
                    || row.RowType == RowType.TableSummaryCoveredRow)
                {
                    var prevCoverdCells = new List<CoveredCellInfo>();
                    foreach (var item in (row as SpannedDataRow).CoveredCells)
                    {
                        prevCoverdCells.Add(item);
                    }
                    (row as SpannedDataRow).CoveredCells.Clear();
                    prevCoverdCells.ForEach(cell =>
                    {
                        if (endIndex < cell.Left)
                            (row as SpannedDataRow).CoveredCells.Add(new CoveredCellInfo(cell.Left - count, cell.Right - count));
                        else if (endIndex < cell.Right)
                            (row as SpannedDataRow).CoveredCells.Add(new CoveredCellInfo(cell.Left, cell.Right - count));
                    });
                }
#if !WP
                if (!(row is DetailsViewDataRow) || (row is DetailsViewDataRow && visiblecolumnIndex < 0))
                {
                    row.VisibleColumns.ForEach(x =>
                    {
                        if (endIndex < x.ColumnIndex) x.ColumnIndex = x.ColumnIndex - count;
                    });
                }

#else
                row.VisibleColumns.ForEach(x =>
                   {
                       if (endIndex < x.ColumnIndex) x.ColumnIndex = x.ColumnIndex - count;
                   });
#endif

            });
        }

        public void ApplyColumnSizeronInitial(double availableWidth)
        {
            this.Owner.GridColumnSizer.InitialRefreshAll(availableWidth);
            this.Owner.IsColumnSizerInitialized = true;
        }

        public void RowsArranged(Size finalSize)
        {
            if (this.Owner.GroupDropArea != null && this.Owner.GridColumnDragDropController != null)
                this.Owner.GridColumnDragDropController.UpdatePopupPosition();
        }

        public void LineSizeChanged()
        {
#if !WP
            if (Owner.NotifyListener != null && Owner is DetailsViewDataGrid)
            {
                var parentGrid = Owner.NotifyListener.GetParentDataGrid();
                DetailsViewManager.AdjustParentsWidth(parentGrid, Owner);
                var dr = parentGrid.RowGenerator.Items.FirstOrDefault(
                    row => (row is DetailsViewDataRow) && (row as DetailsViewDataRow).DetailsViewDataGrid == Owner);
                if (dr != null && dr.WholeRowElement.ItemsPanel != null)
                    dr.WholeRowElement.ItemsPanel.InvalidateMeasure();
            }
#endif
        }
        #endregion

        public void Dispose()
        {
            if (this.Items != null)
            {
                foreach (var item in Items)
                    item.Dispose();
                this.Items.Clear();
                this.Items = null;
            }
            this.Owner = null;
        }
    }
}

