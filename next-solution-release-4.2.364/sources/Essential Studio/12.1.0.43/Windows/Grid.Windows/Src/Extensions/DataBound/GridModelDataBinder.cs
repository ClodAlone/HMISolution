//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelDataBinder.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Reflection;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Defines the default behavior when the grid is notified from the underlying IBindingList
    /// that the data for the current record are changed.
    /// </summary>
    public enum GridCurrentRecordItemChangedBehavior
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Discard any changes for the current record and display the modified data
        /// into the current record.
        /// </summary>
        ReloadCurrentRecord = 1,

        /// <summary>
        /// Keep changes that were made for the current record and keep
        /// the current cell's control text and mark the
        /// current record as modified (show pencil in row header).
        /// </summary>
        KeepCurrentRecord = 2,

        /// <summary>
        /// Keep the text of the current cell, mark current record as modified
        /// (show pencil in row header) when text of current cell differs.
        /// </summary>
        KeepCurrentCellText = 4,
    }

    /// <summary>
    /// Implements a DataObject consumer for text data. Will handle data provided in DataFormats.Text and DataFormats.UnicodeText format.
    /// </summary>
    public class GridDataBoundGridTextDataObjectConsumer : GridSubComponent, IGridDataObjectConsumer
    {
        GridControlBase grid;

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundGridTextDataObjectConsumer"/> object and associates it with a <see cref="GridControlBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> this object should be associated with.</param>
        public GridDataBoundGridTextDataObjectConsumer(GridControlBase grid)
            : base(grid)
        {
            this.grid = grid;
        }

        /// <summary>
        /// Gets the name of the DataObject consumer.
        /// </summary>
        public string Name
        {
            get { return "DataBoundGridText"; }
        }

        /// <summary>
        /// Queries the DataObject consumer if it knows how to handle the IDataObject.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="consumer">Another consumer that is capable of reading the data. Might be NULL.</param>
        /// <param name="options">Reserved for future use.</param>
        /// <returns>True if this consumer is able to read the data from <paramref name="dataObject"/>.</returns>
        public bool QueryAcceptData(IDataObject dataObject, IGridDataObjectConsumer consumer, GridQueryAcceptDataOptions options)
        {
            if (!GridUtil.IsSet(grid.Model.Options.DragDropDropTargetFlags, GridDragDropFlags.Text))
            {
                return false;
            }

            if (!grid.Model.IgnoreReadOnly && grid.Model.ReadOnly)
            {
                return false;
            }

            // Let other consumers have higher precedence than this one.
            return consumer == null && (dataObject.GetDataPresent(DataFormats.Text) || dataObject.GetDataPresent(DataFormats.UnicodeText));
        }

        /// <summary>
        /// Queries the dimension in rows and columns of the data object.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>returns Dimension.</returns>
        public Size DetermineRowColCount(IDataObject dataObject)
        {
            int rowCount = 0;
            int colCount = 0;

            string buffer = null;
            if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                buffer = dataObject.GetData(DataFormats.UnicodeText) as string;
            }
            else if (dataObject.GetDataPresent(DataFormats.Text))
            {
                buffer = dataObject.GetData(DataFormats.Text) as string;
            }

            if (buffer != null)
            {
                grid.Model.TextDataExchange.CalcBufferDimension(buffer, out rowCount, out colCount);
            }

            return new Size(colCount, rowCount);
        }

        /// <summary>
        /// Paste the contents of the data object at the specified cell coordinates.
        /// </summary>
        /// <param name="dataObject">Provides data to be consumed.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>True if successful; False otherwise.</returns>
        public bool DropAtRowCol(IDataObject dataObject, int rowIndex, int colIndex)
        {
            GridModelDataBinder binder = grid.Model.DataProvider as GridModelDataBinder;
            if (binder != null)
            {
                string buffer = null;
                if (dataObject.GetDataPresent(DataFormats.UnicodeText))
                {
                    buffer = dataObject.GetData(DataFormats.UnicodeText) as string;
                }
                else if (dataObject.GetDataPresent(DataFormats.Text))
                {
                    buffer = dataObject.GetData(DataFormats.Text) as string;
                }

                if (buffer != null)
                {
                    GridRangeInfo range = GridRangeInfo.Cell(rowIndex, colIndex);
                    GridRangeInfoList rl = new GridRangeInfoList();
                    rl.Add(range);
                    binder.DataBoundPaste(dataObject, grid.Model.Options.DragDropDropTargetFlags, rl, null);
                }
            }

            return false;
        }
    }

    /// <summary>
    /// This is a helper class for the <see cref="GridDataBoundGrid"/> that manages access to the
    /// data source.
    /// </summary>
    /// <remarks>
    /// <see cref="GridDataBoundGrid"/> implements the <see cref="IGridModelDataProvider"/>
    /// interface that lets it catch <see cref="GridModel.QueryRowCount"/>, <see cref="GridModel.QueryColCount"/>,
    /// <see cref="GridModel.QueryCellInfo"/>, and <see cref="GridModel.SaveCellInfo"/> events
    /// in one place.
    /// <para/>
    /// The methods in this interface are called before the named events are raised and thus
    /// give you a chance to control the event's behavior before other subscribers can handle it.
    /// <para/>
    /// You access the <see cref="GridModelDataBinder"/> instance with the <see cref="GridModel.DataProvider"/>
    /// property of the <see cref="GridModel"/>.
    /// </remarks>
    public class GridModelDataBinder : NonFinalizeDisposable, IGridModelDataProvider, ICurrencyManagerSource
    {
        #region Fields
        internal GridModel gridModel;

        internal GridSortBehavior sortBehavior = GridSortBehavior.DoubleClick;

        bool inSetListManager = false;
        bool metaDataChanged = false;
        internal CurrencyManager listManager;
        IList list = null;
        private object dataSource;
        private string dataMember;

        bool rowDirty = false;
        IEditableObject editableItem = null;
        object currentItem = null;
        bool inAddNew = false;
        bool inEdit = false;
        bool inRemoveRecords = false;
        bool inRowChanged = false;

        ArrayList listManagerToGridPositions = new ArrayList();
        ArrayList recordStates = new ArrayList();
        internal ArrayList levels = new ArrayList(1);
        ArrayList rowHeaderStates = new ArrayList();
        internal int expRecordCount = 0;

        bool directSaveCellInfo = false;
        bool enableRemove = true;
        bool enableEdit = true;
        bool enableAddNew = true;
        internal bool outlineHeaderField = false;

        BindingContext bindingContext;

        string errorValue = string.Empty;
        string errorMessage = string.Empty;
        Exception exception = null;
        bool hasError = false;

        int currentPosition = 0;
        bool inSetCurrentPosition = false;
        internal GridBoundRecordState currentRecordState = null;
        GridHierarchyLevel currentHierarchyLevel = null;
        GridBoundColumnsCollection currentColumns = null;
        int currentListManagerPosition = -1;
        int savedPosition = -1;

        ISite site = null;
        bool isDesignMode;

        static bool autoInitCellTypes = true;
        static bool doNotDisposeLists = false;
        #endregion

        internal int viewCount = 0;
        private bool useComplexBinding;
        bool bindToCurrencyManager = true;

        /// <summary>
        /// Gets or sets a value indicating whether list should be attached to <see cref="CurrencyManager"/>
        /// or if you would like the engine to be detached from a CurrencyManager. (Default is True)
        /// </summary>
        [DefaultValue(true)]
        public bool BindToCurrencyManager
        {
            get
            {
                return this.bindToCurrencyManager; //// && !this.isBindingSource;
            }

            set
            {
                this.bindToCurrencyManager = value;
            }
        }

        [Category("Grid"), Browsable(false), DefaultValue(false)]
        internal bool UseComplexBinding
        {
            get
            {
                return this.useComplexBinding;
            }
            set
            {
                this.useComplexBinding = value;
            }
        }



        #region Initialize
        /// <summary>
        /// Initializes a new <see cref="GridModelDataBinder"/> and associates it with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">The <see cref="GridModel"/>this object should be associated with.</param>
        public GridModelDataBinder(GridModel model)
        {
            this.gridModel = model;
            model.QueryCellModel += new GridQueryCellModelEventHandler(ModelQueryCellModel);
            model.QueryCoveredRange += new GridQueryCoveredRangeEventHandler(ModelQueryCoveredRange);
            model.CommandStack.Enabled = false;
            model.Rows.DefaultSize = 17;
            model.Cols.DefaultSize = 65;
            model.RowHeights[0] = 25;
            model.ColWidths[0] = 35;
            model.Options.ExcelLikeCurrentCell = false;
            model.Options.ExcelLikeSelectionFrame = false;
            model.Options.AllowDragSelectedCols = true;
            model.Options.AllowDragSelectedRows = false;
            model.Options.FloatCellsMode = GridFloatCellsMode.None;
            model.Options.NumberedRowHeaders = false;
            model.Options.NumberedColHeaders = false;
            model.Options.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.GrayWhenLostFocus;
            model.Options.DataObjectConsumerOptions = GridDataObjectConsumerOptions.None;
            model.Options.DragDropDropTargetFlags &= ~GridDragDropFlags.Styles;
            model.Properties.MarkRowHeader = false;
            model.Properties.MarkColHeader = false;

            model.CellModels.Add("RowHeaderCell", new GridDataBoundRowHeaderCellModel(model));
            model.CellModels.Add("ColumnHeaderCell", new GridSortColumnHeaderCellModel(model));
            model.CellModels.Add("DataBoundRowExpandCell", new GridDataBoundRowExpandCellModel(model));

            InitBaseStyles();

            model.CommandStack.Enabled = false;
            model.Options.FloatCellsMode = GridFloatCellsMode.None;
            this.levels.Add(new GridHierarchyLevel(this, 0));
            this.gridColumns.CollectionChanged += new CollectionChangeEventHandler(GridColumnsCollectionChanged);
        }

        internal void ResetBaseStyles()
        {
            GridModel model = this.gridModel;
            model.BaseStylesMap.RegisterStandardStyles();
            InitBaseStyles();
        }

        internal void InitBaseStyles()
        {
            GridModel model = this.gridModel;
            if (!model.BaseStylesMap.Modified)
            {
                GridStyleInfo style = model.BaseStylesMap["Row Header"].StyleInfo;
                style.CellType = "RowHeaderCell";
                style.Enabled = true;
                style = model.BaseStylesMap["Column Header"].StyleInfo;
                style.Enabled = false;
                style.CellType = "ColumnHeaderCell";
                style.Font.Bold = false;
                style = model.BaseStylesMap["Standard"].StyleInfo;
                style.CheckBoxOptions.CheckedValue = "True";
                style.CheckBoxOptions.UncheckedValue = "False";
                model.BaseStylesMap.Modified = false;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.gridColumns != null)
                {
                    foreach (GridBoundColumn col in gridColumns)
                    {
                        if (col != null)
                        {
                            col.Dispose();
                        }
                    }

                    this.gridColumns.CollectionChanged -= new CollectionChangeEventHandler(GridColumnsCollectionChanged);
                    this.gridColumns.Clear();
                }

                this.UnWireDataSource();
                listManager = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid should set <see cref="GridStyleInfo.CellType"/> automatically
        /// for columns that have dates or boolean values.
        /// </summary>
        [Browsable(false)]
        public static bool AutoInitCellTypes
        {
            get
            {
                return autoInitCellTypes;
            }

            set
            {
                autoInitCellTypes = value;
            }
        }
        #endregion
        #region Columns
        /// <summary>
        /// Converts a column index in a grid to a zero-based field number adjusted for column headers. The resulting
        /// field number can be used as an index to look up a <see cref="GridBoundColumn"/> in the <see cref="InternalColumns"/>
        /// collection.
        /// </summary>
        /// <param name="colIndex">The column index in the grid.</param>
        /// <returns>A zero-based field number in the datasource.</returns>
        public int ColIndexToField(int colIndex)
        {
            return Math.Max(colIndex - gridModel.Cols.HeaderCount, 0) - 1;
        }

        /// <summary>
        /// Converts a zero-based field number to a column index in a grid adjusted for column headers.
        /// </summary>
        /// <param name="fieldNum">A zero-based field number in the datasource.</param>
        /// <returns>The column index in the grid.</returns>
        public int FieldToColIndex(int fieldNum)
        {
            if (fieldNum < 0)
            {
                return -1;
            }

            return fieldNum + gridModel.Cols.HeaderCount + 1;
        }

        /// <summary>
        /// Returns the zero-based field number for a column that matches a given name. The resulting
        /// field number can be used as an index to look up a <see cref="GridBoundColumn"/> in the <see cref="InternalColumns"/>
        /// collection.
        /// </summary>
        /// <param name="name">The name of the field to be matched.</param>
        /// <returns>A zero-based field number in the datasource; -1 if not found.</returns>
        /// <remarks>
        /// This function only searches the columns in the root level. If you have several relations
        /// displayed in the grid, the nested relations will not be searched by this function.
        /// </remarks>
        public int NameToField(string name)
        {
            GridBoundColumnsCollection columns = this.InternalColumns;
            for (int fieldNum = 0; fieldNum < columns.Count; fieldNum++)
            {
                GridBoundColumn columnStyle = columns[fieldNum];
                if ((columnStyle.MappingName != null && columnStyle.MappingName == name)
                    || (columnStyle.PropertyDescriptor != null && columnStyle.PropertyDescriptor.Name == name))
                {
                    return fieldNum;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the column index for a column that matches a given name.
        /// Returns the zero-based field number for a column that matches a given name. The resulting
        /// field number can be used as an index for <see cref="GridModel.this[int,int]"/>.
        /// </summary>
        /// <param name="name">The name of the field to be matched.</param>
        /// <returns>The column index in the grid; -1 if not found.</returns>
        /// <remarks>
        /// This function only searches the columns in the root level. If you have several relations
        /// displayed in the grid, the nested relations will not be searched by this function.
        /// </remarks>
        public int NameToColIndex(string name)
        {
            int fieldNum = NameToField(name);
            if (fieldNum != -1)
            {
                return FieldToColIndex(fieldNum);
            }

            return -1;
        }

        /// <summary>
        /// Returns the row index for a row that matches a given name.
        /// </summary>
        /// <param name="name">The name of the row to be matched.</param>
        /// <returns>The row index in the grid; -1 if not found.</returns>
        public int NameToRowIndex(string name)
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

        /// <summary>
        /// Recreates the internal columns collection or loops through the existing <see cref="GridBoundColumns"/>
        /// collection and reinitializes columns with the correct <see cref="GridBoundColumn.PropertyDescriptor"/> based on
        /// the <see cref="GridBoundColumn.MappingName"/>.
        /// </summary>
        /// <remarks>
        /// This method is automatically called when a <see cref="DataSource"/> is assigned to the grid.
        /// </remarks>
        public void InitializeColumns()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            RootHierarchyLevel._InitializeColumns(this.List);
        }

        /// <summary>
        /// Rearranges how columns are displayed in the grid and allows you to specify covered cells
        /// and / or break records into several rows displayed in the grid.
        /// </summary>
        /// <param name="mappingNames">A string array with field names. <para/>
        /// The following strings have a specific meaning: <para/>
        /// "-" specifies a covered cell.<para/>
        /// "." indicates line break inside the record. Subsequent fields will be displayed in another row.<para/>
        /// "" specifies an empty "whitespace" column.<para/>
        /// Other that these, you should use the same mapping names that you also use with <see cref="GridBoundColumn"/> objects.
        /// </param>
        /// <example>
        /// See the "MultiRowRecord" and "ExpandGrid" examples for sample code.
        /// </example>
        public void LayoutColumns(params string[] mappingNames)
        {
            RootHierarchyLevel.LayoutColumns(mappingNames);
        }
        #endregion
        #region Relations
        /// <summary>
        /// Gets the <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the root list.
        /// </summary>
        public GridHierarchyLevel RootHierarchyLevel
        {
            get
            {
                return (GridHierarchyLevel)this.levels[0];
            }
        }

        /// <summary>
        /// Gets the number of hierarchies displayed in this grid. A regular data source without
        /// nested relations will have a <see cref="HierarchyLevelCount"/> of 1.
        /// </summary>
        public int HierarchyLevelCount
        {
            get
            {
                return this.levels.Count;
            }
        }

        /// <summary>
        /// Returns the <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the specified relation.
        /// </summary>
        /// <param name="name">The name of the relation. This should be the same name you used
        /// for adding the relation with <see cref="AddRelation"/>.</param>
        /// <returns>The <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the specified relation.</returns>
        public GridHierarchyLevel GetHierarchyLevel(string name)
        {
            for (int n = 0; n < this.levels.Count; n++)
            {
                GridHierarchyLevel ghl = (GridHierarchyLevel)this.levels[n];
                if (ghl.relation.Name == name)
                {
                    return ghl;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Returns the <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the specified relation.
        /// </summary>
        /// <param name="index">The zero-based hierarchy level for this relation.
        /// </param>
        /// <returns>The <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the specified relation.</returns>
        public GridHierarchyLevel GetHierarchyLevel(int index)
        {
            if (index >= levels.Count || index < 0)
            {
                return null;
            }

            GridHierarchyLevel ghl = (GridHierarchyLevel)this.levels[index];
            return ghl;
        }

        /// <summary>
        /// Adds a <see cref="GridHierarchyLevel"/> to the grid for a specified relation. If you are referring
        /// to a relation within a <see cref="DataSet"/>, the name of the
        /// relation should be the same name you used when adding the relation to <see cref="DataSet.Relations"/>
        /// of <see cref="DataSet"/>.
        /// </summary>
        /// <param name="nestedDataMember">The name of the relation. This can be the name of a <see cref="DataRelation"/>
        /// or the name of a <see cref="PropertyDescriptor"/> that references any <see cref="IList"/>.</param>
        /// <returns>The <see cref="GridHierarchyLevel"/> with information about columns displayed
        /// for the newly added relation.</returns>
        public GridHierarchyLevel AddRelation(string nestedDataMember)
        {
            GridHierarchyLevel ghl = this.levels[this.levels.Count - 1] as GridHierarchyLevel;
            return ghl.AddRelation(nestedDataMember);
        }

        /// <summary>
        /// Gets the number of relations displayed in the grid.
        /// </summary>
        public int RelationCount
        {
            get
            {
                return this.levels.Count - 1;
            }
        }
        #endregion
        #region AddNewEditRemove
        /// <summary>
        /// Gets a value indicating whether a datasource supports adding new records.
        /// </summary>
        public bool AllowAddNew
        {
            get
            {
                if (List == null || !this.enableAddNew)
                {
                    return false;
                }

                IList list = List;
                if (list != null && !list.IsFixedSize && !list.IsReadOnly)
                {
                    IBindingList bindingList = list as IBindingList;
                    // CurrencyManager only supports AddNew functionality when
                    // list is a IBindingList. Otherwise, CurrencyManager.AddNew
                    // throws a NotSupportedException.
                    return bindingList != null && bindingList.AllowNew;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether to support add new. Use AllowAddNew instead.
        /// </summary>
        public bool SupportsAddNew
        {
            get
            {
                return AllowAddNew;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a datasource supports editing records.
        /// </summary>
        public bool AllowEdit
        {
            get
            {
                IList list = List;
                if (list == null || !this.enableEdit)
                {
                    return false;
                }

                if (list != null && !list.IsReadOnly)
                {
                    IBindingList bindingList = list as IBindingList;
                    return bindingList == null || bindingList.AllowEdit;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to save cell values directly into the datasource without moving the current
        /// record and without calling BeginEdit or EndEdit.
        /// </summary>
        /// <remarks>
        /// The clipboard paste and clipboard deleted functions temporarily raise this
        /// flag when pasting several rows.
        /// </remarks>
        public bool DirectSaveCellInfo
        {
            get
            {
                return directSaveCellInfo;
            }

            set
            {
                directSaveCellInfo = value;
            }
        }

        bool allowSetValueOnCurrentItem = true;

        /// <summary>
        /// Gets or sets a value indicating whether the datasource is a DataView and the user edits the contents of a cell
        /// in the current record, the value will also be set in the underlying record.
        /// If this is a problem for, e.g. if you want to be able to modify contents of the
        /// current record from outside after it was switched into edit mode, you can set
        /// this AllowSetValueOnCurrentItem to false.
        /// </summary>
        public bool AllowSetValueOnCurrentItem
        {
            get
            {
                return allowSetValueOnCurrentItem;
            }

            set
            {
                allowSetValueOnCurrentItem = value;
            }
        }

        #endregion
        #region RowIndexRecordMapping
        /// <summary>
        /// Gets the number of records in the datasource. If you are displaying a grid with nested relations, the
        /// only the record count for the root data source is displayed ignoring any expanded nodes.
        /// </summary>
        public int RecordCount
        {
            get
            {
                if (List != null)
                {
                    return List.Count;
                }

                return 0;
            }
        }

        static Type boundColumnsCollectionType = typeof(GridBoundColumnsCollection);

        /// <summary>
        /// Gets or sets if you want to use a class derived from <see cref="GridBoundColumn"/>, you should also derive
        /// a <see cref="GridBoundColumnsCollection"/> and set <see cref="BoundColumnsCollectionType"/>.
        /// </summary>
        /// <example>
        /// This sample demonstrates how to implement a derived CustomGridColumn:
        /// <para/>
        /// <coderef file="D:\sfgrid\Forum\myboundcolumn\CustomGridColumn.cs" name="CustomGridColumn" lang="C#"><code lang="C#">
        ///     /// <summary>
        ///     /// A specialization of the GridBoundColumn class with additional
        ///     /// functionality.
        ///     /// </summary>
        ///     public class CustomGridColumn : GridBoundColumn, ICloneable
        ///     {
        ///        private int width;
        ///        private int widthPercentage;
        /// <para/>
        ///        /// <summary>
        ///        /// Default constructor.
        ///        /// </summary>
        ///         public CustomGridColumn()
        ///         {
        ///         }
        /// <para/>
        ///        public int Width
        ///        {
        ///            get { return width; }
        ///            set { width = value; }
        ///        }
        /// <para/>
        ///        public int WidthPercentage
        ///        {
        ///            get { return widthPercentage; }
        ///            set { widthPercentage = value; }
        ///        }
        /// <para/>
        ///         public CustomGridColumn(PropertyDescriptor prop)
        ///             : base(prop)
        ///         {
        ///         }
        /// <para/>
        ///         object ICloneable.Clone()
        ///         {
        ///             return base.Clone();
        ///         }
        /// <para/>
        /// <para/>
        ///     }
        /// <para/>
        ///     [ListBindableAttribute(false)]
        ///     [Editor(typeof(GridBoundColumnsCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        ///     public class CustomGridColumnsCollection : GridBoundColumnsCollection, ICloneable
        ///     {
        ///         public CustomGridColumnsCollection(GridModelDataBinder table)
        ///             : base(table)
        ///         {
        ///         }
        /// <para/>
        ///         public override GridBoundColumn CreateBoundColumn(PropertyDescriptor pd)
        ///         {
        ///             return new CustomGridColumn(pd);
        ///         }
        /// <para/>
        /// <para/>
        ///         /// <summary>
        ///         /// Creates a new CustomGridColumnsCollection and creates copies of all members in this collection.
        ///         /// </summary>
        ///         /// <returns>A CustomGridColumnsCollection object.</returns>
        ///         public override object Clone()
        ///         {
        ///             GridModelDataBinder owner = null;
        ///             owner = this.Owner as GridModelDataBinder;
        ///             CustomGridColumnsCollection clone = new CustomGridColumnsCollection(owner);
        ///             foreach (GridBoundColumn item in this)
        ///                 clone.Add((GridBoundColumn) item); //.Clone());
        ///             return clone;
        ///         }
        /// <para/>
        ///         /// <overload>
        ///         /// Gets a specified CustomGridColumn in the CustomGridColumnsCollection.
        ///         /// </overload>
        ///         /// <summary>
        ///         /// Gets a specified CustomGridColumn in the CustomGridColumnsCollection.
        ///         /// </summary>
        ///         public new CustomGridColumn this[int index]
        ///         {
        ///             get
        ///             {
        ///                 return (CustomGridColumn) base[index];
        ///             }
        ///         }
        /// <para/>
        ///         /// <summary>
        ///         /// Gets a specified CustomGridColumn in the CustomGridColumnsCollection.
        ///         /// </summary>
        ///         public new CustomGridColumn this[string columnName]
        ///         {
        ///             get
        ///             {
        ///                 return (CustomGridColumn) base[columnName];
        ///             }
        ///         }
        /// <para/>
        ///         /// <summary>
        ///         /// Gets a specified CustomGridColumn in the CustomGridColumnsCollection.
        ///         /// </summary>
        ///         public new CustomGridColumn this[PropertyDescriptor propDesc]
        ///         {
        ///             get
        ///             {
        ///                 return (CustomGridColumn) base[propDesc];
        ///             }
        ///         }
        ///     }</code></coderef>
        /// <para/>
        /// <coderef file="D:\sfgrid\Forum\myboundcolumn\SyncFusionBoundGrid.cs" name="SyncFusionBoundGrid" lang="C#"><code lang="C#">
        ///     public class SyncFusionBoundGrid : GridDataBoundGrid
        ///     {
        ///        /// <summary>
        ///        /// Default constructor.
        ///        /// </summary>
        ///         public SyncFusionBoundGrid() : base()
        ///        {
        ///             GridModelDataBinder.BoundColumnsCollectionType = typeof(CustomGridColumnsCollection);
        ///        }
        /// <para/>
        ///        /// <summary>
        ///        /// A collection of grid bound columns. This property is overridden so
        ///        /// that the type used to define a grid bound column is a specialized
        ///        /// version.
        ///        /// </summary>
        ///        [ Browsable(false), DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Hidden ) ]
        ///        public override GridBoundColumnsCollection GridBoundColumns
        ///        {
        ///            get
        ///            {
        ///                return base.GridBoundColumns;
        ///            }
        ///            set
        ///            {
        /// <para/>
        ///                base.GridBoundColumns = value;
        ///            }
        ///        }
        /// <para/>
        ///         /// <summary>
        ///         /// A collection of grid bound columns. This property is overridden so
        ///         /// that the type used to define a grid bound column is a specialized
        ///         /// version.
        ///         /// </summary>
        ///         [ DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Content ) ]
        ///         [ LocalizableAttribute(true) ]
        ///         [ Description( "Manages the columns to be displayed in the GridDataBoundGrid." ) ]
        ///         [ Category( "Data" ) ]
        ///         [ RefreshProperties( RefreshProperties.All ) ]
        ///         [ Editor( typeof(GridBoundColumnsCollectionEditor), typeof(UITypeEditor) ) ]
        ///         public CustomGridColumnsCollection CustomGridColumns
        ///         {
        ///             get
        ///             {
        ///                 return base.GridBoundColumns as CustomGridColumnsCollection;
        ///             }
        ///             set
        ///             {
        /// <para/>
        ///                 base.GridBoundColumns = value;
        ///             }
        ///         }
        ///     }</code></coderef>
        /// <para/>
        /// </example>
        public static Type BoundColumnsCollectionType
        {
            get
            {
                return boundColumnsCollectionType;
            }

            set
            {
                boundColumnsCollectionType = value;
            }
        }

        /// <summary>
        /// Instantiates a <see cref="GridBoundColumnsCollection"/> that holds the collection of <see cref="GridBoundColumn"/>
        /// objects. If you want to use a class derived from the <see cref="GridBoundColumn"/> you should also derive
        /// a <see cref="GridBoundColumnsCollection"/> and set <see cref="BoundColumnsCollectionType"/>. See <see cref="GridModelDataBinder.BoundColumnsCollectionType"/> for a sample.
        /// </summary>
        /// <returns>An empty <see cref="GridBoundColumnsCollection"/>.</returns>
        public virtual GridBoundColumnsCollection CreateBoundColumnsCollection()
        {
            return (GridBoundColumnsCollection)Activator.CreateInstance(boundColumnsCollectionType, new object[] { this });
        }

        /// <summary>
        /// Converts an absolute row index in a grid to a zero-based position in the same grid adjusted for row headers. The first
        /// grid position is the 0-based index starting with the first non-header row in the grid.
        /// </summary>
        /// <param name="rowIndex">The absolute row index in the grid.</param>
        /// <returns>The zero-based position in the grid adjusted for row headers.</returns>
        /// <remarks>
        /// If you have a grid with 3 row headers:<para/>
        /// RowIndexToPosition(4) will return 1.<para/>
        /// RowIndexToPosition(3) will return 0.<para/>
        /// RowIndexToPosition(2) will return -1.<para/>
        /// RowIndexToPosition(1) will return -1.<para/>
        /// The row index and the grid position do not depend on <see cref="RelationCount"/> and number of rows per record.
        /// </remarks>
        public int RowIndexToPosition(int rowIndex)
        {
            return Math.Max(rowIndex - gridModel.Rows.HeaderCount, 0) - 1;
        }

        /// <summary>
        /// Converts a zero-based position in the grid to an absolute row index in the grid.
        /// </summary>
        /// <param name="gridPosition">A zero-based position in the grid. The first
        /// position is the 0-based index starting with the first non-header row in the grid.</param>
        /// <returns>The row index in the grid.</returns>
        /// <remarks>
        /// If you have a grid with 3 row headers:<para/>
        /// PositionToRowIndex(-1) will throw exception.<para/>
        /// PositionToRowIndex(0) will return 3.<para/>
        /// PositionToRowIndex(1) will return 4.<para/>
        /// </remarks>
        public int PositionToRowIndex(int gridPosition)
        {
            return gridPosition + gridModel.Rows.HeaderCount + 1;
        }

        /// <summary>
        /// Converts an absolute row index in a grid to a zero-based record index of the datasource displayed in the grid.
        /// </summary>
        /// <param name="rowIndex">The absolute row index in the grid.</param>
        /// <returns>Zero-based record index of the data source displayed in the grid.</returns>
        /// <remarks>
        /// The returned value depends on the number of rows per record and the current expansion state of
        /// hierarchy levels in the grid.<para/>
        /// Only the list manager position for the outer hierarchy is returned.
        /// </remarks>
        public int RowIndexToListManagerPosition(int rowIndex)
        {
            int gridPosition = RowIndexToPosition(rowIndex);
            if (gridPosition < 0)
            {
                return -1;
            }

            return PositionToListManagerPosition(gridPosition);
        }

        /// <summary>
        /// Converts a zero-based position of the data source displayed in the grid to an absolute row index in the grid.
        /// </summary>
        /// <param name="record">A zero-based record in the data source displayed in the grid. </param>
        /// <returns>The row index in the grid. </returns>
        /// <remarks>
        /// The returned value depends on the number of rows per record and the current expansion state of
        /// hierarchy levels in the grid.<para/>
        /// Only the list manager position for the outer hierarchy is accepted as input.
        /// </remarks>
        public int ListManagerPositionToRowIndex(int record)
        {
            int gridPosition = this.ListManagerPositionToPosition(record);
            if (gridPosition < 0)
            {
                return -1;
            }

            return this.PositionToRowIndex(gridPosition);
        }

        #region RecordState
        /// <summary>
        /// Returns the <see cref="GridBoundRecordState"/> for the specified absolute row index with information about
        /// the hierarchy level and the record displayed at the specified row.
        /// </summary>
        /// <param name="rowIndex">The absolute row index in the grid.</param>
        /// <returns>A <see cref="GridBoundRecordState"/> with information about
        /// the hierarchy level and the record displayed at the specified row.</returns>
        public GridBoundRecordState GetRecordStateAtRowIndex(int rowIndex)
        {
            if (rowIndex > gridModel.Rows.HeaderCount)
            {
                int gridPosition = this.RowIndexToPosition(rowIndex);
                return GetRecordStateAtPosition(gridPosition);
            }
            else if (rowIndex < this.rowHeaderStates.Count)
            {
                return GetRowHeaderState(rowIndex);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if both rows represent the same record of the underlying data source.
        /// </summary>
        /// <param name="rowIndex1">The first row index.</param>
        /// <param name="rowIndex2">The second row index.</param>
        /// <returns>True if both rows show the same record of the underlying data source.</returns>
        /// <remarks>
        /// Normally different rows will represent different records in the data source. However,
        /// you can break one record into several rows with <see cref="LayoutColumns"/>. In that
        /// case, several rows can represent the same record in the datasource.
        /// </remarks>
        public bool RecordEqualsAtRowIndex(int rowIndex1, int rowIndex2)
        {
            return !this.RecordDiffersAtRowIndex(rowIndex1, rowIndex2);
        }

        ////        ///// <summary>
        ////        ///// Checks if relation is expanded at the specified row in the grid.
        ////        ///// </summary>
        ////        ///// <param name="rowIndex">The absolute row index.</param>
        ////        internal bool IsExpandedAtRowIndex(int rowIndex)
        ////        {
        ////            int gridPosition = this.RowIndexToPosition(rowIndex);
        ////            if (gridPosition < 0)
        ////                return false;
        ////
        ////            return IsExpanded(gridPosition);
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Expands the relation at the specified row in the grid without sending
        ////        ///// RowExpanding / RowExpanded events.
        ////        ///// </summary>
        ////        ///// <param name="rowIndex">The absolute row index.</param>
        ////        internal void ExpandAtRowIndex(int rowIndex)
        ////        {
        ////            int gridPosition = this.RowIndexToPosition(rowIndex);
        ////            if (gridPosition < 0)
        ////                return;
        ////
        ////            ExpandRecord(gridPosition);
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Expands the relation at the specified row in the grid without sending
        ////        ///// RowCollapsing / RowCollapsed events.
        ////        ///// </summary>
        ////        ///// <param name="rowIndex">The absolute row index.</param>
        ////        internal void CollapseAtRowIndex(int rowIndex)
        ////        {
        ////            int gridPosition = this.RowIndexToPosition(rowIndex);
        ////            if (gridPosition < 0)
        ////                return;
        ////
        ////            CollapseRecord(gridPosition);
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Expands all nodes in the grid without raising RowExpanding / RowExpanded events.
        ////        ///// </summary>
        ////        ///// <remarks>
        ////        ///// Expanding all nodes can be a lengthy process. The grid will give feedback
        ////        ///// through an <see cref="OperationFeedback"/> object about the progress
        ////        ///// of the operation and gives the user the option to abort.
        ////        ///// </remarks>
        ////        internal void ExpandAll(bool raiseEvents)
        ////        {
        ////            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
        ////
        ////            this.CancelEdit();
        ////            if (gridModel.ActiveGridView != null)
        ////                gridModel.ActiveGridView.CurrentCell.Deactivate(true);
        ////
        ////            OperationFeedback op = new OperationFeedback(gridModel);
        ////            op.Description = "Expand all records";
        ////            op.AllowCancel = true;
        ////            op.AllowRollback = false;
        ////
        ////            try
        ////            {
        ////                int recordCount = this.RecordCount * RootHierarchyLevel.rowCount;
        ////                int record = 0;
        ////                int levelOneRecord = 0;
        ////                float rootProgress = 0;
        ////
        ////                int position = 0;
        ////                while (position < recordCount + this.expRecordCount)
        ////                {
        ////                    GridBoundRecordState state = GetRecordStateAtPosition(position);
        ////                    if (state.row > 0)
        ////                    {
        ////                        position++;
        ////                        continue;
        ////                    }
        ////
        ////                    ExpandRecord(position);
        ////                    position++;
        ////
        ////                    //// Feedback and option to abort long operation.
        ////                    if (op.ShouldShowFeedback)
        ////                    {
        ////                        if (state.level == 0)
        ////                        {
        ////                            rootProgress = record*100f/recordCount;
        ////                            op.PercentComplete = Math.Min(100, (int) rootProgress);
        ////                            record++;
        ////                            levelOneRecord = 0;
        ////                        }
        ////                        else if (state.level == 1 && recordCount < 25)
        ////                        {
        ////                            int levelOneRecordCount = state.table != null ? state.table.Count : 0;
        ////                            //// let's get more detailed only if root level is not giving enough feedback
        ////                            float f2 = levelOneRecord*100f/(levelOneRecordCount*recordCount);
        ////                            op.PercentComplete = Math.Min(100, (int) (rootProgress + f2));
        ////                            levelOneRecord++;
        ////                        }
        ////                    }
        ////                    if (op.ShouldCancel)
        ////                        return;
        ////                }
        ////            }
        ////            finally
        ////            {
        ////                op.Close();
        ////                gridModel.EndUpdate(true);
        ////            }
        ////        }
        ////
        ////        ///// <summary>
        ////        ///// Collapses all nodes in the grid without sending
        ////        ///// RowCollapsing / RowCollapsed events.
        ////        ///// </summary>
        ////        ///// <remarks>
        ////        ///// Collapsing all nodes can be a lengthy process. The grid will give feedback
        ////        ///// through an <see cref="OperationFeedback"/> object about the progress
        ////        ///// of the operation and gives the user the option to abort.
        ////        ///// </remarks>
        ////        internal void CollapseAll()
        ////        {
        ////            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
        ////
        ////            if (gridModel.ActiveGridView != null)
        ////                gridModel.ActiveGridView.CurrentCell.Deactivate(true);
        ////
        ////            OperationFeedback op = new OperationFeedback(gridModel);
        ////            op.Description = "Collapse all records";
        ////            op.AllowCancel = true;
        ////            op.AllowRollback = false;
        ////
        ////            try
        ////            {
        ////                int count = this.RecordCount * RootHierarchyLevel.rowCount;
        ////                for (int n = 0; n < count; n++)
        ////                {
        ////                    CollapseRecord(n);
        ////                    //// Feedback and option to abort long operation
        ////                    if (op.ShouldShowFeedback)
        ////                        op.PercentComplete = Math.Min(100, n*100/count);
        ////                    if (op.ShouldCancel)
        ////                        return;
        ////                }
        ////            }
        ////            finally
        ////            {
        ////                op.Close();
        ////                gridModel.EndUpdate(true);
        ////            }
        ////        }
        #endregion
        #region Internal
        /// <summary>
        /// Returns the grid position for a zero-based record in list manager position.
        /// </summary>
        /// <param name="position">Zero-based record in list manager.</param>
        /// <returns>Zero-based grid position.</returns>
        internal int ListManagerPositionToPosition(int position)
        {
            if (ForgetAboutRecordState())
            {
                return position;
            }

            if (position < 0)
            {
                return -1;
            }

            if (position >= listManagerToGridPositions.Count)
            {
                return (position * RootHierarchyLevel.rowCount) + this.expRecordCount;
            }
            else
            {
                return (int)listManagerToGridPositions[position];
            }
        }

        /// <summary>
        /// Returns zero-based record in list manager for zero-based grid position.
        /// </summary>
        /// <param name="gridPosition">Zero-base grid position.</param>
        /// <returns>Zero-based record in list manager.</returns>
        internal int PositionToListManagerPosition(int gridPosition)
        {
            if (ForgetAboutRecordState())
            {
                return gridPosition;
            }

            GridBoundRecordState state = GetRecordStateAtPosition(gridPosition);
            if (state == null)
            {
                return -1;
            }

            while (state.parent != null)
            {
                state = state.parent;
            }

            return state.position;
        }

        internal bool RecordDiffersAtRowIndex(int rowIndex1, int rowIndex2)
        {
            if (rowIndex1 == rowIndex2)
            {
                return false;
            }

            GridBoundRecordState state1 = this.GetRecordStateAtRowIndex(rowIndex1);
            GridBoundRecordState state2 = this.GetRecordStateAtRowIndex(rowIndex2);

            if (state1 == null || state2 == null)
            {
                return false;
            }

            return !Object.ReferenceEquals(state1.table, state2.table)
                || state1.level != state2.level || state1.position != state2.position;
        }

        internal bool RecordDiffersAtPosition(int pos1, int pos2)
        {
            if (pos1 == pos2)
            {
                return false;
            }

            if (ForgetAboutRecordState())
            {
                return true;
            }

            GridBoundRecordState state1 = GetRecordStateAtPosition(pos1);
            GridBoundRecordState state2 = GetRecordStateAtPosition(pos2);

            if (state1 == null || state2 == null)
            {
                return false;
            }

            return !Object.ReferenceEquals(state1.table, state2.table)
                || state1.level != state2.level || state1.position != state2.position;
        }

        /// <summary>
        /// Call this if you have changed the underlying datasource and added several relations.
        /// </summary>
        public void ResetHierarchyLevels()
        {
            gridModel.SuspendChangeEvents();
            this.levels = new ArrayList();
            this.levels.Add(new GridHierarchyLevel(this, 0));
            gridModel.Data.Clear();
            gridModel.CoveredRanges.Clear();
            gridModel.Rows.HeaderCount = 0;
            gridModel.Rows.FrozenCount = 0;
            gridModel.Cols.HeaderCount = 0;
            gridModel.Cols.FrozenCount = 0;
            ResetRecordState();
            gridModel.ResumeChangeEvents();
        }

        internal void ResetRecordState()
        {
            for (int i = 0; i < recordStates.Count; i++)
            {
                int k = i;
                if (recordStates[k] is IDisposable)
                {
                    ((IDisposable)recordStates[k]).Dispose();
                }
            }

            recordStates.Clear();
            rowHeaderStates.Clear();
            listManagerToGridPositions.Clear();
            gridModel.ResetVolatileData();
        }

        internal GridBoundRecordState GetRecordStateAtPosition(int position)
        {
            if (position < 0)
            {
                return null;
            }
            else if (position < recordStates.Count)
            {
                return (GridBoundRecordState)recordStates[position];
            }
            else
            {
                if (ForgetAboutRecordState())
                {
                    GridBoundRecordState state = new GridBoundRecordState(null);
                    state.table = this.List;
                    state.listManager = this.listManager;
                    state.position = position;
                    state.childList = null;
                    state.hasChildList = false;
                    state.level = 0;
                    return state;
                }

                GridBoundRecordState lastState;
                if (recordStates.Count == 0)
                {
                    GridBoundRecordState state = new GridBoundRecordState(null);
                    state.table = this.List;
                    state.listManager = this.listManager;
                    state.expanded = false;
                    state.position = 0;
                    state.childList = null;
                    state.hasChildList = this.levels.Count > 1; ////this.relation != null;
                    state.level = 0;
                    recordStates.Add(state);
                    lastState = state;
                    expRecordCount = 0;
                }
                else
                {
                    lastState = (GridBoundRecordState)recordStates[recordStates.Count - 1];
                }

                GridHierarchyLevel ghl = RootHierarchyLevel;
                int newItemCount = position - recordStates.Count + 1;
                if (newItemCount > 0)
                {
                    GridBoundRecordState[] states = new GridBoundRecordState[newItemCount];
                    int gridPosition = position;
                    // need to fill up table up to requested position
                    for (int n = 0; n < newItemCount; n++)
                    {
                        GridBoundRecordState state = new GridBoundRecordState(null);
                        state.table = lastState.table;
                        state.listManager = lastState.listManager;
                        state.expanded = false;
                        if (lastState.row < ghl.rowCount - 1)
                        {
                            state.row = lastState.row + 1;
                            state.position = lastState.position;
                        }
                        else
                        {
                            state.row = 0;
                            state.position = lastState.position + 1;
                        }

                        state.childList = null;
                        state.hasChildList = this.levels.Count > 1; ////lastState.level < this.levels.Count-1;
                        state.level = 0;
                        states[n] = state;
                        lastState = state;
                    }

                    recordStates.AddRange(states);
                }

                return lastState;
            }
        }

        void EnsureListManagerToGridPositionsCount(int lmPosition)
        {
            if (ForgetAboutRecordState())
            {
                return;
            }

            if (this.listManagerToGridPositions.Count == 0)
            {
                this.listManagerToGridPositions.Add(0);
            }

            int lastPos = (int)listManagerToGridPositions[listManagerToGridPositions.Count - 1];
            while (listManagerToGridPositions.Count <= lmPosition)
            {
                this.listManagerToGridPositions.Add(++lastPos);
            }
        }

        void IncreaseListManagerToGridPositionsAt(int lmPosition, int count)
        {
            if (ForgetAboutRecordState())
            {
                return;
            }

            EnsureListManagerToGridPositionsCount(lmPosition);
            for (int n = lmPosition; n < this.listManagerToGridPositions.Count; n++)
            {
                this.listManagerToGridPositions[n] = (int)this.listManagerToGridPositions[n] + count;
            }
        }

        void ReduceListManagerToGridPositionsAt(int lmPosition, int count)
        {
            if (ForgetAboutRecordState())
            {
                return;
            }

            EnsureListManagerToGridPositionsCount(lmPosition);
            for (int n = lmPosition; n < this.listManagerToGridPositions.Count; n++)
            {
                this.listManagerToGridPositions[n] = (int)this.listManagerToGridPositions[n] - count;
            }
        }

        internal bool IsExpanded(int position)
        {
            if (ForgetAboutRecordState())
            {
                return false;
            }

            GridBoundRecordState state = GetRecordStateAtPosition(position);
            if (state.row > 0)
            {
                state = GetRecordStateAtPosition(position - state.row);
            }

            return state.expanded;
        }

        internal bool ForgetAboutRecordState()
        {
            return optimizeListChangedEvent && this.levels.Count == 1 && this.GetHierarchyLevel(0).RowCountPerRecord == 1;
        }

        bool optimizeListChangedEvent = false;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether OptimizeListChangedEvent. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool OptimizeListChangedEvent
        {
            get
            {
                return optimizeListChangedEvent;
            }

            set
            {
                optimizeListChangedEvent = value;
            }
        }

        bool forceUpdateAfterListChangedEvent = true;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether ForceUpdateAfterListChangedEvent. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public bool ForceUpdateAfterListChangedEvent
        {
            get
            {
                return forceUpdateAfterListChangedEvent;
            }

            set
            {
                forceUpdateAfterListChangedEvent = value;
            }
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public delegate IList GetChildListHandler(PropertyDescriptor relation, object component);

        /// <internalonly/>
        /// <summary>
        /// Used internally.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public GetChildListHandler childListHandler = null;

        IList GetChildList(PropertyDescriptor relation, object component)
        {
            IList childList = null;
            if (childListHandler != null)
            {
                childList = childListHandler(relation, component);
            }

            if (childList == null)
            {
                childList = relation.GetValue(component) as IList;
            }

            return childList;
        }

        internal void ExpandRecord(int gridPosition)
        {
            if (ForgetAboutRecordState())
            {
                return;
            }

            GridBoundRecordState state = GetRecordStateAtPosition(gridPosition);
            // Ensure next row is already there.
            GridBoundRecordState nextState = GetRecordStateAtPosition(gridPosition + 1);
            if (state == null || this.levels.Count <= 1 || state.expanded)
            {
                return;
            }

            if (!state.hasChildList)
            {
                return;
            }

            GridHierarchyLevel level = this.levels[state.level] as GridHierarchyLevel;

            if (state.position >= state.table.Count)
            {
                return;
            }

            IList childList = GetChildList(level.relation, state.table[state.position]);
            if (childList == null)
            {
                state.hasChildList = false;
                return;
            }

            int count = childList.Count;
            if (count == 0)
            {
                state.hasChildList = false;
                if (childList is IDisposable)
                {
                    ((IDisposable)childList).Dispose();
                }

                return;
            }

            GridHierarchyLevel childLevel = this.levels[state.level + 1] as GridHierarchyLevel;
            if (state.childList is IDisposable)
            {
                ((IDisposable)state.childList).Dispose();
            }

            state.childList = childList;
            BindingManagerBase childListManager = null;
            if (this.BindToCurrencyManager)
            {
                childListManager = this.BindingContext[childList];
            }

            GridBoundRecordState[] childStates = new GridBoundRecordState[count * childLevel.rowCount];
            int childIndex = 0;
            for (int n = 0; n < count; n++)
            {
                for (int row = 0; row < childLevel.rowCount; row++)
                {
                    GridBoundRecordState childState = new GridBoundRecordState(state);
                    childState.table = childList;
                    childState.listManager = childListManager;
                    childState.expanded = false;
                    childState.position = n;
                    childState.row = row;
                    childState.childList = null;
                    childState.hasChildList = childLevel.relation != null;
                    childState.level = childLevel.level;
                    childStates[childIndex++] = childState;
                }
            }

            int rowIndex = this.PositionToRowIndex(gridPosition + level.rowCount);
            gridModel.Rows.OnRangeInserting(new GridRangeInsertingEventArgs(rowIndex, count, null));
            int lmPosition = PositionToListManagerPosition(gridPosition);
            IncreaseListManagerToGridPositionsAt(lmPosition + 1, count * childLevel.rowCount);
            state.expanded = true;
            recordStates.InsertRange(gridPosition + level.rowCount, childStates);
            state.childCount = count;
            expRecordCount += count * childLevel.rowCount;
            gridModel.ResetVolatileData();
            gridModel.Rows.OnRangeInserted(new GridRangeInsertedEventArgs(rowIndex, count, null, true));
            DumpRecords();
        }

        internal void CollapseRecord(int gridPosition)
        {
            if (ForgetAboutRecordState())
            {
                return;
            }

            GridBoundRecordState state = GetRecordStateAtPosition(gridPosition);
            if (!state.expanded)
            {
                return;
            }

            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
            try
            {
                int count = state.childCount;
                GridHierarchyLevel level = this.levels[state.level] as GridHierarchyLevel;
                GridHierarchyLevel childLevel = this.levels[state.level + 1] as GridHierarchyLevel;
                int childRowCount = count * childLevel.rowCount;
                for (int n = 0; n < childRowCount; n++)
                {
                    CollapseRecord(n + gridPosition + level.rowCount);
                }

                int lmPosition = PositionToListManagerPosition(gridPosition);
                gridModel.Rows.OnRangeRemoving(new GridRangeRemovingEventArgs(this.PositionToRowIndex(gridPosition + level.rowCount), this.PositionToRowIndex(gridPosition + count + level.rowCount - 1)));
                expRecordCount -= count * childLevel.rowCount;

                for (int i = 0; i < count * childLevel.rowCount; i++)
                {
                    int k = gridPosition + level.rowCount + i;
                    if (recordStates[k] is IDisposable)
                    {
                        ((IDisposable)recordStates[k]).Dispose();
                    }
                }

                recordStates.RemoveRange(gridPosition + level.rowCount, count * childLevel.rowCount);
                ReduceListManagerToGridPositionsAt(lmPosition + 1, count * childLevel.rowCount);
                state.childList = null;
                state.expanded = false;
                state.childCount = 0;
                gridModel.ResetVolatileData();
                gridModel.Rows.OnRangeRemoved(new GridRangeRemovedEventArgs(this.PositionToRowIndex(gridPosition + level.rowCount), this.PositionToRowIndex(gridPosition + (count * childLevel.rowCount) + level.rowCount - 1), null, true));
                DumpRecords();
            }
            finally
            {
                gridModel.EndUpdate(true);
            }
        }

        internal void DumpRecords()
        {
            /*
            for (int n = 0; n < this.listManagerToGridPositions.Count; n++)
            {
                int gridPos = (int) listManagerToGridPositions[n];
                Trace.WriteLine(n.ToString() + ": " + gridPos.ToString());
            }
            return;
            for (int n = 0; n < this.recordStates.Count; n++)
            {
                GridBoundRecordState childState = (GridBoundRecordState) recordStates[n];
                Trace.WriteLine(childState.level.ToString() + ": " + childState.position.ToString());
            }
            */
        }

        internal int LastPosition
        {
            get
            {
                return (RecordCount * RootHierarchyLevel.rowCount) + this.expRecordCount;
            }
        }

        internal void SynchronizeColCount()
        {
            int colCount = 0;

            foreach (GridHierarchyLevel level in this.levels)
            {
                colCount = Math.Max(colCount, level.GetColCount());
            }

            if (this.levels.Count > 1)
            {
                colCount++; // 1st column is hierarchy checker.
                gridModel.ColWidths[1] = gridModel.Rows.DefaultSize;
                gridModel.Cols.FrozenCount = 1;
                gridModel.Cols.HeaderCount = 1;
            }

            gridModel.Data.ColCount = colCount;
        }

        internal GridBoundRecordState GetRowHeaderState(int index)
        {
            if (rowHeaderStates.Count == 0)
            {
                InitRowHeaderStates();
            }

            if (index >= rowHeaderStates.Count || index < 0)
            {
                return null;
            }

            return rowHeaderStates[index] as GridBoundRecordState;
        }

        void InitRowHeaderStates()
        {
            int rowHeaderCount = 0;

            rowHeaderStates.Clear();

            GridBoundRecordState parent = null;
            foreach (GridHierarchyLevel ghl in this.levels)
            {
                if (ghl.showHeaders)
                {
                    ghl.headerTopRow = rowHeaderCount;
                    for (int n = 0; n < ghl.rowCount; n++)
                    {
                        GridBoundRecordState rs = new GridBoundRecordState(parent);
                        rs.level = ghl.level;
                        rs.row = n;
                        rs.position = -1;
                        rowHeaderStates.Add(rs);
                        parent = rs;
                        rowHeaderCount++;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        internal void SynchronizeRowHeaderCount()
        {
            InitRowHeaderStates();

            gridModel.Rows.HeaderCount = rowHeaderStates.Count - 1;
            gridModel.Rows.FrozenCount = rowHeaderStates.Count - 1;
        }

        internal void ModelColsMoving(object sender, GridRangeMovingEventArgs e)
        {
            int index = ColIndexToField(e.From);
            int count = e.Count;
            int dest = ColIndexToField(e.Target);

            GridBoundColumnsCollection columns = this.InternalColumns;

            if (dest == index || count == 0 || (columns.Count < index && columns.Count < dest))
            {
                return;
            }

            GridBoundColumnsCollection newColumns = CreateBoundColumnsCollection();

            if (dest > index)
            {
                dest += count;
            }

            for (int n = 0; n <= columns.Count; n++)
            {
                if (n >= index && n < index + count)
                {
                    n += count;
                    if (n > columns.Count)
                    {
                        break;
                    }
                }

                if (n == dest)
                {
                    for (int k = 0; k < count; k++)
                    {
                        newColumns.Add(columns[index + k]);
                    }
                }

                if (n < columns.Count)
                {
                    newColumns.Add(columns[n]);
                }
            }

            this.BeginUpdateInternal();

            this.gridColumns = newColumns;

            gridModel.ResetVolatileData();

            this.EndUpdateInternal();
        }

        #endregion
        #endregion
        #region ErrorHandling
        /// <summary>
        /// Resets error information for the current record.
        /// </summary>
        public void ResetError()
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.Development.TraceVerbose);
            errorValue = string.Empty;
            errorMessage = string.Empty;
            exception = null;
            hasError = false;
        }

        /// <summary>
        /// Gets the error message of the last error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an error occurred in the last save operation.
        /// </summary>
        public bool HasError
        {
            get
            {
                return hasError;
            }
        }

        /// <summary>
        /// Gets the exception for the last save operation.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return exception;
            }
        }

        private string ErrorValue
        {
            get
            {
                return errorValue;
            }
        }

        ////        private bool ListHasErrors
        ////        {
        ////            get
        ////            {
        ////                return listHasErrors;
        ////            }
        ////            set
        ////            {
        ////                listHasErrors = value;
        ////            }
        ////        }

        private bool DataGridSourceHasErrors()
        {
            if (this.List == null)
            {
                return false;
            }

            int count = this.GetListManagerCount();
            for (int index = 0; index < count; index++)
            {
                object obj = this.List[index];
                if (obj is IDataErrorInfo)
                {
                    string error = ((IDataErrorInfo)obj).Error;
                    if (error != null && error.Length != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion
        #region DatasourceEvents
        void GridColumnsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new CollectionChangeEventHandler(this.GridColumnsCollectionChanged), new object[] { sender, e });
                return;
            }
#if DEBUG

            if (Switches.GridDataBoundGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Action, e.Element);
            }
#else
            ;
#endif
            gridModel.BeginUpdate(BeginUpdateOptions.None);
            if (site != null)
            {
                int count = gridColumns.Count;
                for (int n = 0; n < count; n++)
                {
                    GridBoundColumn col = gridColumns[n];
                    if (col != null && col.Site != null && col.Site != site)
                    {
                        site.Container.Add(col);
                    }
                }
            }

            gridModel.ResetVolatileData();
            gridModel.EndUpdate(false);
            OnGridBoundColumnsChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Refreshes the child view using the DataRelation to get to
        /// the child rows of the selected row.
        /// </summary>
        private void RefreshRows(CurrencyManager bmb)
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.Development.TraceVerbose);
            if (!inCancelEdit)
            {
                CancelEdit();
            }

            for (int i = 0; i < recordStates.Count; i++)
            {
                int k = i;
                if (recordStates[k] is IDisposable)
                {
                    ((IDisposable)recordStates[k]).Dispose();
                }
            }

            recordStates.Clear();
            listManagerToGridPositions.Clear();
            gridModel.ResetVolatileData();
            gridModel.Refresh();
        }
        #endregion
        #region GridModel
        internal static void ModelQueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellModelFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellModelFactory != null)
                {
                    e.CellModel = pGridCellModelFactory.CreateCellModel(e.CellType, ((IGridModelSource)sender).Model);
                }
            }
        }

        void IGridModelDataProvider.QueryColCount(GridRowColCountEventArgs e)
        {
            QueryColCount(e);
        }

        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryColCount"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridRowColCountEventArgs"/> that contains the event data.</param>
        protected virtual void QueryColCount(GridRowColCountEventArgs e)
        {
            int n = 0;
            if (List != null)
            {
                n = gridModel.Cols.HeaderCount - (this.levels.Count > 1 ? 1 : 0); // 1st column is hierarchy checker.
            }

            e.Count = gridModel.Data.ColCount + n;
            e.Handled = true;
        }

        void IGridModelDataProvider.QueryRowCount(GridRowColCountEventArgs e)
        {
            QueryRowCount(e);
        }

        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryRowCount"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridRowColCountEventArgs"/> that contains the event data.</param>
        protected virtual void QueryRowCount(GridRowColCountEventArgs e)
        {
            if (List != null)
            {
                int n = gridModel.Rows.HeaderCount;
                if (SupportsAddNew)
                {
                    n += RootHierarchyLevel.rowCount;
                }

                e.Count = (RecordCount * RootHierarchyLevel.rowCount) + n + this.expRecordCount;
                e.Handled = true;
            }
            else
            {
                e.Count = 0;
                e.Handled = true;
            }
        }

        void ModelQueryCoveredRange(object sender, GridQueryCoveredRangeEventArgs e)
        {
            if (this.List == null)
            {
                return;
            }

            GridBoundRecordState state = null;
            if (e.RowIndex > gridModel.Rows.HeaderCount)
            {
                int position = this.RowIndexToPosition(e.RowIndex);
                state = GetRecordStateAtPosition(position);
            }
            else
            {
                state = this.GetRowHeaderState(e.RowIndex);
            }

            if (state == null)
            {
                return;
            }

            GridHierarchyLevel level = this.levels[state.level] as GridHierarchyLevel;

            // row header
            if (e.ColIndex == 0)
            {
                if (e.RowIndex <= gridModel.Rows.HeaderCount && this.gridModel.ActiveGridView != null && this.gridModel.ActiveGridView is GridDataBoundGrid && !((GridDataBoundGrid)this.gridModel.ActiveGridView).IsFilterBarWired)
                {
                    e.Range = GridRangeInfo.Cells(0, 0, gridModel.Rows.HeaderCount, this.levels.Count > 1 ? 1 : 0);
                    e.Handled = true;
                }
                else if (level.rowCount > 1 && state != null)
                {
                    int top = e.RowIndex - state.row;
                    e.Range = GridRangeInfo.Cells(top, 0, top + level.rowCount - 1, 0);
                    e.Handled = true;
                }
            }
            else if (e.ColIndex == 1 && this.levels.Count > 1)
            {
                // checker cell
                if (e.RowIndex <= gridModel.Rows.HeaderCount)
                {
                    e.Range = GridRangeInfo.Cells(0, 0, gridModel.Rows.HeaderCount, 1);
                    e.Handled = true;
                }
                else if (level.rowCount > 1 && state != null)
                {
                    int top = e.RowIndex - state.row;
                    e.Range = GridRangeInfo.Cells(top, 1, top + level.rowCount - 1, 1);
                    e.Handled = true;
                }
            }
            else if (e.ColIndex > 0)
            {
                int fieldNum = this.ColIndexToField(e.ColIndex);
                if (state != null)
                {
                    fieldNum = level.RowFieldToField(state.row, fieldNum);
                }

                if (level.coveredCells != null && fieldNum >= 0 && fieldNum < level.coveredCells.Length)
                {
                    int first = level.coveredCells[fieldNum];
                    if (first != -1)
                    {
                        int last = first;
                        while (last + 1 < level.coveredCells.Length && level.coveredCells[last + 1] == first)
                        {
                            last++;
                        }

                        int row;
                        if (state != null)
                        {
                            first = level.FieldToRowField(first, out row);
                            last = level.FieldToRowField(last, out row);
                        }

                        e.Range = GridRangeInfo.Cells(e.RowIndex, this.FieldToColIndex(first), e.RowIndex, this.FieldToColIndex(last));
                        e.Handled = true;
                    }
                }
            }
        }

        void IGridModelDataProvider.QueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            QueryCellInfo(e);
        }

        /// <summary>
        /// Method handler for the <see cref="GridModel.QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridQueryCellInfoEventArgs"/> that contains the event data.</param>
        protected virtual void QueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            int fieldNum = this.ColIndexToField(e.ColIndex);
            IList list = this.List;
            if (this.levels != null)
            {
                if ((e.ColIndex == 1) && (this.levels.Count > 1))
                {
                    if ((list != null) && (e.RowIndex > this.gridModel.Rows.HeaderCount))
                    {
                        int position = this.RowIndexToPosition(e.RowIndex);
                        GridBoundRecordState recordStateAtPosition = this.GetRecordStateAtPosition(position);
                        if (recordStateAtPosition.row == 0)
                        {
                            GridHierarchyLevel level = this.levels[recordStateAtPosition.level] as GridHierarchyLevel;
                            if (((level.RowStyle != null) && (level.RowStyle.Store != null)) && !level.RowStyle.IsEmpty)
                            {
                                e.Style.ModifyStyle(level.RowStyle, 0);
                            }
                            if (level.relation != null)
                            {
                                e.Style.CellType = "DataBoundRowExpandCell";
                                if (recordStateAtPosition.HasChildList)
                                {
                                    e.Style.CellValue = this.IsExpanded(position) ? 1 : 0;
                                }
                                else
                                {
                                    e.Style.CellValue = -1;
                                }
                            }
                            else
                            {
                                e.Style.CellType = "Static";
                            }
                            e.Style.Borders.Right = this.gridModel.TableStyle.Borders.Right;
                        }
                    }
                }
                else if (fieldNum >= 0)
                {
                    if ((e.RowIndex >= 0) && (e.RowIndex < this.rowHeaderStates.Count))
                    {
                        GridBoundRecordState rowHeaderState = this.GetRowHeaderState(e.RowIndex);
                        if (rowHeaderState != null)
                        {
                            GridHierarchyLevel level2 = this.levels[rowHeaderState.level] as GridHierarchyLevel;
                            fieldNum = level2.RowFieldToField(rowHeaderState.row, fieldNum);
                            if (((level2.HeaderStyle != null) && (level2.HeaderStyle.Store != null)) && !level2.HeaderStyle.IsEmpty)
                            {
                                e.Style.ModifyStyle(level2.HeaderStyle, 0);
                            }
                            GridBoundColumnsCollection internalColumns = level2.InternalColumns;
                            if ((fieldNum >= 0) && (fieldNum < internalColumns.Count))
                            {
                                GridBoundColumn column = internalColumns[fieldNum];
                                if (column.HeaderText.Length > 0)
                                {
                                    e.Style.CellValue = column.HeaderText;
                                }
                                else
                                {
                                    e.Style.CellValue = column.MappingName;
                                }
                                if (column.PropertyDescriptor != null)
                                {
                                    ListSortDirection sortDirection = GridListUtil.GetSortDirection(this.List);
                                    PropertyDescriptor sortProperty = GridListUtil.GetSortProperty(this.List);
                                    if ((sortProperty != null) && sortProperty.Equals(column.PropertyDescriptor))
                                    {
                                        e.Style.Tag = sortDirection;
                                    }
                                }
                                e.Handled= true;
                            }
                            else
                            {
                                e.Style.Enabled = false;
                                e.Style.Borders.Right = GridBorder.Empty;
                                e.Style.CellType = "Static";
                            }
                        }
                    }
                    else if ((e.RowIndex > this.gridModel.Rows.HeaderCount) && (list != null))
                    {
                        int num3 = this.RowIndexToPosition(e.RowIndex);
                        GridBoundRecordState state3 = this.GetRecordStateAtPosition(num3);
                        GridHierarchyLevel level3 = this.levels[state3.level] as GridHierarchyLevel;
                        fieldNum = level3.RowFieldToField(state3.row, fieldNum);
                        GridBoundColumnsCollection columnss2 = level3.InternalColumns;
                        if (((level3.RowStyle != null) && (level3.RowStyle.Store != null)) && !level3.RowStyle.IsEmpty)
                        {
                            e.Style.ModifyStyle(level3.RowStyle, 0);
                        }
                        if ((fieldNum >= 0) && (fieldNum < columnss2.Count))
                        {
                            GridBoundColumn column2 = columnss2[fieldNum];
                            PropertyDescriptor propertyDescriptor = column2.PropertyDescriptor;
                            if (((column2.StyleInfo != null) && (column2.StyleInfo.Store != null)) && !column2.StyleInfo.IsEmpty)
                            {
                                e.Style.ModifyStyle(column2.StyleInfo, 0);
                            }
                            if (propertyDescriptor != null)
                            {
                                if (state3.position < state3.table.Count)
                                {
                                    if ((this.inEdit && !this.RecordDiffersAtPosition(num3, this.currentPosition)) && column2.dirty)
                                    {
                                        e.Style.CellValue = column2.value;
                                    }
                                    else
                                    {
                                        object propertyValue;
                                        object obj2 = state3.table[state3.position];
                                        if ((obj2 is DataRowView) && (((DataRowView)obj2).Row == null))
                                        {
                                            obj2 = null;
                                        }
                                        if (obj2 != null)
                                        {
                                            if (column2.MappingName.Contains(".") && this.useComplexBinding)
                                            {
                                                propertyValue = this.GetPropertyValue(obj2, column2.MappingName);
                                            }
                                            else
                                            {
                                                propertyValue = propertyDescriptor.GetValue(obj2);
                                            }
                                        }
                                        else
                                        {
                                            propertyValue = null;
                                        }
                                        e.Style.CellValue = propertyValue;
                                    }
                                }
                                GridStyleInfo style = e.Style;
                                style.ReadOnly |= propertyDescriptor.IsReadOnly || column2.ReadOnly;
                                if ((list is DataView) && !((DataView)list).AllowEdit)
                                {
                                    e.Style.ReadOnly = true;
                                }
                            }
                            else
                            {
                                e.Style.CellValue = string.Empty;
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            e.Style.Enabled = false;
                            e.Style.Borders.Right = GridBorder.Empty;
                        }
                    }
                }
            }

        }
        private object GetPropertyValue(object obj, string mappingName)
        {
            if (mappingName.Contains("."))
            {
                PropertyDescriptorCollection instanceProperties = GridListUtil.GetInstanceProperties(obj.GetType(), false);
                string[] strArray = mappingName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    string text1 = strArray[strArray.Length - 1];
                    PropertyDescriptorCollection descriptors2 = null;
                    for (int i = 0; i <= (strArray.Length - 2); i++)
                    {
                        descriptors2 = instanceProperties;
                        foreach (PropertyDescriptor descriptor in descriptors2)
                        {
                            if (descriptor.Name.Equals(strArray[i]))
                            {
                                obj = descriptor.GetValue(obj);
                                instanceProperties = GridListUtil.GetInstanceProperties(obj.GetType(), false);
                                goto Label_00B0;
                            }
                        }
                    Label_00B0:
                        if (instanceProperties.Equals(descriptors2))
                        {
                            return string.Empty;
                        }
                    }
                    string str = strArray[strArray.Length - 1];
                    foreach (PropertyDescriptor descriptor2 in instanceProperties)
                    {
                        if (descriptor2.Name.Equals(str))
                        {
                            return descriptor2.GetValue(obj);
                        }
                    }
                }
            }
            return string.Empty;
        }



        void IGridModelDataProvider.SaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            SaveCellInfo(e);
        }

        /// <summary>
        /// Method handler for the <see cref="GridModel.SaveCellInfo"/> event.
        /// </summary>
        /// <param name="e">An <see cref="GridSaveCellInfoEventArgs"/> that contains the event data.</param>
        protected virtual void SaveCellInfo(GridSaveCellInfoEventArgs e)
        {
            // Should I check this.enableEdit here and return or should this be up to the
            // current cell type to check for enable edit.
#if DEBUG

            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else

            ;
#endif

            ////CurrencyManager lm = ListManager;
            if ((this.List != null) && (e.RowIndex > this.gridModel.Rows.HeaderCount))
            {
                int position = this.RowIndexToPosition(e.RowIndex);
                int fieldNum = this.ColIndexToField(e.ColIndex);
                if (fieldNum >= 0)
                {
                    GridBoundRecordState recordStateAtPosition = this.GetRecordStateAtPosition(position);
                    GridHierarchyLevel level = this.levels[recordStateAtPosition.level] as GridHierarchyLevel;
                    fieldNum = level.RowFieldToField(recordStateAtPosition.row, fieldNum);
                    GridBoundColumnsCollection internalColumns = level.InternalColumns;
                    if ((fieldNum >= 0) && (fieldNum < internalColumns.Count))
                    {
                        GridBoundColumn column = internalColumns[fieldNum];
                        PropertyDescriptor propertyDescriptor = column.PropertyDescriptor;
                        if (propertyDescriptor != null)
                        {
                            if (this.DirectSaveCellInfo)
                            {
                                object obj2 = null;
                                if (recordStateAtPosition.table.Count  <= recordStateAtPosition.position)
                                {
                                    this.CurrentPosition = position;
                                    this.BeginEdit();
                                    if (this.BindToCurrencyManager)
                                    {
                                        obj2 = this.ListManager.Current;
                                    }
                                    else
                                    {
                                        obj2 = this.List[this.currentPosition];
                                    }
                                }
                                else
                                    obj2 = recordStateAtPosition.table[recordStateAtPosition.position];
                                if (column.MappingName.Contains(".") && this.useComplexBinding)
                                {
                                    this.SetPropertyValue(obj2, e.Style.CellValue, column.MappingName);
                                }
                                else
                                {
                                    propertyDescriptor.SetValue(obj2, e.Style.CellValue);
                                }
                                if (!this.RecordDiffersAtPosition(this.CurrentPosition, position) && (this.editableItem != null))
                                {
                                    this.editableItem.EndEdit();
                                }
                            }
                            else
                            {
                                object currentItem = (recordStateAtPosition.position < recordStateAtPosition.table.Count) ? recordStateAtPosition.table[recordStateAtPosition.position] : null;
                                if (!this.RecordDiffersAtPosition(this.CurrentPosition, position))
                                {
                                    if (this.currentItem != null)
                                    {
                                        currentItem = this.currentItem;
                                    }
                                    else
                                    {
                                        this.CurrentPosition = position;
                                        this.BeginEdit();
                                        if (this.BindToCurrencyManager)
                                        {
                                            currentItem = this.ListManager.Current;
                                        }
                                        else
                                        {
                                            currentItem = this.List[this.currentPosition];
                                        }
                                    }
                                }
                                if (currentItem != null)
                                {
                                    try
                                    {
                                        column.value = e.Style.CellValue;
                                        if (!column.dirty)
                                        {
                                            column.dirty = true;
                                            if (column.MappingName.Contains(".") && this.useComplexBinding)
                                            {
                                                column.savedValue = this.GetPropertyValue(currentItem, column.MappingName);
                                            }
                                            else
                                            {
                                                column.savedValue = propertyDescriptor.GetValue(currentItem);
                                            }
                                        }
                                        if (currentItem is IDataErrorInfo)
                                        {
                                            string error = ((IDataErrorInfo)currentItem).Error;
                                            if ((error != null) && (error.Length > 0))
                                            {
                                                Trace.WriteLine(error);
                                            }
                                        }
                                        if ((currentItem is DataRowView) && this.AllowSetValueOnCurrentItem)
                                        {
                                            object obj4 = propertyDescriptor.GetValue(currentItem);
                                            if (((obj4 != null) && !obj4.Equals(column.value)) || ((obj4 == null) && (column.value != null)))
                                            {
                                                if (column.MappingName.Contains(".") && this.useComplexBinding)
                                                {
                                                    this.SetPropertyValue(currentItem, e.Style.CellValue, column.MappingName);
                                                }
                                                else
                                                {
                                                    propertyDescriptor.SetValue(currentItem, column.value);
                                                }
                                            }
                                        }
                                        this.rowDirty = true;
                                    }
                                    catch (Exception exception)
                                    {
                                        TraceUtil.TraceExceptionCatched(exception);
                                        this.errorMessage = exception.Message;
                                        this.exception = exception;
                                        this.hasError = true;
                                        this.errorValue = e.Style.Text;
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
                e.Handled= true;
            }

        }
        #endregion

        private void SetPropertyValue(object obj, object value, string mappingName)
        {
            if (mappingName.Contains("."))
            {
                PropertyDescriptorCollection instanceProperties = GridListUtil.GetInstanceProperties(obj.GetType(), false);
                string[] strArray = mappingName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    string text1 = strArray[strArray.Length - 1];
                    PropertyDescriptorCollection descriptors2 = null;
                    for (int i = 0; i <= (strArray.Length - 2); i++)
                    {
                        descriptors2 = instanceProperties;
                        foreach (PropertyDescriptor descriptor in descriptors2)
                        {
                            if (descriptor.Name.Equals(strArray[i]))
                            {
                                obj = descriptor.GetValue(obj);
                                instanceProperties = GridListUtil.GetInstanceProperties(obj.GetType(), false);
                                goto Label_00B0;
                            }
                        }
                    Label_00B0:
                        if (instanceProperties.Equals(descriptors2))
                        {
                            return;
                        }
                    }
                    string str = strArray[strArray.Length - 1];
                    foreach (PropertyDescriptor descriptor2 in instanceProperties)
                    {
                        if (descriptor2.Name.Equals(str))
                        {
                            descriptor2.SetValue(obj, value);
                        }
                    }
                }
            }
        }

 

 

        #region CopyPaste
        internal bool DataBoundPaste(IDataObject iData, int clipboardFlags, GridRangeInfoList rangeList, GridCutPasteEventArgs e)
        {
            int firstRow = gridModel.Rows.HeaderCount + 1;
            int firstCol = gridModel.Cols.HeaderCount + 1;

            GridRangeInfo range = rangeList.GetOuterRange(rangeList.ActiveRange).ExpandRange(firstRow, firstCol, gridModel.RowCount, gridModel.ColCount);

            if (GridUtil.IsSet(clipboardFlags, GridDragDropFlags.Text))
            {
                string psz = null;
                if (iData.GetDataPresent(DataFormats.UnicodeText))
                {
                    psz = iData.GetData(DataFormats.UnicodeText) as string;
                }
                else if (iData.GetDataPresent(DataFormats.Text))
                {
                    psz = iData.GetData(DataFormats.Text) as string;
                }

                if (psz != null)
                {
                    OperationFeedback op = new OperationFeedback(gridModel);
                    bool restoreDirectSaveCellInfo = DirectSaveCellInfo;
                    try
                    {
                        int rowIndex = range.Top, colIndex = range.Left;

                        op.AllowRollback = true;
                        op.Description = SR.GetString("GRID_IDM_PASTINGDATA");
                        op.AllowNestedProgress = false;

                        int rowCount, colCount;
                        ArrayList textArray = gridModel.TextDataExchange.CreateTableFromCVSBuffer(psz, out rowCount, out colCount, op);

                        int bottom = range.Top + rowCount - 1;
                        // Verify that only one range or current cell is selected.
                        if (rowCount == 1 && colCount == 1 && e != null && !e.IgnoreCurrentCell)
                        {
                            // Give the current cell a chance
                            GridCellRendererBase cellRenderer = gridModel.CurrentCellRenderer;
                            if (cellRenderer != null && cellRenderer.HasFocusControl
                                && cellRenderer.CurrentCell.IsEditing && cellRenderer.CanPaste() && cellRenderer.Paste())
                            {
                                return true;
                            }
                        }

                        int nIndex = 0;
                        int size = rowCount * colCount;
                        gridModel.BeginUpdate(BeginUpdateOptions.Invalidate);

                        // Store and deactivate current cell.
                        gridModel.ConfirmChanges();

                        if (rowCount > 1)
                        {
                            this.EndEdit();
                            this.DirectSaveCellInfo = true;
                        }

                        gridModel.CommandStack.BeginTrans(SR.GetString("GRID_IDM_PASTEDATA"));
                        if (bottom > gridModel.RowCount)
                        {
                            if (GridUtil.IsNotSet(clipboardFlags, GridDragDropFlags.NoAppendRows))
                            {
                                gridModel.RowCount = bottom;
                            }

                            bottom = gridModel.RowCount;
                        }
                        bool canceled = false;
                        for (int rowNum = 0; !canceled && rowNum < textArray.Count; rowNum++)
                        {
                            ArrayList rowArray = textArray[rowNum] as ArrayList;
                            if (rowArray == null)
                            {
                                continue;
                            }

                            colIndex = range.Left;

                            for (int colNum = 0; !canceled && colNum < rowArray.Count; colNum++)
                            {
                                GridStyleInfo style = null;
                                style = gridModel[rowIndex, colIndex];

                                // Give the control the chance to validate
                                // and change the pasted text.
                                canceled = !gridModel.TextDataExchange.PasteTextRowCol(rowIndex, colIndex, rowArray[colNum] as string);

                                // Check if user pressed ESC to cancel.
                                op.PercentComplete = (int)((nIndex++) * 100 / size);
                                if (op.ShouldCancel)
                                {
                                    canceled = true;
                                }

                                colIndex++;
                            }

                            rowIndex++;
                        }

                        if (canceled && op.RollbackConfirmed)
                        {
                            gridModel.CommandStack.Rollback();
                        }
                        else
                        {
                            gridModel.CommandStack.CommitTrans();
                        }

                        gridModel.Refresh();

                        return !canceled;
                    }
                    finally
                    {
                        this.DirectSaveCellInfo = restoreDirectSaveCellInfo;
                        op.Close();
                        gridModel.EndUpdate();
                    }
                }
            }

            return false;
        }
        #endregion
        #region CurrentRecord
        void InitCurrentRecordState(int currentPosition)
        {
            if (currentPosition < 0)
            {
                currentRecordState = null;
                currentHierarchyLevel = RootHierarchyLevel;
                currentColumns = currentHierarchyLevel.InternalColumns;
                currentListManagerPosition = -1;
            }
            else
            {
                currentRecordState = GetRecordStateAtPosition(currentPosition);
                currentHierarchyLevel = this.levels[currentRecordState.level] as GridHierarchyLevel;
                currentColumns = currentHierarchyLevel.InternalColumns;
                currentListManagerPosition = PositionToListManagerPosition(currentPosition);
            }
        }

        /// <summary>
        /// Begins an edit on the current record.
        /// </summary>
        public virtual void BeginEdit()
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.Development.TraceVerbose);
            if (List == null)
            {
                return;
            }

            // Fixes an issue when a details grid was attached to this table. At that time the
            // CurrencyManager reset the position to be -1.
            if (this.List.Count == 0)
            {
                this.currentPosition = 0;
            }

            if (this.IsAppendRow)
            {
                AddNew();
            }

            InitCurrentRecordState(this.CurrentPosition);

            if (currentRecordState.position >= currentRecordState.table.Count)
            {
                return;
            }

            object component = currentRecordState.table[currentRecordState.position];
            if (currentItem == null || !Object.Equals(currentItem, component))
            {
                if (editableItem != null)
                {
                    editableItem.EndEdit();
                }

                currentItem = component;
                editableItem = component as IEditableObject;
                if (editableItem != null)
                {
                    editableItem.BeginEdit();
                }

                if (!inAddNew)
                {
                    inEdit = true;
                    OnEditModeChanged(EventArgs.Empty);
                }
            }
        }

        bool inEndEdit = false;

        /// <summary>
        /// Gets a value indicating whether <see cref="EndEdit"/> has been called.
        /// </summary>
        public bool InEndEdit
        {
            get
            {
                return this.inEndEdit;
            }
        }

        /// <summary>
        /// Pushes changes since a <see cref="BeginEdit()"/> call for the current record.
        /// </summary>
        public virtual void EndEdit()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if ((this.List != null) && !this.inEndEdit)
            {
                GridBoundColumnsCollection currentColumns = this.currentColumns;
                if (this.currentItem != null)
                {
                    for (int i = 0; i < currentColumns.Count; i++)
                    {
                        if (currentColumns[i].dirty)
                        {
                            PropertyDescriptor propertyDescriptor = currentColumns[i].PropertyDescriptor;
                            if (propertyDescriptor != null)
                            {
                                object obj2 = null;
                                if (currentColumns[i].value is IConvertible)
                                {
                                    obj2 = NullableHelper.ChangeType(currentColumns[i].value, propertyDescriptor.PropertyType);
                                }
                                else
                                {
                                    obj2 = currentColumns[i].value;
                                }
                                if (currentColumns[i].MappingName.Contains(".") && this.useComplexBinding)
                                {
                                    this.SetPropertyValue(this.currentItem, obj2, currentColumns[i].MappingName);
                                }
                                else
                                {
                                    propertyDescriptor.SetValue(this.currentItem, obj2);
                                }
                            }
                        }
                    }
                    this.inEndEdit = true;
                    try
                    {
                        if (this.editableItem != null)
                        {
                            if (this.IsAnyDirtyField())
                            {
                                int listManagerCount = this.GetListManagerCount();
                                DataView list = null;
                                if (!this.optimizeListChangedEvent)
                                {
                                    list = this.List as DataView;
                                }
                                if (((list != null) && (list.RowFilter != null)) && (list.RowFilter != string.Empty))
                                {
                                    if (!this.inAddNew)
                                    {
                                        this.editableItem.EndEdit();
                                        int num3 = this.GetListManagerCount();
                                        if ((listManagerCount + 1) == num3)
                                        {
                                            string rowFilter = list.RowFilter;
                                            list.BeginInit();
                                            list.RowFilter = string.Empty;
                                            list.RowFilter = rowFilter;
                                            list.EndInit();
                                            if (this.BindToCurrencyManager)
                                            {
                                                this.listManager = (CurrencyManager)this.BindingContext[list];
                                            }
                                            this.ResetRecordState();
                                            this.RefreshRows(this.ListManager);
                                        }
                                    }
                                }
                                else
                                {
                                    this.editableItem.EndEdit();
                                }
                            }
                            else
                            {
                                this.editableItem.CancelEdit();
                            }
                        }
                    }
                    finally
                    {
                        this.inEndEdit = false;
                    }
                }
                if (this.inAddNew)
                {
                    CurrencyManager listManager = this.ListManager;
                    if (listManager != null)
                    {
                        if (this.IsAnyDirtyField())
                        {
                            listManager.EndCurrentEdit();
                            if (addNewForceItemChangedEventHack)
                            {
                                listManager.AddNew();
                                listManager.CancelCurrentEdit();
                            }
                        }
                        else
                        {
                            listManager.CancelCurrentEdit();
                        }
                    }
                }
                if (currentColumns != null)
                {
                    for (int j = 0; j < currentColumns.Count; j++)
                    {
                        currentColumns[j].dirty = false;
                        currentColumns[j].value = null;
                    }
                }
                this.inEdit = false;
                this.inAddNew = false;
                this.rowDirty = false;
                this.editableItem = null;
                this.currentItem = null;
                this.OnEditModeChanged(EventArgs.Empty);
            }

        }

        static bool addNewForceItemChangedEventHack = false;

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether add new ForceItemChangedEvent hack. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool AddNewForceItemChangedEventHack
        {
            get
            {
                return addNewForceItemChangedEventHack;
            }

            set
            {
                addNewForceItemChangedEventHack = value;
            }
        }

        private bool inCancelEdit;

        /// <exclude/>
        /// <summary>Gets a value indicating whether the current edit operation is being cancelled.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public bool InCancelEdit
        {
            get { return inCancelEdit; }
        }

        /// <summary>
        /// Cancels the current edit operation.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (inCancelEdit)
            {
                return;
            }

            inCancelEdit = true;
            try
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                GridBoundColumnsCollection columns = this.currentColumns;
                if (inAddNew && ListManager != null)
                {
                    CurrencyManager lm = ListManager;
                    lm.CancelCurrentEdit();
                }
                else if (currentItem != null)
                {
                    for (int fieldNum = 0; fieldNum < columns.Count; fieldNum++)
                    {
                        GridBoundColumn columnStyle = columns[fieldNum];
                        if (columnStyle.dirty && !this.inbindingList_ListChangedCurrentRecord)
                        {
                            columnStyle.PropertyDescriptor.SetValue(currentItem, columnStyle.savedValue);
                        }
                    }

                    if (editableItem != null)
                    {
                        editableItem.CancelEdit();
                    }
                }

                if (columns != null)
                {
                    for (int fieldNum = 0; fieldNum < columns.Count; fieldNum++)
                    {
                        GridBoundColumn columnStyle = columns[fieldNum];
                        columnStyle.dirty = false;
                        columnStyle.value = null;
                        columnStyle.savedValue = null;
                    }
                }

                inEdit = false;
                inAddNew = false;
                rowDirty = false;
                editableItem = null;
                currentItem = null;
                OnEditModeChanged(EventArgs.Empty);
            }
            finally
            {
                inCancelEdit = false;
            }
        }

        /// <summary>
        /// Resets a field back to its original value in the current record.
        /// </summary>
        /// <param name="fieldNum">The zero-based field index.</param>
        public void ResetField(int fieldNum)
        {
            GridBoundColumnsCollection columns = this.currentColumns;
            if (columns != null)
            {
                GridBoundColumn columnStyle = columns[fieldNum];
                if (columnStyle.dirty)
                {
                    columnStyle.dirty = false;
                    columnStyle.value = null;
                    if (currentItem != null && rowDirty)
                    {
                        PropertyDescriptor pd;
                        pd = columnStyle.PropertyDescriptor;
                        pd.SetValue(currentItem, columnStyle.savedValue);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a field is modified in the current record.
        /// </summary>
        /// <param name="fieldNum">The zero-based field index.</param>
        /// <returns>True if field is changed; False otherwise.</returns>
        public bool IsFieldDirty(int fieldNum)
        {
            GridBoundColumnsCollection columns = this.currentColumns;
            if (columns == null || fieldNum < 0 || fieldNum >= columns.Count)
            {
                return false;
            }

            return columns[fieldNum].dirty;
        }

        /// <summary>
        /// Checks if any field in the current record is modified.
        /// </summary>
        /// <returns>true if there are pending changes; false if current record is not modified.</returns>
        public bool IsAnyDirtyField()
        {
            GridBoundColumnsCollection columns = this.currentColumns;
            if (columns != null)
            {
                for (int fieldNum = 0; fieldNum < columns.Count; fieldNum++)
                {
                    if (columns[fieldNum].dirty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new record to the datasource.
        /// </summary>
        public virtual void AddNew()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            CurrencyManager lm = ListManager;
            if (lm != null && this.SupportsAddNew)
            {
                inAddNew = true;
                lm.AddNew();
                inEdit = true;

                gridModel.ResetVolatileData();
                OnEditModeChanged(EventArgs.Empty);
            }
            else if (List != null && this.SupportsAddNew)
            {
                inAddNew = true;
                ((IBindingList)List).AddNew();
                inEdit = true;

                gridModel.ResetVolatileData();
                OnEditModeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid supports editing records if the underlying datasource
        /// allows it. See <see cref="GridModelDataBinder.AllowEdit"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports editing records if the underlying datasource allows it.")]
        public bool EnableEdit
        {
            get
            {
                return enableEdit;
            }

            set
            {
                enableEdit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid supports editing records if the underlying datasource
        /// allows it. See <see cref="GridModelDataBinder.AllowAddNew"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports adding new records if the underlying datasource allows it.")]
        public bool EnableAddNew
        {
            get
            {
                return enableAddNew;
            }

            set
            {
                enableAddNew = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are pending changes to the current record.
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return rowDirty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="BeginEdit()"/> has been called.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return inEdit;
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="AddNew"/> has been called.
        /// </summary>
        public bool IsAddNew
        {
            get
            {
                return inAddNew;
            }
        }

        /// <summary>
        /// Occurs when <see cref="BeginEdit()"/>, <see cref="EndEdit"/>, <see cref="CancelEdit"/>, or <see cref="AddNew"/> is called.
        /// </summary>
        public event EventHandler EditModeChanged;

        /// <summary>
        /// Raises the <see cref="EditModeChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnEditModeChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource, this.inEdit, this.inAddNew);
            }
#else
            ;
#endif
            if (EditModeChanged != null)
            {
                EditModeChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the zero-based position of the current record in the datasource.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentPosition
        {
            get
            {
                return currentPosition;
            }

            set
            {
                if (value != CurrentPosition)
                {
                    bool diff = this.RecordDiffersAtPosition(value, CurrentPosition);
                    if (inEdit && diff)
                    {
                        EndEdit();
                    }
#if DEBUG
                    if (Switches.Development.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo(value);
                    }
#else
                    ;
#endif
                    SetCurrentPosition(value, true);
                }
            }
        }

        internal void SyncCurrentRecordState(int pos)
        {
            if (this.RecordDiffersAtPosition(pos, currentPosition))
            {
                throw new InvalidOperationException("Do not move current position with this method!");
            }

            if (currentPosition != pos)
            {
                savedPosition = currentPosition;
                currentPosition = pos;
                currentRecordState = GetRecordStateAtPosition(pos);
                currentHierarchyLevel = this.levels[currentRecordState.level] as GridHierarchyLevel;
                currentColumns = currentHierarchyLevel.InternalColumns;
                OnCurrentPositionChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the last position before <see cref="CurrentPosition"/> was changed.
        /// </summary>
        [Browsable(false)]
        public int SavedPosition
        {
            get
            {
                return savedPosition;
            }
        }

        /// <summary>
        /// Gets the row index in the grid for the <see cref="CurrentPosition"/>.
        /// </summary>
        [Browsable(false)]
        public int CurrentRowIndex
        {
            get
            {
                return PositionToRowIndex(CurrentPosition);
            }
        }

        /// <summary>
        /// Gets the row index in the grid for the <see cref="SavedPosition"/>.
        /// </summary>
        [Browsable(false)]
        public int SavedRowIndex
        {
            get
            {
                return PositionToRowIndex(SavedPosition);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the grid's current row is positioned at the append row (if any).
        /// </summary>
        [Browsable(false)]
        public bool IsAppendRow
        {
            get
            {
                return this.SupportsAddNew && !this.RecordDiffersAtPosition(CurrentPosition, this.LastPosition);
            }
        }

        /// <summary>
        /// Changes the current record in the datasource.
        /// </summary>
        /// <param name="value">The zero-based position in the datasource for the new current record.</param>
        /// <param name="raiseCurrentPositionChanged"> specifies if a <see cref="CurrentPositionChanged"/> event should be raised.</param>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public void SetCurrentPosition(int value, bool raiseCurrentPositionChanged)
        {
            if (value != CurrentPosition)
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
#else
                ;
#endif
                inSetCurrentPosition = true;
                try
                {
                    gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                    savedPosition = currentPosition;
                    currentPosition = value;

                    InitCurrentRecordState(currentPosition);
                    GridBoundRecordState state = this.currentRecordState;
                    while (state != null)
                    {
                        if (state.listManager != null && state.listManager.Position != state.position)
                        {
                            state.listManager.Position = state.position;
                        }

                        state = state.Parent;
                    }
                    ////                    if (ListManager != null && ListManager.Position != currentListManagerPosition)
                    ////                        ListManager.Position = currentListManagerPosition;

                    if (raiseCurrentPositionChanged)
                    {
                        OnCurrentPositionChanged(EventArgs.Empty);
                    }

                    gridModel.EndUpdate(true);
                }
                finally
                {
                    inSetCurrentPosition = false;
                }
            }
        }

        /// <summary>
        /// Raises a <see cref="CurrentPositionChanged"/> event.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public void RaiseCurrentPositionChanged()
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            OnCurrentPositionChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the current record position in the datasource is changed.
        /// </summary>
        public event EventHandler CurrentPositionChanged;

        /// <summary>
        /// Raises the <see cref="CurrentPositionChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnCurrentPositionChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource, this.CurrentPosition);
            }
#else
            ;
#endif
            if (CurrentPositionChanged != null)
            {
                CurrentPositionChanged(this, e);
            }
        }
        #endregion
        #region Sort
        /// <summary>
        /// Sorts the datasource by the specified field.
        /// </summary>
        /// <param name="fieldNum">This number specifies the field to use as key for a sort.</param>
        /// <remarks>
        /// When the datasource is sorted by the specified field, the sort direction is toggled
        /// between ascending and descending.<para/>
        /// Sorting is supported only for columns displayed in the root level for grid with
        /// nested relations displayed in the grid.
        /// </remarks>
        public virtual void Sort(int fieldNum)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(fieldNum);
            }
#else
            ;
#endif

            GridBoundColumnsCollection columns = this.InternalColumns;
            PropertyDescriptor pd = columns[fieldNum].PropertyDescriptor;
            _Sort(List, pd);
        }

        internal void _Sort(CurrencyManager lm, PropertyDescriptor pd)
        {
            if (lm != null && GridListUtil.SupportsSort(lm.List))
            {
                if (pd != null)
                {
                    ListSortDirection listSortDirection = GridListUtil.GetSortDirection(lm.List);
                    PropertyDescriptor propertyDescriptor = GridListUtil.GetSortProperty(lm.List);
                    if (propertyDescriptor != null && propertyDescriptor.Equals(pd))
                    {
                        listSortDirection = (listSortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    }
                    else
                    {
                        listSortDirection = ListSortDirection.Ascending;
                    }

                    IList list = lm.List;
                    GridListUtil.SetSort(list, pd, listSortDirection);
                }
            }
        }

        internal void _Sort(IList lm, PropertyDescriptor pd)
        {
            if (lm != null && GridListUtil.SupportsSort(lm))
            {
                if (pd != null)
                {
                    ListSortDirection listSortDirection = GridListUtil.GetSortDirection(lm);
                    PropertyDescriptor propertyDescriptor = GridListUtil.GetSortProperty(lm);
                    if (propertyDescriptor != null && propertyDescriptor.Equals(pd))
                    {
                        listSortDirection = (listSortDirection != ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    }
                    else
                    {
                        listSortDirection = ListSortDirection.Ascending;
                    }

                    IList list = lm;
                    GridListUtil.SetSort(list, pd, listSortDirection);
                }
            }
        }
        #endregion
        #region Remove

        /// <summary>
        /// Gets a value indicating whether the datasource allows removing records.
        /// </summary>
        public bool AllowRemove
        {
            get
            {
                return this.enableRemove && List != null && GridListUtil.GetAllowRemove(List);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the grid supports removing records if the underlying datasource
        /// allows it. See <see cref="GridModelDataBinder.AllowRemove"/>.
        /// </summary>
        [DefaultValue(true)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint)]
        [Description("A value indicating if the grid supports removing records if the underlying datasource allows it.")]
        public bool EnableRemove
        {
            get
            {
                return enableRemove;
            }

            set
            {
                enableRemove = value;
            }
        }

        /// <summary>
        /// Removes the specified records from the datasource (without sending a <see cref="GridDataBoundGrid.RowsDeleting"/> event; use <see cref="GridDataBoundGrid.DeleteRecordsAtRowIndex"/>
        /// instead if you need it.
        /// </summary>
        /// <param name="first">The zero-based position of the first record.</param>
        /// <param name="last">The zero-based position of the last record.</param>
        public virtual void RemoveRecords(int first, int last)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(first, last);
            }
#else
            ;
#endif
            inRemoveRecords = true;
            try
            {
                if (List != null && GridListUtil.GetAllowRemove(List))
                {
                    gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                    for (int index = last; index >= first; index--)
                    {
                        if (listManager != null)
                        {
                            ListManager.RemoveAt(index);
                        }
                        else
                        {
                            List.RemoveAt(index);
                        }
                    }

                    if (this.BindToCurrencyManager)
                    {
                        this.listManager_PositionChanged(ListManager, EventArgs.Empty);
                    }

                    this.OnRowChanged(EventArgs.Empty);
                    this.OnRecordsRemoved(EventArgs.Empty);
                    gridModel.EndUpdate(true);
                }
            }
            finally
            {
                inRemoveRecords = false;
            }
        }
        #endregion
        #region DataSource
        /// <summary>
        ///   <para>Gets or sets the datasource that the gridModel is displaying data for.</para>
        /// </summary>
        [Description(@"Indicates the source of data for the DataGrid."),
        DefaultValue(null),
        Category(@"Data"),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design")]
        public object DataSource
        {
            get
            {
                return this.dataSource;
            }

            set
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                if (value != null && !(value is IList) && !(value is IListSource))
                {
                    throw new Exception("BadDataSourceForComplexBinding");
                }

                if (this.dataSource != null && this.dataSource.Equals(value))
                {
                    return;
                }

                if (value == null || (value == Convert.DBNull && string.Empty != this.DataMember))
                {
                    this.dataSource = null;
                    this.DataMember = string.Empty;
                    if (this.list != null)
                    {
                        this.UnWireDataSource();
                    }

                    this.listManager = null;
                    this.list = null;
                    InitCurrentRecordState(-1);
                    return;
                }

                this.Set_ListManager(value, this.DataMember, false);
            }
        }

        /// <summary>
        ///   <para>Gets or sets the specific list in a <see cref="GridDataBoundGrid.DataSource" /> for which the <see cref="GridDataBoundGrid"/>
        /// control
        /// displays a gridModel.</para>
        /// </summary>
        [Category("Data"),
        DefaultValue(null),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Design"),
        Description("Indicates a sub-list of the DataSource to show in the grid.")]
        public string DataMember
        {
            get
            {
                return this.dataMember;
            }

            set
            {
#if DEBUG
                if (Switches.Development.TraceVerbose)
                {
                    TraceUtil.TraceCurrentMethodInfo();
                }
#endif
                if (this.dataMember != null && this.dataMember == value)
                {
                    return;
                }

                this.Set_ListManager(this.DataSource, value, false);
            }
        }

        GridBoundColumnsCollection gridColumns
        {
            get
            {
                return RootHierarchyLevel.gridColumns;
            }

            set
            {
                RootHierarchyLevel.gridColumns = value;
            }
        }

        GridBoundColumnsCollection internalGridColumns
        {
            get
            {
                return RootHierarchyLevel.internalGridColumns;
            }

            set
            {
                RootHierarchyLevel.internalGridColumns = value;
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridBoundColumn"/> objects in the <see cref="GridDataBoundGrid"/> control.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [LocalizableAttribute(true)]
        public GridBoundColumnsCollection GridBoundColumns
        {
            get
            {
                return gridColumns;
            }

            set
            {
                if (gridColumns != value)
                {
                    gridModel.BeginUpdate(BeginUpdateOptions.None);
                    ////SetSite(gridColumns, value, false);
                    GridBoundColumnsCollection savedColumns = gridColumns;
                    gridColumns = value;
                    ////                    SetSite(gridColumns, savedColumns, true);
                    InitializeColumns();
                    OnGridBoundColumnsChanged(EventArgs.Empty);
                    gridModel.EndUpdate(true);
                }
            }
        }

        /// <summary>
        /// Occurs when columns have been added or removed.
        /// </summary>
        public event EventHandler GridBoundColumnsChanged;

        /// <summary>
        /// Raises the <see cref="GridBoundColumnsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected internal virtual void OnGridBoundColumnsChanged(EventArgs e)
        {
            if (GridBoundColumnsChanged != null)
            {
                GridBoundColumnsChanged(this, e);
            }
        }

        void SetSite(GridBoundColumnsCollection gridColumns, GridBoundColumnsCollection diffColumns, bool add)
        {
            if (gridColumns == null)
            {
                return;
            }

            if (!add)
            {
                gridColumns.CollectionChanged -= new CollectionChangeEventHandler(GridColumnsCollectionChanged);
            }

            if (site != null)
            {
                foreach (GridBoundColumn col in gridColumns)
                {
                    if (col != null)
                    {
                        if (add)
                        {
                            if (diffColumns.Contains(col))
                            {
                                string name = col.Site.Name;
                                site.Container.Remove(col);
                                site.Container.Add(col, name);
                            }
                            else
                            {
                                site.Container.Add(col);
                            }
                        }
                        else
                        {
                            if (!diffColumns.Contains(col))
                            {
                                site.Container.Remove(col);
                            }
                        }
                    }
                }
            }

            if (add)
            {
                gridColumns.CollectionChanged += new CollectionChangeEventHandler(GridColumnsCollectionChanged);
            }
        }

        /// <summary>
        /// Gets or sets Property Site (ISite)
        /// </summary>
        public ISite Site
        {
            get
            {
                return this.site;
            }

            set
            {
                if (this.site != value)
                {
                    this.site = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="GridBoundColumn"/> objects in the <see cref="GridDataBoundGrid"/> control.
        /// This can be either the columns specified by the user or if no columns were specified it returns all
        /// columns that were automatically propagated from the data source.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [LocalizableAttribute(true)]
        public GridBoundColumnsCollection InternalColumns
        {
            get
            {
                return RootHierarchyLevel.InternalColumns;
            }
        }

        /// <summary>
        ///   <para>Sets the <see cref="GridDataBoundGrid.DataSource" /> and <see cref="GridDataBoundGrid.DataMember" /> properties at run-time.</para>
        /// </summary>
        /// <param name="dataSource">The datasource, typed as <see cref="System.Object" />, for the <see cref="GridDataBoundGrid" qualify="true" /> control.</param>
        /// <param name="dataMember">The <see cref="GridDataBoundGrid.DataMember" /> string that specifies the table to bind to within the object returned by the <see cref="GridDataBoundGrid.DataSource" /> property.</param>
        public void SetDataBinding(object dataSource, string dataMember)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.Set_ListManager(dataSource, dataMember, false);
        }

        internal void Set_ListManager(object newDataSource, string newDataMember, bool force)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            this.Set_ListManager(newDataSource, newDataMember, force, true);
        }

        void UpdateListManager()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="BindingContext"/> for this object. You can assign a <see cref="BindingContext"/>
        /// from a parent form to this property.
        /// </summary>
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    if (gridModel != null && gridModel.ActiveGridView != null)
                    {
                        bindingContext = gridModel.ActiveGridView.BindingContext;
                    }
                }

                return bindingContext;
            }

            set
            {
                if (bindingContext != value)
                {
#if DEBUG
                    if (Switches.Development.TraceVerbose)
                    {
                        TraceUtil.TraceCurrentMethodInfo();
                    }
#endif
                    bindingContext = value;
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// This is called when the <see cref="BindingContext"/> property is changed.
        /// </summary>
        /// <param name="e">The EventArgs.Empty</param>
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            this.Set_ListManager(this.dataSource, this.dataMember, true);
        }

        void BeginUpdateInternal()
        {
            gridModel.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
        }

        void EndUpdateInternal()
        {
            gridModel.EndUpdate();
        }

        ////bool isBindingSource = false;

        internal void Set_ListManager(object newDataSource, string newDataMember, bool force, bool forceColumnCreation)
        {
            ////isBindingSource = newDataSource is BindingSource && GridUtil.IsEmpty(newDataMember);

            bool isNewDataSource;
            bool isNewDataMember;
            bool isNewListManager;
            bool needEndUpdate = false;
            CurrencyManager lm;
            IList oldList;

            isNewDataSource = this.DataSource != newDataSource;
            isNewDataMember = this.DataMember != newDataMember;
            if (!force && !isNewDataSource && !isNewDataMember && inSetListManager)
            {
                return;
            }

            inSetListManager = true;
            currentListManagerCount = 0;
            try
            {
                this.UpdateListManager(); // EndEdit
                if (this.list != null)
                {
                    this.UnWireDataSource();
                }

                lm = this.listManager;
                oldList = List;
                isNewListManager = false;
                string t1 = newDataSource != null ? newDataSource.ToString() : string.Empty;
                string t2 = newDataMember != null ? newDataMember.ToString() : string.Empty;
                if (this.BindToCurrencyManager && newDataSource != null && this.BindingContext != null && newDataSource != System.Convert.DBNull)
                {
                    this.listManager = (CurrencyManager)this.BindingContext[newDataSource, newDataMember];
                }
                else
                {
                    this.listManager = null;
                }

                this.dataSource = newDataSource;
                if (newDataMember != null)
                {
                    this.dataMember = newDataMember;
                }

                isNewListManager = this.listManager != lm || this.List != oldList;
                if (this.listManager != null)
                {
                    this.WireDataSource();
                    this.currentListManagerCount = this.ListManager.Count;
                }
                else if (this.List != null)
                {
                    this.WireDataSource();
                    this.currentListManagerCount = this.List.Count;
                }

                if (isNewListManager || metaDataChanged || forceColumnCreation)
                {
                    this.BeginUpdateInternal();
                    needEndUpdate = true;

                    this.ResetRecordState();
                    InitializeColumns();
                    RefreshRows(ListManager);
                    if (this.listManager != null)
                    {
                        if (this.listManager.Position == -1)
                        {
                            this.listManager.Position = CurrentPosition = 0;
                        }
                        else
                        {
                            this.currentPosition = -1;
                            CurrentPosition = this.listManager.Position;
                        }
                    }
                    else
                    {
                        InitCurrentRecordState(-1);
                    }

                    this.EndUpdateInternal();
                    needEndUpdate = false;

                    ////                    this.ListHasErrors = this.DataGridSourceHasErrors();
                    this.OnDataSourceChanged(System.EventArgs.Empty);
                }
            }
            finally
            {
                this.gridModel.ResetVolatileData();
                inSetListManager = false;
                if (needEndUpdate)
                {
                    this.EndUpdateInternal();
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="DataSource"/> property has changed.
        /// </summary>
        public event EventHandler DataSourceChanged;

        /// <summary>
        /// Raises the <see cref="DataSourceChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnDataSourceChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource);
            }
#else
            ;
#endif
            if (DataSourceChanged != null)
            {
                DataSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs after records are removed.
        /// </summary>
        public event EventHandler RecordsRemoved;

        /// <summary>
        /// Raises the <see cref="RecordsRemoved"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnRecordsRemoved(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource);
            }
#else
            ;
#endif
            if (RecordsRemoved != null)
            {
                RecordsRemoved(this, e);
            }
        }

        private void WireDataSource()
        {
            if (this.listManager != null)
            {
                this.listManager.CurrentChanged += new EventHandler(listManager_CurrentChanged);
                this.listManager.PositionChanged += new EventHandler(listManager_PositionChanged);
                this.listManager.ItemChanged += new ItemChangedEventHandler(listManager_ItemChanged);
            }

            this.AddMetaDataChangedNotification();
        }

        private void AddMetaDataChangedNotification()
        {
            IBindingList bindingList = this.List as IBindingList;
            if (bindingList != null)
            {
                bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
            }
        }

        private void UnWireDataSource()
        {
            if (this.listManager != null)
            {
                this.listManager.CurrentChanged -= new EventHandler(listManager_CurrentChanged);
                this.listManager.PositionChanged -= new EventHandler(listManager_PositionChanged);
                this.listManager.ItemChanged -= new ItemChangedEventHandler(listManager_ItemChanged);
            }

            this.RemoveMetaDataChangedNotification();
        }

        private void RemoveMetaDataChangedNotification()
        {
            IBindingList bindingList = this.List as IBindingList;
            if (bindingList != null)
            {
                bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
            }
        }

        Control GetInvokeRequiredControl()
        {
            if (this.gridModel != null)
            {
                GridDataBoundGrid c = this.gridModel.ActiveGridView as GridDataBoundGrid;
                if (c != null && c.InvokeRequired)
                {
                    return c;
                }
            }

            return null;
        }

        /// <summary>
        /// Row in master view changed. Reload row count, rows.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="ea">Event data.</param>
        private void listManager_Changed(object sender, EventArgs ea)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this.listManager_Changed), new object[] { sender, ea });
                return;
            }

#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            ////            bool hasErrors;

            ////            hasErrors = this.ListHasErrors;
            ////            this.ListHasErrors = this.DataGridSourceHasErrors();
            ////            if (hasErrors == this.ListHasErrors)
            ////            {
            ////            if (!this.gridModel.Updating || this.gridModel.UpdateOptions != BeginUpdateOptions.None)
            if (this.IsSuspendBinding)
            {
                suspendChangeFlags |= DataSourceChangedFlag;
            }
            else
            {
                this.RootHierarchyLevel.table = List;
                this.RefreshRows(ListManager);
            }
            ////            }
        }

        const int DataSourceChangedFlag = 1;
        const int MetaDataChangedFlag = 2;
        const int DataSourcePositionChangedFlag = 4;
        const int DataSourceRowChangedFlag = 8;
        int suspendBinding = 0;
        int suspendChangeFlags = 0;

        /// <summary>
        /// Gets a value indicating whether <see cref="SuspendBinding"/> was called.
        /// </summary>
        /// <remarks>
        /// <see cref="SuspendBinding"/> and <see cref="ResumeBinding"/> are two methods that allow the
        /// temporary suspension and resumption of data binding. You would typically suspend data binding
        /// if you want to make several changes to the datasource without immediately updating the grid
        /// after each change.
        /// <para/>
        /// For example, if you want to clear out all the records in your data set and refill it with its original
        /// data, you can improve performance of this operation substantially if the grid does not need to
        /// immediately reflect every row change while the data set is filled.
        /// </remarks>
        public bool IsSuspendBinding
        {
            get
            {
                return suspendBinding > 0;
            }
        }

        /// <summary>
        /// Suspends data binding until <see cref="ResumeBinding"/> is called.
        /// </summary>
        /// <remarks>
        /// <see cref="SuspendBinding"/> and <see cref="ResumeBinding"/> are two methods that allow the
        /// temporary suspension and resumption of data binding. You would typically suspend data binding
        /// if you want to make several changes to the data source without immediately updating the grid
        /// after each change.
        /// <para/>
        /// For example, if you want to clear out all records in your data set and refill it with its original
        /// data, you can improve performance of this operation substantially if the grid does not need to
        /// immediately reflect every row change while the data set is filled.
        /// </remarks>
        public void SuspendBinding()
        {
            ////        Sample Code:
            ////        private void okButton_Click(object sender, System.EventArgs e)
            ////        {
            ////            this.gridDataBoundGrid1.Binder.SuspendBinding();
            ////            this.dataSet11.Clear();
            ////            ReadXml(@"Data\GDBDdata.XML");
            ////            this.gridDataBoundGrid1.Binder.ResumeBinding();
            ////        }
            ////
            suspendBinding++;
        }

        /// <summary>
        /// Resumes data binding after a previous <see cref="SuspendBinding"/> call.
        /// </summary>
        /// <remarks>
        /// <see cref="SuspendBinding"/> and <see cref="ResumeBinding"/> are two methods that allow the
        /// temporary suspension and resumption of data binding. You would typically suspend data binding
        /// if you want to make several changes to the data source without immediately updating the grid
        /// after each change.
        /// <para/>
        /// For example, if you want to clear out all records in your data set and refill it with its original
        /// data, you can improve performance of this operation substantially if the grid does not need to
        /// immediately reflect every row change while the data set is filled.
        /// </remarks>
        public void ResumeBinding()
        {
            if (suspendBinding > 0)
            {
                if (--suspendBinding == 0)
                {
                    if (this.OptimizeListChangedEvent)
                    {
                        if (suspendChangeFlags != 0)
                        {
                            suspendChangeFlags = 0;
                            this.bindingList_ListChanged(this.List, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
                        }
                    }
                    else
                    {
                        gridModel.BeginUpdate();
                        if ((this.suspendChangeFlags & MetaDataChangedFlag) != 0)
                        {
                            MetaDataChanged();
                        }
                        else if ((this.suspendChangeFlags & DataSourceChangedFlag) != 0)
                        {
                            listManager_Changed(this.listManager, EventArgs.Empty);
                        }
                        else if ((this.suspendChangeFlags & DataSourceRowChangedFlag) != 0)
                        {
                            listManager_CurrentChanged(this.listManager, EventArgs.Empty);
                        }
                        else if ((this.suspendChangeFlags & DataSourcePositionChangedFlag) != 0)
                        {
                            listManager_PositionChanged(this.listManager, EventArgs.Empty);
                        }

                        suspendChangeFlags = 0;
                        gridModel.EndUpdate();
                    }
                }
            }
        }

        CurrencyManager ListManager
        {
            get
            {
                if (this.BindToCurrencyManager && this.listManager == null && this.BindingContext != null && this.DataSource != null)
                {
                    return (CurrencyManager)this.BindingContext[this.DataSource, this.DataMember];
                }

                return this.listManager;
            }

            set
            {
                throw new NotSupportedException("DataGridSetListManager");
            }
        }

        internal IList List
        {
            get
            {
                if (this.BindToCurrencyManager)
                {
                    CurrencyManager cm = ListManager;
                    if (cm != null)
                    {
                        list = cm.List;
                    }
                }
                else
                {
                    if (this.list == null && DataSource != null)
                    {
                        object value = DataSource;
                        if (value is IList)
                        {
                            list = (IList)value;
                        }
                        else if (value is IListSource)
                        {
                            list = ((IListSource)value).GetList();
                        }
                    }
                }

                return this.list;
            }

            set
            {
                throw new NotSupportedException("DataGridSetListManager");
            }
        }

        private void listManager_CurrentChanged(object sender, EventArgs ea)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this.listManager_CurrentChanged), new object[] { sender, ea });
                return;
            }

            if (OptimizeListChangedEvent && this.List is IBindingList)
            {
                return;
            }

#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (inAddNew || inRemoveRecords || this.inEndEdit)
            {
                return;
            }

            if (this.IsSuspendBinding)
            {
                suspendChangeFlags |= DataSourceRowChangedFlag;
            }
            else
            {
                this.inRowChanged = true;
                listManager_PositionChanged(sender, ea);
                OnRowChanged(ea);
                this.inRowChanged = false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether in row changed. True while in <see cref="OnRowChanged"/>.
        /// </summary>
        public bool InRowChanged
        {
            get
            {
                return inRowChanged;
            }
        }

        private void listManager_PositionChanged(object sender, EventArgs ea)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this.listManager_PositionChanged), new object[] { sender, ea });
                return;
            }

            // When you raise the ListChanged event on the BindingList, the order
            // in which objects are called is unknown. In most cases, the Forms CurrencyManagers
            // event handler will get called first and after that the GridModelDataBinder.ListChanged
            // handler is called.
            // The CurrencyManager raises the PositionChanged event.
            // By default, the grid listens to PositionChanged, updates
            // the current record, and moves to the new position. This however,
            // conflicts with the ListChanged handler.
            if (!this.BindToCurrencyManager
                || (OptimizeListChangedEvent && this.List is IBindingList
                && this.GetListManagerCount() != this.currentListManagerCount))
            {
                return;
            }
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ListManager.Position, this.CurrentPosition);
            }
#else

            ;
#endif
            if (!inAddNew && !inEndEdit && !inSetCurrentPosition && this.ListManager.Position != currentListManagerPosition)
            {
                if (this.inEdit)
                {
                    EndEdit();
                }

                if (this.IsSuspendBinding)
                {
                    suspendChangeFlags |= DataSourcePositionChangedFlag;
                }
                else
                {
                    CurrentPosition = this.ListManagerPositionToPosition(this.ListManager.Position);
                }
            }
        }

        private void listManager_ItemChanged(object sender, ItemChangedEventArgs ea)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new ItemChangedEventHandler(this.listManager_ItemChanged), new object[] { sender, ea });
                return;
            }

            if (!this.BindToCurrencyManager)
            {
                return;
            }

            if (OptimizeListChangedEvent && this.listManager.List is IBindingList)
            {
                // In a CurrencyManager Master Detail setup, we need to refresh the detail table
                // when the parent position is changed. In such cases, the ListManager.List
                // will return a new DataView.
                if (ea.Index == -1 && !this.inAddNew)
                {
                    if (this.RootHierarchyLevel.table != ListManager.List)
                    {
                        this.listManager_Changed(sender, EventArgs.Empty);
                    }
                }

                return;
            }
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(ea.Index);
            }
#else
            ;
#endif

            if (this.inRemoveRecords)
            {
                return;
            }

            if (this.IsSuspendBinding)
            {
                suspendChangeFlags |= DataSourceRowChangedFlag;
                return;
            }

            if (ea.Index == -1/*0xffffffff*/)
            {
                //// && this.IsSuspendBinding)
                if (this.inAddNew)
                {
                    gridModel.ResetVolatileData();
                    gridModel.Refresh();
                    return;
                }

                if (this.RootHierarchyLevel.table != ListManager.List)
                {
                    this.listManager_Changed(sender, EventArgs.Empty);
                }

                return;
            }

            object item = this.ListManager.List[ea.Index];
            ////            bool listHasErrors = this.ListHasErrors;
            ////if (item is System.ComponentModel.IDataErrorInfo)
            ////            {
            ////                if (((IDataErrorInfo)(item)).Error.Length != 0)
            ////                    this.ListHasErrors = true;
            ////                else if (this.ListHasErrors)
            ////                    this.ListHasErrors = this.DataGridSourceHasErrors();
            ////            }

            OnItemChanged(ea); //// should redraw row then
        }

        /// <summary>
        /// Occurs when contents of a record have changed.
        /// </summary>
        public event ItemChangedEventHandler ItemChanged;

        /// <summary>
        /// Raises the <see cref="ItemChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnItemChanged(ItemChangedEventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource, e.Index);
            }
#else
            ;
#endif
            if (ItemChanged != null)
            {
                ItemChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when contents of a record have changed.
        /// </summary>
        public event EventHandler RowChanged;

        GridCurrentRecordItemChangedBehavior defaultCurrentRecordItemChangedBehavior = GridCurrentRecordItemChangedBehavior.ReloadCurrentRecord;

        /// <summary>
        /// Gets or sets the default behavior when the grid is notified from the underlying IBindingList
        /// that the data for the current record are changed.
        /// </summary>
        public GridCurrentRecordItemChangedBehavior DefaultCurrentRecordItemChangedBehavior
        {
            get
            {
                return this.defaultCurrentRecordItemChangedBehavior;
            }

            set
            {
                this.defaultCurrentRecordItemChangedBehavior = value;
            }
        }

        /// <summary>
        /// Raises the <see cref="RowChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnRowChanged(EventArgs e)
        {
#if DEBUG
            if (Switches.GridModelDataBinderEvents.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.DataSource, this.CurrentPosition);
            }
#else
            ;
#endif
            if (RowChanged != null)
            {
                RowChanged(this, e);
            }
        }

        // event GridCurrentCellListItemChangedEventHandler OnCurrentCellListItemChanged

        /// <summary>
        /// Occurs when the grid is notified from the underlying IBindingList and
        /// lets you dynamically define the behavior how to resolve conflict with
        /// pending changes in current record.
        /// </summary>
        public event GridCurrentCellListItemChangedEventHandler CurrentCellListItemChanged;

        /// <summary>
        /// Occurs when the grid is notified from the underlying IBindingList and
        /// lets you dynamically define the behavior how to resolve conflict with
        /// pending changes in current record.
        /// </summary>
        protected virtual void OnCurrentCellListItemChanged(GridCurrentCellListItemChangedEventArgs e)
        {
            if (CurrentCellListItemChanged != null)
            {
                CurrentCellListItemChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs before the <see cref="GridModelDataBinder"/> processes the <see cref="IBindingList.ListChanged"/> event
        /// of an attached source list.
        /// </summary>
        /// <remarks>
        /// The reason for firing this event is to give a programmer chance to react to a <see cref="IBindingList.ListChanged"/>
        /// event before the binder since there is otherwise no order guaranteed when an IBindingList raises a ListChanged
        /// event.
        /// </remarks>
        [Description("Occurs before the GridModelDataBinder processes the IBindingList.ListChanged event.")]
        public event ListChangedEventHandler BindingListChanged;

        /// <summary>
        /// Raises the <see cref="BindingListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnBindingListChanged(ListChangedEventArgs e)
        {
            if (BindingListChanged != null)
            {
                BindingListChanged(this, e);
            }
        }

        internal bool inbindingList_ListChanged = false;
        bool inbindingList_ListChangedCurrentRecord = false;

        int GetListManagerCount()
        {
            if (this.BindToCurrencyManager)
            {
                return this.ListManager != null ? this.ListManager.Count : 0;
            }
            else
            {
                return this.List != null ? this.List.Count : 0;
            }
        }

        PropertyDescriptorCollection GetListManagerItemProperties()
        {
            PropertyDescriptorCollection pdc = this.listManager != null ? this.listManager.GetItemProperties() : Syncfusion.Collections.ListUtil.GetItemProperties(this.dataSource);
            return pdc;
        }

        internal void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new ListChangedEventHandler(this.bindingList_ListChanged), new object[] { sender, e });
                return;
            }

            ////TraceUtil.TraceCurrentMethodInfo(e.ListChangedType, e.NewIndex, e.OldIndex);

            try
            {
                inbindingList_ListChanged = true;

                OnBindingListChanged(e);

                int rowIndex;

                int rowsPerRecord = this.GetHierarchyLevel(0).RowCountPerRecord;

                if (e.ListChangedType != ListChangedType.ItemAdded)
                {
                    this.lastAddNewIndex = -1;
                }

                switch (e.ListChangedType)
                {
                    case ListChangedType.PropertyDescriptorAdded:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= MetaDataChangedFlag;
                            return;
                        }

                        if (rowsPerRecord == 1 && OptimizeListChangedEvent && this.GetListManagerCount() > 0)
                        {
                            PropertyDescriptorCollection pdc = GetListManagerItemProperties();
                            PropertyDescriptor pd = null;
                            int index = 0;
                            int newIndex = -1;
                            foreach (PropertyDescriptor pdOrg in pdc)
                            {
                                if (!GridListUtil.PropertyDescriptorIsARelation(pdOrg))
                                {
                                    if (index == internalGridColumns.Count)
                                    {
                                        newIndex = internalGridColumns.Count;
                                        pd = pdOrg;
                                        break;
                                    }

                                    if (pdOrg.Name != internalGridColumns[index].PropertyDescriptor.Name)
                                    {
                                        newIndex = index;
                                        pd = pdOrg;
                                        break;
                                    }

                                    index++;
                                }
                            }

                            if (pd != null && newIndex >= 0)
                            {
                                GridBoundColumn internalGridColumn = InternalColumns.CreateBoundColumn(pd);
                                if (newIndex < internalGridColumns.Count)
                                {
                                    internalGridColumns.Insert(newIndex, internalGridColumn);
                                }
                                else
                                {
                                    this.internalGridColumns.Add(internalGridColumn);
                                }

                                int fieldNum = newIndex;
                                if (Object.ReferenceEquals(this.internalGridColumns, this.currentColumns))
                                {
                                    int colIndex = this.FieldToColIndex(fieldNum);
                                    GridModelInsertRangeOptions rro = new GridModelInsertRangeOptions();
                                    rro.RowColSizes = new int[] { gridModel.Cols.DefaultSize };
                                    rro.RowColHide = new bool[] { false };
                                    this.gridModel.Cols.OnRangeInserted(new GridRangeInsertedEventArgs(colIndex, 1, rro, true));
                                }

                                this.SynchronizeColCount();
                            }
                        }
                        else
                        {
                            this.MetaDataChanged();
                        }

                        break;

                    case ListChangedType.PropertyDescriptorChanged:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= MetaDataChangedFlag;
                            return;
                        }

                        if (rowsPerRecord == 1 && OptimizeListChangedEvent && this.GetListManagerCount() > 0)
                        {
                            PropertyDescriptorCollection pdc = GetListManagerItemProperties();
                            PropertyDescriptor pd = pdc[e.NewIndex];
                            GridBoundColumn internalGridColumn = InternalColumns.CreateBoundColumn(pd);
                            int fieldNum = this.internalGridColumns.Add(internalGridColumn);
                            if (Object.ReferenceEquals(this.internalGridColumns, this.currentColumns))
                            {
                                for (int n = 0; n < this.currentColumns.Count; n++)
                                {
                                    if (currentColumns[n].PropertyDescriptor == internalGridColumn.PropertyDescriptor)
                                    {
                                        currentColumns[n].PropertyDescriptor = pd;
                                        fieldNum = n;
                                        break;
                                    }
                                }
                            }

                            internalGridColumn.PropertyDescriptor = pd;
                            if (fieldNum >= 0)
                            {
                                int colIndex = this.FieldToColIndex(fieldNum);
                                this.gridModel.NotifyCellsChanged(new GridCellsChangedEventArgs(GridRangeInfo.Col(colIndex), null, true));
                            }

                            this.SynchronizeColCount();
                        }
                        else
                        {
                            this.MetaDataChanged();
                        }

                        break;

                    case ListChangedType.PropertyDescriptorDeleted:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= MetaDataChangedFlag;
                            return;
                        }

                        if (rowsPerRecord == 1 && OptimizeListChangedEvent && e.NewIndex >= 0 && e.NewIndex < this.internalGridColumns.Count)
                        {
                            PropertyDescriptorCollection pdc = GetListManagerItemProperties();
                            int index = 0;
                            int newIndex = -1;

                            // if last column is deleted, the loop below will not find a difference. Therefore
                            // set last column as initial value.
                            newIndex = internalGridColumns.Count - 1;

                            foreach (PropertyDescriptor pdOrg in pdc)
                            {
                                if (!GridListUtil.PropertyDescriptorIsARelation(pdOrg))
                                {
                                    if (pdOrg.Name != internalGridColumns[index].PropertyDescriptor.Name)
                                    {
                                        newIndex = index;
                                        break;
                                    }

                                    index++;
                                }
                            }

                            if (newIndex >= 0)
                            {
                                GridBoundColumn internalGridColumn = this.internalGridColumns[newIndex];
                                PropertyDescriptor pd = internalGridColumn.PropertyDescriptor;
                                int fieldNum = newIndex;
                                this.internalGridColumns.RemoveAt(newIndex);
                                if (!Object.ReferenceEquals(this.internalGridColumns, this.currentColumns))
                                {
                                    for (int n = 0; n < this.currentColumns.Count; n++)
                                    {
                                        if (currentColumns[n].PropertyDescriptor == pd)
                                        {
                                            this.currentColumns.RemoveAt(n);
                                            fieldNum = n;
                                            break;
                                        }
                                    }
                                }

                                int colIndex = this.FieldToColIndex(fieldNum);
                                GridModelInsertRangeOptions rro = new GridModelInsertRangeOptions();
                                rro.RowColSizes = new int[] { gridModel.ColWidths[colIndex] };
                                rro.RowColHide = new bool[] { gridModel.HideCols[colIndex] };
                                this.gridModel.Cols.OnRangeRemoved(new GridRangeRemovedEventArgs(colIndex, colIndex, rro, true));
                            }

                            this.SynchronizeColCount();
                        }
                        else
                        {
                            this.MetaDataChanged();
                        }

                        break;

                    case ListChangedType.Reset:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= DataSourcePositionChangedFlag | DataSourceRowChangedFlag;
                            return;
                        }

                        this.ResetRecordState();
                        this.gridModel.Refresh();
                        ////this.ResetHierarchyLevels();
                        ////this.MetaDataChanged();
                        this.list = null;
                        break;

                    case ListChangedType.ItemMoved:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= DataSourcePositionChangedFlag | DataSourceRowChangedFlag;
                            return;
                        }

                        if (OptimizeListChangedEvent)
                        {
                            if (!this.ForgetAboutRecordState())
                            {
                                goto case ListChangedType.Reset;
                            }

                            if (e.OldIndex < 0)
                            {
                                goto case ListChangedType.ItemAdded;
                            }
                            else if (e.NewIndex < 0)
                            {
                                goto case ListChangedType.ItemDeleted;
                            }

                            int pos1 = this.PositionToRowIndex(e.OldIndex);
                            int pos2 = this.PositionToRowIndex(e.NewIndex);
                            this.gridModel.ResetVolatileData();
                            this.gridModel.Rows.OnRangeMoved(new GridRangeMovedEventArgs(pos1, rowsPerRecord, pos2, true));
                            ////this.gridModel.ActiveGridView.Update();
                        }

                        break;

                    case ListChangedType.ItemChanged:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= DataSourceRowChangedFlag;
                            return;
                        }

                        if (e.NewIndex != -1)
                        {
                            ////TraceUtil.TraceCurrentMethodInfo(e.ListChangedType, e.NewIndex, e.OldIndex);
                            rowIndex = PositionToRowIndex(e.NewIndex);
                            if (e.NewIndex == this.currentPosition && !this.inEndEdit
                                && this.gridModel.ActiveGridView != null)
                            {
                                GridCurrentCell gcc = this.gridModel.ActiveGridView.CurrentCell;
                                if (gcc.RowIndex >= rowIndex && gcc.RowIndex <= rowIndex + rowsPerRecord - 1)
                                {
                                    inbindingList_ListChangedCurrentRecord = true;
                                    try
                                    {
                                        GridCurrentCellListItemChangedEventArgs ea = new GridCurrentCellListItemChangedEventArgs(e.NewIndex, this.IsEditing, gcc, DefaultCurrentRecordItemChangedBehavior);
                                        OnCurrentCellListItemChanged(ea);

                                        bool keepChanges = (ea.CurrentRecordItemChangedBehavior & GridCurrentRecordItemChangedBehavior.KeepCurrentRecord) != GridCurrentRecordItemChangedBehavior.None;
                                        bool keepCurrentCell = (ea.CurrentRecordItemChangedBehavior & GridCurrentRecordItemChangedBehavior.KeepCurrentCellText) != GridCurrentRecordItemChangedBehavior.None;
                                        
                                        if (keepCurrentCell && gcc.HasCurrentCell)
                                        {
                                            string newText = this.gridModel[gcc.RowIndex, gcc.ColIndex].Text;
                                            string oldText = gcc.Renderer.ControlText;
                                            if (newText != oldText)
                                            {
                                                if (!gcc.IsModified)
                                                {
                                                    if (!this.IsEditing)
                                                    {
                                                        this.BeginEdit();
                                                    }

                                                    if (!gcc.IsModified)
                                                    {
                                                        gcc.BeginEdit();
                                                    }

                                                    gcc.IsModified = true;
                                                }
                                            }
                                        }
                                        else if (!keepChanges && !gcc.IsInConfirmChanges && (e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.PropertyDescriptorDeleted || !gcc.IsEditing))
                                        {
                                            if (gcc.IsEditing)
                                            {
                                                gcc.CancelEdit();
                                            }

                                            if (this.IsEditing)
                                            {
                                                this.CancelEdit();
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        inbindingList_ListChangedCurrentRecord = false;
                                    }
                                }
                            }

                            this.gridModel.InvalidateRange(GridRangeInfo.Rows(rowIndex, rowIndex + rowsPerRecord - 1), GridRangeOptions.MergeAllSpannedCells);
                        }

                        break;

                    case ListChangedType.ItemDeleted:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= DataSourcePositionChangedFlag | DataSourceRowChangedFlag;
                            return;
                        }

                        if (OptimizeListChangedEvent)
                        {
                            Debug.Assert(e.NewIndex != -1);
                            if (e.NewIndex != -1)
                            {
                                int gridPosition = this.ListManagerPositionToPosition(e.NewIndex);
                                int nextGridPosition = this.ListManagerPositionToPosition(e.NewIndex + 1);
                                int rowIndex2 = this.PositionToRowIndex(nextGridPosition) - 1;
                                int recordCount = nextGridPosition - gridPosition;
                                rowIndex = this.PositionToRowIndex(gridPosition);
                                GridModelInsertRangeOptions rro = new GridModelInsertRangeOptions();
                                rro.RowColSizes = new int[rowsPerRecord];
                                for (int n = 0; n < rowsPerRecord; n++)
                                {
                                    rro.RowColSizes[n] = gridModel.Rows.DefaultSize;
                                }

                                rro.RowColHide = new bool[0];
                                if (inAddNew && this.currentPosition == gridPosition)
                                {
                                    rowIndex++;
                                    this.gridModel.ResetVolatileData();
                                    this.gridModel.Rows.OnRangeRemoved(new GridRangeRemovedEventArgs(rowIndex, rowIndex2, rro, true));
                                }
                                else
                                {
                                    if (this.currentPosition == gridPosition)
                                    {
                                        this.gridModel.InvalidateRange(GridRangeInfo.Row(rowIndex), GridRangeOptions.None);
                                    }

                                    if (this.gridModel.ActiveGridView != null)
                                    {
                                        int ccRowIndex = this.gridModel.ActiveGridView.CurrentCell.RowIndex;
                                        if (ccRowIndex >= rowIndex && ccRowIndex <= rowIndex + rowsPerRecord - 1)
                                        {
                                            this.gridModel.ActiveGridView.CurrentCell.ResetCurrentCellWithoutDeactivate();
                                            this.gridModel.ActiveGridView.CurrentCell.SetCurrentCellNoActivate(-1, -1);
                                        }
                                    }

                                    if (!this.ForgetAboutRecordState())
                                    {
                                        GridBoundRecordState state = (GridBoundRecordState)recordStates[gridPosition];

                                        this.expRecordCount = Math.Max(0, expRecordCount - recordCount + 1);
                                        recordStates.RemoveRange(gridPosition, recordCount);
                                        gridModel.Rows.OnRangeRemoving(new GridRangeRemovingEventArgs(rowIndex, rowIndex2));

                                        for (int n = gridPosition; n < recordStates.Count; n++)
                                        {
                                            GridBoundRecordState bst = (GridBoundRecordState)recordStates[n];
                                            if (bst.level == 0)
                                            {
                                                bst.position--;
                                            }
                                        }

                                        if (this.RootHierarchyLevel.rowCount == 1
                                            || this.levels.Count > 1)
                                        {
                                            ReduceListManagerToGridPositionsAt(gridPosition + 1, recordCount - 1);
                                        }
                                    }

                                    this.gridModel.ResetVolatileData();
                                    this.gridModel.Rows.OnRangeRemoved(new GridRangeRemovedEventArgs(rowIndex, rowIndex2, rro, true));

                                    if (this.currentPosition >= gridPosition)
                                    {
                                        SetCurrentPosition(this.currentPosition - 1, true);
                                    }

                                    currentListManagerPosition = currentPosition;
                                    InitCurrentRecordState(currentPosition);
                                }
                            }
                        }

                        break;

                    case ListChangedType.ItemAdded:
                        if (this.IsSuspendBinding)
                        {
                            suspendChangeFlags |= DataSourcePositionChangedFlag | DataSourceRowChangedFlag;
                            return;
                        }

                        if (OptimizeListChangedEvent)
                        {
                            /*if (CurrentRecordManager.InBeginEdit)
                        {
                        }
                        else */
                            if (e.NewIndex != -1)
                            {
                                // A DataView will send two ItemAdded events when adding a record at the end
                                // of the table. Both events will have the same Index.
                                if (e.NewIndex == this.lastAddNewIndex && e.NewIndex == this.RecordCount - 1)
                                {
                                    goto case ListChangedType.ItemChanged;
                                }

                                lastAddNewIndex = e.NewIndex;

                                int gridPosition = this.ListManagerPositionToPosition(e.NewIndex);
                                rowIndex = this.PositionToRowIndex(gridPosition);
                                GridModelInsertRangeOptions iro = new GridModelInsertRangeOptions();
                                iro.RowColSizes = new int[rowsPerRecord];
                                for (int n = 0; n < rowsPerRecord; n++)
                                {
                                    iro.RowColSizes[n] = gridModel.Rows.DefaultSize;
                                }

                                iro.RowColHide = new bool[0];

                                if (this.inAddNew || e.NewIndex == GetListManagerCount() - 1)
                                {
                                    if (inAddNew)
                                    {
                                        rowIndex += rowsPerRecord;
                                    }

                                    this.gridModel.ResetVolatileData();
                                    this.gridModel.Rows.OnRangeInserted(new GridRangeInsertedEventArgs(rowIndex, rowsPerRecord, iro, true));
                                }
                                else
                                {
                                    if (this.gridModel.ActiveGridView != null && !this.inAddNew)
                                    {
                                        int ccRowIndex = this.gridModel.ActiveGridView.CurrentCell.RowIndex;
                                        if (ccRowIndex >= rowIndex)
                                        {
                                            this.gridModel.ActiveGridView.CurrentCell.ResetCurrentCellWithoutDeactivate();
                                            this.gridModel.ActiveGridView.CurrentCell.SetCurrentCellNoActivate(-1, -1);
                                        }
                                    }

                                    if (!this.ForgetAboutRecordState())
                                    {
                                        for (int n = 0; n < rowsPerRecord; n++)
                                        {
                                            GridBoundRecordState state = new GridBoundRecordState(null);
                                            state.table = this.List;
                                            state.listManager = this.listManager;
                                            state.expanded = false;
                                            state.position = e.NewIndex;
                                            state.childList = null;
                                            state.row = n;
                                            state.hasChildList = this.levels.Count > 1; ////this.relation != null;
                                            state.level = 0;
                                            recordStates.Insert(gridPosition + n, state);
                                        }

                                        gridModel.Rows.OnRangeInserting(new GridRangeInsertingEventArgs(rowIndex, rowsPerRecord, iro));

                                        for (int n = gridPosition + rowsPerRecord; n < recordStates.Count; n++)
                                        {
                                            GridBoundRecordState bst = (GridBoundRecordState)recordStates[n];
                                            if (bst.level == 0)
                                            {
                                                bst.position++;
                                            }
                                        }

                                        IncreaseListManagerToGridPositionsAt(gridPosition + 1, rowsPerRecord);
                                    }

                                    this.gridModel.ResetVolatileData();
                                    this.gridModel.Rows.OnRangeInserted(new GridRangeInsertedEventArgs(rowIndex, rowsPerRecord, iro, true));

                                    if (this.currentPosition >= gridPosition)
                                    {
                                        SetCurrentPosition(this.currentPosition + 1, true);
                                    }

                                    currentListManagerPosition = currentPosition;
                                    InitCurrentRecordState(currentPosition);
                                }
                            }
                        }

                        break;
                }

                this.currentListManagerCount = this.GetListManagerCount();

                gridModel.ResetVolatileData();
////&& e.ListChangedType != ListChangedType.ItemDeleted
                if (OptimizeListChangedEvent && gridModel.ActiveGridView != null                    
                    && gridModel.ActiveGridView.Visible
                    && ForceUpdateAfterListChangedEvent)
                {
                    if (e.ListChangedType == ListChangedType.ItemChanged)
                    {
                        //// Avoid ViewLayout.Reset being called from OnPaint. Only cells were changed.
                        //// No need to recaulcate cell coordinates ...
                        gridModel.activeGridView.ViewLayout.Lock();
                        gridModel.ActiveGridView.Update();
                        gridModel.activeGridView.ViewLayout.Unlock();
                    }
                    else
                    {
                        gridModel.ActiveGridView.Update();
                    }
                }
            }
            finally
            {
                inbindingList_ListChanged = false;
            }
        }

        int currentListManagerCount = 0;
        int lastAddNewIndex = -1;

        private void MetaDataChanged()
        {
            if (this.IsSuspendBinding)
            {
                suspendChangeFlags |= MetaDataChangedFlag;
            }
            else
            {
                this.metaDataChanged = true;
                try
                {
                    this.Set_ListManager(this.DataSource, this.DataMember, true);
                }
                finally
                {
                    this.metaDataChanged = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether is design mode. True if component is designed inside a designer; False otherwise.
        /// </summary>
        public bool IsDesignMode
        {
            get
            {
                return isDesignMode;
            }

            set
            {
                isDesignMode = value;
            }
        }

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether DoNotDisposeLists. Used internally.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool DoNotDisposeLists
        {
            get
            {
                return doNotDisposeLists;
            }

            set
            {
                doNotDisposeLists = value;
            }
        }

        #region ICurrencyManagerSource Members

        CurrencyManager ICurrencyManagerSource.GetCurrencyManager()
        {
            return this.ListManager;
        }

        #endregion
    }

    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    class GridListUtil
    {
        public static ListSortDirection GetSortDirection(IList list)
        {
            if (list is IBindingList && ((IBindingList)list).SupportsSorting)
            {
                return ((IBindingList)list).SortDirection;
            }

            return ListSortDirection.Ascending;
        }

        public static PropertyDescriptor GetSortProperty(IList list)
        {
            if (list is IBindingList && ((IBindingList)list).SupportsSorting)
            {
                return ((IBindingList)list).SortProperty;
            }

            return null;
        }

        public static void SetSort(IList list, PropertyDescriptor property, ListSortDirection sortDirection)
        {
            if (list is IBindingList && ((IBindingList)list).SupportsSorting)
            {
                ((IBindingList)list).ApplySort(property, sortDirection);
            }
        }

        public static bool SupportsSort(IList list)
        {
            return list is IBindingList && ((IBindingList)list).SupportsSorting;
        }

        public static bool GetAllowRemove(IList list)
        {
            if (list is System.ComponentModel.IBindingList)
            {
                return ((IBindingList)list).AllowRemove;
            }

            if (list == null)
            {
                return false;
            }

            if (!list.IsReadOnly)
            {
                return !list.IsFixedSize;
            }

            return false;
        }

        public static bool PropertyDescriptorIsARelation(PropertyDescriptor prop)
        {
            if (typeof(IList).IsAssignableFrom(prop.PropertyType))
            {
                return !typeof(Array).IsAssignableFrom(prop.PropertyType);
            }

            return false;
        }

        // static PropertyInfo drpi = null;
        static MethodInfo mInfo = null;

        public static DataRelation GetRelation(PropertyDescriptor pd)
        {
            if (pd.GetType().FullName == "System.Data.DataRelationPropertyDescriptor")
            {
                if (mInfo == null)
                {
                    Type t = pd.GetType();
                    PropertyInfo pi = t.GetProperty("Relation", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreReturn | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (pi != null)
                    {
                        mInfo = pi.GetGetMethod(true);
                    }
                }

                if (mInfo != null)
                {
                    return mInfo.Invoke(pd, new object[0]) as DataRelation;
                }
            }

            return null;
        }

        internal  static PropertyDescriptorCollection GetInstanceProperties(Type type, bool isCollection)
        {
            object item = null;
            try
            {
                ConstructorInfo ci = type.GetConstructor(new Type[0]);
                if (ci != null)
                {
                    item = Activator.CreateInstance(type);
                }
            }
            catch (MissingMethodException ex)
            {
                // no parameterless constructor found.
                // Now I have to fallback on meta data descriptors (without support for ICustomTypeDescriptor)
                TraceUtil.TraceExceptionCatched(ex);
            }

            if (item != null)
            {
                if (isCollection)
                {
                    return GetItemProperties(item);
                }
                else
                {
                    return TypeDescriptor.GetProperties(item, new Attribute[] { new BrowsableAttribute(true) }, false);
                }
            }

            return TypeDescriptor.GetProperties(type, new Attribute[] { new BrowsableAttribute(true) });
        }

        public static PropertyDescriptorCollection GetItemProperties(Type type)
        {
            if (type == null)
            {
                return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            }
            else if (typeof(ITypedList).IsAssignableFrom(type))
            {
                return GetInstanceProperties(type, true);
            }
            else if (typeof(System.Array).IsAssignableFrom(type))
            {
                return GetInstanceProperties(type.GetElementType(), false);
            }
            else
            {
                PropertyInfo[] propertyInfos = type.GetProperties();
                for (int index = 0; index < propertyInfos.Length; index++)
                {
                    if ("Item".Equals(propertyInfos[index].Name)
                        && propertyInfos[index].PropertyType != typeof(object))
                    {
                        return GetInstanceProperties(propertyInfos[index].PropertyType, false);
                    }
                }
            }

            return PropertyDescriptorCollection.Empty;
        }

        public static PropertyDescriptorCollection GetItemProperties(object dataSource)
        {
            if (dataSource == null)
            {
                return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            }
            else
            {
                IList list = dataSource as IList;
                if (list == null && dataSource is IListSource)
                {
                    list = ((IListSource)dataSource).GetList();
                }

                if (dataSource is ITypedList)
                {
                    return ((ITypedList)dataSource).GetItemProperties(null);
                }
                else if (list is ITypedList)
                {
                    return ((ITypedList)list).GetItemProperties(null);
                }
                else if (list != null)
                {
                    PropertyDescriptorCollection pdc = PropertyDescriptorCollection.Empty;
                    if (list.Count > 0)
                    {
                        pdc = TypeDescriptor.GetProperties(list[0], new Attribute[] { new BrowsableAttribute(true) });
                    }

                    if (pdc.Count == 0)
                    {
                        pdc = GetItemProperties(list.GetType());
                    }
                    //// fyi - in Whidbey there is now a ListBindingHelper.GetListItemProperties(list);

                    if (pdc.Count > 0)
                    {
                        return pdc;
                    }
                }

                return GetItemProperties(dataSource.GetType());
            }
        }
    }

    /// <summary>
    /// The <see cref="GridHierarchyLevel"/> class holds information about a hierarchy level in a
    /// grid. A grid has at least one root level. If there are nested relations inside the grid
    /// the grid will hold an additional GridHierarchyLevel for each relation.
    /// </summary>
    /// <remarks>
    /// The grid provides access to <see cref="GridHierarchyLevel"/> though the <see cref="GridModelDataBinder.RootHierarchyLevel"/>
    /// property and <see cref="GridModelDataBinder.GetHierarchyLevel(string)"/> and <see cref="GridModelDataBinder.AddRelation"/> functions
    /// of a <see cref="GridDataBoundGrid.Binder"/>. <para/>
    /// <see cref="GridModelDataBinder.HierarchyLevelCount"/> will provide you with number of hierarchy levels.
    /// </remarks>
    public class GridHierarchyLevel
    {
        internal GridHierarchyLevel(GridModelDataBinder binder, int n)
        {
            level = n;
            this.binder = binder;
            internalGridColumns = binder.CreateBoundColumnsCollection();
            gridColumns = binder.CreateBoundColumnsCollection();
            ////this.gridColumns.CollectionChanged += new CollectionChangeEventHandler(GridColumnsCollectionChanged);
            this.gridModel = binder.gridModel;
        }

        /*        public void RefreshItemProperties(object table)
                {
                    RefreshItemProperties(table, GridListUtil.GetItemProperties(table));
                }

                public void RefreshItemProperties(object table, PropertyDescriptorCollection pdc)
                {
                    TraceUtil.TraceCurrentMethodInfoIf(Switches.Development.TraceVerbose);
                    this.table = table;

                    try
                    {
                        this.itemProperties = pdc;
                        int count = this.gridColumns.Count;
                        for (int index = 0; index < count; index++)
                        {
                            string mappingName = this.gridColumns[index].MappingName;
                            if (mappingName.Length > 0 && this.gridColumns[index].PropertyDescriptor != null)
                                this.gridColumns[index].PropertyDescriptor = this.itemProperties[mappingName];
                        }

                        count = this.InternalColumns.Count;
                        for (int index = 0; index < count; index++)
                        {
                            string mappingName = this.InternalColumns[index].MappingName;
                            if (mappingName.Length > 0 && this.InternalColumns[index].PropertyDescriptor != null)
                                this.InternalColumns[index].PropertyDescriptor = this.itemProperties[mappingName];
                        }
                    }
                    finally
                    {
                    }
                }*/

        internal PropertyDescriptorCollection itemProperties;
        internal int level;
        internal GridModelDataBinder binder;
        internal GridBoundColumnsCollection internalGridColumns;
        internal GridBoundColumnsCollection gridColumns;
        internal int[] coveredCells;
        internal GridModel gridModel;
        internal PropertyDescriptor relation; // contains relation descriptor for sublevel
        internal bool showHeaders = true;
        internal object table;
        internal GridStyleInfo headerStyle = new GridStyleInfo();
        internal GridStyleInfo rowStyle = new GridStyleInfo();
        internal int rowCount = 1;
        internal int headerTopRow = -1;
        int[] lineBreaks = new int[0];
        bool inInitializeColumns = false;

        /// <summary>
        /// Gets the appearance of rows that belong to this relation.
        /// </summary>
        public GridStyleInfo RowStyle
        {
            get
            {
                return rowStyle;
            }
        }

        /// <summary>
        /// Gets the appearance of column headers for rows that belong to this relation.
        /// </summary>
        public GridStyleInfo HeaderStyle
        {
            get
            {
                return headerStyle;
            }
        }

        /// <summary>
        /// Gets Relation descriptor for sublevel
        /// </summary>
        public PropertyDescriptor Relation
        {
            get
            {
                return relation;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="GridBoundColumn"/> objects for this relation.
        /// This can be either the columns specified by the user or if no columns were specified, it returns all
        /// columns that were automatically propagated from the datasource.
        /// </summary>
        public GridBoundColumnsCollection InternalColumns
        {
            get
            {
                return gridColumns.Count == 0 ? internalGridColumns : gridColumns;
            }
        }

        /// <summary>
        /// Gets or sets a collection of <see cref="GridBoundColumn"/> objects in the <see cref="GridDataBoundGrid"/> control.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [LocalizableAttribute(true)]
        public GridBoundColumnsCollection GridBoundColumns
        {
            get
            {
                return gridColumns;
            }

            set
            {
                //// TODO: GridModelDataBinder.set_GridBoundColumns should call this setter

                if (gridColumns != value)
                {
                    gridColumns = value;
                    this._InitializeColumns(this.table, this.itemProperties);
                    OnGridBoundColumnsChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when columns have been added or removed.
        /// </summary>
        public event EventHandler GridBoundColumnsChanged;

        /// <summary>
        /// Raises the <see cref="GridBoundColumnsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        void OnGridBoundColumnsChanged(EventArgs e)
        {
            if (GridBoundColumnsChanged != null)
            {
                GridBoundColumnsChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the zero-based index for this hierarchy level.
        /// </summary>
        public int LevelIndex
        {
            get
            {
                return level;
            }
        }

        /// <summary>
        /// Gets Number of rows per record.
        /// </summary>
        public int RowCountPerRecord
        {
            get
            {
                return rowCount;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether headers for this relation should be shown or hidden.
        /// </summary>
        public bool ShowHeaders
        {
            get
            {
                return showHeaders;
            }

            set
            {
                showHeaders = value;
                binder.SynchronizeRowHeaderCount();
                binder.OnGridBoundColumnsChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns the number of columns that this relation displays in the grid. If a record
        /// is broken into several rows, the column count will be the maximum column count for all
        /// rows in the record.
        /// </summary>
        /// <returns>The number of columns that this relation displays in the grid.</returns>
        public int GetColCount()
        {
            if (rowCount == 1)
            {
                return InternalColumns.Count;
            }

            int colCount = 0;
            int last = 0;
            for (int n = 0; n < rowCount; n++)
            {
                colCount = Math.Max(this.lineBreaks[n] - last - 1, colCount);
                last = this.lineBreaks[n];
            }

            return colCount;
        }

        /// <summary>
        /// Calculates a zero-based field number that can be used as an index in the <see cref="InternalColumns"/> based
        /// on the zero-based row index in the record and the zero-based field column in the grid.
        /// </summary>
        /// <param name="row">Zero-base row index.</param>
        /// <param name="fieldNum">Zero-based field index in the grid. If you have an absolute column index,
        /// you should first convert it with <see cref="GridModelDataBinder.ColIndexToField"/>.</param>
        /// <returns>A zero-based field number that can be used as an index to access a <see cref="GridBoundColumn"/>
        /// in <see cref="InternalColumns"/>.</returns>
        public int RowFieldToField(int row, int fieldNum)
        {
            if (lineBreaks == null || lineBreaks.Length == 0)
            {
                return fieldNum;
            }

            if (row >= lineBreaks.Length)
            {
                return -1;
            }

            int last = row > 0 ? lineBreaks[row - 1] : 0;
            if (fieldNum < 0 || last + fieldNum >= lineBreaks[row] - 1)
            {
                return -1;
            }

            return last + fieldNum;
        }

        /// <summary>
        /// Calculates the zero-based row index in the record and the zero-based field column in the grid base on a
        /// zero-based field number that can be used as an index in the <see cref="InternalColumns"/>.
        /// </summary>
        /// <param name="fieldNum">A field number that can be used as an index in the <see cref="InternalColumns"/>.</param>
        /// <param name="row">Returns the zero-based row index in the record.</param>
        /// <returns>Returns the zero-based field number in the grid. You can call <see cref="GridModelDataBinder.FieldToColIndex"/>
        /// to get the associated column index.</returns>
        public int FieldToRowField(int fieldNum, out int row)
        {
            if (rowCount == 1)
            {
                row = 0;
                return fieldNum;
            }

            int last = 0;
            for (row = 0; row < rowCount; row++)
            {
                if (fieldNum < this.lineBreaks[row])
                {
                    return fieldNum - last;
                }

                last = this.lineBreaks[row];
            }

            return -1;
        }

        /// <summary>
        /// Rearranges how columns are displayed in the grid and allows you to specify covered cells
        /// and / or break records into several rows displayed in the grid.
        /// </summary>
        /// <param name="mappingNames">A string array with field names. <para/>
        /// The following strings have a specific meaning: <para/>
        /// "-" specifies a covered cell.<para/>
        /// "." indicated line break inside the record. Subsequent fields will be displayed in another row.<para/>
        /// "" specifies an empty "whitespace" column.<para/>
        /// Other than these, you should use the same mapping names that you also use with <see cref="GridBoundColumn"/> objects.
        /// </param>
        /// <example>
        /// See the "MultiRowRecord" and "ExpandGrid" examples for sample code.
        /// </example>
        public void LayoutColumns(string[] mappingNames)
        {
            // rearrange bound columns collection
            GridBoundColumnsCollection newColumns = binder.CreateBoundColumnsCollection();
            GridBoundColumnsCollection columns = this.InternalColumns;
            this.rowCount = 1;

            ArrayList breaks = new ArrayList();

            binder.ResetRecordState();

            for (int n = 0; n < mappingNames.Length; n++)
            {
                int found = -1;
                if (mappingNames[n] != null && mappingNames[n].Length > 0)
                {
                    if (mappingNames[n] == ".")
                    {
                        breaks.Add(n + 1);
                        this.rowCount++;
                    }
                    else if (mappingNames[n] != "-")
                    {
                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (columns[i].MappingName == mappingNames[n])
                            {
                                found = i;
                                break;
                            }
                        }
                    }
                }

                if (found != -1)
                {
                    newColumns.Add(columns[found]);
                }
                else
                {
                    GridBoundColumn column = this.InternalColumns.CreateBoundColumn(null);
                    if (GridUtil.IsEmpty(mappingNames[n]))
                    {
                        column.StyleInfo.Enabled = false;
                    }

                    newColumns.Add(column);
                }
            }

            this.gridColumns = newColumns;
            breaks.Add(newColumns.Count + 1);
            this.lineBreaks = (int[])breaks.ToArray(typeof(int));

            // covered cells pattern
            this.coveredCells = new int[newColumns.Count];
            for (int n = 0; n < coveredCells.Length; n++)
            {
                coveredCells[n] = -1;
            }

            int lastField = -1;

            for (int n = 0; n < mappingNames.Length; n++)
            {
                if (mappingNames[n] != null && mappingNames[n] == "-")
                {
                    if (lastField != -1)
                    {
                        this.coveredCells[n] = lastField;
                        this.coveredCells[lastField] = lastField;
                    }
                }
                else
                {
                    lastField = n;
                }
            }

            binder.SynchronizeColCount();
            binder.SynchronizeRowHeaderCount();
            binder.OnGridBoundColumnsChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Creates a new grid column for the specified <see cref="PropertyDescriptor"/>.
        /// </summary>
        /// <param name="pd">The <see cref="PropertyDescriptor"/> for this column.</param>
        void CreateGridColumn(PropertyDescriptor pd)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(pd.Name);
            }
#else
            ;
#endif
            this.internalGridColumns.Add(this._CreateGridColumn(pd));
        }

        GridBoundColumn _CreateGridColumn(PropertyDescriptor pd)
        {
            GridBoundColumn gcs = this.InternalColumns.CreateBoundColumn(pd);
            _InitGridColumn(gcs, pd);
            return gcs;
        }

        void _InitGridColumn(GridBoundColumn gcs, PropertyDescriptor pd)
        {
            bool autoInitCellType = GridModelDataBinder.AutoInitCellTypes;

            gcs.PropertyDescriptor = pd;
            Type type = pd.PropertyType;
            if (gcs.StyleInfo.IsEmpty)
            {
                gcs.StyleInfo.CellValueType = pd.PropertyType;
                if (type.Equals(typeof(Boolean)))
                {
                    if (autoInitCellType)
                    {
                        gcs.StyleInfo.CellType = "CheckBox";
                        gcs.StyleInfo.HorizontalAlignment = GridHorizontalAlignment.Center;
                    }
                }
                else if (type.Equals(typeof(String)))
                {
                }
                else if (type.Equals(typeof(DateTime)))
                {
                    gcs.StyleInfo.Format = "d";
                    if (autoInitCellType)
                    {
                        gcs.StyleInfo.CellType = "MonthCalendar";
                    }
                }
                else if (type.Equals(typeof(Int16)) || type.Equals(typeof(Int32)) || type.Equals(typeof(Int64)) || type.Equals(typeof(UInt16)) || type.Equals(typeof(UInt32)) || type.Equals(typeof(UInt64)) || type.Equals(typeof(Decimal)) || type.Equals(typeof(Double)) || type.Equals(typeof(Single)) || type.Equals(typeof(Byte)) || type.Equals(typeof(SByte)))
                {
                    gcs.StyleInfo.Format = "G";
                    gcs.StyleInfo.HorizontalAlignment = GridHorizontalAlignment.Right;
                    gcs.StyleInfo.RightToLeft = RightToLeft.No;
                }
                else if (type == typeof(Decimal))
                {
                    gcs.StyleInfo.HorizontalAlignment = GridHorizontalAlignment.Right;
                    gcs.StyleInfo.RightToLeft = RightToLeft.No;
                }
            }
            else if (!gcs.StyleInfo.HasCellValueType)
            {
                gcs.StyleInfo.CellValueType = pd.PropertyType;
            }

                      if (gcs.MappingName.Contains(".") && this.binder.UseComplexBinding)
            {
                gcs.MappingName = gcs.MappingName;
            }
            else
            {
                gcs.MappingName = pd.Name;
            }

        }

        bool IsRightToLeft()
        {
            return this.gridModel.ActiveGridView != null && this.gridModel.ActiveGridView.IsRightToLeft();
        }

        void _InitializeColumns(GridBoundColumnsCollection boundColums, ITypedList list)
        {
            _InitializeColumns(boundColums, list != null ? list.GetItemProperties(null) : new PropertyDescriptorCollection(new PropertyDescriptor[0]));
        }

        void _InitializeColumns(GridBoundColumnsCollection boundColums, PropertyDescriptorCollection pdc)
        {
            this.itemProperties = pdc;
            int count = itemProperties.Count;
            for (int index = 0; index < count; index++)
            {
                PropertyDescriptor pd = itemProperties[index];
                if (pd.IsBrowsable)
                {
                    if (GridListUtil.PropertyDescriptorIsARelation(pd))
                    {
#if DEBUG
                        Trace.WriteLine("Nested Relation found: " + pd.Name);
#endif
                    }
                    else
                    {
                        GridBoundColumn column = _CreateGridColumn(pd);
                        boundColums.Add(column);
                    }
                }
            }
        }

        internal void _InitializeColumns(object table)
        {
            _InitializeColumns(table, GridListUtil.GetItemProperties(table));
        }

        internal void _InitializeColumns(object table, PropertyDescriptorCollection pdc)
        {
#if DEBUG
            if (Switches.Development.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo();
            }
#endif
            if (!this.inInitializeColumns)
            {
                this.table = table;
                try
                {
                    this.gridModel.BeginUpdate(0);
                    this.inInitializeColumns = true;
                    this.internalGridColumns.Clear();
                    if (this.gridColumns.Count == 0)
                    {
                        this._InitializeColumns(this.internalGridColumns, pdc);
                        this.binder.SynchronizeColCount();
                    }
                    else
                    {
                        this.itemProperties = pdc;
                        int count = this.gridColumns.Count;
                        for (int i = 0; i < count; i++)
                        {
                            string mappingName = this.gridColumns[i].MappingName;
                            if (mappingName.Length > 0)
                            {
                                bool flag = false;
                                for (int j = 0; j < this.itemProperties.Count; j++)
                                {
                                    if (this.itemProperties[j].Name == mappingName)
                                    {
                                        this._InitGridColumn(this.gridColumns[i], this.itemProperties[j]);
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    GridBoundColumn column = this.gridColumns[i];
                                    PropertyDescriptor propertyDescriptor = this.GetPropertyDescriptor(this.itemProperties, column);
                                    if ((pdc != null) && this.binder.UseComplexBinding)
                                    {
                                        this._InitGridColumn(this.gridColumns[i], propertyDescriptor);
                                    }
                                    else
                                    {
                                        this.gridColumns[i].PropertyDescriptor = null;
                                    }
                                }
                            }
                        }
                        this.binder.SynchronizeColCount();
                    }
                    this.gridModel.Data.RowCount = 1;
                    this.binder.SynchronizeRowHeaderCount();
                }
                finally
                {
                    this.inInitializeColumns = false;
                    this.gridModel.EndUpdate(true);
                }
            }

        }
        private PropertyDescriptor GetPropertyDescriptor(PropertyDescriptorCollection pdc, GridBoundColumn column)
        {
            if (column.MappingName.Contains("."))
            {
                string[] strArray = column.MappingName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length > 0)
                {
                    string text1 = strArray[strArray.Length - 1];
                    PropertyDescriptorCollection descriptors = null;
                    for (int i = 0; i <= (strArray.Length - 2); i++)
                    {
                        descriptors = pdc;
                        foreach (PropertyDescriptor descriptor in descriptors)
                        {
                            if (descriptor.Name.Equals(strArray[i]))
                            {
                                pdc = GridListUtil.GetInstanceProperties(descriptor.PropertyType, false);
                                goto Label_00A2;
                            }
                        }
                    Label_00A2:
                        if (pdc.Equals(descriptors))
                        {
                            return null;
                        }
                    }
                    string str = strArray[strArray.Length - 1];
                    foreach (PropertyDescriptor descriptor2 in pdc)
                    {
                        if (descriptor2.Name.Equals(str))
                        {
                            return descriptor2;
                        }
                    }
                }
            }
            return null;
        }

 


        internal GridHierarchyLevel AddRelation(string nestedDataMember)
        {
            PropertyDescriptorCollection pdc = this.itemProperties;
            int count = pdc.Count;
            for (int index = 0; index < count; index++)
            {
                PropertyDescriptor pd = pdc[index];
                if (pd.IsBrowsable)
                {
                    if (GridListUtil.PropertyDescriptorIsARelation(pd))
                    {
                        if (pd.Name == nestedDataMember)
                        {
                            this.relation = pd;
                            IList list = table as IList;
                            if (list != null && list.Count > 0)
                            {
                                gridModel.BeginUpdate();
                                GridHierarchyLevel level = new GridHierarchyLevel(this.binder, binder.levels.Count);
                                binder.levels.Add(level);
                                object ob = pd.GetValue(list[0]);
                                level._InitializeColumns(ob);
                                binder.ResetRecordState();
                                binder.OnGridBoundColumnsChanged(EventArgs.Empty);
                                gridModel.EndUpdate();
                                return level;
                            }
                            else
                            {
                                //// There is no list I can query so I do instead have to get the "Relation"
                                //// from DataRelationPropertyDescriptor.
                                ////Trace.WriteLine(pd.ToString());
                                DataRelation dr = GridListUtil.GetRelation(pd);
                                if (dr != null && dr.ChildTable != null)
                                {
                                    gridModel.BeginUpdate();
                                    GridHierarchyLevel level = new GridHierarchyLevel(this.binder, binder.levels.Count);
                                    binder.levels.Add(level);
                                    ITypedList bindingList = dr.ChildTable.DefaultView as ITypedList;
                                    level._InitializeColumns(bindingList);
                                    binder.ResetRecordState();
                                    binder.OnGridBoundColumnsChanged(EventArgs.Empty);
                                    gridModel.EndUpdate();
                                    return level;
                                }
                                else
                                {
                                    // Maybe a strong typed collection?
                                    // In that case either ITypedList needs to be implement
                                    // or referenced type must have a default ctor without arguments.
                                    PropertyDescriptorCollection pds = GridListUtil.GetItemProperties(pd.PropertyType);
                                    if (pds.Count > 0)
                                    {
                                        gridModel.BeginUpdate();
                                        GridHierarchyLevel level = new GridHierarchyLevel(this.binder, binder.levels.Count);
                                        binder.levels.Add(level);
                                        level._InitializeColumns((object)null, pds);
                                        binder.ResetRecordState();
                                        binder.OnGridBoundColumnsChanged(EventArgs.Empty);
                                        gridModel.EndUpdate();
                                        return level;
                                    }
                                }
                            }

                            return null;
                        }
                    }
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Holds information about
    /// the hierarchy level and the record displayed at a specific row. Each row in the grid is associated with a
    /// <see cref="GridBoundRecordState"/>. Use the <see cref="GridModelDataBinder.GetRecordStateAtRowIndex"/> to
    /// get access to state information of a row.
    /// </summary>
    public class GridBoundRecordState : IDisposable
    {
        internal GridBoundRecordState parent;
        internal IList table;
        internal int position;
        internal int row;
        internal bool expanded;
        internal IList childList; // childRowList has Count
        internal BindingManagerBase listManager; // DataView
        internal bool hasChildList;
        internal int level;
        internal int childCount;  // save it in case childList count changes (e.g. if foreign key is changed)

        internal GridBoundRecordState(GridBoundRecordState parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets a reference to the record state of the parent node.
        /// </summary>
        public GridBoundRecordState Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Gets a reference to the list this record belongs to. (E.g. DataRowView when you browse a DataTable).
        /// </summary>
        public IList Table
        {
            get
            {
                return table;
            }
        }

        /// <summary>
        /// Gets a reference to the ListManager for the <see cref="Table"/> this record belongs to.
        /// </summary>
        public BindingManagerBase ListManager
        {
            get
            {
                return listManager;
            }
        }

        /// <summary>
        /// Gets the zero-based record index in the the list this row belongs to.
        /// </summary>
        public int Position
        {
            get
            {
                return position;
            }
        }

        /// <summary>
        /// Gets the zero-based row index within a record if the record spans over multiple rows.
        /// </summary>
        public int RowIndexInRecord
        {
            get
            {
                return row;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this node is expanded.
        /// </summary>
        public bool Expanded
        {
            get
            {
                return expanded;
            }
        }

        /// <summary>
        /// Gets a reference to the child list for this node. (E.g. DataRowView when you browse a DataTable).
        /// </summary>
        public IList ChildList
        {
            get
            {
                return childList;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this node has any children.
        /// </summary>
        public bool HasChildList
        {
            get
            {
                return hasChildList;
            }
        }

        /// <summary>
        /// Gets the zero-based index for the hierarchy level. Use <see cref="GridModelDataBinder.GetHierarchyLevel(string)"/>
        /// to get access to the related <see cref="GridHierarchyLevel"/>.
        /// </summary>
        public int LevelIndex
        {
            get
            {
                return level;
            }
        }

        /// <summary>
        /// Gets the number of child records for this node.
        /// </summary>
        public int ChildCount
        {
            get
            {
                return childCount;
            }
        }
        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            if (!GridModelDataBinder.DoNotDisposeLists && this.childList is IDisposable)
            {
                ((IDisposable)childList).Dispose();
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// A method that represents a handler for the <see cref="GridModelDataBinder.CurrentCellListItemChanged"/> event.
    /// </summary>
    public delegate void GridCurrentCellListItemChangedEventHandler(object sender, GridCurrentCellListItemChangedEventArgs e);

    /// <summary>
    /// Lets you dynamically define the behavior when the grid is notified from the underlying IBindingList
    /// that the data for the current records are changed.
    /// </summary>
    public sealed class GridCurrentCellListItemChangedEventArgs : SyncfusionEventArgs
    {
        int recordIndex;
        bool isEditing;
        GridCurrentCell gcc;
        GridCurrentRecordItemChangedBehavior currentRecordItemChangedBehavior;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="recordIndex">The current record position.</param>
        /// <param name="isEditing">Indicates if current record was edited by user.</param>
        /// <param name="gcc">A reference to the GridCurrentCell, lets you check gcc.IsModified, for example.</param>
        /// <param name="currentRecordItemChangedBehavior">Specifies how the grid should resolve this conflict.</param>
        public GridCurrentCellListItemChangedEventArgs(int recordIndex, bool isEditing, GridCurrentCell gcc, GridCurrentRecordItemChangedBehavior currentRecordItemChangedBehavior)
        {
            this.recordIndex = recordIndex;
            this.isEditing = isEditing;
            this.gcc = gcc;
            this.currentRecordItemChangedBehavior = currentRecordItemChangedBehavior;
        }

        /// <summary>
        /// Gets the current record position.
        /// </summary>
        [TraceProperty(true)]
        public int RecordIndex
        {
            get
            {
                return recordIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether current record was edited by user.
        /// </summary>
        [TraceProperty(true)]
        public bool IsEditing
        {
            get
            {
                return isEditing;
            }
        }

        /// <summary>
        /// Gets a reference to the GridCurrentCell, lets you check CurrentCell.IsModified, for example.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentCell CurrentCell
        {
            get
            {
                return gcc;
            }
        }

        /// <summary>
        /// Gets or sets how the grid should resolve this conflict.
        /// </summary>
        [TraceProperty(true)]
        public GridCurrentRecordItemChangedBehavior CurrentRecordItemChangedBehavior
        {
            get
            {
                return currentRecordItemChangedBehavior;
            }

            set
            {
                currentRecordItemChangedBehavior = value;
            }
        }
    }
}