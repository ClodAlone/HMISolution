//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableModel.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Text;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
using Syncfusion.Web.UI.WebControls.Tools;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// A grid model that displays rows with <see cref="Syncfusion.Grouping.Table.DisplayElements"/> of a <see cref="GridTable"/> and
    /// allows grouping, filtering, editing, deleting, and adding of records.
    /// </summary>
    public class GridTableModel : GridModel, ITableEventsTarget
    {
        GridTable table;
        ////bool ownedTable = true;
        internal bool inInitLayout = false;
        bool isExcelWired = false, wasSizeApplied = false;
        bool hierarchicalGroupDropArea = false;
#if ASPNET
#else
        internal GridGroupDropAreaModel groupDropAreaModel;
#endif
        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.
        /// </returns>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            return GetType().Name + " { " + (Table != null ? Table.ToString() : string.Empty) + isdisposed + " }";
        }

        /// <summary>
        /// Holds information about position of current cell, current cell renderer, and last active grid control.
        /// </summary>
        public override GridCurrentCellInfo CurrentCellInfo
        {
            get
            {
                GridControlBase activeGrid = this.ActiveGridView;
                if (activeGrid == null)
                {
                    return null;
                }

                GridCurrentCell gcc = activeGrid.CurrentCell;
                if (gcc == null || !gcc.HasCurrentCell)
                {
                    return null;
                }

                return new GridCurrentCellInfo(activeGrid, gcc.Renderer, gcc.RowIndex, gcc.ColIndex);
            }
            
            set
            {
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public override GridRangeInfoList SelectedRanges
        {
            get
            {
                if (table != null && table.FilteredChildTable != null)
                {
                    return ((GridChildTable)table.FilteredChildTable).SelectedRanges;
                }

                return base.SelectedRanges;
            }
        }

#if ASPNET
#else
        /// <summary>
        /// Virtual method to create the <see cref="GridGroupDropArea"/>.
        /// </summary>
        /// <returns>The new <see cref="GroupDropAreaModel"/>.</returns>
        public virtual GridGroupDropAreaModel CreateGroupDropAreaModel()
        {
            return new GridGroupDropAreaModel(this);
        }
#endif

        /// <summary>
        /// Initializes the table model.
        /// </summary>
        public GridTableModel()
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }

            this.VolatileData = new GridTableModelVolatileData(this);
#if ASPNET
            this.BaseStylesMap.CellTypes.Remove("Header");
            this.BaseStylesMap.CellTypes.Remove("PushButton");
            this.BaseStylesMap.CellTypes.Remove("FormulaCell");
            this.BaseStylesMap.CellTypes.Remove("RichText");
            this.BaseStylesMap.CellTypes.Remove("Control");
            this.BaseStylesMap.CellTypes.Remove("OriginalTextBox");
#else
            groupDropAreaModel = CreateGroupDropAreaModel();
#endif
            //// GridModel model = this;
            //// QueryCellModel += new GridQueryCellModelEventHandler(ModelQueryCellModel);

            CommandStack.Enabled = false;
            Rows.DefaultSize = 17;
            Cols.DefaultSize = 65;
            RowHeights[0] = 28;
            ColWidths[0] = 35;
            Options.ExcelLikeCurrentCell = false;
            Options.ExcelLikeSelectionFrame = false;
            Options.AllowDragSelectedCols = true;
            Options.AllowDragSelectedRows = false;

            Options.AllowSelection = GridSelectionFlags.Any; ////Row|GridSelectionFlags.Table|GridSelectionFlags.Multiple|GridSelectionFlags.Row|GridSelectionFlags.Keyboard|GridSelectionFlags.Shift|GridSelectionFlags.AlphaBlend;
#if ASPNET
            Options.ListBoxSelectionMode = (System.Windows.Forms.SelectionMode)Enum.Parse(typeof(System.Windows.Forms.SelectionMode), SelectionMode.MultiExtended.ToString());
#else
            Options.ListBoxSelectionMode = SelectionMode.MultiExtended;
#endif

            Options.FloatCellsMode = GridFloatCellsMode.None;
            Options.NumberedRowHeaders = false;
            Options.NumberedColHeaders = false;
            Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.GrayWhenLostFocus;
            Options.DataObjectConsumerOptions = GridDataObjectConsumerOptions.None;
            Options.DragDropDropTargetFlags &= ~GridDragDropFlags.Styles;
            Options.UseRightToLeftCompatibleTextBox = true;
            Properties.MarkRowHeader = false;
            Properties.MarkColHeader = false;
            Properties.BackgroundColor = SystemColors.Window;

            Options.SmoothControlResize = false;

            Options.VerticalScrollTips = true;
            Options.HorizontalScrollTips = true;
            Options.VerticalThumbTrack = true;
            Options.HorizontalThumbTrack = true;
            ////Options.RefreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshRow;

            Options.ResizeColsBehavior = GridResizeCellsBehavior.AllowDragOutside | GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineBounds | GridResizeCellsBehavior.OutlineHeaders | GridResizeCellsBehavior.InsideGrid;

            Options.ResizeRowsBehavior = GridResizeCellsBehavior.AllowDragOutside | GridResizeCellsBehavior.ResizeSingle | GridResizeCellsBehavior.OutlineBounds | GridResizeCellsBehavior.OutlineHeaders;
            Options.RefreshCurrentCellBehavior = GridRefreshCurrentCellBehavior.RefreshRow;

            CellModels.Add("RowHeaderCell", new GridTableRowHeaderCellModel(this));
            CellModels.Add("IndentCell", new GridTableIndentCellModel(this));
            CellModels.Add("ColumnHeaderCell", new GridTableColumnHeaderCellModel(this));
            CellModels.Add("StackedHeaderCell", new GridTableColumnHeaderCellModel(this));
            CellModels.Add("NestedTable Row Header", new GridStaticCellModel(this));
            ////CellModels.Add("MultiSelect", new GridDropDownMultiSelectCellModel(this));
            Options.SelectCellsMouseButtonsMask = System.Windows.Forms.MouseButtons.Left | System.Windows.Forms.MouseButtons.Right;
            Options.EnterKeyBehavior = GridDirectionType.None;
            CommandStack.Enabled = false;
            Options.FloatCellsMode = GridFloatCellsMode.None;
        }


        [Category("Grouping Control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        internal bool HierarchicalGroupDropArea
        {
            get
            {
                return this.hierarchicalGroupDropArea;
            }
            set
            {
                this.hierarchicalGroupDropArea = value;
            }
        }
        /// <override/>
        protected override void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            e.ClipboardFlags &= ~(GridDragDropFlags.Styles | GridDragDropFlags.RowHeader);
            base.OnClipboardCanCopy(e);
        }

        /// <summary>
        /// Gets / sets the <see cref="GridTableCellStyleInfo"/> of a cell.
        /// </summary>
        public new GridTableCellStyleInfo this[int rowIndex, int colIndex]
        {
            get
            {
                return (GridTableCellStyleInfo)base[rowIndex, colIndex];
            }
           
            set
            {
                base[rowIndex, colIndex] = value;
            }
        }

        /// <override/>
        protected override void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (e.Range.IsCells || e.Range.IsRows)
            {
                GridTableCellStyleInfo style = (GridTableCellStyleInfo)this[e.Range.Top, e.Range.Left];
                if (style.TableCellIdentity != null && style.TableCellIdentity.TableCellType == GridTableCellType.TopLeftHeaderCell)
                {
                    e.Range = GridRangeInfo.Rows(Table.DisplayElements.IndexOf(Table.TopLevelGroup.GetFirstRecord()), RowCount);
                }
            }

            base.OnSelectionChanging(e);
        }

        /// <summary>
        /// Gets / sets the <see cref="ChildTable"/> that should be displayed
        /// with this model.
        /// </summary>
        public ChildTable FilteredChildTable
        {
            get
            {
                return Table.FilteredChildTable;
            }

            set
            {
                Table.FilteredChildTable = value;
            }
        }

        /// <override/>
        protected override void OnCreatedCutPaste()
        {
            CutPaste.ClipboardFlags &= ~GridDragDropFlags.Styles;
        }

        /// <summary>
        /// Use Table.DisplayElements[rowIndex]
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns>Element at the specified index.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public Element GetDisplayElementAt(int rowIndex)
        {
            return Table.DisplayElements[rowIndex];
        }

        /// <summary>
        /// Use Table.DisplayElements.IndexOf
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>Index of the given element.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public int GetDisplayElementIndexOf(Element el)
        {
            return Table.DisplayElements.IndexOf(el);
        }

        /// <summary>
        /// Use Table.DisplayElements.Count
        /// </summary>
        /// <returns>Number of display elements.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public int GetDisplayElementCount()
        {
            return Table.DisplayElements.Count;
        }

