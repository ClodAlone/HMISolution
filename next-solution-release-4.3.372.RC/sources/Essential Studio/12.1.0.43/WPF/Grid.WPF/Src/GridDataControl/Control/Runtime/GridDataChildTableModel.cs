#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Controls.Cells;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// Specifies the child TableModel for a <see cref="GridDataRelation"/>. It is inherited from <see cref="GridDataTableModel"/>
    /// and has additional properties with the relational objects.
    /// </summary>
    public class GridDataChildTableModel : GridDataTableModel
#if SILVERLIGHT
        , IGridDataChildModelInteractivity
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChildGroup"/> class.
        /// </summary>
        /// <param name="relation">The relation.</param>
        public GridDataChildTableModel(GridDataRelation relation)
            : base()
        {
            this.Relation = relation;
            var visibleColumns = this.Relation.TableProperties.VisibleColumns;
            if (visibleColumns != null)
            {
                ((INotifyCollectionChanged)visibleColumns).CollectionChanged += new NotifyCollectionChangedEventHandler(ChildTableModelVisibleColumns_CollectionChanged);
            }
            foreach (var visibleCol in visibleColumns)
            {
                visibleCol.PropertyChanged += visibleCol_PropertyChanged;
            }
        }
        void visibleCol_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HeaderText")
            {
                var relationVisibleColumn = sender as GridDataVisibleColumn;
                var childVisibleColumn = this.TableProperties.VisibleColumns[relationVisibleColumn.MappingName];
                childVisibleColumn.HeaderText = relationVisibleColumn.HeaderText;
            }
        }

        /// <summary>
        /// This method fires when there is a change in VisibleColumns of ChildTableModel. By listening the event we made the changes to the TableProperties.
        /// </summary>
        void ChildTableModelVisibleColumns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (this.Grid == null || this.IsInSuspend)
            {
                return;
            }
            this.IsInSourceListChanged = true;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (GridDataVisibleColumn v in e.NewItems)
                    {
                        v.PropertyChanged += visibleCol_PropertyChanged;
                        this.TableProperties.VisibleColumns.Add(v);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Reset();
                if (this.TableProperties.ItemsSource != null)
                    this.ColumnCount = this.TableProperties.ShowRowHeader ? 1 : 0;
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                var prevColumn = e.OldItems[e.OldStartingIndex] as GridDataVisibleColumn;
                this.TableProperties.VisibleColumns.UnwireVisibleColumnDescriptor(prevColumn);
                var newColumn = e.NewItems[e.NewStartingIndex] as GridDataVisibleColumn;
                this.TableProperties.VisibleColumns.WireVisibleColumnDescriptor(newColumn);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                this.IsInSourceListChanged = true;
                var column = e.OldItems[0] as GridDataVisibleColumn;
                this.TableProperties.VisibleColumns.UnwireVisibleColumnDescriptor(column);
                column.PropertyChanged -= visibleCol_PropertyChanged;
                this.RemoveColumns(e.OldStartingIndex, 1);

            }
            if (this.Table.HasStackedHeaders)
            {
                var range = GridRangeInfo.Rows(0, this.TableProperties.StackedHeaderRows.Count);
                this.InvalidateCell(range);
            }

            if (this.ColumnAutoSizer != null)
            {
                this.ColumnAutoSizer.RefreshAll();
            }
            this.IsInSourceListChanged = false;
        } 

        /// <summary>
        /// Gets the relation.
        /// </summary>
        /// <value>The relation.</value>
        public GridDataRelation Relation
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the RowFilter for relational DataTable.
        /// </summary>
        public string RowFilter
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the selected childgrid selected item.
        /// </summary>
        /// <value>The selected childgrid selected item.</value>
        public object ChildGridSelectedItem
        {
            get;
            internal set;
        }
        /// <summary>
        /// Gets the selected childgrid selected items collection.
        /// </summary>
        /// <value>The selected childgrid selecteditems.</value>
        public ObservableCollection<object> ChildGridSelectedItems
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the parent table.
        /// </summary>
        /// <value>The parent table.</value>
        public GridDataTable ParentTable
        {
            get
            {
                if (this.ParentRecord != null)
                {
                    return this.ParentRecord.Model.Table;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the parent record.
        /// </summary>
        /// <value>The parent record.</value>
        public GridDataRecord ParentRecord
        {
            get;
            internal set;
        }
        #region Selected child model
        private GridDataChildTableModel _selectedChildModel;
        public GridDataChildTableModel SelectedChildModel
        {
            get
            {
                return _selectedChildModel;
            }
            internal set
            {
                var oldValue = _selectedChildModel;
                _selectedChildModel = value;
                if (oldValue != value)
                    SelectedChildModelChanged(oldValue, value);
            }
        }
        private void SelectedChildModelChanged(GridDataChildTableModel oldModelValue, GridDataChildTableModel newModelValue)
        {
            if (oldModelValue != null)
            {
                if (oldModelValue.Grid != null)
                {
                    var nestedGrid = oldModelValue.Grid as GridDataCellNestedGridEditor;
                    if (nestedGrid != null)
                        nestedGrid.ClearChildGridSelections(oldModelValue);
                }
            }
            if (newModelValue != null)
            {
                var SelectedRanges = this.SelectedRanges.Clone();
                this.Selections.Clear();
                this.SelectedRanges.Clear();
                if (this.Grid.CurrentCell.IsEditing)
                    this.Grid.CurrentCell.EndEdit();
                this.ChildGridSelectedItem = null;
                this.ChildGridSelectedItems.Clear();
                foreach (GridRangeInfo range in SelectedRanges)
                    this.Grid.InvalidateRenderCell(range);
            }
        }
        #endregion
        internal void WireRelationalSourceList(ICollectionViewAdv CollectionView)
        {
            
            this.SetSourceList(CollectionView.SourceCollection, true, true, CollectionView);
        }

        protected override void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            if (ParentTable != null && ParentTable.Model != null)
                e.CellModel = ParentTable.Model.CellModels[e.CellType].Clone();
            base.OnQueryCellModel(e);
        }

        protected override void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            base.OnQueryBaseStyles(e);

            var nestedGridVisualStyle = this.GridVisualStyle as IGridDataNestedVisualStyle;
            if (nestedGridVisualStyle == null)
            {
                return;
            }

            if (e.Cell.RowIndex == 0)
            {
                if (e.Cell.ColumnIndex == 0)
                {
                    e.BaseStyles.Add(new GridStyleInfo() { Borders = this.GetTopLeftCellHeaderCellBorder(nestedGridVisualStyle) });
                }
                else if (e.Cell.ColumnIndex > 0 && e.Cell.ColumnIndex <= this.ColumnCount)
                {
                    e.BaseStyles.Add(new GridStyleInfo() { Borders = this.GetNestedHeaderCellBorder(nestedGridVisualStyle) });
                }
            }
            else if (e.Cell.RowIndex > 0 && e.Cell.ColumnIndex == 0)
            {
                e.BaseStyles.Add(new GridStyleInfo() { Borders = this.GetFirstHeaderColumnBorder(nestedGridVisualStyle) });
            }
            else if (e.Cell.RowIndex > 0 && e.Cell.ColumnIndex == this.ColumnCount)
            {
                e.BaseStyles.Add(new GridStyleInfo() { Borders = this.GetLastHeaderColumnBorder(nestedGridVisualStyle) });
            }
        }

        public event EventHandler NestedGridLoaded;
        public event EventHandler NestedGridUnLoaded;

        internal void RaiseNestedGridLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NestedGridLoaded != null)
            {
                this.NestedGridLoaded(sender, e);
            }
        }

        internal void RaiseNestedGridUnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (NestedGridUnLoaded != null)
            {
                this.NestedGridUnLoaded(sender, e);
            }
        }

        internal CellBordersInfo GetTopLeftCellHeaderCellBorder(IGridDataNestedVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.NestedGridAppearance != null && TableProperties.StyleManager.NestedGridAppearance.TopLeftCellHeaderCellBorder != null)
                return TableProperties.StyleManager.NestedGridAppearance.TopLeftCellHeaderCellBorder;

            return value.TopLeftCellHeaderCellBorder;
        }

        internal CellBordersInfo GetNestedHeaderCellBorder(IGridDataNestedVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.NestedGridAppearance != null && TableProperties.StyleManager.NestedGridAppearance.NestedHeaderCellBorder != null)
                return TableProperties.StyleManager.NestedGridAppearance.NestedHeaderCellBorder;

            return value.NestedHeaderCellBorder;
        }

        internal CellBordersInfo GetFirstHeaderColumnBorder(IGridDataNestedVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.NestedGridAppearance != null && TableProperties.StyleManager.NestedGridAppearance.FirstHeaderColumnBorder != null)
                return TableProperties.StyleManager.NestedGridAppearance.FirstHeaderColumnBorder;

            return value.FirstHeaderColumnBorder;
        }

        internal CellBordersInfo GetLastHeaderColumnBorder(IGridDataNestedVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.NestedGridAppearance != null && TableProperties.StyleManager.NestedGridAppearance.LastHeaderColumnBorder != null)
                return TableProperties.StyleManager.NestedGridAppearance.LastHeaderColumnBorder;

            return value.LastHeaderColumnBorder;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var visibleColumns = this.Relation.TableProperties.VisibleColumns;
                if (visibleColumns != null)
                {
                    ((INotifyCollectionChanged)visibleColumns).CollectionChanged -= new NotifyCollectionChangedEventHandler(ChildTableModelVisibleColumns_CollectionChanged);

                    foreach (var visibleCol in visibleColumns)
                    {
                        visibleCol.PropertyChanged -= visibleCol_PropertyChanged;
                    }
                }
            }
            base.Dispose(disposing);
        }

        public override void UpdateSelectedRanges()
        {
            this.SelectedRanges.Clear();
            var removeItems = new List<object>();
            // when sorting the current item sets, so the selection made from current item.
            if (this.ChildGridSelectedItems.Count == 0 && this.Grid != null && this.Grid.CurrentCell != null && this.Grid.CurrentCell.HasCurrentCell && this.Grid.CurrentCell.RowIndex != 0)
            {
                var recordIndex = this.ResolveIndexToRecordPosition(this.Grid.CurrentCell.RowIndex);
                if (recordIndex != -1)
                {
                    var record = this.View.Records[recordIndex];
                    this.ChildGridSelectedItems.Add(record.Data);
                }
            }
            foreach (var childRecord in this.ChildGridSelectedItems)
            {
                int recordIndex = -1;
                int rowIndex = -1;

                recordIndex = this.View.Records.IndexOfRecord(childRecord);
                rowIndex = this.ResolvePositionToIndex(recordIndex);

                if (recordIndex > -1)
                {
                    var selectRange = GridRangeInfo.Row(rowIndex);
                    this.SelectedRanges.Add(selectRange);
                    if (this.Grid != null)
                        this.Grid.InvalidateCell(selectRange);
                }
                else
                {
                    removeItems.Add(childRecord);
                }
            }

            foreach (var record in removeItems)
                this.ChildGridSelectedItems.Remove(record);

            if (!this.ChildGridSelectedItems.Contains(this.ChildGridSelectedItem) &&
                this.ChildGridSelectedItem != null)
                this.ChildGridSelectedItem = null;

            this.Table.RaiseRecordsSelectionChanged(new GridDataRecordsSelectionChangedEventArgs(removeItems,
                                                                                                       null));
            if (this.Table.HasGroups && this.SelectedRanges.Count == 0)
            {
                int startIndex = this.ResolveStartIndexBasedOnPosition();
                this.SelectedRanges.Add(GridRangeInfo.Row(startIndex));
                this.InvalidateCell(GridRangeInfo.Row(startIndex));
            }
        }

#if SILVERLIGHT
        #region IGridDataChildModelInteractivity Members

        GridModel IGridDataChildModelInteractivity.GetParentModel()
        {
            if (this.ParentTable != null)
            {
                return this.ParentTable.Model;
            }

            return null;
        }

        #endregion
#endif
    }

#if SILVERLIGHT
    public interface IGridDataChildModelInteractivity
    {
        GridModel GetParentModel();
    }
#endif
}
