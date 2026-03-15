#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
//#define TestDrawTextPerformance

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Diagnostics;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Styles;
    using System.Globalization;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Xml;
    using System.IO;
    using System.Windows.Controls;
    using Syncfusion.Windows.Data;
    using System.Diagnostics;

    /// <summary>
    /// This is the <see cref="GridModel"/> class that holds all data information about a grid and provides methods to completely initialize a grid
    /// and attach it later to a <see cref="GridControlBase"/> so that its contents can be rendered to the screen.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridModel"/> holds all data for a grid. It offers many events that you can subscribe to and modify the default behavior of the grid.
    /// A <see cref="GridModel"/> holds a <see cref="GridCellModelCollection"/> that holds models for all cell types used in the grid.
    /// If a cell queries for a new cell type that is not found in the cell model collection, it tries to instantiate a <see cref="GridCellModelBase"/>
    /// object for the specific cell type. A <see cref="GridModel.QueryCellModel"/> event is raised to allow you to create custom cell
    /// types on demand. But you can also instantiate cell models for custom cell types at initialization and add them to <see cref="GridModel.CellModels"/>.
    /// <para/>
    /// There is also a <see cref="GridControl"/> class that combines both <see cref="GridModel"/> and <see cref="GridControlBase"/> into one class. This
    /// gives you easier access to all methods of <see cref="GridModel"/>.
    /// </remarks>
    /// <seealso cref="GridControlBase"/>
    /// <seealso cref="GridControl"/>
    [Serializable]
    public class GridModel : Disposable, IGridVolatileCellStylesHost, IOperationFeedbackProvider
    {

        #region Fields
        GridStyleInfo tableStyle = new GridStyleInfo();
        GridStyleInfo headerStyle = new GridStyleInfo();
        GridStyleInfo footerStyle = new GridStyleInfo();
        IPaddedEditableLineSizeHost rowHeights;
        IPaddedEditableLineSizeHost columnWidths;
        GridCoveredCellInfoCollection coveredCells;
        GridOverlappingCellInfoCollection overlappingCells;
        GridCellSpanBackgroundInfoCollection cellSpanBackgrounds;
        GridCellData data = new GridCellData();
        GridVolatileCellStyles volatileCellStyles;
        GridStyleInfoIndexer rowStyles;
        GridStyleInfoIndexer colStyles;
        int headerRows = 1;
        int headerColumns = 1;
        internal bool isLoaded = false;
        GridBaseStylesMap styleInfoMap = null;
        bool ignoreReadOnly;
        Pen gridLinePen;
        #endregion

#if TestDrawTextPerformance
        public bool SupportsQueryCoveredCellCallback = false;
#else
        /// <summary>
        /// Set this false if you want to skip query covered cell calculations when 
        /// covered ranges list is empty. WHen this is true the grid will assume there could be a QueryCoveredRange override
        /// (e.g. for group captions etc.). So be carefull with this optimization!
        /// </summary>        
        private bool supportsQueryCoveredCellCallback = true;
        public bool SupportsQueryCoveredCellCallback
        {
            get
            {
                return this.supportsQueryCoveredCellCallback;
            }
            set
            {
                this.supportsQueryCoveredCellCallback = value;
            }
        }
#endif

        // Cache scroll values for grid in cell scenarios only.
        double vScrollValue = 0.0;

        /// <summary>
        /// Gets or sets cached vertical scroll value.
        /// </summary>
        public double CachedVScrollValue
        {
            get { return vScrollValue; }
            set { vScrollValue = value; }
        }

        double hScrollValue = 0.0;

        /// <summary>
        /// Gets or sets cached horizontal scroll value.
        /// </summary>
        public double CachedHScrollValue
        {
            get { return hScrollValue; }
            set { hScrollValue = value; }
        }

        bool enableFormulaCalculations = true;

        /// <summary>
        /// Suspend or resume the formula calculation while rendering the grid
        /// </summary>
        internal bool EnableFormulaCalculations
        {
            get { return enableFormulaCalculations; }
            set { enableFormulaCalculations = value; }
        }

        /// <summary>
        /// Gets <see cref="GridVolatileCellStyles"/> that defines an interface that <see cref="IGridVolatileCellStylesHost"/>
        /// utilizes to query cell contents and base styles, look up cell types, and save changes back to the grid.
        /// </summary>
        [XmlIgnore]
        public GridVolatileCellStyles VolatileCellStyles
        {
            get { return volatileCellStyles; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to make changes to Read-only cells.
        /// Set this True if you want to be able make changes to Read-only cells.
        /// </summary>
        public bool IgnoreReadOnly
        {
            get { return ignoreReadOnly; }
            set { ignoreReadOnly = value; }
        }

        private GridFormulaEngine formulaEngine;

        [XmlIgnore]
        public GridFormulaEngine FormulaEngine
        {
            get
            {
                if(formulaEngine == null)
                    this.formulaEngine = ((GridCellFormulaModel)this.CellModels["FormulaCell"]).Engine;
                return formulaEngine;
            }
            set
            {
                formulaEngine = value;
            }
        }

        protected virtual void SuspendFormattedTextCalculation()
        {
            SuspendFormulaParsingAndCalculation = true;
        }

        protected virtual void ResumeFormattedTextCalculation()
        {
            SuspendFormulaParsingAndCalculation = false;
        }

        /// <summary>
        /// Suspend Formaula parsing and calculation when we read the formatted from the GridStyleInfo
        /// </summary>
        internal bool SuspendFormulaParsingAndCalculation
        {
            get;
            set;
        }

#if !SILVERLIGHT
        public bool EnableContextMenu
        {
            get;
            set;
        }
        public bool DisableEditorsContextMenu
        {
            get;
            set;
        }
#endif
        #region Ctor
        /// <summary>
        /// Initializes a new <see cref="GridModel"/>.
        /// </summary>
        public GridModel()
        {
#if !SILVERLIGHT
            this.EnableContextMenu = false;
            this.DisableEditorsContextMenu = false;
#endif
            this.columnWidths = this.OnCreateColumnWidths();
            this.rowHeights = this.OnCreateRowHeights();
            this.Options.CopyPasteOption |= CopyPaste.CopyText;
            this.Options.CopyPasteOption |= CopyPaste.CutText;
            this.Options.CopyPasteOption |= CopyPaste.PasteText;
            volatileCellStyles = CreateVolatileCellStyles();
            coveredCells = new GridCoveredCellInfoCollection(this);
            overlappingCells = new GridOverlappingCellInfoCollection(this);
            cellSpanBackgrounds = new GridCellSpanBackgroundInfoCollection(this);
            selections = new GridModelSelections(this);
            this.rowStyles = new GridStyleInfoIndexer();
            this.colStyles = new GridStyleInfoIndexer();
            RowHeights.DefaultLineSize = 20;
            ColumnWidths.DefaultLineSize = 80;
            RowCount = 1;
            ColumnCount = 1;

            HeaderRows = 1;
            HeaderColumns = 1;
            FrozenRows = 1;
            FrozenColumns = 1;

            gridLinePen = new Pen(Brushes.DarkGray, 0.5d);
            gridLinePen.Freeze();

            tableStyle.CellType = "TextBox";
            tableStyle.BorderMargins.Top = gridLinePen.Thickness;
            tableStyle.BorderMargins.Left = gridLinePen.Thickness;
            tableStyle.BorderMargins.Right = gridLinePen.Thickness / 2;
            tableStyle.BorderMargins.Bottom = gridLinePen.Thickness / 2;
            tableStyle.Background = Brushes.White;
            tableStyle.Borders.Right = gridLinePen;
            tableStyle.Borders.Bottom = gridLinePen;

            headerStyle.Background = SystemColors.ControlBrush;
            headerStyle.CellType = "Static";
            this.ColumnWidths.LineHiddenChanged += new HiddenRangeChangedEventHandler(ColumnWidths_LineHiddenChanged);
            this.RowHeights.LineHiddenChanged += new HiddenRangeChangedEventHandler(RowHeights_LineHiddenChanged);
            gridLinePen = null;
            isLoaded = true;
        }

        void ColumnWidths_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            var hiddenColRanges = new ObservableCollection<GridRangeInfo>();
            if (this.ColumnWidths.LineCount > 1)
            {
                for (int i = 0; i < this.ColumnWidths.LineCount; i++)
                {
                    if (this.ColumnWidths[i] == 0 && this.ShouldHideColumn(i, false))
                    {
                        hiddenColRanges.Add(GridRangeInfo.Col(i));
                    }
                }
            }

            this.HiddenColRanges = new ReadOnlyObservableCollection<GridRangeInfo>(hiddenColRanges);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetColumnHideCommand(this, e.From, e.To, !e.Hide));
            }
        }

        protected virtual bool ShouldHideColumn(int colIndex, bool hide)
        {
            return true;
        }

        void RowHeights_LineHiddenChanged(object sender, HiddenRangeChangedEventArgs e)
        {
            var hiddenRowRanges = new ObservableCollection<GridRangeInfo>();

            if (this.RowHeights.LineCount > 1)
            {
                for (int i = 0; i < this.RowHeights.LineCount; i++)
                {
                    if (this.RowHeights[i] == 0 /* && this.ShouldHideRow(i, false)*/)  // fix for SD 7917 This is commented seriously as it loops for the entire records
                    {                                                                  // and this consumes nearly 10 times of the actual time taken to expand the records. 
                        hiddenRowRanges.Add(GridRangeInfo.Row(i));                     // When we add 5000 records in Grid and try to expand it takes nearly 30 seconds. 
                    }                                                                  // But without calling this method it takes only 2 seconds.
                }
            }

            this.HiddenRowRanges = new ReadOnlyObservableCollection<GridRangeInfo>(hiddenRowRanges);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                if (!(e.From == 0 && e.To == this.RowHeights.LineCount))
                    CommandStack.Push(new GridModelSetRowHideCommand(this, e.From, e.To, !e.Hide));                
            }
        }

        internal bool IsRowHidden(int rowIndex, bool hide)
        {
            return this.ShouldHideRow(rowIndex, hide);
        }

        protected virtual bool ShouldHideRow(int rowIndex, bool hide)
        {
            return true;
        }

        #region Column Autosizer
        private GridColumnAutoSizer sizer;
        public GridColumnAutoSizer Sizer
        {
            get
            {
                if (this.sizer == null)
                {
                    this.sizer = this.CreateAutoSizer();
                }

                return this.sizer;
            }
        }

        protected virtual GridColumnAutoSizer CreateAutoSizer()
        {
            return new GridColumnAutoSizer(this);
        }

        internal void UpdateAutoSizer(bool applySizes, bool columnSizerChanged)
        {
            this.OnUpdateAutoSizer(applySizes, columnSizerChanged);
        }

        protected virtual void OnUpdateAutoSizer(bool applySizes, bool columnSizerChanged)
        {
            if (this.Options.ColumnSizer != GridControlLengthUnitType.None)
            {
                if (applySizes)
                {
                    this.Sizer.ApplySizes();
                }
            }
        }

        #endregion

        #region ExcelLikeDragDrop

        [Description("Occurs when the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and before the data are applied to the grid."),
        Category("Behavior")]
        public event GridOleDropAtRowColEventHandler OleDropAtRowCol;

        /// <summary>
        /// Raises the  <see cref="OleDropAtRowCol"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridOleDropAtRowColEventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, this, e);
            if (OleDropAtRowCol != null)
            {
                OleDropAtRowCol(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseOleDropAtRowCol(GridOleDropAtRowColEventArgs e)
        {
            OnOleDropAtRowCol(e);
        }

        /// <summary>
        /// Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and
        /// the data were applied to the grid.
        /// </summary>
        [Description("Occurs after the user releases the mouse over a cell at the end of an OLE drag-and-drop operation and the data were applied to the grid."),
        Category("Behavior")]
        public event EventHandler OleDroppedData;

        /// <summary>
        /// Raises the <see cref="OleDroppedData"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnOleDroppedData(EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (OleDroppedData != null)
            {
                OleDroppedData(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseOleDroppedData(EventArgs e)
        {
            OnOleDroppedData(e);
        }

        public event GridQueryOleDataSourceDataEventHandler QueryOleDataSourceData;

        protected virtual void OnQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {

            if (!CanGridRaiseEvents)
            {
                return;
            }

            if (QueryOleDataSourceData != null)
            {
                QueryOleDataSourceData(this, e);
            }
        }

        public void RaiseQueryOleDataSourceData(GridQueryOleDataSourceDataEventArgs e)
        {
            OnQueryOleDataSourceData(e);
        }

        /// <summary>
        /// Occurs when the user drops data onto another control using OLE drag-and-drop
        /// and does not press the Control Key. Set e.Cancel = True for this event if you do not
        /// want the grid to clear cell contents of the dragged cells.
        /// </summary>
        [Description("Occurs when a user drops data onto another control using OLE drag-and-drop and does not press the Control Key."),
        Category("Behavior")]
        public event CancelEventHandler QueryDragDropMoveClearCells;

        /// <summary>
        /// Raises the <see cref="QueryDragDropMoveClearCells"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            if (QueryDragDropMoveClearCells != null)
            {
                QueryDragDropMoveClearCells(this, e);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void RaiseQueryDragDropMoveClearCells(CancelEventArgs e)
        {
            OnQueryDragDropMoveClearCells(e);
        }

        public class InternalGridDragDropData
        {
            public bool dndSource = false;

            public int dndStartRow = 0;

            public int dndStartCol = 0;

            public GridRangeInfoList dndSelList = null;

            public int dndForceDropCol = 0;

            public int dndForceDropRow = 0;

            public int dndRowOffset = 0;

            public int dndColOffset = 0;

            public bool dndCurrentCellText = false;       //// True if selected text from current cell is dragged

            public IGridCellRenderer dndCurrentCellControl = null;

            public static bool dndGridSource = false;   //// True if grid is a data source.

            public static int dndRowsCopied = 0;   ///// Number of rows / cols copied in OnDndCacheGlobalData.

            public static int dndColsCopied = 0;

            public static bool dndGridTargetStyle = false;   //// True if grid was a target and style info was copied.

            public bool directDragDrop = false;
        }

        InternalGridDragDropData dragDropModel;

        public InternalGridDragDropData DragDropData
        {
            get
            {
                if (dragDropModel == null)
                {
                    dragDropModel = new InternalGridDragDropData();
                }

                return dragDropModel;
            }
        }
        #endregion

        #region UndoRedo

        [NonSerialized]
        internal bool selectionStateChanged = false;

        [NonSerialized]
        internal GridControlBase activeGridView = null;

        [NonSerialized]
        internal bool inInit = false;

        [NonSerialized]
        internal string updateCommand = null;

        [NonSerialized]
        GridModelCommandManager commandStack = null;

        /// <summary>
        /// Gets or sets the active grid view.
        /// </summary>
        /// <remarks>
        /// If there are several views associated with this model, only one <see cref="GridControlBase"/> can be active.
        /// <para/>
        /// Changing the active view will result in calls to <see cref="GridCurrentCell.Deactivate"/>
        /// and <see cref="GridCurrentCell.Activate"/> for the involved controls.
        /// <para/>
        /// </remarks>
        [XmlIgnore]
        public GridControlBase ActiveGridView
        {
            get
            {
                return activeGridView;
            }

            set
            {
                if (activeGridView != value)
                    activeGridView = value;
            }
        }

        [NonSerialized]
        int suspendRecordUndo = 0;

        /// <summary>
        /// Suspend logging undo information.
        /// </summary>
        [DebuggerStepThrough()]
        public void SuspendRecordUndo()
        {
            suspendRecordUndo++;
        }

        /// <summary>
        /// Resume logging undo information.
        /// </summary>
        [DebuggerStepThrough()]
        public void ResumeRecordUndo()
        {
            if (suspendRecordUndo > 0)
            {
                suspendRecordUndo--;
            }
        }

        /// <summary>
        /// Gets a value indicating whether undo information should be logged.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool ShouldRecordUndo
        {
            [DebuggerStepThrough()]
            get
            {
                return !inInit && suspendRecordUndo == 0;
            }
        }

        /// <summary>
        /// Gets undo and redo in the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridModelCommandManager CommandStack
        {
            get
            {
                if (commandStack == null)
                {
                    commandStack = new GridModelCommandManager(this);
                }

                return commandStack;
            }
        }

        protected virtual void SetGridModelCommandManager(GridModel gridModel)
        {
            commandStack = new GridModelCommandManager(gridModel);
        }

        internal bool canDisposeGrid = true;

        /// <summary>
        /// Records current selection state - current cell and selected ranges. Will be used for restoring selections when performing undo / redo,
        /// </summary>
        /// <param name="currentRow">The row index of current cell.</param>
        /// <param name="currentCol">The column index of current cell.</param>
        /// <param name="ranges">The current list of selected ranges.</param>
        public void ChangeSelectionState(int currentRow, int currentCol, GridRangeInfo[] ranges)
        {
            // called from CommandStack when doing an undo or redo
            this.Selections.Clear(true);
            if (ActiveGridView != null)
            {
                ActiveGridView.CurrentCell.MoveTo(currentRow, currentCol);
            }

            foreach (GridRangeInfo range in ranges)
            {
                if (range != null && !range.IsEmpty)
                {
                    this.Selections.Add(range);
                }
            }
        }

        /// <overload>
        /// Applies a text to the specified range of cells.
        /// </overload>
        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <remarks>
        /// <see cref="GridModel.ChangeCells"/> will reset volatile data cache,
        /// generate undo information, force recalculation of floating cells, and
        /// raise <see cref="GridModel.CellsChanging"/> and <see cref="GridModel.CellsChanged"/> method.
        /// <para/>
        /// When you change cells directly with an indexer, this results in a call to <see cref="GridModel.ChangeCells"/>
        /// with modifyType set to <see cref="StyleModifyType.Changes"/>.
        /// <para/>
        /// </remarks>
        /// <example>
        /// The following example assigns a previously create style with a bold font to a cell:
        /// <code lang="C#">
        ///             GridStyleInfo boldFontStyle = new GridStyleInfo();
        ///             boldFontStyle.TextColor =  Color.FromArgb(238, 122, 3);
        ///             boldFontStyle.Font = boldFont;
        ///             model[rowIndex, 1].Text = "Interior";
        ///             model.ChangeCells(GridRangeInfo.Cell(rowIndex, 1), boldFontStyle);
        ///             </code>
        /// </example>
        public virtual bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo, StyleModifyType modifyType)
        {
            bool success = false;
            GridStyleInfo[] savedCellsInfo = null;    // will be filled with style setting
            //// start op, generate undo info
            OperationFeedback op = new OperationFeedback(this);
            op.Name = "ChangeCells";
            op.Description = Syncfusion.Windows.Controls.Grid.Resources.SR.GetString("DescriptionChangeCells", range);
            op.AllowCancel = CommandStack.IsRecording || !CommandStack.Enabled;
            op.AllowRollback = CommandStack.IsRecording;

            try
            {
                //// bool bIsMouseAction = GetHitState() > 0;  // op has a AllowProgress setting
                GridRangeInfo intRange = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
                int dwSize = intRange.Width * intRange.Height;

                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    try
                    {
                        savedCellsInfo = GetCellsInfo(intRange);
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }

                        return false;
                    }
                }

                GridRangeInfo restoreRange = GridRangeInfo.Empty;
                //ResetVolatileData();
                try
                {
                    int cellIndex = 0;
                    int counter = 0;
                    for (int rowIndex = intRange.Top; rowIndex <= intRange.Bottom; rowIndex++)
                    {
                        for (int colIndex = intRange.Left; colIndex <= intRange.Right; colIndex++)
                        {
                            if (cellsInfo.Length > 1)
                            {
                                cellIndex = ((rowIndex - intRange.Top) * intRange.Width) + (colIndex - intRange.Left);
                            }

                            if (cellIndex >= cellsInfo.Length)
                            {
                                break;
                            }

                            GridStyleInfo cellInfo = cellsInfo[cellIndex];
                            if (cellInfo != null)
                            {
                                success |= SetCellInfo(rowIndex, colIndex, cellInfo, modifyType);
                            }
                            else
                            {
                                success |= SetCellInfo(rowIndex, colIndex, null, StyleModifyType.Remove);
                            }
                            op.PercentComplete = (int)((++counter) * 100 / dwSize);
                            if (op.ShouldCancel)
                            {
                                if (rowIndex == range.Top)
                                {
                                    restoreRange = GridRangeInfo.Cells(rowIndex, range.Left, rowIndex, colIndex);
                                }
                                else
                                {
                                    restoreRange = GridRangeInfo.Cells(range.Top, range.Left, rowIndex, range.Right);
                                }

                                throw new ArgumentException();
                            }
                        }
                    }
                }
                catch (ArgumentException ucex)
                {
                    TraceUtil.TraceExceptionCatched(ucex);
                    if (!ExceptionManager.RaiseExceptionCatched(this, ucex))
                    {
                        throw;
                    }

                    if (success && savedCellsInfo != null)
                    {
                        if (CommandStack.IsRecording)
                        {
                            CommandStack.Mode = GridCommandMode.Rollback;
                            ChangeCells(restoreRange, savedCellsInfo, StyleModifyType.Copy);
                            CommandStack.Mode = GridCommandMode.Recording;
                            savedCellsInfo = null;
                        }

                        success = false;
                    }
                }
                if (CommandStack.ShouldGenerateUndoInfo && savedCellsInfo != null)
                {
                    CommandStack.Push(new GridChangeCellsCommand(this, range, savedCellsInfo, StyleModifyType.Copy));
                }
            }
            finally
            {
                op.Close();
            }

            return success;
        }

        public virtual GridStyleInfo[] GetCellsInfo(GridRangeInfo range)
        {
            try
            {
                int size = range.Width * range.Height;
                GridStyleInfo[] cellsInfo = new GridStyleInfo[size];
                for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                {
                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        GridStyleInfo styleInfo = new GridStyleInfo();
                        int dwIndex = ((rowIndex - range.Top) * range.Width) + (colIndex - range.Left);
                        if (GetCellInfo(rowIndex, colIndex, styleInfo))
                        {
                            cellsInfo[dwIndex] = styleInfo;
                        }
                    }
                }

                return cellsInfo;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                return null;
            }
        }

        public bool GetCellInfo(int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (ActiveGridView != null)
            {
                var renderstyle = ActiveGridView.GetRenderStyleInfo(rowIndex, colIndex);
                if (renderstyle != null)
                {
                    style.Store.ModifyStyle(renderstyle.Store, StyleModifyType.ApplyNew);
                }
                return true;
            }
            return false;
        }

        public bool SetCellInfo(int rowIndex, int colIndex, GridStyleInfo style, StyleModifyType modifyType)
        {
            if (style != null)
            {
                GridStyleInfoIdentity identity = style.Identity as GridStyleInfoIdentity;

                if (identity == null || identity.Data == null || !identity.OffLine)
                {
                    // Avoid recursive calls when style is changed within OnSaveCellInfo.
                    // Otherwise, if identity is not offline, changing a property will
                    // result in OnStyleChanged firing again and a call to ChangeCells.
                    // GridStyleInfo.OnStyleChanged checks for offline flag and will not
                    // call SetCellInfo if the style is offline.
                    identity = new GridStyleInfoIdentity(this.VolatileCellStyles, rowIndex, colIndex, true);
                    GridStyleInfoStore store = (GridStyleInfoStore)style.Store;
                    if (style.Identity == null)
                    {
                        style.Identity = identity;
                    }
                    else
                    {
                        style = new GridStyleInfo(identity, store);
                    }
                }
            }
            try
            {
                int r = rowIndex;
                int c = colIndex;

                GridStyleInfoStore store = null;
                if (modifyType == StyleModifyType.Remove || (style == null && modifyType == StyleModifyType.Copy))
                {
                    store = Data[r, c];
                    if (store == null || 0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                    {
                        Data[r, c] = null;
                    }
                }
                else
                {
                    if (style == null)
                    {
                        throw new ArgumentNullException("style");
                    }

                    if (modifyType != StyleModifyType.Copy)
                    {
                        store = Data[r, c];
                    }

                    if (store != null)
                    {
                        if (0 == store.GetShortValue(GridStyleInfoStore.ReadOnlyProperty))
                        {
                            store.ModifyStyle(style.Store, modifyType);
                        }
                    }
                    else
                    {
                        GridStyleInfoStore store2 = (GridStyleInfoStore)style.Store;
                        if (r <= this.RowCount && c <= this.ColumnCount)
                        {
                            if (store2 == null)
                            {
                                Data[r, c] = store2;
                            }
                            else
                            {
                                Data[r, c] = (GridStyleInfoStore)store2.Clone();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            // return false; Unreachable code.
        }

        /// <summary>
        /// Applies a text to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="textValue">The text to be saved in cells.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, string textValue)
        {
            GridStyleInfo cellInfo = new GridStyleInfo();
            cellInfo.Text = textValue;
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, StyleModifyType.Changes);
        }

        /// <summary>
        /// Applies a style to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellInfo">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="modifyType">A <see cref="StyleModifyType"/> that specifies the style operation to be performed.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo cellInfo, StyleModifyType modifyType)
        {
            return ChangeCells(range, new GridStyleInfo[] { cellInfo }, modifyType);
        }

        /// <summary>
        /// Applies an array of styles to the specified range of cells.
        /// </summary>
        /// <param name="range">A <see cref="GridRangeInfo"/> that specifies the range of cells.</param>
        /// <param name="cellsInfo">The array of <see cref="GridStyleInfo"/> objects that holds cell information.</param>
        /// <returns>A <see cref="System.Boolean"/> that indicates if the operation was successful.</returns>
        /// <genoverload/>
        public bool ChangeCells(GridRangeInfo range, GridStyleInfo[] cellsInfo)
        {
            return ChangeCells(range, cellsInfo, StyleModifyType.Changes);
        }
        #endregion

        /// <summary>
        /// Clears cell styles for a given list
        /// </summary>
        /// <param name="gridRangeInfoList"></param>
        /// <param name="clearStyles"></param>
        /// <returns></returns>
        internal bool ClearCells(GridRangeInfoList gridRangeInfoList, bool clearStyles)
        {
            foreach (GridRangeInfo range in gridRangeInfoList)
            {
                for (int i = range.Left; i <= range.Right; i++)
                {
                    for (int j = range.Top; j <= range.Bottom; j++)
                    {
                        if (clearStyles)
                        {
                            this[j, i] = new GridStyleInfo();
                        }
                        else
                        {
                            GridStyleInfo style = this[j, i];
                            style.ApplyFormattedText(string.Empty);
                        }
                    }
                }
            }

            this.InvalidateVisual(true);
            return true;
        }
        [XmlIgnore]
        Func<GridModel, GridVolatileCellStyles> volatileCellStylesFactoryMethod;

        /// <summary>
        /// This Factory Method setter lets you replace the default GridVolatileData store
        /// with your own custom volatile data store derived from IGridVolatileData.
        /// </summary>
        /// <remarks>
        /// <example>
        /// This example shows how to attach a CustomVolatileData object derived from IGridVolatileData.
        /// <code lang="C#">
        /// public Window1()
        /// {
        ///     InitializeComponent();
        /// 
        ///     // Replace default GridVolatileData with CustomVolatileData
        ///     grid.Model.VolatileCellStylesFactoryMethod = delegate(GridModel model)
        ///     {
        ///         return new GridVolatileCellStyles(model, new CustomVolatileData());
        ///     };
        /// 
        ///     // a really large row and column count.
        ///     grid.Model.RowCount = 9900;
        ///     grid.Model.ColumnCount = 100; // 1 million
        /// 
        ///     // fill cell contents on demand.
        ///     grid.Model.QueryCellInfo += new GridQueryCellInfoEventHandler(Model_QueryCellInfo);
        /// }
        /// </code>
        /// <code lang="VB">
        /// Public Sub New()
        ///     InitializeComponent()
        /// 
        ///     ' Replace default GridVolatileData with CustomVolatileData
        ///     grid.Model.VolatileCellStylesFactoryMethod = 
        ///         Function(model As GridModel) New GridVolatileCellStyles(model, New CustomVolatileData())
        /// 
        ///     ' a really large row and column count.
        ///     grid.Model.RowCount = 9900
        ///     grid.Model.ColumnCount = 100
        ///     ' 1 million
        ///     ' Resize millions of rows instantly - pixel scrolling is updated accordingly.
        ///     grid.Model.TableStyle.CellType = "TextBox"
        ///     ' see checkBoxRows_Checked below for instantly hiding most of the 99 million rows
        /// 
        ///     AddHandler grid.Model.QueryCellInfo, AddressOf Model_QueryCellInfo
        /// End Sub
        /// </code>
        /// </example>
        /// </remarks>
        [XmlIgnore]
        public Func<GridModel, GridVolatileCellStyles> VolatileCellStylesFactoryMethod
        {
            get { return volatileCellStylesFactoryMethod; }
            set
            {
                if (volatileCellStylesFactoryMethod != value)
                {
                    volatileCellStylesFactoryMethod = value;

                    // recreate GridVolatileCellStyles if necessaery
                    if (volatileCellStyles != null)
                    {
                        volatileCellStyles = CreateVolatileCellStyles();
                    }
                }
            }
        }

        /// <summary>
        /// Creates the GridVolatileCellStyles objects which allocates style objects and maintains a weak reference to them.<para/>
        /// Override this method to provide your own custom custom volatile data store or assign a delegate as shown in <see cref="VolatileCellStylesFactoryMethod"/> method. The default version for CreateVolatileCellStyles checks VolatileCellStylesFactoryMethod property and calls the delegate.
        /// </summary>
        /// <returns></returns>
        protected virtual GridVolatileCellStyles CreateVolatileCellStyles()
        {
            if (VolatileCellStylesFactoryMethod != null)
                return VolatileCellStylesFactoryMethod.Invoke(this);

            return new GridVolatileCellStyles(this);
        }
        private bool isDisposed = false;
        /// <summary>
        /// Occurs when this component is about to be disposed.
        /// </summary>
        public event EventHandler Disposing;

        protected virtual void OnDisposing()
        {
            EventHandler handler = Disposing;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.Views != null)
                {
                    foreach (GridControlBase grid in this.Views)
                    {
                        var eventsHost = grid as IGridModelEventsHost;
                        if (eventsHost != null)
                        {
                            eventsHost.OnDisposing(disposing);
                        }
                        GridTooltipService.Dispose(grid);
                        GridCommentService.Dispose(grid);
                    }
                }
                if (this.BaseStylesMap != null)
                {
                    this.styleInfoMap.Dispose();
                    this.styleInfoMap = null;
                }
                if (this.cellModels != null)
                {
                    this.cellModels.Dispose();
                    this.cellModels = null;
                }
                if (this.collectionToIListTable !=null)
                {
                    this.collectionToIListTable.Clear();
                    this.collectionToIListTable = null;
                }
                if (this.cellSpanBackgrounds !=null)
                {
                    this.cellSpanBackgrounds.Clear();
                    this.cellSpanBackgrounds = null;
                }
                if (this.colStyles != null)
                {
                    this.colStyles = null;
                }
                if (this.columnWidths != null)
                {
                    this.columnWidths.DefaultLineSizeChanged -= OnDefaultColumnWidthsLineSizeChanged;
                    this.columnWidths.Dispose();
                    this.columnWidths = null;
                }
                if (this.coveredCells !=null)
                {
                    this.coveredCells.Clear();
                    this.coveredCells = null;
                }
                if (this.CoveredRanges !=null)
                {
                    this.CoveredRanges.Clear();
                }
                if (Data != null)
                {
                    this.data.Dispose();
                    this.data = null;
                }
                if (this.footerStyle != null)
                {
                    this.footerStyle.ClearCache();
                    this.footerStyle.Dispose(true);
                    this.footerStyle = null;
                }
                if (this.gridContextMenu != null)
                {
                    this.gridContextMenu = null;
                }
                if (this.gridCutPaste != null)
                {
                    this.gridCutPaste = null;
                }
                if (this.gridLinePen != null)
                {
                    this.gridLinePen = null;
                }
                if (this.headerStyle != null)
                {
                    this.headerStyle.ClearCache();
                    this.headerStyle.Dispose(true);
                    this.headerStyle = null;
                }
                if (this.HiddenColRanges != null)
                {
                    this.HiddenColRanges = null;
                }
                if (this.HiddenRowRanges != null)
                {
                    this.HiddenRowRanges = null;
                }
                if (this.options != null)
                this.options.Dispose();
                this.options = null;
                if (this.rowHeights != null)
                {
                    this.rowHeights.DefaultLineSizeChanged -= OnDefaultRowHeightsLineSizeChanged;
                    this.rowHeights.Dispose();
                    this.rowHeights = null;
                }
                if (this.rowStyles != null)
                {
                    this.rowStyles = null;
                }
                if (this.SelectedCells != null)
                {
                    this.SelectedCells.Dispose();
                }
                if (this.selectedRanges != null)
                {
                    this.selectedRanges.Clear();
                    this.selectedRanges = null;
                }
                if (this.selections != null)
                {
                    this.selections.Clear(false);
                    this.selections = null;
                }
                if (this.sizer != null)
                {
                    this.sizer.Dispose();
                    this.sizer = null;
                }
                if (this.tableStyle != null)
                {
                    this.tableStyle.ClearCache();
                    this.tableStyle.Dispose(true);
                    this.tableStyle = null;
                }
                if (this.textDataExchange != null)
                    this.textDataExchange = null;
                if (this.userData !=null)
                {
                    this.userData.Clear();
                    this.userData = null;
                }
                if (this.Views != null)
                {
                    if (canDisposeGrid)
                    {
                        foreach (GridControlBase view in this.Views)
                        {
                            view.Dispose(false);
                        }
                    }
                    this.views.Clear();
                    this.views = null;
                }
                if (this.overlappingCells != null)
                {
                    overlappingCells.Clear();
                    overlappingCells = null;
                }
                if (cellSpanBackgrounds != null)
                {
                    cellSpanBackgrounds.Clear();
                    cellSpanBackgrounds = null;
                }
                if (this.volatileCellStyles != null)
                {
                    this.volatileCellStyles.Dispose();
                    this.volatileCellStyles.Clear();
                    this.volatileCellStyles = null;
                }
                if (this.formulaEngine != null)
                {
                    this.formulaEngine.Dispose();
                    this.formulaEngine = null;
                }
                if (this.GraphicModel != null)
                {
                    this.GraphicModel.Dispose();
                    this.graphicModel = null;
                }
                if (!this.CurrentCellState.IsEmpty)
                {
                    this.currentCellState.Dispose();
                }
                isDisposed = true;
            }
        }

        #endregion

        #region ShortCut Properties
        /// <summary>
        /// Gets or sets the number of rows in the grid.
        /// </summary>
        public int RowCount
        {
            get
            {
                return RowHeights.LineCount;
            }

            set
            {
                if (value > RowCount)
                    InsertRows(RowCount, value - RowCount);
                else if (value < RowCount)
                    RemoveRows(value, RowCount - value);
                //RowHeights.LineCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of columns in the grid.
        /// </summary>
        public int ColumnCount
        {
            get
            {
                return ColumnWidths.LineCount;
            }

            set
            {
                if (value > ColumnCount)
                    InsertColumns(ColumnCount, value - ColumnCount);
                else if (value < ColumnCount)
                    RemoveColumns(value, ColumnCount - value);
            }
        }
        #endregion

        #region Options
        GridModelOptions options = new GridModelOptions();

        /// <summary>
        /// A <see cref="GridModelOptions"/> that allows you to adjust behavior and appearance of the grid.
        /// </summary>
        [Browsable(false)]
        public GridModelOptions Options
        {
            get
            {
                return options;
            }
            set
            {
                options = value;
            }
        }
        #endregion

        #region Cell Height, Content, Spans, VolatileCellStyles
        /// <summary>
        /// Gets row heights for the grid.
        /// </summary>
        public IPaddedEditableLineSizeHost RowHeights
        {
            get { return rowHeights; }
        }

        protected virtual IPaddedEditableLineSizeHost OnCreateRowHeights()
        {
            LineSizeCollection lineSizeCollection = new LineSizeCollection();
            lineSizeCollection.DefaultLineSizeChanged += OnDefaultRowHeightsLineSizeChanged;
            lineSizeCollection.LineSizeChanged += OnRowLineSizeChanged;
            return lineSizeCollection;
        }

        void OnRowLineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetRowSizeCommand(this, e.From, e.To, e.OldSize));
            }
        }

        void OnDefaultRowHeightsLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetDefaultRowSizeCommand(this, e.OldValue));
            }
        }

        /// <summary>
        /// Gets column widths for the grid.
        /// </summary>
        [XmlIgnore]
        public IPaddedEditableLineSizeHost ColumnWidths
        {
            get { return columnWidths; }
            set { columnWidths = value; }
        }

        protected virtual IPaddedEditableLineSizeHost OnCreateColumnWidths()
        {
            LineSizeCollection lineSizeCollection = new LineSizeCollection();
            lineSizeCollection.DefaultLineSizeChanged += OnDefaultColumnWidthsLineSizeChanged;
            lineSizeCollection.LineSizeChanged += OnColumnLineSizeChanged;
            return lineSizeCollection;
        }

        void OnColumnLineSizeChanged(object sender, RangeChangedEventArgs e)
        {
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetColumnSizeCommand(this, e.From, e.To, e.OldSize));
            }
        }

        void OnDefaultColumnWidthsLineSizeChanged(object sender, DefaultLineSizeChangedEventArgs e)
        {
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelSetDefaultColumnSizeCommand(this, e.OldValue));
            }
        }

        /// <summary>
        /// Gets covered cells in the grid.
        /// </summary>
        [XmlIgnore]
        public GridCoveredCellInfoCollection CoveredCells
        {
            get { return coveredCells; }
        }

        GraphicModel graphicModel;
		[XmlIgnore]
        public GraphicModel GraphicModel
        {
            get
            {
                if (graphicModel == null)
                {
                    graphicModel = new GraphicModel();
                }
                return graphicModel;
            }
            set
            {
                graphicModel = value;
            }
        }

        [XmlIgnore]
        public GridOverlappingCellInfoCollection OverlappingCells
        {
            get { return overlappingCells; }
        }

        /// <summary>
        /// Gets cell spans in the grid.
        /// </summary>
        [XmlIgnore]
        public GridCellSpanBackgroundInfoCollection CellSpanBackgrounds
        {
            get { return cellSpanBackgrounds; }
        }

        /// <summary>
        /// Gets or sets a <see cref="GridCellData"/> object that saves cell contents and row and column headers for the <see cref="GridModel"/>.
        /// </summary>
        [XmlArray("Data")]
        [XmlArrayItem("GridStyleInfoStoreData", typeof(SerializableKeyValuePair<RowColumnIndex, GridStyleInfoStore>))]
        public GridCellData Data
        {
            get { return data; }
        }

        #endregion

        #region Header and Footer
        /// <summary>
        /// Gets or sets the number of frozen rows.
        /// </summary>
        public int FrozenRows
        {
            get
            {
                return RowHeights.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = RowHeights.HeaderLineCount;
                RowHeights.HeaderLineCount = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFrozenRowsCountCommand(this, savedValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of frozen columns.
        /// </summary>
        public int FrozenColumns
        {
            get
            {
                return ColumnWidths.HeaderLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = ColumnWidths.HeaderLineCount;
                ColumnWidths.HeaderLineCount = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFrozenColumnsCountCommand(this, savedValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of footer rows.
        /// </summary>
        public int FooterRows
        {
            get
            {
                return RowHeights.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = RowHeights.FooterLineCount;
                RowHeights.FooterLineCount = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFooterRowsCountCommand(this, savedValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of footer columns.
        /// </summary>
        public int FooterColumns
        {
            get
            {
                return ColumnWidths.FooterLineCount;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = ColumnWidths.FooterLineCount;
                ColumnWidths.FooterLineCount = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetFooterColumnsCountCommand(this, savedValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of header rows.
        /// </summary>
        public int HeaderRows
        {
            get { return headerRows; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = headerRows;
                headerRows = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetHeaderRowsCountCommand(this, savedValue));
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of header columns.
        /// </summary>
        public int HeaderColumns
        {
            get { return headerColumns; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Negative values not allowed.");
                int savedValue = headerColumns;
                headerColumns = value;
                if (CommandStack.ShouldGenerateUndoInfo)
                {
                    CommandStack.Push(new GridModelSetHeaderColumnsCountCommand(this, savedValue));
                }
            }
        }

        #endregion

        #region Base Styles
        /// <summary>
        /// Gets the table style. Individual cells will inherit attributes from the table style.
        /// </summary>
        public GridStyleInfo TableStyle
        {
            get { return tableStyle; }
            set { tableStyle = value; }
        }

        /// <summary>
        /// Gives you access to the row style information of a row.
        /// </summary>
        /// <value>The row styles.</value>
        /// <overload>
        /// Gives you access to the row style information of a row.
        /// </overload>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change row style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        /// model.RowStyles[2].Font.Bold = true;
        /// model.RowStyles[2].Font.Size = 16;
        /// model.RowStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        /// model.RowStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        /// model.RowStyles[2].CellType = "Static";
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set for the cell,
        /// the <see cref="GridStyleInfo"/> object that is returned by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        /// model.RowStyles[1].Background = Brushes.Red;
        /// Brush color = model[1, 1].Background;
        /// // model[1, 1].TextColor will return Brushes.Red
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfoIndexer RowStyles
        {
            get
            {
                return this.rowStyles;
            }
        }

        /// <overload>
        /// Gives you access to the column style information of a column.
        /// </overload>
        /// <summary>
        /// Gives you access to the column style information of a column.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change column style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        ///             model.ColStyles[2].Font.Bold = true;
        ///             model.ColStyles[2].Font.Size = 16;
        ///             model.ColStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        ///             model.ColStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        ///             model.ColStyles[2].CellType = "Static";
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set,
        /// the <see cref="GridStyleInfo"/> object that is return by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        ///             model.ColStyles[1].Background = Brushes.Red;
        ///                 Brush color = model[1, 1].Background;
        ///                 // model[1, 1].TextColor will return Brushes.Red
        /// </code>
        /// </example>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridStyleInfoIndexer ColStyles
        {
            get
            {
                return this.colStyles;
            }
        }

        /// <summary>
        /// Gets the header style.
        /// </summary>
        public GridStyleInfo HeaderStyle
        {
            get { return headerStyle; }
        }

        /// <summary>
        /// Gets the footer style.
        /// </summary>
        public GridStyleInfo FooterStyle
        {
            get { return footerStyle; }
        }
        #endregion

        #region Cell Styles and Events

        protected virtual void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryBaseStyles(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryBaseStyles(e);
            }

            if (QueryBaseStyles != null)
                QueryBaseStyles(this, e);
        }

        protected virtual void OnCommitCellInfo(GridCommitCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnCommitCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnCommitCellInfo(e);
            }

            if (CommitCellInfo != null)
            {
                CommitCellInfo(this, e);
            }
        }

        protected virtual void OnCommittedCellInfo(GridCommitCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnCommittedCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnCommittedCellInfo(e);
            }

            if (CommittedCellInfo != null)
                CommittedCellInfo(this, e);
        }

#if !SILVERLIGHT
        public event GridQueryContextMenuInfoEventHandler QueryContextMenuInfo;

        public void RaiseQueryContextMenuInfo(GridQueryContextMenuInfoEventArgs e)
        {
            OnQueryContextMenuInfo(e);
        }

        private ContextMenu gridContextMenu;
        protected virtual ContextMenu OnContextMenuCreating()
        {            
            return new ContextMenu();
        }

        internal ContextMenu GridContextMenu
        {
            get
            {
                if (this.gridContextMenu == null)
                {
                    this.gridContextMenu = this.OnContextMenuCreating();
                }

                return this.gridContextMenu;
            }
        }

        protected virtual void OnQueryContextMenuInfo(GridQueryContextMenuInfoEventArgs e)
        {
            GridContextMenu.IsOpen = false;

            if (GridContextMenu.ItemsSource == null)
                GridContextMenu.Items.Clear();
            else
                GridContextMenu.ItemsSource = null;

            if (e.Style.ContextMenuTemplate != null)
                if (e.Style.ContextMenuTemplate.DataTemplateKey != null)
                    GridContextMenu.ItemTemplate = e.Style.ContextMenuTemplate;

            if (QueryContextMenuInfo != null)
                QueryContextMenuInfo(this, e);

            if (this.EnableContextMenu && !e.Cancel)
            {
                if (e.Style.ContextMenuItemSource != null)
                    GridContextMenu.ItemsSource = e.Style.ContextMenuItemSource;
                else
                    GridContextMenu.ItemsSource = e.Style.ContextMenuItems;

                if (GridContextMenu.ItemsSource != null || GridContextMenu.Items.Count > 0)
                    GridContextMenu.IsOpen = true;
            }
        }
#endif

        protected virtual void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellInfo(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellInfo(e);
            }

            if (QueryCellInfo != null)
                QueryCellInfo(this, e);


            if (e.Style.HasConditionalFormat)
            {
                ApplyConditionalFormat(e.Style);
                e.Handled = true;
            }
        }

        bool overrideSytle = false;
        private void ApplyConditionalFormat(GridStyleInfo style)
        {
            if (style.HasConditionalFormat)
            {
                string value;
                if (style.ApplyConditionalFormatBasedOn == ApplyConditionalBasedOn.FormulaValue)
                {
                    var condition = style.ConditionalFormat;
                    value = style.GetFormulaValue(condition.FormulaText, condition.FormulaTag);
                }
                else
                {
                    if (style.CellType == "FormulaCell")
                    {
                        if (style.FormulaTag != null)
                            value = this.FormulaEngine.ComputedValue(style.FormulaTag.Formula);
                        else
                            value = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
                        
                        if (style.HasFormat)
                        {
                            var zeroFormatedText = 0.ToString(style.Format);
                            if (value == zeroFormatedText || value == string.Empty)
                                value = "0";
                            else
                                value = value.Trim(zeroFormatedText.ToArray());
                        }
                    }
                    else
                    {
                        value = style.CellValue != null ? style.CellValue.ToString() : string.Empty;
                    }
                }
                object record;
                int r1;
                double r2;
                bool r3;
                DateTime r4;
                if (int.TryParse(value, out r1))
                {
                    record = r1;
                }
                else if (double.TryParse(value, out r2))
                {
                    record = r2;
                }
                else if (bool.TryParse(value, out r3))
                {
                    record = r3;
                }
                else if (DateTime.TryParse(value, out r4))
                {
                    r2 = (r4.ToOADate());
                    if (r2 < 61)
                    {
                        r2 = (r4.ToOADate() - 1);
                    }
                    record = r2;
                }
                else
                {
                    record = value.ToString();
                }
                var formating = style.ConditionalFormat;
                try
                {
                    if (record.GetType() != typeof(string))
                    {
                        foreach (var condition in formating.Conditions)
                        {
                            if (condition.Value != null)
                            {
#if !SyncfusionFramework3_5
                                if (string.IsNullOrWhiteSpace(condition.Value.ToString()) ||
                                    string.IsNullOrEmpty(condition.Value.ToString()))
#else
                                if (string.IsNullOrEmpty(condition.Value.ToString()))
#endif
                                {
                                    record = record.ToString();
                                    break;
                                }
                            }
                        }
                    }
                    else if ((string)record == string.Empty)
                    {
                        foreach (var condition in formating.Conditions)
                        {
                            if (condition.ConditionType == GridConditionType.Equals ||
                                 condition.ConditionType == GridConditionType.NotEquals) continue;
                            record = 0;
                            break;
                        }
                    }
                    var delg = formating.GetCompiledDelegate(style, record.GetType());
                    if (delg != null)
                        overrideSytle = (bool)delg.DynamicInvoke(new object[] { record });
                    else
                        overrideSytle = false;
                }
                catch (Exception ex)
                {
                    overrideSytle = false;
                    throw new InvalidOperationException(ex.Message);
                }

                if (overrideSytle)
                {
                    //If we import excel sheet with conditional formatting then the formatting style was initially stored in the grid model
                    //if we are change the particular cell value then the default style was not applied to that cell
                    //So that the default style was copied and applied to the again when the particular condition failed
                    if (style.ConditionalFormat.defaultStyleStore.IsEmpty)
                    {
                        CopyStyleFromDefaultStyle(style.ConditionalFormat.defaultStyleStore, style.Store, style.ConditionalFormat.Style.Store);
                    }
                    style.ModifyStyle(style.ConditionalFormat.Style, StyleModifyType.Override);
                }
            }
            else if(!style.ConditionalFormat.defaultStyleStore.IsEmpty)
            {
                style.ModifyStyle(style.ConditionalFormat.defaultStyleStore as GridStyleInfoStore, StyleModifyType.Override);
            }
        }

        private void CopyStyleFromDefaultStyle(GridStyleInfoStore targetStyleInfoStore, GridStyleInfoStore sourceStyleInfoStore, GridStyleInfoStore basedOnStyle)
        {
            if (basedOnStyle.IsEmpty)
                return;
            foreach (StyleInfoProperty sip in basedOnStyle.StyleInfoProperties)
            {
                if (basedOnStyle.HasValue(sip))
                {
                    var value = sourceStyleInfoStore.GetValue(sip);
                    targetStyleInfoStore.SetValue(sip, value);
                }
            }
        }

        /// <summary>
        /// Occurs when the model queries for style information about a specific cell.
        /// This event allows you to customize cell contents at run-time on demand, just before
        /// the cell is drawn
        /// </summary>
        public event GridQueryCellInfoEventHandler QueryCellInfo;
        /// <summary>
        /// Occurs when the model is about to save style information about a specific cell.
        /// </summary>
        public event GridCommitCellInfoEventHandler CommitCellInfo;
        /// <summary>
        /// Occurs after the model has saved the style information about a specific cell.
        /// </summary>
        public event GridCommitCellInfoEventHandler CommittedCellInfo;
        /// <summary>
        /// Occurs when the model queries information about a base style.
        /// </summary>
        public event GridQueryBaseStylesEventHandler QueryBaseStyles;
        /// <summary>
        /// Gives you access to the style information of a cell.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change cell contents.
        /// </remarks>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="columnIndex">Column index.</param>
        /// <returns>Cell style.</returns>
        public GridStyleInfo this[int rowIndex, int columnIndex]
        {
            get
            {
                return volatileCellStyles[rowIndex, columnIndex];
            }
            set
            {
                volatileCellStyles[rowIndex, columnIndex].CopyFrom(value);
            }
        }


        /// <summary>
        /// Read only collection of hidden columns.
        /// </summary>
        [XmlIgnore]
        public ReadOnlyObservableCollection<GridRangeInfo> HiddenColRanges { get; protected internal set; }
        /// <summary>
        /// Read only collection of hidden rows.
        /// </summary>
        [XmlIgnore]
        public ReadOnlyObservableCollection<GridRangeInfo> HiddenRowRanges { get; protected internal set; }

        #endregion

        #region VolatileCellStylesHost Members

        //void IGridVolatileCellStylesHost.SaveCellInfo(RowColumnIndex cell, GridStyleInfo style)
        //{
        //    SaveCellInfoEventArgs e = new SaveCellInfoEventArgs(cell, style);
        //    OnSaveCellInfo(e);
        //    if (!e.Handled)
        //    {
        //        GridStyleInfoStore store = data[cell.RowIndex, cell.ColumnIndex];
        //        if (store != null)
        //            store.ModifyStyle(style.Store, StyleModifyType.Copy);
        //        else
        //        {
        //            store = new GridStyleInfoStore();
        //            data[cell.RowIndex, cell.ColumnIndex] = store;
        //            style.Store.CopyTo(store);
        //        }
        //    }
        //}

        void IGridVolatileCellStylesHost.QueryCellInfo(RowColumnIndex cell, GridStyleInfo style)
        {
            GridStyleInfoStore store = data[cell.RowIndex, cell.ColumnIndex];
            if (store != null)
                style.ModifyStyle(store, StyleModifyType.Override);

            GridQueryCellInfoEventArgs e = new GridQueryCellInfoEventArgs(cell, style);
            OnQueryCellInfo(e);
            if (!e.Handled)
            {
            }
        }

        IStyleInfo[] IGridVolatileCellStylesHost.QueryBaseStyles(RowColumnIndex cell, GridStyleInfo style)
        {
            GridQueryBaseStylesEventArgs e = new GridQueryBaseStylesEventArgs(cell, style);
            OnQueryBaseStyles(e);
            if (!e.Handled)
            {
                if (cell.ColumnIndex < HeaderColumns
                    || cell.RowIndex < HeaderRows)
                    e.BaseStyles.Add(headerStyle);

                GridStyleInfo rowStyle;
                this.rowStyles.TryGetValue(e.Cell.RowIndex, out rowStyle);
                if (rowStyle != null)
                {
                    e.BaseStyles.Add(rowStyle);
                }

                GridStyleInfo colStyle;
                this.colStyles.TryGetValue(e.Cell.ColumnIndex, out colStyle);
                if (colStyle != null)
                {
                    e.BaseStyles.Add(colStyle);
                }

                if (cell.ColumnIndex >= ColumnWidths.LineCount - FooterColumns
                    || cell.RowIndex >= RowHeights.LineCount - FooterRows)
                    e.BaseStyles.Add(footerStyle);

                e.BaseStyles.Add(tableStyle);
            }


            // Base style.
            string baseStyleName = style.HasBaseStyle ? style.BaseStyle : "";

            if (baseStyleName.Length == 0)
            {
                foreach (GridStyleInfo si in e.BaseStyles)
                {
                    if (si.HasBaseStyle)
                    {
                        baseStyleName = si.BaseStyle;
                        if (baseStyleName.Length > 0)
                            break;
                    }
                }
            }

            // Row or column header.
            //if (baseStyleName.Length == 0)
            //{
            //    if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0)
            //    {
            //        if (cell.RowIndex <= RowHeights.HeaderLineCount && cell.ColumnIndex <= ColumnWidths.HeaderLineCount)
            //            baseStyleName = "Header";
            //        else if (cell.RowIndex <= RowHeights.HeaderLineCount)
            //            baseStyleName = "Column Header";
            //        else if (cell.ColumnIndex <= ColumnWidths.HeaderLineCount)
            //            baseStyleName = "Row Header";
            //    }
            //}

            // Load all parent base styles (including standard style).
            int level;
            GridStyleInfo[] infoMapStyles = BaseStylesMap.GetBaseStylesMapStyles(baseStyleName, out level);

            int cellStyleCount = e.BaseStyles.Count;
            // Combine the two arrays.
            GridStyleInfo[] baseStyles = new GridStyleInfo[cellStyleCount + level];
            for (int n = 0; n < cellStyleCount; n++)
                baseStyles[n + level] = e.BaseStyles[n];
            if (infoMapStyles != null)
                Array.Copy(infoMapStyles, 0, baseStyles, 0, level);

            // Each GridStyleInfoIdentity will cache the baseStyles.
            return baseStyles;
        }

        void IGridVolatileCellStylesHost.CommitCellInfo(RowColumnIndex cell, GridStyleInfo style, StyleInfoProperty sip)
        {
            GridCommitCellInfoEventArgs e = new GridCommitCellInfoEventArgs(cell, style, sip);
            OnCommitCellInfo(e);
            if (!e.Handled)
            {
                GridStyleInfoStore store = data[cell.RowIndex, cell.ColumnIndex];
                if (store == null)
                {
                    store = new GridStyleInfoStore();
                    if (cell.RowIndex > -1 && cell.ColumnIndex > -1)
                        data[cell.RowIndex, cell.ColumnIndex] = store;
                }
                if (sip != null)
                    store.SetValue(sip, style.Store.GetValue(sip));
                else
                    store.ModifyStyle(style.Store, StyleModifyType.Changes);

                if (sip != null && sip.PropertyName != "FormulaTag")
                {
                    GridRangeInfo range = GridRangeInfo.Auto(cell.RowIndex, cell.ColumnIndex);
                    ChangeCells(range, new GridStyleInfo[] { style }, StyleModifyType.Changes);
                }
            }
            OnCommittedCellInfo(e);
           
        }
        #endregion

        #region Insert and Remove Rows

        /// <summary>
        /// Inserts specified number of rows at the given row index.
        /// </summary>
        /// <param name="insertAtRowIndex">The row index to insert.</param>
        /// <param name="count">Number of rows to be inserted</param>
        public virtual void InsertRows(int insertAtRowIndex, int count)
        {
            InsertRowsCore(insertAtRowIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtRowIndex, count);
            OnRowsInserted(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelRemoveRowsCommand(this, insertAtRowIndex, count));
            }
        }

        /// <summary>
        /// Occurs after a range of rows has been inserted.
        /// See <see cref="GridRangeInsertedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeInsertedEventHandler RowsInserted;

        protected virtual void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsInserted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsInserted(e);
            }

            if (RowsInserted != null)
                RowsInserted(this, e);
        }

        /// <summary>
        /// Move row style and reset the default row style for inserted rows.
        /// </summary>
        /// <param name="insertAtRowIndex"></param>
        /// <param name="count"></param>
        private void InsertRowStyle(int insertAtRowIndex, int count)
        {
            GridStyleInfoIndexer rStyles = rowStyles;
            this.rowStyles = new GridStyleInfoIndexer();

            for (int rowIndex = 0; rowIndex < this.RowCount; rowIndex++)
            {
                GridStyleInfo rStyle;
                rStyles.TryGetValue(rowIndex, out rStyle);
                if (rStyle != null)
                {
                    if (rowIndex >= insertAtRowIndex)
                    {
                        int moveTo = rowIndex + count;
                        this.rowStyles[moveTo] = rStyle;
                    }
                    else
                        rowStyles[rowIndex] = rStyle;
                }
            }
        }

        protected virtual void InsertRowsCore(int insertAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.InsertRows(insertAtRowIndex, count, moveCellsState.VolatileCellStyles);
            if (RowHeights.SupportsInsertRemove)
                RowHeights.InsertLines(insertAtRowIndex, count, moveCellsState.LineSizes);
            Data.InsertRows(insertAtRowIndex, count, moveCellsState.Data);
            CoveredCells.InsertRows(insertAtRowIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.InsertRows(insertAtRowIndex, count, moveCellsState.CellSpanBackgrounds);
            InsertRowStyle(insertAtRowIndex, count);
            GridControlBase grid = null;
            foreach (GridControlBase gridView in views)
            {
                grid = gridView;
                if (moveCellsState.IsEmpty)
                    gridView.ModelInsertRows(insertAtRowIndex, count, null);
                else
                    gridView.ModelInsertRows(insertAtRowIndex, count, moveCellsState.GridViews[gridView]);
            }

            var suspendSelection = moveCellsState != null ? moveCellsState.SuspendSelections : false;
            if (!suspendSelection)
            {
                GridDataControl dataGrid = null;
                if (grid != null)
                {
                    dataGrid = grid.FindParentElementOfType<GridDataControl>();               
                }
                if (dataGrid != null)
                {
                    if(dataGrid.Model.TableProperties.AutoFocusCurrentItem)
                        Selections.InsertRows(insertAtRowIndex, count, moveCellsState.Selections);
                }
                else
                    Selections.InsertRows(insertAtRowIndex, count, moveCellsState.Selections);
            }
        }

        /// <summary>
        /// Deletes given number of rows from the specified index.
        /// </summary>
        /// <param name="removeAtRowIndex">The row index.</param>
        /// <param name="count">Number of rows to be removed.</param>
        public virtual void RemoveRows(int removeAtRowIndex, int count)
        {
            RemoveRowsCore(removeAtRowIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtRowIndex, count);
            OnRowsRemoved(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelInsertRowsCommand(this, removeAtRowIndex, count));
            }
        }

        /// <summary>
        /// Occurs when a range of rows is removed.
        /// See <see cref="GridRangeRemovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeRemovedEventHandler RowsRemoved;

        protected virtual void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsRemoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsRemoved(e);
            }

            if (RowsRemoved != null)
                RowsRemoved(this, e);
        }

        protected virtual void RemoveRowsCore(int removeAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.RemoveRows(removeAtRowIndex, count, moveCellsState.VolatileCellStyles);
            if (RowHeights.SupportsInsertRemove)
                RowHeights.RemoveLines(removeAtRowIndex, count, moveCellsState.LineSizes);
            Data.RemoveRows(removeAtRowIndex, count, moveCellsState.Data);
            CoveredCells.RemoveRows(removeAtRowIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.RemoveRows(removeAtRowIndex, count, moveCellsState.CellSpanBackgrounds);
            GridControlBase grid=null;
            int currentrowindex = -1;
            foreach (GridControlBase gridView in views)
            {
                grid = gridView;
                currentrowindex = grid.CurrentCell.RowIndex;
                if (moveCellsState.IsEmpty)
                    gridView.ModelRemoveRows(removeAtRowIndex, count, null);
                else
                {
                    GridViewMoveCellsState viewState = gridView.CreateGridViewMoveCellsState();
                    gridView.ModelRemoveRows(removeAtRowIndex, count, viewState);
                    moveCellsState.GridViews[gridView] = viewState;
                }
            }

            var suspendSelection = moveCellsState != null ? moveCellsState.SuspendSelections : false;
            if (!suspendSelection)
            {
                GridDataControl dataGrid = null;
                CollectionViewAdv view = null;
                if (grid != null)
                {
                    dataGrid = grid.FindParentElementOfType<GridDataControl>();
                    if(dataGrid!=null)
                        view = dataGrid.Model.View as CollectionViewAdv;
                }
                if (((dataGrid != null && dataGrid.Model.IsInDeteteRecord) || (grid is GridDataCellNestedGridEditor && (grid as GridDataCellNestedGridEditor).TableModel.IsInDeteteRecord)) && 
                    view != null && !view.IsInPropertyChange  && this.SelectedRanges.Count > 0)
                {
                    if (removeAtRowIndex == this.RowCount && removeAtRowIndex==currentrowindex)
                    {
                        removeAtRowIndex = removeAtRowIndex - 1;//If deleted row is the last row then selection should remain in the previous row.                        
                        grid.CurrentCell.MoveTo(removeAtRowIndex, grid.CurrentCell.ColumnIndex);
                    }
                                        GridRangeInfo selectedRange = null;
                                        if (this.Options.ListBoxSelectionMode == GridSelectionMode.None &&
                                               this.Options.AllowSelection == GridSelectionFlags.Cell |
                                               this.Options.AllowSelection == GridSelectionFlags.Any)
                                        {
                                         
                                            selectedRange = GridRangeInfo.Cell(removeAtRowIndex, grid.CurrentCell.ColumnIndex);
                                          
                                        }
                                        else
                                        {
                                            selectedRange = GridRangeInfo.Row(removeAtRowIndex);
                                        }


                    // Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);//Previous Code
                    //Previously while deleting the Row, Deleted row selection remove here. But newly selected item doesn't set here
                    // Now It will achieve through the RaiseSelectionChanged event
                  //  GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(GridRangeInfo.Row(removeAtRowIndex), SelectedRanges, GridSelectionReason.DeleteRow);
                GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(selectedRange, SelectedRanges, GridSelectionReason.DeleteRow); ;
                    this.RaiseSelectionChanged(e);
                }
                else
                {
                    if (dataGrid != null)
                    {
                        if(dataGrid.Model.TableProperties.AutoFocusCurrentItem)
                            Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);
                    }
                    else
                        Selections.RemoveRows(removeAtRowIndex, count, moveCellsState.Selections);
                }
            }
        }
        #endregion

        #region Insert and Remove Columns

        /// <summary>
        /// Inserts the given number of columns at the specified column index.
        /// </summary>
        /// <param name="insertAtColumnIndex">The column index to insert.</param>
        /// <param name="count">Number of columns to be inserted.</param>
        public virtual void InsertColumns(int insertAtColumnIndex, int count)
        {
            InsertColumnsCore(insertAtColumnIndex, count, null);

            GridRangeInsertedEventArgs e = new GridRangeInsertedEventArgs(insertAtColumnIndex, count);
            OnColumnsInserted(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelRemoveColumnsCommand(this, insertAtColumnIndex, count));
            }
        }

        /// <summary>
        /// Occurs after a range of columns has been inserted.
        /// See <see cref="GridRangeInsertedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeInsertedEventHandler ColumnsInserted;

        protected virtual void OnColumnsInserted(GridRangeInsertedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsInserted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsInserted(e);
            }

            if (ColumnsInserted != null)
                ColumnsInserted(this, e);
        }

        protected virtual void InsertColumnsCore(int insertAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.InsertColumns(insertAtColumnIndex, count, moveCellsState.VolatileCellStyles);
            if (ColumnWidths.SupportsInsertRemove)
                ColumnWidths.InsertLines(insertAtColumnIndex, count, moveCellsState.LineSizes);
            Data.InsertColumns(insertAtColumnIndex, count, moveCellsState.Data);
            CoveredCells.InsertColumns(insertAtColumnIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.InsertColumns(insertAtColumnIndex, count, moveCellsState.CellSpanBackgrounds);

            foreach (GridControlBase gridView in views)
            {
                if (moveCellsState.IsEmpty)
                    gridView.ModelInsertColumns(insertAtColumnIndex, count, null);
                else
                    gridView.ModelInsertColumns(insertAtColumnIndex, count, moveCellsState.GridViews[gridView]);
            }

            Selections.InsertColumns(insertAtColumnIndex, count, moveCellsState.Selections);
        }

        /// <summary>
        /// Removes given number of columns from the specified index.
        /// </summary>
        /// <param name="removeAtColumnIndex">The column index.</param>
        /// <param name="count">Number of columns to be removed.</param>
        public virtual void RemoveColumns(int removeAtColumnIndex, int count)
        {
            RemoveColumnsCore(removeAtColumnIndex, count, null);

            GridRangeRemovedEventArgs e = new GridRangeRemovedEventArgs(removeAtColumnIndex, count);
            OnColumnsRemoved(e);
            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelInsertColumnsCommand(this, removeAtColumnIndex, count));
            }
        }

        /// <summary>
        /// Occurs after a range of columns has been removed.
        /// See <see cref="GridRangeRemovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeRemovedEventHandler ColumnsRemoved;

        protected virtual void OnColumnsRemoved(GridRangeRemovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsRemoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsRemoved(e);
            }

            if (ColumnsRemoved != null)
                ColumnsRemoved(this, e);
        }

        protected virtual void RemoveColumnsCore(int removeAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (moveCellsState == null) moveCellsState = GridMoveCellsState.Empty;

            VolatileCellStyles.RemoveColumns(removeAtColumnIndex, count, moveCellsState.VolatileCellStyles);
            if (ColumnWidths.SupportsInsertRemove)
                ColumnWidths.RemoveLines(removeAtColumnIndex, count, moveCellsState.LineSizes);
            Data.RemoveColumns(removeAtColumnIndex, count, moveCellsState.Data);
            CoveredCells.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CoveredCells);
            CellSpanBackgrounds.RemoveColumns(removeAtColumnIndex, count, moveCellsState.CellSpanBackgrounds);

            foreach (GridControlBase gridView in views)
            {
                if (moveCellsState.IsEmpty)
                    gridView.ModelRemoveColumns(removeAtColumnIndex, count, null);
                else
                {
                    GridViewMoveCellsState viewState = gridView.CreateGridViewMoveCellsState();
                    gridView.ModelRemoveColumns(removeAtColumnIndex, count, viewState);
                    moveCellsState.GridViews[gridView] = viewState;
                }
            }

            Selections.RemoveColumns(removeAtColumnIndex, count, moveCellsState.Selections);
        }

        #endregion

        #region Move Rows or Columns
        /// <summary>
        /// Moves given number of rows from one position to another.
        /// </summary>
        /// <param name="removeAtRowIndex">Source row index.</param>
        /// <param name="count">Number of rows to be moved.</param>
        /// <param name="insertAtRowIndex">Destination row index.</param>
        public void MoveRows(int removeAtRowIndex, int count, int insertAtRowIndex)
        {
            GridMoveCellsState moveCellsState = CreateGridMoveCellsState(RowHeights.CreateMoveLines());
            RemoveRowsCore(removeAtRowIndex, count, moveCellsState);
            InsertRowsCore(insertAtRowIndex, count, moveCellsState);

            GridRangeMovedEventArgs e = new GridRangeMovedEventArgs(removeAtRowIndex, count, insertAtRowIndex);
            OnRowsMoved(e);

            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelMoveRowsCommand(this, removeAtRowIndex, count, insertAtRowIndex));
            }
        }

        /// <summary>
        /// Occurs after a range of rows has been moved.
        /// See <see cref="GridRangeMovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeMovedEventHandler RowsMoved;

        protected virtual void OnRowsMoved(GridRangeMovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnRowsMoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnRowsMoved(e);
            }

            if (RowsMoved != null)
                RowsMoved(this, e);
        }

        /// <summary>
        /// Moves the given number of columns from one position to another.
        /// </summary>
        /// <param name="removeAtColumnIndex">Source column index.</param>
        /// <param name="count">Number of columns to be moved.</param>
        /// <param name="insertAtColumnIndex">Destination column index.</param>
        public void MoveColumns(int removeAtColumnIndex, int count, int insertAtColumnIndex)
        {
            GridMoveCellsState moveCellsState = CreateGridMoveCellsState(ColumnWidths.CreateMoveLines());
            RemoveColumnsCore(removeAtColumnIndex, count, moveCellsState);

            GridTreeModel model = this as GridTreeModel;
            if (model != null)
            {
                if (insertAtColumnIndex < removeAtColumnIndex)
                    InsertColumnsCore(insertAtColumnIndex, count, moveCellsState);
                else
                    InsertColumnsCore(insertAtColumnIndex - 1, count, moveCellsState);
            }
            else
                InsertColumnsCore(insertAtColumnIndex, count, moveCellsState);

            GridRangeMovedEventArgs e = new GridRangeMovedEventArgs(removeAtColumnIndex, count, insertAtColumnIndex);
            OnColumnsMoved(e);

            if (CommandStack.ShouldGenerateUndoInfo)
            {
                CommandStack.Push(new GridModelMoveColumnsCommand(this, removeAtColumnIndex, count, insertAtColumnIndex));
            }
        }

        /// <summary>
        /// Occurs after a range of columns has been moved.
        /// See <see cref="GridRangeMovedEventArgs"/> for more details.
        /// </summary>
        public event GridRangeMovedEventHandler ColumnsMoved;

        protected virtual void OnColumnsMoved(GridRangeMovedEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnColumnsMoved(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnColumnsMoved(e);
            }

            if (ColumnsMoved != null)
                ColumnsMoved(this, e);
        }

        protected virtual GridMoveCellsState CreateGridMoveCellsState(IEditableLineSizeHost lineSizes)
        {
            return new GridMoveCellsState(lineSizes, CreateVolatileCellStyles());
        }
        #endregion

        #region BaseStylesMap
        /// <summary>
        /// The <see cref="GridBaseStylesMap"/> that is associated with this <see cref="GridModel"/>.
        /// </summary>
        [XmlIgnore]
        public GridBaseStylesMap BaseStylesMap
        {
            get
            {
                if (styleInfoMap == null)
                {
                    styleInfoMap = OnCreateBaseStylesMap();
                    OnBaseStylesMapChanged(EventArgs.Empty);
                }
                return styleInfoMap;
            }
            set
            {
                styleInfoMap = value;
                OnBaseStylesMapChanged(EventArgs.Empty);
            }
        }
        /// <summary>
        /// Determines whether <see cref="GridModel.BaseStylesMap"/> has been associated with this <see cref="GridModel"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasBaseStylesMap
        {
            get
            {
                return styleInfoMap != null;
            }
        }
        /// <summary>
        /// This method is called the first time <see cref="GridModel.BaseStylesMap"/> and no
        /// <see cref="GridBaseStylesMap"/> has been associated with the <see cref="GridModel"/> before.
        /// </summary>
        /// <returns>A <see cref="GridBaseStylesMap"/> object.</returns>
        protected virtual GridBaseStylesMap OnCreateBaseStylesMap()
        {
            GridBaseStylesMap styleInfoMap = new GridBaseStylesMap();
            styleInfoMap.RegisterStandardStyles();
            return styleInfoMap;
        }

        /// <summary>
        /// Raises the BaseStylesMapChanged event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data. </param>
        protected virtual void OnBaseStylesMapChanged(EventArgs e)
        {
            try
            {
                if (this.Views != null)
                {
                    foreach (GridControlBase grid in this.Views)
                    {
                        IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                        if (eventsHost != null)
                        {
                            eventsHost.OnBaseStyleMapsChanged(e);
                        }
                    }
                }

                if (this.EventsHost != null)
                    this.EventsHost.OnBaseStyleMapsChanged(e);

                if (BaseStylesMapChanged != null)
                    BaseStylesMapChanged(this, e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;
            }
        }

        internal void RaiseBaseStylesMapChanged(EventArgs e)
        {
            OnBaseStylesMapChanged(e);
        }

        /// <summary>
        /// Occurs when the reference for <see cref="GridModel.BaseStylesMap"/> in <see cref="GridModel"/> has changed.
        /// </summary>
        [
        Description("Occurs when the reference to the BaseStylesMap has changed."),
        Category("Behavior")
        ]
        public event EventHandler BaseStylesMapChanged;


        #endregion

        #region CellModels
        GridCellModelCollection cellModels = null;

        /// <summary>
        /// Manages cell types for the grid.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridCellModelCollection CellModels
        {
            get
            {
                if (cellModels == null)
                    cellModels = new GridCellModelCollection(this);
                return cellModels;
            }
        }

        GridCellModelBase IGridVolatileCellStylesHost.LookupCellModel(string id)
        {
            return CellModels[id];
        }


        /// <summary>
        /// Occurs when the CellModels collection is changed.
        /// </summary>
        [
        Description("Occurs when the CellModels collection is changed"),
        Category("Behavior")
        ]
        public event CollectionChangeEventHandler CellModelsChanged;

        /// <summary>
        /// Raises the <see cref="GridModel.CellModelsChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CollectionChangeEventArgs" /> that contains the event data.</param>// Events
        protected virtual void OnCellModelsChanged(CollectionChangeEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnCellModelsChanged(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnCellModelsChanged(e);
            }

            if (CellModelsChanged != null)
            {
                CellModelsChanged(this, e);
            }
        }

        internal bool ignoreCellModelsChanged = false;

        internal void RaiseCellModelsChanged(CollectionChangeEventArgs e)
        {
            if (!ignoreCellModelsChanged)
                OnCellModelsChanged(e);
        }



        /// <summary>
        /// Occurs when the <see cref="GridModel.QueryCellModel"/> is querying for the <see cref="GridCellModelBase"/>
        /// and the cell type is not found in the GridCellModelCollection.
        /// </summary>
        /// <remarks>
        /// The GridModel has a table with all cell types used in the grid. Whenever the grid encounters
        /// a new cell type that it cannot find in the table it will raise a <see cref="GridModel.QueryCellModel"/> event.
        /// The <see cref="GridStyleInfo.CellType"/> identifies the name of the cell type. The
        /// <see cref="GridQueryCellModelEventArgs.CellModel"/> should receive the new instance of the
        /// associated cell object. This object will be stored in the table together with its name and
        /// reused among cells with the same <see cref="GridStyleInfo.CellType"/>.
        /// <para/>
        /// You should process this event if you want to add custom cell types and initialize these
        /// cell types on demand when associated cells are accessed the first time.
        /// </remarks>
        /// <seealso cref="GridQueryCellModelEventHandler"/>
        [
        Description("Occurs is querying for a cell type."),
        Category("Behavior")
        ]
        public event GridQueryCellModelEventHandler QueryCellModel;

        /// <summary>
        /// Raises the <see cref="GridModel.QueryCellModel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryCellModelEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryCellModel(GridQueryCellModelEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellModel(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellModel(e);
            }

            if (QueryCellModel != null)
                QueryCellModel(this, e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        /// <param name="e">The <see cref="GridQueryCellModelEventArgs"/>.</param>
        public void RaiseQueryCellModel(GridQueryCellModelEventArgs e)
        {
            OnQueryCellModel(e);

            if (e.CellModel == null)
            {
                IGridCellModelFactory pGridCellObjectFactory = GridFactoryProvider.CellModelFactory;

                if (pGridCellObjectFactory == null)
                {
                    pGridCellObjectFactory = new GridBaseCellModelFactory(true);
                    GridFactoryProvider.Init(pGridCellObjectFactory);
                }
                e.CellModel = pGridCellObjectFactory.CreateCellModel(e.CellType, this);
            }
        }

        #endregion

        #region CellText
        /// <summary>
        /// Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value.
        /// </summary>
        [
        Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
        Category("Data")
        ]
        public event GridCellTextEventHandler QueryCellFormattedText;

        /// <summary>
        /// Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value.
        /// </summary>
        [
        Description("Occurs each time the GridStyleInfo.FormattedText is called to set the raw string that represents the underlying cell's value."),
        Category("Data")
        ]
        public event GridCellTextEventHandler SaveCellFormattedText;

        /// <summary>
        /// Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value.
        /// </summary>
        [
        Description("Handle this event to provide support for parsing the formatted string and convert it into the the underlying cell's value."),
        Category("Data")
        ]
        public event GridCellTextEventHandler ParseCommonFormats;

        /// <summary>
        /// Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value.
        /// </summary>
        [
Description("Occurs each time the GridStyleInfo.Text is called to get the string that represents the underlying cell's value"),
Category("Data")
]
        public event GridCellTextEventHandler QueryCellText;

        protected virtual void OnQueryCellText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellText(e);
            }

            if (QueryCellText != null)
                QueryCellText(this, e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="GridCellTextEventArgs"/>.</param>
        public void RaiseQueryCellText(GridCellTextEventArgs e)
        {
            OnQueryCellText(e);
        }

        /// <summary>
        /// Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value.
        /// </summary>
        [
        Description("Occurs each time the GridStyleInfo.Text is called to set the raw string that represents the underlying cell's value."),
        Category("Data")
        ]
        public event GridCellTextEventHandler SaveCellText;

        protected virtual void OnSaveCellText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSaveCellText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSaveCellText(e);
            }

            if (SaveCellText != null)
                SaveCellText(this, e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="GridCellTextEventArgs"/>.</param>
        public void RaiseSaveCellText(GridCellTextEventArgs e)
        {
            OnSaveCellText(e);
        }

        protected virtual void OnQueryCellFormattedText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellFormattedText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellFormattedText(e);
            }

            if (QueryCellFormattedText != null)
                QueryCellFormattedText(this, e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="GridCellTextEventArgs"/></param>
        public void RaiseQueryCellFormattedText(GridCellTextEventArgs e)
        {
            OnQueryCellFormattedText(e);
        }

        protected virtual void OnSaveCellFormattedText(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSaveCellFormattedText(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSaveCellFormattedText(e);
            }

            if (SaveCellFormattedText != null)
                SaveCellFormattedText(this, e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="GridCellTextEventArgs"/>.</param>
        public void RaiseSaveCellFormattedText(GridCellTextEventArgs e)
        {
            OnSaveCellFormattedText(e);
        }

        protected virtual void OnParseCommonFormats(GridCellTextEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnParseCommonFormats(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnParseCommonFormats(e);
            }

            if (ParseCommonFormats != null)
                ParseCommonFormats(this, e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="e">The <see cref="GridCellTextEventArgs"/>.</param>
        public void RaiseParseCommonFormats(GridCellTextEventArgs e)
        {
            OnParseCommonFormats(e);

            DefaultParseCommonFormats(e);
        }

        internal void DefaultParseCommonFormats(GridCellTextEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Text == "" || e.Text == null)
                    return;
                //If this is a numeric type with a common format that is not automatically handled by the Parse
                //routine, we'll take care of it here.

                switch (e.Style.Format)
                {
                    case "P":
                        if (!e.Text.EndsWith("%"))
                            return;
                        string s = e.Text;
                        s = s.TrimEnd('%');
                        decimal d = decimal.Parse(s, System.Globalization.NumberStyles.Any, e.Style.GetCulture(true)) * (decimal)0.01;
                        e.Style.CellValue = Convert.ChangeType(d, e.Style.CellValueType);
                        e.Handled = true;
                        break;
                    case "X":
                        Int64 i = Int64.Parse(e.Text, System.Globalization.NumberStyles.AllowHexSpecifier, e.Style.GetCulture(true));
                        e.Style.CellValue = Convert.ChangeType(i, e.Style.CellValueType);
                        e.Handled = true;
                        break;
                    default:
                        return;
                }
            }
        }

        #endregion

        #region QueryCoveredRange
        internal void RaiseQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            OnQueryCoveredRange(e);
        }

        protected virtual void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCoveredRange(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCoveredRange(e);
            }

            if (QueryCoveredRange != null)
                QueryCoveredRange(this, e);
        }


        /// <summary>
        /// Occurs when the model queries information about a covered range at a specific cell.
        /// </summary>
        public event GridQueryCoveredRangeEventHandler QueryCoveredRange;
        #endregion

        #region QueryCellSpanBackgrounds
        internal void RaiseQueryCellSpanBackgrounds(GridQueryCellSpanBackgroundsEventArgs e)
        {
            OnQueryCellSpanBackgrounds(e);
        }

        protected virtual void OnQueryCellSpanBackgrounds(GridQueryCellSpanBackgroundsEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnQueryCellSpansBackground(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnQueryCellSpansBackground(e);
            }

            if (QueryCellSpanBackgrounds != null)
                QueryCellSpanBackgrounds(this, e);
        }

        /// <summary>
        /// Occurs when the model queries information about a spanned background at a specific cell.
        /// </summary>
        public event GridQueryCellSpanBackgroundsEventHandler QueryCellSpanBackgrounds;
        #endregion

        #region Views

        List<GridControlBase> views = new List<GridControlBase>();

        /// <summary>
        /// Gets the grid views for the current grid model.
        /// </summary>
        public IEnumerable<GridControlBase> Views
        {
            get { return views; }
        }

        internal void RemoveView(GridControlBase gridControlBase)
        {
            views.Remove(gridControlBase);
            if (CurrentCellState.GridControl == gridControlBase)
                currentCellState.GridControl = null;
            foreach (GridCellModelBase cellModel in CellModels.Values)
            {
                if (cellModel.ActiveRenderer == gridControlBase)
                    cellModel.ActiveRenderer = null;
            }
        }

        internal void AddView(GridControlBase gridControlBase)
        {
            views.Add(gridControlBase);
            if (CurrentCellState.GridControl == null && !CurrentCellState.IsEmpty)
                currentCellState.GridControl = gridControlBase;
            if (GraphicModel != null && GraphicModel.GridControl == null)
                graphicModel.SetGridControl(gridControlBase);
        }

        #endregion

        #region InvalidateCell

        /// <summary>
        /// Calls GridControlBase.InvalidateCell for each GridControlBase object
        /// associated with this GridModel.
        /// </summary>
        /// <param name="cellRowColumnIndex">The cell row column index.</param>
        public void InvalidateCell(RowColumnIndex cellRowColumnIndex)
        {
            if (!isDisposed)
            {
                foreach (GridControlBase g in views)
                {
                    g.InvalidateCell(cellRowColumnIndex);
                }
            }
        }

        /// <summary>
        /// Calls GridControlBase.InvalidateCell for each GridControlBase object
        /// associated with this GridModel.
        /// </summary>
        /// <param name="span">The spanned ranges to be repainted.</param>
        public void InvalidateCell(CellSpanInfoBase span)
        {
            if (!isDisposed)
            {
                foreach (GridControlBase g in views)
                {
                    g.InvalidateCell(span);
                }
            }
        }

        /// <summary>
        /// Calls GridControlBase.InvalidateCell for each GridControlBase object
        /// associated with this GridModel.
        /// </summary>
        /// <param name="gridRangeInfo">The range of cells to be repainted.</param>
        public void InvalidateCell(GridRangeInfo gridRangeInfo)
        {
            if (!isDisposed)
            {
                CellSpanInfoBase span = gridRangeInfo.ToCellSpan(this);
                InvalidateCell(span);
            }
        }

        /// <summary>
        /// Invalidates the complete visual.
        /// </summary>
        public void InvalidateVisual()
        {
            InvalidateVisual(true);
        }

        /// <summary>
        /// Invalidates the complete visual.
        /// </summary>
        /// <param name="setArrangeDirty">When set to true, forces a complete new layout.</param>
        public void InvalidateVisual(bool setArrangeDirty)
        {
            if(views !=null)
            foreach (GridControlBase g in views)
            {
                g.InvalidateVisual(setArrangeDirty);
            }
        }

        #endregion

        #region Selected Cells

        GridModelSelections selections;

        /// <summary>
        /// Manages selected ranges in the grid. Allows you to add and remove selections, determines
        /// selection state of a specific cell and more.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public GridModelSelections Selections
        {
            get
            {
                return selections;
            }
        }

        /// <summary>
        /// Gets or sets selected cells in the grid.
        /// </summary>
        public GridRangeInfo SelectedCells
        {
            get
            {
                if (SelectedRanges.ActiveRange != null)
                    return SelectedRanges.ActiveRange;
                return GridRangeInfo.Empty;
            }
            set
            {
                Selections.SelectRange(SelectedCells, false);
                Selections.SelectRange(value, true);
            }
        }

        #endregion
        #region CurrentCellState, SelectedRanges
        GridModelCurrentCellState currentCellState = GridModelCurrentCellState.Empty;

        /// <summary>
        /// GridCurrentCell.Activate and GridCurrentCell.Deactivate
        /// set and reset this state.
        /// </summary>
        public GridModelCurrentCellState CurrentCellState
        {
            get { return currentCellState; }
            set { currentCellState = value; }
        }

        GridRangeInfoList selectedRanges = null;

        /// <summary>
        /// Gets Selected Ranges.
        /// </summary>
        public virtual GridRangeInfoList SelectedRanges
        {
            get
            {
                if (selectedRanges == null)
                    selectedRanges = new GridRangeInfoList();
                return selectedRanges;
            }
        }

        #endregion

        #region Selection Events
        #region SelectionChanged
        internal void RaiseSelectionChanged(GridSelectionChangedEventArgs e)
        {
            OnSelectionChanged(e);
            selectionStateChanged = true;
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectionChanged(GridSelectionChangedEventArgs e)
        {
            if (!CanGridRaiseEvents) return;
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSelectionChanged(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSelectionChanged(e);
            }

            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event after
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outline
        /// the selected range of cells.
        /// </remarks>
        [
        Description("Occurs after internal data structures were updated with new selection state from a SelectRange command."),
        Category("Behavior")
        ]
        public event GridSelectionChangedEventHandler SelectionChanged;
        #endregion
        #region SelectionChanging

        internal void RaiseSelectionChanging(GridSelectionChangingEventArgs e)
        {
            // Be aware that when GridOptions.ExcelLikeCurrentCell is set and you cancel the current cell 
            // activation in CurrentCellActivating event that RaiseSelectionChanging already was already 
            // called earlier from ProcessSetCurrentCell. If you do not want this you should also handle 
            // the SelectionChanging event.

            if (e.Range != null && e.Range.IsCells && CoveredRanges.Count != 0)
            {
                this.CoveredRanges.InvalidateRanges();
                e.Range = CoveredRanges.Ranges.GetOuterRange(e.Range);
            }

            try
            {
                OnSelectionChanging(e);
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                TraceUtil.TraceExceptionCatched(ex);
                //if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                throw;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangingEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectionChanging(GridSelectionChangingEventArgs e)
        {
            if (!CanGridRaiseEvents) return;
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnSelectionChanging(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnSelectionChanging(e);
            }

            if (SelectionChanging != null)
                SelectionChanging(this, e);
        }

        /// <summary>
        /// Occurs before the model updates internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outlines
        /// the selected range of cells.
        /// <para/>
        /// You can disallow the selection of specific cells at run-time when
        /// you assign true to <see cref="CancelEventArgs.Cancel"/>.<para/>
        /// You can also modify the <see cref="GridSelectionChangingEventArgs.Range"/> to include additional cells.
        /// </remarks>
        /// <seealso cref="GridSelectionChangingEventHandler"/>
        /// <seealso cref="GridSelectionChangedEventArgs"/>
        [
        Description("Occurs before internal data structures are updated with new selection state from a SelectRange command."),
        Category("Behavior")
        ]
        public event GridSelectionChangingEventHandler SelectionChanging;

        #endregion
        #endregion

        #region HyperlinkCellEventHandler

        internal static readonly RoutedEvent CellRequestNavigateEvent = EventManager.RegisterRoutedEvent(
            "CellRequestNavigate",
            RoutingStrategy.Direct,
            typeof(CellRequestNavigateEventHandler),
            typeof(GridModel));

        public event CellRequestNavigateEventHandler CellRequestNavigate;

        protected virtual void OnCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            if (CellRequestNavigate != null)
                CellRequestNavigate(this, e);
        }

        internal void RaiseCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            OnCellRequestNavigate(e);
        }

        #endregion

        #region UserData
        /// <summary>
        ///   <para> Gets / sets the user-definable data for the current object.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        public IDictionary UserData
        {
            get
            {
                if (this.userData == null)
                    this.userData = new ListDictionary();
                return this.userData;
            }
        }

        [NonSerialized]
        ListDictionary userData = null;
        #endregion

        #region CutPaste Coding
        /// <summary>
        /// User set the object which implements the IGridCutPaste
        /// </summary>
        private IGridCopyPaste gridCutPaste;

        /// <summary>
        /// For Copy or paste the formatted text
        /// </summary>
        private GridModelTextDataExchange textDataExchange;

        /// <summary>
        /// User call the CanCopy(), Copy() using this object
        /// </summary>
        private GridModelCutPaste cutPaste;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanPaste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanPaste;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanCopy;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Cut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCanCut;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Paste"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardPaste;

        public event GridCutPasteEventHandler ClipboardPasted;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.Copy"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCopy;

        /// <summary>
        /// Occurs when the <see cref="GridModelCutPaste.CanCut"/> is called on the <see cref="CutPaste"/> component of a <see cref="GridModel"/>.
        /// </summary>
        public event GridCutPasteEventHandler ClipboardCut;

        /// <summary>
        /// Gets or sets the GridCutPaste
        /// </summary>
        [XmlIgnore]
        public IGridCopyPaste GridCopyPaste
        {
            get
            {
                if (cutPaste == null)
                {
                    cutPaste = new GridModelCutPaste(this);

                }

                return this.gridCutPaste;
            }

            set
            {
                this.gridCutPaste = value;
            }
        }

        [XmlIgnore]
        /// <summary>
        /// Gets text data exchange for the grid. Lets you copy cell text to a stream or clipboard and recreate the
        /// cell text at a later time.
        /// </summary>
        public virtual GridModelTextDataExchange TextDataExchange
        {
            get
            {
                if (this.textDataExchange == null)
                {
                    this.textDataExchange = new GridModelTextDataExchange(this);
                }

                return this.textDataExchange;
            }
            set
            {
                this.textDataExchange = value;
            }
        }
      
        /// <summary>
        /// Gets clipboard operations for the grid.
        /// </summary>
        public GridModelCutPaste CutPaste
        {
            get
            {
                if (this.cutPaste == null)
                {
                    this.cutPaste = new GridModelCutPaste(this);
                }

                return this.cutPaste;
            }
        }

        /// <summary>
        /// Used Internally.
        /// </summary>
        /// <param name="e">GridCutPasteEventArgs</param>
        internal void RaiseClipboardCanPaste(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanPaste(e);
        }

        internal void RaiseClipboardCanCopy(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanCopy(e);
        }

        /// <summary>
        /// used internally.
        /// </summary>
        internal void RaiseClipboardCanCut(GridCutPasteEventArgs e)
        {
            this.OnClipboardCanCut(e);
        }

        /// <summary>
        /// Used Internally
        /// </summary>
        /// <param name="e"></param>
        internal void RaiseClipboardPaste(GridCutPasteEventArgs e)
        {
            this.OnClipboardPaste(e);
        }

        internal void RaiseClipboardPasted(GridCutPasteEventArgs e)
        {
            this.OnClipboardPasted(e);
        }


        internal void RaiseClipboardCopy(GridCutPasteEventArgs e)
        {
            this.OnClipboardCopy(e);
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        internal void RaiseClipboardCut(GridCutPasteEventArgs e)
        {
            this.OnClipboardCut(e);
        }


        /// <summary>
        /// Raises the <see cref="ClipboardCanPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanPaste(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanPaste(e);
            }

            if (this.ClipboardCanPaste != null)
            {
                this.ClipboardCanPaste(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCopy(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanCopy(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanCopy(e);
            }

            if (this.ClipboardCanCopy != null)
            {
                this.ClipboardCanCopy(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCanCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCanCut(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCanCut(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCanCut(e);
            }

            if (this.ClipboardCanCut != null)
            {
                this.ClipboardCanCut(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardPaste"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardPaste(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardPaste(e);
                }
                var renderer = grid.CurrentCell.Renderer;

                if (renderer != null)
                {
                    grid.CurrentCell.Renderer.RaiseClipboardPaste(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardPaste(e);
            }

            if (this.ClipboardPaste != null)
            {
                this.ClipboardPaste(this, e);
            }
        }
        /// <summary>
        /// This event calls after Clipboard paste.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnClipboardPasted(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardPasted(e);
                }
                var renderer = grid.CurrentCell.Renderer;

                if (renderer != null)
                {
                    grid.CurrentCell.Renderer.RaiseClipboardPasted(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardPasted(e);
            }

            if (this.ClipboardPasted != null)
            {
                this.ClipboardPasted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCopy"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCopy(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCopy(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCopy(e);
            }

            if (this.ClipboardCopy != null)
            {
                this.ClipboardCopy(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ClipboardCut"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridCutPasteEventArgs" /> that contains the event data.</param>
        protected virtual void OnClipboardCut(GridCutPasteEventArgs e)
        {
            foreach (GridControlBase grid in this.Views)
            {
                IGridModelEventsHost eventsHost = grid as IGridModelEventsHost;
                if (eventsHost != null)
                {
                    eventsHost.OnClipboardCut(e);
                }
            }

            if (this.EventsHost != null)
            {
                this.EventsHost.OnClipboardCut(e);
            }

            if (this.ClipboardCut != null)
            {
                this.ClipboardCut(this, e);
            }
        }
        #endregion

        /// <summary>
        /// Gets <see cref="IGridModelEventsHost"/> that defines the events for the grid model.
        /// </summary>
        [XmlIgnore]
        public IGridModelEventsHost EventsHost
        {
            get;
            set;
        }

        /// <summary>
        /// This is called from GridDropDownGridListControlCellModel to initialize datasource on demand.
        /// Override this method to calculate the datasource on demand
        /// only when it is needed and not every time in QueryStyleInfo. Default behavior is to return
        /// style.ChoiceList if not empty. If style.ChoiceList is empty, style.ItemsSource is returned.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object.</param>
        /// <returns>The style datasource.</returns>
        public virtual IEnumerable GetStyleDataSource(GridStyleInfo style)
        {
            IEnumerable dataSource = null;
            // Type valueType = style.CellValueType; Unused local variable

            if (style.ChoiceList != null && style.ChoiceList.Count > 0)
                dataSource = style.ChoiceList;
            else
            {
                if (style.ItemsSource is System.Windows.Data.DataSourceProvider)
                    dataSource = (style.ItemsSource as System.Windows.Data.DataSourceProvider).Data as IEnumerable;
                else
                    dataSource = style.ItemsSource as IEnumerable;
            }
            if (dataSource == null)
            {
                PropertyDescriptor pd = GetPropertyDescriptor(style);
                TypeConverter tc = GetTypeConverter(style);
                if (tc != null && tc.CanConvertTo(typeof(string)) && tc.GetStandardValuesSupported())
                {
                    return this.GetCachedStandardValues(tc, pd != null ? pd.PropertyType : style.CellValueType);
                }
            }

            return dataSource;

        }

        /// <summary>
        /// Returns GridStyleInfo.PropertyDescriptor.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A PropertyDescriptor</returns>
        internal PropertyDescriptor GetPropertyDescriptor(GridStyleInfo style)
        {
            return style.PropertyDescriptor;
        }

        /// <summary>
        /// Returns a TypeConverter with type information about the style.CellValue.
        /// </summary>
        /// <param name="style">The style object</param>
        /// <returns>A TypeConverter</returns>
        internal TypeConverter GetTypeConverter(GridStyleInfo style)
        {
            PropertyDescriptor pd = GetPropertyDescriptor(style);
            if (pd != null)
                return pd.Converter;

            Type type = style.CellValueType;
            if (type != null)
                return TypeDescriptor.GetConverter(type);

            return null;
        }


        Hashtable collectionToIListTable = new Hashtable();

        /// <internalonly/>
        /// <summary>
        /// Returns a list with standard values / possible choices for the specified TyepConverter and Type. Helper routined for <see cref="GetStyleDataSource"/>.
        /// </summary>
        /// <param name="converter">The TypeConverter</param>
        /// <param name="propertyType">The Type</param>
        /// <returns>Possible choices for the specified TyepConverter and Type</returns>
        public GridPropertyStandardValuesList GetCachedStandardValues(TypeConverter converter, Type propertyType)
        {
            if (converter != null && converter.GetStandardValuesSupported())
            {
                string name = converter.GetType().FullName + "@" + propertyType.FullName;
                if (collectionToIListTable.Contains(name))
                    return collectionToIListTable[name] as GridPropertyStandardValuesList;
                else
                {
                    ICollection list = converter.GetStandardValues();
                    GridPropertyStandardValuesList sl = new GridPropertyStandardValuesList("Value", propertyType);
                    sl.AddRange(list);
                    collectionToIListTable[name] = sl;
                    return sl;
                }
            }

            return null;
        }




        #region To Rename

        public GridCoveredCellInfoCollection CoveredRanges
        {
            get { return CoveredCells; }
        }

        bool CanGridRaiseEvents
        {
            get { return true; }
        }


        #endregion

        /// <summary>
        /// Returns true if all the GridRangeInfo in this list is of Type Col, otherwise returns false.
        /// </summary>
        public bool IsCols(GridRangeInfoList rangeList)
        {
            foreach (GridRangeInfo range in rangeList)
            {
                if (range.IsCols)
                    continue;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if all the GridRangeInfo in this list is of Type Row, otherwise returns false.
        /// </summary>
        public bool IsRows(GridRangeInfoList rangeList)
        {
            foreach (GridRangeInfo range in rangeList)
            {
                if (range.IsRows)
                    continue;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Resizes a range of columns to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeColumnsToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            return ResizeColumnsToFit(range, options, false);
        }

        public bool ResizeColumnsToFit(GridRangeInfo range, GridResizeToFitOptions options, bool EnableOptimizedResize)
        {
            if (range.IsEmpty)
                return false;

            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            bool includeHiddenRows = (options & GridResizeToFitOptions.IncludeHiddenCells) != 0;
            // bool none = (options & GridResizeToFitOptions.None) == 0; Unused local variable
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            using (Disposable op = new Disposable()) //OperationFeedback op = new OperationFeedback(Model))
            {
                //op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                double maxWidth = double.MaxValue;

                try
                {
                    //bool bAbort = false;
                    CoveredCellInfo coveredRange;
                    double[] newWidths = LineSizeUtil.GetRange(ColumnWidths, range.Left, range.Right);
                    double[] oldWidths = (double[])newWidths.Clone();
                    List<double> widths = new List<double>();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newWidths.Length; n++)
                            newWidths[n] = -1;
                    }

                    bool doHeader = includeHeaders && range.Top > 0;

                    for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                    {
                        if (doHeader)
                            rowIndex = 0;

                        double width = 0;
                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            coveredRange = CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            bool isCovered = coveredRange != null;

                            // Skip invisible rows.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Width == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                                // Skip invisible rows.
                                canResize = RowHeights[rowIndex] > 0;

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = this[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Width);

                                    // Subtract col heights of previous cols (only the last col can be resized).
                                    if (colIndex > coveredRange.Left)
                                        size.Width -= (int)LineSizeUtil.GetTotal(this.ColumnWidths, coveredRange.Left, colIndex - 1);
                                }
                                else
                                {
                                    if (EnableOptimizedResize)
                                    {
                                        styleInfo = this.Data[rowIndex, colIndex] == null ? new GridStyleInfo() : new GridStyleInfo(this.Data[rowIndex, colIndex]);
                                        GridCellModelBase cellModel = styleInfo.CellModel;
                                        if (cellModel == null)
                                            cellModel = this.CellModels["Static"];

                                        size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                    }
                                    else
                                    {
                                        styleInfo = this[rowIndex, colIndex];
                                        GridCellModelBase cellModel = styleInfo.CellModel;
                                        //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                        size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                    }
                                }

                                width = size.Width + 2;
                            }
                            else
                                width = this.ColumnWidths[colIndex];

                            widths.Add(width);
                            //if (op.ShouldCancel)
                            //    throw new GridUserCanceledException();

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Right <= range.Right)
                                    colIndex = coveredRange.Right;
                                else
                                    continue;
                            }

                            if (canResize && width > newWidths[colIndex - range.Left])
                            {
                                newWidths[colIndex - range.Left] = width;
                            }
                            else if (!canResize && includeHiddenRows)
                            {
                                newWidths[colIndex - range.Left] = width;
                            }
                        }

                        //op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        width = Math.Min(maxWidth, width);

                        if (doHeader)
                        {
                            doHeader = false;
                            rowIndex = range.Top - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newWidths.Length; n++)
                        equal &= newWidths[n] == oldWidths[n];

                    if (!equal)
                    {
                        using (this.ColumnWidths.DeferRefresh())
                        {
                            LineSizeUtil.SetRange(ColumnWidths, range.Left, range.Right, newWidths);
                        }
                    }
                }
                catch
                {
                }

                this.InvalidateVisual();

                return true;
            }
        }

        /// <summary>
        /// Resizes a range of rows to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeRowsToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            if (range.IsEmpty)
                return false;

            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            // bool includeColumns = (options & GridResizeToFitOptions.IncludeHiddenCells) != 0; Unused local variable
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            using (Disposable op = new Disposable()) //OperationFeedback op = new OperationFeedback(Model))
            {
                //op.Description = SR.GetString("GRID_IDM_RESIZECOLS");

                double maxHeight = double.MaxValue;

                try
                {
                    //bool bAbort = false;
                    CoveredCellInfo coveredRange;
                    double[] newHeights = LineSizeUtil.GetRange(RowHeights, range.Top, range.Bottom);
                    double[] oldHeights = (double[])newHeights.Clone();
                    if (!noShrinkSize)
                    {
                        for (int n = 0; n < newHeights.Length; n++)
                        {
                            newHeights[n] = -1;
                        }
                    }

                    bool doHeader = includeHeaders && range.Left > 0;

                    for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                    {
                        if (doHeader)
                        {
                            colIndex = 0;
                        }

                        double height = 0;
                        for (int rowIndex = range.Top; rowIndex <= range.Bottom; rowIndex++)
                        {
                            coveredRange = CoveredCells.GetCoveredCell(rowIndex, colIndex);
                            bool isCovered = coveredRange != null;

                            // Skip invisible columns.
                            bool canResize = false;

                            // Covered cells.
                            if (isCovered && !includeCellsWithinCoveredRange)
                            {
                                if (resizeCoveredCells || coveredRange.Height == 1)
                                {
                                    canResize =
                                        rowIndex == coveredRange.Top // Must be the first covered row.
                                        && colIndex == coveredRange.Right // And the last covered col.
                                        // All cells of covered cell must be with in range to be resized.
                                        && range.Left <= coveredRange.Left && range.Bottom >= coveredRange.Bottom;
                                }
                            }
                            else
                                // Skip invisible columns.
                                canResize = ColumnWidths[colIndex] > 0;

                            if (canResize)
                            {
                                GridStyleInfo styleInfo = null;

                                Size size;

                                if (isCovered)
                                {
                                    styleInfo = this[coveredRange.Top, coveredRange.Left];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(coveredRange.Left, coveredRange.Top, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Height);

                                    // Subtract col heights of previous cols (only the last col can be resized).
                                    if (rowIndex > coveredRange.Top)
                                        size.Height -= (int)LineSizeUtil.GetTotal(this.RowHeights, coveredRange.Top, rowIndex - 1);
                                }
                                else
                                {
                                    styleInfo = this[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);

                                    size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Height);
                                }

                                height = Math.Round(size.Height) + 2;
                            }
                            else
                            {
                                height = this.RowHeights[rowIndex];
                            }

                            //if (op.ShouldCancel)
                            //    throw new GridUserCanceledException();

                            if (isCovered && canResize)
                            {
                                if (coveredRange.Bottom <= range.Bottom)
                                {
                                    rowIndex = coveredRange.Bottom;
                                }
                                else
                                {
                                    continue;
                                }
                            }

                            if (canResize && height > newHeights[rowIndex - range.Top])
                            {
                                newHeights[rowIndex - range.Top] = height;
                            }
                            else if (canResize && includeHeaders)
                            {
                                newHeights[rowIndex - range.Top] = height;
                            }
                        }

                        //op.PercentComplete = (rowIndex - range.Top) * 100 / range.Height;

                        height = Math.Min(maxHeight, height);

                        if (doHeader)
                        {
                            doHeader = false;
                            colIndex = range.Left - 1;
                        }
                    }

                    bool equal = true;
                    for (int n = 0; n < newHeights.Length; n++)
                        equal &= newHeights[n] == oldHeights[n];

                    if (!equal)
                    {
                        using (this.RowHeights.DeferRefresh())
                        {
                            LineSizeUtil.SetRange(RowHeights, range.Top, range.Bottom, newHeights);
                        }
                    }
                }
                catch
                {
                }

                this.InvalidateVisual();

                return true;
            }
        }

        #region Serialization

#if !SILVERLIGHT
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// </summary>
        /// <param name="model">The file name.</param>
        public virtual void Serialize(string fileName)
#else
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// Opens up a SaveFileDialog and saves the serialized data in XML.
        /// </summary>
        public virtual void Serialize()
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if !SILVERLIGHT
                using (var sw = new XmlTextWriter(fileName, Encoding.Default))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#else
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };
                if (sfd.ShowDialog() == true)
                {
                    var stream = sfd.OpenFile();
                    if (stream != null)
                    {
                        using (var sw = new StreamWriter(stream))
                        {
                            xs.Serialize(sw.BaseStream, this);
                        }
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes to stream.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
#if !SILVERLIGHT
        public virtual void SerializeToStream(TextWriter textWriter)
#else
        public virtual void SerializeToStream(Stream stream)
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if !SILVERLIGHT
                using (var sw = new XmlTextWriter(textWriter))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#else
                using (var sw = new StreamWriter(stream))
                {
                    xs.Serialize(sw.BaseStream, this);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes the <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> properties as string.
        /// </summary>
        /// <returns>Serialized data as String</returns>
        public virtual string SerializeAsString()
        {
            var result = string.Empty;
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
                using (var sWriter = new StringWriter())
                {
#if !SILVERLIGHT
                    using (var sw = new XmlTextWriter(sWriter))
                    {
                        xs.Serialize(sw, this);
                    }
#else
                    using (var sw = XmlWriter.Create(sWriter))
                    {
                        xs.Serialize(sw, this);
                    }
#endif
                    result = sWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/>.
        /// </summary>
        public virtual void Deserialize(string fileName)
#else
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> 
        /// from an XML file read with an OpenFileDialog.
        /// </summary>
        public virtual void Deserialize()
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(fileName))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#else
                OpenFileDialog ofd = new OpenFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };

                if (ofd.ShowDialog() == true)
                {
                    var stream = ofd.File.OpenRead();
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                         var newModel = xs.Deserialize(sr) as GridModel;
                         Apply(newModel);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
#if !SILVERLIGHT
        public virtual void DeserializeFromStream(TextReader textReader)
#else
        public virtual  void DeserializeFromStream(Stream stream)
#endif
        {
            try
            {
                var xs = new XmlSerializer(typeof(GridModel));
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(textReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#else
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> from a FileStream. 
        /// The File can be read from an Isolated Storage with the <see cref="System.IO.FileStream"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="System.IO.FileStream"/> has to be closed or used inside a closure when calling this method.
        /// </remarks>
        /// <param name="fileStream">The file stream.</param>
        public virtual  void Deserialize(FileStream fileStream)
        {
            var xs = new XmlSerializer(typeof(GridModel));
            try
            {
                using (var sr = new StreamReader(fileStream))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }

            }
            catch (Exception)
            {
                throw ex;
            }
        }
#endif

        /// <summary>
        /// Deserializes <see cref="Syncfusion.Windows.Controls.Grid.GridModel"/> properties from string.
        /// </summary>
        /// <param name="content">The content.</param>
        public virtual void DeserializeFromString(string content)
        {
            var xs = new XmlSerializer(typeof(GridModel));
            using (var sReader = new StringReader(content))
            {
#if !SILVERLIGHT
                using (var sr = new XmlTextReader(sReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#else
                using (var sr = XmlReader.Create(sReader))
                {
                    var newModel = xs.Deserialize(sr) as GridModel;
                    Apply(newModel);
                }
#endif
            }
        }

        private void Apply(GridModel newModel)
        {
            for (int i = 0; i < this.views.Count(); i++)
            {
                var grid = this.views[i];

                // Only the Table Properties can be serialized. Hence some the values which are collection and objects cant be seriialized. 
                // the Properties that are not serializable are added to the new GridModel before deserilization. 

                //newModel.columnWidths = grid.Model.columnWidths;
                //newModel.rowHeights = grid.Model.rowHeights;

                grid.Model = newModel;
                grid.InvalidateCells();
            }
        }
        #endregion


        #region OperationFeedback

        /// <summary>
        /// Occurs when an operation takes a longer time and the user should be notified
        /// about its status and have a chance to abort.
        /// </summary>
        /// <remarks>
        /// See <see cref="OperationFeedbackEventArgs"/> for more detailed discussion about this event.
        /// </remarks>
        [Description("Occurs when an operation takes a longer time and the user should be notified about its status."),
        Category("Behavior")]
        public event OperationFeedbackEventHandler OperationFeedback;

        [NonSerialized]
        Stack feedbackStack = new Stack();
        void IOperationFeedbackProvider.RaiseOperationFeedbackEvent(OperationFeedbackEventArgs e)
        {
            if (OperationFeedback != null)
            {
                OperationFeedback(this, e);
            }
        }

        Stack IOperationFeedbackProvider.FeedbackStack
        {
            get { return feedbackStack; }
        }
        #endregion;
    }

    

    /// <internalonly/>
    public class GridPropertyStandardValuesList : ArrayList, ITypedList
    {
        string name;
        Type elementType;
        PropertyDescriptorCollection pdc;
        int maxLength = -1;
        internal string format = "";
        internal CultureInfo ci;

        /// <internalonly/>
        public GridPropertyStandardValuesList(string name, Type elementType)
        {
            this.name = name;
            this.elementType = elementType;
        }

        #region ITypedList Members

        /// <internalonly/>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (pdc == null)
            {
                PropertyDescriptor pd = new GridPropertyStandardValuesSelfPropertyDescriptor(this, name, elementType);
                pdc = new PropertyDescriptorCollection(new PropertyDescriptor[] { pd });
            }
            return pdc;
        }

        /// <internalonly/>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return name;
        }
        #endregion

        /// <internalonly/>
        public int GetMaxLength(string format, CultureInfo ci)
        {
            if (this.maxLength == -1 || format != this.format || ci != this.ci)
            {
                this.maxLength = -1;
                this.format = format;
                this.ci = ci;
                for (int n = 0; n < Count; n++)
                {
                    maxLength = Math.Max(ValueConvert.FormatValue(this[n], elementType, format, ci, null, null).Length, maxLength);
                }
            }
            return maxLength;
        }

        /// <internalonly/>
        public override int Add(object value)
        {
            return base.Add(value);
        }

        /// <internalonly/>
        public override void AddRange(ICollection c)
        {
            base.AddRange(c);
        }

        /// <internalonly/>
        public override void Clear()
        {
            base.Clear();
        }

        /// <internalonly/>
        public override void Insert(int index, object value)
        {
            base.Insert(index, value);
        }

        /// <internalonly/>
        public override void InsertRange(int index, ICollection c)
        {
            base.InsertRange(index, c);
        }

        /// <internalonly/>
        public override void Remove(object obj)
        {
            base.Remove(obj);
        }

        /// <internalonly/>
        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);
        }

        /// <internalonly/>
        public override void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
        }

        /// <internalonly/>
        public override object this[int index]
        {
            get
            {
                return base[index];
            }

            set
            {
                base[index] = value;
            }
        }



    }


    /// <internalonly/>
    public class GridPropertyStandardValuesSelfPropertyDescriptor : PropertyDescriptor
    {
        Type type;
        GridPropertyStandardValuesList parent;

        /// <summary>
        /// Initializes a new PropertyDescriptor and attaches it to a FieldDescriptor.
        /// </summary>
        public GridPropertyStandardValuesSelfPropertyDescriptor(GridPropertyStandardValuesList parent, string name, Type type)
            : base(name, null)
        {
            this.parent = parent;
            this.type = type;
        }

        /// <override/>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }


        /// <override/>
        public override void SetValue(object component, object value)
        {
        }


        /// <override/>
        public override void ResetValue(object component)
        {
        }


        /// <override/>
        public override object GetValue(object component)
        {
            return ValueConvert.FormatValue(component, type, parent.format, parent.ci, null, null);
        }


        /// <override/>
        public override bool CanResetValue(object component)
        {
            return false;
        }


        /// <override/>
        public override Type PropertyType
        {
            get
            {
                return type;
            }
        }

        /// <override/>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <override/>
        public override Type ComponentType
        {
            get
            {
                return type;
            }
        }

        /// <override/>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <override/>
        public override bool Equals(object other)
        {
            return Object.ReferenceEquals(this, other);
        }
    }


    public struct GridModelCurrentCellState : IDisposable
    {
        GridControlBase grid;
        RowColumnIndex cellRowColumnIndex;

        public static GridModelCurrentCellState Empty = new GridModelCurrentCellState(null, RowColumnIndex.Empty);

        public GridModelCurrentCellState(GridControlBase grid, RowColumnIndex cellRowColumnIndex)
        {
            this.grid = grid;
            this.cellRowColumnIndex = cellRowColumnIndex;
        }

        public bool IsEmpty
        {
            get { return grid == null; }
        }

        [XmlIgnore]
        public GridControlBase GridControl
        {
            get { return grid; }
            internal set { grid = value; }
        }

        public RowColumnIndex CellRowColumnIndex
        {
            get { return cellRowColumnIndex; }
        }

        public int RowIndex
        {
            get { return cellRowColumnIndex.RowIndex; }
        }

        public int ColumnIndex
        {
            get { return cellRowColumnIndex.ColumnIndex; }
        }

        public void Dispose()
        {
            if (grid != null)
            {
                grid.Dispose(true);
                grid = null;
            }
        }
    }
}