#if ASPNET
#else
        /// <summary>
        /// Returns the <see cref="GridGroupingControl"/> that hosts this control.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupingControl GroupingControl
        {
            get
            {
                Control c = this.ActiveGridView;
                while (c != null)
                {
                    if (c is GridGroupingControl)
                    {
                        return (GridGroupingControl)c;
                    }

                    c = c.Parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="GridGroupDropAreaModel"/> of the <see cref="GridGroupingControl"/>.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupDropAreaModel GroupDropAreaModel
        {
            get
            {
                return groupDropAreaModel;
            }
        }
#endif
        private GridGroupDropAreaAlignment groupDropAlign;
        /// <summary>
        /// Gets / sets whether the <see cref="GridGroupDropAreaAlignment"/> should be top,right,left or bottom.
        /// </summary>
        public GridGroupDropAreaAlignment GroupDropAlign
        {
            get
            {
                return this.Table.Engine.ParentControl.GroupDropAreaAlignment;
            }
            set
            {
                groupDropAlign = this.Table.Engine.ParentControl.GroupDropAreaAlignment;
            }
        }
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !this.IsDisposed)
            {
                if (Engine.VerboseEnsureObjectLifeTime)
                {
                    TraceUtil.TraceCurrentMethodInfo(table);
                }

                UnwireTable();
                ////if (false && table != null && ownedTable)
                ////{
                ////  table.Dispose();
                ////}
            }

            base.Dispose(disposing);
        }

        #region Columns

        /// <summary>
        /// Returns the number of indent columns.
        /// </summary>
        /// <returns>If records don't have nested tables, the method returns GroupedColumns.Count+1; otherwise GroupedColumns.Count+2.</returns>
        public int GetColumnIndentCount()
        {
            return Table.TableDescriptor.GetColumnIndentCount();
        }

        /// <internalonly/>
        /// <summary>Converts a column index to a zero-based number adjusted for column headers collection.</summary>
        /// <param name="colIndex">Column index.</param>
        /// <returns>Value indicating the field position.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public int ColIndexToField(int colIndex)
        {
            return Table.TableDescriptor.ColIndexToField(colIndex);
        }

        /// <overload>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header cell.
        /// </overload>
        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified row and column index if it is a header cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(int rowIndex, int colIndex)
        {
            return Table.GetHeaderColumnDescriptorAt(rowIndex, colIndex);
        }

        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified column index in the top-most row.
        /// </summary>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(int colIndex)
        {
            return Table.GetHeaderColumnDescriptorAt(colIndex);
        }

        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified row and column index if it is a header cell.
        /// </summary>
        /// <param name="cell">The row and column index.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(GridRangeInfo cell)
        {
            return Table.GetHeaderColumnDescriptorAt(cell);
        }

        /// <exclude/>
        /// <summary>
        /// Determines the <see cref="GridStackedHeaderSpan"/> displayed at the specified row and column index if it is a header cell.
        /// </summary>
        /// <param name="cell">The row and column index.</param>
        /// <returns>The <see cref="GridStackedHeaderSpan"/> displayed at the specified cell if it is a header cell; NULL otherwise.</returns>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(GridRangeInfo cell)
        {
            return Table.GetStackedHeaderSpanAt(cell);
        }

        /// <overload>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified header or record field cell.
        /// </overload>
        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified header or record field cell.
        /// </summary>
        /// <param name="cell">The row and column index.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header or record field cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetColumnDescriptorAt(GridRangeInfo cell)
        {
            return Table.GetColumnDescriptorAt(cell);
        }

        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified header or record field cell.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified cell if it is a header or record field cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetColumnDescriptorAt(int rowIndex, int colIndex)
        {
            return Table.GetColumnDescriptorAt(rowIndex, colIndex);
        }

        /// <summary>
        /// Converts a zero-based number to a column index in a grid adjusted for column headers (adding <see cref="GetColumnIndentCount"/>).
        /// </summary>
        /// <param name="fieldNum">A zero-based field number.</param>
        /// <returns>The column index in the grid.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public int FieldToColIndex(int fieldNum)
        {
            return Table.TableDescriptor.FieldToColIndex(fieldNum);
        }

        /// <summary>
        /// Returns the zero-based index for a column. The resulting
        /// number can be used as an index to look up a <see cref="GridColumnDescriptor"/> in the <see cref="GridTableDescriptor.Columns"/>
        /// collection.
        /// </summary>
        /// <param name="name">The name of the column to be matched.</param>
        /// <returns>A zero-based field number in the <see cref="GridTableDescriptor.Columns"/> collection; -1 if not found.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public int NameToField(string name)
        {
            return Table.TableDescriptor.NameToField(name);
        }

        /// <summary>
        /// Returns the column index for a column that matches a given name.
        /// Returns the zero-based field number for a column that matches a given name. The resulting
        /// field number can be used as an index for the <see cref="GridModel.this"/>.
        /// </summary>
        /// <param name="name">The name of the field to be matched.</param>
        /// <returns>The column index in the grid; -1 if not found.</returns>
        /// <remarks>
        /// This function only searches the columns in the root level. If you have several relations
        /// displayed in the grid, the nested relations will not be searched by this function.
        /// </remarks>
        [Syncfusion.Documentation.DocumentationExclude]
        public override int NameToColIndex(string name)
        {
            if (!HasTable)
            {
                return -1;
            }

            int fieldNum = NameToField(name);
            if (fieldNum != -1)
            {
                return FieldToColIndex(fieldNum);
            }

            return -1;
        }

        /// <summary>
        /// Returns the column index where the caption bar of a group should be drawn.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <returns>If the element is not a CaptionRow, one is returned; otherwise the column index
        /// where the caption bar should be drawn.</returns>
        public int GetCaptionColIndex(Element el)
        {
            if (el is ChildTable || (CaptionSection.IsCaption(el) && el.ParentElement is ChildTable))
            {
                return 1;
            }
            else
            {
                return el.GroupLevel + 1 + (Table.TableDescriptor.Relations.NestedCount > 0 ? 1 : 0);
            }
        }

        /// <summary>
        /// Returns the row index for a row that matches a given name.
        /// </summary>
        /// <param name="name">The name of the row to be matched.</param>
        /// <returns>The row index in the grid; -1 if not found.</returns>
        [Syncfusion.Documentation.DocumentationExclude]
        public override int NameToRowIndex(string name)
        {
            int rowIndex = -1;
            try
            {
                rowIndex = int.Parse(name);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
                // not a valid number
            }

            return rowIndex;
        }

        #endregion

        /// <summary>
        /// Determines if the <see cref="Table"/> has been set.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasTable
        {
            get
            {
                return table != null;
            }
        }

        bool inSetTable = false;
        bool inTableChanged = false;

        /// <summary>
        /// True when <see cref="Table"/> property setter is called; False after it returned.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InSetTable
        {
            get
            {
                return inSetTable;
            }
        }

        /// <summary>
        /// True when <see cref="TableChanged"/> event is called; False after it is returned.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InTableChanged
        {
            get
            {
                return inTableChanged;
            }
        }

        /// <summary>
        /// Gets / sets the Legacy styles
        /// </summary>
        [Category("Appearance"),
        Browsable(true),
        Description("Allows Legacy styles to enable and diable."),
        DefaultValue(true)]
        public override bool EnableLegacyStyle
        {
            get
            {
                if (this.ActiveGridView != null && this.ActiveGridView is GridNestedTableControl && this.ActiveGridView.GetGridWindow() != null)
                {
                    this.ActiveGridView.Model.EnableLegacyStyle = this.ActiveGridView.GetGridWindow().Model.EnableLegacyStyle;
                }
                return base.EnableLegacyStyle;
            }
            set
            {
                if (base.EnableLegacyStyle != value)
                {
                    base.EnableLegacyStyle = value;
                    if (this.CellModels.ContainsKey("FilterBarCell"))
                    {
                        if ((!(this.CellModels["FilterBarCell"].GetType().Name.Equals("GridListFilterBarCellModel") || this.CellModels["FilterBarCell"].GetType().Name.Equals("GridTableFilterBarCellModel"))) 
                            && !this.CellModels["FilterBarCell"].GetType().Name.Equals("GridTableFilterBarExtCellModel"))
                        {
                            if (this.EnableGridListControlInComboBox && !this.EnableLegacyStyle)
                            {
                                this.CellModels["FilterBarCell"] = new GridTableFilterBarGridListCellModel(this, true);
                            }
                            else
                            {
                                this.CellModels["FilterBarCell"] = new GridTableFilterBarCellModel(this);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// The <see cref="GridTable"/> with display elements to be displayed in the grid.
        /// </summary>
        public GridTable Table
        {
            get
            {
#if ASPNET
#else
                if (table == null && this.GroupingControl != null)
                {
                    Table = GroupingControl.Table;
                }
#endif
                return table;
            }
            
            set
            {
                if (!Object.ReferenceEquals(table, value))
                {
                    inSetTable = true;
                    OnTableChanging(EventArgs.Empty);
                    UnwireTable();
                    table = value;
                    ////Console.WriteLine(value);
                    table.TableDescriptor.ForwardTableEvents = this;
                    this.ForwardTableEvents = this.Table.Engine.ParentControl;
                    ////ownedTable = false;
                    WireTable();

                    inSetTable = false;

                    inTableChanged = true;
                    OnTableChanged(EventArgs.Empty);
                    inTableChanged = false;
                }
            }
        }

        /// <summary>
        /// Occurs after the <see cref="Table"/> is replaced.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public event EventHandler TableChanged;

        /// <summary>
        /// Occurs before the <see cref="Table"/> is replaced.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public event EventHandler TableChanging;

        /// <summary>
        /// Raises the <see cref="TableChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        [Syncfusion.Documentation.DocumentationExclude]
        protected virtual void OnTableChanged(EventArgs e)
        {
            if (TableChanged != null)
            {
                TableChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TableChanging"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        [Syncfusion.Documentation.DocumentationExclude]
        protected virtual void OnTableChanging(EventArgs e)
        {
            if (TableChanging != null)
            {
                TableChanging(this, e);
            }
        }

        /// <summary>
        /// Replaces the <see cref="Table"/> with raising <see cref="TableChanging"/> or <see cref="TableChanged"/> events.
        /// </summary>
        /// <param name="table">The new table.</param>
        [Syncfusion.Documentation.DocumentationExclude]
        public void SetTableInternal(GridTable table)
        {
            if (this.table != table)
            {
                this.table = table;
                ////Console.WriteLine(table);
                table.TableDescriptor.ForwardTableEvents = this;
                this.ForwardTableEvents = this.Table.Engine.ParentControl;
                table.TableModel = this;
                ResetVolatileData();
                ////rowCount = -1;
                ////colCount = -1;
                ////ownedTable = false;
            }
        }

        void WireTable()
        {
            if (table != null)
            {
                table.TableModel = this;
            }
        }

        void UnwireTable()
        {
            if (table != null)
            {
                table.TableModel = null;
            }
        }

        internal void table_Disposed()
        {
            UnwireTable();
            table = null;
        }

        int updateColumnWidthsColumnsVersion = -1;
        int updateColumnWidthsVisibleColumnsVersion = -1;
        int updateColumnWidthsRelationsVersion = -1;
        int updateColumnWidthsInitializeElementsVersion = -1;
        int updateColumnWidthsEngineAppearanceVersion = -1;

        /// <overload>
        /// Determines if changes were made to the <see cref="GridTableDescriptor.Columns"/> collection
        /// or other changes affecting the widths of columns and if necessary, calculates the maximum
        /// column width for each column.
        /// </overload>
        /// <summary>
        /// Determines if changes were made to the <see cref="GridTableDescriptor.Columns"/> collection
        /// or other changes affecting the widths of columns and if necessary, calculates the maximum
        /// column width for each column.
        /// </summary>
        /// <returns>True if column widths were recalculated; False if cache is still good.</returns>
        public bool UpdateColumnWidths()
        {
            return UpdateColumnWidths(false);
        }

        /// <summary>
        /// Determines if changes were made to the <see cref="GridTableDescriptor.Columns"/> collection
        /// or other changes affecting the widths of columns and if necessary, calculates the maximum
        /// column width for each column.
        /// </summary>
        /// <param name="force">If true forces recalculation of column widths.</param>
        /// <returns>True if column widths were recalculated; False if cache is still good.</returns>
        public bool UpdateColumnWidths(bool force)
        {
            table.EnsureInitialized(false);
            if (!(table != null && (this.table.IsNewUniformChildListRelation() || this.table.SourceList != null)))
            {
                return false;
            }

            // Update width of related tables
            if (updateColumnWidthsRelationsVersion != ((TableDescriptor)Table.TableDescriptor).Relations.Version)
            {
                GridCellModelBase[] cellModels = new GridCellModelBase[CellModels.Count];
                CellModels.Values.CopyTo(cellModels, 0);
                foreach (GridCellModelBase cellModel in cellModels)
                {
                    GridNestedTableControlCellModel tm = cellModel as GridNestedTableControlCellModel;
                    if (tm != null && tm.RelatedTableModel != null && tm.RelatedTable != null && !tm.RelatedTable.IsDisposed)
                    {
                        if (tm.RelatedTableModel.Table == null)
                        {
                            tm.RelatedTableModel.Table = tm.RelatedTable;
                        }

                        if (tm.RelatedTableModel.Table != null)
                        {
                            tm.RelatedTableModel.UpdateColumnWidths();
                        }
                    }
                }

                updateColumnWidthsRelationsVersion = ((TableDescriptor)Table.TableDescriptor).Relations.Version;
            }

            GridTableDescriptor td = table.TableDescriptor;
            ////|| updateColumnWidthsVisibleColumnsVersion != td.VisibleColumns.Version
            if (force || (td.Columns.Count > 0 && (updateColumnWidthsColumnsVersion != td.Columns.Version               
                || updateColumnWidthsInitializeElementsVersion != Table.CategorizeElementsVersion
                || updateColumnWidthsEngineAppearanceVersion != Table.Engine.AppearanceVersion)))
            {
                td.VisibleColumns.columnWidthsDirty = true;
                int[] widths = new int[td.Columns.Count];
                int n = 0;

                IGraphicsProvider graphicsProvider = this.GetGraphicsProvider();  // It is important to hold onto this object as long ad Graphics context is needed!
                Graphics g = graphicsProvider.Graphics;   // Do not dispose this object! It is a cached Display Device context.

                //// Initialize summaries
                td.EnsureSummaryDescriptors();

                //// Get preferred width
                foreach (GridColumnDescriptor column in td.Columns)
                {
                    if (column.Name.IndexOf("Title") != -1)
                    {
                        int w = Table.GetPreferredColumnWidth(g, column);
                    }

                    widths[n++] = Table.GetPreferredColumnWidth(g, column);
                }

                //// Assign to columns
                for (n = 0; n < widths.Length; n++)
                {
                    if (!td.Columns[n].ShouldSerializeWidth())
                    {
                        td.Columns[n].SetWidthInt(widths[n]);
                    }

                    #region handled for rendering Excel filter images in Grid Header
                    if (this.Table != null && this.Table.TableDescriptor != null &&
                         (this.Table.TableDescriptor.Columns[td.Columns[n].Name] != null && this.Table.TableDescriptor.Columns[td.Columns[n].Name].Width != 0))
                    {
                        if (GetFilterCellModel())
                        {
                            if (!td.Columns[n].ShouldSerializeWidth())
                            {
                                if (!wasSizeApplied || (wasSizeApplied && isExcelWired))
                                    td.Columns[n].SetWidthInt(widths[n] + 15);
                                else
                                    td.Columns[n].SetWidthInt(widths[n]);
                            }
                            else
                            {
                                if (!wasSizeApplied)
                                    td.Columns[n].SetWidthInt(this.Table.TableDescriptor.Columns[td.Columns[n].Name].Width + 15);
                                else
                                    td.Columns[n].SetWidthInt(this.Table.TableDescriptor.Columns[td.Columns[n].Name].Width);
                            }
                        }
                        else
                        {
                            if (!td.Columns[n].ShouldSerializeWidth())
                            {
                                if (isExcelWired && !wasSizeApplied)
                                    td.Columns[n].SetWidthInt(widths[n] - 15);
                                else
                                    td.Columns[n].SetWidthInt(widths[n]);
                            }
                            else
                            {
                                if (isExcelWired && !wasSizeApplied)
                                    td.Columns[n].SetWidthInt(this.Table.TableDescriptor.Columns[td.Columns[n].Name].Width - 15);
                                else
                                    td.Columns[n].SetWidthInt(this.Table.TableDescriptor.Columns[td.Columns[n].Name].Width);
                            }
                        }
                    #endregion

                        if (td.Columns[n].isImageApplied)
                            td.Columns[n].SetWidthInt(widths[n] + 20);
                        if (td.Columns[n].isImageApplied && td.Columns[n].HeaderImageAlignment == HeaderImageAlignment.Right && (this.Table.TableDescriptor.SortedColumns.Contains(td.Columns[n].Name) || this.Table.TableDescriptor.GroupedColumns.Contains(td.Columns[n].Name)))
                            td.Columns[n].SetWidthInt(widths[n] + 40);
                    }
                }

                //// Calculate width of TopLevelGroup CaptionText and ChildGroup CaptionText

                string topLevelCaptionText = Table.TableDescriptor.TopLevelGroupOptions.CaptionText;
                ////CaptionSection cs = Table.TopLevelGroup.Caption;
                topLevelCaptionText = GridEngine.GetGroupCaptionDisplayText(Table.TopLevelGroup, topLevelCaptionText);
                int indentCount = Table.TableDescriptor.GroupedColumns.Count;
                int captionTextWidth = ((int)g.MeasureString(topLevelCaptionText, Table.TableDescriptor.Appearance.GroupCaptionCell.GdipFont).Width) + (indentCount * Table.TableDescriptor.TableOptions.IndentWidth);

                ///// Child groups
                foreach (SortColumnDescriptor groupedColumn in Table.TableDescriptor.GroupedColumns)
                {
                    GridColumnDescriptor cd = Table.TableDescriptor.Columns.FindByMappingName(groupedColumn.Name);
                    if (cd != null)
                    {
                        //// Note: Width is based on raw text specified in CaptionText, e.g. {Category}: {RecordCount} Items.
                        string text = cd.GroupByOptions.CaptionText;
                        indentCount--;
                        int width = ((int)g.MeasureString(text, cd.GroupByAppearance.GroupCaptionCell.GdipFont).Width) + (indentCount * Table.TableDescriptor.TableOptions.IndentWidth);
                        captionTextWidth = Math.Max(width, captionTextWidth);
                    }
                }

                //// Calculate total width of all visible columns
                int totalWidth = 0;
                int[] factor;
                GridColumnDescriptor[] columns2 = td.VisibleColumns.GetWidthColumns(out factor);

                for (n = 0; n < columns2.Length; n++)
                {
                    if (columns2[n] != null)
                    {
                        totalWidth += columns2[n].Width / factor[n];
                    }
                }

                //// If width of caption is larger than width of columns, increase width of last column.
                if (totalWidth < captionTextWidth)
                {
                    n = columns2.Length - 1;
                    if (n >= 0 && columns2[n] != null && !columns2[n].ShouldSerializeWidth())
                    {
                        columns2[n].SetWidthInt(captionTextWidth - totalWidth + columns2[n].Width);
                    }
                }

                updateColumnWidthsColumnsVersion = td.Columns.Version;
                updateColumnWidthsVisibleColumnsVersion = td.VisibleColumns.Version;
                updateColumnWidthsInitializeElementsVersion = Table.CategorizeElementsVersion;
                updateColumnWidthsEngineAppearanceVersion = Table.Engine.AppearanceVersion;
            }

            #region for rendering filter image in column header
            if (isExcelWired)
                wasSizeApplied = true;
            #endregion

            return true;
        }

        
        private bool GetFilterCellModel()
        {
            foreach (GridCellModelBase cellModel in this.CellModels.Values)
            {
                if (cellModel.ToString().Contains("GridExcelFilterCellModel") || cellModel.ToString().Contains("Grid2007ExcelFilterCellModel"))
                {
                    isExcelWired = true;
                    return true;
                }
            }
            isExcelWired = false;
            return false;

        }

        /// <override/>
        protected override void OnQueryColCount(GridRowColCountEventArgs e)
        {
            throw new InvalidOperationException("OnQueryColCount should never get hit in a GridTableModel.");
        }

        /// <override/>
        protected override void OnQueryRowCount(GridRowColCountEventArgs e)
        {
            throw new InvalidOperationException("OnQueryRowCount should never get hit in a GridTableModel.");
        }

        internal static bool traceQueryCoveredRange = false;

        void WireNestedTableControlCellModel(GridNestedTableControlCellModel nestedTableControlCellModel)
        {
            GridTableModel nestedTableModel = nestedTableControlCellModel.RelatedTableModel;
        }

        void InvalidateCurrentRecord()
        {
            InvalidateRange(Table.GetCurrentRecordRangeInfo(), GridRangeOptions.None);
        }

        internal bool IsTopAddNewRecord(Element el)
        {
            return el is AddNewRecord && el.GroupLevel == 0;
        }

        /// <override/>
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            throw new InvalidOperationException("OnQueryCellInfo should never get hit in a GridTableModel.");
        }

        /// <override/>
        protected override void OnQueryColWidth(GridRowColSizeEventArgs e)
        {
            if (!HasTable)
            {
                e.Size = 0;
            }
            else
            {
                if (Table.SourceList == null && Table.GetHorizontalScrollWidth() == 0)
                {
                    e.Size = 0; ////Cols.DefaultSize; // could show caption with ": 0 Items." here.
                }
                else if (e.Index == 0)
                {
                    e.Size = Table.DefaultRowHeaderWidth;
                }
                else if (e.Index <= Table.TableDescriptor.GroupedColumns.Count)
                {
                    e.Size = Table.DefaultIndentWidth;
                }
                else if (Table.TableDescriptor.Relations.NestedCount > 0 && e.Index == Table.TableDescriptor.GroupedColumns.Count + 1)
                {
                    e.Size = Table.DefaultTableIndentWidth;
                }
                else if (e.Index < ColCount)
                {
                    int fieldNum = ColIndexToField(e.Index);
                    GridTableDescriptor td = Table.TableDescriptor;
                    int[] factor;
                    GridColumnDescriptor[] columns = Table.TableDescriptor.VisibleColumns.GetWidthColumns(out factor);

                    if (fieldNum >= 0 && fieldNum < columns.Length)
                    {
                        GridColumnDescriptor column = columns[fieldNum];
                        if (column != null)
                        {
                            e.Size = column.Width / factor[fieldNum];
                        }
                    }
                }
                else if (e.Index == ColCount)
                {
                    e.Size = Table.LastColumnWidth;
                }
                else
                {
                    e.Size = 0;
                }
            }

            e.Handled = true;
            base.OnQueryColWidth(e);
        }

        /// <override/>
        protected override void OnSaveColWidth(GridRowColSizeEventArgs e)
        {
            //// RT
            if (!HasTable || e.Index < GetColumnIndentCount() || e.Index > ColCount)
            {
            }
            else
            {
                int fieldNum = ColIndexToField(e.Index);
                GridTableDescriptor td = table.TableDescriptor;
                int[] factor;
                GridColumnDescriptor[] columns = Table.TableDescriptor.VisibleColumns.GetWidthColumns(out factor);

                if (fieldNum >= 0 && fieldNum < columns.Length)
                {
                    GridColumnDescriptor column = columns[fieldNum];
                    if (column != null)
                    {
                        column.Width = e.Size * factor[fieldNum];
                        e.Handled = true;
                    }
                }
            }
            
            base.OnSaveColWidth(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.ColsHiding" /> event.
        /// </summary>
        /// <param name="e">A <see cref="GridRowColHidingEventArgs" /> that contains the event data.</param>
        protected override void OnColsHiding(GridRowColHidingEventArgs e)
        {
            base.OnColsHiding(e);

            if (!e.Cancel)
            {
                GridTableDescriptor td = table.TableDescriptor;
                int[] factor;
                GridColumnDescriptor[] columns = Table.TableDescriptor.VisibleColumns.GetWidthColumns(out factor);

                for (int n = e.From; n <= e.To; n++)
                {
                    if (e.Values[n - e.From])
                    {
                        int fieldNum = ColIndexToField(n);

                        if (fieldNum >= 0 && fieldNum < columns.Length)
                        {
                            GridColumnDescriptor column = columns[fieldNum];
                            if (column != null)
                            {
                                if (ActiveGridView != null && ActiveGridView.CurrentCell.ColIndex == n)
                                {
                                    ActiveGridView.CurrentCell.Deactivate(true);
                                }

                                column.Width = 0;
                            }
                        }
                    }
                }
            
                e.Cancel = true;
            }
        }
        
        /// <override/>
        protected override void OnQueryRowHeightTotal(GridRowColSizeTotalEventArgs e)
        {
            base.OnQueryRowHeightTotal(e);

            if (e.Handled || !HasTable)
            {
                return;
            }

            e.Size = Table.GetRecordHeightTotal(this.FilteredChildTable, e.From, e.Last);
            e.Handled = true;
        }

        /// <override/>
        protected override void OnQueryRowHeight(GridRowColSizeEventArgs e)
        {
            base.OnQueryRowHeight(e);

            if (e.Handled || !HasTable)
            {
                return;
            }

            e.Size = Table.GetRecordHeight(this.FilteredChildTable, e.Index);
            e.Handled = true;
        }

        /// <override/>
        protected override void OnSaveHideRow(GridRowColHideEventArgs e)
        {
            e.Handled = true;
        }

        /// <override/>
        protected override void OnRowsHiding(GridRowColHidingEventArgs e)
        {
            base.OnRowsHiding(e);

            if (Table.Engine.CounterLogic == EngineCounters.YAmount
                || Table.Engine.CounterLogic == EngineCounters.All)
            {
                // Hiding rows is not supported with grouping grid since the counter logic
                // would get messed up in such case.
                e.Cancel = true;
            }
        }

        /// <override/>
        protected override void OnSaveRowHeight(GridRowColSizeEventArgs e)
        {
            base.OnSaveRowHeight(e);

            if (e.Handled || !HasTable)
            {
                return;
            }

            //// TODO: Option to specify whether changes should be saved in Table.TableOptions or TableDescriptor.TableOptions
            //// (At the moment, Table.TableOptions is same as TableDescriptor.TableOptions.)

            if (e.Index < Table.NestedDisplayElements.Count)
            {
                Element el = Table.NestedDisplayElements[e.Index];

                GridTable tb = (GridTable)el.ParentTable;

                if (el is IGridRowHeight && ((IGridRowHeight)el).SupportsRowHeight())
                {
                    ((IGridRowHeight)el).RowHeight = e.Size;
                    el.InvalidateCounterBottomUp();
                }
                else if (CaptionSection.IsCaption(el))
                {
                    tb.DefaultCaptionRowHeight = e.Size;
                }
                else if (el is ColumnHeaderRow || el is ColumnHeaderSection)
                {
                    tb.DefaultColumnHeaderRowHeight = e.Size;
                }
                else
                {
                    tb.DefaultRecordRowHeight = e.Size;
                }

                e.Handled = true;
            }
        }

        /// <override/>
        protected override void OnRowHeightsChanged(GridRowColSizeChangedEventArgs e)
        {
            base.OnRowHeightsChanged(e);
            Refresh();
        }

        /// <override/>
        protected override void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            base.OnQueryCoveredRange(e);
            if (!e.Handled && HasTable)
            {
                Table.RaiseQueryCoveredRange(e);
            }

            if (traceQueryCoveredRange)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
        }

        /// <summary>
        /// Returns the <see cref="GridRangeInfo"/> that spans the cells where a row header for the specified element should be displayed. Can
        /// span multiple rows if a table has multiple rows per record.
        /// </summary>
        /// <param name="element">The display element.</param>
        /// <returns>The cell range.</returns>
        public GridRangeInfo GetRowHeaderRange(Element element)
        {
            return Table.GetRowHeaderRange(element);
        }

        /// <override/>
        protected override void OnSaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            if (HasTable && e.RowIndex >= 0 && e.ColIndex >= 0)
            {
                if (e.RowIndex < GetDisplayElementCount())
                {
                    Element el = GetDisplayElementAt(e.RowIndex);

                    if ((el is RecordRow || el is Record) && e.ColIndex > el.GroupLevel)
                    {
                        Record record = Record.GetParentRecord(el);
                        GridColumnDescriptor column = GetColumnDescriptorAt(e.RowIndex, e.ColIndex);

                        if (column != null && column.FieldDescriptor != null)
                        {
                            record.SetValue(column.FieldDescriptor, e.Style.CellValue);
                        }
                    }
                }
             
                e.Handled = true;
            }
            
            base.OnSaveCellInfo(e);
        }
        
        /// <summary>
        /// Returns the <see cref="GridRangeInfo"/> that spans the cells where a column should be displayed for a specific record. Can
        /// span multiple rows if a table has multiple rows per record or multiple grid columns.
        /// </summary>
        /// <param name="record">The Record.</param>
        /// <param name="fieldDescriptorName">The name of the field descriptor (which is GridColumnDescriptor.MappingName).</param>
        /// <returns>The cell range.</returns>
        public GridRangeInfo RecordFieldToRangeInfo(Record record, string fieldDescriptorName)
        {
            int rowIndex = GetDisplayElementIndexOf(record);
            if (rowIndex != -1)
            {
                int r, c;
                Table.TableDescriptor.ColumnToRowColIndex(fieldDescriptorName, out r, out c);
                int colIndex = this.GetColumnIndentCount();
                if (r >= 0 && c >= 0)
                {
                    colIndex = FieldToColIndex(c);
                    rowIndex += r;
                }
        
                return GridRangeInfo.Cell(rowIndex, colIndex);
            }
            
            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// Returns the <see cref="GridRangeInfo"/> that spans the cells where a column should be displayed for a specific record. Can
        /// span multiple rows if a table has multiple rows per record or multiple grid columns.
        /// </summary>
        /// <param name="record">The Record.</param>
        /// <param name="fd">The field descriptor.</param>
        /// <returns>The cell range.</returns>
        public GridRangeInfo RecordFieldToRangeInfo(Record record, FieldDescriptor fd)
        {
            return RecordFieldToRangeInfo(record, fd.Name);
        }

        internal void RegisterNestedTableCellModels()
        {
            for (int n = 0; n < Table.RelatedTables.Count; n++)
            {
                GridTable relatedTable = (GridTable)Table.RelatedTables[n];
                RelationDescriptor rd = Table.TableDescriptor.Relations[n];
                //// ForeignListItems
                if (rd.RelationKind == RelationKind.ForeignKeyReference
                    || rd.RelationKind == RelationKind.ListItemReference
                    || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                {
#if FK_SUPPORT
                    RegisterForeignKeyTableCellModel(relatedTable);
#endif
                }
                else
                {
                    RegisterNestedTableCellModel(relatedTable);
                }
            }
        }

        internal GridNestedTableControlCellModel RemoveNestedTableCellModel(string relationName)
        {
            string cellType = "RT" + relationName;
            if (this.CellModels.ContainsKey(cellType))
            {
                GridNestedTableControlCellModel nestedTableControlCellModel = (GridNestedTableControlCellModel)this.CellModels[cellType];
                this.CellModels.Remove(cellType);
                nestedTableControlCellModel.Dispose();
            }
        
            return null;
        }

        internal GridNestedTableControlCellModel RegisterNestedTableCellModel(GridTable relatedTable)
        {
            string cellType = "RT" + relatedTable.TableDescriptor.Name;
            if (!this.CellModels.ContainsKey(cellType) || !(this.CellModels[cellType] is GridNestedTableControlCellModel))
            {
                GridNestedTableControlCellModel nestedTableControlCellModel = this.Table.Engine.CreateNestedTableControlCellModel(this, relatedTable);
                ////LLthis.CellModels.Remove(cellType);
                this.CellModels.Add(cellType, nestedTableControlCellModel);
                WireNestedTableControlCellModel(nestedTableControlCellModel);
                ////nestedTableControlCellModel.TableModel.UpdateColumnWidths();
                return nestedTableControlCellModel;
            }

            return null;
        }

        /// <override/>
        /// <summary>Gets the datasource of the specified style.</summary>
        /// <param name="style">The specified style.</param>
        /// <returns>returns the Datasource.</returns>
        public override object GetStyleDataSource(GridStyleInfo style)
        {
            if (style.ChoiceList != null && style.ChoiceList.Count > 0)
            {
                return style.ChoiceList;
            }
            else if (style.DataSource != null)
            {
                return style.DataSource;
            }
            else
            {
                GridTableCellStyleInfo tableStyleInfo = style as GridTableCellStyleInfo;
                GridTableCellStyleInfoIdentity id = tableStyleInfo.TableCellIdentity;
                Element el = id.DisplayElement;
                Record record = Record.GetParentRecord(el);
                bool savedUseDefaultValueInGetValue = false;
                AddNewRecord addNewRecord = record as AddNewRecord;
                if (addNewRecord != null)
                {
                    savedUseDefaultValueInGetValue = addNewRecord.UseDefaultValueInGetValue;
                    addNewRecord.UseDefaultValueInGetValue = true;
                }

                try
                {
                    Table table = record.ParentTable;
                    GridColumnDescriptor column = id.Column;
                    FieldDescriptor fd = column.FieldDescriptor;
                    if (fd != null)
                    {
                        RelationDescriptor rd = fd.GetRelation();
                        if (fd.IsRelatedField())
                        {
                            if (fd.IsComplexPropertyField())
                            {
                                return ((GridTable)table.RelatedTables[rd.Name]).TopLevelGroup.GroupTypedListRecords;
                            }
                            else if (rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                            {
                                return record.GetRelatedChildTable(rd).GroupTypedListRecords;
                                //// ForeignListItems
                            }
                            else if (rd.RelationKeys.Count > 0)
                            {
                                FieldDescriptor foreignKeyField = rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField;
                                FieldDescriptor displayField = fd.GetRelatedDescriptor();

                                /* Make it work with GridListControl cell
                                // style.CellValue = record.GetValue(foreignKeyField);
                                // style.CellValueType = foreignKeyField.GetPropertyType();*/
                                GridTable relatedTable = (GridTable)table.RelatedTables[rd.Name];
                                if (relatedTable != null)
                                {
                                    if (rd.RelationKeys.Count == 1)
                                    {
                                        return relatedTable.TopLevelGroup.GroupTypedListRecords;
                                    }
                                    else
                                    {
                                        //// get child table ....
                                        object[] values = new object[rd.RelationKeys.Count - 1];
                                        for (int n = 0; n < rd.RelationKeys.Count - 1; n++)
                                        {
                                            values[n] = record.GetValue(rd.RelationKeys[n].ParentKeyField);
                                        }

                                        ChildTable ct = relatedTable.AddChildTableIfNotExists(values);
                                        return ct.GroupTypedListRecords;
                                    }
                                }
                            }
                        }
                        else
                        {
                            PropertyDescriptor pd = fd.GetPropertyDescriptor();
                            if (pd != null && pd.Converter != null && pd.Converter.CanConvertTo(typeof(string)) && pd.Converter.GetStandardValuesSupported())
                            {
                                return this.GetCachedStandardValues(pd.Converter, pd.PropertyType);
                            }
                        }
                    }
                }
                finally
                {
                    if (addNewRecord != null)
                    {
                        addNewRecord.UseDefaultValueInGetValue = savedUseDefaultValueInGetValue;
                    }
                }
            }

            return null;
        }

        bool inClearingCells = false;

        /// <override/>
        protected /*internal*/ override void OnClearingCells(GridClearingCellsEventArgs e)
        {
            if (inClearingCells)
            {
                e.ClearStyle = false;
                return;
            }

            base.OnClearingCells(e);
            if (e.Handled)
            {
                return;
            }

            try
            {
                inClearingCells = true;
                e.Handled = true;

                if (this.ReadOnly || !(this.Table.SourceListAllowEdit && this.Table.TableDescriptor.AllowEdit))
                {
                    return;
                }

                if (this.ActiveGridView != null && e.RangeList.Count == 1 && e.RangeList[0].Equals(this.ActiveGridView.CurrentCell.RangeInfo))
                {
                    this.Table.CurrentRecordManager.BeginEdit();
                }

                //// this will trigger a recursive call to OnClearingCells
                e.Result = ClearCells(e.RangeList, e.ClearStyle);
            }
            finally
            {
                inClearingCells = false;
            }
        }

        /// <override/>
        protected override void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            base.OnQueryCellFormattedText(e);

            Table.Engine.RaiseQueryCellFormattedText(e);
        }

        /// <override/>
        protected override void OnQueryCellText(GridCellTextEventArgs e)
        {
            base.OnQueryCellText(e);

            Table.Engine.RaiseQueryCellText(e);
        }

        /// <override/>
        protected override void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            base.OnParseCommonFormats(e);

            Table.Engine.RaiseParseCommonFormats(e);
        }

        /// <override/>
        protected override void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            base.OnSaveCellFormattedText(e);

            Table.Engine.RaiseSaveCellFormattedText(e);
        }

        /// <override/>
        protected override void OnSaveCellText(GridCellTextEventArgs e)
        {
            base.OnSaveCellText(e);

            Table.Engine.RaiseSaveCellText(e);
        }

        internal GridBorder gridBorder = null;
        internal GridBorder fixedBorder = null;

        /// <summary>
        /// Creates a GridBorder object based on TableOptions.GridLineBorder. If its style is set to 
        /// GridBorderStyle.Standard the default properties Options.DefaultGridBorderStyle
        /// and Properties.GridLineColor will be used.
        /// </summary>
        /// <returns>Grid line border.</returns>
        public override GridBorder GetGridLineBorder()
        {
            if (gridBorder == null)
            {
                gridBorder = Table.TableOptions.GridLineBorder;
            }

            if (gridBorder.Style != GridBorderStyle.Standard)
            {
                return gridBorder;
            }

            return base.GetGridLineBorder();
        }

        /// <summary>
        /// Creates a GridBorder object based on TableOptions.GridLineBorder. If its style is set to 
        /// GridBorderStyle.Standard the default properties Options.DefaultGridBorderStyle
        /// and Properties.FixedLineColor will be used.
        /// </summary>
        /// <returns>Fixed line border.</returns>
        public override GridBorder GetFixedLineBorder()
        {
            if (fixedBorder == null)
            {
                fixedBorder = Table.TableOptions.GridLineBorder;
            }

            if (fixedBorder.Style != GridBorderStyle.Standard)
            {
                return fixedBorder;
            }

            return base.GetFixedLineBorder();
        }

        #region ITableEventsTarget
        //// tevent ExceptionRaised ExceptionRaisedEventArgs

        /// <summary>
        /// Occurs when an unknown exception has been cached while modifying underlying data in the datasource.
        /// </summary>
        /// <remarks>
        /// If necessary, you can rethrow the exception in your event handler.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs when an unknown exception has been cached while modifying underlying data in the datasource.")]
        public event ExceptionRaisedEventHandler ExceptionRaised;
       
        /// <summary>
        /// Raises the <see cref="ExceptionRaised"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ExceptionRaisedEventArgs" /> that contains the event data.</param>
        protected virtual void OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (ExceptionRaised != null)
            {
                ExceptionRaised(this, e);
            }
        }
     
        void ITableEventsTarget.OnExceptionRaised(ExceptionRaisedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnExceptionRaised(e);
            }

            OnExceptionRaised(e);
        }
       
        //// tevent GroupCollapsing GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsing;
       
        /// <summary>
        /// Raises the <see cref="GroupCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsing(GroupEventArgs e)
        {
            if (GroupCollapsing != null)
            {
                GroupCollapsing(this, e);
            }
        }
       
        void ITableEventsTarget.OnGroupCollapsing(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsing(e);
            }

            OnGroupCollapsing(e);
        }
        
        //// tevent GroupCollapsed GroupEventArgs

        /// <summary>
        /// Occurs before a group is collapsed.
        /// </summary>
        [Description("Occurs before a group is collapsed.")]
        [Category("Table")]
        public event GroupEventHandler GroupCollapsed;
        
        /// <summary>
        /// Raises the <see cref="GroupCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupCollapsed(GroupEventArgs e)
        {
            if (GroupCollapsed != null)
            {
                GroupCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnGroupCollapsed(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupCollapsed(e);
            }

            OnGroupCollapsed(e);
        }

        //// tevent GroupExpanding GroupEventArgs

        /// <summary>
        /// Occurs before a group is expanded.
        /// </summary>
        [Description("Occurs before a group is expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanding;

        /// <summary>
        /// Raises the <see cref="GroupExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanding(GroupEventArgs e)
        {
            if (GroupExpanding != null)
            {
                GroupExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanding(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanding(e);
            }

            OnGroupExpanding(e);
        }

        //// tevent GroupExpanded GroupEventArgs

        /// <summary>
        /// Occurs after a group was expanded.
        /// </summary>
        [Description("Occurs after a group was expanded.")]
        [Category("Table")]
        public event GroupEventHandler GroupExpanded;

        /// <summary>
        /// Raises the <see cref="GroupExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupExpanded(GroupEventArgs e)
        {
            if (GroupExpanded != null)
            {
                GroupExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupExpanded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupExpanded(e);
            }

            OnGroupExpanded(e);
        }

        //// tevent RecordCollapsing RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs before a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsing;

        /// <summary>
        /// Raises the <see cref="RecordCollapsing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsing(RecordEventArgs e)
        {
            if (RecordCollapsing != null)
            {
                RecordCollapsing(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsing(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsing(e);
            }

            OnRecordCollapsing(e);
        }

        //// tevent RecordCollapsed RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is collapsed.
        /// </summary>
        [Description("Occurs after a record with nested tables is collapsed.")]
        [Category("Table")]
        public event RecordEventHandler RecordCollapsed;

        /// <summary>
        /// Raises the <see cref="RecordCollapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordCollapsed(RecordEventArgs e)
        {
            if (RecordCollapsed != null)
            {
                RecordCollapsed(this, e);
            }
        }

        void ITableEventsTarget.OnRecordCollapsed(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordCollapsed(e);
            }

            OnRecordCollapsed(e);
        }

        //// tevent RecordExpanding RecordEventArgs

        /// <summary>
        /// Occurs before a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs before a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanding;

        /// <summary>
        /// Raises the <see cref="RecordExpanding"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanding(RecordEventArgs e)
        {
            if (RecordExpanding != null)
            {
                RecordExpanding(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanding(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanding(e);
            }

            OnRecordExpanding(e);
        }

        //// tevent RecordExpanded RecordEventArgs

        /// <summary>
        /// Occurs after a record with nested tables is expanded.
        /// </summary>
        [Description("Occurs after a record with nested tables is expanded.")]
        [Category("Table")]
        public event RecordEventHandler RecordExpanded;

        /// <summary>
        /// Raises the <see cref="RecordExpanded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordExpanded(RecordEventArgs e)
        {
            if (RecordExpanded != null)
            {
                RecordExpanded(this, e);
            }
        }

        void ITableEventsTarget.OnRecordExpanded(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordExpanded(e);
            }

            OnRecordExpanded(e);
        }

        //// tevent RecordDeleting RecordEventArgs

        /// <summary>
        /// Occurs before a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="Syncfusion.Grouping.Engine.SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs before a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleting;

        /// <summary>
        /// Raises the <see cref="RecordDeleting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleting(RecordEventArgs e)
        {
            if (RecordDeleting != null)
            {
                RecordDeleting(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleting(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleting(e);
            }

            OnRecordDeleting(e);
        }

        //// tevent RecordDeleted RecordEventArgs

        /// <summary>
        /// Occurs after a record is deleted.
        /// </summary>
        /// <remarks>
        /// This event is raised only when the <see cref="Table"/> or <see cref="Record"/> triggers the deletion. If
        /// the underlying source list deletes the record, a <see cref="Syncfusion.Grouping.Engine.SourceListListChanged"/> event is raised instead.
        /// </remarks>
        [Description("Occurs after a record is deleted.")]
        [Category("Table")]
        public event RecordEventHandler RecordDeleted;

        /// <summary>
        /// Raises the <see cref="RecordDeleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordDeleted(RecordEventArgs e)
        {
            if (RecordDeleted != null)
            {
                RecordDeleted(this, e);
            }
        }

        void ITableEventsTarget.OnRecordDeleted(RecordEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordDeleted(e);
            }

            OnRecordDeleted(e);
        }

        //// tevent CurrentRecordContextChange CurrentRecordContextChangeEventArgs

        /// <summary>
        /// Occurs before and after the status of the current record is changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information which current record state was changed.
        /// </summary>
        [Description("Occurs before and after the status of the current record is changed.")]
        [Category("Table")]
        public event CurrentRecordContextChangeEventHandler CurrentRecordContextChange;

        /// <summary>
        /// Raises the <see cref="CurrentRecordContextChange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CurrentRecordContextChangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (CurrentRecordContextChange != null)
            {
                CurrentRecordContextChange(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordContextChange(e);
            }

            OnCurrentRecordContextChange(e);
        }

        //// tevent CurrentRecordManagerReset tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="CurrentRecordManager.Reset"/> method of the <see cref="CurrentRecordManager"/> is called.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this events and resets any "Current Cell" state when this
        /// event is raised.
        /// </remarks>
        [Description("Occurs when the CurrentRecordManager.Reset method of the CurrentRecordManager is called.")]
        [Category("Table")]
        public event TableEventHandler CurrentRecordManagerReset;

        /// <summary>
        /// Raises the <see cref="CurrentRecordManagerReset"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (CurrentRecordManagerReset != null)
            {
                CurrentRecordManagerReset(this, e);
            }
        }

        void ITableEventsTarget.OnCurrentRecordManagerReset(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCurrentRecordManagerReset(e);
            }

            OnCurrentRecordManagerReset(e);
        }

        //// tevent GroupSummaryInvalidated GroupEventArgs

        /// <summary>
        /// Occurs when a summary has been marked dirty.
        /// </summary>
        /// <remarks>
        /// The GridGroupingControl listens to this event and will force a repaint of the specified summary if it is visible
        /// when this event was raised.
        /// </remarks>
        [Description("Occurs when a summary has been marked dirty.")]
        [Category("Table")]
        public event GroupEventHandler GroupSummaryInvalidated;

        /// <summary>
        /// Raises the <see cref="GroupSummaryInvalidated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (GroupSummaryInvalidated != null)
            {
                GroupSummaryInvalidated(this, e);
            }
        }

        void ITableEventsTarget.OnGroupSummaryInvalidated(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupSummaryInvalidated(e);
            }

            OnGroupSummaryInvalidated(e);
        }

        //// tevent SourceListListChanged TableListChangedEventArgs

        /// <summary>
        /// Occurs before the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list. More detailed <see cref="SourceListRecordChanged"/> events will be
        /// raised after this event.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event before the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChanged;

        /// <summary>
        /// Raises the <see cref="SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (SourceListListChanged != null)
            {
                SourceListListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChanged(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChanged(e);
            }

            OnSourceListListChanged(e);
        }

        //// tevent SourceListListChangedCompleted TableListChangedEventArgs

        /// <summary>
        /// Occurs after the <see cref="Table"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer the chance to react to an <see cref="IBindingList.ListChanged"/>
        /// event right after the engine since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs after the Table processes the IBindingList.ListChanged event.")]
        [Category("Table")]
        public event TableListChangedEventHandler SourceListListChangedCompleted;

        /// <summary>
        /// Raises the <see cref="SourceListListChangedCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (SourceListListChangedCompleted != null)
            {
                SourceListListChangedCompleted(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListListChangedCompleted(TableListChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListListChangedCompleted(e);
            }

            OnSourceListListChangedCompleted(e);
        }

        //// tevent SourceListRecordChanged RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying datasource is added, removed, or changed and after
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Description("Occurs when a record in the underlying data source is added, removed, or changed and the table is updated.")]
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanged;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanged != null)
            {
                SourceListRecordChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanged(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanged(e);
            }

            OnSourceListRecordChanged(e);
        }

        ////  tevent SourceListRecordChanging RecordChangedEventArgs

        /// <summary>
        /// Occurs when a record in the underlying data source is added, removed, or changed and before
        /// the <see cref="Table"/> is updated with that change.
        /// </summary>
        [Category("Table")]
        public event RecordChangedEventHandler SourceListRecordChanging;

        /// <summary>
        /// Raises the <see cref="SourceListRecordChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (SourceListRecordChanging != null)
            {
                SourceListRecordChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSourceListRecordChanging(RecordChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSourceListRecordChanging(e);
            }

            OnSourceListRecordChanging(e);
        }

        //// tevent GroupAdded GroupEventArgs

        /// <summary>
        /// Occurs when a new group is added to a categorized table after a record is changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a new group is added to a categorized table after a record is changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupAdded;

        /// <summary>
        /// Raises the <see cref="GroupAdded"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupAdded(GroupEventArgs e)
        {
            if (GroupAdded != null)
            {
                GroupAdded(this, e);
            }
        }

        void ITableEventsTarget.OnGroupAdded(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupAdded(e);
            }

            OnGroupAdded(e);
        }

        //// tevent GroupRemoving GroupEventArgs

        /// <summary>
        /// Occurs when a group was removed from a categorized table after a record was changed. The event does not
        /// occur during categorization of the table. See the <see cref="CategorizedRecords"/> elements to when categorization
        /// finished.
        /// </summary>
        [Description("Occurs when a group was removed from a categorized table after a record was changed.")]
        [Category("Table")]
        public event GroupEventHandler GroupRemoving;

        /// <summary>
        /// Raises the <see cref="GroupRemoving"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnGroupRemoving(GroupEventArgs e)
        {
            if (GroupRemoving != null)
            {
                GroupRemoving(this, e);
            }
        }

        void ITableEventsTarget.OnGroupRemoving(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnGroupRemoving(e);
            }

            OnGroupRemoving(e);
        }

        //// tevent SortingItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs before the records for a group are sorted.
        /// </summary>
        /// <remarks>
        /// The engine has built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Description("Occurs before the records for a group are sorted.")]
        [Category("Table")]
        public event GroupEventHandler SortingItemsInGroup;

        /// <summary>
        /// Raises the <see cref="SortingItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (SortingItemsInGroup != null)
            {
                SortingItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortingItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortingItemsInGroup(e);
            }

            OnSortingItemsInGroup(e);
        }

        //// tevent SortedItemsInGroup GroupEventArgs

        /// <summary>
        /// Occurs after the records for a group were sorted.
        /// </summary>
        /// <remarks>
        /// The engine has built-in optimization for sorting columns that allows it to perform the sorting
        /// on an on-demand basis group-by-group. Suppose you have a table with 200 different countries and
        /// you change the sort order of the cities. It is not necessary to sort the whole table. Instead,
        /// the individual groups can be sorted when they are scrolled into view. SortingItemsInGroup and
        /// SortedItemsInGroup events are fired in such cases when a specific group was sorted on demand.
        /// <para/>
        /// If the whole table was set dirty (see <see cref="Syncfusion.Grouping.Table.TableDirty"/>), then the whole table
        /// is simply recategorized. In that case, only a CategorizedElements event is raised but no
        /// SortingItemsInGroup event.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after the records for a group are sorted.")]
        public event GroupEventHandler SortedItemsInGroup;

        /// <summary>
        /// Raises the <see cref="SortedItemsInGroup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GroupEventArgs" /> that contains the event data.</param>
        protected virtual void OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (SortedItemsInGroup != null)
            {
                SortedItemsInGroup(this, e);
            }
        }

        void ITableEventsTarget.OnSortedItemsInGroup(GroupEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSortedItemsInGroup(e);
            }

            OnSortedItemsInGroup(e);
        }

        //// tevent InvalidatingCounters tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateCounterTopDown"/> of a <see cref="Table"/> is called
        /// and before all counters are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateCounterTopDown method of a table is called.")]
        public event TableEventHandler InvalidatingCounters;

        /// <summary>
        /// Raises the <see cref="InvalidatingCounters"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingCounters(TableEventArgs e)
        {
            if (InvalidatingCounters != null)
            {
                InvalidatingCounters(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingCounters(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingCounters(e);
            }

            OnInvalidatingCounters(e);
        }

        //// tevent InvalidatingSummaries tableEventsTarget

        /// <summary>
        /// Occurs when the <see cref="Syncfusion.Grouping.Table.InvalidateSummariesTopDown"/> of a <see cref="Table"/> is called
        /// and before all summaries in that table are marked dirty.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when the Table.InvalidateSummariesTopDown of a table is called.")]
        public event TableEventHandler InvalidatingSummaries;

        /// <summary>
        /// Raises the <see cref="InvalidatingSummaries"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnInvalidatingSummaries(TableEventArgs e)
        {
            if (InvalidatingSummaries != null)
            {
                InvalidatingSummaries(this, e);
            }
        }

        void ITableEventsTarget.OnInvalidatingSummaries(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnInvalidatingSummaries(e);
            }

            OnInvalidatingSummaries(e);
        }

        //// tevent CategorizingRecords tableEventsTarget

        /// <summary>
        /// Occurs before records are categorized after a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set True, e.g. because schema information for a table is changed
        /// or because the grouped columns are changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Syncfusion.Grouping.Element"/> of the <see cref="Table"/> will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs before records are categorized after a table is marked dirty.")]
        public event TableEventHandler CategorizingRecords;

        /// <summary>
        /// Raises the <see cref="CategorizingRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizingRecords(TableEventArgs e)
        {
            if (CategorizingRecords != null)
            {
                CategorizingRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizingRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizingRecords(e);
            }

            OnCategorizingRecords(e);
        }

        //// tevent CategorizedRecords tableEventsTarget

        /// <summary>
        /// Occurs after records were categorized after a table is marked dirty (<see cref="Syncfusion.Grouping.Table.TableDirty"/>).
        /// </summary>
        /// <remarks>
        /// When <see cref="Syncfusion.Grouping.Table.TableDirty"/> is set True, e.g. because schema information for a table was changed
        /// or because the grouped columns were changed, the table will categorize records on demand the first time
        /// information about a record is accessed. At that time, the <see cref="Element"/> of the <see cref="Table"/>
        /// will start
        /// categorization.
        /// </remarks>
        [Category("Table")]
        [Description("Occurs after records are categorized after a table was marked dirty.")]
        public event TableEventHandler CategorizedRecords;

        /// <summary>
        /// Raises the <see cref="CategorizedRecords"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnCategorizedRecords(TableEventArgs e)
        {
            if (CategorizedRecords != null)
            {
                CategorizedRecords(this, e);
            }
        }

        void ITableEventsTarget.OnCategorizedRecords(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnCategorizedRecords(e);
            }

            OnCategorizedRecords(e);
        }

        //// tevent TableSourceListChanged Table

        /// <summary>
        /// Occurs after the data source was replaced.
        /// </summary>
        [Category("Table")]
        [Description("Occurs after the datasource was replaced.")]
        public event TableEventHandler TableSourceListChanged;

        /// <summary>
        /// Raises the <see cref="TableSourceListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TableEventArgs" /> that contains the event data.</param>
        protected virtual void OnTableSourceListChanged(TableEventArgs e)
        {
            if (TableSourceListChanged != null)
            {
                TableSourceListChanged(this, e);
            }
        }

        void ITableEventsTarget.OnTableSourceListChanged(TableEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnTableSourceListChanged(e);
            }

            OnTableSourceListChanged(e);
        }

        //// tevent RecordValueChanging RecordValueChanging

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.
        /// </summary>
        [Category("Table")]
        [Description("Occurs when a RecordFieldCell cell's value is changed and before Record.SetValue is called.")]
        public event RecordValueChangingEventHandler RecordValueChanging;

        /// <summary>
        /// Raises the <see cref="RecordValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (RecordValueChanging != null)
            {
                RecordValueChanging(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanging(RecordValueChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanging(e);
            }

            OnRecordValueChanging(e);
        }

        //// tevent RecordValueChanged RecordValueChanged

        /// <summary>
        /// Occurs when a RecordFieldCell cell's value is changed and after Record.SetValue returned.
        /// </summary>
        public event RecordValueChangedEventHandler RecordValueChanged;

        /// <summary>
        /// Raises the <see cref="RecordValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="RecordValueChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (RecordValueChanged != null)
            {
                RecordValueChanged(this, e);
            }
        }

        void ITableEventsTarget.OnRecordValueChanged(RecordValueChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnRecordValueChanged(e);
            }

            OnRecordValueChanged(e);
        }

        //// tevent DisplayElementChanging DisplayElementChanging

        /// <summary>
        /// When number of visible elements were changed.
        /// </summary>
        public event DisplayElementChangingEventHandler DisplayElementChanging;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangingEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (DisplayElementChanging != null)
            {
                DisplayElementChanging(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanging(DisplayElementChangingEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanging(e);
            }

            OnDisplayElementChanging(e);
        }

        //// tevent DisplayElementChanged DisplayElementChanged

        /// <summary>
        /// When number of visible elements were changed.
        /// </summary>
        public event DisplayElementChangedEventHandler DisplayElementChanged;

        /// <summary>
        /// Raises the <see cref="DisplayElementChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="DisplayElementChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (DisplayElementChanged != null)
            {
                DisplayElementChanged(this, e);
            }
        }

        void ITableEventsTarget.OnDisplayElementChanged(DisplayElementChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnDisplayElementChanged(e);
            }

            OnDisplayElementChanged(e);
        }

        //// tevent SelectedRecordsChanging SelectedRecordsChanging

        /// <summary>
        /// Occurs before the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection is modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanging;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanging != null)
            {
                SelectedRecordsChanging(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanging(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanging(e);
            }

            OnSelectedRecordsChanging(e);
        }

        //// tevent SelectedRecordsChanged SelectedRecordsChanged

        /// <summary>
        /// Occurs after the <see cref="Syncfusion.Grouping.Table.SelectedRecords"/> collection was modified.
        /// </summary>
        public event SelectedRecordsChangedEventHandler SelectedRecordsChanged;

        /// <summary>
        /// Raises the <see cref="SelectedRecordsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="SelectedRecordsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (SelectedRecordsChanged != null)
            {
                SelectedRecordsChanged(this, e);
            }
        }

        void ITableEventsTarget.OnSelectedRecordsChanged(SelectedRecordsChangedEventArgs e)
        {
            if (this.tableEventsTarget != null)
            {
                tableEventsTarget.OnSelectedRecordsChanged(e);
            }

            OnSelectedRecordsChanged(e);
        }
        
        ITableEventsTarget tableEventsTarget;

        /// <summary>
        /// Gets or sets a object that handles events raised by the <see cref="Table"/> object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ITableEventsTarget ForwardTableEvents
        {
            get
            {
                return this.tableEventsTarget;
            }

            set
            {
                this.tableEventsTarget = value;
            }
        }
        #endregion
    }

    internal class GridTableModelVolatileData : object, IGridVolatileData
    {
        GridTableModel model;

        public GridTableModelVolatileData(GridTableModel model)
        {
            this.model = model;
        }

        #region IGridVolatileData Members

        public void ResetRowCount()
        {
        }

        public int ColCount
        {
            get
            {
                if (model.HasTable)
                {
                    return model.Table.TableDescriptor.GetColumnSetColCount() + model.Table.TableDescriptor.GroupedColumns.Count + 2;
                }

                return 0;
            }

            set
            {
            }
        }

        public int RowCount
        {
            get
            {
                if (model.HasTable)
                {
                    return model.Table.DisplayElements.Count - 1;
                }

                return 0;
            }

            set
            {
            }
        }

        public bool HasRowCount
        {
            get
            {
                return true;
            }
        }

        public void Clear()
        {
        }

        public bool HasColCount
        {
            get
            {
                return true;
            }
        }

        public void ResetColCount()
        {
        }

        public void ResetItem(GridCellPos cell)
        {
        }

        #endregion

        #region IGridData Members

        public GridStyleInfo this[int rowIndex, int colIndex]
        {
            get
            {
                return model.Table.GetTableCellStyle(rowIndex, colIndex);
            }

            set
            {
            }
        }

        public GridCellModelBase LookupCellModel(string id)
        {
            return model.CellModels[id];
        }

        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                return model.BaseStylesMap;
            }
        }

        public GridStyleInfo[] GetBaseStyles(GridStyleInfo styleInfo, int rowIndex, int colIndex)
        {
            return new GridStyleInfo[0];
        }
        #endregion
    }
}