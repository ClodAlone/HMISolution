//-------------------------------------------------------------------------------------------------
// <copyright file="GridTable.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

#if ASPNET
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Web.UI.WebControls.Tools;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
using System.Windows.Forms;
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Manages all the records from the underlying source list. The source list can be any IList collection.
    /// If it implements IBindingList, the GridTable will listen to the ListChangedEvent and update its internal
    /// data whenever changes are made to the source list.
    /// </summary>
    /// <remarks>
    /// See the <see cref="GridTableBase"/> class for more details overview about this class.
    /// </remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GridTable :
#if ASPNET
                                Table,
#else
 GridTableBase,
#endif
 IGridTableOptionsSource, IGridTableCellAppearanceSource
    {
        /// <summary>
        /// Occurs before and after the status of the current record was changed. Check the <see cref="CurrentRecordContextChangeEventArgs.Action"/>
        /// of the <see cref="CurrentRecordContextChangeEventArgs"/> to get information on which current record state was changed.
        /// </summary>
        public CurrentRecordContextChangeEventHandler CurrentRecordContextChangeTarget;

        #region Fields
        GridTableDescriptor _tableDescriptor;

        GridTableModel tableModel = null;
        private int lastColumnWidth;
        ////GridTableOptionsStyleInfo tableOptions;

#if ASPNET
#else
        internal int mouseClickColIndex;
        internal int mouseClickRowIndex;
        internal int mouseDownRowIndex;
        internal int mouseDownColIndex;
        internal int mouseMoveRowIndex;
        internal int mouseMoveColIndex;
#endif
        ////for saving HeaderID's when Table is separateed on several parts after grouping or forign key applying
        internal ArrayList HeaderList = new ArrayList();
        internal GridRangeInfo activeRange = GridRangeInfo.Empty;
        #endregion

        #region Construct, Wire and Dispose

        /// <summary>
        /// Initializes a new table object that belongs to a <see cref="TableDescriptor"/> and optionally belongs to a parent table.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor with schema information about the table.</param>
        /// <param name="relationParentTable">The parent table of this table; NULL if this table is not a child table of a relation.</param>
        public GridTable(GridTableDescriptor tableDescriptor, GridTable relationParentTable)
            : base(tableDescriptor, relationParentTable)
        {
            this.TableModel = new GridTableModel();
        }

        /// <override/>
        protected override void WireTableDescriptor()
        {
            this._tableDescriptor = (GridTableDescriptor)TableDescriptor;
            _tableDescriptor.ColumnSets.Changing += new ListPropertyChangedEventHandler(ColumnSets_Changing);
            _tableDescriptor.Columns.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(Columns_Changed);
            _tableDescriptor.ColumnSets.Changed += new ListPropertyChangedEventHandler(ColumnSets_Changed);
            _tableDescriptor.SummaryRows.Changed += new Syncfusion.Collections.ListPropertyChangedEventHandler(SummaryRows_Changed);
            _tableDescriptor.TopLevelGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
            _tableDescriptor.ChildGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changed);
            _tableDescriptor.TableOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
#if ASPNET
                                                _tableDescriptor.GridRelations.Inner.Changing += new ListPropertyChangedEventHandler(Relations_Changing);
#else
            _tableDescriptor.Relations.Inner.Changing += new ListPropertyChangedEventHandler(Relations_Changing);
#endif
            _tableDescriptor.VisibleColumns.TotalWidthRequest += new CancelEventHandler(VisibleColumns_TotalWidthRequest);

            _tableDescriptor.AllowEditChanged += new EventHandler(tableDescriptor_AllowEditChanged);
            if (_tableDescriptor.Engine != null)
            {
                //// _tableDescriptor.Engine.SourceListChanged += new EventHandler(Engine_SourceListChanged); handled in base class
                _tableDescriptor.Engine.DataSourceChanged += new EventHandler(Engine_DataSourceChanged);
                _tableDescriptor.Engine.DataMemberChanged += new EventHandler(Engine_DataMemberChanged);
                _tableDescriptor.Engine.TopLevelGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
                _tableDescriptor.Engine.ChildGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changed);
                _tableDescriptor.Engine.NestedTableGroupOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
                _tableDescriptor.Engine.TableOptions.Changed += new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                ////_tableDescriptor.Engine.PropertyChanged += new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
            }

            base.WireTableDescriptor();
        }

        /// <override/>
        protected override void UnwireTableDescriptor()
        {
            if (_tableDescriptor == null)
            {
                return;
            }

            _tableDescriptor.ColumnSets.Changing -= new ListPropertyChangedEventHandler(ColumnSets_Changing);
            _tableDescriptor.Columns.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(Columns_Changed);
            _tableDescriptor.ColumnSets.Changed -= new ListPropertyChangedEventHandler(ColumnSets_Changed);
            _tableDescriptor.SummaryRows.Changed -= new Syncfusion.Collections.ListPropertyChangedEventHandler(SummaryRows_Changed);
            _tableDescriptor.TopLevelGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
            _tableDescriptor.ChildGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changed);
            _tableDescriptor.TableOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
            _tableDescriptor.VisibleColumns.TotalWidthRequest -= new CancelEventHandler(VisibleColumns_TotalWidthRequest);
#if ASPNET
                                                _tableDescriptor.GridRelations.Inner.Changing -= new ListPropertyChangedEventHandler(Relations_Changing);
#else
            _tableDescriptor.Relations.Inner.Changing -= new ListPropertyChangedEventHandler(Relations_Changing);
