#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if WinRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Utility;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.UI.Xaml.Utility;
#endif

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public abstract class DataRowBase : IRowElement, IDisposable 
    {
        #region Fields

        bool isSelectedRow = false;
        List<DataColumnBase> visibleColumns = new List<DataColumnBase>();
        object rowData = null;
        private bool isEditing;
        int rowIndex = -1;
        Visibility rowVisibility = Visibility.Visible;
        RowType rowType = RowType.DefaultRow;
        bool isFixedRow = false;
        Rect arrangeRect;
        protected bool suspendUpdateStyle;
        internal bool IsEnsured;
        internal int RowLevel = 1;
        internal VirtualizingCellsControl WholeRowElement;
        internal RowRegion RowRegion = RowRegion.Body;

        #endregion

        #region Property
#if !WP
        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnExpandedStateChanged();
            }
        }
#endif
        public bool IsEditing
        {
            get { return isEditing; }
            set { isEditing = value; }
        }
        

        public object RowData
        {
            get 
            { 
                return rowData;
            }
            set
            {
                this.rowData = value;
                OnRowDataChanged();
            }
        }

        public int RowIndex
        {
            get 
            { 
                return rowIndex;
            }
            internal set
            {
                rowIndex = value;
                OnRowIndexChanged();
            }
        }

        public RowType RowType
        {
            get { return rowType; }
            internal set { rowType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this row is selected or not
        /// </summary>
        /// <value><see langword="true"/> if this instance ; otherwise, <see langword="false"/>.</value>
        /// <remarks></remarks>
        public bool IsSelectedRow
        {
            get
            {
                return isSelectedRow;
            }
            set
            {
                if (value == isSelectedRow) return;
                isSelectedRow = value;
                OnPropertyChanged("IsSelectedRow");
            }
        }

        private bool isFocusedRow;

        /// <summary>
        /// Get or sets the value indicating row is focused or not.
        /// </summary>
        public bool IsFocusedRow
        {
            get { return isFocusedRow; }
            set 
            {
                if (value == isFocusedRow) return;
                isFocusedRow = value;
                OnPropertyChanged("IsFocusedRow");
            }
        }
        

        private bool isCurrentRow;

        public bool IsCurrentRow
        {
            get 
            {
                return isCurrentRow; 
            }
            internal set 
            {
                isCurrentRow = value; 
            }
        }
        
        public bool IsFixedRow
        {
            get { return isFixedRow; }
            internal set { isFixedRow = value; }
        }

        private bool isAddNewRow;

        public bool IsAddNewRow
        {
            get { return isAddNewRow; }
            internal set { isAddNewRow = value; OnPropertyChanged("IsAddNewRow"); }
        }

        #endregion
        
        #region internal Properties

        internal Visibility RowVisibility
        {
            get
            {
                return rowVisibility;
            }
            set
            {
                rowVisibility = value;
                OnRowVisibilityChanged();
            }
        }

        internal List<DataColumnBase> VisibleColumns
        {
            get { return visibleColumns; }
        }

        #endregion

        #region Property changed

        private void OnRowDataChanged()
        {
            if (this.WholeRowElement != null)
            {
                this.WholeRowElement.DataContext = this.rowData;
                foreach (var dataColumn in this.VisibleColumns.Where(column => column.GridColumn is GridTemplateColumn))
                {
                    dataColumn.UpdateBinding(this.rowData, false);
                }
            }
        }

        protected virtual void OnRowIndexChanged()
        {
            if (this.RowIndex > 0)
            {
                this.VisibleColumns.ForEach(col => { col.RowIndex = this.RowIndex; });
                if (!suspendUpdateStyle)
                    this.UpdateRowStyles(this.WholeRowElement);
            }
        }

        private void OnRowVisibilityChanged()
        {
            this.WholeRowElement.Visibility = this.rowVisibility;
            if (this.RowVisibility == Visibility.Visible)
            {
                if (this.IsCurrentRow)
                {
                    var column = this.VisibleColumns.FirstOrDefault(dataColumn => dataColumn.IsSelectedColumn);
                    if (column != null && column.IsEditing && column.Renderer != null && column.Renderer.HasCurrentCellState)
                        column.Renderer.SetFocus(true);
                }
            }
        }

#if !WP
        protected virtual void OnExpandedStateChanged()
        {
            
        }
#endif
        #endregion

        #region Ctor

        protected DataRowBase()
        {

        }

        #endregion

        #region internal methods

        internal void InitializeDataRow(VisibleLinesCollection visibleColumns)
        {
            this.OnGenerateVisibleColumns(visibleColumns);
            this.WholeRowElement = OnCreateRowElement();
        }

        internal void SuspendUpdateStyle()
        {
            this.suspendUpdateStyle = true;
            this.VisibleColumns.ForEach(column => column.isSuspendUpdateStyle = true);
        }

        internal void ResumeUpdateStyle()
        {
            this.suspendUpdateStyle = false;
            this.VisibleColumns.ForEach(column => column.isSuspendUpdateStyle = false);
        }

        #endregion

        #region abstract methods

        protected abstract VirtualizingCellsControl OnCreateRowElement();

        protected abstract void OnGenerateVisibleColumns(VisibleLinesCollection visibleColumnLines);

        internal abstract void EnsureColumns(VisibleLinesCollection visibleColumnLines);

        internal abstract void UpdateRowStyles(ContentControl row);

        #endregion

        #region protected methods

        protected IList<IColumnElement> GetVisibleColumns()
        {
#if !SILVERLIGHT && !WP
            return VisibleColumns.ToList<IColumnElement>();
#else
            return Tolist(visibleColumns);
#endif
        }

#if SILVERLIGHT || WP
        IList<IColumnElement> Tolist(List<DataColumnBase> items)
        {
            IList<IColumnElement> rowElement = new List<IColumnElement>();
            foreach (var item in items)
            {
                rowElement.Add(item);
            }
            return rowElement;
        }
#endif

        protected void CollapseColumn(DataColumnBase column)
        {
            column.IsEnsured = true;
            column.ColumnVisibility = Visibility.Collapsed;
        }

        protected virtual Rect RowManupulation(Rect rect)
        {
            if (this.RowRegion == Grid.RowRegion.Header)
                return new Rect()
                {
                    Height = rect.Height,
                    Width = rect.Width,
                    Y = rect.Y,
                    X = rect.X - this.WholeRowElement.BorderThickness.Left
                };
            if (this.RowRegion == Grid.RowRegion.Footer)
                return new Rect()
                {
                    Height = rect.Height,
                    Width = rect.Width,
                    Y = rect.Y - this.WholeRowElement.BorderThickness.Top,
                    X = rect.X - this.WholeRowElement.BorderThickness.Left
                };
            return rect;
        }

        public virtual void MeasureElement(Size size)
        {
            this.WholeRowElement.Measure(size);
        }

        public virtual void ArrangeElement(Rect rect)
        {
            this.WholeRowElement.Arrange(rect);
        }

        internal virtual void UpdateCurrentCellSelection()
        {
           
        }

        protected abstract DataColumnBase CreateIndentColumn(int index);
       

        #endregion

        #region IDataRow

        FrameworkElement IElement.Element
        {
            get { return this.WholeRowElement; }
        }

        int IElement.Index
        {
            get { return this.RowIndex; }
        }

        RowRegion IRowElement.RowRegion
        {
            get { return this.RowRegion; }
        }

        public int Level
        {
            get { return RowLevel; }
        }

        Rect IRowElement.ArrangeRect
        {
            get { return arrangeRect; }
            set { arrangeRect = value; }
        }

        Rect IRowElement.RowManupulation(Rect rect)
        {
            return RowManupulation(rect);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            IElement thisdr = this;
            var dr = obj as IElement;
            if (dr != null)
            {
                if (thisdr.Index > dr.Index)
                    return 1;
                else if (thisdr.Index < dr.Index)
                    return -1;
                else
                    return 0;
            }
            return 0;
        }
        #endregion

        #endregion

        #region Dispose

        public virtual void Dispose()
        {
            if (this.visibleColumns != null)
            {
                foreach (var item in visibleColumns)
                    item.Dispose();
                visibleColumns.Clear();
                visibleColumns = null;
            }
            if (this.WholeRowElement != null)
            {
                WholeRowElement.Dispose();
                this.WholeRowElement = null;
            }
            this.rowData = null;
        }

        #endregion
    }

    public abstract class GridDataRow : DataRowBase
    {
        #region Property

        internal SfDataGrid DataGrid
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods
#if !WP
        protected override void OnExpandedStateChanged()
        {
            if (DataGrid == null || DataGrid.DetailsViewDefinition == null || DataGrid.DetailsViewDefinition.Count <= 0)
                return;
            var expanderColumn = this.VisibleColumns.FirstOrDefault(col => col.IsExpanderColumn);
            if (expanderColumn == null || this.RowRegion == Grid.RowRegion.Header) return;
            var control = expanderColumn.ColumnElement as GridDetailsViewExpanderCell;
            if (control != null) control.IsExpanded = IsExpanded;
        }
#endif
        protected VisibleLineInfo GetColumnVisibleLineInfo(int index)
        {
            VisibleLineInfo line = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLineAtLineIndex(index);
            return line;
        }

        protected double GetVisibleLineOrigin()
        {
            int repeatSizeCount;
            int columnIndex = this.RowData is Group ? this.RowLevel - 1 : this.RowLevel;
            if (this.DataGrid.ShowRowHeader)
                columnIndex += 1;
#if !WP
            if (this.DataGrid.DetailsViewManager.HasDetailsView && this.RowType == RowType.DefaultRow)
                columnIndex += 1;
#endif
            if (this.RowLevel < 0)
                columnIndex = 0;
            if (this.DataGrid.VisualContainer.ColumnWidths.GetHidden(columnIndex, out repeatSizeCount))
            {
                columnIndex += repeatSizeCount;
            }
            var lineInfo = this.GetColumnVisibleLineInfo(columnIndex);
            if (lineInfo == null)
            {
                var lines = this.DataGrid.VisualContainer.ScrollColumns.GetVisibleLines();
                if (lines.Count > lines.FirstBodyVisibleIndex)
                    lineInfo = lines[lines.FirstBodyVisibleIndex];
                else
                    return 0;
            }
            return lineInfo.ClippedOrigin;
        }

        protected bool AllowRowHoverHighlighting()
        {
            return DataGrid.AllowRowHoverHighlighting;
        }

        protected virtual double GetColumnSize(int index, bool lineNull)
        {
            if (lineNull)
            {
                DoubleSpan[] CurrentPos = this.DataGrid.VisualContainer.ScrollColumns.RangeToRegionPoints(index, index, true);
                return CurrentPos[1].Length;
            }
            var line = GetColumnVisibleLineInfo(index);
            if (line == null)
                return 0;
            return line.Size;
        }

        protected void SetSelectionBorderBindings(VirtualizingCellsControl cellsControl)
        {
            var bind = new Binding
            {
                Path = new PropertyPath("RowSelectionBrush"),
                Source = DataGrid.SelectionController,
                Mode = BindingMode.TwoWay
            };
            cellsControl.SetBinding(VirtualizingCellsControl.RowSelectionBrushProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("IsSelectedRow"),
                Source = this,
                Converter = new BoolToVisiblityConverter(),
                Mode = BindingMode.TwoWay
            };
            cellsControl.SetBinding(VirtualizingCellsControl.SelectionBorderVisiblityProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("IsFocusedRow"),
                Source = this,
                Converter = new BoolToVisiblityConverter(),
                Mode = BindingMode.TwoWay
            };
            cellsControl.SetBinding(VirtualizingCellsControl.CurrentFocusRowVisibilityProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("GroupRowSelectionBrush"),
                Source = DataGrid.SelectionController,
                Mode = BindingMode.TwoWay
            };
            cellsControl.SetBinding(VirtualizingCellsControl.GroupRowSelectionBrushProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("RowHoverBackgroundBrush"),
                Source = DataGrid.SelectionController,
                Mode = BindingMode.TwoWay
            };
            cellsControl.SetBinding(VirtualizingCellsControl.RowHoverBackgroundBrushProperty, bind);
        }

        protected void SetCurrentCellBorderBinding(UIElement columnElement)
        {
            if (!(columnElement is GridCell)) return;
            var bind = new Binding
            {
                Path = new PropertyPath("CurrentCellBorderThickness"),
                Source = DataGrid,
                Mode = BindingMode.TwoWay
            };
            ((GridCell)columnElement).SetBinding(GridCell.CurrentCellBorderThicknessProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("CurrentCellBorderBrush"),
                Source = DataGrid,
                Mode = BindingMode.TwoWay
            };
            ((GridCell)columnElement).SetBinding(GridCell.CurrentCellBorderBrushProperty, bind);
        }

        public override void Dispose()
        {
            base.Dispose();
            this.DataGrid = null;
        }

        protected void CreateRowHeaderColumn(int index)
        {
            DataColumnBase dc = new DataColumn();
            dc.IsEnsured = true;
            dc.RowIndex = this.RowIndex;
            dc.ColumnIndex = index;
            dc.IsEditing = false;
            dc.GridColumn = null;
            dc.Renderer = this.DataGrid.CellRenderers["RowHeader"];
            dc.SelectionController = this.DataGrid.SelectionController;
            if (this.RowIndex >= 0 && this.RowIndex <= this.DataGrid.GetHeaderIndex())
            {
                dc.Renderer = null;
                dc.ColumnElement = new GridRowHeaderIndentCell();
            }
            else
                dc.InitializeColumnElement(this.RowData, false);
            this.VisibleColumns.Add(dc);
            ApplyRowHeaderVisualState();
        }

        #endregion

        #region Internal Methods
        internal void ApplyRowHeaderVisualState()
        {
            var columnBase = this.VisibleColumns.FirstOrDefault(col => col.ColumnElement is GridRowHeaderCell);
            if (columnBase == null)
                return;
            GridRowHeaderCell rowHeaderCell = columnBase.ColumnElement as GridRowHeaderCell;
            if (!this.DataGrid.ShowRowHeader)
                return;
            if (rowHeaderCell != null)
            {
                if (this.IsCurrentRow)
                    rowHeaderCell.State = "CurrentRow";
                else if (this.IsAddNewRow)
                    rowHeaderCell.State = "AddNewRow";
                else
                    rowHeaderCell.State = "Normal";

                if (this.IsEditing)
                    rowHeaderCell.State = "EditingRow";

                var dataValidation = this.RowData;

                if (dataValidation != null)
                {
#if SyncfusionFramework4_5 || SILVERLIGHT
                    if ((dataValidation as INotifyDataErrorInfo) != null)
                    {
                        if (DataValidation.ValidateRowINotifyDataErrorInfo(this.RowData))
                        {
                            rowHeaderCell.RowErrorMessage = GridResourceWrapper.RowErrorMessage;
                            if (!rowHeaderCell.State.Equals("Normal"))
                                rowHeaderCell.State = "Error_CurrentRow";
                            else
                                rowHeaderCell.State = "Error";
                            rowHeaderCell.ApplyVisualState();
                            return;
                        }
                        else
                        {
                            if (this.DataGrid.View.CurrentItem == null)
                                rowHeaderCell.State = "Normal";
                        }
                    }
#endif
#if !WinRT
                    if ((dataValidation as IDataErrorInfo) != null)
                    {
                        if (DataValidation.ValidateRow(this.RowData))
                        {
                            rowHeaderCell.RowErrorMessage = (dataValidation as IDataErrorInfo).Error;
                            if (!rowHeaderCell.State.Equals("Normal"))
                                rowHeaderCell.State = "Error_CurrentRow";
                            else
                                rowHeaderCell.State = "Error";
                            rowHeaderCell.ApplyVisualState();
                            return;
                        }
                        else
                        {
                            if (this.DataGrid.View.CurrentItem == null && !this.IsAddNewRow)
                                rowHeaderCell.State = "Normal";
                        }
                    }
#endif
                }
                if (this.RowType == Grid.RowType.TableSummaryRow || this.RowType == Grid.RowType.TableSummaryCoveredRow)
                    rowHeaderCell.State = "Footer";
                rowHeaderCell.ApplyVisualState();
                
            }
        }
        #endregion
    }
}
