//-------------------------------------------------------------------------------------------------
// <copyright file="GridFilterBar.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.ComponentModel;

using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;
using Syncfusion.Styles;
using Syncfusion.Diagnostics;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    #region Filter Bar Events
    /// <summary>
    /// Represents a method that handles the <see cref="GridFilterBar.FilterBarTextChanged"/> event.
    /// </summary>
    public delegate void GridFilterBarTextChangedEventHandler(object sender, GridFilterBarTextChangedEventArgs e);

    /// <summary>
    /// Represents a method that handles the <see cref="GridFilterBar.CreatingColumnHeader"/> event.
    /// </summary>
    public delegate void GridFilterBarCreatingColumnHeaderEventHandler(object sender, GridFilterBarCreatingColumnHeaderEventArgs e);

    /// <summary>
    /// Represents a method that handles the <see cref="GridFilterBar.FilterBarShowDialog"/> event.
    /// </summary>
    public delegate void GridFilterBarShowDialogEventHandler(object sender, GridFilterBarShowDialogEventArgs e);

    /// <summary>
    /// Represents a method that handles the <see cref="GridFilterBar.FilterBarFilterCompleted"/> event.
    /// </summary>
    public delegate void GridFilterBarFilterCompletedEventHandler(object sender, GridFilterBarTextChangedEventArgs e);

    /// <summary>
    /// EventArgs used by the <see cref="GridFilterBar.CreatingColumnHeader"/> event.
    /// </summary>
    public class GridFilterBarCreatingColumnHeaderEventArgs : SyncfusionCancelEventArgs
    {
        private string _colName;

        /// <summary>
        ///  Constructor for GridFilterBarCreatingColumnHeaderEventArgs.
        /// </summary>
        /// <param name="colName">Mapping name of the column that is about to have a
        /// header cell created for it.</param>
        public GridFilterBarCreatingColumnHeaderEventArgs(string colName)
            : base()
        {
            _colName = colName;
        }

        /// <summary>
        /// Gets Mapping name of the column.
        /// </summary>
        [TraceProperty(true)]
        public string ColName
        {
            get { return _colName; }
        }
    }

    /// <summary>
    /// EventArgs used by the <see cref="GridFilterBar.FilterBarTextChanged"/> event.
    /// </summary>
    public class GridFilterBarTextChangedEventArgs : SyncfusionEventArgs
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public GridFilterBarTextChangedEventArgs()
            : base()
        {
        }
    }

    /// <summary>
    /// EventArgs used by the <see cref="GridFilterBar.FilterBarShowDialog"/> event.
    /// </summary>
    public sealed class GridFilterBarShowDialogEventArgs : SyncfusionHandledEventArgs
    {
        string filterCriteria;
        DialogResult result;

        /// <summary>
        /// Initializes the event args.
        /// </summary>
        /// <param name="filterCriteria">The filter criteria.</param>
        /// <param name="result">The dialog result.</param>
        public GridFilterBarShowDialogEventArgs(string filterCriteria, DialogResult result)
        {
            this.filterCriteria = filterCriteria;
            this.result = result;
        }

        /// <summary>
        /// Gets or sets the filter criteria.
        /// </summary>
        [TraceProperty(true)]
        public string FilterCriteria
        {
            get
            {
                return filterCriteria;
            }

            set
            {
                filterCriteria = value;
            }
        }

        /// <summary>
        /// Gets or sets the dialog result (if Handled = True was set).
        /// </summary>
        [TraceProperty(true)]
        public DialogResult Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }
    #endregion

    /// <summary>
    /// Encapsulates the functionality of adding a header row of combo boxes
    /// to a GridDataBoundGrid so the rows displayed in the grid can be filtered
    /// using the combo boxes.
    /// </summary>
    /// <remarks>
    /// Please note that currently, FilterBars are only supported for simple 
    /// GridDataBoundGrids that are not hierarchical. 
    /// </remarks>
    public class GridFilterBar
    {
        /// <summary>
        /// Initializes a new <see cref="GridFilterBar"/> object.
        /// </summary>
        public GridFilterBar()
        {
            _grid = null;
            _style = null;
            _row = -1;
            _dataTable = null;
        }

        #region Events
        /// <summary>
        /// Raised immediately prior to the GridFilterBar.RowFilter property changing.
        /// </summary>
        /// <remarks>Use this event if you need to know when the RowFilter is about to change.
        /// For example, handling this event will allow you to display the current row filter.</remarks>
        public event GridFilterBarTextChangedEventHandler FilterBarTextChanged;

        /// <summary>
        /// Raises the <see cref="GridFilterBar.FilterBarTextChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFilterBarTextChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnFilterBarTextChanged(GridFilterBarTextChangedEventArgs e)
        {
            if (FilterBarTextChanged != null)
            {
                FilterBarTextChanged(this, e);
            }
        }

        /// <summary>
        /// Lets you control whether or not a column will display a drop-down in the added
        /// filter row.
        /// </summary>
        /// <remarks>To indicate that a column should not contain a filter cell, set
        /// the Cancel member of the events args to True.</remarks>
        public event GridFilterBarCreatingColumnHeaderEventHandler CreatingColumnHeader;

        /// <summary>
        /// Raises the <see cref="GridFilterBar.CreatingColumnHeader"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFilterBarCreatingColumnHeaderEventArgs"/> that contains the event data.</param>
        protected virtual void OnCreatingColumnHeader(GridFilterBarCreatingColumnHeaderEventArgs e)
        {
            if (CreatingColumnHeader != null)
            {
                CreatingColumnHeader(this, e);
            }
        }

        /// <summary>
        /// Lets you display a custom filter dialog of your choosing.
        /// </summary>
        /// <remarks>
        /// Your dialog should provide a properly formatted e.FilterCriteria that will be assigned to the 
        /// GridFilterBar.RowFilter property to determine the filtered contents of the grid. To indicate that
        /// your filter should be applied, set e.Result = DialogResult.OK and e.Handled = True. 
        /// To cancel the filter operation and leave the filter state of 
        /// the grid as is, set e.Result = DialogResult.Cancel and e.Handled = True.  
        /// To have the grid display its default dialog, set e.Handled = False.
        /// </remarks>
        /// <example>
        ///   <para>The following example displays some arbitrary dialog. The dialog returns information
        ///   necessary to set the proper values in the <see cref="GridFilterBarShowDialogEventArgs"/> e.
        ///   If you want the default dialog shown, set e.handled = False. If you do not want the default dialog
        ///   displayed, you set e.Handled = True. When e.Handled = True, you can indicate that the value 
        ///   of e.FilterCriteria should be used to filter the display by setting e.Result = DialogResult.OK. 
        ///   </para>
        ///   <code lang="C#">
        ///            //Show my own custom filter. 
        ///            private void GridFilterBarShowDialogEventHandler(object send, GridFilterBarShowDialogEventArgs e)
        ///            {
        ///                MyFilterDialog dlg = new MyFilterDialog();
        ///                DialogResult result = dlg.ShowDialog();
        /// <para/>
        ///                if(result == DialogResult.Ignore)
        ///                {
        ///                    //Show the default dialog.
        ///                    e.Handled = false;
        ///                }
        ///                else 
        ///                {
        ///                    //Otherwise, don't show default and set the result.
        ///                    e.Handled = true;
        ///                    e.Result = result; //cancel or OK-filter the grid with e.FilterCriteria
        ///                    e.FilterCriteria = dlg.textBox1.Text; // the filter string
        ///                }
        ///            }
        ///   </code>
        /// </example>
        public event GridFilterBarShowDialogEventHandler FilterBarShowDialog;

        /// <summary>
        /// Raises the  <see cref="FilterBarShowDialog"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFilterBarShowDialogEventArgs" /> that contains the event data.</param>
        protected virtual void OnFilterBarShowDialog(GridFilterBarShowDialogEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo(e);
            if (FilterBarShowDialog != null)
            {
                FilterBarShowDialog(this, e);
            }
        }

        /// <summary>
        /// Raised immediately after a filter has be completed.
        /// </summary>
        /// <remarks>Use this event if you need to know when your user has completed a filter action.</remarks>
        public event GridFilterBarFilterCompletedEventHandler FilterBarFilterCompleted;

        /// <summary>
        /// Raises the <see cref="GridFilterBar.FilterBarTextChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridFilterBarTextChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnFilterBarFilterCompleted(GridFilterBarTextChangedEventArgs e)
        {
            if (FilterBarFilterCompleted != null)
            {
                FilterBarFilterCompleted(this, e);
            }
        }

        #endregion

        #region Fields
        private GridDataBoundGrid _grid;
        private int _row;
        private DataTable _dataTable;
        private GridStyleInfo _style;
        private bool _parenthesesAroundColumnFilters = true;
        private string _rowFilter;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected string originalFilterOnTable = string.Empty;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected string originalSortOnTable = string.Empty;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected DataView originalDataView = null;
        #endregion

        #region Protected Field Accessors
        /// <summary>
        /// Accessor for the wired grid.  
        /// </summary>
        /// <returns>Returns the wired <see cref="GridDataBoundGrid"/> object.</returns>
        protected virtual GridDataBoundGrid GetGrid()
        {
            return _grid;
        }

        /// <summary>
        /// Specifies the wired grid.
        /// </summary>
        /// <param name="g">The <see cref="GridDataBoundGrid"/> object to be wired.</param>
        protected virtual void SetGrid(GridDataBoundGrid g)
        {
            _grid = g;
        }

        /// <summary>
        /// The row index for the FilterBar row.  
        /// </summary>
        /// <returns>Returns the row index where the Filterbar is displayed.</returns>
        protected virtual int GetFilterRow()
        {
            return _row;
        }

        /// <summary>
        /// Sets the row index where the FilterBar is displayed.
        /// </summary>
        /// <param name="r">The row index where the FilterBar is displayed.</param>
        protected virtual void SetFilterRow(int r)
        {
            _row = r;
        }

        /// <summary>
        /// The DataTable associated with the displayed data in the wired grid.
        /// </summary>
        /// <returns>The DataTable.</returns>
        protected virtual DataTable GetDataTable()
        {
            return _dataTable;
        }

        /// <summary>
        /// Sets the DataTable associated with the displayed data in the wired grid.
        /// </summary>
        /// <param name="dt">The DataTable.</param>
        protected virtual void SetDataTable(DataTable dt)
        {
            _dataTable = dt;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the RowFilter for use as the DataView.RowFilter property for the DataView
        /// associated with the current GridDataBoundGrid.DataSource.
        /// </summary>
        /// <remarks>Setting this property will apply the filter contained in the string
        /// to the default DataView of the GridDataBoundGrid. This member is set as you
        /// use the combo boxes on the FilterBar. But you can also set it explicitly as well.</remarks>
        public string RowFilter
        {
            get
            {
                string s = _rowFilter;
                if (this.originalFilterOnTable.Length > 0)
                {
                    if (_rowFilter.Length > 0)
                    {
                        s = string.Format("({0}) AND ({1})", originalFilterOnTable, _rowFilter);
                    }
                    else
                    {
                        s = originalFilterOnTable;
                    }
                }

                return s;
            }

            set    // Triggers a new filter on the dataview.
            {
                if (_rowFilter != value)
                {
                    _rowFilter = value;

                    OnFilterBarTextChanged(new GridFilterBarTextChangedEventArgs());

                    try
                    {
                        GridDataBoundGrid grid = GetGrid();
                        DataTable dataTable = GetDataTable();
                        int row = GetFilterRow();
                        if (grid != null)
                        {
                            int col = grid.CurrentCell.HasCurrentCell ? grid.CurrentCell.ColIndex : 1;
                            grid.BeginUpdate();
                            if (originalDataView != null)
                            {
                                originalDataView.RowFilter = this.RowFilter;
                            }
                            else
                            {
                                dataTable.DefaultView.RowFilter = _rowFilter;
                            }

                            grid.CurrentCell.MoveTo(row + 1, col, GridSetCurrentCellOptions.None);
                            if (GridUtil.IsEmpty(_rowFilter))
                            {
                                this.ResetFilterRow(grid);
                            }

                            grid.EndUpdate(true);
                            OnFilterBarFilterCompleted(new GridFilterBarTextChangedEventArgs());
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        MessageBoxAdv.Show(ex.Message.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the default style for the cells in the FilterBar row. 
        /// </summary>
        /// <remarks>You can use this property to control the basic appearance, such as BackColor
        /// or Font, of the cells on the FilterBar row.</remarks>
        public GridStyleInfo StyleInfo
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to enclosed each column filter in parentheses when they are ANDed to 
        /// create the <see cref="RowFilter"/> string. 
        /// </summary>
        /// <remarks>The default value is true. This property only has an effect if you have 
        /// multiple column filters set, and have also set a 
        /// compound filter using an OR clause in the Custom dialog. In this case, the parentheses
        /// are needed to ensure the proper evaluation of the logical expression. Versions earlier than
        /// 4.1.0.0, did not provide these added parentheses. If you need this earlier behavior, then set
        /// this property to false.
        /// </remarks>
        public bool ParenthesesAroundColumnFilters
        {
            get { return _parenthesesAroundColumnFilters; }
            set { _parenthesesAroundColumnFilters = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this FilterBar object is associated with a GridDataBoundGrid.
        /// </summary>
        public bool Wired
        {
            get { return GetGrid() != null; }
        }

        #endregion

        #region Binding Code

        /// <summary>
        /// Associates a GridDataBoundGrid with this FilterBar.
        /// </summary>
        /// <param name="grid">The GridDataBoundGrid.</param>
        public void WireGrid(GridDataBoundGrid grid)
        {
            if (grid != null)
            {
                GridStyleInfo style = new GridStyleInfo();
                style.ModifyStyle(grid.BaseStylesMap["Header"].StyleInfo, StyleModifyType.Copy);
                style.CellType = "ComboBox";
                style.ExclusiveChoiceList = true;
                style.BaseStyle = "Standard";
                style.Font.Bold = false;
                style.BackColor = grid.TableStyle.BackColor;
                style.Borders.Bottom = new GridBorder(GridBorderStyle.Dashed);
                WireGrid(grid, style);
                grid.isFilterBarWired = true;
            }
        }

        /// <summary>
        /// Associates a GridDataBoundGrid with this FilterBar.
        /// </summary>
        /// <param name="grid">The GridDataBoundGrid.</param>
        /// <param name="style">The GridStyleInfo object that sets the appearance of cells in the FilterBar row.</param>
        public virtual void WireGrid(GridDataBoundGrid grid, GridStyleInfo style)
        {
            if (GetGrid() != null)
            {
                UnwireGrid();
            }

            if (grid != null)
            {
                SetDataTable(grid.DataSource as DataTable);
                if (GetDataTable() == null)
                {
                    DataSet ds = grid.DataSource as DataSet;
                    if (ds != null)
                    {
                        ////to check later: use this code for all cases???
                        CurrencyManager cm = grid.BindingContext[grid.DataSource, grid.DataMember] as CurrencyManager;
                        DataView dv = (cm != null) ? cm.List as DataView : null;
                        if (dv != null)
                        {
                            originalFilterOnTable = dv.RowFilter;
                            originalSortOnTable = dv.Sort;
                            originalDataView = dv;
                            SetDataTable(dv.Table);
                        }
                    }
                }

                if (GetDataTable() == null)
                {
                    DataView dv = grid.DataSource as DataView;
                    if (dv != null)
                    {
                        originalFilterOnTable = dv.RowFilter;
                        originalSortOnTable = dv.Sort;
                        originalDataView = dv;
                        SetDataTable(dv.Table);
                    }
                }

                DataTable dt = GetDataTable();

                if (dt != null)
                {
                    StyleInfo = style;

                    ////Add a fixed row at the top.
                    grid.Model.Data.RowCount++;
                    SetFilterRow(grid.Model.Rows.HeaderCount + 1);
                    int row = GetFilterRow();
                    grid.Model.Rows.HeaderCount = row;
                    grid.Model.Rows.FrozenCount = grid.Model.Rows.FrozenCount + 1;

                    grid.Model.ChangeCells(GridRangeInfo.Cells(row, 1, row, grid.Model.ColCount), style, StyleModifyType.Override);

                    DataView dv = new DataView(dt, originalFilterOnTable, originalSortOnTable, DataViewRowState.CurrentRows);

                    GridBoundColumnsCollection gbcc;
                    gbcc = (grid.GridBoundColumns.Count == 0) ? grid.Binder.InternalColumns : grid.GridBoundColumns;

                    for (int gbcIndex = 0; gbcIndex < gbcc.Count; ++gbcIndex)
                    {
                        string colName = gbcc[gbcIndex].MappingName;
                        int col = grid.Binder.NameToColIndex(colName);

                        GridFilterBarCreatingColumnHeaderEventArgs e = new GridFilterBarCreatingColumnHeaderEventArgs(colName);
                        OnCreatingColumnHeader(e);
                        if (e.Cancel)
                        {
                            grid[row, col].ShowButtons = GridShowButtons.Hide;
                            grid[row, col].DropDownStyle = GridDropDownStyle.Editable;
                            grid[row, col].ReadOnly = true;
                            continue;
                        }
                        ////Allow event handler to change the colname used to define the choices.
                        colName = e.ColName;

                        dv.Sort = colName;
                        grid[row, col].DataSource = CreateUniqueEntries(dv, colName);
                        grid[row, col].DisplayMember = colName;
                        grid[row, col].ValueMember = colName;
                    }

                    ////Hook up handers.
                    grid.CurrentCellAcceptedChanges += new CancelEventHandler(GridCurrentCellAcceptedChanges);
                    grid.CurrentCellShowingDropDown += new GridCurrentCellShowingDropDownEventHandler(GridCurrentCellShowingDropDown);
                    grid.CurrentCellCloseDropDown += new PopupClosedEventHandler(GridCurrentCellCloseDropDown);
                    grid.KeyDown += new KeyEventHandler(GridKeyDown);
                    SetGrid(grid);
                }
            }
        }

        /// <summary>
        /// Disassociates a grid with this FilterBar.
        /// </summary>
        public virtual void UnwireGrid()
        {
            GridDataBoundGrid grid = GetGrid();
            if (grid != null)
            {
                grid.BeginUpdate();
                int row = GetFilterRow();
                grid.Model.Rows.RemoveRange(row, row);
                grid.Model.Rows.HeaderCount -= 1;
                grid.Model.Rows.FrozenCount -= 1;

                grid.CurrentCellAcceptedChanges -= new CancelEventHandler(GridCurrentCellAcceptedChanges);
                grid.CurrentCellShowingDropDown -= new GridCurrentCellShowingDropDownEventHandler(GridCurrentCellShowingDropDown);
                grid.CurrentCellCloseDropDown -= new PopupClosedEventHandler(GridCurrentCellCloseDropDown);
                grid.KeyDown -= new KeyEventHandler(GridKeyDown);

                if (originalDataView != null)
                {
                    originalDataView.RowFilter = originalFilterOnTable;
                    originalFilterOnTable = string.Empty;
                    originalSortOnTable = string.Empty;
                    originalDataView = null;
                }

                grid.EndUpdate();
                grid.Refresh();
                SetGrid(null);
            }
        }
        #endregion

        #region Event Handlers

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void GridKeyDown(object sender, KeyEventArgs e)
        {
            GridDataBoundGrid grid = sender as GridDataBoundGrid;
            if (grid != null && grid.CurrentCell.HasCurrentCellAt(GetFilterRow()))
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        bool controlKeyDown = (e.Modifiers & Keys.Control) != Keys.None;
                        bool shiftKeyDown = (e.Modifiers & Keys.Shift) != Keys.None;
                        bool menuKeyDown = (e.Modifiers & Keys.Menu) != Keys.None;
                        if (!menuKeyDown && !controlKeyDown && !shiftKeyDown)
                        {
                            // close dropdown with PopupCloseType.Done 
                            grid.CurrentCell.ConfirmChanges();
                            // do not move current cell
                            e.Handled = true;
                        }

                        break;
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void GridCurrentCellShowingDropDown(object sender, GridCurrentCellShowingDropDownEventArgs e)
        {
            GridDataBoundGrid grid = sender as GridDataBoundGrid;
            if (grid != null)
            {
                GridCurrentCell cc = grid.CurrentCell;
                if (cc.RowIndex == this.GetFilterRow())
                {
                    if (cc.Renderer.StyleInfo.ShowButtons == GridShowButtons.Hide)
                        return;
                    GridBoundColumnsCollection gbcc;
                    gbcc = (grid.GridBoundColumns.Count == 0) ? grid.Binder.InternalColumns : grid.GridBoundColumns;
                    int colIndex = grid.binder.ColIndexToField(cc.ColIndex);
                    string colName = gbcc[colIndex].MappingName;
                    DataView dv;
                    if (grid.DataSource is System.Data.DataView)
                    {
                        dv = ((System.Data.DataView)grid.DataSource);

                    }
                    else if (grid.DataSource is DataSet && !string.IsNullOrEmpty(grid.DataMember))
                    {
                        CurrencyManager cm = grid.BindingContext[grid.DataSource, grid.DataMember] as CurrencyManager;
                        dv = (cm != null) ? cm.List as DataView : null;                                          
                    }
                    else
                    {
                        dv = (grid.DataSource as DataTable).DefaultView; 
                    }
                    string sortedColumn = dv.Sort;
                    cc.Lock();
                    grid.BeginUpdate();
                    dv.Sort = colName;
                    DataTable dt ;
                    if (string.IsNullOrEmpty(cc.Renderer.ControlText))
                        dt = CreateUniqueEntries(dv, colName);
                    else
                        dt = grid[cc.RowIndex, cc.ColIndex].DataSource as DataTable;
                    dv.Sort = sortedColumn;
                    grid.EndUpdate();
                    cc.Unlock();
                    if (grid[cc.RowIndex, cc.ColIndex].DataSource != null)
                        grid[cc.RowIndex, cc.ColIndex].DataSource = null;
                    grid[cc.RowIndex, cc.ColIndex].DataSource = dt;
                    if (dt != null)
                    {
                        int maxItems = Math.Max(12, grid.ViewLayout.LastVisibleRow - grid.TopRowIndex);
                        int nItems = Math.Min(maxItems, dt.Rows.Count);

                        GridComboBoxCellRenderer cr = grid.CellRenderers[grid[cc.RowIndex, cc.ColIndex].CellType] as GridComboBoxCellRenderer;
                        if (cr != null)
                        {
                            ((GridComboBoxListBoxPart)cr.ListBoxPart).DropDownRows = nItems;
                        }
                    }
                }
                else
                {
                    GridComboBoxCellRenderer cr = grid.CellRenderers[grid[cc.RowIndex, cc.ColIndex].CellType] as GridComboBoxCellRenderer;
                    if (cr != null)
                    {
                        ((GridComboBoxListBoxPart)cr.ListBoxPart).DropDownRows = 12;
                    }
                }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void GridCurrentCellAcceptedChanges(object sender, CancelEventArgs e)
        {
            GridDataBoundGrid grid = sender as GridDataBoundGrid;
            if (grid != null)
            {
                GridCurrentCell cc = grid.CurrentCell;
                if (cc.RowIndex == this.GetFilterRow())
                {
                    cc.Lock();
                    this.RowFilter = GetFilterFromRow(grid);
                    cc.Unlock();
                }
            }
        }

        ////used to reset a cancel out of custom dialog
        private string saveOriginalText;

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected void GridCurrentCellCloseDropDown(object sender, Syncfusion.Windows.Forms.PopupClosedEventArgs e)
        {
            GridDataBoundGrid grid = sender as GridDataBoundGrid;
            if (grid != null && e.PopupCloseType == PopupCloseType.Done)
            {
                GridCurrentCell cc = grid.CurrentCell;
                if (cc.RowIndex == this.GetFilterRow())
                {
                    if (grid.ExcelLikeAlignment && !grid[cc.RowIndex, cc.ColIndex].HasHorizontalAlignment)
                    {
                        if (grid.Binder.InternalColumns[cc.ColIndex - 1].StyleInfo.CellValueType != typeof(string))
                            grid[cc.RowIndex, cc.ColIndex].HorizontalAlignment = GridHorizontalAlignment.Right;
                        else
                            grid[cc.RowIndex, cc.ColIndex].HorizontalAlignment = GridHorizontalAlignment.Left;
                    }

                    if (cc.Renderer.ControlText == GridFilterBarStrings[_custom_])
                    {
                        this.saveOriginalText = grid[cc.RowIndex, cc.ColIndex].Text;
                        cc.IsModified = true;
                    }

                    cc.ConfirmChanges();
                }
            }
        }

        #endregion

        #region Utility Methods
        /// <summary>
        /// Creates a DataTable of unique choices for a FilterBar entry.
        /// </summary>
        /// <param name="dv">The DataView that is being displayed in the grid.</param>
        /// <param name="colName">The column name of the column whose FilterBar choices are being constructed.</param>
        /// <returns>A DataTable.</returns>
        /// <remarks>This method is called to generate the list of entries for the default drop-down cell. 
        /// The method inserts (none) and (custom) as the first two entries in the list. The other entries
        /// are unique occurrences of entries from the specified column. The (none) and (custom) strings can be 
        /// changed through <see cref="GridFilterBarStrings"/>.
        /// </remarks>
        protected virtual DataTable CreateUniqueEntries(DataView dv, string colName)
        {
            DataTable dt = new DataTable(colName);
            dt.Columns.Add(new DataColumn(colName));

            DataRow dr;
            if (GridFilterBarStrings[_none_].Length > 0)
            {
                dr = dt.NewRow();
                dr[0] = GridFilterBarStrings[_none_];
                dt.Rows.Add(dr);
            }

            if (GridFilterBarStrings[_custom_].Length > 0)
            {
                dr = dt.NewRow();
                dr[0] = GridFilterBarStrings[_custom_];
                dt.Rows.Add(dr);
            }

            string s = string.Empty;
            for (int i = 0; i < dv.Count; ++i)
            {
                if (s != dv[i].Row[colName].ToString())
                {
                    s = dv[i].Row[colName].ToString();
                    dr = dt.NewRow();
                    dr[0] = s;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        /// <summary>
        /// Returns the filter string based on current cell values in the FilterBar.
        /// </summary>
        /// <param name="grid">The grid holding the FilterBar.</param>
        /// <returns>Returns a string that is appropriate to use as a DataView.RowFilter string.</returns>
        protected virtual string GetFilterFromRow(GridDataBoundGrid grid)
        {
            GridCurrentCell cc = grid.CurrentCell;
            int col = cc.ColIndex;
            int row = this.GetFilterRow();
            DataTable dt = this.GetDataTable();
            string s = cc.Renderer.ControlText;
            string colName;
            string rowFilter = string.Empty;

            if (cc.IsActive)
            {
                cc.Deactivate(false);
            }

            if (GridUtil.IsEmpty(s) || s == GridFilterBarStrings[_none_])
            {
                grid[row, col].Text = string.Empty;
                grid[row, col].Tag = string.Empty;
            }
            else if (s == GridFilterBarStrings[_custom_])
            {
                GridFilterBarShowDialogEventArgs e = new GridFilterBarShowDialogEventArgs(string.Empty, DialogResult.OK);

                ////Needed to allow popup.
                grid.CurrentCell.ConfirmChanges();

                bool bShowDialog = true;
                while (bShowDialog)
                {
                    OnFilterBarShowDialog(e);

                    if (!e.Handled)
                    {
                        GridFilterBarCustomDlg dlg = new GridFilterBarCustomDlg();
                        string gridColName = grid.Binder.InternalColumns[grid.Binder.ColIndexToField(col)].MappingName;
                        dlg.colLabel.Text = grid.Binder.InternalColumns[gridColName].HeaderText;
                        dlg.SetStrings(GridFilterBarStrings);
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            grid[row, col].Text = GridFilterBarStrings[_custom_];

                            ////Uncomment the following 2 lines to make the custom filter the complete filter
                            ////instead of and'ing with the existing filter:
                            ////ResetFilterRow(grid);
                            ////return dlg.FilterString;
                            grid[row, col].Tag = dlg.FilterString;
                        }
                        else
                        {
                            cc.Renderer.ControlText = saveOriginalText;
                        }
                    }
                    else if (e.Result == DialogResult.OK)
                    {
                        ////Uncomment the following 2 lines to make the custom filter the complete filter
                        ////instead of and'ing with the existing filter:
                        ////ResetFilterRow(grid);
                        ////return e.FilterCriteria;

                        if (!this.IsFilterStringValid(e.FilterCriteria))
                        {
                            continue;
                        }

                        grid[row, col].Text = GridFilterBarStrings[_custom_];
                        grid[row, col].Tag = e.FilterCriteria;
                    }

                    bShowDialog = false;
                }
            }
            else
            {
                string gridColName = grid.Binder.InternalColumns[grid.Binder.ColIndexToField(col)].MappingName;
                colName = dt.Columns[gridColName].ColumnName;
                grid[row, col].Text = s;
                if (s.IndexOf("'") != -1)
                {
                    s = s.Replace("'", "''");
                }

                grid[row, col].Tag = string.Format("[{0}] = '{1}'", colName, s);
            }

            ////Otherwise form the filter string from the filter row values.
            for (int j = 1; j <= grid.Model.ColCount; ++j)
            {
                s = grid[row, j].Tag as string;
                if (s != null && s.Length > 0)
                {
                    if (rowFilter.Length > 0)
                    {
                        rowFilter += GridFilterBarStrings[_and_];
                    }

                    if (ParenthesesAroundColumnFilters)
                    {
                        rowFilter += string.Format("({0})", s);
                    }
                    else
                    {
                        rowFilter += (string)s;
                    }
                }
            }

            return rowFilter;
        }

        private bool IsFilterStringValid(string filterString)
        {
            string filter = string.Empty;
            try
            {
                GetGrid().BeginUpdate();
                filter = GetDataTable().DefaultView.RowFilter;
                GetDataTable().DefaultView.RowFilter = filterString;
            }
            catch (Exception ex)
            {
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                GetDataTable().DefaultView.RowFilter = filter;
                GetGrid().CancelUpdate();
                MessageBoxAdv.Show(GridFilterBarStrings[improper_custom_filter] + ex.Message);
                return false;
            }

            GetDataTable().DefaultView.RowFilter = filter;
            GetGrid().CancelUpdate();
            return true;
        }

        /// <summary>
        /// Blanks the filter string on every column.
        /// </summary>
        /// <param name="grid">The grid holding the FilterBar.</param>
        public void ResetFilterRow(GridDataBoundGrid grid)
        {
            int row = this.GetFilterRow();
            for (int j = 1; j <= grid.Model.ColCount; ++j)
            {
                grid[row, j].Text = string.Empty;
                grid[row, j].Tag = string.Empty;
            }
        }
        #endregion

        #region Strings
        
        /// <summary>
        /// String array that holds the strings used in GridFilterBar.
        /// </summary>
        /// <remarks>If you want to change these strings,
        /// you can set your new strings into the appropriate position in the GridFilterBarStrings 
        /// array. Here is the code that shows the default settings. You should assign your new
        /// strings to the corresponding positions. 
        /// <para/>
        /// The first two entries are the special items in the droplists for filter combo box cells. If you 
        /// set one or both of these strings to the empty string, that option will not appear in
        /// the droplist.
        /// </remarks>
        /// <example>Here is the code that shows position of each string in GridFilterBarStrings.
        /// <code lang="C#">
        ///        public string[] GridFilterBarStrings = new string[]
        ///        {
        ///            "(none)",                                        //0
        ///            "(custom)",                                        //1
        ///            "equals",                                        //2
        ///            "does not equal",                                //3
        ///            "is greater than",                                //4
        ///            "is greater than or equal to",                    //5
        ///            "is less than",                                    //6
        ///            "is less than or equal to",                        //7
        ///            "begins with",                                    //8
        ///            "does not begin with",                            //9
        ///            "ends with",                                    //10
        ///            "does not end with",                            //11
        ///            "contains",                                        //12
        ///            "does not contain",                                //13
        ///            "Use * to represent any series of characters",    //14
        ///            "Show rows where:",                                //15
        ///            "Improper custom filter: ",                        //16
        ///            "I and "                                        //17
        ///        };
        /// </code>
        /// </example>
        public string[] GridFilterBarStrings = new string[]
            {
                "(none)",                        ////0
                "(custom)",                        ////1
                "equals",                        ////2
                "does not equal",                ////3
                "is greater than",                ////4
                "is greater than or equal to",    ////5
                "is less than",                    ////6
                "is less than or equal to",        ////7
                "begins with",                    ////8
                "does not begin with",            ////9
                "ends with",                    ////10
                "does not end with",            ////11
                "contains",                        ////12
                "does not contain",                ////13
                "Use * to represent any series of characters",    ////14
                "Show rows where:",                ////15
                "Improper custom filter: ",        ////16
                " and ",       ////17
                "or",////18
                "OK",////19
                "Cancel",////20
                "Custom Row Filter", ////21
                "and" ////22
            };

        internal int _none_ = 0;
        internal int _custom_ = 1;
        internal int _equals_ = 2;
        internal int does_not_equal = 3;
        internal int is_greater_than = 4;
        internal int is_greater_than_or_equal_to = 5;
        internal int is_less_than = 6;
        internal int is_less_than_or_equal_to = 7;
        internal int begins_with = 8;
        internal int does_not_begin_with = 9;
        internal int ends_with = 10;
        internal int does_not_end_with = 11;
        internal int _contains_ = 12;
        internal int does_not_contain = 13;
        internal int Use_to_represent_any_series_of_characters = 14;
        internal int Show_rows_where = 15;
        internal int improper_custom_filter = 16;
        internal int _and_ = 17;
        internal int custom_Row_Filter_Or_ = 18;
        internal int _OK_ = 19;
        internal int _Cancel_ = 20;
        internal int custom_Row_Filter = 21;
        internal int custom_Row_Filter_And = 22;
        #endregion
    }
}