#endif

            _tableDescriptor.AllowEditChanged -= new EventHandler(tableDescriptor_AllowEditChanged);
            if (_tableDescriptor.Engine != null)
            {
                //// _tableDescriptor.Engine.SourceListChanged -= new EventHandler(Engine_SourceListChanged); handled in base class
                _tableDescriptor.Engine.DataSourceChanged -= new EventHandler(Engine_DataSourceChanged);
                _tableDescriptor.Engine.DataMemberChanged -= new EventHandler(Engine_DataMemberChanged);
                _tableDescriptor.Engine.TopLevelGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TopLevelGroupOptions_Changed);
                _tableDescriptor.Engine.ChildGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(ChildGroupOptions_Changed);
                _tableDescriptor.Engine.NestedTableGroupOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(NestedTableGroupOptions_Changed);
                _tableDescriptor.Engine.TableOptions.Changed -= new Syncfusion.Styles.StyleChangedEventHandler(TableOptions_Changed);
                ////_tableDescriptor.Engine.PropertyChanged -= new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
            }

            ////_tableDescriptor = null;
            base.UnwireTableDescriptor();
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnwireTableDescriptor();
                if (tableModel != null)
                {
                    tableModel.table_Disposed();
                    tableModel = null;
                }

                CurrentRecordContextChangeTarget = null;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Strong Typed Parent and Model

        /// <summary>
        /// The GridTableModel that is used to display this table in a grid.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableModel TableModel
        {
            get
            {
                return tableModel;
            }

            set
            {
                if (tableModel != value)
                {
                    tableModel = value;
                    if (tableModel != null)
                    {
                        tableModel.Table = this;
                    }

                    TableDescriptor.VisibleColumns.columnWidthsDirty = true;
                    this.UpdateTableModelOptions();
                }
            }
        }

        /// <override/>
        /// <summary>Returns the table descriptor.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor TableDescriptor
        {
            get
            {
                if (_tableDescriptor == null)
                {
                    _tableDescriptor = (GridTableDescriptor)base.TableDescriptor;
                }

                return _tableDescriptor;
            }
        }

        /// <override/>
        /// <summary>Gets the parent table of this table or NULL if this table is not a child table of a relation.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable RelationParentTable
        {
            get
            {
                return (GridTable)base.RelationParentTable;
            }
        }

        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion

        #region ColumnDescriptor Mapping

        /// <overload>
        /// Returns the header column's GridColumnDescriptor at the specified grid row and column or NULL if the specified
        /// grid column is not a column header.
        /// </overload>
        /// <summary>
        /// Returns the header column's GridColumnDescriptor for the specified grid column in the first row of the GridColumnHeaderSection
        /// or NULL if no GridColumnHeaderSection is found or the grid column is not a column header.
        /// </summary>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The header column descriptor.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(int colIndex)
        {
            GridColumnDescriptor[,] recordRowColumns = TableDescriptor.RecordRowColumns;
            int fieldNum = TableDescriptor.ColIndexToField(colIndex);
            if (fieldNum < recordRowColumns.GetLength(1))
            {
                return recordRowColumns[0, fieldNum];
            }

            return null;
        }

        /// <summary>
        /// Returns the header columns GridColumnDescriptor at the specified grid row and column or NULL if the specified
        /// row is not a ColumnHeaderRow or the grid column is not a column header.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The header column descriptor.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(int rowIndex, int colIndex)
        {
            Element el = DisplayElements[rowIndex];
            if (el is ColumnHeaderRow || el is ColumnHeaderSection)
            {
                return GetColumnDescriptorAt(rowIndex, colIndex);
            }

            return null;
        }

        /// <exclude/>
        /// <summary>
        /// Returns the header columns GridStackedHeaderDescriptor at the specified grid row and column or NULL if the specified
        /// row is not a GridStackedHeaderRow or the grid column is not a column header.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>A <see cref="GridStackedHeaderSpan"/>.</returns>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(int rowIndex, int colIndex)
        {
            Element el = DisplayElements[rowIndex];
            if (el is GridStackedHeaderRow || el is GridStackedHeaderSection)
            {
                return GetStackedHeaderSpanAt(el, colIndex);
            }

            return null;
        }

        /// <summary>
        /// Returns the header columns GridStackedHeaderDescriptor at the specified grid row and column or NULL if the specified
        /// row is not a GridStackedHeaderRow or the grid column is not a column header.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The stacked header.</returns>
        public GridStackedHeaderDescriptor GetStackedHeaderAt(int rowIndex, int colIndex)
        {
            GridStackedHeaderSpan span = GetStackedHeaderSpanAt(rowIndex, colIndex);
            if (span != null)
            {
                return span.header;
            }

            return null;
        }

        /// <summary>
        /// Returns the header column's GridColumnDescriptor at the specified grid row and column.
        /// </summary>
        /// <param name="cell">The GridRangeInfo.</param>
        /// <returns>The header column descriptor.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(GridRangeInfo cell)
        {
            return GetHeaderColumnDescriptorAt(cell.Top, cell.Left);
        }

        /// <exclude/>
        /// <summary>
        /// Returns the header column's GridStackedHeaderSpan at the specified grid row and column.
        /// </summary>
        /// <param name="cell">The GridRangeInfo.</param>
        /// <returns>A <see cref="GridStackedHeaderSpan"/>.</returns>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(GridRangeInfo cell)
        {
            return GetStackedHeaderSpanAt(cell.Top, cell.Left);
        }

        /// <summary>
        /// Returns the record field cell's GridColumnDescriptor at the specified grid row and column.
        /// </summary>
        /// <param name="cell">Record field cell.</param>
        /// <returns>The column descriptor.</returns>
        public GridColumnDescriptor GetColumnDescriptorAt(GridRangeInfo cell)
        {
            return GetColumnDescriptorAt(cell.Top, cell.Left);
        }

        /// <summary>
        /// Returns the record field cell's GridColumnDescriptor at the specified display element and column.
        /// </summary>
        /// <param name="element">The Element.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The column descriptor.</returns>
        public GridColumnDescriptor GetColumnDescriptorAt(Element element, int colIndex)
        {
            int fieldNum = TableDescriptor.ColIndexToField(colIndex);
            if (fieldNum >= 0)
            {
                GridColumnDescriptor[,] recordRowColumns = TableDescriptor.RecordRowColumns;
                if (fieldNum < recordRowColumns.GetLength(1))
                {
                    Element el = element;
                    int row;
                    if (el.ParentTable == this)
                    {
                        if (RecordsAsDisplayElements)
                        {
                            return recordRowColumns[0, fieldNum];
                        }
                        else
                        {
                            if (el is Record || el is RowElementsSection)
                            {
                                return recordRowColumns[0, fieldNum];
                            }
                            else if (el is RecordRow)
                            {
                                Record record = el.ParentRecord;
                                RecordRow rr = (RecordRow)el;
                                row = record.RecordRows.IndexOf(rr);
                                return recordRowColumns[row, fieldNum];
                            }
                            else if (el is ColumnHeaderRow)
                            {
                                ColumnHeaderSection cs = (ColumnHeaderSection)el.ParentElement;
                                ColumnHeaderRow rr = (ColumnHeaderRow)el;
                                row = cs.ColumnHeaderRows.IndexOf(rr);
                                return recordRowColumns[row, fieldNum];
                            }
                            else if (el is FilterBarRow)
                            {
                                FilterBarSection cs = (FilterBarSection)el.ParentElement;
                                FilterBarRow rr = (FilterBarRow)el;
                                row = cs.FilterBarRows.IndexOf(rr);
                                return recordRowColumns[row, fieldNum];
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the record field cell's GridStackedHeaderDescriptor at the specified display element and column.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The stacked header.</returns>
        public GridStackedHeaderDescriptor GetStackedHeaderDescriptorAt(Element element, int colIndex)
        {
            GridStackedHeaderSpan span = GetStackedHeaderSpanAt(element, colIndex);
            if (span != null)
            {
                return span.header;
            }

            return null;
        }

        /// <exclude/>
        /// <summary>
        /// Returns the record field cell's GridColumnDescriptor at the specified display element and column.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The column descriptor at the specified element and column.</returns>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(Element element, int colIndex)
        {
            int fieldNum = TableDescriptor.ColIndexToField(colIndex);
            if (fieldNum >= 0)
            {
                GridColumnDescriptor[,] recordRowColumns = TableDescriptor.RecordRowColumns;
                if (fieldNum < recordRowColumns.GetLength(1))
                {
                    Element el = element;
                    int row;
                    if (el.ParentTable == this)
                    {
                        if (el is GridStackedHeaderSection)
                        {
                            GridStackedHeaderSpan span = this.TableDescriptor.StackedHeaderRows[0].GetStackedHeaderSpanAt(fieldNum);
                            return span;
                        }
                        else if (el is GridStackedHeaderRow)
                        {
                            GridStackedHeaderSection section = (GridStackedHeaderSection)el.ParentSection;
                            RowElement rr = (RowElement)el;
                            row = section.RowElements.IndexOf(rr);
                            GridStackedHeaderSpan span = this.TableDescriptor.StackedHeaderRows[row].GetStackedHeaderSpanAt(fieldNum);
                            return span;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the record field cell's GridColumnDescriptor at the specified grid row and column.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <returns>The column descriptor.</returns>
        public GridColumnDescriptor GetColumnDescriptorAt(int rowIndex, int colIndex)
        {
            int fieldNum = TableDescriptor.ColIndexToField(colIndex);
            if (fieldNum >= 0)
            {
                GridColumnDescriptor[,] recordRowColumns = TableDescriptor.RecordRowColumns;
                if (fieldNum < recordRowColumns.GetLength(1) && rowIndex < DisplayElements.Count)
                {
                    Element el = DisplayElements[rowIndex];
                    int row;
                    if (el.ParentTable == this)
                    {
                        if (RecordsAsDisplayElements)
                        {
                            return recordRowColumns[0, fieldNum];
                        }

                        if (el is Record || el is RowElementsSection)
                        {
                            return recordRowColumns[0, fieldNum];
                        }
                        else if (el is RecordRow)
                        {
                            Record record = el.ParentRecord;
                            RecordRow rr = (RecordRow)el;
                            row = record.RecordRows.IndexOf(rr);
                            return recordRowColumns[row, fieldNum];
                        }
                        else if (el is ColumnHeaderRow)
                        {
                            ColumnHeaderSection cs = (ColumnHeaderSection)el.ParentElement;
                            ColumnHeaderRow rr = (ColumnHeaderRow)el;
                            row = cs.ColumnHeaderRows.IndexOf(rr);
                            return recordRowColumns[row, fieldNum];
                        }
                        else if (el is FilterBarRow)
                        {
                            FilterBarSection cs = (FilterBarSection)el.ParentElement;
                            FilterBarRow rr = (FilterBarRow)el;
                            row = cs.FilterBarRows.IndexOf(rr);
                            return recordRowColumns[row, fieldNum];
                        }
                    }
                }
            }

            return null;
        }
        #endregion

        #region Record Mapping
        internal int GetCurrentRecordStartRowIndex()
        {
            return this.DisplayElements.IndexOf((Element)this.CurrentElement);
        }

        internal GridRangeInfo GetCurrentRecordRangeInfo()
        {
            return GetRecordRangeInfo(this.CurrentElement);
        }

        internal GridRangeInfo GetRecordRangeInfo(Element r)
        {
            if (r is Record)
            {
                Record rc = (Record)r;
                return GetRecordRangeInfo(rc);
            }
            else if (r is NestedTable)
            {
                NestedTable nt = (NestedTable)r;
                int rowIndex = this.NestedDisplayElements.IndexOf(nt);
                int count = nt.GetVisibleCount();
                GridRangeInfo rg = GridRangeInfo.Rows(rowIndex, rowIndex + count - 1);
                return rg;
            }

            return GridRangeInfo.Empty;
        }

        internal GridRangeInfo GetRecordRangeInfo(Record r)
        {
            if (r == null)
            {
                return GridRangeInfo.Empty;
            }

            int rowIndex = this.NestedDisplayElements.IndexOf(r);
            ////int rowIndex2 = this.NestedDisplayElements.IndexOf(r);
            ////TraceUtil.TraceCurrentMethodInfo(r, rowIndex, rowIndex2);
            ////rowIndex = this.DisplayElements.IndexOf(r);
            ////rowIndex2 = this.NestedDisplayElements.IndexOf(r);
            return GridRangeInfo.Rows(rowIndex, rowIndex + r.GetRecordRowsVisibleCount() - 1);
        }

        /// <summary>
        /// Returns the <see cref="GridRangeInfo"/> with the range of cells that an element spans.
        /// </summary>
        /// <param name="e">The element.</param>
        /// <returns>The <see cref="GridRangeInfo"/> with the range of cells that an element spans.</returns>
        public GridRangeInfo GetElementRangeInfo(Element e)
        {
            if (e == null)
            {
                return GridRangeInfo.Empty;
            }

            int rowIndex = this.NestedDisplayElements.IndexOf(e);
            return GridRangeInfo.Rows(rowIndex, rowIndex + e.GetVisibleCount() - 1);
        }
        #endregion

        #region Table Overrides

        /// <override/>
        protected override void OnSourceListItemChanged(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            int v = Engine.Version;

            base.OnSourceListItemChanged(sender, e, te);

            // Force Record.GetValue to be called again when a record was changed
            // when Engine.AllowCacheStyles was specified.
            if (!TableDirty
                && !CountersDirty
                && v == Engine.Version
                && Engine.AllowCacheStyles
                && !this.RecordsAsDisplayElements)
            {
                GridRecord r = (GridRecord)UnsortedRecords[e.NewIndex];
                foreach (GridRecordRow row in r.RecordRows)
                {
                    IGridTableCellStyleCache ics = row as IGridTableCellStyleCache;
                    if (ics != null)
                    {
                        ics.ResetStyles();
                    }
                }
            }
        }

        /// <override/>
        protected override void OnSourceListChanged(TableEventArgs e)
        {
            if (ShouldSerializeAppearance())
            {
                GridTableCellAppearance appearance = Appearance;
                if (this.SourceListAllowEdit && TableDescriptor.AllowEdit)
                {
                    appearance.AnyRecordFieldCell.ResetReadOnly();
                }
                else
                {
                    appearance.AnyRecordFieldCell.ReadOnly = true;
                }
            }

            base.OnSourceListChanged(e);
            ResetRepaintElementsInQueue();
        }

#if ASPNET
#else
        /// <override/>
        protected override void OnSourceListReset(object sender, ListChangedEventArgs e, TableListChangedEventArgs te)
        {
            base.OnSourceListReset(sender, e, te);

            // Only invalidate if reset had any effect and IgnoreReset optimization was not specified.
            if (TableDirty || CountersDirty)
            {
                Engine.TableControl.MarkResync(false);
            }
        }
#endif

        /// <override/>
        /// <summary>Gets the kind of display element.</summary>
        public override DisplayElementKind Kind
        {
            get
            {
                return DisplayElementKind.Table;
            }
        }

#if ASPNET
#else
        /// <summary>
        /// Determines if the current thread is the same UI thread as the parent control or if
        /// the current method call should be marshaled.
        /// </summary>
        /// <returns>
        /// A control that can be used to marshal the current method by calling its Invoke method.
        /// </returns>
        /// <override/>
        protected override Control GetInvokeRequiredControl()
        {
            if (this._tableDescriptor != null && this._tableDescriptor.Engine != null)
            {
                Control c = this._tableDescriptor.Engine.ParentControl;
                if (c != null && c.IsHandleCreated && c.InvokeRequired)
                {
                    return c;
                }
            }

            return null;
        }
#endif
        /// <override/>
        protected override void OnCategorizedRecords(TableEventArgs e)
        {
            if (this.TableModel != null)
            {
                TableModel.RegisterNestedTableCellModels();
            }

            columnsMaxLengthFirstNRecordsTable = null;

            base.OnCategorizedRecords(e);
        }

        #region ColumnMaxLength
        Hashtable columnsMaxLengthFirstNRecordsTable;

        /// <exclude/>
        /// <summary>Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore, EditorBrowsable(EditorBrowsableState.Never)]
        public Hashtable ColumnsMaxLengthFirstNRecordsTable
        {
            get
            {
                return columnsMaxLengthFirstNRecordsTable;
            }

            set
            {
                columnsMaxLengthFirstNRecordsTable = value;
            }
        }

        /// <summary>
        /// Returns the maximum length for the given field name in the first n records
        /// of the datasource. n is defined by TableOptions.ColumnsMaxLengthFirstNRecords property.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>Maximum length of the field.</returns>
        public int GetColumnsNRowsMaxLength(string fieldName)
        {
            if ((this.TableOptions.ColumnsMaxLengthStrategy & GridColumnsMaxLengthStrategy.FirstNRecords) != 0)
            {
                if (columnsMaxLengthFirstNRecordsTable == null)
                {
                    InitializeColumnsMaxLengthFirstNRecords();
                }

                if (columnsMaxLengthFirstNRecordsTable != null)
                {
                    object obj = columnsMaxLengthFirstNRecordsTable[fieldName];
                    if (obj is int)
                    {
                        return (int)obj;
                    }
                }
            }

            return -1;
        }

        /// <exclude/>
        void InitializeColumnsMaxLengthFirstNRecords()
        {
            if ((this.TableOptions.ColumnsMaxLengthStrategy & GridColumnsMaxLengthStrategy.FirstNRecords) != 0)
            {
                columnsMaxLengthFirstNRecordsTable = new Hashtable();
                OnInitializeColumnsMaxLengthFirstNRecords(columnsMaxLengthFirstNRecordsTable);
            }
        }

        /// <summary>
        /// This method is called internally when GridColumnsMaxLengthStrategy.FirstNRecords has been
        /// specified and the column lengths for each field need to be initialized.
        /// </summary>
        /// <param name="columnsMaxLengthFirstNRecordsTable">The hashtable uses the FieldDescriptor.Name as key and 
        /// the column length in characters as value.</param>
        protected virtual void OnInitializeColumnsMaxLengthFirstNRecords(Hashtable columnsMaxLengthFirstNRecordsTable)
        {
            if ((this.TableOptions.ColumnsMaxLengthStrategy & GridColumnsMaxLengthStrategy.FirstNRecords) != 0)
            {
                foreach (GridColumnDescriptor column in TableDescriptor.Columns)
                {
                    if (column.MaxLength == -1
                                    && column.FieldDescriptor != null
                                    && (!column.FieldDescriptor.IsRelatedField() || !column.AllowDropDownCell))
                    {
                        string name = column.FieldDescriptor.Name;
                        columnsMaxLengthFirstNRecordsTable[name] = -1; //// column.HeaderText.Length + 3;
                    }
                }

                IList recordList = GetMaxLengthSampleRecords();

                int nRecords = 0;
                if (recordList != null)
                {
                    nRecords = Math.Min(recordList.Count, TableOptions.ColumnsMaxLengthFirstNRecords);
                }

                if (nRecords == 0)
                {
                    return;
                }

                foundSampleRecordsForColumnWidths = true;

                ChildTable savedFilteredChildTable = FilteredChildTable;
                for (int recordIndex = 0; recordIndex < nRecords; recordIndex++)
                {
                    Record record = (Record)recordList[recordIndex];
                    foreach (GridColumnDescriptor column in TableDescriptor.Columns)
                    {
                        if (column.MaxLength == -1
                                        && column.FieldDescriptor != null
                                        && (!column.FieldDescriptor.IsRelatedField() || !column.AllowDropDownCell))
                        {
                            string name = column.FieldDescriptor.Name;
                            int len = this.GetColumnsNRowsMaxLength(name);
                            record.ParentTable.FilteredChildTable = record.ParentChildTable;
                            GridStyleInfo style = GetRecordColumnStyle(record, column);
                            string text = style.FormattedText.TrimEnd();
                            if (text.Length > len)
                            {
                                columnsMaxLengthFirstNRecordsTable[name] = text.Length;
                            }
                        }
                    }
                }

                FilteredChildTable = savedFilteredChildTable;

                //// TODO: Also check summary columns
            }
        }

        /// <summary>
        /// Returns list of sample records.
        /// </summary>
        /// <returns>list</returns>
        protected virtual IList GetMaxLengthSampleRecords()
        {
            IList recordList = Records;

            if (PassThroughGroupingResult != null)
            {
                IEnumerable sampleItems = PassThroughGroupingResult.GetSampleItems();

                if (sampleItems == null)
                {
                    recordList = UnsortedRecords;

                    //// Load first n records. This will call Table.PopulatePassThroughGroup
                    //// and add at given number of record to UnsortedRecords collection
                    PopulatePassThroughGroupsUntilFoundNRecords(TopLevelGroup, TableOptions.ColumnsMaxLengthFirstNRecords);
                }
                else
                {
                    ArrayList al = new ArrayList();
                    int n = 0;
                    foreach (object item in sampleItems)
                    {
                        Record r = new Record(this);
                        r.SetSourceIndex(n++, SourceListVersion, item);
                        r.ParentElement = TopLevelGroup;
                        al.Add(r);
                    }

                    recordList = al;
                }
            }
            else
            {
                if (UnsortedRecords.Count > Records.Count && TableDescriptor.RecordFilters.Count == 0)
                {
                    //// valid reason is if filter is set ...
                    System.Diagnostics.Trace.WriteLine("UnsortedRecords.Count is greater than Records.Count. I delay evaluation of ColumnMaxLength for first n records");
                    return null;
                }
            }

            return recordList;
        }

        bool foundSampleRecordsForColumnWidths = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="childTable"></param>
        protected override void OnPopulatedChildTable(ChildTable childTable)
        {
            if (!foundSampleRecordsForColumnWidths)
            {
                columnsMaxLengthFirstNRecordsTable = null;
                Engine.AppearanceVersion++;
                TableModel.UpdateColumnWidths(true);
            }
        }

        /// <summary>
        /// Populates pass through groups for specified number of records.
        /// </summary>
        /// <param name="g">The parent group.</param>
        /// <param name="count">The record count.</param>
        public void PopulatePassThroughGroupsUntilFoundNRecords(Group g, int count)
        {
            DetailsSection details = g.Details;
            if (details.HasRecords)
            {
                return;
            }

            GroupsDetails details2 = (GroupsDetails)details;
            for (int n = 0; n < details2.Groups.Count; n++)
            {
                Group g2 = details2.Groups[n];

                PopulatePassThroughGroupIfEmpty(g2);

                if (UnsortedRecords.Count > count)
                {
                    return;
                }

                PopulatePassThroughGroupsUntilFoundNRecords(g2, count);
            }
        }

        /// <summary>
        /// Returns the preferred column width in pixels for the specified column taking the
        /// length of the column header text and the value returned by GetColumnMaxLength
        /// into consideration.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="column">The Column.</param>
        /// <returns>Preferred column width.</returns>
        public int GetPreferredColumnWidth(Graphics g, GridColumnDescriptor column)
        {
            return GetPreferredColumnWidth(g, column, 350);
        }

        /// <summary>
        /// Returns the preferred column width in pixels for the specified column taking the
        /// length of the column header text and the value returned by GetColumnMaxLength
        /// into consideration.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/></param>
        /// <param name="column">The <see cref="GridColumnDescriptor"/></param>
        /// <param name="maxWidth">Width of the max.</param>
        /// <returns>Preferred column width.</returns>
        public virtual int GetPreferredColumnWidth(Graphics g, GridColumnDescriptor column, int maxWidth)
        {
            GridStyleInfo style = column.Appearance.AnyRecordFieldCell;
            GridCellModelBase cellModel = TableModel.CellModels[style.CellType];
            Font font = style.GdipFont;

            int charWidth = (int)g.MeasureString("Abc", font).Width / 3;
#if !ASPNET
            column.AvgCharWidth = charWidth; // let's us reuse thw width later in other contexts.
#endif

            int columnMaxLength = GetColumnMaxLength(column);

            int maxColumnWidth = 0;
            if (columnMaxLength == -1)
            {
                maxColumnWidth = TableOptions.DefaultColumnWidth;
            }
            else
            {
                maxColumnWidth = (int)columnMaxLength * charWidth;

                //// check for buttons in cell model, any text / border margins .
                if (cellModel != null)
                {
                    maxColumnWidth += cellModel.ButtonBarSize.Width + style.TextMargins.ToMargins().Width + style.BorderMargins.ToMargins().Width;
                }
            }

            //// check header text
            style = column.Appearance.ColumnHeaderCell;
            int headerWidth = (int)g.MeasureString(column.HeaderText, style.GdipFont).Width + style.TextMargins.ToMargins().Width + style.BorderMargins.ToMargins().Width + 16;

            maxColumnWidth = Math.Max(headerWidth, maxColumnWidth);

            if (maxColumnWidth > maxWidth)
            {
                ////maxHeight = maxColumnWidth / 350;
                maxColumnWidth = maxWidth;
            }

            return maxColumnWidth;
        }

        /// <summary>
        /// Returns the calculated maximum length in characters for the specified column.
        /// The length is taken from a previous call to OnInitializeColumnsMaxLengthFirstNRecords
        /// or from a MaxLengthSummary that automatically tracks changes to fields in the column.
        /// </summary>
        /// <param name="column">The Column.</param>
        /// <returns>Maximum length of the column.</returns>
        public int GetColumnMaxLength(GridColumnDescriptor column)
        {
            int columnMaxLength = column.MaxLength;
            if (columnMaxLength != -1)
            {
                return columnMaxLength;
            }

            FieldDescriptor fd = column.FieldDescriptor;
            if (fd != null)
            {
                //// foreign key fields.
                if (fd.IsRelatedField() && column.AllowDropDownCell)
                {
                    GridTable rtable = (GridTable)RelatedTables[fd.GetRelation().Name];
                    if (rtable != null)
                    {
                        //// case for foreign key fields. Column width is stored in related table.
                        FieldDescriptor rfield = fd.GetRelatedDescriptor();

                        //// nested foreign key fields - get innermost field.
                        while (rfield.IsRelatedField())
                        {
                            rtable = (GridTable)rtable.RelatedTables[rfield.GetRelation().Name];
                            rfield = rfield.GetRelatedDescriptor();
                        }

                        columnMaxLength = rtable.GetColumnMaxLength(column, rfield);
                    }
                }

                //// fields in this table.

                if (columnMaxLength == -1)
                {
                    PropertyDescriptor pd = fd.GetPropertyDescriptor();
                    if (pd != null && pd.Converter != null && pd.Converter.GetStandardValuesSupported())
                    {
                        GridStyleInfo style = column.Appearance.AnyRecordFieldCell;

                        //// check for buttons in cell model, any text / border margins .
                        GridPropertyStandardValuesList sl = TableModel.GetCachedStandardValues(pd.Converter, pd.PropertyType);
                        if (sl != null)
                        {
                            columnMaxLength = sl.GetMaxLength(style.Format, style.GetCulture(true));
                        }
                    }
                    else
                    {
                        columnMaxLength = GetColumnMaxLength(column, fd);
                    }
                }
            }

            return columnMaxLength;
        }

        /// <summary>
        /// Returns the calculated maximum length in characters for the specified field.
        /// The length is taken from a previous call to OnInitializeColumnsMaxLengthFirstNRecords
        /// or from a MaxLengthSummary that automatically tracks changes to fields in the column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="rfield">The rfield.</param>
        /// <returns>Maximum length of the column.</returns>
        public virtual int GetColumnMaxLength(GridColumnDescriptor column, FieldDescriptor rfield)
        {
            //// check first nrows, returns -1 if option is not enabled.
            int columnMaxLength = GetColumnsNRowsMaxLength(rfield.Name);
            if (columnMaxLength != -1)
            {
                return columnMaxLength;  //// column.HeaderText is not checked in GridTable.InitializeColumnsMaxLengthFirstNRecords
            }

            //// AutoSizeMaxLength summary
            int index = TableDescriptor.Summaries.IndexOf(rfield.Name + "AutoSizeMaxLength");
            if (index != -1)
            {
                Syncfusion.Collections.BinaryTree.ITreeTableSummary[] rsummaries = GetSummaries(this);
                GridMaxLengthSummary summary = rsummaries[index] as GridMaxLengthSummary;
                if (summary != null)
                {
                    return summary.MaxLength; //// Math.Max(summary.MaxLength, column.HeaderText.Length + 3);
                }
            }

            return -1;
        }
        #endregion

        /// <override/>
        protected override void OnRemovingRelatedTable(TableEventArgs e)
        {
            GridTable relatedTable = (GridTable)e.Table;
            string cellType = "RT" + relatedTable.TableDescriptor.Name;
            if (this.TableModel != null)
            {
                if (TableModel.CellModels.ContainsKey(cellType))
                {
                    GridCellModelBase cellModel = TableModel.CellModels[cellType];
                    TableModel.CellModels.Remove(cellType);
                    cellModel.Dispose();
                }
            }

            base.OnRemovingRelatedTable(e);
        }

        /// <override/>
        protected override void OnAddedRelatedTable(TableEventArgs e)
        {
            if (this.TableModel != null)
            {
                TableModel.RegisterNestedTableCellModels();
            }

            base.OnAddedRelatedTable(e);
        }

        bool inCurrentRecordContextChange = false;

        /// <override/>
        protected override void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            ////SS                                                TraceUtil.TraceCurrentMethodInfo(TableDescriptor.Name, "***BEGIN", e);

            base.OnCurrentRecordContextChange(e);

            if (inCurrentRecordContextChange || !e.Success || e.Cancel)
            {
                return;
            }

            inCurrentRecordContextChange = true;

            try
            {
                switch (e.Action)
                {
                    case CurrentRecordAction.BeginEditCalled:
                        break;

                    case CurrentRecordAction.BeginEditComplete:
                        ////RepaintElement(e.Record);
                        break;

                    case CurrentRecordAction.CancelEditCalled:
                        break;

                    case CurrentRecordAction.CancelEditComplete:
                        ////RepaintElement(e.Record);
                        break;

                    case CurrentRecordAction.EndEditCalled:
                        ////if (!inControlEndEdit)
                        ////                e.Cancel = !ControlEndEdit();
                        break;

                    case CurrentRecordAction.EndEditComplete:
                        ////RepaintElement(e.Record);
                        break;

                    case CurrentRecordAction.NavigateCalled:
                        break;

                    case CurrentRecordAction.NavigateComplete:
                        break;

                    case CurrentRecordAction.LeaveRecordCalled:
                        ////                                                                                                if (e.Record is Record)
                        ////                                                                                                {
                        ////                                                                                                                if (TableOptions.ListBoxSelectionMode == SelectionMode.One)
                        ////                                                                                                                                SelectedRecords.Remove((Record) e.Record);
                        ////                                                                                                }
                        break;

                    case CurrentRecordAction.LeaveRecordComplete:
                        ////this.RepaintElement(e.Record);
                        break;

                    case CurrentRecordAction.EnterRecordCalled:
                        break;

                    case CurrentRecordAction.EnterRecordComplete:
                        if (!ignoreEnterRecordComplete && e.Record is Record && TableOptions.AllowSelection == GridSelectionFlags.None)
                        {
                            Record r = (Record)e.Record;
                            if (TableOptions.ListBoxSelectionMode == SelectionMode.One)
                            {
                                ////|| TableOptions.ListBoxSelectionMode == SelectionMode.MultiExtended)
                                if (!SelectedRecords.Contains(r))
                                {
                                    SelectedRecords.Clear();
                                    SelectedRecords.Add(r);
                                }
                            }
                        }
                        ////RepaintElement(e.Record);
                        break;
                }

                if (this.CurrentRecordContextChangeTarget != null)
                {
                    this.CurrentRecordContextChangeTarget(this, e);
                }
            }
            finally
            {
                inCurrentRecordContextChange = false;
            }

            ////SSTraceUtil.TraceCurrentMethodInfo(TableDescriptor.Name, "Done", e);
            ////                                                System.Threading.Thread.Sleep(300);
            ////
            ////                                                if (Control.ModifierKeys != Keys.None)
            ////                                                                System.Threading.Thread.Sleep(1000);
            ////
            ////                                                if ((Control.ModifierKeys & Keys.Control) != 0)
            ////                                                {
            ////                                                                System.Threading.Thread.Sleep(3000);
            ////                                                                if ((Control.ModifierKeys & Keys.Shift) != 0)
            ////                                                                                System.Diagnostics.Debugger.Break();
            ////                                                }
        }

        internal bool ignoreEnterRecordComplete = false;

        /// <override/>
        protected override void OnGroupSummaryInvalidated(GroupEventArgs e)
        {
#if !ASPNET
            if (!Engine.ParentControl.UseCustomUpdateOnListChanged && Engine.UseOldListChangedHandler)
#endif
            {
                IGridGroupOptionsSource sto = e.Group as IGridGroupOptionsSource;

                if (sto == null || !sto.HasGroupOptions)
                {
                    sto = e.Group.ParentTableDescriptor as IGridGroupOptionsSource;
                }

                if (sto == null || sto.GroupOptions.RepaintCaptionWhenItemsChanged)
                {
                    RepaintElement(e.Group.Caption);
                }

                if (e.Group.Summary != null)
                {
                    RepaintElement((Element)e.Group.Summary);
                }
            }

            base.OnGroupSummaryInvalidated(e);
        }

        #endregion

        #region TableDescriptor Changed Events
        int oldRecordRowCount = -1;
        private void ColumnSets_Changing(object sender, ListPropertyChangedEventArgs e)
        {
            oldRecordRowCount = TableDescriptor.RowsPerRecord;
        }

        private void Columns_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            if (e.Property == "Appearance" || e.Property == "Width" || e.Property == "ReadOnly" || e.Property == "HeaderText" || e.Action == ListPropertyChangedType.Remove || e.Action == ListPropertyChangedType.Move)
            {
                TableDescriptor.summaries_savedColumnsVersion = TableDescriptor.Columns.Version; // do not update column descriptors - otherwise max length won't valid.
            }
            else if (e.Property == "AllowFilter")
            {
                if (this.TableDescriptor.HasSummaryFilterBarChoices)
                {
                    this.SummariesDirty = true;
                }
            }
            else
            {
                this.CountersDirty = true;
                this.SummariesDirty = true;
            }
        }

        private void ColumnSets_Changed(object sender, ListPropertyChangedEventArgs e)
        {
            if (oldRecordRowCount != TableDescriptor.RowsPerRecord)
            {
                this.CountersDirty = true;
                this.SummariesDirty = true;
            }
        }

        private void TopLevelGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            CountersDirty = true;
            this.SummariesDirty = true;
        }

        private void ChildGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            CountersDirty = true;
            this.SummariesDirty = true;
        }

        private void NestedTableGroupOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            CountersDirty = true;
            this.SummariesDirty = true;
        }

        private void TableOptions_Changed(object sender, Syncfusion.Styles.StyleChangedEventArgs e)
        {
            CountersDirty = true;
            this.SummariesDirty = true;
            UpdateTableModelOptions();
        }
        
        void UpdateTableModelOptions()
        {
            if (this.TableModel != null)
            {
                this.TableModel.BeginUpdate();
                this.TableModel.Options.AllowSelection = this.TableDescriptor.TableOptions.AllowSelection;
#if ASPNET
                                                                this.TableModel.Options.ListBoxSelectionMode = (System.Windows.Forms.SelectionMode)Enum.Parse(
                                                                                typeof(System.Windows.Forms.SelectionMode),
                                                                                this.TableDescriptor.TableOptions.ListBoxSelectionMode.ToString());
#else
                this.TableModel.Options.ListBoxSelectionMode = this.TableDescriptor.TableOptions.ListBoxSelectionMode;

                this.tableModel.Options.GridVisualStyles = this.TableDescriptor.TableOptions.GridVisualStyles;
                this.tableModel.Options.GridVisualStylesDrawing = this.TableDescriptor.TableOptions.GridVisualStylesDrawing;
#endif
                this.TableModel.EndUpdate(false);
            }
        }

        void RemoveNestedTableCellModel(RelationDescriptor rd)
        {
            if (rd.RelationKind == RelationKind.ForeignKeyReference
                            || rd.RelationKind == RelationKind.ListItemReference
                            || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
            {
                //// ForeignListItems
                ////TableModel.CellModels.Remove("ForeignKeyCell");
            }
            else
            {
                TableModel.RemoveNestedTableCellModel(rd.Name);
            }
        }

        private void Relations_Changing(object sender, ListPropertyChangedEventArgs le)
        {
            if (le != null && this.TableModel != null)
            {
                RelationDescriptor rd = le.Item as RelationDescriptor;
                switch (le.Action)
                {
                    case ListPropertyChangedType.ItemPropertyChanged:
                        {
                            if (le.Property == "RelationKind" || le.Property == "Name" || le.Property == "ChildTableName" || le.Property == "MappingName")
                            {
                                RemoveNestedTableCellModel(rd);
                            }

                            break;
                        }

                    case ListPropertyChangedType.ItemChanged:
                    case ListPropertyChangedType.Remove:
                        {
                            if (le.Property == "RelationKind" || le.Property == "Name" || le.Property == "ChildTableName" || le.Property == "MappingName")
                            {
                                RemoveNestedTableCellModel(rd);
                            }

                            break;
                        }

                    case ListPropertyChangedType.Refresh:
                        {
                            foreach (RelationDescriptor relation in this.TableDescriptor.Relations)
                            {
                                RemoveNestedTableCellModel(relation);
                            }

                            break;
                        }
                }
            }
        }

#if ASPNET
#else
        /// <override/>
        protected override void Engine_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            if (TableModel != null)
            {
                TableModel.fixedBorder = null;
                TableModel.gridBorder = null;

                if (e.PropertyName == "TableOptions")
                {
                    this.UpdateTableModelOptions();
                }
            }

            if (e.PropertyName == "TableDescriptor")
            {
                TableDescriptor tableDescriptor = Engine.TableDescriptor;
                e = (DescriptorPropertyChangedEventArgs)e.Inner;

                if (e.PropertyName == "Relations")
                {
                    e = e.GetNestedChildTableDescriptorEvent(ref tableDescriptor);
                }

                //// In case I need that ...
                ////                                                                object app = tableDescriptor;
                ////                                                                GridTableCellStyleInfoChangedEventArgs tableCellStyleInfoChangedEventArgs = GridEngine.GetNestedAppearanceEvent(e, ref app);
                ////                                                                IGridTableCellAppearanceSource appearanceHolder = app as IGridTableCellAppearanceSource;

                switch (e.PropertyName)
                {
                    case "Columns":
                    case "VisibleColumns":
                    case "PrimaryKeyColumns":
                    case "ConditionalFormats":
                    case "RelationChildColumns":
                    case "ColumnSets":
                    ////case "Summaries":
                    case "SummaryRows":
                    case "TopLevelGroupOptions": //// see TopLevelGroupOptions_Changed etc. above
                        return;

                    case "Appearance":
                        {
                            if (tableDescriptor != this.TableDescriptor
                                            && (!this.TableDescriptor.IsChildOf(tableDescriptor) || !this.TableDescriptor.InheritAppearanceFomParent))
                            {
                                return;
                            }

                            break;
                        }

                    case "ChildGroupOptions":
                        {
                            if (tableDescriptor != this.TableDescriptor && !this.TableDescriptor.IsChildOf(tableDescriptor))
                            {
                                return;
                            }

                            break;
                        }

                    case "SortedColumns":
                        {
                            if (tableDescriptor != this.TableDescriptor)
                            {
                                return;
                            }

                            if (e.Inner is ListPropertyChangedEventArgs)
                            {
                                ListPropertyChangedEventArgs le = (ListPropertyChangedEventArgs)e.Inner;
                                if (/*le.Property == "SortDirection" || */
                                    le.Action == ListPropertyChangedType.Refresh
                                    || TableDescriptor.GroupedColumns.Count > 0
                                    || (TableDescriptor.ParentRelation != null && TableDescriptor.ParentRelation.RelationKind == RelationKind.UniformChildList)
                                    || TableDescriptor.RelationChildColumns.Count > 0)
                                {
                                    //// || this.UnsortedRecords.Count <= 1
                                    return;
                                }

                                TableDirty = true;
                            }

                            if (Engine != null && Engine.TableControl != null)
                            {
                                Engine.TableControl.synchronizeGridShouldScrollCurrentCell = true;
                            }

                            return;
                        }

                    case "RecordFilters":
                        {
                            if (tableDescriptor != this.TableDescriptor
                                && !tableDescriptor.IsChildOf(this.TableDescriptor))
                            {
                                return;
                            }

                            CountersDirty = true;
                            this.SummariesDirty = true;
                            if (Engine != null && Engine.TableControl != null)
                            {
                                Engine.TableControl.synchronizeGridShouldScrollCurrentCell = true;
                            }

                            return;
                        }

                    case "GroupedColumns":
                        {
                            if (tableDescriptor != this.TableDescriptor
                                && !tableDescriptor.IsChildOf(this.TableDescriptor))
                            {
                                return;
                            }

                            if (e.Inner is ListPropertyChangedEventArgs)
                            {
                                if (tableDescriptor != this.TableDescriptor)
                                {
                                    return;
                                }

                                ListPropertyChangedEventArgs le = (ListPropertyChangedEventArgs)e.Inner;
                                if (le.Property == "SortDirection")
                                {
                                    return;
                                }
                            }

                            if (Engine != null && Engine.TableControl != null)
                            {
                                Engine.TableControl.synchronizeGridShouldScrollCurrentCell = true;
                            }

                            break;
                        }
                }
            }

            CountersDirty = true;
            this.SummariesDirty = true;

            base.Engine_PropertyChanged(sender, e);
        }
#endif
        private void SummaryRows_Changed(object sender, Syncfusion.Collections.ListPropertyChangedEventArgs e)
        {
            switch (e.Action)
            {
                case ListPropertyChangedType.Add:
                case ListPropertyChangedType.Remove:
                case ListPropertyChangedType.Refresh:
                    CountersDirty = true;
                    SummariesDirty = true;
                    break;
            }

            if (e.Property != "Appearance")
            {
                this.SummariesDirty = true;
            }
        }

        private void tableDescriptor_AllowEditChanged(object sender, EventArgs e)
        {
            if (CurrentRecordManager.IsEditing)
            {
                CurrentRecordManager.CancelEdit();
            }

            if (ShouldSerializeAppearance())
            {
                GridTableCellAppearance appearance = Appearance;
                if (this.SourceListAllowEdit && TableDescriptor.AllowEdit)
                {
                    appearance.AnyRecordFieldCell.ResetReadOnly();
                }
                else
                {
                    appearance.AnyRecordFieldCell.ReadOnly = true;
                }
            }
        }

        #endregion

        #region Engine Changed Events
        private void Engine_DataSourceChanged(object sender, EventArgs e)
        {
            TableDirty = true;
        }

        private void Engine_DataMemberChanged(object sender, EventArgs e)
        {
            TableDirty = true;
        }

        #endregion

        #region Covered Range
        /// <summary>
        /// Determines if the cell belongs to a covered range and returns the covered range of the cell or
        /// the cell itself as <see cref="GridRangeInfo"/> if it is not a covered range.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell.</param>
        /// <param name="colIndex">The column index of the cell.</param>
        /// <returns>Returns the covered range the cell belongs; if cell is not part of a covered range
        /// the cell itself is returned as <see cref="GridRangeInfo"/>.</returns>
        public GridRangeInfo GetCoveredRange(int rowIndex, int colIndex)
        {
            GridTableQueryCoveredRangeEventArgs e = new GridTableQueryCoveredRangeEventArgs(this, rowIndex, colIndex);
#if ASPNET
                                                this.RaiseQueryCoveredRange(e);
#else
            OnQueryCoveredRange(e);
#endif
            // ASPNET related: Sometimes OnQueryCoveredRange doesn't handle the event and e.Range is empty.
            if (e.Handled)
            {
                return e.Range;
            }
            else
            {
                return GridRangeInfo.Cell(rowIndex, colIndex);
            }
        }

        /// <summary>
        /// Occurs to determine if the cell belongs to a covered range and returns the covered range of the cell or
        /// the cell itself as <see cref="GridRangeInfo"/> if it is not a covered range.
        /// </summary>
        public event GridTableQueryCoveredRangeEventHandler QueryCoveredRange;

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        internal void RaiseQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            GridTableQueryCoveredRangeEventArgs inner = new GridTableQueryCoveredRangeEventArgs(this, e.RowIndex, e.ColIndex, e.Range);
            RaiseQueryCoveredRange(inner);
            e.Handled = inner.Handled;
            e.Range = inner.Range;
        }

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        internal void RaiseQueryCoveredRange(GridTableQueryCoveredRangeEventArgs e)
        {
            ////using (MeasureTime.Measure("RaiseQueryCoveredRange"))
            {
                if (Engine != null)
                {
                    Engine.RaiseQueryCoveredRange(e);
                }

                if (!e.Handled)
                {
                    OnQueryCoveredRange(e);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="QueryCoveredRange"/> event and calculates the covered range for a cell. Examples for covered ranges are the caption bar of a group,
        /// a summary row with GridSummaryStyle.FillRow, or the first record field cell of an add new record (which spans
        /// over the record field and plus minus button of groups below).
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCoveredRangeEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCoveredRange(GridTableQueryCoveredRangeEventArgs e)
        {
            if (QueryCoveredRange != null)
            {
                QueryCoveredRange(this, e);
            }

            if (e.Handled)
            {
                return;
            }

            GridTable thisTable = this;
            if (e.RowIndex < thisTable.DisplayElements.Count)
            {
                Element el = thisTable.DisplayElements[e.RowIndex];

                switch (el.Kind)
                {
                    case DisplayElementKind.NestedTable:
                        {
                            NestedTable nestedTable = (NestedTable)el;
                            ChildTable childTable = nestedTable.ChildTable;
                            if (childTable != null)
                            {
                                int tablePos = thisTable.DisplayElements.IndexOf(nestedTable);
                                int firstRow = tablePos;
                                int lastRow = firstRow + nestedTable.GetVisibleCount() - 1;

                                if (e.RowIndex >= firstRow)
                                {
                                    int columnIndentCount = GetColumnIndentCount();
                                    if (e.ColIndex >= columnIndentCount && e.ColIndex <= Math.Max(100, TableDescriptor.GetColCount()))
                                    {
                                        //// TODO: the 100 just makes sure it is large enough. But a better count is necessary ...
                                        e.Range = GridRangeInfo.Cells(firstRow, GetColumnIndentCount(), lastRow, Math.Max(100, TableDescriptor.GetColCount()));
                                        e.Handled = true;
                                        ////Console.WriteLine(e.Handled.ToString() + " / " + e.Range.ToString());
                                    }
                                    else if (e.ColIndex == columnIndentCount - 1)
                                    {
                                        if (childTable.ParentTable is IGridTableOptionsSource &&
                                            ((IGridTableOptionsSource)childTable.ParentTable).TableOptions.ShowTableIndentAsCoveredRange)
                                        {
                                            e.Range = GridRangeInfo.Cells(firstRow, e.ColIndex, lastRow, e.ColIndex);
                                            e.Handled = true;
                                        }
                                    }
                                    else if (e.ColIndex == 0)
                                    {
                                        if (childTable.ParentTable is IGridTableOptionsSource &&
                                            ((IGridTableOptionsSource)childTable.ParentTable).TableOptions.ShowTableRowHeaderAsCoveredRange)
                                        {
                                            e.Range = GridRangeInfo.Cells(firstRow, e.ColIndex, lastRow, e.ColIndex);
                                            e.Handled = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case DisplayElementKind.FilterBar:
                    case DisplayElementKind.ColumnHeader:
                        {
                            if (e.ColIndex >= el.GroupLevel + 1 || e.ColIndex == 0)
                            {
                                int fieldNum = TableDescriptor.ColIndexToField(e.ColIndex); //// - Table.GroupedColumns.Count - 1;
                                GridRangeInfo[,] recordRowCoveredRanges = thisTable.TableDescriptor.RecordRowCoveredRanges;
                                if (fieldNum < recordRowCoveredRanges.GetLength(1))
                                {
                                    int row = 0;
                                    int rowCount = 1;
                                    if (!(RecordsAsDisplayElements || el is Record || el is RowElementsSection))
                                    {
                                        RowElementsSection headerSection = (RowElementsSection)el.ParentElement;
                                        RowElement rr = (RowElement)el;
                                        row = headerSection.RowElements.IndexOf(rr);
                                        rowCount = headerSection.RowElements.Count;
                                    }

                                    if (e.ColIndex == 0)
                                    {
                                        //// Row Headers spanning multiple rows
                                        e.Range = GridRangeInfo.Cells(e.RowIndex - row, 0, e.RowIndex - row + rowCount - 1, 0);
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        GridRangeInfo rg = recordRowCoveredRanges[row, fieldNum];
                                        if (rg != null && (rg.Width > 0 || rg.Height > 0))
                                        {
                                            int rowDelta = e.RowIndex - row;
                                            int colDelta = GetColumnIndentCount();
                                            e.Range = rg.OffsetRange(rowDelta, colDelta);

                                            if (e.Range.Left <= GetColumnIndentCount())
                                            {
                                                e.Range = e.Range.UnionRange(GridRangeInfo.Cells(e.Range.Top, el.GroupLevel + 1, e.Range.Bottom, GetColumnIndentCount()));
                                            }

                                            e.Handled = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }
#if ASPNET
#else

                    case DisplayElementKind.StackedHeader:
                        {
                            if (e.ColIndex >= el.GroupLevel + 1 || e.ColIndex == 0)
                            {
                                int fieldNum = TableDescriptor.ColIndexToField(e.ColIndex); //// - Table.GroupedColumns.Count - 1;
                                GridRangeInfo[,] recordRowCoveredRanges = thisTable.TableDescriptor.RecordRowCoveredRanges;
                                if (fieldNum < recordRowCoveredRanges.GetLength(1))
                                {
                                    int row = 0;
                                    int rowCount = 1;
                                    if (el is GridStackedHeaderRow)
                                    {
                                        GridStackedHeaderSection headerSection = (GridStackedHeaderSection)el.ParentElement;
                                        RowElement rr = (RowElement)el;
                                        row = headerSection.RowElements.IndexOf(rr);
                                        rowCount = headerSection.RowElements.Count;
                                    }

                                    if (e.ColIndex == 0)
                                    {
                                        //// Row Headers spanning multiple rows
                                        e.Range = GridRangeInfo.Cells(e.RowIndex - row, 0, e.RowIndex - row + rowCount - 1, 0);
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        GridStackedHeaderSpan span = TableDescriptor.StackedHeaderRows[row].GetStackedHeaderSpanAt(fieldNum);
                                        int colDelta = GetColumnIndentCount();
                                        if (span != null)
                                        {
                                            e.Range = GridRangeInfo.Cells(e.RowIndex, colDelta + span.firstCol, e.RowIndex, colDelta + span.lastCol);
                                            if (e.Range.Left <= GetColumnIndentCount())
                                            {
                                                e.Range = e.Range.UnionRange(GridRangeInfo.Cells(e.Range.Top, el.GroupLevel + 1, e.Range.Bottom, GetColumnIndentCount()));
                                            }

                                            e.Handled = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }
#endif
                    case DisplayElementKind.GroupFooter:
                    case DisplayElementKind.GroupHeader:
                    case DisplayElementKind.RecordPreview:
                    case DisplayElementKind.GroupPreview:
                        {
                            int maxCaptionColCount = Math.Max(el.GroupLevel + 2, TableDescriptor.GetColCount() - 1);
                            if (e.ColIndex >= el.GroupLevel + 1 && e.ColIndex <= maxCaptionColCount)
                            {
                                e.Range = GridRangeInfo.Cells(e.RowIndex, el.GroupLevel + 1, e.RowIndex, maxCaptionColCount);
                                e.Handled = true;
                            }

                            break;
                        }

                    case DisplayElementKind.Caption:
                        {
                            int maxCaptionColCount = Math.Max(this.GetColumnIndentCount(), TableDescriptor.GetColCount() - 1);
                            int startCol = el.GroupLevel + 1;
                            IGridGroupOptionsSource g = el.ParentGroup as IGridGroupOptionsSource;
                            if (g != null && !g.GroupOptions.ShowCaptionPlusMinus)
                            {
                                startCol--;
                            }

                            int d = 0; //// (ParentTableDescriptor.Relations.NestedCount > 0) ? 1 : 0;
                            ////if (!((GridTable) el.ParentTable).TableOptions.ShowRecordPlusMinus)
                            ////    d = 0;
                            if (e.ColIndex >= startCol && e.ColIndex <= maxCaptionColCount)
                            {
                                if (g == null || !g.GroupOptions.ShowCaptionSummaryCells)
                                {
                                    e.Range = GridRangeInfo.Cells(e.RowIndex, startCol, e.RowIndex, maxCaptionColCount);
                                    e.Handled = true;
                                }
                                else
                                {
                                    e.Range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                                    if (e.Range.Left <= GetColumnIndentCount())
                                    {
                                        e.Range = e.Range.UnionRange(GridRangeInfo.Cells(e.Range.Top, el.GroupLevel + 1 + d, e.Range.Bottom, GetColumnIndentCount()));
                                        e.Handled = true;
                                    }
                                }
                            }
                            else if (e.ColIndex == el.GroupLevel)
                            {
                                e.Range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                                e.Handled = true; //// PlusMinus
                            }
                            else if (e.ColIndex > maxCaptionColCount)
                            {
                                e.Range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                                e.Handled = true;
                            }

                            break;
                        }

                    case DisplayElementKind.Empty:
                        {
                            if (el != null && e.ColIndex >= el.GroupLevel + 1 && e.ColIndex <= GetColumnIndentCount())
                            {
                                e.Range = GridRangeInfo.Cells(e.RowIndex, el.GroupLevel + 1, e.RowIndex, GetColumnIndentCount());
                                e.Handled = true;
                            }

                            break;
                        }

                    case DisplayElementKind.Record:
                    case DisplayElementKind.AddNewRecord:
                        {
                            if (e.ColIndex >= el.GroupLevel + 1 || e.ColIndex == 0)
                            {
                                int fieldNum = TableDescriptor.ColIndexToField(e.ColIndex); // - Table.GroupedColumns.Count - 1;
                                GridRangeInfo[,] recordRowCoveredRanges = thisTable.TableDescriptor.RecordRowCoveredRanges;
                                if (fieldNum < recordRowCoveredRanges.GetLength(1) && recordRowCoveredRanges.GetLength(0) > 0)
                                {
                                    Record record = Record.GetParentRecord(el);
                                    if (record != null)
                                    {
                                        int d = (el.Kind == DisplayElementKind.Record && this.ParentTableDescriptor.Relations.NestedCount > 0) ? 1 : 0;
                                        if (!((GridTable)record.ParentTable).TableOptions.ShowRecordPlusMinus)
                                        {
                                            d = 0;
                                        }

                                        int row = 0;
                                        int rowCount = 1;

                                        if (!(RecordsAsDisplayElements || el is Record || el is RowElementsSection))
                                        {
                                            RecordRow rr = (RecordRow)el;
                                            row = record.RecordRows.IndexOf(rr);
                                            rowCount = record.RecordRows.Count;
                                        }

                                        if (e.ColIndex == 0 || (d > 0 && e.ColIndex == el.GroupLevel + 1))
                                        {
                                            //// Row Headers spanning multiple rows
                                            e.Range = GridRangeInfo.Cells(e.RowIndex - row, e.ColIndex, e.RowIndex - row + rowCount - 1, e.ColIndex);
                                            e.Handled = true;
                                        }
                                        else if (e.ColIndex >= el.GroupLevel + 1 + d)
                                        {
                                            GridRangeInfo rg = recordRowCoveredRanges[row, fieldNum];
                                            if (rg != null && (rg.Width > 0 || rg.Height > 0))
                                            {
                                                int rowDelta = e.RowIndex - row;
                                                int colDelta = GetColumnIndentCount();
                                                e.Range = rg.OffsetRange(rowDelta, colDelta);
                                                e.Handled = true;

                                                if (e.Range.Left <= GetColumnIndentCount())
                                                {
                                                    e.Range = e.Range.UnionRange(GridRangeInfo.Cells(e.Range.Top, el.GroupLevel + 1 + d, e.Range.Bottom, GetColumnIndentCount()));
                                                    e.Handled = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }

                    case DisplayElementKind.Summary:
                        {
                            GridSummaryRow sr = (GridSummaryRow)el;
                            if (e.ColIndex > el.GroupLevel)
                            {
                                int titleCount = sr.SummaryRowDescriptor.TitleColumnCount;
                                if (sr.IsFillRow)
                                {
                                    if (e.ColIndex < TableDescriptor.GetColCount())
                                    {
                                        e.Range = GridRangeInfo.Cells(e.RowIndex, el.GroupLevel + 1, e.RowIndex, TableDescriptor.GetColCount() - 1);
                                    }
                                    else
                                    {
                                        e.Range = GridRangeInfo.Cell(e.RowIndex, e.ColIndex);
                                    }

                                    e.Handled = true;
                                }
                                else if (e.ColIndex >= el.GroupLevel + 1 && e.ColIndex <= thisTable.TableDescriptor.GroupedColumns.Count + Math.Max(1, titleCount))
                                {
                                    e.Range = GridRangeInfo.Cells(e.RowIndex, el.GroupLevel + 1, e.RowIndex, thisTable.TableDescriptor.GroupedColumns.Count + Math.Max(1, titleCount));
                                    e.Handled = true;
                                }
                            }
                            else if (e.ColIndex >= GetColumnIndentCount())
                            {
                                if (!sr.IsFillRow)
                                {
                                    int fieldNum = TableDescriptor.ColIndexToField(e.ColIndex); //// - Table.GroupedColumns.Count - 1;
                                    GridRangeInfo[,] recordRowCoveredRanges = thisTable.TableDescriptor.RecordRowCoveredRanges;
                                    if (fieldNum < recordRowCoveredRanges.GetLength(1))
                                    {
                                        int row = 0;
                                        if (!(el is SummarySection))
                                        {
                                            GridSummarySection summarySection = (GridSummarySection)el.ParentElement;
                                            GridSummaryRow rr = (GridSummaryRow)el;
                                            row = summarySection.SummaryRows.IndexOf(rr);
                                        }

                                        GridRangeInfo rg = recordRowCoveredRanges[row, fieldNum];
                                        if (rg != null && (rg.Width > 0 || rg.Height > 0))
                                        {
                                            int rowDelta = e.RowIndex - row;
                                            int colDelta = thisTable.TableDescriptor.GroupedColumns.Count + 1;
                                            e.Range = rg.OffsetRange(rowDelta, colDelta);
                                            e.Handled = true;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                }

                if (!e.Handled)
                {
                    if (e.ColIndex > 0 && e.ColIndex <= el.GroupLevel)
                    {
                        Group group = el.ParentGroup;
                        if (group != null)
                        {
                            for (int n = e.ColIndex; n < el.GroupLevel; n++)
                            {
                                group = group.ParentGroup as Group;
                            }

                            IGridGroupOptionsSource g = group as IGridGroupOptionsSource;

                            if (g != null && g.GroupOptions.ShowGroupIndentAsCoveredRange)
                            {
                                int tablePos = thisTable.DisplayElements.IndexOf(group);
                                int firstRow = tablePos + 1;
                                int lastRow = tablePos + group.GetVisibleCount() - 1;
                                e.Range = GridRangeInfo.Cells(firstRow, e.ColIndex, lastRow, e.ColIndex);
                                e.Handled = true;
                            }
                        }
                    }
                }

                if (e.Handled)
                {
                    e.Handled = e.Range.Width > 1 || e.Range.Height > 1;
                }
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
            if (element == null)
            {
                return GridRangeInfo.Empty;
            }

            if (!RecordsAsDisplayElements)
            {
                Element parentElement = element.ParentElement;
                if (element is RecordRow)
                {
                    Record r = element.ParentRecord;
                    int rowIndex = this.DisplayElements.IndexOf(r);
                    ////TraceUtil.TraceCurrentMethodInfo(this.DisplayElements.IndexOf(element), rowIndex, rowIndex+this.TableDescriptor.RowsPerRecord-1);
                    return GridRangeInfo.Cells(rowIndex, 0, rowIndex + this.TableDescriptor.RowsPerRecord - 1, 0);
                }
                else if (parentElement is ColumnHeaderSection)
                {
                    int rowIndex = this.DisplayElements.IndexOf(parentElement);
                    ////TraceUtil.TraceCurrentMethodInfo(this.DisplayElements.IndexOf(element), rowIndex, rowIndex+this.TableDescriptor.RowsPerRecord-1);
                    return GridRangeInfo.Cells(rowIndex, 0, rowIndex + this.TableDescriptor.RowsPerRecord - 1, 0);
                }
                else if (parentElement is GridStackedHeaderSection)
                {
                    int rowIndex = this.DisplayElements.IndexOf(parentElement);
                    ////TraceUtil.TraceCurrentMethodInfo(this.DisplayElements.IndexOf(element), rowIndex, rowIndex+this.TableDescriptor.RowsPerRecord-1);
                    return GridRangeInfo.Cells(rowIndex, 0, rowIndex + this.TableDescriptor.StackedHeaderRows.VisibleRowCount - 1, 0);
                }
                else if (element is Record || element is ColumnHeaderSection)
                {
                    int rowIndex = this.DisplayElements.IndexOf(element);
                    return GridRangeInfo.Cells(rowIndex, 0, rowIndex + this.TableDescriptor.RowsPerRecord - 1, 0);
                }
            }

            return GridRangeInfo.Cell(this.DisplayElements.IndexOf(element), 0);
        }

        #endregion

        #region Metrics
        /// <summary>
        /// Returns the column width of the last column. The last column in the grid is used to fill empty space after
        /// the last record field. This is needed for hierarchical display, when a nested table is wider than the current
        /// table.
        /// </summary>
        public int LastColumnWidth
        {
            get
            {
                int t = GetHorizontalScrollWidth();
                return this.lastColumnWidth;
            }
        }

        /// <override/>
        /// <summary>Gets or sets the default width of row headers.</summary>
        public override int DefaultRowHeaderWidth
        {
            get
            {
                if (this.TableOptions.ShowRowHeader
                                && this.TableDescriptor.RowsPerRecord > 0)
                {
                    int value = TableOptions.RowHeaderWidth;
                    return value < 0 ? base.DefaultRowHeaderWidth : value;
                }

                return 0;
            }

            set
            {
                TableOptions.RowHeaderWidth = value;
            }
        }

        /// <override/>
        /// <summary>Default width of table indents.</summary>
        public override int DefaultTableIndentWidth
        {
            get
            {
                if (this.TableOptions.ShowTableIndent
                                && this.TableDescriptor.RowsPerRecord > 0)
                {
                    return DefaultIndentWidth;
                }

                return 0;
            }

            set
            {
                TableOptions.IndentWidth = value;
            }
        }

        /// <override/>
        /// <summary>Default height of group caption rows.</summary>
        public override int DefaultCaptionRowHeight
        {
            get
            {
                int value = TableOptions.CaptionRowHeight;
                return value < 0 ? base.DefaultCaptionRowHeight : value;
            }

            set
            {
                TableOptions.CaptionRowHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of column header rows.</summary>
        public override int DefaultColumnHeaderRowHeight
        {
            get
            {
                int value = TableOptions.ColumnHeaderRowHeight;
                return value < 0 ? base.DefaultColumnHeaderRowHeight : value;
            }

            set
            {
                TableOptions.ColumnHeaderRowHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of empty section rows.</summary>
        public override int DefaultEmptySectionHeight
        {
            get
            {
                int value = TableOptions.EmptySectionHeight;
                return value < 0 ? base.DefaultEmptySectionHeight : value;
            }

            set
            {
                TableOptions.EmptySectionHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of filter bar rows.</summary>
        public override int DefaultFilterBarRowHeight
        {
            get
            {
                int value = TableOptions.FilterBarRowHeight;
                return value < 0 ? base.DefaultFilterBarRowHeight : value;
            }

            set
            {
                TableOptions.FilterBarRowHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of group footer sections.</summary>
        public override int DefaultGroupFooterSectionHeight
        {
            get
            {
                int value = TableOptions.GroupFooterSectionHeight;
                return value < 0 ? base.DefaultGroupFooterSectionHeight : value;
            }

            set
            {
                TableOptions.GroupFooterSectionHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of group header sections.</summary>
        public override int DefaultGroupHeaderSectionHeight
        {
            get
            {
                int value = TableOptions.GroupHeaderSectionHeight;
                return value < 0 ? base.DefaultGroupHeaderSectionHeight : value;
            }

            set
            {
                TableOptions.GroupHeaderSectionHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of group preview rows.</summary>
        public override int DefaultGroupPreviewSectionHeight
        {
            get
            {
                int value = TableOptions.GroupPreviewSectionHeight;
                return value < 0 ? base.DefaultGroupPreviewSectionHeight : value;
            }

            set
            {
                TableOptions.GroupPreviewSectionHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default width of group indents.</summary>
        public override int DefaultIndentWidth
        {
            get
            {
                int value = TableOptions.IndentWidth;
                return value < 0 ? base.DefaultIndentWidth : value;
            }

            set
            {
                TableOptions.IndentWidth = value;
            }
        }

        /// <override/>
        /// <summary>Default height of record preview rows.</summary>
        public override int DefaultRecordPreviewRowHeight
        {
            get
            {
                int value = TableOptions.RecordPreviewRowHeight;
                return value < 0 ? base.DefaultRecordPreviewRowHeight : value;
            }

            set
            {
                TableOptions.RecordPreviewRowHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of record rows.</summary>
        public override int DefaultRecordRowHeight
        {
            get
            {
                int value = TableOptions.RecordRowHeight;
                return value < 0 ? base.DefaultRecordRowHeight : value;
            }

            set
            {
                TableOptions.RecordRowHeight = value;
            }
        }

        /// <override/>
        /// <summary>Default height of summary rows.</summary>
        public override int DefaultSummaryRowHeight
        {
            get
            {
                int value = TableOptions.SummaryRowHeight;
                return value < 0 ? base.DefaultSummaryRowHeight : value;
            }

            set
            {
                TableOptions.SummaryRowHeight = value;
            }
        }

        #endregion

        #region Width

        private void VisibleColumns_TotalWidthRequest(object sender, CancelEventArgs e)
        {
#if ASPNET
#else
            if (this.TableModel != null)
            {
                TableModel.UpdateColumnWidths();
            }
#endif
        }

        /// <summary>
        /// Calculates the width of row headers and all indent columns before
        /// the first record column.
        /// </summary>
        /// <returns>Total width.</returns>
        public int GetTotalWidthOfRowHeadersAndIndent()
        {
            int columnIndentCount = GetColumnIndentCount();

            int width = this.DefaultRowHeaderWidth;
            for (int n = 0; n < this.TableDescriptor.GroupedColumns.Count; n++)
            {
                width += DefaultIndentWidth;
            }

            if (this.TableDescriptor.Relations.NestedCount > 0)
            {
                width += this.DefaultTableIndentWidth;
            }

            return width;
        }

        /// <summary>
        /// Calculates the width of row headers and all indent columns before
        /// the first record column.
        /// </summary>
        /// <param name="includeWidthOfParentTableIndent">Specifies if width of nested table indents and parent table row headers should be added.</param>
        /// <returns>Total width.</returns>
        public int GetTotalWidthOfRowHeadersAndIndent(bool includeWidthOfParentTableIndent)
        {
            if (!includeWidthOfParentTableIndent)
            {
                return GetTotalWidthOfRowHeadersAndIndent();
            }

            int indentWidth = GetTotalWidthOfRowHeadersAndIndent();
            GridTable parentTable = RelationParentTable;
            while (parentTable != null)
            {
                indentWidth += parentTable.GetTotalWidthOfRowHeadersAndIndent();
                parentTable = parentTable.RelationParentTable;
            }

            return indentWidth;
        }

        /// <summary>
        /// Calculates the width of the table considering all indent columns and visible columns.
        /// </summary>
        /// <param name="checkNestedTables">True if nested tables should also be considered.</param>
        /// <returns>Total width.</returns>
        public int GetTotalWidthOfTable(bool checkNestedTables)
        {
            int indentWidth = GetTotalWidthOfRowHeadersAndIndent();
            int width = indentWidth + this.TableDescriptor.VisibleColumns.TotalWidth;

            int maxWidth = width;
            if (checkNestedTables)
            {
                Table[] tables = new Table[RelatedTables.Count];
                RelatedTables.CopyTo(tables, 0); // trigger SynchronizeRelatedTables
                foreach (GridTable relatedTable in tables)
                {
                    switch (relatedTable.ParentTableDescriptor.ParentRelation.RelationKind)
                    {
                        case RelationKind.RelatedMasterDetails:
                        case RelationKind.UniformChildList:
                            maxWidth = Math.Max(maxWidth, indentWidth + relatedTable.GetTotalWidthOfTable(true));
                            break;
                    }
                }
            }

            return maxWidth;
        }

        /// <summary>
        /// Calculates the width of the table considering all indent columns and visible columns
        /// and internally updates the lastColumnWidth field to autosize the last grid column after
        /// the last table column if nested tables are wider.
        /// </summary>
        /// <returns>returns HorizontalScrollWidth</returns>
        internal int GetHorizontalScrollWidth()
        {
            return GetHorizontalScrollWidth(false);
        }

        /// <summary>
        /// Calculates the width of the table considering all indent columns and visible columns
        /// and internally updates the lastColumnWidth field to autosize the last grid column after
        /// the last table column if nested tables are wider.
        /// </summary>
        /// <returns>returns HorizontalScrollWidth</returns>
        internal int GetHorizontalScrollWidth(bool isNested)
        {
            int indentWidth = GetTotalWidthOfRowHeadersAndIndent();
            int width = indentWidth + this.TableDescriptor.VisibleColumns.TotalWidth;

            int maxWidth = width;
            Table[] tables = new Table[RelatedTables.Count];
            RelatedTables.CopyTo(tables, 0); // trigger SynchronizeRelatedTables
            foreach (GridTable relatedTable in tables)
            {
                switch (relatedTable.ParentTableDescriptor.ParentRelation.RelationKind)
                {
                    case RelationKind.RelatedMasterDetails:
                    case RelationKind.UniformChildList:
                        maxWidth = Math.Max(maxWidth, indentWidth + relatedTable.GetHorizontalScrollWidth(true));
                        break;
                }
            }

            int ll = lastColumnWidth;
            bool b = lastColumnWidth != maxWidth - width;

            ////                                                if (!isNested)
            ////                                                {
            lastColumnWidth = maxWidth - width;

            ////                                                                foreach (GridTable relatedTable in RelatedTables)
            ////                                                                                relatedTable.FixLastColumnWidth(maxWidth - indentWidth);
            ////                                               }

#if ASPNET
#else
            if (b)
            {
                if (TableDescriptor.ParentRelation != null && TableDescriptor.ParentRelation.RelationKind == RelationKind.ForeignKeyKeyWords)
                {
                    //// Do nothing - avoids flickering when dropping down related table.
                    ////Console.WriteLine(ll);
                }
                else
                {
                    //// TODO: Add support for multiple views
                    if (TableModel != null && TableModel.ActiveGridView != null)
                    {
                        TableModel.ActiveGridView.ViewLayout.Reset();
                        TableModel.ActiveGridView.Invalidate();
                    }
                }
            }
#endif
            ////                                                savedWidth = width;
            ////                                                savedIndentWidth = indentWidth;

            return maxWidth;
        }

        ////                                int savedWidth = 0;
        ////                                int savedIndentWidth = 0;
        ////
        ////                                internal void FixLastColumnWidth(int maxWidth)
        ////                                {
        ////                                                int width = savedWidth; //// saved in previous GetHorizontalScrollWidth call.
        ////
        ////                                                if (lastColumnWidth != maxWidth-width)
        ////                                                {
        ////                                                                //// TODO: Add support for multiple views
        ////                                                                if (TableModel != null && TableModel.ActiveGridView != null)
        ////                                                                {
        ////                                                                                TableModel.ActiveGridView.ViewLayout.Reset();
        ////                                                                                TableModel.ActiveGridView.Invalidate();
        ////                                                                }
        ////                                                }
        ////                                                lastColumnWidth = maxWidth-width;
        ////
        ////                                                foreach (GridTable relatedTable in RelatedTables)
        ////                                                                relatedTable.FixLastColumnWidth(maxWidth - savedIndentWidth);
        ////                                }

        #endregion

        #region Record Heights
        internal int GetRecordHeight(int pos)
        {
            return GetRecordHeight(null, pos);
        }

        internal int GetRecordHeightTotal(ChildTable childTable, int first, int last)
        {
            int count = NestedDisplayElements.Count;
            if (first >= count)
            {
                return 0;
            }

            last = Math.Min(count - 1, last);

            Element firstElement = NestedDisplayElements[first];
            if (first == last)
            {
                ////TraceUtil.TraceCurrentMethodInfo(first);
                return (int)firstElement.GetYAmountCount();
            }

            if (Engine.SupportsYAmount)
            {
                double firstYAmountPosition = NestedDisplayElements.GetYAmountPositionOf(firstElement);

                double lastYAmountPosition;
                if (last == count - 1)
                {
                    Element lastElement = NestedDisplayElements[last];
                    lastYAmountPosition = NestedDisplayElements.GetYAmountPositionOf(lastElement);
                    lastYAmountPosition += lastElement.GetYAmountCount();
                }
                else
                {
                    Element lastElement = NestedDisplayElements[last + 1];
                    lastYAmountPosition = NestedDisplayElements.GetYAmountPositionOf(lastElement);
                    if (lastYAmountPosition == -1)
                    {
                        lastYAmountPosition = NestedDisplayElements.GetYAmountPositionOf(lastElement);
                        lastElement = NestedDisplayElements[last];
                        lastYAmountPosition = NestedDisplayElements.GetYAmountPositionOf(lastElement);
                        lastYAmountPosition += lastElement.GetYAmountCount();
                    }
                }

                ////TraceUtil.TraceCurrentMethodInfo(first, last, lastYAmountPosition-firstYAmountPosition);
                return (int)(lastYAmountPosition - firstYAmountPosition);
            }
            else
            {
                int height = 0;
                for (int n = first; n <= last; n++)
                {
                    height += (int)NestedDisplayElements[n].GetYAmountCount();
                }

                return height;
            }
        }

        internal int GetRecordHeight(ChildTable childTable, int pos)
        {
            ////TraceUtil.TraceCurrentMethodInfo(pos);
            ChildTable savedChildTable = this.FilteredChildTable;
            this.FilteredChildTable = childTable;
            try
            {
                if (pos < NestedDisplayElements.Count)
                {
                    Element el = NestedDisplayElements[pos];
                    ////if (el is NestedTable)
                    ////                el = NestedDisplayElements[pos];
                    if (el != null)
                    {
                        return (int)el.GetYAmountCount();
                    }
                }

                return 0;
            }
            finally
            {
                this.FilteredChildTable = savedChildTable;
            }
        }
        #endregion

        #region NestedTable Cells
        /// <summary>
        /// Returns the cell type name for a <see cref="NestedTable"/> based on its TableDescriptor name. The
        /// returned name is "RT" + relatedTable.TableDescriptor.Name.
        /// </summary>
        /// <param name="nestedTable">The nested table.</param>
        /// <returns>The cell type name.</returns>
        public string GetNestedTableCellType(NestedTable nestedTable)
        {
            int nestedTableIndex = nestedTable.ParentRecord.NestedTables.IndexOf(nestedTable);
            if (nestedTable.ChildTable == null)
            {
                return "Static";
            }

            GridTable relatedTable = (GridTable)nestedTable.ChildTable.ParentTable;
            string cellType = "RT" + relatedTable.TableDescriptor.Name;
            ////                                                if (!this.CellModels.ContainsKey(cellType))
            ////                                                {
            ////                                                                ////Console.WriteLine(subTable.Table == Table);
            ////                                                                GridNestedTableControlCellModel nestedTableControlCellModel = new GridNestedTableControlCellModel(this, (GridTable) nestedTable.ChildTable.ParentTable);
            ////                                                                this.CellModels.Add(cellType, nestedTableControlCellModel);
            ////                                                                WireNestedTableControlCellModel(nestedTableControlCellModel);
            ////                                                                nestedTableControlCellModel.TableModel.UpdateColumnWidths();
            ////                                                }

            return cellType;
        }

        #endregion

        #region Cell Style and Identity
        
        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="column">The column.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(Record record, GridColumnDescriptor column)
        {
            if (!record.GetVisibleInHierarchy())
            {
                return GetRecordColumnStyle(record, column);
            }

            //// Get the row and column index in the grid for a column (works also
            //// if column sets with multiple rows are specified).
            int relativeRowIndex, colIndex;
            this.TableDescriptor.ColumnToRowColIndex(column.MappingName, out relativeRowIndex, out colIndex);

            ChildTable savedFilteredChildTable = FilteredChildTable;
            record.ParentTable.FilteredChildTable = record.ParentChildTable;
            int recordRowIndex = record.ParentChildTable.DisplayElements.IndexOf(record);

            int rowIndex = recordRowIndex + relativeRowIndex;
            GridTableCellStyleInfo style = this.GetTableCellStyle(rowIndex, colIndex + this.GetColumnIndentCount());
            FilteredChildTable = savedFilteredChildTable;
            return style;
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="visibleColumn">The column.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(Record record, GridVisibleColumnDescriptor visibleColumn)
        {
            // Get the row and column index in the grid for a column (works also
            //// if column sets with multiple rows are specified).
            GridColumnDescriptor cd = this.TableDescriptor.Columns[visibleColumn.Name];

            if (!record.GetVisibleInHierarchy())
            {
                return GetRecordColumnStyle(record, cd);
            }

            int relativeRowIndex, colIndex;
            this.TableDescriptor.ColumnToRowColIndex(cd.MappingName, out relativeRowIndex, out colIndex);

            ChildTable savedFilteredChildTable = FilteredChildTable;
            record.ParentTable.FilteredChildTable = record.ParentChildTable;
            int recordRowIndex = record.ParentChildTable.DisplayElements.IndexOf(record);

            int rowIndex = recordRowIndex + relativeRowIndex;
            GridTableCellStyleInfo style = this.GetTableCellStyle(rowIndex, colIndex + this.GetColumnIndentCount());
            FilteredChildTable = savedFilteredChildTable;
            return style;
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="field">The field for the column.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(Record record, FieldDescriptor field)
        {
            if (!record.GetVisibleInHierarchy())
            {
                return GetRecordColumnStyle(record, TableDescriptor.Columns.FindByField(field));
            }

            //// Get the row and column index in the grid for a column (works also
            //// if column sets with multiple rows are specified).
            int relativeRowIndex, colIndex;
            this.TableDescriptor.ColumnToRowColIndex(field.Name, out relativeRowIndex, out colIndex);

            ChildTable savedFilteredChildTable = FilteredChildTable;
            record.ParentTable.FilteredChildTable = record.ParentChildTable;
            int recordRowIndex = record.ParentChildTable.DisplayElements.IndexOf(record);

            int rowIndex = recordRowIndex + relativeRowIndex;
            GridTableCellStyleInfo style = this.GetTableCellStyle(rowIndex, colIndex + this.GetColumnIndentCount());
            FilteredChildTable = savedFilteredChildTable;
            return style;
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">The name of field for the column (e.g. GridColumnDescriptor.MappingName returns the field name).</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(Record record, string fieldName)
        {
            if (!record.GetVisibleInHierarchy())
            {
                return GetRecordColumnStyle(record, TableDescriptor.Columns.FindByMappingName(fieldName));
            }

            //// Get the row and column index in the grid for a column (works also
            //// if column sets with multiple rows are specified).
            int relativeRowIndex, colIndex;
            this.TableDescriptor.ColumnToRowColIndex(fieldName, out relativeRowIndex, out colIndex);

            ChildTable savedFilteredChildTable = FilteredChildTable;
            record.ParentTable.FilteredChildTable = record.ParentChildTable;
            int recordRowIndex = record.ParentChildTable.DisplayElements.IndexOf(record);

            int rowIndex = recordRowIndex + relativeRowIndex;
            GridTableCellStyleInfo style = this.GetTableCellStyle(rowIndex, colIndex + this.GetColumnIndentCount());
            FilteredChildTable = savedFilteredChildTable;
            return style;
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="fieldName">The column.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(ISummarySection el, string fieldName)
        {
            return GetTableCellStyle((Element)el, fieldName);
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="fieldName">The column.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(Element el, string fieldName)
        {
            if (el is Record)
            {
                return GetTableCellStyle((Record)el, fieldName);
            }
            else if (el is RecordRow)
            {
                return GetTableCellStyle(el.ParentRecord, fieldName);
            }
            else
            {
                if (el is RowElement)
                {
                    el = el.ParentSection;
                }

                //// Get the row and column index in the grid for a column (works also
                //// if column sets with multiple rows are specified).
                int relativeRowIndex, colIndex;
                this.TableDescriptor.ColumnToRowColIndex(fieldName, out relativeRowIndex, out colIndex);

                ChildTable savedFilteredChildTable = FilteredChildTable;
                el.ParentTable.FilteredChildTable = el.ParentChildTable;
                int recordRowIndex = 0;
                if (ParentChildTable != null)
                    recordRowIndex = el.ParentChildTable.DisplayElements.IndexOf(el);

                int rowIndex = recordRowIndex + relativeRowIndex;
                GridTableCellStyleInfo style = this.GetTableCellStyle(rowIndex, colIndex + this.GetColumnIndentCount());
                FilteredChildTable = savedFilteredChildTable;
                return style;
            }
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo GetTableCellStyle(int rowIndex, int colIndex)
        {
            Element displayElement = null;

            if (rowIndex == -1)
            {
                displayElement = this;
            }
            else if (rowIndex >= 0 && rowIndex < DisplayElements.Count)
            {
                displayElement = DisplayElements[rowIndex];
            }

            if (displayElement != null)
            {
                GridTableCellStyleInfo style = null;

                //// Look first in display elements cache if element supports caching
                IGridTableCellStyleCache styleInfoWeakReferences = displayElement as IGridTableCellStyleCache;
                if (styleInfoWeakReferences != null)
                {
                    style = styleInfoWeakReferences.GetStyleAtColumn(colIndex + 1);
                }

                //// Not found.
                if (style == null)
                {
                    //// allocate style object and initialize its identity
                    style = CreateTableCellStyle(displayElement, rowIndex, colIndex);

                    //// save style object if element supports caching
                    if (styleInfoWeakReferences != null)
                    {
                        styleInfoWeakReferences.SetStyleAtColumn(colIndex + 1, style);
                    }
                }

                return style;
            }

            ////throw new InvalidOperationException("No Display Element found at row " + rowIndex.ToString());
            this.ClearCollectionCaches();
            return CreateTableCellStyle(displayElement, rowIndex, colIndex);
            ////return GridTableCellStyleInfo.Empty;
        }

        /// <summary>
        /// Returns the <see cref="GridTableCellStyleInfo"/> for a specified grid row and column.
        /// </summary>
        /// <param name="element">YThe element displayed at row index.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>returns GridTableCellStyleInfo</returns>
        internal GridTableCellStyleInfo CreateTableCellStyle(Element element, int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo style;
            ////using (MeasureTime.Measure("CreateTableCellStyle"))
            {
                //// allocate style object and initialize its identity
                GridTableCellStyleInfoIdentity tableCellIdentity = CreateTableCellIdentity(element, rowIndex, colIndex);
                style = new GridTableCellStyleInfo(tableCellIdentity);

                if (tableCellIdentity != null)
                {
                    //// Suspend OnStyleChanged
                    style.BeginInit();

                    //// initialize style
                    ////using (MeasureTime.Measure("SetupStyleInfo"))
                    this.SetupStyleInfo(tableCellIdentity, style);

                    //// raise event, give user a change to customize style settings
                    GridTableCellStyleInfoEventArgs e = new GridTableCellStyleInfoEventArgs(tableCellIdentity, style, null);
                    ////using (MeasureTime.Measure("RaiseQueryCellStyleInfo"))
                    this.RaiseQueryCellStyleInfo(e);
#if FK_SUPPORT
                                                                if (e.TableCellIdentity.Column != null &&
                                                                                e.TableCellIdentity.Column.FieldDescriptor != null &&
                                                                                e.TableCellIdentity.Column.FieldDescriptor.IsRelatedField() &&
                                                                                e.Style.CellType == "ForeignKeyCell")
                                                                {
                                                                                e.Style.CellType = "RT" + e.TableCellIdentity.Column.FieldDescriptor.GetRelation().ChildTableDescriptor.Name;
                                                                }
#endif
                    // Resume OnStyleChanged
                    style.EndInit();
                }
            }

            return style;
        }

        internal GridTableCellStyleInfoIdentity CreateRecordColumnIdentity(Element element, GridColumnDescriptor column)
        {
            if (element != null)
            {
                Element displayElement = element;

                IGridData volatileData = null;
                if (TableModel != null)
                {
                    volatileData = TableModel.VolatileData;
                }

                GridTableCellStyleInfoIdentity e = new GridTableCellStyleInfoIdentity(volatileData, 0, 0);

                int groupedColumnsCount = TableDescriptor.GroupedColumns.Count;

                e.DisplayElement = displayElement;
                e.Table = this;

                if (displayElement.GroupLevel > 0)
                {
                    e.GroupedColumn = this.TableDescriptor.GroupedColumns[displayElement.GroupLevel - 1];
                }

                e.Column = column;

                e.TableCellType = GridTableCellType.AnyCell;
                Record record = Record.GetParentRecord(displayElement);
                if (record != null)
                {
                    Group g = record.ParentGroup;
                    if (g != null)
                    {
                        int recordIndex = g.FilteredRecords.IndexOf(record);
                        if (recordIndex % 2 == 0)
                        {
                            e.TableCellType = GridTableCellType.RecordFieldCell;
                        }
                        else
                        {
                            e.TableCellType = GridTableCellType.AlternateRecordFieldCell;
                        }
                    }
                }

                return e;
            }

            return null;
        }

        /// <summary>
        /// Creates a style object and initializes its identity to 
        /// emulate how the value would appear in a cell in the grid.
        /// </summary>
        /// <param name="record">The Record.</param>
        /// <param name="column">The Column.</param>
        /// <returns>A style object for the specified column in the given record.</returns>
        public GridTableCellStyleInfo GetRecordColumnStyle(Record record, GridColumnDescriptor column)
        {
            GridTableCellStyleInfo style;
            ////using (MeasureTime.Measure("CreateTableCellStyle"))
            {
                //// allocate style object and initialize its identity
                GridTableCellStyleInfoIdentity tableCellIdentity = CreateRecordColumnIdentity(record, column);
                style = new GridTableCellStyleInfo(tableCellIdentity);

                if (tableCellIdentity != null)
                {
                    //// Suspend OnStyleChanged
                    style.BeginInit();

                    //// initialize style
                    ////using (MeasureTime.Measure("SetupStyleInfo"))
                    this.SetupStyleInfo(tableCellIdentity, style);

                    //// raise event, give user a change to customize style settings
                    GridTableCellStyleInfoEventArgs e = new GridTableCellStyleInfoEventArgs(tableCellIdentity, style, null);
                    ////using (MeasureTime.Measure("RaiseQueryCellStyleInfo"))
                    this.RaiseQueryCellStyleInfo(e);

                    //// Resume OnStyleChanged
                    style.EndInit();
                }
            }

            return style;
        }

        internal GridTableCellStyleInfoIdentity CreateTableCellIdentity(Element element, int rowIndex, int colIndex)
        {
            ////using (MeasureTime.Measure("CreateTableCellIdentity"))
            {
                if (element != null)
                {
                    Element displayElement = element;

                    IGridData volatileData = null;
                    if (TableModel != null)
                    {
                        volatileData = TableModel.VolatileData;
                    }

                    GridTableCellStyleInfoIdentity e = new GridTableCellStyleInfoIdentity(volatileData, rowIndex, colIndex);

                    int groupedColumnsCount = TableDescriptor.GroupedColumns.Count;

                    e.DisplayElement = displayElement;
                    e.Table = this;

                    if (displayElement.GroupLevel > 0)
                    {
                        e.GroupedColumn = this.TableDescriptor.GroupedColumns[displayElement.GroupLevel - 1];
                    }

                    switch (displayElement.Kind)
                    {
                        case DisplayElementKind.Table:
                            e.TableCellType = GridTableCellType.AnyCell;
                            break;

                        case DisplayElementKind.Empty:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.EmptySectionRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.NestedTable:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.NestedTableRowHeaderCell;
                            }
                            else if (colIndex <= groupedColumnsCount)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex == groupedColumnsCount + 1)
                            {
                                e.TableCellType = GridTableCellType.NestedTableIndentCell;
                            }
                            else if (colIndex == groupedColumnsCount + 2)
                            {
                                e.TableCellType = GridTableCellType.NestedTableCell;
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.Caption:
                            {
                                int startCol = displayElement.GroupLevel + 1;
                                IGridGroupOptionsSource g = displayElement.ParentGroup as IGridGroupOptionsSource;
                                if (g != null && !g.GroupOptions.ShowCaptionPlusMinus)
                                {
                                    startCol--;
                                }

                                e.TableCellType = GridTableCellType.EmptyCell;

                                if (colIndex == -1)
                                {
                                    e.TableCellType = GridTableCellType.AnyCell;
                                }
                                else if (colIndex == startCol)
                                {
                                    //// if-statement prevents first column in a TopLevelGroup to become
                                    //// a header-cell when ShowCaptionSummaryCells was specified for TopLevelGroup.
                                    if (!(g != null && g.GroupOptions.ShowCaptionSummaryCells && displayElement.GroupLevel == 0))
                                    {
                                        e.TableCellType = GridTableCellType.GroupCaptionCell;
                                    }
                                }
                                else if (colIndex == displayElement.GroupLevel)
                                {
                                    e.TableCellType = GridTableCellType.GroupCaptionPlusMinusCell;
                                }
                                else if (colIndex == 0)
                                {
                                    e.TableCellType = GridTableCellType.GroupCaptionRowHeaderCell;
                                }
                                else if (colIndex == displayElement.GroupLevel - 1)
                                {
                                    e.TableCellType = GridTableCellType.GroupIndentTCell;
                                    e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                                }
                                else if (colIndex < displayElement.GroupLevel)
                                {
                                    e.TableCellType = GridTableCellType.GroupIndentICell;
                                    e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                                }

                                //// Check if none of the above criteria was met.
                                if (e.TableCellType == GridTableCellType.EmptyCell)
                                {
                                    if (g != null && g.GroupOptions.ShowCaptionSummaryCells)
                                    {
                                        e.TableCellType = GridTableCellType.GroupCaptionSummaryCell;
                                        if (displayElement.GroupLevel >= 1)
                                        {
                                            e.GroupedColumn = this.TableDescriptor.GroupedColumns[displayElement.GroupLevel - 1];
                                        }

                                        GridSummaryRowDescriptor srd = TableDescriptor.SummaryRows[g.GroupOptions.CaptionSummaryRow];
                                        if (srd != null)
                                        {
                                            int fieldNum = TableDescriptor.ColIndexToField(colIndex);
                                            GridSummaryColumnDescriptor sc = srd.GetSummaryColumnAtCol(fieldNum);
                                            if (sc != null && sc.Style == GridSummaryStyle.Column)
                                            {
                                                e.SummaryColumn = sc;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        e.TableCellType = GridTableCellType.EmptyCell;
                                    }
                                }

                                break;
                            }

                        case DisplayElementKind.ColumnHeader:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0 && displayElement.GroupLevel == 0)
                            {
                                e.TableCellType = GridTableCellType.TopLeftHeaderCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.RecordRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.Column = GetColumnDescriptorAt(displayElement, colIndex);
                                if (e.Column == null)
                                {
                                    e.TableCellType = GridTableCellType.EmptyCell;
                                }
                                else
                                {
                                    e.TableCellType = GridTableCellType.ColumnHeaderCell;
                                }
                            }

                            break;
#if ASPNET
#else

                        case DisplayElementKind.StackedHeader:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0 && displayElement.GroupLevel == 0)
                            {
                                e.TableCellType = GridTableCellType.TopLeftHeaderCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.RecordRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.StackedHeader = GetStackedHeaderDescriptorAt(displayElement, colIndex);
                                if (e.StackedHeader == null)
                                {
                                    e.TableCellType = GridTableCellType.EmptyCell;
                                }
                                else
                                {
                                    e.TableCellType = GridTableCellType.StackedHeaderCell;
                                }
                            }

                            break;
#endif
                        case DisplayElementKind.FilterBar:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.FilterBarRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.Column = GetColumnDescriptorAt(displayElement, colIndex);
                                if (e.Column == null)
                                {
                                    e.TableCellType = GridTableCellType.EmptyCell;
                                }
                                else
                                {
                                    e.TableCellType = GridTableCellType.FilterBarCell;
                                    string sdName = e.Column.Name + "FilterBarChoices";
                                    if (TableDescriptor.Summaries.Contains(sdName))
                                        e.FilterBarSummaryDescriptor = this.TableDescriptor.Summaries[sdName];
                                }
                            }

                            break;

                        case DisplayElementKind.GroupFooter:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.GroupFooterRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupFooterIndentCell;
                                ////if (displayElement.GroupLevel > 0 || displayElement.ParentChildTable.ParentNestedTable == null)
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex == displayElement.GroupLevel + 1)
                            {
                                e.TableCellType = GridTableCellType.GroupFooterSectionCell;
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.GroupHeader:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                //// && (displayElement.GroupLevel > 0 || displayElement.ParentChildTable.ParentNestedTable == null))
                                e.TableCellType = GridTableCellType.GroupHeaderRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupHeaderIndentCell;
                                ////if (displayElement.GroupLevel > 0 || displayElement.ParentChildTable.ParentNestedTable == null)
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex == displayElement.GroupLevel + 1)
                            {
                                e.TableCellType = GridTableCellType.GroupHeaderSectionCell;
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.GroupPreview:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.GroupPreviewRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex == displayElement.GroupLevel + 1)
                            {
                                e.TableCellType = GridTableCellType.GroupPreviewCell;
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.RecordPreview:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.RecordPreviewRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.RecordPreviewCell;
                            }

                            break;

                        case DisplayElementKind.AddNewRecord:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.AddNewRecordRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else
                            {
                                e.Column = GetColumnDescriptorAt(displayElement, colIndex);
                                if (e.Column == null)
                                {
                                    e.TableCellType = GridTableCellType.EmptyCell;
                                }
                                else
                                {
                                    e.TableCellType = GridTableCellType.AddNewRecordFieldCell;
                                }
                            }

                            break;

                        case DisplayElementKind.Summary:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                e.TableCellType = GridTableCellType.SummaryRowHeaderCell;
                            }
                            else if (colIndex <= displayElement.GroupLevel)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex < TableDescriptor.GetColCount())
                            {
                                bool summaryField = false;

                                GridSummaryRow sr = (GridSummaryRow)displayElement;
                                if (colIndex == displayElement.GroupLevel + 1)
                                {
                                    if (sr.IsFillRow)
                                    {
                                        e.TableCellType = GridTableCellType.SummaryFillRowCell;
                                    }
                                    else if (sr.SummaryRowDescriptor.TitleColumnCount > 0)
                                    {
                                        e.TableCellType = GridTableCellType.SummaryTitleCell;
                                    }
                                    else
                                    {
                                        //// TitleColumnCount == 0
                                        summaryField = true;
                                    }

                                    if (!summaryField)
                                    {
                                        foreach (GridSummaryColumnDescriptor sc in sr.SummaryRowDescriptor.SummaryColumns)
                                        {
                                            if (sc.Style == GridSummaryStyle.FillRow)
                                            {
                                                e.SummaryColumn = sc;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    summaryField = true;
                                }

                                if (summaryField)
                                {
                                    if (sr.IsFillRow)
                                    {
                                        e.TableCellType = GridTableCellType.EmptyCell;
                                    }
                                    else
                                    {
                                        int fieldNum = TableDescriptor.ColIndexToField(colIndex);
                                        GridSummaryColumnDescriptor sc = sr.SummaryRowDescriptor.GetSummaryColumnAtCol(fieldNum);
                                        if (sc != null && sc.Style == GridSummaryStyle.Column)
                                        {
                                            e.TableCellType = GridTableCellType.SummaryFieldCell;
                                            e.SummaryColumn = sc;
                                        }
                                        else
                                        {
                                            e.TableCellType = GridTableCellType.SummaryEmptyCell;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                e.TableCellType = GridTableCellType.EmptyCell;
                            }

                            break;

                        case DisplayElementKind.Record:
                            if (colIndex == -1)
                            {
                                e.TableCellType = GridTableCellType.AnyCell;
                            }
                            else if (colIndex == 0)
                            {
                                Record record = Record.GetParentRecord(displayElement);

                                int recordIndex = record.ParentGroup.FilteredRecords.IndexOf(record);
                                if (recordIndex % 2 == 0)
                                {
                                    e.TableCellType = GridTableCellType.RecordRowHeaderCell;
                                }
                                else
                                {
                                    e.TableCellType = GridTableCellType.AlternateRecordRowHeaderCell;
                                }
                            }
                            else if (colIndex <= groupedColumnsCount)
                            {
                                e.TableCellType = GridTableCellType.GroupIndentICell;
                                e.GroupedColumn = this.TableDescriptor.GroupedColumns[colIndex - 1];
                            }
                            else if (colIndex == groupedColumnsCount + 1 && this.TableDescriptor.Relations.NestedCount > 0)
                            {
                                if (!((GridTable)displayElement.ParentTable).TableOptions.ShowRecordPlusMinus)
                                {
                                    colIndex++;
                                    goto case DisplayElementKind.Record;
                                }

                                e.TableCellType = GridTableCellType.RecordPlusMinusCell;
                            }
                            else
                            {
                                e.Column = GetColumnDescriptorAt(displayElement, colIndex);

                                if (e.Column == null)
                                {
                                    e.TableCellType = GridTableCellType.EmptyCell;
                                }
                                else
                                {
                                    Record record = Record.GetParentRecord(displayElement);
                                    int recordIndex = record.ParentGroup.FilteredRecords.IndexOf(record);
                                    if (recordIndex % 2 == 0)
                                    {
                                        e.TableCellType = GridTableCellType.RecordFieldCell;
                                    }
                                    else
                                    {
                                        e.TableCellType = GridTableCellType.AlternateRecordFieldCell;
                                    }
                                }
                            }

                            break;
                    }

                    if (e.TableCellType == GridTableCellType.GroupIndentICell || e.TableCellType == GridTableCellType.GroupIndentTCell)
                    {
                        Group g = displayElement.ParentGroup;
                        int startCol = e.ColIndex;
                        int groupLevel = displayElement.GroupLevel;

                        if (e.DisplayElement.Kind == DisplayElementKind.AddNewRecord)
                        {
                            if (g.Sections.IndexOf(displayElement.ParentSection) > g.Sections.IndexOf(g.Details))
                            {
                                groupLevel++;
                            }
                        }

                        if (startCol > 0 && startCol <= groupLevel)
                        {
                            Group group = displayElement.ParentGroup;
                            if (group != null)
                            {
                                for (int n = startCol; n < displayElement.GroupLevel; n++)
                                {
                                    g = group;
                                    group = group.ParentGroup;
                                }

                                if (group != null && group.Groups.IndexOf(g) == group.Groups.Count - 1)
                                {
                                    if (e.TableCellType == GridTableCellType.GroupIndentTCell)
                                    {
                                        e.TableCellType = GridTableCellType.GroupIndentLCell;
                                    }
                                    else
                                    {
                                        e.TableCellType = GridTableCellType.GroupIndentCell;
                                    }
                                }
                                else if (displayElement.Kind == DisplayElementKind.AddNewRecord
                                    && g.Sections.IndexOf(displayElement.ParentSection) > g.Sections.IndexOf(g.Details))
                                {
                                    if (!(colIndex < displayElement.GroupLevel))
                                    {
                                        e.TableCellType = GridTableCellType.GroupIndentCell;
                                    }
                                    ////TraceUtil.TraceCurrentMethodInfo(displayElement.Kind, colIndex, displayElement.GroupLevel, g.Info, g.GroupLevel, group.Info, group.GroupLevel);
                                    ////TraceUtil.TraceCurrentMethodInfo(g.Groups.Count, group.Groups.Count, group.ParentGroup.Groups.Count);
                                }
                            }
                        }
                    }

                    return e;
                }

                return null;
            }
        }

        /// <summary>
        /// Create a GridTableCellStyleInfo and fills in values based on the given GridTableCellStyleInfoIdentity. Call this 
        /// method if you want to manually setup the GridTableCellStyleInfoIdentity with its TableCellType
        /// and other cell-specific information and then fill its cell value, format etc. The cell
        /// does not have to be visible in the grid.
        /// </summary>
        /// <param name="tableCellIdentity">The <see cref="GridTableCellStyleInfoIdentity"/></param>
        /// <returns>A <see cref="GridTableCellStyleInfo"/>.</returns>
        public GridTableCellStyleInfo CreateTableCellStyle(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            GridTableCellStyleInfo style = new GridTableCellStyleInfo(tableCellIdentity);

            if (tableCellIdentity != null)
            {
                //// Suspend OnStyleChanged
                style.BeginInit();

                //// initialize style
                this.SetupStyleInfo(tableCellIdentity, style);

                //// raise event, give user a change to customize style settings
                GridTableCellStyleInfoEventArgs e = new GridTableCellStyleInfoEventArgs(tableCellIdentity, style, null);
                this.RaiseQueryCellStyleInfo(e);

                //// Resume OnStyleChanged
                style.EndInit();
            }

            return style;
        }

        internal void SetupStyleInfo(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            if (e == null)
            {
                return;
            }

            Element el = e.DisplayElement;

            switch (e.TableCellType)
            {
                case GridTableCellType.EmptyCell:
                    SetupEmptyCell(e, style);
                    break;

                case GridTableCellType.NestedTableRowHeaderCell:
                    SetupNestedTableRowHeaderCell(e, style);
                    break;

                case GridTableCellType.NestedTableCell:
                    SetupNestedTableCell(e, style);
                    break;

                case GridTableCellType.GroupIndentCell:
                case GridTableCellType.GroupIndentTCell:
                case GridTableCellType.GroupIndentICell:
                case GridTableCellType.GroupIndentLCell:
                    SetupGroupIndentCell(e, style);
                    break;

                case GridTableCellType.NestedTableIndentCell:
                    SetupTableIndentCell(e, style);
                    break;

                case GridTableCellType.GroupCaptionPlusMinusCell:
                    SetupGroupCaptionPlusMinusCell(e, style);
                    break;

                case GridTableCellType.GroupCaptionCell:
                    SetupGroupCaptionCell(e, style);
                    break;

                case GridTableCellType.RowHeaderCell:
                    break;

                case GridTableCellType.ColumnHeaderCell:
                    SetupColumnHeaderCell(e, style);
                    break;

                case GridTableCellType.StackedHeaderCell:
                    SetupStackedHeaderCell(e, style);
                    break;

                case GridTableCellType.FilterBarCell:
                    SetupFilterBarCell(e, style);
                    break;

                case GridTableCellType.AddNewRecordFieldCell:
                    SetupAddNewRecordFieldCell(e, style);
                    break;

                case GridTableCellType.SummaryFillRowCell:
                    SetupSummaryFillRowCell(e, style);
                    break;

                case GridTableCellType.GroupCaptionSummaryCell:
                case GridTableCellType.SummaryFieldCell:
                    SetupSummaryFieldCell(e, style);
                    break;

                case GridTableCellType.SummaryEmptyCell:
                    SetupEmptySummaryCell(e, style);
                    break;

                case GridTableCellType.SummaryTitleCell:
                    SetupSummaryTitleCell(e, style);
                    break;

                case GridTableCellType.RecordPlusMinusCell:
                    SetupRecordPlusMinusCell(e, style);
                    break;

                case GridTableCellType.AlternateRecordFieldCell:
                case GridTableCellType.RecordFieldCell:
                    SetupRecordFieldCell(e, style);
                    break;
            }
        }

        void SetupGroupCaptionCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            CaptionSection cs = CaptionSection.GetCaptionSection(el);
            ////                                                object cat = cs.ParentGroup.Category;
            ////                                                if (cat == null)
            ////                                                                cat = "(null)";
            ////                                                string caption = "{2} Item(s)";
            ////                                                string groupName = cs.ParentGroup.Name;
            ////                                                if (groupName != "")
            ////                                                                caption = "{0}: {1} - " + caption;
            IGridGroupOptionsSource g = cs.ParentGroup as IGridGroupOptionsSource;
            string captionText;
            if (g != null)
            {
                captionText = g.GroupOptions.CaptionText;
            }
            else
            {
                captionText = "{CategoryCaption}: {Category} - {RecordCount} Items";
            }

            style.CellValue = GetGroupCaptionDisplayText(cs, captionText); ////);..//. String.Format(caption, groupName, cat, cs.ParentGroup.GetFilteredRecordCount());
            style.ReadOnly = true;
        }

        internal string GetGroupCaptionDisplayText(CaptionSection cs, string format)
        {
            return GridEngine.GetGroupCaptionDisplayText(cs.ParentGroup, format);
        }

        void SetupColumnHeaderCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            style.CellValue = e.Column.HeaderText;

            //// Sort Indicator
            SortColumnDescriptor sd = null;
            if (TableDescriptor.GroupedColumns.Contains(e.Column.MappingName))
            {
                sd = TableDescriptor.GroupedColumns[e.Column.MappingName];
            }
            else
            {
                int index = TableDescriptor.SortedColumns.IndexOf(e.Column.MappingName);
                if (index != -1)
                {
                    sd = TableDescriptor.SortedColumns[e.Column.MappingName];
                    if (TableDescriptor.TableOptions.AllowMultiColumnSort && TableDescriptor.SortedColumns.Count > 1)
                    {
                        style.ValueMember = index.ToString();
                    }
                }
            }

            if (sd != null)
            {
                style.Tag = sd.SortDirection;
            }

            style.ReadOnly = true;
        }

        void SetupStackedHeaderCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;

            style.CellValue = e.StackedHeader.HeaderText;
            style.ReadOnly = true;
        }

        void SetupFilterBarCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            int fieldNum;
            if (e.ColIndex <= TableDescriptor.GroupedColumns.Count + 1)
            {
                fieldNum = 0;
            }
            else
            {
                fieldNum = e.ColIndex - TableDescriptor.GroupedColumns.Count - 1;
            }

            style.CellValue = null;
            GridGroupingControl gc = Engine != null ? Engine.ParentControl : null;
            if(gc.OptimizeFilterPerformance ||
                (!TableDescriptor.HasCustomSummaryFilterBarChoices && e.FilterBarSummaryDescriptor == null))
            {
                if (!e.Column.AllowFilter)
                {
                    style.CellType = "Static";
                    style.Enabled = false;
                }
            }
        }

        void SetupAddNewRecordFieldCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            GridColumnDescriptor column = e.Column;
            AddNewRecord record = Record.GetParentRecord(el) as AddNewRecord;
            SetupRecordField(record, column, style);
        }

        void SetupGroupIndentCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;

            if (TableOptions.ShowTreeLines)
            {
                if (e.ColIndex < el.GroupLevel || (e.DisplayElement.Kind == DisplayElementKind.AddNewRecord
                                && !e.DisplayElement.ParentGroup.Details.HasRecords))
                {
                    style.Borders.Right = GridBorder.Empty;
                }

                style.Borders.Bottom = GridBorder.Empty;
                style.Tag = TableOptions.TreeLineBorder;
            }
            else
            {
                style.CellValue = string.Empty;
                ////e.TableCellType = GridTableCellType.GroupIndentCell;
            }

            style.ReadOnly = true;
        }

        void SetupTableIndentCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            style.ReadOnly = true;
        }

        void SetupEmptyCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            style.ReadOnly = true;
        }

        void SetupNestedTableRowHeaderCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            style.ReadOnly = true;
        }

        void SetupNestedTableCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            style.CellType = GetNestedTableCellType((NestedTable)el);
        }

        void SetupGroupCaptionPlusMinusCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            style.Description = el.ParentGroup.IsExpanded ? "-" : "+";
            style.ReadOnly = true;
#if ASPNET
#else
            if (this._tableDescriptor.TableOptions.GridVisualStyles != GridVisualStyles.SystemTheme)
            {
                style.BorderMargins = new GridMarginsInfo(2, 2, 2, 2);
            }
#endif
        }

        void SetupRecordFieldCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            Record record = Record.GetParentRecord(el);
            GridColumnDescriptor column = e.Column;

            SetupRecordField(record, column, style);
        }

        void SetupRecordField(Record record, GridColumnDescriptor column, GridTableCellStyleInfo style)
        {
            FieldDescriptor fd = column.FieldDescriptor;
            bool readOnly = column.ReadOnly;
            if (fd != null)
            {
                ////FK_SUPPORT

                RelationDescriptor rd = fd.GetRelation();
                if (rd != null)
                {
                    rd.EnsureInitialized();
                }

                if (fd.IsRelatedField() && column.AllowDropDownCell)
                {
                    //// Identity inherits GridPropertyTypeDefaultStyleCollection.ForeignKeyCell style.CellType = "ForeignKeyCell";

                    if (rd.RelationKeys.Count > 0)
                    {
                        FieldDescriptor foreignKeyField = rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField;
                        FieldDescriptor displayField = fd.GetRelatedDescriptor();

                        //// Make it work with GridListControl cell
                        if (foreignKeyField != null)
                        {
                            style.CellValue = record.GetValue(foreignKeyField);
                            style.CellValueType = foreignKeyField.GetPropertyType();
                            style.PropertyDescriptor = null; ////foreignKeyField.GetPropertyDescriptor();

                            //// Moved datasource initialization code to GridTableDropDownListCellModel.GetDataSource.
                            ////                                                                                if (rd.RelationKeys.Count == 1)
                            ////                                                                                                style.DataSource = ((GridTable) RelatedTables[rd.ChildTableName]).TopLevelGroup.GroupTypedListRecords;
                            ////                                                                                else
                            ////                                                                                {
                            ////                                                                                                //// get child table ....
                            ////                                                                                                object[] values = new object[rd.RelationKeys.Count-1];
                            ////                                                                                                for (int n = 0; n < rd.RelationKeys.Count-1; n++)
                            ////                                                                                                                values[n] = record.GetValue(rd.RelationKeys[n].ParentKeyField);
                            ////                                                                                                int indexOf = ((GridTable) RelatedTables[rd.ChildTableName]).TopLevelGroup.Groups.FindGroup(values);
                            ////                                                                                                if (indexOf != -1)
                            ////                                                                                                {
                            ////                                                                                                                ChildTable ct = (ChildTable) ((GridTable) RelatedTables[rd.ChildTableName]).TopLevelGroup.Groups[indexOf];
                            ////                                                                                                                style.DataSource = ct.GroupTypedListRecords;
                            ////                                                                                                }
                            ////                                                                                }

                            style.ValueMember = rd.RelationKeys[rd.RelationKeys.Count - 1].ChildKeyFieldName;
                            style.DisplayMember = displayField.Name;
                            if (!column.ShouldSerializeReadOnly())
                            {
                                readOnly = foreignKeyField.ReadOnly;
                            }
                        }
                    }
                    else if (fd.IsComplexPropertyField())
                    {
                        FieldDescriptor parentField = fd.GetParentFieldDescriptor();
                        PropertyDescriptor pd = fd.GetPropertyDescriptor();
                        PropertyDescriptor parentPd = parentField.GetPropertyDescriptor();
                        style.PropertyDescriptor = null;
                        style.CellValue = record.GetValue(parentField);
                        //// TODO: Might need to check for nested properties here (e.g. a Photo inside a nested property)
                        style.CellValueType = parentPd.PropertyType;
                        style.ValueMember = "__data__";
                        style.DisplayMember = pd.Name;
                        if (!column.ShouldSerializeReadOnly())
                        {
                            readOnly = pd.IsReadOnly;
                        }
                    }
                    else
                    {
                        ////style.CellValue = rec
                    }
                }
                else
                {
                    object value = record.GetValue(fd);
                    //// Need to check for nested properties here (e.g. a Products_Categories_Photo)
                    if (fd.IsRelatedField() && fd.GetRelation().RelationKind != RelationKind.ForeignKeyKeyWords)
                    {
                        //// ForeignListItems
                        fd = fd.GetNestedRelatedDescriptor();
                    }
                    else
                    {
                        PropertyDescriptor pd = fd.GetPropertyDescriptor();
                        if (pd != null && pd.Converter != null && pd.Converter.CanConvertTo(typeof(string)) && pd.Converter.GetStandardValuesSupported())
                        {
                            ////                                                                                                                style.ValueMember = "";
                            ////                                                                                                                style.DisplayMember = fd.Name;
                            ////                                                                                                                if (pd.Converter.GetStandardValuesExclusive() && style.DropDownStyle = GridDropDownStyle.Editable)
                            ////                                                                                                                                style.DropDownStyle = GridDropDownStyle.AutoComplete;
                        }
                    }

                    Type type = null;
                    //// Give column.Appearance.AnyRecordFieldCell and column.Appearance.AnyCell precedence over fd.GetPropertyType()
                    if (column != null)
                    {
                        type = column.Appearance.AnyRecordFieldCell.CellValueType;
                    }

                    if (type == null)
                    {
                        type = fd.GetPropertyType();
                    }

                    if (type != null)
                    {
                        style.CellValueType = type;
                    }

                    if (value != null && type != null && value.GetType() != type)
                    {
                        System.Globalization.CultureInfo culture = style.GetCulture(true);
                        if (culture == null)
                        {
                            culture = Engine.Culture;
                        }

                        style.CellValue = GridCellValueConvert.ChangeType(value, type, culture, true);
                    }
                    else
                    {
                        style.CellValue = value;
                    }

                    if (fd.GetPropertyDescriptor() != null)
                    {
                        style.PropertyDescriptor = fd.GetPropertyDescriptor();
                    }
                }
            }

            readOnly |= !this.SourceListAllowEdit;
            readOnly |= !TableDescriptor.AllowEdit;
            style.ReadOnly |= readOnly;
        }

        void SetupSummaryTitleCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            GridSummaryRow sr = el as GridSummaryRow;
            if (sr != null)
            {
                style.CellValue = sr.SummaryRowDescriptor.Title;
            }

            //// maybe use floating cells here?
            style.ReadOnly = true;
        }

        void SetupRecordPlusMinusCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            Record record = Record.GetParentRecord(el);
            if (record.HasNestedTables && el is RecordRow)
            {
                e.TableCellType = GridTableCellType.RecordPlusMinusCell;
                if (record.RecordRows.IndexOf((RecordRow)el) == 0)
                {
                    style.Description = record.IsExpanded ? "-" : "+";
#if ASPNET
#else
                    if (this._tableDescriptor.TableOptions.GridVisualStyles != GridVisualStyles.SystemTheme)
                    {
                        style.BorderMargins = new GridMarginsInfo(2, 2, 2, 2);
                    }
#endif
                }
            }
        }

        void SetupEmptySummaryCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            style.ReadOnly = true;
        }

        void SetupSummaryFieldCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            if (e.SummaryColumn != null)
            {
                Element el = e.DisplayElement;
                ISummary summary = e.SummaryColumn.GetSummary(this, el);
                style.CellValue = e.SummaryColumn.GetDisplayText(summary, el.ParentGroup.PassThroughItem);
                ////if (summary != null)
                ////    style.CellTipText = summary.ToString();
                style.ReadOnly = true;
            }
        }

        void SetupSummaryFillRowCell(GridTableCellStyleInfoIdentity e, GridTableCellStyleInfo style)
        {
            Element el = e.DisplayElement;
            GridSummaryRow sr = el as GridSummaryRow;
            if (sr != null)
            {
                foreach (GridSummaryColumnDescriptor sc in sr.SummaryRowDescriptor.SummaryColumns)
                {
                    if (sc.Style == GridSummaryStyle.FillRow)
                    {
                        ISummary summary = sc.GetSummary(this, sr);
                        style.CellValue = sc.GetDisplayText(summary, el.ParentGroup.PassThroughItem);

                        ////if (summary != null)
                        ////{
                        ////    style.CellTipText = summary.ToString();
                        ////}
                        break;
                    }
                }
            }
            ////style.Borders.Right = TableCellStyle.Borders.Right;
            style.ReadOnly = true;
        }

        #endregion

        #region QueryCellStyleInfo Event
        /// <summary>
        /// Occurs for each cell before a <see cref="GridTableControl"/>
        /// starts painting and lets users customize the display of cells.
        /// </summary>
        public event GridTableCellStyleInfoEventHandler QueryCellStyleInfo;

        /// <summary>
        /// Raises the <see cref="QueryCellStyleInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellStyleInfo(GridTableCellStyleInfoEventArgs e)
        {
            if (QueryCellStyleInfo != null)
            {
                QueryCellStyleInfo(this, e);
            }
        }
        
        /// <summary>
        /// Raises the <see cref="QueryCellStyleInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridTableCellStyleInfoEventArgs" /> that contains the event data.</param>
        internal void RaiseQueryCellStyleInfo(GridTableCellStyleInfoEventArgs e)
        {
            OnQueryCellStyleInfo(e);

            if (TableDescriptor != null)
            {
                TableDescriptor.RaiseQueryCellStyleInfo(e);
            }
        }

        #endregion

        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
                }

                return appearance;
            }

            set
            {
                if (value != null)
                {
                    Appearance.InitializeFrom(value);
                }
                else
                {
                    ResetAppearance();
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="Appearance"/> has been modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeAppearance()
        {
            return appearance != null && appearance.IsModified;
        }

        /// <summary>
        /// Discards any changes for the <see cref="Appearance"/> object.
        /// </summary>
        public void ResetAppearance()
        {
            if (appearance != null && appearance.IsModified)
            {
                appearance.Reset();
            }
        }

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion

        #region IGridTableOptionsSource Members

        ////                                [System.Xml.Serialization.XmlIgnore]
        ////                                [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        ////                                [Browsable(false)]
        bool IGridTableOptionsSource.HasTableOptions
        {
            get
            {
                return ShouldSerializeTableOptions();
            }
        }

        /// <summary>
        /// Lets you set table-wide properties like the width of the indent column, or whether header rows should be visible.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridTableOptionsStyleInfo TableOptions
        {
            get
            {
                return TableDescriptor.TableOptions;
                ////                                                                if (tableOptions == null)
                ////                                                                                tableOptions = new GridTableOptionsStyleInfo(new GridTableOptionsStyleInfoIdentity(this));
                ////                                                                return tableOptions;
            }

            set
            {
                TableOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// Determines whether <see cref="TableOptions"/> were modified
        /// and contents should be serialized at design-time.
        /// </summary>
        /// <returns>True if contents were changed; False otherwise.</returns>
        public bool ShouldSerializeTableOptions()
        {
            return false; ////TableDescriptor.ShouldSerializeTableOptions();
            ////return this.tableOptions != null && !tableOptions.IsEmpty;
        }

        /// <summary>
        /// Discards any changes for the <see cref="TableOptions"/> object.
        /// </summary>
        public void ResetTableOptions()
        {
            TableOptions = GridTableOptionsStyleInfo.Empty;
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanged(GridTableOptionsChangedEventArgs e)
        {
            if (this.Engine != null)
            {
                ((IGridTableOptionsSource)Engine).RaiseTableOptionsChanged(e);
            }

            this.RaiseDisplayElementChanged(this, -1, -1, true, true, false);
        }

        void IGridTableOptionsSource.RaiseTableOptionsChanging(GridTableOptionsChangedEventArgs e)
        {
            if (this.Engine != null)
            {
                ((IGridTableOptionsSource)Engine).RaiseTableOptionsChanging(e);
            }

            this.RaiseDisplayElementChanging(this, -1, 0, true, true, false);
        }

        IGridTableOptionsSource IGridTableOptionsSource.GetParentTableOptionsSource()
        {
            return this.TableDescriptor;
        }

        #endregion

        #region Helper Methods
        int rangeOfColumnHeaderSectionVersion = -1;
        GridRangeInfo rangeOfColumnHeaderSection = null;
        internal GridRangeInfo GetRangeOfColumnHeaderSection()
        {
            if (rangeOfColumnHeaderSectionVersion != Engine.Version)
            {
                int rowIndex = 0;
                int firstRowIndex = -1;
                int lastRowIndex = -1;
                rangeOfColumnHeaderSectionVersion = Engine.Version;
                foreach (Element el in TopLevelGroup.DisplayElements)
                {
                    if (el is ColumnHeaderRow || el is ColumnHeaderSection)
                    {
                        if (firstRowIndex == -1)
                        {
                            firstRowIndex = rowIndex;
                        }

                        lastRowIndex = rowIndex;
                        break;
                    }

                    rowIndex++;
                }

                if (firstRowIndex == -1)
                {
                    rangeOfColumnHeaderSection = GridRangeInfo.Empty;
                }
                else
                {
                    rangeOfColumnHeaderSection = GridRangeInfo.Rows(firstRowIndex, lastRowIndex);
                }
            }

            return rangeOfColumnHeaderSection;
        }

        int rangeOfStackedHeaderSectionVersion = -1;
        GridRangeInfo rangeOfStackedHeaderSection = null;
        internal GridRangeInfo GetRangeOfStackedHeaderSection()
        {
            if (rangeOfStackedHeaderSectionVersion != Engine.Version)
            {
                int rowIndex = 0;
                int firstRowIndex = -1;
                int lastRowIndex = -1;
                rangeOfStackedHeaderSectionVersion = Engine.Version;
                foreach (Element el in TopLevelGroup.DisplayElements)
                {
                    if (el is GridStackedHeaderRow || el is GridStackedHeaderSection)
                    {
                        if (firstRowIndex == -1)
                        {
                            firstRowIndex = rowIndex;
                        }

                        lastRowIndex = rowIndex;
                        break;
                    }

                    rowIndex++;
                }

                if (firstRowIndex == -1)
                {
                    rangeOfStackedHeaderSection = GridRangeInfo.Empty;
                }
                else
                {
                    rangeOfStackedHeaderSection = GridRangeInfo.Rows(firstRowIndex, lastRowIndex);
                }
            }

            return rangeOfStackedHeaderSection;
        }

        internal GridRangeInfo GetRangeOfHeaderColumnDescriptor(GridColumnDescriptor column)
        {
            int firstHeaderRowIndex = GetRangeOfColumnHeaderSection().Top;
            int resultRow;
            int resultCol;
            if (TableDescriptor.ColumnToRowColIndex(column.MappingName, out resultRow, out resultCol))
            {
                int offset;
                int d = (ParentTableDescriptor.Relations.NestedCount > 0) ? 1 : 0;
                if (resultCol > 0)
                {
                    offset = TableDescriptor.GroupedColumns.Count + d + 1;
                }
                else
                {
                    offset = 1;
                }

                return GridRangeInfo.Cell(firstHeaderRowIndex + resultRow, offset + resultCol);
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>returns GridRangeInfo</returns>
        /// <exclude/>
        public GridRangeInfo GetRangeOfStackedHeaderSpan(GridStackedHeaderSpan column)
        {
            int firstHeaderRowIndex = GetRangeOfStackedHeaderSection().Top;
            int resultRow;
            int resultCol;

            GridStackedHeaderRowDescriptor parentRow = column.parentRow;
            resultRow = TableDescriptor.StackedHeaderRows.IndexOf(parentRow);
            resultCol = column.firstCol;
            int offset = GetColumnIndentCount();

            resultRow = firstHeaderRowIndex + resultRow;
            return GridRangeInfo.Cells(resultRow, offset + resultCol, resultRow, column.lastCol + GetColumnIndentCount());
        }

        bool IsTopAddNewRecord(Element el)
        {
            return el is AddNewRecord && el.GroupLevel == 0;
        }

        int GetColumnIndentCount()
        {
            return TableDescriptor.GetColumnIndentCount();
        }

        #endregion

        #region BROWSING
#if ASPNET
                                public void MoveFirst()
                                {
                                                Group recParent = this.TopLevelGroup;
                                                while(recParent.Groups.Count > 0)
                                                                recParent = recParent.Groups[0];
                                                if(recParent.Records.Count > 0)
                                                                this.CurrentRecordManager.NavigateTo(recParent.Records[0], false);
                                }
                                public void MoveLast()
                                {
                                                Group g = this.TopLevelGroup;
                                                while(g.Groups.Count > 0)
                                                                g = g.Groups[g.Groups.Count - 1];
                                                Record r = g.Records[g.Records.Count - 1];
                                                this.CurrentRecordManager.NavigateTo(r, false);
                                }
#endif
        #endregion
        #region Repaint Element Queue
#if ASPNET
                                internal void RepaintElement(Element element){/*Do nothing in ASPNET*/}
                                public void ResetRepaintElementsInQueue(){/*Do nothing in ASPNET*/}
#else
        internal ArrayList repaintElementQueue = new ArrayList();

        private void element_Disposed(object sender, EventArgs e)
        {
            ((Element)sender).Disposed -= new EventHandler(element_Disposed);
            this.repaintElementQueue.Remove(sender);
        }

        internal void RepaintElement(Element element)
        {
            ////SS                                                if (element is NestedTable && Control.ModifierKeys == Keys.Control)
            ////SS                                                                System.Diagnostics.Debugger.Break();
            repaintElementQueue.Add(element);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void AddElementToRepaintQueue(Element element)
        {
            ////SS                                                if (element is NestedTable && Control.ModifierKeys == Keys.Control)
            ////SS                                                                System.Diagnostics.Debugger.Break();
            repaintElementQueue.Add(element);
            element.Disposed += new EventHandler(element_Disposed);
        }

        /// <summary>
        /// Clears the internal repaint elements queue. Call this method if you made changes to the table
        /// and you experience an exception triggered by a RepaintElementsInQueue method call. The method
        /// lets you work around any issues if elements are in the queue that have just been deleted from
        /// the table.
        /// </summary>
        public void ResetRepaintElementsInQueue()
        {
            ArrayList repaintElements = new ArrayList(repaintElementQueue);
            repaintElementQueue.Clear();

            foreach (Element el in repaintElements)
            {
                if (el != null)
                {
                    el.Disposed -= new EventHandler(element_Disposed);
                }
            }

            repaintElements.Clear();
        }

        internal void RepaintElementsInQueue()
        {
            GridTableControl mainTableControl = Engine.TableControl;
            Table engineTable = Engine.Table;
            if (mainTableControl != null)
            {
                ArrayList repaintElements = new ArrayList(repaintElementQueue);

                foreach (Element el in repaintElements)
                {
                    Element element = FixVirtualMode(el);

                    if (element != null && element.ParentTable != null && this.DisplayElements.Contains(element) && element.GetVisibleInHierarchy())
                    {
                        if (Engine.SupportsYAmount)
                        {
                            double yAmountPos;
                            if (element is NestedTable)
                            {
                                yAmountPos = engineTable.NestedDisplayElements.GetYAmountPositionOf(element);
                            }
                            else
                            {
                                yAmountPos = engineTable.DisplayElements.GetYAmountPositionOf(element);
                            }

                            double yAmountCount = element.GetYAmountCount();
                            int scrollPos = mainTableControl.VScrollBar.Value;
                            int scrollMin = mainTableControl.VScrollBar.Minimum;

                            if (yAmountPos > scrollPos)
                            {
                                yAmountPos = yAmountPos - scrollPos + scrollMin;
                            }
                            else if (yAmountPos > scrollMin)
                            {
                                if (yAmountPos + yAmountCount > scrollPos)
                                {
                                    yAmountCount = scrollPos - yAmountPos;
                                    yAmountPos = scrollMin;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            Rectangle r = new Rectangle(0, (int)yAmountPos, mainTableControl.Width, (int)yAmountCount);
                            mainTableControl.Invalidate(r);
                        }
                        else
                        {
                            int yAmountPos = engineTable.NestedDisplayElements.IndexOf(element);
                            int yAmountCount = element.GetVisibleCount();
                            int scrollPos = mainTableControl.VScrollBar.Value;
                            int scrollMin = mainTableControl.VScrollBar.Minimum;

                            if (yAmountPos > scrollPos)
                            {
                                yAmountPos = yAmountPos - scrollPos + scrollMin;
                            }
                            else if (yAmountPos > scrollMin)
                            {
                                if (yAmountPos + yAmountCount > scrollPos)
                                {
                                    yAmountCount = scrollPos - yAmountPos;
                                    yAmountPos = scrollMin;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            Rectangle r = mainTableControl.RangeInfoToRectangle(GridRangeInfo.Rows(yAmountPos, yAmountPos + yAmountCount - 1));
                            mainTableControl.Invalidate(r);
                        }
                    }
                }

                ResetRepaintElementsInQueue();
                repaintElements.Clear();
            }
        }
#endif
        #endregion
        
        /// <exclude/>
        protected override void Engine_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "TableDescriptor")
            {
                TableDescriptor tableDescriptor = ((Engine)sender).TableDescriptor;
                e = (DescriptorPropertyChangedEventArgs)e.Inner;

                if (e.PropertyName == "Relations")
                {
                    e = e.GetNestedChildTableDescriptorEvent(ref tableDescriptor);
                }

                if (e.PropertyName == "Columns")
                {
                    ListPropertyChangedEventArgs le = (ListPropertyChangedEventArgs)e.Inner;
                    if (le.Action == ListPropertyChangedType.ItemPropertyChanged)
                    {
                        ////if (le.Property == "Width")
                        if (le.Property == "Appearance" || le.Property == "Width"
                                        || le.Property == "ReadOnly" || le.Property == "HeaderText"
                                        || le.Action == ListPropertyChangedType.Remove
                                        || le.Action == ListPropertyChangedType.Move
                                        || le.Property == "AllowFilter")
                        {
                            return;
                        }
                    }
                }
            }
            else if (e.PropertyName == "Appearance")
            {
                //// Base class will end edit mode, which is not necessary.
                return;
            }

            base.Engine_PropertyChanging(sender, e);
        }
        
        /// <summary>
        /// Returns the relative row index in the RowElements collection if displayElement is a RecordRow or ColumnHeader row.
        /// (A record can have column sets with multiple rows per record)
        /// </summary>
        /// <param name="rowElement">The row element</param>
        /// <returns>The relative row index of the element.</returns>
        public int GetRelativeRowIndex(RowElement rowElement)
        {
            int relativeRow = 0;

            Element parent = rowElement.ParentElement;

            if (rowElement is RecordRow)
            {
                relativeRow = ((GridRecordRowsPart)parent).RowElements.IndexOf(rowElement);
            }
            else if (parent is RowElementsSection)
            {
                relativeRow = ((RowElementsSection)parent).RowElements.IndexOf(rowElement);
            }
            else if (parent is IRowElementsContainer)
            {
                relativeRow = ((IRowElementsContainer)parent).RowElements.IndexOf(rowElement);
            }

            return relativeRow;
        }
        
        /// <summary>
        /// Returns the style information with TableCellIdentity for the given element and x-coordinate.
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="xPosition">The x position in client coordinate.</param>
        /// <returns>The style element with identity information for the cell that is displayed at the specified coordinate.</returns>
        /// <remarks>
        /// <seealso cref="GridTableControl.PointToTableCellStyle"/>
        /// <seealso cref="GridTableControl.PointToNestedDisplayElement"/>
        /// </remarks>
        public GridTableCellStyleInfo GetTableCellStyle(Element element, int xPosition)
        {
            //// Get the tabledescriptor associated with that element
            GridTableDescriptor td = (GridTableDescriptor)element.ParentTableDescriptor;
            GridTable table = (GridTable)element.ParentTable;

            //// Calculate nested indent for this table
            int indentWidth = table.GetTotalWidthOfRowHeadersAndIndent(true);

            if (indentWidth > xPosition)
            {
                //// the mouse is over a row header or indent.
                return table.GetIndentTableCellStyle(element.ParentChildTable, element, xPosition);
            }

            GridTable mainTable = Engine.Table;

            int relativeRow = element is RowElement ? GetRelativeRowIndex((RowElement)element) : 0;

            //// Get GridColumnDescriptor and column index (column index for the nested table)
            GridColumnDescriptor[,] recordRowColumns = td.RecordRowColumns;

            int colIndex = -1;
            int currentX = indentWidth;
            GridColumnDescriptor column = null;

            if (currentX <= xPosition)
            {
                //// Check for column
                for (int n = 0; n < recordRowColumns.GetLength(1); n++)
                {
                    column = recordRowColumns[relativeRow, n];
                    if (column.Width + currentX > xPosition)
                    {
                        colIndex = n;
                        break;
                    }

                    currentX += column.Width;
                }

                //// Get the style
                if (colIndex != -1)
                {
                    return table.GetTableCellStyle(element, column.MappingName);
                }
                else
                {
                    return table.GetIndentTableCellStyle(element.ParentChildTable, element, xPosition);
                }
            }

            return null;
        }

        GridTableCellStyleInfo GetIndentTableCellStyle(ChildTable childTable, Element element, int xPosition)
        {
            int indentWidth = 0;

            GridTable parentTable = RelationParentTable;

            if (parentTable != null)
            {
                indentWidth = parentTable.GetTotalWidthOfRowHeadersAndIndent(true);

                if (indentWidth > xPosition)
                {
                    //// the mouse is over a row header or indent.
                    return parentTable.GetIndentTableCellStyle(childTable.ParentNestedTable.ParentChildTable, element, xPosition);
                }
            }

            int width = indentWidth;
            int columnIndentCount = GetColumnIndentCount();
            int rowIndex = 0;
            if (childTable != null)
                rowIndex = childTable.DisplayElements.IndexOf(element);

            width += this.DefaultRowHeaderWidth;
            if (xPosition < width)
            {
                return GetTableCellStyle(rowIndex, 0);
            }

            for (int n = 0; n < this.TableDescriptor.GroupedColumns.Count; n++)
            {
                width += DefaultIndentWidth;
                if (xPosition < width)
                {
                    return GetTableCellStyle(rowIndex, n + 1);
                }
            }

            if (this.TableDescriptor.Relations.NestedCount > 0)
            {
                width += this.DefaultTableIndentWidth;
                if (xPosition < width)
                {
                    return GetTableCellStyle(rowIndex, TableDescriptor.GroupedColumns.Count + 1);
                }
            }

            return GetTableCellStyle(rowIndex, TableDescriptor.GetColCount()); // empty cell after all columns
        }

        Hashtable summaryColumnFields = null;

        /// <override/>
        protected override void OnEnsureSortFields()
        {
            // Calculate all summary columns dependant on fields. Use
            // summaryColumnFields collection later to get a list of summary columns
            // that need to be updated when value of field was changed.
            GridTableDescriptor td = TableDescriptor;
            summaryColumnFields = new Hashtable();
            foreach (GridSummaryRowDescriptor summaryRow in td.SummaryRows)
            {
                foreach (GridSummaryColumnDescriptor summaryColumn in summaryRow.SummaryColumns)
                {
                    int fieldIndex = td.Fields.IndexOf(summaryColumn.DataMember);
                    GridSummaryColumnDescriptorCollection al;
                    if (summaryColumnFields.Contains(fieldIndex))
                    {
                        al = summaryColumnFields[fieldIndex] as GridSummaryColumnDescriptorCollection;
                }
                    else
                    {
                        al = new GridSummaryColumnDescriptorCollection();
                        summaryColumnFields[fieldIndex] = al;
                    }

                    al.InnerAdd(summaryColumn.Clone()); //// Important: Do noy use .Add method here so that columns parentrow does not get reset.
                }
            }
        }

        /// <override/>
        protected override void OnPrepareRemoving(object row)
        {
            TableDescriptor td = TableDescriptor;
            foreach (int index in this.summaryColumnFields.Keys)
            {
                if (index == -1)
                {
                    continue; //// index is -1 for RecordCount summary...
                }

                FieldDescriptor fd = td.Fields[index];
                if (fd == null || fd.IsPropertyField())
                {
                    PropertyDescriptor pd = fd.GetPropertyDescriptor();
                    object value = GetValue(row, pd);

                    ChangedFieldInfo ci = new ChangedFieldInfo(td, pd.Name, value, null);
                    this.AddChangedField(ci);
                }
            }

            base.OnPrepareRemoving(row);
        }

        /// <summary>
        /// Returns an array of GridSummaryColumnDescriptor objects that are affected by
        /// changes to the field and need to be redrawn.
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>An array of summary column descriptors.</returns>
        public GridSummaryColumnDescriptorCollection GetSummaryColumnCollection(int fieldIndex)
        {
            if (summaryColumnFields.Contains(fieldIndex))
            {
                return summaryColumnFields[fieldIndex] as GridSummaryColumnDescriptorCollection;
            }

            return new GridSummaryColumnDescriptorCollection();
        }
    }

    /// <summary>
    /// Represents the method that handles a <see cref="GridTable.QueryCoveredRange"/> event which can be
    /// marked as handled.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridTableQueryCoveredRangeEventArgs"/> that contains the event data.</param>
    public delegate void GridTableQueryCoveredRangeEventHandler(object sender, GridTableQueryCoveredRangeEventArgs e);

    /// <summary>
    /// Provides data about the <see cref="GridTable.QueryCoveredRange"/> event which can be marked as handled.
    /// </summary>
    /// <remarks>
    /// GridTableQueryCoveredRangeEventArgs is a custom event argument class used by the
    /// <see cref="GridTable.QueryCoveredRange"/> event to query information about
    /// covered cells at a specified cell.
    /// <para/>
    /// This event allows you to specify covered ranges at run-time, e.g when you have
    /// a large grid with repeating patterns of covered ranges. If the specified row and
    /// column index is part of a covered cell's range, you should assign the coordinates
    /// of the covered cell to <see href="GridTableQueryCoveredRangeEventArgs.Range"/> and
    /// set <see cref="SyncfusionHandledEventArgs.Handled"/> to True.
    /// <para/>
    /// <see cref="SyncfusionHandledEventArgs.Handled"/> indicates that you supplied data
    /// from your event handler and no further querying for data about covered range information
    /// for this cell is necessary.
    /// <para/>
    /// See the VirtualGrid sample for an example of how to use this event.
    /// </remarks>
    /// <seealso cref="GridTableQueryCoveredRangeEventHandler"/>
    /// <seealso cref="GridTable.QueryCoveredRange"/>
    public class GridTableQueryCoveredRangeEventArgs : GridQueryCoveredRangeEventArgs
    {
        GridTable table;

        /// <overload>
        /// Initalizes a new object.
        /// </overload>
        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="table">The <see cref="GridTable"/></param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridTableQueryCoveredRangeEventArgs(GridTable table, int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
            this.table = table;
        }

        /// <summary>
        /// Initalizes a new object.
        /// </summary>
        /// <param name="table">The <see cref="GridTable"/></param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">A <see cref="GridRangeInfo"/> that will receive the resulting range for the covered cell.</param>
        public GridTableQueryCoveredRangeEventArgs(GridTable table, int rowIndex, int colIndex, GridRangeInfo range)
            : base(rowIndex, colIndex, range)
        {
            this.table = table;
        }

        /// <summary>
        /// The Grid table
        /// </summary>
        public GridTable Table
        {
            get
            {
                return table;
            }
        }
    }
}