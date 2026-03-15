//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableControl.cs" company="syncfusion">
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
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Table = Syncfusion.Grouping.Table;
#if SyncfusionFramework4_0
using System.Windows.Automation.Provider;
#elif SyncfusionFramework3_5
using System.Windows.Automation.Provider;
#endif

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// A grid control that displays rows with <see cref="Syncfusion.Grouping.Table.DisplayElements"/> of a <see cref="GridTable"/> and
    /// allows grouping and filtering of records and editing, deleting, and adding of records.
    /// </summary>
    public class GridTableControl : GridControlBase, Syncfusion.Grouping.ITableProvider
    {
        bool SupportsYAmount
        {
            get
            {
                return this.TableDescriptor.Engine.SupportsYAmount;
            }
        }
        #region Fields
        bool initialized = false;
        bool inInitialize = false;
        bool inUpdateScrollBars = false;
        bool inCurrentRecordContextChange = false;
        bool inPaint = false;
        int cachedLastRow = -1;
        int cachedLastRowCount = -1;
        internal ChildTable MouseOperationChildTable = null;
        static bool paintCalled = false;
        Rectangle paintRect = Rectangle.Empty;
        bool inRefresh = false;
        bool leaveRow = false;
        bool rejected = false;
        internal GridColumnDescriptor currentCellColumnDescriptor = null;
        internal bool currentCellIsEditing = false;
        internal object currentCellEditingState;
        bool inTable_DisplayElementChanging = false;
        Element synchronizeGridNavigateTo;
        bool inControlBeginEdit = false;
        bool inOnCurrentCellAcceptedChanges = false;
        bool inControlEnterRecord = false;
        Element currentCellMoveToElement = null;
        bool inControlLeaveRecord = false;
        bool inOnCurrentCellActivating = false;
        internal bool isDeactivated = false;
        string controlText;
        bool inOnCurrentCellDeactivating = false;
        bool inOnQueryNextCurrentCellPosition = false;
        bool _inProcessKeyEventArgs = false;
        int maximumWidth = -1;
        int frozenRowCount = 0;
        int frozenRowCountVersion = 0;

        static bool traceSynchronizeGridWithEngine = false;
        internal bool InMouseMove = false;
        bool inControlCancelEdit = false;
        bool inControlEndEdit = false;
        internal int _rowIndex, _colIndex;
        internal bool HitMouseDoubleClick = false;
        internal bool HitMouseClick = false;
        bool wasEnquired = false;
        //// internal CurrentRecordContextChangeEventHandler onCurrentRecordContextChange;
        #endregion
        #region Construct
        static GridTableControl()
        {
#if TRACE
            traceSynchronizeGridWithEngine = Switches.SynchronizeGridWithEngine.TraceVerbose;
#endif
        }

        ////                                      /// <overload>
        ////                                      /// Initializes a new control.
        ////                                      /// </overload>
        ////                                      /// <summary>
        ////                                      /// Initializes a new control.
        ////                                      /// </summary>
        ////                                      public GridTableControl()
        ////                                                         : this(new GridTableModel())
        ////                                      {
        ////                                                         object unused = rejected;
        ////                                                         unused = inOnCurrentCellDeactivating ;
        ////                                      }

        /// <summary>
        /// Initializes a new control.
        /// </summary>
        /// <param name="model">The model for this control.</param>
        public GridTableControl(GridModel model)
            : base(model)
        {
            this.PreJitPaint = false;
            ////base.WantEnterKey = false;
            GridFactoryProvider.Init(new GridGroupingCellObjectFactory());
            if (!(this is GridNestedTableControl))
            {
                Application.Idle += new EventHandler(Application_Idle);
                ////this.EnableIntelliMouse = true;
                this.HScrollPixel = true;
            }
            //// onCurrentRecordContextChange = new CurrentRecordContextChangeEventHandler(this.Table_CurrentRecordContextChange);
            ////OptimizeInsertRemoveCells = true;
            ////OptimizeDrawBackground = true;
            AllowTextBoxAutoSize = false;
            //// this.SetStyle(ControlStyles.DoubleBuffer, false);
            //// this.SetStyle(ControlStyles.AllPaintingInWmPaint, false);
            this.InsideScrollMargins = new Size(40, 20);
            CurrentCell.defaultUpdateFlag = false;
            object unused = rejected;
            unused = inOnCurrentCellDeactivating;
            this.TabStop = false;
        }

        /// <override/>
        protected override void OnSplitterPaneClosed(EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            base.OnSplitterPaneClosed(e);
        }

        /// <override/>
        /// <summary>
        /// Returns a string holding the current object.
        /// </summary>
        /// <returns>String representation of the current object.</returns>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            return GetType().Name + " { " + (Table != null ? Table.ToString() : string.Empty) + isdisposed + " }";
        }

        #endregion
        #region PreJIt
        /// <override/>
        protected override void OnEnsurePaintCodeJitted()
        {
            if (paintCalled)
            {
                return;
            }
        }

        void _EnsurePaintCodeJitted()
        {
            paintCalled = true;
            GridEngine engine = new GridEngine();
            Model.Table = engine.Table;
            base.OnEnsurePaintCodeJitted();
        }
        #endregion
        #region WireModel
        /// <summary>
        /// Sets up listeners for the <see cref="GridModel"/> and initializes mouse controllers and data object consumers.
        /// </summary>
        protected override void WireModel()
        {
            base.WireModel();

            Model.SelectionChanged += new GridSelectionChangedEventHandler(ModelSelectionChanged);
            Model.PrepareClearSelection += new EventHandler(ModelPrepareClearSelection);
            Model.PrepareChangeSelection += new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);

            Model.TableChanged += new EventHandler(Model_TableChanged);
            Model.TableChanging += new EventHandler(Model_TableChanging);

            //// if (HasTable)
            //// WireTable(Table);

            Model.CurrentRecordContextChange += new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
            Model.DisplayElementChanging += new DisplayElementChangingEventHandler(Table_DisplayElementChanging);
            Model.DisplayElementChanged += new DisplayElementChangedEventHandler(Table_DisplayElementChanged);
            Model.TableSourceListChanged += new TableEventHandler(Table_SourceListChanged);
            Model.CurrentRecordManagerReset += new TableEventHandler(Table_CurrentRecordManagerReset);
            Model.SourceListRecordChanged += new RecordChangedEventHandler(Table_SourceListRecordChanged);
            Model.SourceListRecordChanging += new RecordChangedEventHandler(Table_SourceListRecordChanging);
            Model.SourceListListChanged += new TableListChangedEventHandler(Table_SourceListListChanged);
            Model.RecordValueChanged += new RecordValueChangedEventHandler(Table_RecordValueChanged);
            Model.SelectedRecordsChanged += new SelectedRecordsChangedEventHandler(Model_SelectedRecordsChanged);
            Model.SelectedRecordsChanging += new SelectedRecordsChangedEventHandler(Model_SelectedRecordsChanging);
        }

        /// <summary>
        /// Releases listeners for the <see cref="GridModel"/>.
        /// </summary>
        protected override void UnwireModel()
        {
            base.UnwireModel();

            if (Model != null)
            {
                Model.SelectionChanged -= new GridSelectionChangedEventHandler(ModelSelectionChanged);
                Model.PrepareClearSelection -= new EventHandler(ModelPrepareClearSelection);
                Model.PrepareChangeSelection -= new GridPrepareChangeSelectionEventHandler(ModelPrepareChangeSelection);

                Model.TableChanged -= new EventHandler(Model_TableChanged);
                Model.TableChanging -= new EventHandler(Model_TableChanging);

                ////  if (Model.HasTable)
                ////  UnwireTable(Table);
                Model.CurrentRecordContextChange -= new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
                Model.DisplayElementChanging -= new DisplayElementChangingEventHandler(Table_DisplayElementChanging);
                Model.DisplayElementChanged -= new DisplayElementChangedEventHandler(Table_DisplayElementChanged);
                Model.TableSourceListChanged -= new TableEventHandler(Table_SourceListChanged);
                Model.CurrentRecordManagerReset -= new TableEventHandler(Table_CurrentRecordManagerReset);
                Model.SourceListRecordChanged -= new RecordChangedEventHandler(Table_SourceListRecordChanged);
                Model.SourceListRecordChanging -= new RecordChangedEventHandler(Table_SourceListRecordChanging);
                Model.SourceListListChanged -= new TableListChangedEventHandler(Table_SourceListListChanged);
                Model.RecordValueChanged -= new RecordValueChangedEventHandler(Table_RecordValueChanged);
                Model.SelectedRecordsChanged -= new SelectedRecordsChangedEventHandler(Model_SelectedRecordsChanged);
                Model.SelectedRecordsChanging -= new SelectedRecordsChangedEventHandler(Model_SelectedRecordsChanging);
            }
        }
        #endregion
        #region GridControlBase Initialize Overrides

        /// <override/>
        protected override void InitializeMouseControllers()
        {
                if (ExcelLikeFrameSelections == null && !this.IsWindowless)
                {
                    ExcelLikeFrameSelections = new GridPaintExcelLikeSelection(this);
                }

            Model.Options.ControllerOptions = GridControllerOptions.None;
            ////GridTableSelectCellsMouseController
            {
                GridTableSelectCellsMouseController msc = new GridTableSelectCellsMouseController(this);
                msc.MultiExtendedShouldMoveCurrentCell = false;
                this.MouseControllerDispatcher.Add(msc);
            }
            ////GridTableClickCellsMouseController
            {
                GridTableClickCellsMouseController msc = new GridTableClickCellsMouseController(this);
                ////msc.MultiExtendedShouldMoveCurrentCell = false;
                this.MouseControllerDispatcher.Add(msc);
            }

            this.MouseControllerDispatcher.Add(new GridResizeCellsMouseController(this));

            base.InitializeMouseControllers();
        }

        GridTableControlSelectRecords selectRecords;

        /// <summary>
        /// Handles the multiple record selection behavior of the control.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridTableControlSelectRecords SelectRecords
        {
            get
            {
                if (selectRecords == null)
                {
                    selectRecords = new GridTableControlSelectRecords(this);
                }

                return selectRecords;
            }
        }

        private void Model_SelectedRecordsChanged(object sender, SelectedRecordsChangedEventArgs e)
        {
            SelectRecords.ProcessSelectedRecordsChanged(e);
        }

        private void Model_SelectedRecordsChanging(object sender, SelectedRecordsChangedEventArgs e)
        {
            SelectRecords.ProcessSelectedRecordsChanging(e);
        }

        /// <override/>
        protected override void OnMouseActivating(CancelEventArgs e)
        {
            // Needed when clicking from one nested cell into another nested cell.
            Element oldCurrentRecord = Table.CurrentElement;
            while (oldCurrentRecord is NestedTable)
            {
                if (oldCurrentRecord.ParentTable != null)
                    oldCurrentRecord = ((NestedTable)oldCurrentRecord).ChildTable.ParentTable.CurrentElement;
            }

            if (oldCurrentRecord != null && oldCurrentRecord.ParentTable != null)
            {
                int rowIndex0 = Table.NestedDisplayElements.IndexOf(oldCurrentRecord);
                if (ViewLayout.IsRowVisible(rowIndex0))
                {
                    InvalidateRange(GridRangeInfo.Row(rowIndex0));
                }
            }

            base.OnMouseActivating(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessTableControlMoveCurrentCellDirection(GridTableControlMoveCurrentCellDirectionEventArgs e)
        {
            SelectRecords.MoveCurrentCellDirection(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessTableClickCellsMouseDown(GridTableClickCellsEventArgs e)
        {
            SelectRecords.MouseDown(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessTableClickCellsMouseMove(GridTableClickCellsEventArgs e)
        {
            SelectRecords.MouseMove(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessTableClickCellsMouseUp(GridTableClickCellsEventArgs e)
        {
            SelectRecords.MouseUp(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessTableClickCellsCancelMode(GridTableClickCellsEventArgs e)
        {
            SelectRecords.CancelMode(e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessSelectedRecordCellDrawn(GridTableControl tableControl, GridDrawCellEventArgs e)
        {
            SelectRecords.SelectedRecordCellDrawn(tableControl, e);
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ProcessSelectedRecordPrepareViewStyleInfo(GridTableControl tableControl, GridPrepareViewStyleInfoEventArgs e)
        {
            SelectRecords.SelectedRecordPrepareViewStyleInfo(tableControl, e);
        }

        /// <override/>
        protected override void InitLayout()
        {
            try
            {
                Model.inInitLayout = true;
                if (Model.ActiveGridView == null)
                {
                    Model.ActiveGridView = this;
                }

                base.InitLayout();
            }
            finally
            {
                Model.inInitLayout = false;
            }
        }

        /// <override/>
        /// <summary>Initializes the control.</summary>
        public override void Initialize()
        {
            if (inInitialize)
            {
                return;
            }

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inInitialize = true;
            try
            {
                if (!Model.inInitLayout)
                {
                    base.Initialize();

                    this.SelectRecords.Init();

                    ////this.VerticalThumbTrack = true;
                    ////this.HorizontalThumbTrack = true;
                    this.ForceCurrentCellMoveTo = true;

                    this.MouseControllerDispatcher.Add(new GridTableControlDragHeaderMouseController(this));
                    this.MouseControllerDispatcher.Add(new GroupDragStackedHeaderMouseController(this));

                    foreach (string name in Model.CellModels.Keys)
                    {
                        object obj = this.CellRenderers[name];
                    }

                    initialized = true;
                }
            }
            finally
            {
                inInitialize = false;
            }
        }

        /// <summary>
        /// Gets whether the <see cref="Initialize"/> method was called.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool Initialized
        {
            get
            {
                return initialized;
            }
        }

        /// <override/>
        /// <summary>
        /// Creates a new table control.
        /// </summary>
        /// <param name="parent">The parent control.</param>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns>The new table control.</returns>
        public override Control CreateNewControl(Control parent, int row, int column)
        {
            GridTableControl other = this.GroupingControl.CreateTableControl(Model);
            other.groupingControl = this.groupingControl;
            return other;
        }

        #endregion
        #region PaintSelectCells
        private GridPaintSelectCells paintSelectCells = null;

        /// <override/>
        protected override void IntUpdateSelectRange()
        {
            if (UpdateSelectRange_Range != null && !UpdateSelectRange_Range.IsEmpty)
            {
                PaintSelectCells.UpdateSelectRange(UpdateSelectRange_Range, UpdateSelectRange_OldRange);
                UpdateSelectRange_Range = null;
                UpdateSelectRange_OldRange = null;
            }
        }

        /// <override/>
        /// <summary>Used internally.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override IGridPaintSelectCells PaintSelectCells
        {
            get
            {
                if (paintSelectCells == null)
                {
                    paintSelectCells = new GridPaintSelectCells(this);
                }

                return paintSelectCells;
            }
        }

        void ModelSelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.OldRanges != null)
            {
                if (PaintSelectCells != null)
                {
                    PaintSelectCells.UpdateSelectRange(e.Range, e.OldRanges);
                }
            }
        }

        void ModelPrepareClearSelection(object sender, EventArgs e)
        {
            if (PaintSelectCells != null)
            {
                PaintSelectCells.PrepareClearSelection();
            }
        }

        void ModelPrepareChangeSelection(object sender, GridPrepareChangeSelectionEventArgs e)
        {
            if (PaintSelectCells != null)
            {
                PaintSelectCells.PrepareChangeSelection(e.OldRange, e.NewRange);
            }
        }

        #endregion
        #region HitTest methods
        /// <summary>
        /// Determines the <see cref="GridColumnDescriptor"/> displayed at the specified point if it is a header cell.
        /// </summary>
        /// <param name="point">The point in device coordinates.</param>
        /// <returns>The <see cref="GridColumnDescriptor"/> displayed at the specified point if it is a header cell; NULL otherwise.</returns>
        public GridColumnDescriptor GetHeaderColumnDescriptorAt(Point point)
        {
            return Model.GetHeaderColumnDescriptorAt(PointToRangeInfo(point));
        }

        /// <exclude/>
        /// <summary>
        /// Determines the <see cref="GridStackedHeaderSpan"/> displayed at the specified point if it is a header cell.
        /// </summary>
        /// <param name="point">The point in device coordinates.</param>
        /// <returns>The <see cref="GridStackedHeaderSpan"/> displayed at the specified point if it is a header cell; NULL otherwise.</returns>
        public GridStackedHeaderSpan GetStackedHeaderSpanAt(Point point)
        {
            return Model.GetStackedHeaderSpanAt(PointToRangeInfo(point));
        }
        #endregion
        #region GridTableModel and NavigationBarRecordChanged handler

        /// <summary>
        /// The model for this view.
        /// </summary>
        public new GridTableModel Model
        {
            get
            {
                return (GridTableModel)base.Model;
            }

            set
            {
                base.Model = value;
            }
        }

        private void Model_NavigationBarRecordChanged(object sender, CurrentRecordEventArgs e)
        {
            SplitterControl parent = this.Parent as SplitterControl;
            bool scroll = parent != null && parent.ActivePane == this;
            if (HasTable)
            {
                this.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
                this.SynchronizeCurrentCellWithRecord(Table.CurrentElement, scroll);
                this.EndUpdate(false);
            }
        }

        #endregion
        #region Control Overrides
        /// <override/>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (InUpdateWithCustomPaint)
            {
                base.OnPaint(e);
                return;
            }

            if (this.GroupingControl != null)
            {
                if (!GroupingControl.OnTableControlPaint(this, e))
                {
                    return;
                }
            }

            /* if (this.ParentDesignMode)
            //                                                         {
            //                                                                            TableDescriptor.Engine.Table.InvalidateCounterTopDown(true);
            //                                                                            TableDescriptor.ColumnSets.Version++;
            //                                                                            TableDescriptor.VisibleColumns.Version++;
            //                                                                            TableDescriptor.Columns.Version++;
            //                                                                            TableDescriptor.Fields.Version++;
            //                                                                            TableDescriptor.ItemPropertiesVersion++;
            //
            //                                                                            foreach (GridTable t in TableDescriptor.Engine.Table.RelatedTables)
            //                                                                            {
            //                                                                                               t.InvalidateCounterTopDown(true);
            //                                                                                               t.TableDescriptor.ColumnSets.Version++;
            //                                                                                               t.TableDescriptor.VisibleColumns.Version++;
            //                                                                                               t.TableDescriptor.Columns.Version++;
            //                                                                                               t.TableDescriptor.Fields.Version++;
            //                                                                                               t.TableDescriptor.ItemPropertiesVersion++;
            //                                                                            }
            //                                                                            ViewLayout.Reset();
            //                                                                            this.SynchronizeGridWithEngine();
            //
            //                                                                            Syncfusion.Grouping.Diagnostics.IterateThroughDisplayElement(Table);
            //                                                                            Syncfusion.Grouping.Diagnostics.IterateThroughNestedDisplayElement(Table);
                                                                   }*/
            if (!this.IsWindowless)
            {
                GridGroupingControl groupingControl = this.GroupingControl;
                if (groupingControl != null)
                {
                    GridEngine engine = groupingControl.Engine;
                    engine.parentControl = groupingControl;
                    engine.ForwardTableEvents = groupingControl;
                }
            }

            KillDelayUpdateTimer();

            paintCalled = true;

            // TraceUtil.TraceCurrentMethodInfo(e.ClipRectangle, e.Graphics.ClipBounds);

            // With Windows Vista when changing TextBox.MultiLine or other properties
            // in OnDraw routine it will immeditately force a repaint of the grid area.
            if (inPaint)
            {
                return;
            }

            Debug.Assert(!inPaint);
            inPaint = true;
            if (!initialized)
            {
                Initialize();
            }

            paintRect = e.ClipRectangle;
            FixTopLeftRowCol(); ////SR1

            try
            {
                base.OnPaint(e);
                if (this.ParentDesignMode && (this.groupingControl == null || (this.groupingControl != null && this.groupingControl.DataSource == null)))
                {
                    SetControlDisplayInfo(e.Graphics);
                }
            }
            finally
            {
                inPaint = false;
            }
        }

        private void SetControlDisplayInfo(Graphics g)
        {
            Rectangle rect = this.ClientRectangle;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            ////sf.FormatFlags &= ~StringFormatFlags.NoWrap;
            g.DrawString("Right click and choose the required menu options (or use smart tag in VS.NET) to configure the Grid.", this.Font, new SolidBrush(SystemColors.ControlDark), rect, sf);
        }

        /// <override/>
        protected override void OnDrawCell(GridDrawCellEventArgs e)
        {
            // if (this.ParentDesignMode)
            // TraceUtil.TraceCurrentMethodInfo(e);
            Table.repaintElementQueue.Remove(((GridTableCellStyleInfo)e.Style).TableCellIdentity.DisplayElement);
            Table.repaintElementQueue.Remove(((GridTableCellStyleInfo)e.Style).TableCellIdentity.DisplayElement.ParentRecord);
            base.OnDrawCell(e);
        }

        /// <override/>
        protected override void OnSizeChanged(EventArgs e)
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);
            if (!initialized)
            {
                return;
            }

            base.OnSizeChanged(e);
        }

        #endregion
        #region ScrollControl overrides
        /// <override/>
        protected override void OnScrollTipFeedback(ScrollTipFeedbackEventArgs e)
        {
            if (HasTable)
            {
                if (e.Value >= 0)
                {
                    if (e.ScrollBar == ScrollBars.Horizontal)
                    {
                        int colIndex;
                        int delta;
                        if (this.HScrollPixel)
                        {
                            this.HScrollPixelPosToColIndex(e.Value, out colIndex, out delta);
                        }
                        else
                        {
                            colIndex = e.Value;
                        }

                        int field = Model.ColIndexToField(colIndex);
                        GridColumnDescriptor[,] recordRowColumns = Table.TableDescriptor.RecordRowColumns;
                        if (field < recordRowColumns.GetLength(1))
                        {
                            GridColumnDescriptor column = recordRowColumns[0, field];
                            if (column != null)
                            {
                                e.Text = column.HeaderText;
                            }

                            e.Font = Table.Appearance.ColumnHeaderCell.Font.GdipFont;
                            if (e.Action == ScrollTipActions.ThumbTrack)
                            {
                                e.Size = ScrollTip.GetPreferredSize("Long Column Text");
                            }
                        }
                    }
                    else if (e.ScrollBar == ScrollBars.Vertical)
                    {
                        int rowIndex;
                        int delta;
                        if (this.VScrollPixel)
                        {
                            this.VScrollPixelPosToRowIndex(e.Value, out rowIndex, out delta);
                        }
                        else
                        {
                            rowIndex = e.Value;
                        }

                        Table table = Table.EngineTable;
                        if (rowIndex > 0 && rowIndex < table.NestedDisplayElements.Count)
                        {
                            Element el = table.NestedDisplayElements[rowIndex];
                            string t = string.Empty;
                            Group parentGroup = el.ParentGroup;
                            object cat = parentGroup.CategoriesToString();
                            if (cat == null || cat is DBNull)
                            {
                                cat = "(Null)";
                            }

                            t = "Category: " + cat.ToString() + "   ";
                            if (el.ParentElement is Record && !(el.ParentElement is AddNewRecord))
                            {
                                t += " - ";
                            }

                            if (parentGroup != null && parentGroup.IsTopLevelGroup)
                            {
                                e.Size = new Size(200, 20);
                                if (Record.GetParentRecord(el) is AddNewRecord)
                                {
                                    t = "AddNew Record";
                                }
                                else if (Record.GetParentRecord(el) != null)
                                {
                                    t = "Record " + (Table.FilteredRecords.IndexOf(el.ParentRecord) + 1).ToString();
                                }
                            }

                            e.Text = t;
                            e.Font = Table.Appearance.ColumnHeaderCell.Font.GdipFont;
                            ////if (e.Action == ScrollTipActions.ThumbTrack)
                            e.Size = ScrollTip.GetPreferredSize(e.Text);
                        }
                    }
                }
            }

            base.OnScrollTipFeedback(e);
        }
        #endregion
        #region Scrolling
        /// <summary>
        /// Should scroll the nested table.
        /// </summary>
        /// <param name="nt">The nestedtable.</param>
        /// <returns>returns true if frozen column is not equal to string.empty, else false</returns>
        /// <exclude/>
        protected internal virtual bool ShouldScrollNestedTable(NestedTable nt)
        {
            GridTableDescriptor td = nt.ChildTable.ParentTableDescriptor as GridTableDescriptor;
            return td.GetFrozenColumn() != string.Empty;
        }

        /// <summary>
        /// Should invalidate when scrolled.
        /// </summary>
        /// <param name="nt">The nested table.</param>
        /// <returns>returns true if frozen column is not equal to string.empty, else false</returns>
        /// <exclude/>
        protected internal virtual bool ShouldInvalidatedWhenScrolled(NestedTable nt)
        {
            GridTableDescriptor td = nt.ChildTable.ParentTableDescriptor as GridTableDescriptor;
            if (td.ParentTableDescriptor != null && td.GetFrozenColumn() != string.Empty)
                return true;
            return td.GetFrozenColumn() == string.Empty;
        }

        /// <override/>
        /// <summary>Returns the number of frozen columns.</summary>
        /// <returns>Frozen column count.</returns>
        public override int InternalGetFrozenCols()
        {
            if (TableDescriptor != null && TableDescriptor.GetFrozenColumn() != string.Empty)
            {
                return TableDescriptor.GetColumnIndentCount() + TableDescriptor.GetFrozenColumnCount() - 1;
            }

            return base.InternalGetFrozenCols();
        }

        /// <override/>
        /// <summary>Returns the number of frozen rows.</summary>
        /// <returns>Frozen row count.</returns>
        public override int InternalGetFrozenRows()
        {
            if (!(this is GridNestedTableControl) && Model.HasTable && Model.Table.TopLevelGroup != null && frozenRowCountVersion != Model.Table.Engine.Version)
            {
                int frozen = 0;
                if (Table.TopLevelGroup.IsExpanded)
                {
                    foreach (Element el in Table.DisplayElements)
                    {
                        ////TraceUtil.TraceCurrentMethodInfo(el, el.GetVisibleCount(), el.ParentGroup.IsChildVisible(el.ParentSection));
                        if (el is NestedTable || el.GroupLevel > 0 || el.TableLevel > 0 || el.Kind == DisplayElementKind.Record)
                        {
                            frozen--;  //// Adjust for (frozen below header ...)
                            break;
                        }

                        frozen++;
                    }
                }

                ////TraceUtil.TraceCurrentMethodInfo(frozen, frozenRowCount);
                frozenRowCountVersion = Model.Table.Engine.Version;
                if (frozen != frozenRowCount)
                {
                    frozenRowCount = frozen;
                    this.InternalSetTopRow(frozenRowCount + 1);
                    ViewLayout.Reset();
                    FixTopLeftRowCol();
                }
            }

            return frozenRowCount;
        }

        void InitializeFrozenRowCount()
        {
        }

        void FixTopLeftRowCol()
        {
            int nfr = Math.Min(Model.RowCount, this.InternalGetFrozenRows()) + 1;
            int nfc = Math.Min(Model.ColCount, this.InternalGetFrozenCols()) + 1;
            if (this.TopRowIndex > this.Model.RowCount || this.TopRowIndex < nfr)
            {
                this.InternalSetTopRow(nfr);
                this.synchronizeGridShouldInvalidate |= !(this.TopRowIndex > this.Model.RowCount || this.TopRowIndex < nfr);
                ViewLayout.Reset();
            }

            if (this.LeftColIndex > this.Model.ColCount || this.LeftColIndex < nfc)
            {
                this.InternalSetLeftCol(nfc);
                this.synchronizeGridShouldInvalidate |= !(this.LeftColIndex > this.Model.ColCount || this.LeftColIndex < nfc);
                ViewLayout.Reset();
            }
        }

        /// <override/>
        /// <summary>Updates the scrollbars with current scroll position and scroll range.</summary>
        public override void UpdateScrollBars()
        {
            if (inUpdateScrollBars)
            {
                return;
            }

            if (!initialized || Updating)
            {
                return;
            }

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);

            this.InternalSetVScrollPixel(Table.Engine.SupportsYAmount && Table.TableOptions.VerticalPixelScroll);

            inUpdateScrollBars = true;
            try
            {
                InitializeFrozenRowCount();
                Model.UpdateColumnWidths();
                base.UpdateScrollBars();
            }
            finally
            {
                inUpdateScrollBars = false;
            }
        }

        /// <overload>
        /// Scrolls an element into view.
        /// </overload>
        /// <summary>
        /// Scrolls an element into view.
        /// </summary>
        /// <param name="el">The element to scroll into view.</param>
        /// <param name="reason">Scroll reason</param>
        /// <returns>True if scrolled; False if element was already visible or not valid.</returns>
        public bool ScrollInView(Element el, GridScrollCurrentCellReason reason)
        {
            GridTableControl tableControl = this.GetTableControlWindow();
            int rowIndex = tableControl.Table.NestedDisplayElements.IndexOf(el);
            if (rowIndex != -1)
            {
                return tableControl.ScrollCellInView(GridRangeInfo.Row(rowIndex), reason);
            }

            return false;
        }

        /// <summary>
        /// Scrolls an element into view.
        /// </summary>
        /// <param name="el">The element to scroll into view.</param>
        /// <returns>True if scrolled; False if element was already visible or not valid.</returns>
        public bool ScrollInView(Element el)
        {
            return ScrollInView(el, GridScrollCurrentCellReason.Any);
        }

        bool allowScrollCaptionHeaderInView = false;

        /// <override/>
        protected override void OnTopRowChanged(GridRowColIndexChangedEventArgs e)
        {
            int index = TopRowIndex;

            if (!VScrollBar.IsThumbTracking && index < e.SavedValue && allowScrollCaptionHeaderInView)
            {
                if (index > this.InternalGetFrozenRows() && index < Table.DisplayElements.Count)
                {
                    int newIndex = index;
                    Element el = Model.GetDisplayElementAt(newIndex);
                    if (Record.GetParentRecord(el) != null)
                    {
                        Element cs = Model.GetDisplayElementAt(newIndex - 1);
                        if (CaptionSection.IsCaption(cs) && cs.GroupLevel == el.GroupLevel)
                        {
                            newIndex--;
                        }

                        cs = Model.GetDisplayElementAt(newIndex - 1);
                        while (CaptionSection.IsCaption(cs) && cs.GroupLevel == el.GroupLevel - 1)
                        {
                            newIndex--;
                            cs = Model.GetDisplayElementAt(newIndex - 1);
                        }
                    }

                    if (newIndex != index)
                    {
                        TopRowIndex = newIndex;
                    }
                }
            }

            base.OnTopRowChanged(e);
        }

        /// <override/>
        protected override void OnLeftColChanged(GridRowColIndexChangedEventArgs e)
        {
            if (!HScrollBar.IsThumbTracking && HasTable)
            {
                int index = LeftColIndex;

                if (index < e.SavedValue && !this.HScrollPixel)
                {
                    if (index <= this.TableDescriptor.GroupedColumns.Count + 1 && index > 1)
                    {
                        LeftColIndex = 1;
                    }
                }
            }

            base.OnLeftColChanged(e);
        }

        #endregion
        #region ScrollBar Metrics
        /// <override/>
        /// <summary>Gets the row height for the given range of rows.</summary>
        /// <param name="fromRowIndex">The first row.</param>
        /// <param name="toRowIndex">The last row.</param>
        /// <param name="maxSize">Abort calculation if height is greater than this value.</param>
        /// <returns>Returns the row height.</returns>
        public override int GetRowRangeHeight(int fromRowIndex, int toRowIndex, int maxSize)
        {
            return Table.GetRecordHeightTotal(Model.FilteredChildTable, fromRowIndex, toRowIndex);
        }

        /// <override/>
        /// <summary>Gets the smallest value to be used as TopRowIndex.</summary>
        /// <param name="rectMove">Returns the rectangle with scroll bounds for the grid.</param>
        /// <returns>Maximum top row index.</returns>
        public override int GetMaximumPossibleTopRow(out Rectangle rectMove)
        {
            if (!SupportsYAmount)
            {
                return base.GetMaximumPossibleTopRow(out rectMove);
            }

            int nTopRow = TopRowIndex;
            int nfr = this.InternalGetFrozenRows();
            rectMove = this.ViewLayout.RectangleBottomOfRow(nfr + 1, GridCellSizeKind.VisibleSize);
            if (cachedLastRow != -1 && cachedLastRowCount == Model.RowCount)
            {
                return cachedLastRow;
            }

            double total = this.Table.DisplayElements.YAmountCount;
            double find = Math.Max(0, total - rectMove.Height);
            Element yAmountCount = Table.NestedDisplayElements.GetItemAtYAmount(find);

            if (yAmountCount == null)
            {
                Debugger.Break();
                yAmountCount = Table.NestedDisplayElements.GetItemAtYAmount(find);
                Table.InvalidateCounterTopDown(true);

                double total2 = this.Table.DisplayElements.YAmountCount;
                double find2 = Math.Max(0, total2 - rectMove.Height);
                Element displayMetrics2 = Table.NestedDisplayElements.GetItemAtYAmount(find);
                TraceUtil.TraceCurrentMethodInfo(displayMetrics2);
            }

            int lastRow = Math.Max(nfr, Table.NestedDisplayElements.IndexOf(yAmountCount) + 1);
            cachedLastRow = lastRow;
            cachedLastRowCount = Model.RowCount;
            lastRow = Math.Min(lastRow, Table.NestedDisplayElements.Count - 1);
            return lastRow;
        }

        /// <override/>
        /// <summary>
        /// Calculates the vertical scroll pixel position for the given row.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns>Vertical scroll pixel position.</returns>
        public override int RowIndexToVScrollPixelPos(int rowIndex)
        {
            if (!SupportsYAmount)
            {
                throw new NotSupportedException("YAmountCount is disabled.");
            }

            if (Table == null)
            {
                return VScrollBar.Minimum;
            }

            if (rowIndex >= Table.DisplayElements.Count)
            {
                return (int)Table.NestedDisplayElements.YAmountCount;
            }

            Element el = Table.NestedDisplayElements[rowIndex];
            double yAmountCount = Table.NestedDisplayElements.GetYAmountPositionOf(el);
            return (int)yAmountCount;
        }

        /// <override/>
        /// <summary>Gets the total height of all rows in the grid.</summary>
        /// <returns>Total row height.</returns>
        public override int GetVScrollPixelHeight()
        {
            if (!SupportsYAmount)
            {
                throw new NotSupportedException("YAmountCount is disabled.");
            }

            return (int)(Table != null ? ((Table.Records.Count == 0 || Table.Records.Count > 1)? Table.DisplayElements.YAmountCount + 1 : Table.DisplayElements.YAmountCount - 1) : 0);
        }

        /// <override/>
        /// <summary>
        /// Determines the row index that is located at the specified vertical pixel scroll position.
        /// </summary>
        /// <param name="pixelPos">The absolute vertical pixel scroll position.</param>
        /// <param name="rowIndex">Returns the resultant row index.</param>
        /// <param name="pixelDelta">Returns the number of pixels the top row is above the view area for the scroll position.</param>
        public override void VScrollPixelPosToRowIndex(int pixelPos, out int rowIndex, out int pixelDelta)
        {
            if (!SupportsYAmount)
            {
                throw new NotSupportedException("YAmountCount is disabled.");
            }

            Element el = Table.NestedDisplayElements.GetItemAtYAmount(pixelPos);
            rowIndex = Table.NestedDisplayElements.IndexOf(el);
            pixelDelta = pixelPos - (int)Table.NestedDisplayElements.GetYAmountPositionOf(el);
        }

        /// <override/>
        /// <summary>Gets the total width of the grid for pixel scrolling.</summary>
        /// <returns>Total grid width.</returns>
        public override int GetHScrollPixelWidth()
        {
            if (maximumWidth >= 0)
            {
                return maximumWidth;
            }

            return base.GetHScrollPixelWidth();
        }

        internal int GetHorizontalScrollWidth()
        {
            int width = Table.GetHorizontalScrollWidth();
            return width;
        }
        #endregion
        #region Error Marker and Selected Records

        /// <override/>
        protected override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            base.OnPrepareViewStyleInfo(e);

            GridTableCellStyleInfo style = (GridTableCellStyleInfo)e.Style;
            if (e.RowIndex.Equals(this._rowIndex) && e.ColIndex.Equals(this._colIndex))
            {
                if ((((style.TableCellIdentity.TableCellType == GridTableCellType.ColumnHeaderCell
                    || style.TableCellIdentity.TableCellType == GridTableCellType.StackedHeaderCell)
                    && HitMouseDoubleClick)
                    || (style.TableCellIdentity.TableCellType == GridTableCellType.StackedHeaderCell
                    && HitMouseClick)))
                {
                    style.CellAppearance = GridCellAppearance.Sunken;
                }
            }

            if (e.Cancel || style.TableCellIdentity == null)
            {
                return;
            }

            Element el = style.TableCellIdentity.DisplayElement;
            Record r = Record.GetParentRecord(el);
            if (r != null)
            {
                if (Table.CurrentElement == r)
                {
                    if (e.ColIndex > el.GroupLevel || (Model.IsTopAddNewRecord(el.ParentElement) && e.ColIndex > 0))
                    {
                        CurrentRecordProperty prop = this.GetCurrentRecordManagerPropertyAt(e.RowIndex, e.ColIndex);

                        if (prop != null && prop.IsError)
                        {
                            e.Style.CellTipText = prop.Exception.Message;
                            e.Style.TextMargins.Right += 15;  // for error icon
                        }
                    }
                }

                if (r.IsSelected())
                {
                    Table.Engine.TableControl.ProcessSelectedRecordPrepareViewStyleInfo(this, e);
                }
            }
        }

        /// <override/>
        protected override void OnCellDrawn(GridDrawCellEventArgs e)
        {
            base.OnCellDrawn(e);

            GridTableCellStyleInfo style = (GridTableCellStyleInfo)e.Style;

            if (e.Cancel || style.TableCellIdentity == null)
            {
                return;
            }

            Element el = style.TableCellIdentity.DisplayElement;
            GridTableCellType ct = style.TableCellIdentity.TableCellType;
            if (ct == GridTableCellType.RecordFieldCell || ct == GridTableCellType.AlternateRecordFieldCell || ct == GridTableCellType.AddNewRecordFieldCell || ct == GridTableCellType.GroupCaptionCell)
            {
                if (Table.IsCurrentRecord(e.RowIndex) && this.CurrentCell.ShowErrorIcon)
                {
                    CurrentRecordProperty prop = this.GetCurrentRecordManagerPropertyAt(e.RowIndex, e.ColIndex);

                    if (prop != null && prop.IsError)
                    {
                        //// Highlight error column.
                        Brush br = new SolidBrush(Color.FromArgb(64, Color.Red));
                        e.Graphics.FillRectangle(br, e.Bounds);
                        br.Dispose();

                        try
                        {
                            Rectangle iconBounds = Rectangle.FromLTRB(e.Bounds.Right - 15, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
                            iconBounds.Offset(-2 - e.Style.CellModel.ButtonBarSize.Width, 0);
                            GridGroupingBitmaps.IconPainter.PaintIcon(e.Graphics, iconBounds, Point.Empty, "SFERROR.BMP", Color.Red);
                        }
                        catch
                        {
                        }
                    }

                    /* else
                    // {
                    //  GridColumnDescriptor cd = Model.GetColumnDescriptorAt(e.RowIndex, e.ColIndex);
                    //   if (cd != null)
                    //  {
                    // Brush br = new SolidBrush(Color.FromArgb(64, SystemColors.Highlight));
                    // e.Graphics.FillRectangle(br, e.Bounds);
                    //                                        br.Dispose();
                    //                                                                                                                  }
                    //                                                                                               }*/
                }

                /*                                                                            else if (Model.Options.ListBoxSelectionMode == SelectionMode.None &&
                //                                                                                               Model.Options.AllowSelection == GridSelectionFlags.None &&
                //                                                                                               Table.CurrentElement != null && CurrentCell.HasCurrentCellAt(e.RowIndex))
                //                                                                            {
                //                                                                                               CaptionRow cs = el as  CaptionRow;
                //                                                                                               if (cs != null && cs.ParentTable == this.Table && e.ColIndex >= Model.GetCaptionColIndex(cs))
                //                                                                                               {
                //                                                                                                                  Brush br = new SolidBrush(Color.FromArgb(64, SystemColors.Highlight));
                //                                                                                                                  e.Graphics.FillRectangle(br, e.Bounds);
                //                                                                                                                  br.Dispose();
                //                                                                                               }
                                                                                          }*/
                Record r = Record.GetParentRecord(el);
                if (r != null && r.IsSelected())
                {
                    Table.Engine.TableControl.ProcessSelectedRecordCellDrawn(this, e);
                }
            }
        }
        #endregion
        #region GridTable and Changed Events
        /// <summary>
        /// The <see cref="GridTable"/> that this control is showing.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTable Table
        {
            get
            {
                return Model.Table;
            }

            set
            {
                Model.Table = value;
            }
        }

        /// <summary>
        /// Gets whether the <see cref="Table"/> property has been assigned.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool HasTable
        {
            get
            {
                return Model.HasTable;
            }
        }

        /// <summary>
        /// The <see cref="GridTableDescriptor"/> that this control is showing.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public GridTableDescriptor TableDescriptor
        {
            get
            {
                if (Table != null)
                {
                    return Table.TableDescriptor;
                }

                return null;
            }
        }

        private void Model_TableChanged(object sender, EventArgs e)
        {
            ////WireTable(Table);
            ////_TableDescriptor = TableDescriptor;
        }

        private void Model_TableChanging(object sender, EventArgs e)
        {
            ////UnwireTable(Table);
        }

        /*                                      private GridTableDescriptor tableDescriptor;
        //
        //                                      GridTableDescriptor _TableDescriptor
        //                                      {
        //                                                         get
        //                                                         {
        //                                                                            return this.tableDescriptor;
        //                                                         }
        //                                                         set
        //                                                         {
        //                                                                            if (this.tableDescriptor != value)
        //                                                                            {
        //                                                                                               //UnwireTableDescriptor(tableDescriptor);
        //                                                                                               this.tableDescriptor = value;
        //                                                                                               //WireTableDescriptor(value);
        //                                                                            }
        //                                                         }
        //                                      }

        //                                      void WireTableDescriptor(GridTableDescriptor td)
        //                                      {
        //                                                         if (td == null)
        //                                                                            return;
        //                                                         td.PropertyChanged += new DescriptorPropertyChangedEventHandler(td_PropertyChanged);
        //                                                         td.Engine.PropertyChanged += new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
        //                                                         td.RecordFilters.Changing += new ListPropertyChangedEventHandler(RecordFilters_Changing);
        //                                                         td.Columns.Changed += new ListPropertyChangedEventHandler(Columns_Changed);
        //                                                         td.ColumnSets.Changed += new ListPropertyChangedEventHandler(ColumnSets_Changed);
        //                                                         td.VisibleColumns.Changed += new ListPropertyChangedEventHandler(VisibleColumns_Changed);
        //                                                         td.ExpressionFields.Changed += new ListPropertyChangedEventHandler(ExpressionFields_Changed);
        //                                                         td.Fields.Changed += new ListPropertyChangedEventHandler(Fields_Changed);
        //                                      }
        //
        //                                      void UnwireTableDescriptor(GridTableDescriptor td)
        //                                      {
        //                                                         if (td == null)
        //                                                                            return;
        //                                                         td.PropertyChanged -= new DescriptorPropertyChangedEventHandler(td_PropertyChanged);
        //                                                         td.Engine.PropertyChanged -= new DescriptorPropertyChangedEventHandler(Engine_PropertyChanged);
        //                                                         td.RecordFilters.Changing -= new ListPropertyChangedEventHandler(RecordFilters_Changing);
        //                                                         td.Columns.Changed -= new ListPropertyChangedEventHandler(Columns_Changed);
        //                                                         td.ColumnSets.Changed -= new ListPropertyChangedEventHandler(ColumnSets_Changed);
        //                                                         td.VisibleColumns.Changed -= new ListPropertyChangedEventHandler(VisibleColumns_Changed);
        //                                                         td.ExpressionFields.Changed -= new ListPropertyChangedEventHandler(ExpressionFields_Changed);
        //                                                         td.Fields.Changed -= new ListPropertyChangedEventHandler(Fields_Changed);
                                            }*/

        void WireTable(GridTableDescriptor table)
        {
            /*                                                         TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose|Switches.CurCellNestedGrid.TraceVerbose);
            //                                                         if (table != null)
            //                                                         {
            //                                                                            table.CurrentRecordContextChange += new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
            //                                                                            table.DisplayElementChanging += new DisplayElementChangingEventHandler(Table_DisplayElementChanging);
            //                                                                            table.DisplayElementChanged += new DisplayElementChangedEventHandler(Table_DisplayElementChanged);
            //                                                                            table.TableSourceListChanged += new TableEventHandler(Table_SourceListChanged);
            //                                                                            table.CurrentRecordManagerReset += new EventHandler(Table_CurrentRecordManagerReset);
            //                                                                            table.SourceListRecordChanged += new RecordChangedEventHandler(Table_SourceListRecordChanged);
            //                                                                            table.SourceListRecordChanging += new RecordChangedEventHandler(Table_SourceListRecordChanging);
            //                                                                            table.SourceListListChanged += new TableListChangedEventHandler(Table_SourceListListChanged);
            //                                                                            table.RecordValueChanged += new RecordValueChangedEventHandler(Table_RecordValueChanged);
            //
            //                                                         }*/
        }

        void UnwireTable(GridTableDescriptor table)
        {
            /*                                                         TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose|Switches.CurCellNestedGrid.TraceVerbose);
            //                                                         if (table != null)
            //                                                         {
            //                                                                            table.CurrentRecordContextChange -= new CurrentRecordContextChangeEventHandler(Table_CurrentRecordContextChange);
            //                                                                            table.DisplayElementChanging -= new DisplayElementChangingEventHandler(Table_DisplayElementChanging);
            //                                                                            table.DisplayElementChanged -= new DisplayElementChangedEventHandler(Table_DisplayElementChanged);
            //                                                                            table.TableSourceListChanged -= new TableEventHandler(Table_SourceListChanged);
            //                                                                            table.CurrentRecordManagerReset -= new EventHandler(Table_CurrentRecordManagerReset);
            //                                                                            table.SourceListRecordChanged -= new RecordChangedEventHandler(Table_SourceListRecordChanged);
            //                                                                            table.SourceListRecordChanging -= new RecordChangedEventHandler(Table_SourceListRecordChanging);
            //                                                                            table.SourceListListChanged -= new TableListChangedEventHandler(Table_SourceListListChanged);
            //                                                                            table.RecordValueChanged -= new GridRecordValueChangedEventHandler(Table_RecordValueChanged);
                                                                   }*/
        }
        #endregion
        #region Table Event Handler
        internal Record synchronizeGridNavigateToBookmark;
        DisplayElementKind synchronizeGridNavigateToKind;

        /// <internalonly/>
        /// <summary>
        /// Triggers when the displayelement tends to change.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_DisplayElementChanging(object sender, DisplayElementChangingEventArgs e)
        {
            if (inTable_DisplayElementChanging)
            {
                return;
            }

            ////throw new InvalidOperationException();
            inTable_DisplayElementChanging = true;

            if (e.SyncCurrentRecordPos && Table.CurrentElement != null)
            {
                SaveCurrentCellStateBeforeSorting();
                synchronizeGridNavigateTo = Table.CurrentElement;
                synchronizeGridNavigateTo.Disposed += new EventHandler(synchronizeGridNavigateTo_Disposed);
                synchronizeGridNavigateToKind = synchronizeGridNavigateTo.Kind;
                if (synchronizeGridNavigateTo.GroupLevel > 0)
                {
                    if (synchronizeGridNavigateToBookmark == null)
                    {
                        synchronizeGridNavigateToBookmark = synchronizeGridNavigateTo.ParentGroup.GetFirstRecord();
                    }

                    if (synchronizeGridNavigateToBookmark != null)
                    {
                        synchronizeGridNavigateToBookmark.Disposed += new EventHandler(synchronizeGridNavigateToBookmark_Disposed);
                    }
                }
                else
                {
                    this.ResetSynchronizeGridNavigateToBookmark();
                }

                synchronizeGridShouldScrollCurrentCell |= e.ScrollCurrentRecordInView;
                synchronizeGridShouldRestoreCurrentCell = true;
            }

            if (e.LeaveCurrentRecord || e.SyncCurrentRecordPos)
            {
                e.Cancel |= DeactivateCurrentCell(e.AllowCancel);
            }

            inTable_DisplayElementChanging = false;
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers when the displayelement changes.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_DisplayElementChanged(object sender, DisplayElementChangedEventArgs e)
        {
            if (inRefresh)
            {
                return;
            }

            ////TraceUtil.TraceCurrentMethodInfo();
            GridTableControl tableControl = (GridTableControl)GetWindow();
            if (this.Table.Engine.InvalidateAllWhenListChanged && this.Table.InSourceListListChangedHandler)
            {
                synchronizeGridShouldInvalidate = true;
                synchronizeGridShouldnextUpdateScrollBars = true;
            }
            else if (!synchronizeGridShouldInvalidate)
            {
                if (e.NewCount == e.OldCount && e.Element != null && !(e.Element is Table))
                {
                    Rectangle r = RangeInfoToRectangle(tableControl.Table.GetElementRangeInfo(e.Element));
                    if (/*e.Element == null || */r.Height > GridBounds.Height && r.IntersectsWith(GridBounds))
                    {
                        synchronizeGridShouldInvalidate = true;
                    }
                    else
                    {
                        tableControl.Invalidate(r);
                    }
                }
                else
                {
                    synchronizeGridShouldInvalidate = true;
                    synchronizeGridShouldnextUpdateScrollBars = true;
                }
            }

            if (e.SyncCurrentRecordPos)
            {
                synchronizeGridShouldScrollCurrentCell |= e.ScrollCurrentRecordInView;
                synchronizeGridShouldRestoreCurrentCell = true;
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers when the sourcelist changes.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_SourceListChanged(object sender, TableEventArgs e)
        {
            CurrentCell.ResetCurrentCellWithoutDeactivate();
            // Commented out: Calling CancelMode will end "Begin/EndUpdate" mode which
            // causes the grid to flicker if you change the data source.
            // this.OnCancelMode(EventArgs.Empty);
            this.Invalidate();
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers when the currentrecordmanager resets.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_CurrentRecordManagerReset(object sender, TableEventArgs e)
        {
            this.CurrentCell.ResetCurrentCellWithoutDeactivate();
            ResetSynchronizeGridNavigateTo();
            this.currentCellEditingState = null;
            this.synchronizeGridShouldRestoreCurrentCell = true;
            synchronizeGridNavigateToBookmark = null;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_CurrentRecordContextChange(object sender, CurrentRecordContextChangeEventArgs e)
        {
            if (inCurrentRecordContextChange || !e.Success || e.Cancel)
            {
                CurrentCell.Activate(CurrentCell.RowIndex, CurrentCell.ColIndex);
                return;
            }

            inCurrentRecordContextChange = true;

            ////TraceUtil.TraceCurrentMethodInfo(e);
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    e,
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                     inProcessKeyEventArgs,
                     "Updating",
                     Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(4);
            }
#endif
            try
            {
                switch (e.Action)
                {
                    case CurrentRecordAction.BeginEditCalled:
                        Table.ClearCollectionCaches();
                        if (true)
                        {
                            //// e.Cancel = ...
                        }

                        break;
                    case CurrentRecordAction.BeginEditComplete:
                        Table.ClearCollectionCaches();
                        if (!this.GetGridWindow().IgnoreCurrentCellInvalidate)
                        {
                            InvalidateCurrentRecord();
                        }

                        break;
                    case CurrentRecordAction.CancelEditCalled:
                        Table.ClearCollectionCaches();
                        if (!inControlCancelEdit)
                        {
                            e.Cancel = !ControlCancelEdit();
                        }

                        break;
                    case CurrentRecordAction.CancelEditComplete:
                        Table.ClearCollectionCaches();
                        if (!this.GetGridWindow().IgnoreCurrentCellInvalidate)
                        {
                            InvalidateCurrentRecord();
                        }

                        break;
                    case CurrentRecordAction.EndEditCalled:
                        Table.ClearCollectionCaches();
                        if (!inControlEndEdit)
                        {
                            e.Cancel = !ControlEndEdit(false);
                        }

                        break;
                    case CurrentRecordAction.EndEditComplete:
                        Table.ClearCollectionCaches();
                        if (!this.GetGridWindow().IgnoreCurrentCellInvalidate)
                        {
                            if (!this.inControlEndEdit)
                            {
                                InvalidateCurrentRecord();
                            }
                        }

                        if (Table.CurrentRecord is AddNewRecord)
                        {
                            OnRecordAddedEndEditComplete();
                        }

                        break;
                    case CurrentRecordAction.NavigateCalled:
                        break;
                    case CurrentRecordAction.NavigateComplete:
                        if (!CurrentCell.IsInMoveTo && !GetNestedCurrentCell().IsInMoveTo && Table.CurrentRecordManager.ShouldScrollInView())
                        {
                            GetNestedCurrentCell().ScrollInView(GridScrollCurrentCellReason.MoveTo);
                        }

                        break;
                    case CurrentRecordAction.LeaveRecordCalled:
                        if (true)
                        {
                            e.Cancel = !(CurrentCell.IsLocked || CurrentCell.ConfirmChanges(true));
                            if (!e.Cancel)
                            {
                                if (CurrentCell.HasCurrentCell && !inControlLeaveRecord)
                                {
                                    //// BeginUpdate(BeginUpdateOptions.InvalidateAndScroll|BeginUpdateOptions.SynchronizeScrollBars);
                                    CurrentCell.Deactivate(true);
                                    //// EndUpdate(false);
                                }
                            }
                        }

                        break;
                    case CurrentRecordAction.LeaveRecordComplete:
                        if (e.Success)
                        {
                            Record r = e.Record as Record;
                            if (!this.GetGridWindow().IgnoreCurrentCellInvalidate)
                            {
                                if (r != null)
                                {
                                    GridTableControl mainControl = (GridTableControl)GetGridWindow();
                                    //// int rowIndex1 = mainControl.Table.NestedDisplayElements.IndexOf(r);
                                    //// int rowIndex2 = rowIndex1 + r.RecordRows.Count;
                                    //// if (mainControl.ViewLayout.IsRowVisible(rowIndex1) || mainControl.ViewLayout.IsRowVisible(rowIndex2))
                                    //// mainControl.InvalidateRange(GridRangeInfo.Rows(rowIndex1, rowIndex2));
                                    mainControl.InvalidateElement(r.GetRecordDisplayElement());
                                }
                            }
                        }

                        break;
                    case CurrentRecordAction.EnterRecordCalled:
                        break;
                    case CurrentRecordAction.CurrentFieldChanged:
                    case CurrentRecordAction.EnterRecordComplete:
                        if (e.Success)
                        {
                            Record r = e.Record as Record;
                            if (!inControlEnterRecord)
                            {
                                SplitterControl parent = this.Parent as SplitterControl;
                                bool scroll = parent != null && parent.ActivePane == this;
                                int rowIndex = Model.GetDisplayElementIndexOf(e.Record);
                                if (rowIndex < 0)
                                {
                                    Table.Engine.BumpVersion();
                                    rowIndex = Model.GetDisplayElementIndexOf(e.Record);
                                }

                                if (rowIndex == -1)
                                {
                                    return;
                                }

                                int colIndex = Model.GetCaptionColIndex(e.Record);
                                if (r != null)
                                {
                                    colIndex = Model.GetColumnIndentCount();
                                }

                                colIndex = Math.Max(CurrentCell.ColIndex, colIndex);
                                if (r != null)
                                {
                                    FieldDescriptor field = e.Record.ParentTable.CurrentRecordManager.CurrentField;
                                    if (field != null)
                                    {
                                        GridRangeInfo rg = Model.RecordFieldToRangeInfo(Table.CurrentRecordManager.CurrentRecord, Table.CurrentRecordManager.CurrentField);
                                        colIndex = Math.Max(Model.GetColumnIndentCount(), rg.Left);
                                        rowIndex = rg.Top;
                                    }
                                }

                                CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                                ////TraceUtil.TraceCurrentMethodInfo(e.Record, rowIndex, colIndex);
                                if (!CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                                {
                                    ////Model.Options.ShowCurrentCellBorderBehavior == GridShowCurrentCellBorder.AlwaysVisible
                                    ////|| Model.Options.ShowCurrentCellBorderBehavior == GridShowCurrentCellBorder.GrayWhenLostFocus
                                    ////|| Model.Options.ShowCurrentCellBorderBehavior == GridShowCurrentCellBorder.WhenGridActive
                                    //// && this.IsActiveControl)
                                    if (Table.TableOptions.ListBoxSelectionCurrentCellOptions != GridListBoxSelectionCurrentCellOptions.HideCurrentCell || !Element.IsRecord(r))
                                    {
                                        /* NestedTable*/
                                        while (colIndex > 0 && !Model[rowIndex, colIndex].Enabled)
                                        {
                                            colIndex--;
                                        }

                                        if (colIndex > 0)
                                        {
                                            // Added CurrentRecordManager.RestoreOldCurrentField() method in 4.1.0.31
                                            // wich lets me fix the current field in case CurrentCell.Move failed in GridTableControl.
                                            bool succ = CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.NoSyncCurrentCell);
                                            if (succ || e.Action != CurrentRecordAction.CurrentFieldChanged)
                                            {
                                                if (scroll && !(CurrentCell.Renderer is GridNestedTableControlCellRenderer) && Table.CurrentRecordManager.ShouldScrollInView())
                                                {
                                                    CurrentCell.ScrollInView(GridScrollCurrentCellReason.SynchronizeRecord);
                                                }
                                            }
                                            else
                                            {
                                                Table.CurrentRecordManager.RestoreOldCurrentField();
                                            }
                                        }
                                    }
                                }
                            }

                            if (!this.GetGridWindow().IgnoreCurrentCellInvalidate)
                            {
                                if (r != null)
                                {
                                    GridTableControl mainControl = (GridTableControl)GetGridWindow();
                                    //// int rowIndex1 = mainControl.Table.NestedDisplayElements.IndexOf(r);
                                    //// int rowIndex2 = rowIndex1 + r.RecordRows.Count;
                                    //// if (mainControl.ViewLayout.IsRowVisible(rowIndex1) || mainControl.ViewLayout.IsRowVisible(rowIndex2))
                                    //// mainControl.InvalidateRange(GridRangeInfo.Rows(rowIndex1, rowIndex2));
                                    mainControl.InvalidateElement(r.GetRecordDisplayElement());
                                }
                            }
                        }

                        break;
                }
            }
            finally
            {
                inCurrentRecordContextChange = false;
            }
        }

        /// <override/>
        protected override void InvalidateDeactivatedCurrentCell(int rowIndex, int colIndex, Rectangle savedBounds)
        {
            if (rowIndex >= 0 && rowIndex < Table.DisplayElements.Count && !(Table.DisplayElements[rowIndex] is NestedTable))
            {
                base.InvalidateDeactivatedCurrentCell(rowIndex, colIndex, savedBounds);
            }
        }

        //// Element leaveRecordElement;
        #endregion
        #region ControlEdit
        bool ControlCancelEdit()
        {
            Debug.Assert(!inControlCancelEdit);

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inControlCancelEdit = true;
            try
            {
                if (CurrentCell.IsModified)
                {
                    //// BeginUpdate(BeginUpdateOptions.InvalidateAndScroll|BeginUpdateOptions.SynchronizeScrollBars);
                    CurrentCell.RejectChanges();
                    //// EndUpdate(false);
                }

                if (CurrentCell.IsModified)
                {
                    return false;
                }

                if (inCurrentRecordContextChange)
                {
                    //// If I don't call ResetCachedState and call CurrentCell.Refresh()
                    //// current cell will stay empty after pressing Escape when previously a message box
                    //// was displayed.
                    Table.CurrentRecordManager.ResetCachedState();

                    if (CurrentCell.HasCurrentCell)
                    {
                        CurrentCell.Refresh();
                    }

                    return true;
                }

                Table.CurrentRecordManager.CancelEdit();

                return !Table.CurrentRecordManager.IsEditing;
            }
            finally
            {
                inControlCancelEdit = false;
            }
        }

        internal bool ControlEndEdit(bool synchronizeCurrentCellAfterEndEditFailed)
        {
            Debug.Assert(!inControlEndEdit);

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inControlEndEdit = true;
            try
            {
                if (inOnCurrentCellAcceptedChanges)
                {
                    Debug.Assert(!CurrentCell.IsModified);
                }

                if (CurrentCell.IsModified)
                {
                    //// BeginUpdate(BeginUpdateOptions.InvalidateAndScroll|BeginUpdateOptions.SynchronizeScrollBars);
                    CurrentCell.ConfirmChanges(true);
                    //// EndUpdate(false);
                }

                if (CurrentCell.IsModified)
                {
                    return false;
                }

                if (HasTable && Table.CurrentElement is NestedTable)
                {
                    NestedTable nestedTable = (NestedTable)Table.CurrentElement;
                    if (nestedTable.ChildTable != null)
                    {
                        Table relatedTable = nestedTable.ChildTable.ParentTable;
                        string cellType = "RT" + relatedTable.TableDescriptor.Name;
                        GridNestedTableControlCellRenderer nestedTableRenderer = this.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
                        if (nestedTableRenderer != null && CurrentCell.HasCurrentCell)
                        {
                            nestedTableRenderer.OnControlEndEdit(synchronizeCurrentCellAfterEndEditFailed);
                            if (nestedTableRenderer.Control.Table.CurrentRecordManager.IsEditing)
                            {
                                if (synchronizeCurrentCellAfterEndEditFailed)
                                {
                                    SynchronizeCurrentCellAfterEndEditFailed();
                                }

                                return false;
                            }
                        }
                        else
                        {
                            nestedTable.ChildTable.ParentTable.CurrentRecordManager.EndEdit();
                            if (nestedTable.ChildTable.ParentTable.CurrentRecordManager.IsEditing)
                            {
                                if (synchronizeCurrentCellAfterEndEditFailed)
                                {
                                    SynchronizeCurrentCellAfterEndEditFailed();
                                }

                                return false;
                            }
                        }
                    }
                }

                CurrentCell.EndEdit();

                if (inCurrentRecordContextChange)
                {
                    return true;
                }

                Table.CurrentRecordManager.EndEdit();
                if (Table.CurrentRecordManager.IsEditing)
                {
                    if (synchronizeCurrentCellAfterEndEditFailed)
                    {
                        SynchronizeCurrentCellAfterEndEditFailed();
                    }

                    return false;
                }

                return true;
            }
            finally
            {
                inControlEndEdit = false;
            }
        }

        void OnRecordAddedEndEditComplete()
        {
        }

        bool ControlBeginEdit()
        {
            Debug.Assert(!inControlBeginEdit);

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inControlBeginEdit = true;
            try
            {
                if (!inCurrentRecordContextChange)
                {
                    Table.CurrentRecordManager.BeginEdit();
                    if (!Table.CurrentRecordManager.IsEditing)
                    {
                        return false;
                    }
                }

                return true;
                //// try
                //// {
                //// BeginUpdate(BeginUpdateOptions.InvalidateAndScroll|BeginUpdateOptions.SynchronizeScrollBars);
                //// return GetCurrentCell().BeginEdit(/*focus?*/);
                //// }
                //// finally
                //// {
                //// EndUpdate(false);
                //// }
            }
            finally
            {
                inControlBeginEdit = false;
            }
        }

        bool ControlLeaveRecord()
        {
            if (Table.CurrentRecordManager.InNavigate)
            {
                return true;
            }

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inControlLeaveRecord = true;
            try
            {
                if (!ControlEndEdit(false))
                {
                    return false;
                }

                if (HasTable && Table.CurrentElement is NestedTable)
                {
                    NestedTable nestedTable = (NestedTable)Table.CurrentElement;
                    if (nestedTable.ChildTable != null)
                    {
                        Table relatedTable = nestedTable.ChildTable.ParentTable;
                        relatedTable.CurrentRecordManager.LeaveRecord(false);
                        if (relatedTable.CurrentRecordManager.CurrentElement != null)
                        {
                            return false;
                        }
                    }
                }

                if (Table.CurrentRecordManager.HasCurrentElement)
                {
                    Table.CurrentRecordManager.LeaveRecord(false);
                }

                return Table.CurrentRecordManager.CurrentElement == null;
            }
            finally
            {
                inControlLeaveRecord = false;
            }
        }

        bool ControlEnterRecord(Element record)
        {
            if (Table.CurrentRecordManager.InNavigate)
            {
                return true;
            }

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            inControlEnterRecord = true;
            try
            {
                Element r = (Element)record;
                return Table.CurrentRecordManager.NavigateTo(r) == r;
            }
            finally
            {
                inControlEnterRecord = false;
            }
        }

        bool isInValidating;

        bool IsInValidating
        {
            get
            {
                return isInValidating;
            }

            set
            {
                isInValidating = value;
            }
        }

        /// <override/>
        protected override void OnValidating(CancelEventArgs e)
        {
            isInValidating = true;

            try
            {
                // Note (after v4.1.0.64): Moved HandleEnterKey call outside this method to OnGridValidating so that it is called
                // after the ScrollControl.Validating event is raised. ScrollControl.OnValidating will also
                // set IsValidating flag = true, you can check this flag then in CurrentCellXyz events.
                base.OnValidating(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                e.Cancel = true;
            }
            finally
            {
                isInValidating = false;
            }
        }

        /// <override/>
        protected override void OnGridValidating(CancelEventArgs e)
        {
            if (HasTable)
            {
                HandleEnterKey();
                e.Cancel |= Table.CurrentRecordManager.IsModified;
                if (this.CurrentCell.IsModified && !this.CurrentCell.IsValid)
                    e.Cancel = true;
            }

            base.OnGridValidating(e);
        }

        #endregion
        #region Current Cell Overrides
        /// <summary>
        /// Returns the GridCurrentCell object.
        /// </summary>
        /// <returns>
        /// The <see cref="P:Syncfusion.Windows.Forms.Grid.GridControlBase.CurrentCell"/>.
        /// </returns>
        /// <override/>
        protected override GridCurrentCell GetCurrentCell()
        {
            return GetNestedCurrentCell();
        }

        /// <override/>
        protected override void OnCurrentCellCloseDropDown(PopupClosedEventArgs e)
        {
            if (e.PopupCloseType == PopupCloseType.Done)
            {
                Element el = Model.GetDisplayElementAt(CurrentCell.RowIndex);
                if (el is FilterBarSection)
                {
                    //// int fieldNum = Model.ColIndexToField(CurrentCell.ColIndex);
                    //// if (fieldNum >= 0 && fieldNum < Table.Columns.Count)
                    //// {
                    //// }
                }
            }

            base.OnCurrentCellCloseDropDown(e);
        }
        bool StartEditCanceled = false;
        /// <override/>
        protected override void OnCurrentCellStartEditing(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            e.Cancel = this.groupingControl.BrowseOnly;
            if (e.Cancel)
                return;

            base.OnCurrentCellStartEditing(e);

            StartEditCanceled = e.Cancel;
            if (HasTable && this.IsCurrentElementAtCurrentCell())
            {
                CurrentRecordProperty prop = GetCurrentCellRecordProperty();
                if (prop != null && prop.IsModified)
                {
                    CurrentCell.IsModified = true;
                }
            }
        }

        /// <override/>
        protected override void OnCurrentCellControlGotFocus(ControlEventArgs e)
        {
            if (this.IsCurrentElementAtCurrentCell() && CurrentCell.IsEditing)
            {
                CurrentRecordProperty prop = GetCurrentCellRecordProperty();
                if (prop != null && prop.IsModified)
                {
                    CurrentCell.IsModified = true;
                }
            }
#if DEBUG

            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else

                                                         ;
#endif
            base.OnCurrentCellControlGotFocus(e);
        }

        /// <override/>
        protected override void OnControlGotFocus()
        {
            if (this.IsCurrentElementAtCurrentCell() && CurrentCell.IsEditing)
            {
                CurrentRecordProperty prop = GetCurrentCellRecordProperty();
                if (prop != null && prop.IsModified)
                {
                    CurrentCell.IsModified = true;
                }
            }
#if DEBUG

            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else

                                                         ;
#endif
            base.OnControlGotFocus();
        }

        /// <override/>
        protected override void OnCurrentCellChanging(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif
            base.OnCurrentCellChanging(e);

            if (!this.IsCurrentElementAtCurrentCell() || e.Cancel)
            {
                return;
            }

            if (!CanEditCell(CurrentCell.RowIndex, CurrentCell.ColIndex))
            {
                e.Cancel = true;
            }
            else if (!Table.CurrentRecordManager.IsEditing)
            {
                CurrentRecordProperty prop = GetCurrentCellRecordProperty();
                if (prop != null && prop.FieldDescriptor != null && !prop.FieldDescriptor.ForceImmediateSaveValue)
                {
                    e.Cancel = !ControlBeginEdit();
                }
            }
        }

        /// <override/>
        protected override void NotifyCurrentCellChanged()
        {
            //// catch the special case that user called EndEdit from RecordValueChanged event
            //// or some other event after CurrentCellChanging and before CurrentCellChanged.
            //// In such case the context of the current cell was changed and therefore the
            //// CurrentCell should not be marked modified anymore.
            Element innerMostCurrentElement = Table.GetInnerMostCurrentElement();
            if (innerMostCurrentElement != null)
            {
                if (!this.IsCurrentElementAtCurrentCell() || !Table.CurrentRecordManager.IsEditing)
                {
                    this.SynchronizeCurrentCellWithRecord(innerMostCurrentElement);
                    return;
                }
            }

            base.NotifyCurrentCellChanged();
        }

        /// <override/>
        protected override void OnCurrentCellChanged(EventArgs e)
        {
            base.OnCurrentCellChanged(e);

            if (!this.IsCurrentElementAtCurrentCell())
            {
                this.SynchronizeCurrentCellWithRecord(Table.GetInnerMostCurrentElement());
                return;
            }

            if (CurrentCell.IsModified && !Table.CurrentRecordManager.IsEditing)
            {
                ControlBeginEdit();
            }
        }

        /// <override/>
        protected override void OnCurrentCellRejectedChanges(EventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            base.OnCurrentCellRejectedChanges(e);

            if (inControlCancelEdit)
            {
                return;
            }

            if (!IsCurrentElementAtCurrentCell())
            {
                return;
            }

            CancelUpdate();
            Model.ResetVolatileData();
            GridTableCellStyleInfo style = Table.TableModel[CurrentCell.RowIndex, CurrentCell.ColIndex];
            if (style.TableCellIdentity.Column != null)
            {
                Table.CurrentRecordManager.ResetModifiedValue(style.TableCellIdentity.Column.FieldDescriptor);
            }

            if (!Table.CurrentRecordManager.IsAnyPropertyModified)
            {
                Table.CurrentRecordManager.CancelEdit();
                if (!Table.CurrentRecordManager.IsEditing)
                {
                    this.RefreshRange(Table.GetCurrentRecordRangeInfo());
                }
            }

            CurrentCell.Refresh();
        }

        /// <override/>
        protected override void OnCurrentCellAcceptedChanges(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            base.OnCurrentCellAcceptedChanges(e);

            if (this.inControlEndEdit)
            {
                return;
            }

            if (e.Cancel || !HasTable || (CurrentCell.IsInMoveTo && !ShouldNavigateTo(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
               || (!CurrentCell.IsInMoveTo && !IsCurrentElementAtCurrentCell()))
            {
                return;
            }

            inOnCurrentCellAcceptedChanges = true;
            GridCurrentCell gcc = CurrentCell;
            gcc.Lock();
            try
            {
                if (CurrentCell.IsInMoveTo && !Table.IsCurrentRecord(CurrentCell.MoveToRowIndex))
                {
                    Element r = Table.CurrentElement;
                    if (!ControlLeaveRecord())
                    {
                        e.Cancel = true;
                        InvalidateCurrentRecord();
                    }
                    else
                    {
                        if (r != null && r.IsDisposed)
                        {
                            this.MarkResync(false);
                        }
                    }
                }
            }
            finally
            {
                inOnCurrentCellAcceptedChanges = false;
                gcc.Unlock();
            }

            isDeactivated = true;
        }

        /// <override/>
        protected override void OnCurrentCellConfirmChangesFailed(EventArgs e)
        {
            CurrentRecordProperty prop = this.GetCurrentRecordManagerPropertyAt(CurrentCell.RowIndex, CurrentCell.ColIndex);

            if (prop != null && CurrentCell.Exception != null)
            {
                prop.Exception = CurrentCell.Exception;
            }

            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);
            base.OnCurrentCellConfirmChangesFailed(e);
        }

        /// <override/>
        protected override void OnCurrentCellActivateFailed(GridCurrentCellActivateFailedEventArgs e)
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);
            base.OnCurrentCellActivateFailed(e);
        }

        /// <override/>
        protected override void OnCurrentCellDeactivateFailed(EventArgs e)
        {
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);
            base.OnCurrentCellDeactivateFailed(e);

            if (!CurrentCell.IsModified && HasTable)
            {
                int fieldNum = Model.ColIndexToField(CurrentCell.ColIndex);
                if (fieldNum < Table.CurrentRecordManager.Properties.Count && Table.CurrentRecordManager.Properties[fieldNum].IsModified)
                {
                    CurrentCell.Renderer.ControlValue = Table.CurrentRecordManager.Properties[fieldNum].ModifiedValue;
                }
            }
        }

        /*                                                       /// <override/>
        //                                      protected override void OnCurrentCellConfirmChangesFailed(EventArgs e)
        //                                      {
        //                                                         TraceUtil.TraceCurrentMethodInfoIf(Switches.GridTableControl.TraceVerbose, this.PaneDesc);
        //                                                         base.OnCurrentCellConfirmChangesFailed(e);
        //
        //                                                         if (CurrentCell.IsInMoveTo && !HasRecordAtCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex)
        //                                                                            || !CurrentCell.IsInMoveTo && !IsCurrentElementAtCurrentCell())
        //                                                                            return;
        //
        ////                                                         if (!CurrentCell.IsInDeactivate && !this.IsInLeaveOrValidate)
        ////                                                                            this.RaiseValidateFailed(CurrentCell.ColIndex);
        //                                      }
        //
        //                                      /// <override/>
        //                                      protected override void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        //                                      {
        //                                                         TraceUtil.TraceCurrentMethodInfoIf(Switches.GridTableControl.TraceVerbose, PaneDesc, e);
        //                                                         base.OnCurrentCellMoveFailed(e);
        //
        //                                                         if (IsCurrentCellUnboundCell(CurrentCell.MoveToRowIndex, CurrentCell.MoveToColIndex))
        //                                                                            return;
        //
        ////                                                         this.RaiseValidateFailed(CurrentCell.ColIndex);
                                          }*/

        /// <override/>
        protected override void OnCurrentCellMoving(GridCurrentCellMovingEventArgs e)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    e,
                    CurrentCell,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            base.OnCurrentCellMoving(e);

            if (!HasTable)
            {
                return;
            }

            ////Table.Engine.BumpVersion();
            leaveRow = false;
            currentCellMoveToElement = Model.GetDisplayElementAt(e.RowIndex);

            ////TraceUtil.TraceCurrentMethodInfo(currentCellMoveToElement );
            if (!e.Cancel)
            {
                if (this.ShouldNavigateTo(currentCellMoveToElement) && !IsCurrent(currentCellMoveToElement))
                {
                    leaveRow = true;
                    ////e.Options |= GridSetCurrentCellOptions.BeginEndUpdate;
                    ////e.Options &= GridSetCurrentCellOptions.NoSetFocus|GridSetCurrentCellOptions.NoSelectRange; // All but NoSetFocus is cleared
                    BeginUpdate(BeginUpdateOptions.InvalidateAndScroll | BeginUpdateOptions.SynchronizeScrollBars);
                }

                if (CaptionSection.IsCaption(currentCellMoveToElement) && currentCellMoveToElement.ParentTable == Table)
                {
                    IGridGroupOptionsSource gso = currentCellMoveToElement as IGridGroupOptionsSource;
                    if (gso != null && !gso.GroupOptions.ShowCaptionSummaryCells)
                    {
                        e.ColIndex = Model.GetCaptionColIndex(Model.GetDisplayElementAt(e.RowIndex));
                    }
                }
            }
        }

        /// <override/>
        protected override void OnCurrentCellMoved(GridCurrentCellMovedEventArgs e)
        {
            currentCellMoveToElement = null;
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            base.OnCurrentCellMoved(e);
            if (!this.InSynchronizeCurrentCellWithRecord)
            {
                GridColumnDescriptor cd = Model.GetColumnDescriptorAt(CurrentCell.RangeInfo);
                if (cd != null)
                {
                    Table.CurrentRecordManager.CurrentField = cd.FieldDescriptor;
                }
            }

            if (leaveRow)
            {
                EndUpdate(false);
            }
        }

        /// <override/>
        protected override void OnCurrentCellMoveFailed(GridCurrentCellMoveFailedEventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.PaneDesc);
            }
#else
                                                         ;
#endif

            currentCellMoveToElement = null;
            base.OnCurrentCellMoveFailed(e);
            if (leaveRow)
            {
                EndUpdate(false);
            }
        }

        /// <override/>
        protected override void OnCurrentCellActivating(GridCurrentCellActivatingEventArgs e)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    e,
                    CurrentCell,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(PaneDesc, e);
            }
#else
                                                         ;
#endif

            Debug.Assert(!inOnCurrentCellActivating);

            if (!HasTable)
            {
                return;
            }

            inOnCurrentCellActivating = true;

            try
            {
                if (this.currentCellMoveToElement != null)
                {
                    ////Table.ClearCollectionCaches();
                    Element el = Model.GetDisplayElementAt(e.RowIndex);
                    int index = Model.GetDisplayElementIndexOf(el);
                    ////TraceUtil.TraceCurrentMethodInfo(e.RowIndex, index);
                    if (index >= 0)
                    {
                        if (e.RowIndex != index)
                        {
                            e.RowIndex = index;
                        }
                    }
                    ////else
                    //// Debugger.Break();
                }

                if (Table.TableOptions.ListBoxSelectionMode != SelectionMode.None
                    && Table.TableOptions.ListBoxSelectionCurrentCellOptions == GridListBoxSelectionCurrentCellOptions.HideCurrentCell)
                {
                    Element el = Table.DisplayElements[e.RowIndex];

                    if (Element.IsRecord(el))
                    {
                        if (Table.TableDescriptor.Relations.NestedCount > 0 && Table.TableOptions.ShowRecordPlusMinus && !(Element.GetRecord(el) is AddNewRecord))
                        {
                            e.ColIndex = 1;
                        }
                        else
                        {
                            e.ColIndex = 0;
                        }
                    }
                }

                base.OnCurrentCellActivating(e);

                if (e.Cancel || this.inCurrentRecordContextChange)
                {
                    return;
                }

                GridCurrentCell gcc = CurrentCell;
                try
                {
                    gcc.Lock();

                    Element el = Model.GetDisplayElementAt(e.RowIndex);

                    if ( el is GridFilterBarRow)
                    {
                        e.Cancel = !ControlEnterRecord(el.ParentElement);
                    }
                    else if (el is NestedTable )
                    {
                        e.Cancel = !ControlEnterRecord(el);
                    }
                    else if (el is RecordRow)
                    {
                        e.Cancel = !ControlEnterRecord(el.ParentRecord);
                    }
                    else if (el is Record)
                    {
                        e.Cancel = !ControlEnterRecord(el);
                    }
                    else if (CaptionSection.IsCaption(el))
                    {
                        e.Cancel = !ControlEnterRecord(el);
                    }                    
                }
                finally
                {
                    gcc.Unlock();
                }
            }
            finally
            {
                inOnCurrentCellActivating = false;
                ////TraceUtil.TraceCurrentMethodInfo(CurrentCell);
            }
        }

        /// <override/>
        protected override void OnDrawCurrentCellBorder(GridDrawCurrentCellBorderEventArgs e)
        {
            base.OnDrawCurrentCellBorder(e);

            if (!e.Cancel)
            {
                if (Table.TableOptions.ListBoxSelectionMode != SelectionMode.None
                    && Table.TableOptions.ListBoxSelectionCurrentCellOptions == GridListBoxSelectionCurrentCellOptions.HideCurrentCell)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <override/>
        protected override void OnCurrentCellActivated(EventArgs e)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    CurrentCell,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
            if (HasTable)
            {
                if (CurrentCell.RowIndex == -1 || CurrentCell.ColIndex == -1)
                {
                    return;
                }

                GridColumnDescriptor columnDescriptor = Model.GetColumnDescriptorAt(CurrentCell.RowIndex, CurrentCell.ColIndex);

                // helps us later keep with keeping in sync after columns have changed ...
                if (columnDescriptor != null)
                {
                    Table.CurrentRecordManager.CurrentField = columnDescriptor.FieldDescriptor;
                }
            }
            ////TraceUtil.TraceCurrentMethodInfo(CurrentCell);
            base.OnCurrentCellActivated(e);
        }

        /// <override/>
        protected override void OnCurrentCellDeactivating(CancelEventArgs e)
        {
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                Syncfusion.Diagnostics.TraceUtil.TraceCurrentMethodInfo(this.PaneDesc, CurrentCell.RowIndex, CurrentCell.ColIndex);
            }
#endif
            base.OnCurrentCellDeactivating(e);

            if (!HasTable || e.Cancel || (CurrentCell.IsInMoveTo && !this.ShouldNavigateTo(currentCellMoveToElement)) || (!CurrentCell.IsInMoveTo && !this.IsCurrentElementAtCurrentCell()))
            {
                return;
            }

            isDeactivated = false;
            rejected = false;

            if (CurrentCell.IsInMoveTo && !IsCurrent(this.currentCellMoveToElement))
            {
                ////binder.ResetError();
                controlText = CurrentCell.Renderer.ControlText;
            }

            if (!CurrentCell.IsModified)
            {
                inOnCurrentCellDeactivating = true;
                GridCurrentCell gcc = CurrentCell;
                gcc.Lock();
                try
                {
                    if (CurrentCell.IsInMoveTo && !IsCurrent(this.currentCellMoveToElement))
                    {
                        ////Table toTable = Model.GetDisplayElementAt(CurrentCell.MoveToRowIndex).ParentTable;
                        ////Table fromTable = Model.GetDisplayElementAt(CurrentCell.RowIndex).ParentTable;
                        if (!ControlLeaveRecord())
                        {
                            e.Cancel = true;
                            InvalidateCurrentRecord();
                        }
                    }
                }
                finally
                {
                    inOnCurrentCellDeactivating = false;
                    gcc.Unlock();
                }
            }

            // otherwise, wait for AcceptedChanges
        }

        #endregion
        #region SynchronizeCurrentCellWithRecord
        /// <summary>
        /// Returns the <see cref="GridControlBase.CurrentCell"/> object or if the current cell is a nested table, returns the <see cref="GridControlBase.CurrentCell"/>
        /// of the <see cref="GridNestedTableControl"/>.
        /// </summary>
        /// <returns>The most inner <see cref="GridCurrentCell"/> object.</returns>
        public GridCurrentCell GetNestedCurrentCell()
        {
            if (CurrentCell.Renderer is GridNestedTableControlCellRenderer)
            {
                return ((GridTableControl)CurrentCell.Renderer.Control).GetNestedCurrentCell();
            }

            return CurrentCell;
        }

        /// <override/>
        protected override void GetScrollOutOfViewCurrentCellState(out GridCellRendererBase cellRenderer, out Control cellControl, out Rectangle currentCellBounds, out int currentCellRowIndex, out int currentCellColIndex, GridDirectionType direction)
        {
            if (direction == GridDirectionType.Down || direction == GridDirectionType.Up)
            {
                GridCurrentCell nestedGcc = this.GetNestedCurrentCell();
                cellRenderer = nestedGcc.Renderer;
                currentCellBounds = Rectangle.Empty;
                cellControl = null;

                // GridNestedTableControlCellRenderer ensures that when windows message events such as VScroll
                // or HScroll are sent that the context will be such that the FilteredChildTable is set
                // to the current nested table (with the current record). Therefore I don't have to
                // worry about switching GridNestedTableControlCellRenderer context here.
                if (CurrentCell.GetCurrentCell(out currentCellRowIndex, out currentCellColIndex))
                {
                    if (cellRenderer != null)
                    {
                        GridTable innerTable = ((GridTableControl)cellRenderer.Grid).Table;
                        Element row = innerTable.DisplayElements[nestedGcc.RowIndex];
                        if (row != null)
                        {
                            if (SupportsYAmount)
                            {
                                double yPos = Table.NestedDisplayElements.GetYAmountPositionOf(row);
                                currentCellBounds.Y = (int)yPos - this.GetCurrentVScrollPixelPos() + this.GetVScrollPixelMinimum();
                                currentCellBounds.Height = (int)row.GetYAmountCount();
                                currentCellBounds.X = ViewLayout.VscrollAreaBounds.Left;
                                currentCellBounds.Width = 1;
                                cellControl = cellRenderer.Control;
                            }
                            else
                            {
                                int rowIndex = Table.NestedDisplayElements.IndexOf(row);
                                currentCellBounds = ViewLayout.RectangleBottomOfRow(rowIndex - 1);
                                cellControl = cellRenderer.Control;
                            }
                        }
                    }
                }
            }
            else
            {
                base.GetScrollOutOfViewCurrentCellState(out cellRenderer, out cellControl, out currentCellBounds, out currentCellRowIndex, out currentCellColIndex, direction);
            }
        }

        void SynchronizeCurrentCellWithRecord(Element r)
        {
            SynchronizeCurrentCellWithRecord(r, true);
        }

        private bool inSynchronizeCurrentCellWithRecord = false;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InSynchronizeCurrentCellWithRecord
        {
            get
            {
                return this.inSynchronizeCurrentCellWithRecord;
            }
        }

        internal void SynchronizeCurrentCellWithRecord(Element r, bool scroll)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    scroll,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
            if (!Table.Elements.Contains(r))
            {
                return;
            }

            inSynchronizeCurrentCellWithRecord = true;

            try
            {
                GridCurrentCell gcc = CurrentCell;
                gcc.Lock();

                // Exand table and record in group
                if (r is Record && r.ParentTable.CurrentRecordManager.ForceShowCurrentRecord)
                {
                    Table.ShowRecord((Record)r, false);
                }

                gcc.Unlock();                                       ////1111
                if (r == null || r.ParentChildTable == null)
                {
                    return;
                }

                int rowIndex = r.ParentChildTable.DisplayElements.IndexOf(r);
                ViewLayout.Reset();
                ////CurrentCell.HasCurrentCell &&
                if ((CurrentCell.RowIndex >= Model.GetDisplayElementCount()) || (CurrentCell.ColIndex > Model.ColCount))
                {
                    CurrentCell.ResetCurrentCellWithoutDeactivate();
                }

                Model.ResetVolatileData();
                UpdateScrollBars();
                if (rowIndex == -1)
                {
                    CurrentCell.Deactivate(true);
                }
                else
                {
                    int colIndex = CurrentCell.ColIndex;
                    if (Table.CurrentRecordManager.CurrentRecord != null && Table.CurrentRecordManager.CurrentField != null)
                    {
                        GridRangeInfo rg = Model.RecordFieldToRangeInfo(Table.CurrentRecordManager.CurrentRecord, Table.CurrentRecordManager.CurrentField);
                        colIndex = rg.Left;
                        rowIndex = rg.Top;
                        if (!CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                        {
                            CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.NoSyncCurrentCell, true);
                        }
                        else
                        {
                            scroll = false;
                        }

                        if (scroll)
                        {
                            Group parentGroup = Table.CurrentRecordManager.CurrentElement != null ? Table.CurrentRecordManager.CurrentElement.ParentGroup : null;
                            if (parentGroup != null)
                            {
                                int topRowIndex = -1;
                                while (parentGroup != null)
                                {
                                    Rectangle rectMove = this.ViewLayout.RectangleBottomOfRow(this.InternalGetFrozenRows() + 1, GridCellSizeKind.VisibleSize);
                                    if (SupportsYAmount)
                                    {
                                        double pos = Table.DisplayElements.GetYAmountPositionOf(parentGroup);
                                        double pos1 = Table.DisplayElements.GetYAmountPositionOf(Table.CurrentRecordManager.CurrentRecord);
                                        if (pos1 - pos <= rectMove.Height)
                                        {
                                            topRowIndex = Table.DisplayElements.IndexOf(parentGroup);
                                        }
                                    }
                                    else
                                    {
                                        int pos = Math.Max(0, Table.DisplayElements.IndexOf(parentGroup));
                                        int pos1 = Table.DisplayElements.IndexOf(Table.CurrentRecordManager.CurrentRecord);
                                        if (this.GetRowRangeHeight(pos, pos1, rectMove.Height) <= rectMove.Height)
                                        {
                                            topRowIndex = pos;
                                        }
                                    }

                                    parentGroup = parentGroup.ParentGroup;
                                }

                                if (topRowIndex > InternalGetFrozenRows())
                                {
                                    this.ScrollCellInView(topRowIndex, CurrentCell.ColIndex, GridScrollCurrentCellReason.SynchronizeRecord);
                                }
                            }
                        }
                    }
                    else if (Table.CurrentRecordManager.CurrentElement != null && Table.CurrentRecordManager.ForceShowCurrentRecord)
                    {
                        colIndex = Math.Max(colIndex, Model.GetCaptionColIndex(Table.CurrentRecordManager.CurrentElement));
                        CurrentCell.AdjustRowColIfCoveredCell(ref rowIndex, ref colIndex);
                        if (!CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                        {
                            CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.NoSyncCurrentCell, true);
                        }
                        else
                        {
                            scroll = false;
                        }
                    }
                    else
                    {
                        scroll = false;
                    }

                    if (scroll && Table.CurrentRecordManager.ShouldScrollInView())
                    {
                        CurrentCell.ScrollInView(GridScrollCurrentCellReason.SynchronizeRecord);
                    }
                }
            }
            finally
            {
                inSynchronizeCurrentCellWithRecord = false;
            }
        }
        #endregion
        #region Current Cell Keyboard
        /// <summary>
        /// This is called from GridControlBase.ProcessKeyEventArgs and allows your customized cell renderer
        /// to process keyboard events before the GridControlBase gets the actual KeyDown / KeyUp event.
        /// </summary>
        /// The <see cref="T:System.Windows.Forms.Message"/> with data of the keyboard event.
        /// <returns>
        /// True if key was handled; False otherwise.
        /// </returns>
        /// <override/>
        int keystroke = 0;
        bool keypass = false;
        /// <summary>
        /// Determine the process key event.
        /// </summary>
        /// <param name="m">windows message</param>
        /// <returns>boolean value</returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            inProcessKeyEventArgs = true;
            try
            {
                keystroke++;
                if (!keypass && keystroke > 1 && m.Msg == WM_KEYDOWN)
                {
                    keypass = true;
                }
                return base.ProcessKeyEventArgs(ref m);
            }
            finally
            {
                inProcessKeyEventArgs = false;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool inProcessKeyEventArgs
        {
            get
            {
                return _inProcessKeyEventArgs;
            }

            set
            {
                _inProcessKeyEventArgs = value;
            }
        }

        /// <override/>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if ((e.Modifiers & Keys.Control) != 0 && (e.Modifiers & Keys.Shift) != 0 && e.KeyCode == Keys.R)
                {
                    this.GetTableControlWindow().Reinitialize();
                    ActiveXSnapshot.FakeLeftMouseClick(GetTableControlWindow(), new Point(1, 1));
                    e.Handled = true;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = HandleEnterKey();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = HandleEscapeKey();
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    e.Handled = HandleDeleteKey();
                }
                else if (e.KeyCode == Keys.Right)
                {
                    if (HasTable && CurrentCell.HasCurrentCell)
                    {
                        if (Table.TableOptions.AllowSelection == GridSelectionFlags.None)
                        {
                            Element el = Model.GetDisplayElementAt(CurrentCell.RowIndex);
                            if (CaptionSection.IsCaption(el))
                            {
                                Group g = el.ParentGroup;
                                if (!g.IsExpanded)
                                {
                                    g.IsExpanded = true;
                                    ScrollCellInView(CurrentCell.RowIndex + 1, CurrentCell.ColIndex, GridScrollCurrentCellReason.KeyPress);
                                    e.Handled = true;
                                }
                                else
                                {
                                    CurrentCell.MoveTo(CurrentCell.RowIndex + 1, CurrentCell.ColIndex + 1);
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Left)
                {
                    if (HasTable && CurrentCell.HasCurrentCell)
                    {
                        if (Table.TableOptions.AllowSelection == GridSelectionFlags.None)
                        {
                            Element el = Model.GetDisplayElementAt(CurrentCell.RowIndex);
                            if (CaptionSection.IsCaption(el) && !el.ParentGroup.IsTopLevelGroup)
                            {
                                Group g = el.ParentGroup;
                                if (g.IsExpanded)
                                {
                                    g.IsExpanded = !g.IsExpanded;
                                }
                                else if (g.ParentGroup != null && g.ParentGroup.GroupLevel >= 0)
                                {
                                    int newRowIndex = Model.GetDisplayElementIndexOf(g.ParentGroup.Caption);
                                    CurrentCell.MoveTo(newRowIndex, CurrentCell.ColIndex);
                                }

                                e.Handled = true;
                            }
                            else
                            {
                                Element g = Model.GetDisplayElementAt(CurrentCell.RowIndex);
                                if (g.ParentGroup != null && g.ParentGroup.GroupLevel >= 0
                                    && CurrentCell.ColIndex <= g.GroupLevel + 1)
                                {
                                    int newRowIndex = Model.GetDisplayElementIndexOf(g.ParentGroup.Caption);
                                    CurrentCell.MoveTo(newRowIndex, CurrentCell.ColIndex);
                                }
                            }
                        }
                    }
                }
            }

            allowScrollCaptionHeaderInView = true;
            try
            {
                base.OnKeyDown(e);
                Form frm = this.FindParentForm();
                if (e.KeyCode == Keys.C && e.Control && frm != null && frm.IsMdiChild)
                {
                    Element tableCurrentElement = Table.CurrentElement;
                    if (tableCurrentElement is NestedTable)
                    {
                        GridNestedTable nestedTable = (GridNestedTable)tableCurrentElement;
                        string t = this.Table.GetNestedTableCellType(nestedTable);
                        GridNestedTableControlCellRenderer rend = (GridNestedTableControlCellRenderer)this.CellRenderers[t];
                        GridNestedTableControl nestedTableControl = rend.Control;
                        if (nestedTableControl != null && nestedTableControl.Model.CutPaste.CanCopy())
                        {
                            nestedTableControl.Model.CutPaste.Copy();
                        }
                    }

                }
            }
            finally
            {
                allowScrollCaptionHeaderInView = false;
            }
        }

        /// <override/>
        protected override void OnMoveCurrentCellDirection(GridMoveCurrentCellDirectionEventArgs e)
        {
            base.OnMoveCurrentCellDirection(e);
        }

        /// <override/>
        protected override void OnQueryNextCurrentCellPosition(GridQueryNextCurrentCellPositionEventArgs e)
        {
            // skip plusminus buttons
            if (!inOnQueryNextCurrentCellPosition && HasTable)
            {
                inOnQueryNextCurrentCellPosition = true;
                try
                {
                    int newRowIndex = e.RowIndex;
                    int newColIndex = e.ColIndex;

                    switch (e.Direction)
                    {
                        case GridDirectionType.Down:
                            if (newRowIndex < Table.DisplayElements.Count && Table.DisplayElements[newRowIndex] is NestedTable)
                            {
                                if (newRowIndex + Table.DisplayElements[newRowIndex].GetVisibleCount() < Table.DisplayElements.Count)
                                {
                                    newRowIndex += Table.DisplayElements[newRowIndex].GetVisibleCount();
                                    e.RowIndex = newRowIndex;
                                    e.ColIndex = newColIndex;
                                    e.Result = true;
                                    e.Handled = true;
                                }
                                else
                                {
                                    e.Result = false;
                                    e.Handled = true;
                                }
                            }

                            break;

                        case GridDirectionType.Up:
                            if (newRowIndex > 0 && Table.DisplayElements[newRowIndex] is NestedTable)
                            {
                                if (newRowIndex - Table.DisplayElements[newRowIndex].GetVisibleCount() >= 0)
                                {
                                    newRowIndex -= Table.DisplayElements[newRowIndex].GetVisibleCount();
                                    e.RowIndex = newRowIndex;
                                    e.ColIndex = newColIndex;
                                    e.Result = true;
                                    e.Handled = true;
                                }
                                else
                                {
                                    e.Result = false;
                                    e.Handled = true;
                                }
                            }

                            break;
                    }

                    if (!this.GetNextCurrentCellPosition(e.Direction, ref newRowIndex, ref newColIndex))
                    {
                        if (e.RowIndex >= 0 && e.RowIndex < Model.GetDisplayElementCount())
                        {
                            Element el = Model.GetDisplayElementAt(e.RowIndex);
                            if (newColIndex != el.GroupLevel)
                            {
                                newColIndex = el.GroupLevel;
                                if (GetNextCurrentCellPosition(e.Direction, ref newRowIndex, ref newColIndex))
                                {
                                    e.RowIndex = newRowIndex;
                                    e.ColIndex = newColIndex;
                                    e.Result = true;
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    inOnQueryNextCurrentCellPosition = false;
                }
            }

            base.OnQueryNextCurrentCellPosition(e);
        }

        /// <override/>
        /// <summary>User pressed key down.</summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> holding the event data.</param>
        public override void OnCurrentCellKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && this.WantEnterKey && e.Modifiers != Keys.Control)
            {
                e.Handled = HandleEnterKey();
            }
            else if (e.KeyCode == Keys.Escape && this.WantEscapeKey)
            {
                e.Handled = HandleEscapeKey();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                e.Handled = HandleDeleteKey();
            }

            base.OnCurrentCellKeyDown(e);
        }

        ///<override/>
        /// <summary>
        /// Called when user press a key.
        /// </summary>
        /// <param name="e">A <see cref="KeyPressEventArgs"/> holding the event data</param>
        public override void OnCurrentCellKeyPress(KeyPressEventArgs e)
        {
            base.OnCurrentCellKeyPress(e);
            if (keypass && inProcessKeyEventArgs && (this.Model.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0)
            {
                GridCurrentCell cc = this.CurrentCell;
                if (cc.Renderer != null && cc.Renderer.CurrentStyle.CellType.Equals(GridCellTypeName.TextBox))
                {
                    GridTextBoxCellRenderer tr = (GridTextBoxCellRenderer)cc.Renderer;
                    if (!Char.IsControl(e.KeyChar) && !StartEditCanceled)
                        tr.ControlText += e.KeyChar;
                    tr.TextBox.SelectionStart = tr.ControlText.Length + 1;
                }
            }
        }

        /// <summary>
        /// Called when Delete key is pressed. Default implementation deletes selected records.
        /// </summary>
        /// <returns>true if records were deleted; false if no action was necessary.</returns>
        public virtual bool HandleDeleteKey()
        {
            GridCurrentCell gcc = this.GetNestedCurrentCell();
            if (gcc != null)
            {
                GridTableControl tableControl = gcc.Grid as GridTableControl;
                if (tableControl != null)
                {
                    return tableControl._HandleDeleteKey();
                }
            }

            return false;
        }

        bool _HandleDeleteKey()
        {
            GridCurrentCell gcc = GetCurrentCell();
            if ((gcc.HasCurrentCell && gcc.HasControlFocus) || !this.TableDescriptor.AllowRemove || !this.Table.SourceListAllowRemove)
            {
                return false;
            }

            bool cleared = false;

            GridRangeInfoList ranges = Model.SelectedRanges.GetRowRanges(GridRangeInfoType.Rows | GridRangeInfoType.Table);
            ArrayList records = new ArrayList();
            foreach (GridRangeInfo range in ranges)
            {
                int bottom = Math.Min(range.Bottom, Model.RowCount);
                for (int row = range.Top; row <= bottom; row++)
                {
                    Element el = Table.DisplayElements[row];
                    if (el is Record)
                    {
                        records.Add((Record)el);
                    }
                    else
                    {
                        RecordRow recordRow = el as RecordRow;
                        if (recordRow != null && recordRow.ParentRecord.Kind == DisplayElementKind.Record
                                           && recordRow.ParentRecord.RecordRows.IndexOf(recordRow) == 0)
                        {
                            records.Add(recordRow.ParentRecord);
                        }
                    }
                }
            }

            BeginUpdate(BeginUpdateOptions.InvalidateAndScroll);
            foreach (Record r in records)
            {
                r.Delete();
                if (r.IsDisposed)
                {
                    if (!cleared)
                    {
                        Model.SelectedRanges.Clear();
                        this.synchronizeGridShouldInvalidate = true;
                        this.Invalidate();
                        cleared = true;
                    }
                }
            }

            EndUpdate(false);

            return records.Count > 0;
        }

        /// <summary>
        /// Called when Escape key is pressed. Default implementation discards current record changes.
        /// </summary>
        /// <returns>true if changes were discarded; false if no action was necessary.</returns>
        public virtual bool HandleEscapeKey()
        {
            GridCurrentCell gcc = this.GetNestedCurrentCell();
            if (gcc != null)
            {
                GridTableControl tableControl = gcc.Grid as GridTableControl;
                if (tableControl != null)
                {
                    return tableControl._HandleEscapeKey();
                }
            }

            return false;
        }

        bool _HandleEscapeKey()
        {
            GridCurrentCell gcc = GetCurrentCell();
            if (HasTable && gcc.HasCurrentCell)
            {
                if (gcc.IsModified)
                {
                    return false;
                }

                if (Table.CurrentRecordManager.IsEditing)
                {
                    Table.CurrentRecordManager.CancelEdit();
                    if (!Table.CurrentRecordManager.IsEditing)
                    {
                        this.RefreshRange(Table.GetCurrentRecordRangeInfo());
                    }
                }
                else if (gcc.IsEditing)
                {
                    gcc.CancelEdit();
                }

                return true;
            }

            return false;
        }

        /// <override/>
        protected override void DrawInvertCell(Graphics g, int rowIndex, int colIndex, Rectangle rectItem, bool inPaint)
        {
            if (Table.DisplayElements[rowIndex] is NestedTable)
            {
                return;
            }

            base.DrawInvertCell(g, rowIndex, colIndex, rectItem, inPaint);
        }

        /// <summary>
        /// Called when Enter key is pressed grid is validated. Default implementation commits current record changes.
        /// </summary>
        /// <returns>true if changes were saved; false if no action was necessary.</returns>
        public virtual bool HandleEnterKey()
        {
            GridCurrentCell gcc = this.GetNestedCurrentCell();
            if (gcc != null)
            {
                GridTableControl tableControl = gcc.Grid as GridTableControl;
                if (tableControl != null)
                {
                    return tableControl._HandleEnterKey();
                }
            }

            return false;
        }

        bool _HandleEnterKey()
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom();
            }
#endif
            if (HasTable && CurrentCell.HasCurrentCell)
            {
                Element el = Model.GetDisplayElementAt(CurrentCell.RowIndex);
                if (el == null)
                {
                    CurrentCell.ResetCurrentCellWithoutDeactivate();
                    return true;
                }

                if (CaptionSection.IsCaption(el) && el.GroupLevel > 0)
                {
                    if (!this.IsInValidating)
                    {
                        Group g = el.ParentGroup;
                        g.IsExpanded = !g.IsExpanded;
                        return true;
                    }
                }
                else
                {
                    if (CurrentCell.Model is GridTableFilterBarCellModel && CurrentCell.IsEditing)
                    {
                        return false;
                    }

                    if (CurrentCell.IsModified)
                    {
                        CurrentCell.ConfirmChanges();

                        if ((Table.TableModel.Options.ActivateCurrentCellBehavior & GridCellActivateAction.DblClickOnCell) != 0 && 
                            (Table.TableModel.Options.AllowSelection == GridSelectionFlags.Any || Table.TableModel.Options.AllowSelection == GridSelectionFlags.None))
                        {
                            CurrentCell.Deactivate(false);
                            CurrentCell.MoveTo(CurrentCell.RowIndex, CurrentCell.ColIndex);
                            CurrentCell.Activate(CurrentCell.RowIndex, CurrentCell.ColIndex);
                        }

                        if (CurrentCell.Exception != null)
                        {
                            CancelUpdate();
                            CurrentCell.DisplayWarningText(CurrentCell.ErrorMessage);
                            return true;
                        }
                    }

                    if (Table.CurrentRecordManager.IsModified)
                    {
#if DEBUG
                        if (Switches.GroupingGrid.TraceVerbose)
                        {
                            Trace.WriteLine("Table: " + Model.GetDisplayElementCount().ToString());
                            Trace.WriteLine(Switches.GroupingGrid.TraceVerbose, "Details: " + Table.TopLevelGroup.Details.GetVisibleCount().ToString());
                        }
#endif
                        bool isAddNew = Table.CurrentRecord is AddNewRecord;
                        Record rp = Table.CurrentRecordManager.CurrentRecord;
                        Table.CurrentRecordManager.EndEdit();
                        if (!Table.CurrentRecordManager.IsEditing)
                        {
                            if (!this.groupingControl.Engine.InvalidateAllWhenListChanged)
                            {
                                Update();
                            }

                            if (isAddNew && Table.LastChangedRecord != null)
                            {
                                Table.CurrentRecordManager.NavigateTo(Table.LastChangedRecord);
                                if (Table.TableOptions.ListBoxSelectionMode != SelectionMode.None)
                                {
                                    Table.LastChangedRecord.SetSelected(true);
                                }
                            }
                            else if (rp != null && !rp.IsDisposed && rp.MeetsFilterCriteria())
                            {
                                Table.CurrentRecordManager.NavigateTo(rp); //// this should expand nested parent tables when record now belongs to another child table.
                            }
                            else
                            {
                                Table.CurrentRecord = null;
                            }

                            if (Table.CurrentRecord != null)
                            {
                                // this scrolls the current record into view and also positions the current cell
                                this.SynchronizeCurrentCellWithRecord(Table.CurrentRecord, true);

                                if (Table.Engine.BindToCurrencyManager && Table.GetCurrencyManager() != null)
                                {
                                    Table.GetCurrencyManager().Position = Table.UnsortedRecords.IndexOf(Table.CurrentRecord);
                                }
                            }
                        }
                        else
                        {
                            SynchronizeCurrentCellAfterEndEditFailed();
                        }

                        /*                                                                                                                  else
                        //                                                                                                                  {
                        //                                                                                                                                     Update();
                        //                                                                                                                                     CurrentCell.ScrollInView();
                                                                                                                                        }*/
#if DEBUG
                        if (Switches.GroupingGrid.TraceVerbose)
                        {
                            Trace.WriteLine("Table: " + Model.GetDisplayElementCount().ToString());
                            Trace.WriteLine("Details: " + Table.TopLevelGroup.Details.GetVisibleCount().ToString());
                        }
#endif
                    }

                    return true;
                }
            }

            return false;
        }

        internal void SynchronizeCurrentCellAfterEndEditFailed()
        {
            if (Table.CurrentRecordManager.IsAnyPropertyInvalid)
            {
                CurrentRecordProperty p = null;
                if (Table.CurrentRecordManager.CurrentField != null)
                {
                    p = Table.CurrentRecordManager.Properties[Table.CurrentRecordManager.CurrentField];
                }

                if (p == null || !p.IsError)
                {
                    foreach (CurrentRecordProperty prop in Table.CurrentRecordManager.Properties)
                    {
                        // jump to invalid field here.
                        if (prop.IsError)
                        {
                            Table.CurrentRecordManager.CurrentField = prop.FieldDescriptor;
                            break;
                        }
                    }
                }
            }

            this.SynchronizeCurrentCellWithRecord(Table.CurrentRecord, true);
        }

        #endregion
        #region Cell Click
        /// <override/>
        protected override void OnPushButtonClick(GridCellPushButtonClickEventArgs e)
        {
            base.OnPushButtonClick(e);

            if (!HasTable)
            {
                return;
            }

            GridTableCellStyleInfo style = Table.GetTableCellStyle(e.RowIndex, e.ColIndex);
            if (style.TableCellIdentity == null)
            {
                return;
            }

            Element el = style.TableCellIdentity.DisplayElement;

            if (style.TableCellIdentity.TableCellType == GridTableCellType.GroupCaptionPlusMinusCell)
            {
                if (CaptionSection.IsCaption(el) && el.ParentTable == Table)
                {
                    Group g = el.ParentGroup;
                    bool shouldExpand = !g.IsExpanded;
                    Table.CurrentRecordManager.NavigateTo(g.Caption, false, false);
                    g.IsExpanded = shouldExpand;
                    ////e.Cancel = true;
                }
            }
            else if (style.TableCellIdentity.TableCellType == GridTableCellType.RecordPlusMinusCell)
            {
                Record r = Record.GetParentRecord(el);
                bool shouldExpand = !r.IsExpanded;
                Table.CurrentRecord = r;
                r.IsExpanded = shouldExpand;
                ////e.Cancel = true;
            }
        }

        ///<override/>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Element el = null;
            if (HasTable && !IsDisposed)
            {
                int rowIndex, colIndex;
                if (this.PointToRowCol(e.Location, out rowIndex, out colIndex))
                {
                    el = Model.GetDisplayElementAt(rowIndex);
                    this._rowIndex = rowIndex;
                    this._colIndex = colIndex;
                }
            }
            if (el != null && el.Kind == DisplayElementKind.StackedHeader
                && e.Button == MouseButtons.Left)
            {
                if (e.Clicks == 1)
                {
                    HitMouseClick = true;
                }
                else if (e.Clicks == 2)
                {
                    HitMouseDoubleClick = true;
                }
            }
            if (el != null && el.Kind == DisplayElementKind.ColumnHeader
                && e.Button == MouseButtons.Left && e.Clicks == 2)
            {
                HitMouseDoubleClick = true;
            }
            base.OnMouseDown(e);
        }

        /// <override/>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            HitMouseDoubleClick = false;
            HitMouseClick = false;
            base.OnMouseUp(e);
        }

        /// <override/>
        protected override void OnCellDoubleClick(GridCellClickEventArgs e)
        {
            Element el = null;
            if (HasTable && !IsDisposed)
            {
                el = Model.GetDisplayElementAt(e.RowIndex);
            }

            base.OnCellDoubleClick(e);

            if (el != null && !el.IsDisposed && !e.Cancel && HasTable && !IsDisposed)
            {
                if (CaptionSection.IsCaption(el) && el.ParentTable == Table)
                {
                    Group g = el.ParentGroup;
                    g.IsExpanded = !g.IsExpanded;
                }
            }
        }

        /// <override/>
        protected override void OnCellButtonClicked(GridCellButtonClickedEventArgs e)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    e,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
            base.OnCellButtonClicked(e);

            if (e.Cancel || !HasTable || IsDisposed)
            {
                return;
            }

            GridTableCellStyleInfo style = Table.GetTableCellStyle(e.RowIndex, e.ColIndex);
            if (style.TableCellIdentity == null)
            {
                return;
            }

            Element el = style.TableCellIdentity.DisplayElement;

            if (style.TableCellIdentity.TableCellType == GridTableCellType.GroupCaptionPlusMinusCell)
            {
                if (CaptionSection.IsCaption(el) && el.ParentTable == Table)
                {
                    Group g = el.ParentGroup;
                    bool shouldExpand = !g.IsExpanded;
                    Table.CurrentRecordManager.NavigateTo(g.Caption, false, false);
                    g.IsExpanded = shouldExpand;
                    e.Cancel = true;
                }
            }
            else if (style.TableCellIdentity.TableCellType == GridTableCellType.RecordPlusMinusCell)
            {
                Record r = Record.GetParentRecord(el);
                bool shouldExpand = !r.IsExpanded;
                Table.CurrentRecord = r;
                r.IsExpanded = shouldExpand;
                e.Cancel = true;
            }
        }

        /// <override/>
        protected override void OnCellClick(GridCellClickEventArgs e)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    e,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
            base.OnCellClick(e);

            if (e.Cancel)
            {
                return;
            }

            GridTableCellStyleInfo style = Table.GetTableCellStyle(e.RowIndex, e.ColIndex);
            if (style.TableCellIdentity == null)
            {
                return;
            }

            Element el = style.TableCellIdentity.DisplayElement;

            if ((el is ColumnHeaderRow || el is ColumnHeaderSection) && el.ParentTable == Table)
            {
                GridColumnDescriptor column = style.TableCellIdentity.Column;

                if (Table.TableOptions.AllowSortColumns && column != null)
                {
                    GridQueryAllowSortColumnEventArgs ae = new GridQueryAllowSortColumnEventArgs(this, column, e);
                    ae.AllowSort = column.AllowSort;
                    RaiseQueryAllowSortColumn(ae);
                    if (ae.AllowSort)
                    {
                        if (!this.ControlEndEdit(true))
                        {
                            e.Cancel = true;
                            return;
                        }

                        ////Debug.WriteLine(Control.ModifierKeys);
                        bool ctrlKeyPressed = TableDescriptor.TableOptions.AllowMultiColumnSort && (Control.ModifierKeys & Keys.Control) != 0;

                        ////TableDescriptor.GroupedColumns.FixCollection();
                        SortColumnDescriptor sd = TableDescriptor.GroupedColumns[column.MappingName];
                        if (sd != null)
                        {
                            if (sd.SortDirection == ListSortDirection.Ascending)
                            {
                                sd.SortDirection = ListSortDirection.Descending;
                            }
                            else
                            {
                                sd.SortDirection = ListSortDirection.Ascending;
                            }
                        }
                        else
                        {
                            sd = TableDescriptor.SortedColumns[column.MappingName];
                            if (sd != null && (TableDescriptor.SortedColumns.Count == 1 || ctrlKeyPressed))
                            {
                                if (sd.SortDirection == ListSortDirection.Ascending)
                                {
                                    sd.SortDirection = ListSortDirection.Descending;
                                }
                                else
                                {
                                    sd.SortDirection = ListSortDirection.Ascending;
                                }
                            }
                            else if (ctrlKeyPressed)
                            {
                                TableDescriptor.SortedColumns.Add(column.MappingName);
                            }
                            else
                            {
                                TableDescriptor.SortedColumns.Clear();
                                this.BeginUpdate(BeginUpdateOptions.Invalidate);
                                TableDescriptor.SortedColumns.Add(column.MappingName);
                                this.EndUpdate(true);
                            }
                        }

                        e.Cancel = true;
                        Table.CurrentRecordManager.CurrentField = column.FieldDescriptor;
                        //// TODO: Better would be to have engine figure out what to update Refresh()
                        ////if (this.IsWindowless)
                        ////                   Refresh();
                        GetGridWindow().Invalidate();
                        this.synchronizeGridShouldScrollCurrentCell = true;
                        this.synchronizeGridShouldRestoreCurrentCell = true;
                    }
                }
            }
            else if (CaptionSection.IsCaption(el))
            {
                if (style.TableCellIdentity.TableCellType == GridTableCellType.GroupCaptionCell)
                {
                    Table.CurrentRecordManager.NavigateTo(el);
                }
            }
        }

        #endregion

        #region SynchronizeWithTable

        bool _synchronizeGridShouldnextUpdateScrollBars = false;
        bool _synchronizeGridShouldUpdateColumnWidths = true;
        bool _synchronizeGridShouldRestoreCurrentCell = false;
        bool _synchronizeGridShouldScrollCurrentCell = false;
        bool _synchronizeGridShouldInvalidate = false;

        /// <summary>
        /// Returns the parent control that has a window handle.
        /// </summary>
        /// <returns>This control or the parent control that has a window handle if this control is windowless.</returns>
        public GridTableControl GetTableControlWindow()
        {
            return (GridTableControl)GetWindow();
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool synchronizeGridShouldnextUpdateScrollBars
        {
            get
            {
                return GetTableControlWindow()._synchronizeGridShouldnextUpdateScrollBars;
            }

            set
            {
                GetTableControlWindow()._synchronizeGridShouldnextUpdateScrollBars = value;
#if DEBUG
                if (traceSynchronizeGridWithEngine && value && value != synchronizeGridShouldnextUpdateScrollBars)
                {
                    TraceUtil.TraceCurrentMethodInfo(
                        "inSynchronizeGridWithEngine",
                        inSynchronizeGridWithEngine,
                        "IsInMoveTo",
                        CurrentCell.IsInMoveTo,
                        "IsInActiveOrDeactivate",
                        CurrentCell.IsInActiveOrDeactivate,
                        "inProcessKeyEventArgs",
                        inProcessKeyEventArgs,
                        "Updating",
                        Updating || ((GridTableControl)GetWindow()).Updating);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool synchronizeGridShouldUpdateColumnWidths
        {
            get
            {
                return GetTableControlWindow()._synchronizeGridShouldUpdateColumnWidths;
            }

            set
            {
                GetTableControlWindow()._synchronizeGridShouldUpdateColumnWidths = value;
#if DEBUG
                if (traceSynchronizeGridWithEngine && value && value != synchronizeGridShouldUpdateColumnWidths)
                {
                    TraceUtil.TraceCurrentMethodInfo(
                        "inSynchronizeGridWithEngine",
                        inSynchronizeGridWithEngine,
                        "IsInMoveTo",
                        CurrentCell.IsInMoveTo,
                        "IsInActiveOrDeactivate",
                        CurrentCell.IsInActiveOrDeactivate,
                        "inProcessKeyEventArgs",
                        inProcessKeyEventArgs,
                        "Updating",
                        Updating || ((GridTableControl)GetWindow()).Updating);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool synchronizeGridShouldRestoreCurrentCell
        {
            get
            {
                return GetTableControlWindow()._synchronizeGridShouldRestoreCurrentCell;
            }

            set
            {
#if DEBUG
                if (traceSynchronizeGridWithEngine && value && value != synchronizeGridShouldRestoreCurrentCell)
                {
                    TraceUtil.TraceCurrentMethodInfo(
                        "inSynchronizeGridWithEngine",
                        inSynchronizeGridWithEngine,
                        "IsInMoveTo",
                        CurrentCell.IsInMoveTo,
                        "IsInActiveOrDeactivate",
                        CurrentCell.IsInActiveOrDeactivate,
                        "inProcessKeyEventArgs",
                        inProcessKeyEventArgs,
                        "Updating",
                        Updating || ((GridTableControl)GetWindow()).Updating);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
                GetTableControlWindow()._synchronizeGridShouldRestoreCurrentCell = value;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool synchronizeGridShouldScrollCurrentCell
        {
            get
            {
                return GetTableControlWindow()._synchronizeGridShouldScrollCurrentCell;
            }

            set
            {
#if DEBUG
                if (traceSynchronizeGridWithEngine && value && value != synchronizeGridShouldScrollCurrentCell)
                {
                    TraceUtil.TraceCurrentMethodInfo(
                        "inSynchronizeGridWithEngine",
                        inSynchronizeGridWithEngine,
                        "IsInMoveTo",
                        CurrentCell.IsInMoveTo,
                        "IsInActiveOrDeactivate",
                        CurrentCell.IsInActiveOrDeactivate,
                        "inProcessKeyEventArgs",
                        inProcessKeyEventArgs,
                        "Updating",
                        Updating || ((GridTableControl)GetWindow()).Updating);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
                GetTableControlWindow()._synchronizeGridShouldScrollCurrentCell = value;
            }
        }

        /// <overload>
        /// Forces the grid to synchronize ittself with changes in the underlying <see cref="Table"/> the next time
        /// an Idle message is received or any Window message is processed.
        /// </overload>
        /// <summary>
        /// Forces the grid to synchronize itself with changes in the underlying <see cref="Table"/> the next time
        /// an Idle message is received or any Window message is processed.
        /// </summary>
        public void MarkResync()
        {
            MarkResync(false);
        }

        /// <summary>
        /// Forces the grid to synchronize itself with changes in the underlying <see cref="Table"/> the next time
        /// an Idle message is received or any Window message is processed.
        /// </summary>
        /// <param name="resetWidth">Specifies if the width should be reset.</param>
        public void MarkResync(bool resetWidth)
        {
            synchronizeGridShouldInvalidate = true;
            synchronizeGridShouldRestoreCurrentCell = true;
            synchronizeGridShouldUpdateColumnWidths = true;
            synchronizeGridShouldnextUpdateScrollBars = true;
            if (resetWidth)
            {
                this.maximumWidth = -1;
            }
        }

        Timer delayUpdateTimer;
        int firstDelayUpdateTimerTicks = 0;
        bool inKillDelayUpdateTimer = false;

        private void delayUpdateTimer_Tick(object sender, EventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            if (!inKillDelayUpdateTimer && Environment.TickCount >= firstDelayUpdateTimerTicks)
            {
                KillDelayUpdateTimer();
                if (this.allowRaiseDelayUpdateTime)
                {
                    this.GroupingControl.Update();
                }
            }
        }

        internal void KillDelayUpdateTimer()
        {
            if (inKillDelayUpdateTimer || delayUpdateTimer == null)
            {
                return;
            }

            ////TraceUtil.TraceCurrentMethodInfo();
            inKillDelayUpdateTimer = true;

            delayUpdateTimer.Tick -= new EventHandler(delayUpdateTimer_Tick);
            delayUpdateTimer.Dispose();
            delayUpdateTimer = null;

            inKillDelayUpdateTimer = false;
        }

        internal void StartDelayUpdateTimer()
        {
            ////TraceUtil.TraceCurrentMethodInfo();
            if (delayUpdateTimer == null && this.allowRaiseDelayUpdateTime && GroupingControl != null)
            {
                if (!this.DesignMode && (GroupingControl.DelayUpdateBehavior & GridDelayUpdateBehavior.Timer) != 0)
                {
                    // Don't do this in design mode ...
                    firstDelayUpdateTimerTicks = Environment.TickCount + 20;
                    delayUpdateTimer = new Timer();

                    delayUpdateTimer.Interval = 20;
                    delayUpdateTimer.Tick += new EventHandler(delayUpdateTimer_Tick);
                }

                if ((GroupingControl.DelayUpdateBehavior & GridDelayUpdateBehavior.PostCustomMessage) != 0)
                {
                    PostMessage(Handle, WM_CUSTOM, 0, 0);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern internal static IntPtr PostMessage(IntPtr hwnd, int msg, int wparam, int lparam);

        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        internal bool synchronizeGridShouldInvalidate
        {
            get
            {
                return GetTableControlWindow()._synchronizeGridShouldInvalidate || (Table != null && Table.TableDirty);
            }

            set
            {
                if (!this.IsWindowless && value && value != GetTableControlWindow()._synchronizeGridShouldInvalidate)
                {
                    StartDelayUpdateTimer();
                }

#if DEBUG
                if (traceSynchronizeGridWithEngine && value && value != synchronizeGridShouldInvalidate)
                {
                    TraceUtil.TraceCurrentMethodInfo(
                        "inSynchronizeGridWithEngine",
                        inSynchronizeGridWithEngine,
                        "IsInMoveTo",
                         CurrentCell.IsInMoveTo,
                         "IsInActiveOrDeactivate",
                         CurrentCell.IsInActiveOrDeactivate,
                         "inProcessKeyEventArgs",
                         inProcessKeyEventArgs,
                         "Updating",
                         Updating || ((GridTableControl)GetWindow()).Updating);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
                GetTableControlWindow()._synchronizeGridShouldInvalidate = value;
                GetTableControlWindow().cachedLastRow = -1;
            }
        }

        void UpdateNavigationBar()
        {
            GridGroupingControl g = this.TableDescriptor.Engine.ParentControl as GridGroupingControl;
            if (g != null)
            {
                g.UpdateNavigationBar();
            }
        }

        /// <summary>
        /// Returns the <see cref="GridGroupingControl"/> that hosts this control.
        /// </summary>
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GridGroupingControl GroupingControl
        {
            get
            {
                return groupingControl;
            }
        }

        internal GridGroupingControl groupingControl;

        /// <summary>
        /// Gets / sets the <see cref="GridGroupDropArea"/> of the <see cref="GridGroupingControl"/>.
        /// </summary>
        public GridGroupDropArea GroupDropArea
        {
            get
            {
                if (groupDropArea == null)
                {
                    groupDropArea = (GridGroupDropArea)Model.GroupDropAreaModel.ActiveGridView;
                }

                return groupDropArea;
            }

            set
            {
                groupDropArea = value;
            }
        }

        GridGroupDropArea groupDropArea;

        private bool inSynchronizeGridWithEngine = false;

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [ReadOnly(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InSynchronizeGridWithEngine
        {
            get
            {
                return this.inSynchronizeGridWithEngine;
            }
        }

        /// <summary>
        /// Synchronize the grid display with changes in the underlying <see cref="Table"/>.
        /// </summary>
        public void SynchronizeGridWithEngine()
        {
            try
            {
#if DEBUG
                /*if (false && traceSynchronizeGridWithEngine)
                //{
                //    TraceUtil.TraceCurrentMethodInfo("inSynchronizeGridWithEngine", inSynchronizeGridWithEngine, "IsInMoveTo",
                //        CurrentCell.IsInMoveTo, "IsInActiveOrDeactivate", CurrentCell.IsInActiveOrDeactivate,
                //        "inProcessKeyEventArgs", inProcessKeyEventArgs,
                //        "Updating", Updating || ((GridTableControl) GetWindow()).Updating);
                }*/
#endif
                if (Table != null && Table.LockOutEnsureInitialized)
                {
                    return;
                }

                bool anyAction = false;

                KillDelayUpdateTimer();

                if (this is GridNestedTableControl && this.GetTableControlWindow() != null)
                {
                    this.GetTableControlWindow().SynchronizeGridWithEngine();
                    return;
                }

                if (!Model.HasTable || inSynchronizeGridWithEngine || CurrentCell.IsInMoveTo || CurrentCell.IsInEndEdit || CurrentCell.IsInActiveOrDeactivate || inProcessKeyEventArgs)
                {
                    return;
                }

                this.TableDescriptor.EnsureSummaryDescriptors();
                this.TableDescriptor.EnsureRecordRowColumns(); //// TODO: related tables

                SelectRecords.SyncActiveRange();

                int width = this.GetHorizontalScrollWidth();
                if (width != this.maximumWidth)
                {
                    this.synchronizeGridShouldnextUpdateScrollBars = true;
                }

                maximumWidth = width;

                if (!this.initialized)
                {
                    return;
                }

                if (Updating || ((GridTableControl)GetWindow()).Updating)
                {
                    if (synchronizeGridShouldInvalidate)
                    {
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("in Updating - Invalidate and return");
                        }

                        Table.ResetRepaintElementsInQueue();
                        Invalidate();
                        synchronizeGridShouldInvalidate = false;
                    }

                    return;
                }

                GridCurrentCell nestedCurrentCell = GetNestedCurrentCell();
                if (/*nestedCurrentCell.IsInMoveTo || */nestedCurrentCell.IsInActiveOrDeactivate)
                {
                    if (traceSynchronizeGridWithEngine)
                    {
                        TraceUtil.TraceCurrentMethodInfo("in CurrentCell operation - return");
                    }

                    return;
                }

                inSynchronizeGridWithEngine = true;
                try
                {
                    if (traceSynchronizeGridWithEngine)
                    {
                        Trace.Indent();
                    }

                    if (Table != null && (synchronizeGridShouldInvalidate || synchronizeGridShouldRestoreCurrentCell))
                    {
#if DEBUG
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("InitializePropertyDescriptors");
                        }
#endif
                        Table.TableDescriptor.InitializePropertyDescriptors();
                        anyAction = true;
                    }

                    if (Table != null && Table.TableDirty)
                    {
                        synchronizeGridShouldUpdateColumnWidths = true;
                    }

                    if (synchronizeGridShouldnextUpdateScrollBars || synchronizeGridShouldRestoreCurrentCell || synchronizeGridShouldInvalidate)
                    {
#if DEBUG
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Reset ViewLayout");
                        }
#endif
                        Model.ResetVolatileData();
                        ViewLayout.Reset();

                        InitializeFrozenRowCount();
                        Table.EngineVersion++;

                        int r = ViewLayout.LastVisibleRow;
                        anyAction = true;
                    }

                    if (synchronizeGridShouldUpdateColumnWidths)
                    {
#if DEBUG
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("UpdateColumnWidths");
                        }
#endif
                        synchronizeGridShouldUpdateColumnWidths = !Model.UpdateColumnWidths();
                        if (!synchronizeGridShouldUpdateColumnWidths)
                        {
                            this.maximumWidth = this.GetHorizontalScrollWidth();
                            this.ViewLayout.Reset();
                        }

                        anyAction = true;
                    }

                    if (synchronizeGridShouldnextUpdateScrollBars || synchronizeGridShouldInvalidate)
                    {
                        InitializeFrozenRowCount();
#if DEBUG
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("UpdateScrollBars");
                        }
#endif
                        GridCurrentCell gcc = CurrentCell;
                        gcc.Lock();
                        this.UpdateScrollBars();
                        UpdateNavigationBar();
                        gcc.Unlock();
                        anyAction = true;
                    }

                    if (this.synchronizeGridShouldInvalidate)
                    {
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("Invalidate");
                        }

                        Invalidate();
                        anyAction = true;
                    }

                    if (synchronizeGridShouldRestoreCurrentCell)
                    {
                        if (synchronizeGridNavigateTo != null && synchronizeGridNavigateTo.GetVisibleCount() == 0)
                        {
                            synchronizeGridNavigateTo = null;
                        }

                        if (synchronizeGridNavigateToBookmark != null && synchronizeGridNavigateToBookmark.GetVisibleCount() == 0)
                        {
                            synchronizeGridNavigateToBookmark = null;
                        }

                        synchronizeGridNavigateTo = Table.FixVirtualMode(synchronizeGridNavigateTo);
                        synchronizeGridNavigateToBookmark = (Record)Table.FixVirtualMode(synchronizeGridNavigateToBookmark);

#if DEBUG
                        if (traceSynchronizeGridWithEngine)
                        {
                            TraceUtil.TraceCurrentMethodInfo("RestoreCurrentCell");
                        }
#endif
                        inSynchronizeGridShouldRestoreCurrentCell = true;
                        ViewLayout.Reset();
                        if (synchronizeGridNavigateTo == null)
                        {
                            Group g = null;
                            if (this.synchronizeGridNavigateToBookmark != null)
                            {
                                if (this.synchronizeGridShouldScrollCurrentCell)
                                {
                                    ScrollInView(synchronizeGridNavigateToBookmark, GridScrollCurrentCellReason.SynchronizeRecord);
                                }

                                g = synchronizeGridNavigateToBookmark.ParentGroup;
                            }

                            if (g == null)
                            {
                                g = Table.TopLevelGroup;
                            }

                            switch (this.synchronizeGridNavigateToKind)
                            {
                                case DisplayElementKind.Caption:
                                    synchronizeGridNavigateTo = g.Caption;
                                    if (synchronizeGridNavigateTo != null)
                                    {
                                        synchronizeGridNavigateTo.Disposed += new EventHandler(synchronizeGridNavigateTo_Disposed);
                                    }

                                    break;
                                case DisplayElementKind.AddNewRecord:
                                    synchronizeGridNavigateTo = g.FindAddNewRecord();
                                    if (synchronizeGridNavigateTo != null)
                                    {
                                        synchronizeGridNavigateTo.Disposed += new EventHandler(synchronizeGridNavigateTo_Disposed);
                                    }

                                    break;
                            }
                        }

                        if (Table.CurrentElement == null
                                           && synchronizeGridNavigateTo != null
                                           && !synchronizeGridNavigateTo.IsDisposed
                                           && synchronizeGridNavigateTo.ParentElement != null)
                        {
                            if (synchronizeGridNavigateTo.GetVisibleCount() > 0)
                            {
                                Table.CurrentRecordManager.NavigateTo(synchronizeGridNavigateTo);
                            }
                            ////ViewLayout.Reset();
                            ////ScrollInView(synchronizeGridNavigateTo);
                        }

                        ResetSynchronizeGridNavigateTo();
                        ResetSynchronizeGridNavigateToBookmark();

                        //// get inner-most current element
                        Element innerMostCurrentElement = Table.GetInnerMostCurrentElement();
                        if (innerMostCurrentElement != null && innerMostCurrentElement.ParentTable != null)
                        {
                            //// make sure all parent child tables are expanded.
                            if (innerMostCurrentElement.ParentTable.CurrentRecordManager.ForceShowCurrentRecord)
                            {
                                Table.ShowRecord(innerMostCurrentElement, true);
                            }

                            //// Scroll innerMostCurrentElement into view.

                            //// If that caused a DisplayElementChanged notification then make sure display is invalidated.
                            if (synchronizeGridShouldInvalidate)
                            {
                                Invalidate();
                            }

                            ////int rowIndex = Table.NestedDisplayElements.IndexOf(innerMostCurrentElement);
                            ////this.ScrollCellInView(rg);
                            //// synchronize current cell and switch current cell context for nested table controls.
                            Element tableCurrentElement = Table.CurrentElement;
                            if (tableCurrentElement != null)
                            {
                                this.SynchronizeCurrentCellWithRecord(tableCurrentElement, false);

                                tableCurrentElement = Table.CurrentElement; //// just in case it was changed ...
                                //// not disposed ...
                                if (tableCurrentElement != null && tableCurrentElement.ParentTable != null)
                                {
                                    GridTableControl tableControl = this;

                                    Stack restoreStack = new Stack(); //// see SwitchNestedTableAndRestore below.

                                    while (tableCurrentElement is NestedTable)
                                    {
                                        GridNestedTable nestedTable = (GridNestedTable)tableCurrentElement;
                                        GridChildTable childTable = (GridChildTable)nestedTable.ChildTable;
                                        if (childTable != null && !childTable.IsDisposed)
                                        {
                                            tableCurrentElement = childTable.ParentTable.CurrentElement;
                                            if (tableCurrentElement == null)
                                            {
                                                //// CurrentElement got lost in previous rend.SwitchNestedTableAndRestore call. Not sure why, but
                                                //// we can restore it here from innerMostCurrentElement.
                                                tableCurrentElement = innerMostCurrentElement;
                                                while (tableCurrentElement != null && tableCurrentElement.ParentChildTable != childTable)
                                                {
                                                    ChildTable ct = tableCurrentElement.ParentChildTable;
                                                    if (ct == null)
                                                    {
                                                        tableCurrentElement = null;
                                                    }
                                                    else
                                                    {
                                                        tableCurrentElement = ct.ParentNestedTable;
                                                    }
                                                }

                                                childTable.ParentTable.CurrentElement = tableCurrentElement;
                                            }
                                        }
                                        else
                                        {
                                            tableCurrentElement = null;
                                        }

                                        //// Disposed check
                                        if (tableCurrentElement != null && tableCurrentElement.ParentElement == null)
                                        {
                                            tableCurrentElement = null;
                                        }

                                        //// NOTE: tableCurrentElement is the current element in the inner table, not this table
                                        if (tableCurrentElement != null)
                                        {
                                            //// Get GridNestedTableControl for inner table, register it if necessary.
                                            string t = this.Table.GetNestedTableCellType(nestedTable);
                                            tableControl.Model.RegisterNestedTableCellModel(nestedTable.ChildTable.ParentTable);
                                            GridNestedTableControlCellRenderer rend = (GridNestedTableControlCellRenderer)tableControl.CellRenderers[t];
                                            GridNestedTableControl nestedTableControl = rend.Control;
                                            nestedTableControl.Model.Table.TableModel = nestedTableControl.Model;

                                            //// Fix current cell if it is out of sync.
                                            restoreStack.Push(rend.SwitchNestedTableAndRestore(nestedTable));
                                            nestedTableControl.SynchronizeCurrentCellWithRecord(tableCurrentElement, false);
                                            tableControl = nestedTableControl;
                                        }
                                    }

                                    //// Restore editing state (focus, caret position)
                                    if (currentCellIsEditing)
                                    {
                                        FieldDescriptor fd = this.currentCellColumnDescriptor != null ? this.currentCellColumnDescriptor.FieldDescriptor : null;
                                        if (fd != null && fd == tableControl.Table.CurrentRecordManager.CurrentField)
                                        {
                                            tableControl.CurrentCell.BeginEdit();
                                            tableControl.CurrentCell.Renderer.SetEditState(currentCellEditingState);
                                        }
                                    }

                                    if (synchronizeGridShouldScrollCurrentCell && CurrentCell.RowIndex < tableControl.Table.NestedDisplayElements.Count && CurrentCell.RowIndex != -1)
                                    {
                                        //// prevent scrolling horizontally
                                        tableControl.LeftColChanging += new GridRowColIndexChangingEventHandler(tableControl_LeftColChanging);
                                        tableControl.ScrollCellInView(CurrentCell.RowIndex, tableControl.LeftColIndex);
                                        tableControl.LeftColChanging -= new GridRowColIndexChangingEventHandler(tableControl_LeftColChanging);
                                    }

                                    if (inOnBeforePaint)
                                    {
                                        //// Restore GridNestedTableControlCellRenderer states when this was called from within EndUpdate(),
                                        //// e.g. from EndResizingCells which was called from GridNestedTableControlCellRenderer.MouseUp.
                                        //// If we don't restore context GridNestedTableControlCellRenderer.MouseUp would operate on wrong
                                        //// context.
                                        while (restoreStack.Count > 0)
                                        {
                                            ((IDisposable)restoreStack.Pop()).Dispose();
                                        }
                                    }
                                }

                                /*
                                {
                                                                                         if (row >= 0)
                                                                                         {
                                                                                                            int col = Math.Max(0, tableControl.CurrentCell.ColIndex);
                                                                                                            while (col < tableControl.Model.ColCount && !tableControl.GetViewStyleInfo(row, col).Enabled)
                                                                                                                               col++;

                                                                                                            if (tableControl.GetViewStyleInfo(row, col).Enabled)
                                                                                                                               tableControl.CurrentCell.MoveTo(row, col);
                                                                                                            else
                                                                                                                               tableControl.CurrentCell.ResetCurrentCellWithoutDeactivate();

                                                                                                            if (tableControl.CurrentCell.HasCurrentCell && tableControl.CurrentCell.RowIndex < tableControl.Table.DisplayElements.Count)
                                                                                                            {
                                                                                                                               //int row = Table.DisplayElements.IndexOf(tableCurrentElement);
                                                                                                                               if (tableCurrentElement is NestedTable)
                                                                                                                               {
                                                                                                            rend.SwitchNestedTableAndRestore(nestedTable, row, tableControl.CurrentCell.ColIndex);
                                                                                                            nestedTableControl.SynchronizeCurrentCellWithRecord(tableCurrentElement, synchronizeGridShouldScrollCurrentCell);
                                                                                                                               }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                               Console.WriteLine("Could not activate current cell.");
                                                                                                                               tableCurrentElement.ParentTable.CurrentRecordManager.Reset();
                                                                                                                               tableCurrentElement = null;

                                                                                                            }
                                                                                         }
                                                                      }
                                                                      if (childTable != null && !childTable.IsDisposed)
                                                                      {
                                                                                         tableCurrentElement = childTable.ParentTable.CurrentElement;
                                                                                         tableControl = nestedTableControl;
                                                                      }
                                */
                                ////this.synchronizeGridShouldScrollCurrentCell &= !inOnBeforePaint;
                                ////this.GetNestedCurrentCell().ScrollInView(GridScrollCurrentCellReason.SynchronizeRecord);

                                ////this.SynchronizeCurrentCellWithRecord(tableCurrentElement);
                            }
                        }
                        else if (Table.SourceList != null && Table.SourceList.Count > 0)
                        {
                            //// avoids blanking out the display when no record is selected after initial display
                            //// and user click on a sort column header ...
                            ////this.SynchronizeCurrentCellWithRecord(Table.TopLevelGroup.GetFirstRecord());
                        }

                        UpdateNavigationBar();
                        this.UpdateScrollBars();
                        currentCellColumnDescriptor = null;
                        anyAction = true;
                        if (anyAction)
                        {
                        }

                        inSynchronizeGridShouldRestoreCurrentCell = false;
                    }

#if DEBUG
                    if (traceSynchronizeGridWithEngine)
                    {
                        Trace.Unindent();
                    }
#endif

                    //// CurrentCell.warnDeactivate = true;
                    synchronizeGridShouldInvalidate = false;
                    synchronizeGridShouldRestoreCurrentCell = false;
                    synchronizeGridShouldUpdateColumnWidths = false;
                    synchronizeGridShouldnextUpdateScrollBars = false;
                    synchronizeGridShouldScrollCurrentCell = false;
                }
                finally
                {
                    inSynchronizeGridWithEngine = false;
                }
#if DEBUG
                if (traceSynchronizeGridWithEngine && anyAction && HasTable)
                {
                    TraceUtil.TraceCurrentMethodInfo("Return", this, CurrentCell, Table.CurrentElement);
                    TraceUtil.TraceCalledFrom(10);
                }
#endif
            }
            catch (Exception ex)
            {
                //// something went wrong and I don't know if the engine is in good shape now.
                //// The safest is to reinitialize all counters and have the current cell
                //// and current record be reset.
                TraceUtil.TraceExceptionCatched(ex);
                Reinitialize();
            }

            if(Table != null)
                Table.RepaintElementsInQueue();
        }

        void tableControl_LeftColChanging(object sender, GridRowColIndexChangingEventArgs e)
        {
            //// see synchronizeGridShouldScrollCurrentCell above - prevents scrolling horizontally
            e.Cancel = true;
        }

        bool inSynchronizeGridShouldRestoreCurrentCell = false;

        /// <override/>
        protected override void OnVScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
        {
            if (inSynchronizeGridShouldRestoreCurrentCell)
            {
                if (!this.synchronizeGridShouldScrollCurrentCell)
                {
                    e.Cancel = true;
                }
            }

            base.OnVScrollPixelPosChanging(e);

            if (!e.Cancel && this.VScrollPixel && this.TableDescriptor.Relations.NestedCount > 0 && this.InternalGetFrozenCols() >= TableDescriptor.GetColumnIndentCount())
            {
                GridTableControl tc = this; ////(GridTableControl) sender;
                for (int n = 0; n <= tc.ViewLayout.VisibleRows; n++)
                {
                    int rowIndex = tc.GetRow(n);
                    if (rowIndex < tc.Table.DisplayElements.Count)
                    {
                        Element el = tc.Table.DisplayElements[rowIndex];
                        if (el is NestedTable)
                        {
                            if (this.ShouldInvalidatedWhenScrolled((NestedTable)el))
                            {
                                tc.InvalidateElement(el);
                            }

                            n += el.GetVisibleCount() - 1;
                        }
                    }
                }
            }
        }

        /// <override/>
        protected override void OnTopRowChanging(GridRowColIndexChangingEventArgs e)
        {
            if (inSynchronizeGridShouldRestoreCurrentCell)
            {
                if (!this.synchronizeGridShouldScrollCurrentCell)
                {
                    e.Cancel = true;
                }
            }

            base.OnTopRowChanging(e);
        }

        #endregion

        #region WndProc and Idle
        bool inWndProc = false;
        bool allowRaiseDelayUpdateTime = true;

        //// <override/>
        ////[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal const int WM_NCPAINT = 133; // 0x0085
        internal const int WM_PAINT = 15; // 0x000f
        internal const int WM_KEYDOWN = 256; // 0x0100
        internal const int WM_MOUSEMOVE = 512; // 0x0200
        internal const int WM_MOUSEHOVER = 673; // 0x02a1
        internal const int WM_KILLFOCUS = 8; // 0x0008

        internal const int WM_MOUSEFIRST = 512; // 0x0200
        internal const int WM_MOUSELAST = 522; // 0x020a
        internal const int WM_KEYFIRST = 256; // 0x0100
        internal const int WM_KEYLAST = 264; // 0x0108
        internal const int WM_CUSTOM = 0xFB83;  // custom message to force delayed SynchronizeGridWithEngine
        internal const int WM_SYSKEYDOWN = 260; // 0x0104
        internal const int WM_SYSKEYUP = 261; // 0x0105
        const int WM_GETOBJECT = 0x003D;
        const int WM_Destroy = 0x0002;
        int sysKeyUpTick = int.MinValue;
        bool sysKeyUpTickSet = false;

        /// <override/>
        protected override void WndProc(ref Message msg)
        {
            bool oldAllowRaiseDelayUpdateTime = allowRaiseDelayUpdateTime;

            ////Trace.WriteLine(msg.ToString());
            if (!this.ParentDesignMode && !inWndProc)
            {
                if ((msg.Msg >= WM_MOUSEFIRST && msg.Msg <= WM_MOUSELAST)
                                   || (msg.Msg >= WM_KEYFIRST && msg.Msg <= WM_KEYLAST)
                                   || msg.Msg == WM_MOUSEHOVER
                                   || msg.Msg == WM_CUSTOM)
                {
                    this.SynchronizeGridWithEngine();
                    allowRaiseDelayUpdateTime = false;
                }
            }

            bool oldInWndProc = inWndProc;
            inWndProc = true;

            switch (msg.Msg)
            {
#if SyncfusionFramework4_0
                case WM_GETOBJECT:
                    if (this.GroupingControl.AccessibilityEnabled)
                    {
                        msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                        return;
                    }
                    else
                        goto default;
#elif SyncfusionFramework3_5
                      case WM_GETOBJECT:
                    if (this.GroupingControl.AccessibilityEnabled)
                    {
                        msg.Result = AutomationInteropProvider.ReturnRawElementProvider(Handle, msg.WParam, msg.LParam, this.Provider);
                        return;
                    }
                    else
                        goto default;
#endif
                case WM_PAINT:
                    ////TraceUtil.TraceCurrentMethodInfo(msg, this.ParentDesignMode, this.UpdateOptions, this.ShouldPrepareUpdate(false));
                    ////if (!this.ParentDesignMode && !inSynchronizeGridWithEngine)
                    ////                   this.SynchronizeGridWithEngine();
                    base.WndProc(ref msg);
                    break;
                case WM_SYSKEYDOWN:
                    sysKeyUpTick = int.MinValue;
                    sysKeyUpTickSet = false;
                    base.WndProc(ref msg);
                    break;

                case WM_SYSKEYUP:
                    sysKeyUpTick = Environment.TickCount;
                    sysKeyUpTickSet = true;
                    base.WndProc(ref msg);
                    break;

                case WM_KILLFOCUS:
                    if (sysKeyUpTickSet && sysKeyUpTick - Environment.TickCount < 200)
                    {
                        GridCurrentCell cc = this.GetNestedCurrentCell();
                        if (cc != null && cc.IsDroppedDown && cc.Renderer != null)
                        {
                            cc.CloseDropDown(Syncfusion.Windows.Forms.PopupCloseType.Deactivated);
                        }
                    }

                    sysKeyUpTick = int.MinValue;
                    sysKeyUpTickSet = false;
                    base.WndProc(ref msg);
                    break;
                default:
                    base.WndProc(ref msg);
                    break;
            }
            ////
            inWndProc = oldInWndProc;
            ////if (!this.ParentDesignMode && !inWndProc)
            ////                   SynchronizeGridWithEngine();
            if (!this.ParentDesignMode && !inWndProc && !IsDisposed)
            {
                if ((msg.Msg >= WM_MOUSEFIRST && msg.Msg <= WM_MOUSELAST)
                                   || (msg.Msg >= WM_KEYFIRST && msg.Msg <= WM_KEYLAST)
                                   || msg.Msg == WM_MOUSEHOVER)
                {
                    this.SynchronizeGridWithEngine();
                }
            }

            allowRaiseDelayUpdateTime = oldAllowRaiseDelayUpdateTime;
        }

        internal bool ParentDesignMode
        {
            get
            {
#if EMUDESIGN
                                                                            return true;
#endif
                Control parent = Parent;
                while (parent != null)
                {
                    ISite site = Parent.Site;
                    if (site != null)
                    {
                        return site.DesignMode;
                    }

                    if (parent is GridGroupingControl)
                    {
                        return ((GridGroupingControl)parent).InDesigner;
                    }

                    parent = parent.Parent;
                }

                return DesignMode;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (this.Table != null && this.Table.IsDisposed)
            {
                throw new InvalidOperationException("The GridTable for this control has been disposed. Check for any references to this control (e.g. subscribed events) that have not been released.");
            }
            //// Note: if this exception is about a nested table control or drop-down table list,
            //// check if Relations_Changed is handled correctly. Those controls must unwire themselves
            //// when they become obsolete.

            if (!inWndProc)
            {
                SynchronizeGridWithEngine();
            }
        }
        #endregion
        #region Refresh, Focus and Windowsless support
        /// <override/>
        protected override void Refresh(bool fromModel)
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom();
            }
#endif
            ////TraceUtil.TraceCurrentMethodInfo();
            ////TraceUtil.TraceCalledFrom(8);
#if DEBUG
            Trace.WriteLineIf(Switches.GroupingGrid.TraceVerbose, String.Format("Refresh({0})", fromModel));
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);
#endif

            if (!this.initialized || this.inSynchronizeGridWithEngine || this.inRefresh || this.inPaint
                                                                            || Updating || ((GridTableControl)GetWindow()).Updating)
            {
                synchronizeGridShouldInvalidate = true;
                synchronizeGridShouldnextUpdateScrollBars = true;
                return;
            }

            synchronizeGridShouldInvalidate = true;
            synchronizeGridShouldnextUpdateScrollBars = true;
            ////synchronizeGridShouldUpdateColumnWidths = true;
            SynchronizeGridWithEngine();
            base.Refresh(fromModel);
        }

        /// <override/>
        protected override void OnWindowScrolling(ScrollWindowEventArgs e)
        {
            //// GridCurrentCell cc = this.GetNestedCurrentCell();
            //// if (cc.Renderer != null)
            //// {
            //// Control c = cc.Renderer.Control;
            //// if (c != null && c.Focused)
            //// {
            //// Rectangle r = c.Bounds;
            //// }
            //// }
            base.OnWindowScrolling(e);

            //// Fixes issue with scrolling of nested tables when customer did manually set Model.Cols.FrozenCount on parent table.
            //// if (e.XAmount != 0 && this.TableDescriptor.Relations.NestedCount > 0 && Model.Cols.FrozenCount >= TableDescriptor.GetColumnIndentCount())
            if (e.XAmount != 0 && this.TableDescriptor.Relations.NestedCount > 0 && this.InternalGetFrozenCols() >= TableDescriptor.GetColumnIndentCount())
            {
                GridTableControl tc = this; ////(GridTableControl) sender;
                for (int n = 0; n <= tc.ViewLayout.VisibleRows; n++)
                {
                    int rowIndex = tc.GetRow(n);
                    if (rowIndex < tc.Table.DisplayElements.Count)
                    {
                        Element el = tc.Table.DisplayElements[rowIndex];
                        if (el is NestedTable)
                        {
                            //// Console.WriteLine(el.ToString());
                            if (this.ShouldInvalidatedWhenScrolled((NestedTable)el))
                            {
                                tc.InvalidateElement(el);
                            }

                            n += el.GetVisibleCount() - 1;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determine the hscroll position changing.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHScrollPixelPosChanging(GridScrollPositionChangingEventArgs e)
        {
            base.OnHScrollPixelPosChanging(e);

            if (!e.Cancel && this.HScrollPixel && this.TableDescriptor.Relations.NestedCount > 0 && this.InternalGetFrozenCols() >= TableDescriptor.GetColumnIndentCount())
            {
                GridTableControl tc = this;
                for(int n = tc.TopRowIndex; n <= tc.ViewLayout.LastVisibleRow; n++)
                {
                    int rowIndex = n;
                    if (rowIndex < tc.Table.DisplayElements.Count)
                    {
                        Element el = tc.Table.DisplayElements[rowIndex];
                        if (el is NestedTable)
                        {
                            if (this.ShouldInvalidatedWhenScrolled((NestedTable)el))
                            {
                                tc.InvalidateElement(el);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determine the hscroll pixel
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHorizontalScroll(ScrollEventArgs e)
        {
            base.OnHorizontalScroll(e);

            if (!this.HScrollPixel && this.TableDescriptor.Relations.NestedCount > 0 && this.InternalGetFrozenCols() >= TableDescriptor.GetColumnIndentCount())
            {
                GridTableControl tc = this;
                for (int n = tc.TopRowIndex; n <= tc.ViewLayout.LastVisibleRow; n++)
                {
                    int rowIndex = n;
                    if (rowIndex < tc.Table.DisplayElements.Count)
                    {
                        Element el = tc.Table.DisplayElements[rowIndex];
                        if (el is NestedTable)
                        {
                            if (this.ShouldInvalidatedWhenScrolled((NestedTable)el))
                            {
                                tc.InvalidateElement(el);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Determine the vertical scroll
        /// </summary>
        /// <param name="e"></param>
        protected override void OnVerticalScroll(ScrollEventArgs e)
        {
            base.OnVerticalScroll(e);

            if (!this.VScrollPixel && this.TableDescriptor.Relations.NestedCount > 0 && this.InternalGetFrozenCols() >= TableDescriptor.GetColumnIndentCount())
            {
                GridTableControl tc = this;
                for(int n = tc.TopRowIndex ;n <= tc.ViewLayout.LastVisibleRow;n++)
                {
                    int rowIndex = n;
                    if (rowIndex < tc.Table.DisplayElements.Count)
                    {
                        Element el = tc.Table.DisplayElements[rowIndex];
                        if (el is NestedTable)
                        {
                            if (this.ShouldInvalidatedWhenScrolled((NestedTable)el))
                            {
                                tc.InvalidateElement(el);
                            }
                        }
                    }
                }
            }
        }

        /// <override/>
        /// <summary>Called to scroll the specified cell into view.</summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="dontScroll">Specified if top row index and left column index should be changed
        /// without raising scroll events and without updating the screen.</param>
        /// <param name="reason">The reason for scrolling the current cell into view.</param>
        /// <returns>True if the operation is successful.</returns>
        public override bool ProcessScrollCellInView(int rowIndex, int colIndex, bool dontScroll, GridScrollCurrentCellReason reason)
        {
            this.VScrollPixel &= Table.TableOptions.VerticalPixelScroll;
            // Note: colIndex == 0 means "don't scroll horizontally", see GridNestedTableControl.ProcessScrollCellInView
            if (colIndex > 0 && colIndex < this.TableDescriptor.GetColumnIndentCount())
            {
                colIndex = this.InternalGetFrozenCols() + 1;
            }

            return base.ProcessScrollCellInView(rowIndex, colIndex, dontScroll, reason);
        }

        /// <override/>
        /// <summary>Sets input focus to the control.</summary>
        /// <returns>True if the control is focused.</returns>
        public override bool Focus()
        {
            try
            {
                this.SynchronizeGridWithEngine();
                return base.Focus();
            }
            finally
            {
            }
        }

        /// <override/>
        /// <summary>Updates the control.</summary>
        public override void Update()
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(10);
            }
#endif
            ////  TraceUtil.TraceCurrentMethodInfo();
            //// TraceUtil.TraceCalledFrom(8);

            //// TODO: In case there is a drawing glitch that happens when scrollbars change
            //// in UpdateScrollBars, review this if-statement below.
            if (!this.initialized || this.inSynchronizeGridWithEngine || this.inRefresh || this.inPaint
                               || Updating || ((GridTableControl)GetWindow()).Updating)
            {
                this.recalcScrollBars = ScrollBars.None;
                return;
            }

            this.SynchronizeGridWithEngine();
            ////if (Table.TableDescriptor.Summaries.Count > 0)
            if (this.HasTable && this.Table.SummariesDirty)
            {
                this.Table.TopLevelGroup.GetSummaries(Table);
            }

            base.Update();

            ////synchronizeGridShouldInvalidate = true;
            ////  if (!this.inPaint)// && !synchronizeGridShouldInvalidate)// && !this.refreshGridNextIdle)
            ////  base.Update();
        }

        /// <override/>
        /// <summary>Invalidates the control.</summary>
        public override void Invalidate()
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    this.InvalidBounds,
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                TraceUtil.TraceCalledFrom(1);
            }

            Trace.WriteLineIf(Switches.GroupingGrid.TraceVerbose, "Invalidate()");
#endif
            KillDelayUpdateTimer();
            if (this.GroupingControl != null)
            {
                this.GroupingControl.SetInvalidated();
            }

            base.Invalidate();
        }

        /// <override/>
        /// <summary>Invalidates the specified area of the control.</summary>
        /// <param name="rc">Area to invalidate.</param>
        public override void Invalidate(Rectangle rc)
        {
            if (rc.Width == 0 || rc.Height == 0)
            {
                return;
            }

#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    rc,
                    this.InvalidBounds,
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                ////TraceUtil.TraceCalledFrom(4);
            }
#endif
            KillDelayUpdateTimer();
            if (rc.Width > 0 && rc.Height > 0 && rc.IntersectsWith(GridBounds))
            {
                base.Invalidate(rc);
            }
        }

        bool inOnBeforePaint = false;

        /// <override/>
        protected override void OnBeforePaint(EventArgs e)
        {
            inOnBeforePaint = true;
            ////SR1
            if (this.ParentDesignMode)
            {
                ViewLayout.Reset();
            }

            if (!this.IsDisposed && !inSynchronizeGridWithEngine)
            {
                this.SynchronizeGridWithEngine();
            }

            FixTopLeftRowCol();
            inOnBeforePaint = false;
        }

        /// <override/>
        /// <summary>Invalidates the specified region of the control.</summary>
        /// <param name="rc">Area to invalidate.</param>
        /// <param name="invalidateChildren">Specifies if child controls should also be invalidated.</param>
        public override void Invalidate(Rectangle rc, bool invalidateChildren)
        {
            if (rc.Width == 0 || rc.Height == 0)
            {
                return;
            }

#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    rc,
                    this.InvalidBounds,
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                    CurrentCell.IsInMoveTo,
                    "IsInActiveOrDeactivate",
                    CurrentCell.IsInActiveOrDeactivate,
                    "inProcessKeyEventArgs",
                    inProcessKeyEventArgs,
                    "Updating",
                    Updating || ((GridTableControl)GetWindow()).Updating);
                ////TraceUtil.TraceCalledFrom(4);
            }
#endif
            KillDelayUpdateTimer();
            base.Invalidate(rc);
        }
        #endregion
        #region Cell Tip
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                KillDelayUpdateTimer();

                if (this.cellToolTip1 != null)
                {
                    this.cellToolTip1.RemoveAll();
                    this.cellToolTip1.Dispose();
                    this.cellToolTip1 = null;
                }

                //// if (onCurrentRecordContextChange != null)
                //// onCurrentRecordContextChange = null;
                Application.Idle -= new EventHandler(Application_Idle);
                if (parentForm != null)
                {
                    parentForm.Closed -= new EventHandler(parentForm_Closed);
                    parentForm = null;
                }

                if (this.synchronizeGridNavigateToBookmark != null)
                {
                    this.synchronizeGridNavigateToBookmark.Disposed -= new EventHandler(synchronizeGridNavigateToBookmark_Disposed);
                    this.synchronizeGridNavigateToBookmark = null;
                }

                if (this.synchronizeGridNavigateTo != null)
                {
                    this.synchronizeGridNavigateTo.Disposed -= new EventHandler(synchronizeGridNavigateTo_Disposed);
                    this.synchronizeGridNavigateTo = null;
                }

                MouseOperationChildTable = null;
                currentCellColumnDescriptor = null;
                ResetSynchronizeGridNavigateTo();
                currentCellMoveToElement = null;
                //// onCurrentRecordContextChange = null;
                if (paintSelectCells != null)
                {
                    paintSelectCells.Dispose();
                    paintSelectCells = null;
                }
                //// tableDescriptor = null;
            }
        }

        private ToolTip cellToolTip1 = null;
        private int tipRow = -1;
        internal int tipCol = -1;

        /// <summary>
        /// Returns a reference to the ToolTip object used for displaying
        /// <see cref="GridStyleInfo.CellTipText"/> as ToolTips for cells.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolTip CellToolTip
        {
            get
            {
                if (cellToolTip1 == null)
                {
                    cellToolTip1 = CreateCellToolTip();
                    cellToolTip1.Disposed += new EventHandler(cellToolTip1_Disposed);
                    if (parentForm != null)
                    {
                        parentForm.Closed -= new EventHandler(parentForm_Closed);
                    }

                    parentForm = FindFormHelper.FindForm(this);
                    if (parentForm != null)
                    {
                        parentForm.Closed += new EventHandler(parentForm_Closed);
                    }
                }

                return cellToolTip1;
            }

            set
            {
                if (cellToolTip1 != value)
                {
                    if (cellToolTip1 != null)
                    {
                        cellToolTip1.RemoveAll();
                        cellToolTip1.Dispose();
                    }

                    cellToolTip1 = value;
                    cellToolTip1.Disposed += new EventHandler(cellToolTip1_Disposed);
                }
            }
        }

        Form parentForm;

        /// <summary>
        /// Creates and initializes a ToolTip object. InitialDelay will be 500 and
        /// ReshowDelay will be set to 0 by default.
        /// </summary>
        /// <returns>The initialized ToolTip object for this grid.</returns>
        protected virtual ToolTip CreateCellToolTip()
        {
            ToolTip toolTip = new ToolTip();
            toolTip.InitialDelay = 100; ////half a second delay
            toolTip.ReshowDelay = 0;
            return toolTip;
        }

        Element tipElement = null;

        /// <summary>
        /// Occurs when mouse position leaves from the client area of the <see cref="GridTableControl"/>.
        /// </summary>
        /// <param name="e">event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            Point pt = this.PointToClient(Control.MousePosition);
            if (pt.X <= -1 || pt.Y <= -1)
            {
                tipElement = null;
            }
            base.OnMouseLeave(e);
        }

        internal void OnCellTipsMouseMove(MouseEventArgs e)
        {
            int row, col;
            Point pt = new Point(e.X, e.Y);
            if (PointToRowCol(new Point(e.X, e.Y), out row, out col))
            {
                CurrentCell.AdjustRowColIfCoveredCell(ref row, ref col);
            }

            if (IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null)
            {
                row = -1;
            }

            Element mouseDisplayElement = this.GetTableControlWindow().PointToNestedDisplayElement(pt);
            if (col != tipCol || row != tipRow || mouseDisplayElement != tipElement)
            {
                GridTableCellStyleInfo style = GetTableViewStyleInfo(row, col);
                tipElement = mouseDisplayElement;
                ToolTip toolTip = ((GridTableControl)this.GetGridWindow()).CellToolTip;
                tipCol = col;
                tipRow = row;
                if (toolTip != null && toolTip.Active)
                {
                    toolTip.Active = false; ////turn it off
                }

                if (row == -1)
                {
                    return;
                }

                ////TraceUtil.TraceCurrentMethodInfo(row, col, style.CellTipText);
                if (style.TableCellIdentity == null)
                {
                    return;
                }

                if (style.TableCellIdentity.TableCellType == GridTableCellType.NestedTableCell)
                {
                    NestedTable nt = style.TableCellIdentity.DisplayElement as NestedTable;
                    if (nt != null)
                    {
                        GridTableControl nestedControl = GroupingControl.GetTableControl(nt.ChildTable.Name);
                        if (nestedControl != null)
                        {
                            nestedControl.tipCol = -1;
                        }
                    }

                    return;
                }

                GridActivateToolTipEventArgs ae = new GridActivateToolTipEventArgs(row, col, style);
                OnActivateToolTip(ae);
                if (!ae.Cancel && !GridUtil.IsEmpty(style.CellTipText))
                {
                    toolTip.SetToolTip(GetGridWindow(), style.CellTipText);
                    toolTip.Active = true; ////make it active so it can show
                    if (((GridTableControl)this.GetGridWindow()).CellToolTip != null)
                    {
                        ((GridTableControl)this.GetGridWindow()).CellToolTip.RemoveAll();
                        ((GridTableControl)this.GetGridWindow()).CellToolTip.Dispose();
                        ((GridTableControl)this.GetGridWindow()).CellToolTip.Show(style.CellTipText, this.GetGridWindow(), e.Location, toolTip.AutomaticDelay);
                        ((GridTableControl)this.GetGridWindow()).CellToolTip.AutomaticDelay = toolTip.AutomaticDelay;
                        ((GridTableControl)this.GetGridWindow()).CellToolTip.AutoPopDelay = toolTip.AutoPopDelay;
                    }
                }
                style.Dispose();
            }
        }

        /// <summary>
        /// Occurs when mouse has moved to a new cell and a ToolTip is initialized for that cell.
        /// </summary>
        /// <remarks>
        /// You have two options: <para/>
        /// 1) Set e.Style.CellTipText<para/>
        /// - Or - <para/>
        /// 2) Initalize the ToolTip directly (see <see cref="CellToolTip"/> property) and then
        /// set e.Cancel = True;<para/>
        /// </remarks>
        public event GridActivateToolTipEventHandler ActivateToolTip;

        /// <summary>
        /// Raises the <see cref="ActivateToolTip"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridActivateToolTipEventArgs" /> that contains the event data.</param>
        protected virtual void OnActivateToolTip(GridActivateToolTipEventArgs e)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.GridControlBaseEvents.TraceVerbose, Name, e);
            if (ActivateToolTip != null)
            {
                ActivateToolTip(this, e);
            }
        }

        /// <override/>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            InMouseMove = true;
            OnCellTipsMouseMove(e);
            base.OnMouseMove(e);
            InMouseMove = false;
        }

        private void cellToolTip1_Disposed(object sender, EventArgs e)
        {
            cellToolTip1.Disposed -= new EventHandler(cellToolTip1_Disposed);
            this.cellToolTip1 = null;
        }

        private void parentForm_Closed(object sender, EventArgs e)
        {
            if (this.cellToolTip1 != null)
            {
                this.cellToolTip1.RemoveAll();
                this.cellToolTip1.Dispose();
            }

            this.cellToolTip1 = null;
            Form parentForm = FindFormHelper.FindForm(this);
            if (parentForm != null)
            {
                parentForm.Closed -= new EventHandler(parentForm_Closed);
            }
        }

        #endregion
        #region GetTableViewStyleInfo
        /// <summary>
        /// This virtual method is called from GetViewStyleInfo. It creates a temporary GridTableCellStyleInfo
        /// object and raises the <see cref="GridControlBase.PrepareViewStyleInfo"/> event to allow custom formatting of
        /// a cell by changing its style object just before it is drawn.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>The style object.</returns>
        public virtual GridTableCellStyleInfo GetTableViewStyleInfo(int rowIndex, int colIndex)
        {
            GridTableCellStyleInfo style = Model.Table.GetTableCellStyle(rowIndex, colIndex);
            GridTableCellStyleInfoIdentity identity = (GridTableCellStyleInfoIdentity)style.Identity;
            ////Debug.Assert(style.Identity != null);
            if (identity == null)
            {
                return style;
            }

            GridTableCellStyleInfo viewStyle = new GridTableCellStyleInfo(new GridTableCellViewStyleInfoIdentity(style, identity));
            RaisePrepareViewStyleInfo(rowIndex, colIndex, viewStyle);
            return viewStyle;
        }

        /// <summary>
        /// Queries cell information that includes custom formatting with respect to current view state.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="forceQueryCellInfo">For the QueryCellInfo to be called and not cache the styles.</param>
        /// <returns>
        /// The <see cref="T:Syncfusion.Windows.Forms.Grid.GridStyleInfo"/> object that holds cell information.
        /// </returns>
        /// <override/>
        public override sealed GridStyleInfo GetViewStyleInfo(int rowIndex, int colIndex, bool forceQueryCellInfo)
        {
            return GetTableViewStyleInfo(rowIndex, colIndex);
        }

        #endregion
        #region Resizing Overrides
        /// <override/>
        protected override void OnResizingRows(GridResizingRowsEventArgs e)
        {
            base.OnResizingRows(e);

            if (e.Cancel || !HasTable)
            {
                return;
            }

            if (e.Reason == GridResizeCellsReason.HitTest)
            {
                Point p = e.Point; ////PointToClient(pt);
                GridRangeInfo rg = PointToRangeInfo(p);
                if (rg.IsEmpty || rg.Top >= Model.GetDisplayElementCount())
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = !Object.ReferenceEquals(Model.GetDisplayElementAt(rg.Top).ParentTable, this.Table);
                }
            }
            else if (e.Reason == GridResizeCellsReason.DoubleClick)
            {
                Model.RowHeights.ResizeToFit(GridRangeInfo.Row(e.Rows.Top), GridResizeToFitOptions.IncludeCellsWithinCoveredRange | GridResizeToFitOptions.ResizeCoveredCells);
                e.Cancel = true;
            }
        }

        /// <override/>
        protected override void OnResizingColumns(GridResizingColumnsEventArgs e)
        {
            base.OnResizingColumns(e);

            if (e.Cancel || !HasTable)
            {
                return;
            }

            if (e.Reason == GridResizeCellsReason.HitTest)
            {
                Point p = e.Point; ////PointToClient(pt);
                GridRangeInfo rg = PointToRangeInfo(p);
                if (rg.IsEmpty || (Table.DisplayElements[rg.Top].Kind != DisplayElementKind.ColumnHeader && !this.AllowColumnResizeUsingCellBoundaries))
                {
                    e.Cancel = true;
                }
            }
            else if (e.Reason == GridResizeCellsReason.DoubleClick)
            {
                int[] widths = new int[e.Columns.Width];
               
                int n = 0;
                int left = e.Columns.Left;
                int right = e.Columns.Right;
                for (int colIndex = e.Columns.Left; colIndex <= e.Columns.Right; colIndex++)
                {
                    widths[n] = -1;
                    GridColumnDescriptor column = Model.GetHeaderColumnDescriptorAt(colIndex);
                    if (colIndex < Model.ColCount && left == right)
                    {
                        GridColumnDescriptor column1 = Model.GetHeaderColumnDescriptorAt(colIndex + 1);
                        if (column1 != null && column1.Width == 0)
                        {
                            column = column1;
                            colIndex++;
                            left++;
                            right++;
                        }
                    }

                    if (column != null)
                    {
                        using (Graphics g = GetWindow().CreateGraphics())
                        {
                            widths[n] = Table.GetPreferredColumnWidth(g, column);
                        }
                       
                        if (GetFilterCellModel())
                        {
                            if (wasEnquired)
                                widths[n] += 15;
                        }

                        if (column.isImageApplied )
                            widths[n] += 20;
                        if(column.HeaderImageAlignment == HeaderImageAlignment.Right && (this.TableDescriptor.SortedColumns.Contains(column.Name)||this.TableDescriptor.GroupedColumns.Contains(column.Name)))
                            widths[n] += 20;
                    }

                    n++;
                }

                if (!this.UnHideColsOnDblClick && (this.Model.HideCols[e.Columns.Left] || this.Model.ColWidths[e.Columns.Left + 1] == 0))
                {
                    e.Cancel = true;
                    return;
                }
                e.Cancel = true;
                Model.ColWidths.SetRange(left, right, widths, true);
            }
        }

       
        private bool GetFilterCellModel()
        {
            foreach (GridCellModelBase cellModel in this.Model.CellModels.Values)
            {
                if (cellModel.ToString().Contains("GridExcelFilterCellModel") || cellModel.ToString().Contains("Grid2007ExcelFilterCellModel"))
                {
                    wasEnquired = true;
                    return true;
                }
            }
            wasEnquired = false;
            return false;

        }

        #endregion 
        #region Current Cell Helper Methods
        void SaveCurrentCellStateBeforeSorting()
        {
            object r = Table.CurrentElement;
            if (CurrentCell.IsEditing)
            {
                if (CurrentCell.IsModified)
                {
                    CurrentCell.ConfirmChanges(true);
                }

                CurrentCell.ResetError();
                if (CurrentCell.IsModified)
                {
                    CurrentCell.RejectChanges();
                }

                CurrentCell.EndEdit();
            }
        }

        internal bool DeactivateCurrentCell(bool allowCancel)
        {
            if (CurrentCell.HasCurrentCell)
            {
                if (CurrentCell.RowIndex >= Table.DisplayElements.Count)
                {
                    CurrentCell.ResetCurrentCellWithoutDeactivate();
                    return true;
                }

                currentCellIsEditing = CurrentCell.IsEditing;
                if (inRefresh || Table.InInitialize || Table.SummariesDirty || Table.TableDirty || Table.CountersDirty)
                {
                    if (Table.DisplayElements[CurrentCell.RowIndex] is NestedTable)
                    {
                        foreach (GridTable relatedTable in Table.RelatedTables)
                        {
                            string cellType = "RT" + relatedTable.TableDescriptor.Name;
                            if (this.CellRenderers.ContainsKey(cellType))
                            {
                                GridNestedTableControlCellRenderer nestedTableRenderer = this.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
                                if (nestedTableRenderer != null)
                                {
                                    nestedTableRenderer.OnDeactivateCurrentCell(false);
                                }
                            }
                        }
                    }

                    CurrentCell.ResetCurrentCellWithoutDeactivate();
                }
                else
                {
                    if (CurrentCell.Renderer != null)
                    {
                        currentCellEditingState = CurrentCell.Renderer.GetEditState();
                    }

                    currentCellColumnDescriptor = Model.GetColumnDescriptorAt(CurrentCell.RowIndex, CurrentCell.ColIndex);
                    if (CurrentCell.IsEditing)
                    {
                        CurrentCell.EndEdit();
                    }

                    if (Table.DisplayElements[CurrentCell.RowIndex] is NestedTable)
                    {
                        foreach (GridTable relatedTable in Table.RelatedTables)
                        {
                            if (relatedTable.CurrentElement != null)
                            {
                                string cellType = "RT" + relatedTable.TableDescriptor.Name;
                                if (this.CellRenderers.ContainsKey(cellType))
                                {
                                    GridNestedTableControlCellRenderer nestedTableRenderer = this.CellRenderers[cellType] as GridNestedTableControlCellRenderer;
                                    if (nestedTableRenderer != null)
                                    {
                                        if (!nestedTableRenderer.OnDeactivateCurrentCell(false))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return !CurrentCell.Deactivate(allowCancel);
                }
            }

            return true;
        }

        void InvalidateCurrentRecord()
        {
#if DEBUG
            if (traceSynchronizeGridWithEngine)
            {
                TraceUtil.TraceCurrentMethodInfo(
                    "inPaint",
                    inPaint,
                    "inSynchronizeGridWithEngine",
                    inSynchronizeGridWithEngine,
                    "IsInMoveTo",
                     CurrentCell.IsInMoveTo,
                     "IsInActiveOrDeactivate",
                     CurrentCell.IsInActiveOrDeactivate,
                     "inProcessKeyEventArgs",
                     inProcessKeyEventArgs,
                     "Updating",
                     Updating || ((GridTableControl)GetWindow()).Updating);
                ////TraceUtil.TraceCalledFrom(4);
            }
#endif
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);
            //// if (Table.CurrentRecord is AddNewRecord)
            //// this.DelayRefreshRange(GridRangeInfo.Table());
            //// else
            if (Table.CurrentRecord != null)
            {
                Rectangle bounds = this.RangeInfoToRectangle(Table.GetCurrentRecordRangeInfo(), GridRangeOptions.None);
                //// GridUtil.SetLeft(ref bounds, 0);
                Invalidate(bounds);
            }
            ////this.DelayRefresh();
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                Trace.WriteLine(String.Format("InvalidateCurrentRecord {0} {1}", Table.GetCurrentRecordRangeInfo(), Table));
            }
#endif
        }

        bool IsCurrentElementAtCurrentCell()
        {
            // If current cell is at filter bar, then this returns False.
            return HasTable && CurrentCell.HasCurrentCell && this.IsCurrent(CurrentCell.RowIndex);
        }

        internal CurrentRecordProperty GetCurrentCellRecordProperty()
        {
            // Returns NULL if current cell is a nested table.
            return GetCurrentRecordManagerPropertyAt(CurrentCell.RowIndex, CurrentCell.ColIndex);
        }

        internal CurrentRecordProperty GetCurrentRecordManagerPropertyAt(int rowIndex, int colIndex)
        {
            // Returns NULL if rowIndex is a nested table.
            GridColumnDescriptor column = Model.GetColumnDescriptorAt(rowIndex, colIndex);
            if (column != null && column.FieldDescriptor != null)
            {
                return Table.CurrentRecordManager.Properties[column.FieldDescriptor.Name];
            }

            return null;
        }

        bool CanEditCell(int rowIndex, int colIndex)
        {
            return rowIndex >= 0 && rowIndex < Table.DisplayElements.Count && !Model[rowIndex, colIndex].ReadOnly;
        }

        bool ShouldNavigateTo(Element element)
        {
            return element is RecordRow || element is CaptionRow || element is NestedTable || element is Record || element is CaptionSection;
        }

        bool ShouldNavigateTo(int rowIndex)
        {
            return HasTable && rowIndex >= 0 && rowIndex < Table.DisplayElements.Count && ShouldNavigateTo(Table.DisplayElements[rowIndex]);
        }

        bool IsCurrent(Element element)
        {
            if (element is RecordRow)
            {
                element = element.ParentRecord;
            }

            return HasTable && Table.CurrentElement == element;
        }

        bool IsCurrent(int rowIndex)
        {
            return HasTable && rowIndex >= 0 && rowIndex < Table.DisplayElements.Count && IsCurrent(Table.DisplayElements[rowIndex]);
        }

        bool ShouldNavigateTo(int rowIndex, int colIndex)
        {
            return ShouldNavigateTo(rowIndex);
        }
        #endregion

        /// <override/>
        /// <summary>Gets the background color of the grid.</summary>
        /// <returns>Grid background color.</returns>
        public override Color GetBackgroundColor()
        {
            return Table.TableDescriptor.Appearance.EmptyCell.BackColor;
        }

        /// <override/>
        /// <summary>Gets or sets the multiplier for mouse wheel scrolling.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int VScrollIncrement
        {
            get
            {
                if (this.VScrollPixel)
                {
                    return this.Table.TableOptions.RecordRowHeight;
                }

                return base.VScrollIncrement;
            }

            set
            {
                base.VScrollIncrement = value;
            }
        }

        /// <override/>
        /// <summary>Gets or sets the multiplier for the mouse wheel scrolling.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override int HScrollIncrement
        {
            get
            {
                if (this.HScrollPixel)
                {
                    return 20;
                }

                return base.HScrollIncrement;
            }

            set
            {
                base.HScrollIncrement = value;
            }
        }
        /// <summary>
        /// Gets the provider.
        /// </summary>
#if SyncfusionFramework4_0
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        public GroupingGridUIAProvider Provider
        {
            get
            {
                return new GroupingGridUIAProvider(this);
            }
        }
#elif SyncfusionFramework3_5
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
         public GroupingGridUIAProvider Provider
        {
            get
            {
                return new GroupingGridUIAProvider(this);
            }
        }
#endif
        /// <override/>
        /// <summary>Returns the pane information.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override string PaneDesc
        {
            get
            {
                return Disposing || IsDisposed ? "Dispose" : ((HasTable ? Table.ToString() + ":" : string.Empty) + base.PaneDesc);
            }
        }

        /// <summary>
        /// Marks the table dirty and forces reinitializing categories, counters, and summaries. The display is refreshed.
        /// </summary>
        public void Reinitialize()
        {
            if (Table != null)
            {
                // CTRL-SHIFT-R will call Reinitialize - emergency reset when things go out of sync ....
                Table.CurrentRecordManager.Reset();
                if (CurrentCell.Renderer != null)
                {
                    CurrentCell.Renderer.Hide();
                }

                CurrentCell.ResetCurrentCellWithoutDeactivate();
                this.InternalSetCurrentCellObject(new GridCurrentCell(this));
                Table.CurrentRecordManager.Reinititalize();
                Table.TableDescriptor.Fields.Version++;
                Table.SelectedRecords.InternalClear();
                foreach (Table relatedTable in Table.RelatedTables)
                {
                    relatedTable.TableDirty = true;
                    relatedTable.SelectedRecords.InternalClear();
                }

                Table.TableDirty = true;
            }
            Invalidate();
            this.synchronizeGridShouldInvalidate = true;
            this.synchronizeGridShouldnextUpdateScrollBars = true;
            this.synchronizeGridShouldUpdateColumnWidths = true;
            Invalidate();
        }

        Element GetDisplayElement(Element record)
        {
            return (Element)record;
        }

        /// <summary>
        /// Invalidates the display area for the specified element.
        /// </summary>
        /// <param name="element">Element to invalidate.</param>
        public void InvalidateElement(Element element)
        {
            if (this.Visible && element != null && element.GetVisibleInHierarchy())
            {
                GridTableControl tableControl = GetTableControlWindow();
                Rectangle r = ElementToRectangle(element);
                if (tableControl.ClientRectangle.IntersectsWith(r))
                {
                    tableControl.Invalidate(r);
                }
            }
        }

        /// <summary>
        /// Returns the display area for the specified record. If it has nested tables only the bounding rectangle of the parent rows are returned and nested tables are excluded.
        /// </summary>
        /// <param name="record">The grouping record</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Rectangle.Empty if record is not visible; otherwise, the display area in client coordinates</returns>
        public Rectangle RecordRowsToRectangle(Record record, GridCellSizeKind sizeKind)
        {
            Rectangle bounds;
            if (record.HasNestedTables)
            {
                int c = record.RecordRows.Count;
                bounds = ElementToRectangle(record.RecordRows[0], sizeKind);
                for (int n = 1; n < c; n++)
                {
                    bounds.Height += (int)record.RecordRows[n].GetYAmountCount();
                }
            }
            else
            {
                bounds = ElementToRectangle(record, sizeKind);
            }

            return bounds;
        }

        /// <summary>
        /// Returns the display area for the specified element.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>Rectangle.Empty if element is not visible; otherwise, the display area in client coordinates</returns>
        public Rectangle ElementToRectangle(Element element)
        {
            return ElementToRectangle(element, GridCellSizeKind.VisibleSize);
        }

        /// <summary>
        /// Returns the display area for the specified element.
        /// </summary>
        /// <param name="element">The elements</param>
        /// <param name="sizeKind">Specifies how to handle the the top-most row when
        /// pixel scrolling is enabled and cells are only partially visible.</param>
        /// <returns>Rectangle.Empty if element is not visible; otherwise, the display area in client coordinates</returns>
        public Rectangle ElementToRectangle(Element element, GridCellSizeKind sizeKind)
        {
            if (element == null)
            {
                return Rectangle.Empty;
            }

            GridTableControl tableControl = GetTableControlWindow();

            if (SupportsYAmount)
            {
                int yOffset = -1;
                if (GroupingControl != null)
                {
                    if (element is NestedTable)
                    {
                        yOffset = (int)GroupingControl.Table.DisplayElements.GetYAmountPositionOf(element);
                    }
                    else
                    {
                        yOffset = (int)GroupingControl.Table.NestedDisplayElements.GetYAmountPositionOf(element);
                    }
                }

                int yAmount = (int)element.GetYAmountCount();
                int yTop = tableControl.GetVScrollPixelMinimum();
                int yBottom = tableControl.Bottom;
                int yPos = tableControl.GetCurrentVScrollPixelPos();

                Rectangle r = Rectangle.FromLTRB(0, yOffset - yPos + yTop, tableControl.Width, yOffset + yAmount - yPos + yTop);
                if (r.Top < yTop && r.Bottom >= yTop && sizeKind == GridCellSizeKind.VisibleSize)
                {
                    GridUtil.SetTop(ref r, yTop);
                }

                return r.Height <= 0 ? Rectangle.Empty : r;
            }
            else
            {
                int row1 = -1;
                if (GroupingControl != null)
                {
                    if (element is NestedTable)
                    {
                        row1 = (int)GroupingControl.Table.DisplayElements.IndexOf(element);
                    }
                    else
                    {
                        row1 = (int)GroupingControl.Table.NestedDisplayElements.IndexOf(element);
                    }
                }
                int rowCount = (int)element.GetVisibleCount();

                Rectangle r = tableControl.RangeInfoToRectangle(GridRangeInfo.Rows(row1, row1 + rowCount - 1));
                return r.Height <= 0 ? Rectangle.Empty : r;
            }
        }

        /// <summary>
        /// Determines if the specified element is visible and scrolled into view
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>True if it is visible.</returns>
        public bool IsElementVisible(Element element)
        {
            if (element != null && element.GetVisibleInHierarchy())
            {
                GridTableControl tableControl = GetTableControlWindow();
                Rectangle r = ElementToRectangle(element);
                return tableControl.ClientRectangle.IntersectsWith(r);
            }

            return false;
        }

        /// <summary>
        /// Returns the display area for a given field and record.
        /// </summary>
        /// <param name="el">The record or element</param>
        /// <param name="fieldName">The field (FieldDescriptor.Name or ColumnDescriptor.MappingName)</param>
        /// <returns>Rectangle.Empty if field or record is not visible; otherwise, the display area in client coordinates</returns>
        public Rectangle RecordFieldToRectangle(Element el, string fieldName)
        {
            if (el != null && el.GetVisibleInHierarchy())
            {
                GridTableControl tableControl = GetTableControlWindow();

                GridRecord record = el as GridRecord;

                Rectangle bounds;
                if (record != null && record.HasNestedTables)
                {
                    bounds = tableControl.ElementToRectangle(record.RecordRows[0]);
                }
                else
                {
                    bounds = tableControl.ElementToRectangle(el);
                }

                if (tableControl.ClientRectangle.IntersectsWith(bounds))
                {
                    GridTable gridTable = (GridTable)el.ParentTable;
                    GridTableDescriptor td = gridTable.ParentTableDescriptor;
                    int indent = 0;
                    if (gridTable.RelationParentTable != null)
                    {
                        indent += gridTable.RelationParentTable.GetTotalWidthOfRowHeadersAndIndent(true);
                    }

                    int r, c;
                    gridTable.TableDescriptor.ColumnToRowColIndex(fieldName, out r, out c);
                    int colIndex = gridTable.TableModel.FieldToColIndex(c);

                    if (el is Record)
                    {
                        ////Record record = (Record) el;
                        if (r > 0)
                        {
                            for (int row = 0; row < r; row++)
                            {
                                bounds.Y += (int)record.RecordRows[r].GetYAmountCount();
                            }

                            bounds.Height = (int)record.RecordRows[r].GetYAmountCount();
                        }
                        else if (record.RecordRows.Count > 0)
                        {
                            bounds.Height = (int)record.RecordRows[0].GetYAmountCount();
                        }
                    }

                    int hScrollOffset = tableControl.GetCurrentHScrollPixelPos() - tableControl.GetHScrollPixelMinimum();
                    if (IsRightToLeft())
                    {
                        bounds.X += tableControl.ClientRectangle.Width - (gridTable.TableModel.ColWidths.GetTotal(0, colIndex, tableControl.Width + hScrollOffset) + indent);
                        bounds.Width = gridTable.TableModel.ColWidths[colIndex];
                        bounds.X += hScrollOffset;
                    }
                    else
                    {
                        bounds.X += indent + gridTable.TableModel.ColWidths.GetTotal(0, colIndex - 1, tableControl.Width + hScrollOffset);
                        bounds.Width = gridTable.TableModel.ColWidths[colIndex];
                        bounds.X -= hScrollOffset;
                    }

                    if (tableControl.ClientRectangle.IntersectsWith(bounds))
                    {
                        return bounds;
                    }
                }
            }

            return System.Drawing.Rectangle.Empty;
        }

        /// <summary>
        /// Paints the whole element or only one field of a record to a given Graphics context. If you
        /// specify no field the whole rectangle is painted. If you specify a field name (see Fields collection
        /// for names) then only the one column in the record is painted. Painting fields works for Records,
        /// Summaries and GroupCaptions.
        /// </summary>
        /// <param name="el">The element (or record) to be drawn.</param>
        /// <param name="field">A field name (matching FieldDescriptor.Name or GridColumnDescriptor.MappingName)</param>
        public void PaintElement(Element el, string field)
        {
            if (el == null || el.IsDisposed)
            {
                return;
            }

            //// ... get (or instantiate) the cached graphics object ...
            Graphics graphics = null;

            if (HasDoubleBufferSurface)
            {
                graphics = GetCachedGraphics();
            }

            PaintElement(graphics, el, field);
        }

        /// <summary>
        /// Paints the whole element or only one field of a record to a given Graphics context. If you
        /// specify no field the whole rectangle is painted. If you specify a field name (see Fields collection
        /// for names) then only the one column in the record is painted. Painting fields works for Records,
        /// Summaries and GroupCaptions.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="el">The element (or record) to be drawn.</param>
        /// <param name="field">A field name (matching FieldDescriptor.Name or GridColumnDescriptor.MappingName)</param>
        public void PaintElement(Graphics graphics, Element el, string field)
        {
            if (el == null || el.IsDisposed)
            {
                return;
            }

            GridEngine engine = el.Engine as GridEngine;
            if (engine == null)
            {
                return;
            }

            if (field == null)
            {
                field = string.Empty;
            }

            GridGroupingControl gridGroupingControl = engine.ParentControl as GridGroupingControl;

            //// If we got the grid grouping control...
            if (gridGroupingControl != null)
            {
                GridTableControl gridTableControl = gridGroupingControl.TableControl;
                GridTable gridTable = gridGroupingControl.Table;
                GridRecord record = el as GridRecord;

                //// Ensure we got a grid table control and record...
                if (gridTableControl != null && el != null && el.GetVisibleInHierarchy())
                {
                    Rectangle bounds;

                    //// ... then calculate the bounds of the record ...
                    if (field != string.Empty)
                    {
                        bounds = gridTableControl.RecordFieldToRectangle(el, field);
                        /* tricky: Clipping at top row and left column if cell is only partially visible.
                        // DrawClippedGrid already takes care of this nicely, so while allowDirectPaintElement
                        // code might have minor performance gains it is not worth the effort at this time.
                        // Biggest gain would be that borders don't have to be drawn if solid ...
                        //
                        //if (allowDirectPaintElement && graphics != null)
                        //{
                        //    if (!bounds.IsEmpty && bounds.Height > 0 && bounds.IntersectsWith(gridTableControl.GridBounds))
                        //    {
                        //        GridTable gt = ((GridTable) el.ParentTable);
                        //        GridRangeInfo cellInfo = GetCellRange(el, field);
                        //        ChildTable savedFilteredChildTable = null;
                        //        if (gt.ParentTableDescriptor.ParentRelation != null)
                        //        {
                        //            savedFilteredChildTable = gt.FilteredChildTable;
                        //            gt.FilteredChildTable = el.ParentChildTable;
                        //        }

                        //        GridControlBase grid = gt.TableModel.ActiveGridView;
                        //        GridTableCellStyleInfo style = (GridTableCellStyleInfo) grid.GetViewStyleInfo(cellInfo.Top, cellInfo.Left);

                        //        // When borders are solid lines then we do not have to repaint them. If they
                        //        // are not solid then the background color might have changed and we need to paint them
                        //        // again. If the border style changes dynamically it best to set TableOptions.GridLineBorder
                        //        // to be dotted, then borders will always be drawn.
                        //        bool drawBorders = gt.TableOptions.GridLineBorder.Style != GridBorderStyle.Solid;

                        //        //Rectangle clipRectangle = Rectangle.Empty;

                        //        //if (cellInfo.Top == gridTableControl.TopRowIndex)
                        //        //{
                        //        //    clipRectangle = bounds;
                        //        //    bounds.Y -= gridTableControl.GetCurrentVScrollPixelDelta();
                        //        //}
                        //        //if (cellInfo.Left == gridTableControl.LeftColIndex)
                        //        //{
                        //        //    if (clipRectangle.IsEmpty)
                        //        //        clipRectangle = bounds;

                        //        //    bounds.X -= gridTableControl.GetCurrentHScrollPixelDelta();
                        //        //}

                        //        grid.DrawSingleCell(graphics, cellInfo.Top, cellInfo.Left, bounds, style, true, drawBorders);

                        //        if (savedFilteredChildTable != null)
                        //            gt.FilteredChildTable = savedFilteredChildTable;
                        //    }
                        //    return;
                        }*/
                    }
                    else
                    {
                        if (record != null)
                        {
                            bounds = gridTableControl.RecordRowsToRectangle(record, GridCellSizeKind.VisibleSize);
                        }
                        else if ((el is CaptionSection || el is ColumnHeaderSection || el is GridSummarySection 
                            || el is AddNewRecord || el is CaptionRow || el is ColumnHeaderRow)
                            && el.ParentGroup.IsTopLevelGroup)
                        {
                            bounds = gridTableControl.RangeInfoToRectangle(GridRangeInfo.Row(gridTableControl.Table.NestedDisplayElements.IndexOf(el)));
                        }
                        else
                        {
                            bounds = gridTableControl.ElementToRectangle(el);
                        }
                    }

                    // If we have a valid range in which to draw...
                    if (!bounds.IsEmpty && bounds.Height > 0 && bounds.IntersectsWith(gridTableControl.GridBounds))
                    {
                        if (graphics == null)
                        {
                            gridTableControl.UpdateWithDrawClippedGrid(bounds);
                        }
                        else
                        {
                            // Declare a variable to store the clip bounds
                            Region oldClipRegion = null;

                            // Calculate whether the clip bounds need to be further clipped to prevent
                            // drawing a row over the column header
                            bool shouldClip;
                            if (gridTable.Engine.SupportsYAmount && gridTable.Engine.TableOptions.VerticalPixelScroll)
                            {
                                double yPos = gridTable.NestedDisplayElements.GetYAmountPositionOf(el);
                                shouldClip = yPos < gridTableControl.GetCurrentVScrollPixelPos() && bounds.Bottom > gridTableControl.GetVScrollPixelMinimum();
                            }
                            else
                            {
                                shouldClip = false;
                            }

                            // If we need to clip...
                            if (shouldClip)
                            {
                                // ... store the current clip bounds
                                oldClipRegion = graphics.Clip;

                                // ... and set the new clip area
                                Rectangle r = gridTableControl.ViewLayout.ScrollAreaBounds;
                                r.Intersect(bounds);
                                graphics.SetClip(r);
                            }

                            // Draw the record in the graphics object...
                            gridTableControl.DrawClippedGrid(graphics, bounds, false);

                            // If we used additional clipping...
                            if (shouldClip)
                            {
                                // ... revert back to the original clipping area
                                graphics.Clip = oldClipRegion;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns the cell coordinates for a given element and field in a record.
        /// </summary>
        /// <param name="el">The element.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <returns>Cell coordinates.</returns>
        public GridRangeInfo GetCellRange(Element el, string fieldName)
        {
            GridRangeInfo cellInfo;

            if (el is RecordRow)
            {
                el = el.ParentRecord;
            }

            if (el is RowElement)
            {
                el = el.ParentSection;
            }

            GridTableDescriptor tableDescriptor = (GridTableDescriptor)el.ParentTableDescriptor;
            GridTable table = (GridTable)el.ParentTable;

            // Get the row and column index in the grid for a column (works also
            // if column sets with multiple rows are specified).
            int relativeRowIndex, colIndex;
            tableDescriptor.ColumnToRowColIndex(fieldName, out relativeRowIndex, out colIndex);

            ChildTable childTable = el.ParentChildTable;
            int recordRowIndex = childTable.DisplayElements.IndexOf(el);

            int rowIndex = recordRowIndex + relativeRowIndex;
            colIndex += tableDescriptor.GetColumnIndentCount();
            ////cellInfo = GridRangeInfo.Cell(rowIndex, colIndex);
            Model.CoveredRanges.Find(rowIndex, colIndex, out cellInfo);

            return cellInfo;
        }

        /// <summary>
        /// Returns the display area for a given column and record.
        /// </summary>
        /// <param name="record">The record</param>
        /// <param name="column">The ColumnDescriptor</param>
        /// <returns>Rectangle.Empty if field or record is not visible; otherwise, the display area in client coordinates</returns>
        public Rectangle RecordColumnToRectangle(Record record, GridColumnDescriptor column)
        {
            return RecordFieldToRectangle(record, column.MappingName);
        }

        /// <summary>
        /// Invalidates the display area for a given record and field.
        /// </summary>
        /// <param name="record">The record</param>
        /// <param name="fieldName">The field (FieldDescriptor.Name or ColumnDescriptor.MappingName)</param>
        public void InvalidateRecordField(Record record, string fieldName)
        {
            GridTableControl tableControl = GetTableControlWindow();
            System.Drawing.Rectangle bounds = RecordFieldToRectangle(record, fieldName);
            if (!bounds.IsEmpty)
            {
                tableControl.Invalidate(bounds);
            }
        }

        /// <summary>
        /// Invalidates the display area for a given record and column.
        /// </summary>
        /// <param name="record">The record</param>
        /// <param name="column">The ColumnDescriptor</param>
        public void InvalidateRecordColumn(Record record, GridColumnDescriptor column)
        {
            InvalidateRecordField(record, column.MappingName);
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers after the record change reflected in binded source list.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_SourceListRecordChanged(object sender, RecordChangedEventArgs e)
        {
            /*                                                         bool inAddNewBeginEdit = e.Action == RecordChangedType.Added && CurrentRecordManager.InBeginEdit;
            //                                                         bool syncCurrentRecordPos = Table.CurrentElement != null && !(
            //                                                                            CurrentRecordManager.InBeginEdit||
            //                                                                            CurrentRecordManager.InCancelEdit||
            //                                                                            CurrentRecordManager.InEndEdit||
            //                                                                            CurrentRecordManager.InEnterRecord||
            //                                                                            CurrentRecordManager.InLeaveRecord||
                                                                                       CurrentRecordManager.InNavigate);*/
            if (GroupingControl.UseCustomUpdateOnListChanged || !GroupingControl.Engine.UseOldListChangedHandler)
            {
                return;
            }

            if (this.DisableScrollWindow)
            {
                ViewLayout.Reset();
                return;
            }

            // Note: this control (if windowless, nested) might be editing a different table at at this time.
            if (Table.TableDirty)
            {
                return;
            }

            switch (e.Action)
            {
                case RecordChangedType.Changed:
                    {
                        if (!e.SortedPositionChanged && !e.Record.GetVisibleInHierarchy())
                        {
                            return;
                        }

                        GridTableControl tableControl = GetTableControlWindow();
                        e.RaiseDisplayElementChanged = e.SortedPositionChanged;

                        if (e.TableListChangedEventArgs.ListChangedType == ListChangedType.ItemAdded)
                        {
                            tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                        }

                        if ((!this.Table.CurrentRecordManager.InEndEdit && GroupingControl.InvalidateAllWhenListChanged) || e.SortedPositionChanged)
                        {
                            tableControl.Invalidate();

                            // Fixes issue that current cell is not scrolled when a record was changed and its visibility
                            // (RecordFilter) was affected.
                            if (!this.Table.CurrentRecordManager.InEndEdit && e.SortedPositionChanged)
                            {
                                tableControl.synchronizeGridShouldRestoreCurrentCell = true;
                            }
                        }
                        else
                        {
                            if (e.TableListChangedEventArgs == null || e.TableListChangedEventArgs.ShouldInvalidateScreen)
                            {
                                ////if (e.SortedPositionChanged)
                                ViewLayout.Reset();

                                InvalidateElement(e.Record);
                            }
                        }

                        break;
                    }

                case RecordChangedType.Removed:
                    {
                        e.RaiseDisplayElementChanged = Table.TableDescriptor.IsGrouped;
                        GridTableControl tableControl = GetTableControlWindow();
                        tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                        tableControl.synchronizeGridShouldRestoreCurrentCell = true;

                        RemovedRecordInfo ri = e.Reserved as RemovedRecordInfo;
                        if (ri != null)
                        {
                            //// Have to act on parent table control (windowless nested controls could
                            //// be in a totally different context at this time, e.g. editing a different
                            //// childtable).
                            if (SupportsYAmount)
                            {
                                ////  int yOffset;
                                //// if (removedElement is NestedTable)
                                //// yOffset = (int) engineTable.DisplayElements.GetYAmountPositionOf(removedElement);
                                //// else
                                //// yOffset = (int) engineTable.NestedDisplayElements.GetYAmountPositionOf(removedElement);
                                //// int yAmount = (int) removedElement.GetYAmountCount();
                                int yOffset = ri.yOffset;
                                int yAmount = ri.yAmount;
                                int yTop = tableControl.GetVScrollPixelMinimum();
                                int yBottom = tableControl.Bottom;
                                int yPos = tableControl.GetCurrentVScrollPixelPos();

                                bool scroll = false;

                                if (yOffset < yTop)
                                {
                                    tableControl.Invalidate();
                                }
                                else if (yOffset + yAmount < yPos)
                                {
                                    tableControl.InternalSetCurrentVScrollPixelPos((int)(yPos + yAmount));
                                }
                                else if (yOffset < yPos && yOffset + yAmount >= yPos)
                                {
                                    yAmount -= yPos - yOffset;
                                    scroll = true;
                                }
                                else if (yOffset > yPos && yOffset + yAmount < yPos + yBottom - yTop)
                                {
                                    scroll = true;
                                }
                                else if (yOffset < yPos + yBottom - yTop)
                                {
                                    scroll = true;
                                }

                                //// if (yOffset < yBottom)
                                //// {
                                //// int y = yOffset - yPos + yTop;
                                //// Rectangle r = new Rectangle(0, y, tableControl.Bottom-y, tableControl.Right);
                                //// tableControl.Invalidate(r);
                                //// }
                                if (scroll)
                                {
                                    int y = (int)(yOffset - yPos + yTop); //// - yAmount;
                                    Rectangle r = new Rectangle(0, y, tableControl.Right, yBottom - y);

                                    if (y + yAmount <= yBottom)
                                    {
                                        if (!this.OptimizeInsertRemoveCells)
                                        {
                                            tableControl.Invalidate(r);
                                        }

                                        tableControl.ScrollWindow(0, (int)-yAmount, r, r, true);
                                    }
                                    else
                                    {
                                        tableControl.Invalidate(r);
                                    }
                                }
                            }
                            else
                            {
                                //// int yOffset = (int) engineTable.NestedDisplayElements.IndexOf(removedElement);
                                //// int yAmount = (int) removedElement.GetVisibleCount();
                                int yOffset = ri.yOffset;
                                int yAmount = ri.yAmount;
                                int yTop = tableControl.VScrollBar.Minimum;
                                int yBottom = tableControl.ViewLayout.LastVisibleRow;
                                int yPos = tableControl.VScrollBar.Value;

                                bool scroll = false;

                                if (yOffset < yTop)
                                {
                                    tableControl.Invalidate();
                                }
                                else if (yOffset + yAmount < yPos)
                                {
                                    tableControl.InternalSetTopRow((int)(yPos + yAmount));
                                }
                                else if (yOffset < yPos && yOffset + yAmount >= yPos)
                                {
                                    yAmount -= yPos - yOffset;
                                    scroll = true;
                                }
                                else if (yOffset > yPos && yOffset + yAmount < yPos + yBottom - yTop)
                                {
                                    scroll = true;
                                }
                                else if (yOffset < yPos + yBottom - yTop)
                                {
                                    scroll = true;
                                }

                                //// if (yOffset < yBottom)
                                //// {
                                //// int y = yOffset - yPos + yTop;
                                //// Rectangle r = new Rectangle(0, y, tableControl.Bottom-y, tableControl.Right);
                                //// tableControl.Invalidate(r);
                                //// }
                                if (scroll)
                                {
                                    int y = (int)(yOffset - yPos + yTop); //// - yAmount;
                                    Rectangle r = tableControl.ViewLayout.RectangleBottomOfRow(y);

                                    if (y + yAmount <= yBottom)
                                    {
                                        if (!this.OptimizeInsertRemoveCells)
                                        {
                                            tableControl.Invalidate(r);
                                        }

                                        int heightInPixel = tableControl.GetRowRangeHeight(yOffset, yOffset + yAmount - 1, tableControl.Height);
                                        tableControl.ScrollWindow(0, (int)-heightInPixel, r, r, true);
                                    }
                                    else
                                    {
                                        tableControl.Invalidate(r);
                                    }
                                }
                            }
                        }

                        break;
                    }

                case RecordChangedType.Added:
                    {
                        e.RaiseDisplayElementChanged = false;

                        Element addedElement = e.Record;

                        // All parent groups that only have one item will have been inserted.
                        if (e.Group != null)
                        {
                            addedElement = e.Group;
                        }

                        Table engineTable = e.Record.EngineTable;
                        GridTableControl tableControl = GetTableControlWindow();

                        if (!this.Table.CurrentRecordManager.InEndEdit && GroupingControl.InvalidateAllWhenListChanged)
                        {
                            tableControl.Invalidate();
                            tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                            return;
                        }

                        if (!addedElement.GetVisibleInHierarchy())
                        {
                            return;
                        }

                        // Have to act on parent table control (windowless nested controls could
                        // be in a totally different context at this time, e.g. editing a different
                        // childtable).
                        if (SupportsYAmount)
                        {
                            int yOffset;
                            if (addedElement is NestedTable)
                            {
                                yOffset = (int)engineTable.DisplayElements.GetYAmountPositionOf(addedElement);
                            }
                            else
                            {
                                yOffset = (int)engineTable.NestedDisplayElements.GetYAmountPositionOf(addedElement);
                            }

                            int yAmount = (int)addedElement.GetYAmountCount();
                            int yTop = GetTableControlWindow().GetVScrollPixelMinimum();
                            int yBottom = tableControl.Bottom;
                            int yPos = tableControl.GetCurrentVScrollPixelPos();

                            bool scroll = false;

                            if (yOffset < yTop)
                            {
                                tableControl.Invalidate();
                            }
                            else if (yOffset + yAmount < yPos)
                            {
                                tableControl.InternalSetCurrentVScrollPixelPos((int)(yPos + yAmount));
                            }
                            else if (yOffset < yPos && yOffset + yAmount >= yPos)
                            {
                                yAmount -= yPos - yOffset;
                                scroll = true;
                            }
                            else if (yOffset > yPos && yOffset + yAmount < yPos + yBottom - yTop)
                            {
                                scroll = true;
                            }
                            else if (yOffset < yPos + yBottom - yTop)
                            {
                                scroll = true;
                            }

                            //// if (yOffset < yBottom)
                            //// {
                            //// int y = yOffset - yPos + yTop;
                            //// Rectangle r = new Rectangle(0, y, tableControl.Bottom-y, tableControl.Right);
                            //// tableControl.Invalidate(r);
                            //// }
                            if (scroll)
                            {
                                int y = (int)(yOffset - yPos + yTop) - yAmount;
                                Rectangle r = new Rectangle(0, y, tableControl.Right, yBottom - y);

                                if (y + yAmount > yBottom)
                                {
                                    tableControl.Invalidate(r);
                                }
                                else
                                {
                                    tableControl.ScrollWindow(0, (int)yAmount, r, r, true);
                                    if (!this.OptimizeInsertRemoveCells)
                                    {
                                        tableControl.Invalidate(r);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int yOffset = (int)engineTable.NestedDisplayElements.IndexOf(addedElement);
                            int yAmount = (int)addedElement.GetVisibleCount();
                            int yTop = tableControl.VScrollBar.Minimum;
                            int yBottom = tableControl.ViewLayout.LastVisibleRow;
                            int yPos = tableControl.VScrollBar.Value;

                            bool scroll = false;

                            if (yOffset < yTop)
                            {
                                tableControl.Invalidate();
                            }
                            else if (yOffset + yAmount < yPos)
                            {
                                tableControl.InternalSetTopRow((int)(yPos + yAmount));
                                ViewLayout.Reset();
                            }
                            else if (yOffset < yPos && yOffset + yAmount >= yPos)
                            {
                                yAmount -= yPos - yOffset;
                                scroll = true;
                            }
                            else if (yOffset > yPos && yOffset + yAmount < yPos + yBottom - yTop)
                            {
                                scroll = true;
                            }
                            else if (yOffset < yPos + yBottom - yTop)
                            {
                                scroll = true;
                            }

                            //// if (yOffset < yBottom)
                            //// {
                            //// int y = yOffset - yPos + yTop;
                            //// Rectangle r = new Rectangle(0, y, tableControl.Bottom-y, tableControl.Right);
                            //// tableControl.Invalidate(r);
                            //// }
                            if (scroll)
                            {
                                int y = (int)(yOffset - yPos + yTop) - yAmount;
                                Rectangle r = tableControl.ViewLayout.RectangleBottomOfRow(y + 1);

                                if (y + yAmount > yBottom)
                                {
                                    tableControl.Invalidate(r);
                                }
                                else
                                {
                                    int heightInPixel = tableControl.GetRowRangeHeight(yOffset, yOffset + yAmount - 1, tableControl.Height);
                                    tableControl.ScrollWindow(0, (int)heightInPixel, r, r, true);
                                    if (!this.OptimizeInsertRemoveCells)
                                    {
                                        tableControl.Invalidate(r);
                                    }
                                }
                            }
                        }

                        tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                        break;
                    }
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers when the record change reflected in binded source list.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_SourceListRecordChanging(object sender, RecordChangedEventArgs e)
        {
            //// bool inAddNewBeginEdit = e.Action == RecordChangedType.removed && CurrentRecordManager.InBeginEdit;
            //// bool syncCurrentRecordPos = Table.CurrentElement != null && !(
            //// CurrentRecordManager.InBeginEdit||
            //// CurrentRecordManager.InCancelEdit||
            //// CurrentRecordManager.InEndEdit||
            //// CurrentRecordManager.InEnterRecord||
            //// CurrentRecordManager.InLeaveRecord||
            //// CurrentRecordManager.InNavigate);
            if (this.DisableScrollWindow || GroupingControl.UseCustomUpdateOnListChanged || !GroupingControl.Engine.UseOldListChangedHandler)
            {
                return;
            }

            //// Note: this control (if windowless, nested) might be editing a different table at at this time
            switch (e.Action)
            {
                case RecordChangedType.Removed:
                    {
                        e.RaiseDisplayElementChanged = Table.TableDescriptor.IsGrouped;

                        Element removedElement = e.Record;

                        //// All parent groups that only have one item will have been inserted.
                        if (e.Group != null)
                        {
                            removedElement = e.Group;
                        }

                        Table engineTable = e.Record.EngineTable;
                        GridTableControl tableControl = GetTableControlWindow();

                        if (Table.TableDirty)
                        {
                            tableControl.Invalidate();
                            tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                            return;
                        }

                        if (!removedElement.GetVisibleInHierarchy())
                        {
                            return;
                        }

                        if (!Table.CurrentRecordManager.InEndEdit && GroupingControl.InvalidateAllWhenListChanged)
                        {
                            tableControl.Invalidate();
                            tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                            return;
                        }

                        //// Extract information for removedElement that is later needed when
                        //// RecordChanged event is raised. At that time removedElement is not valid anymore,
                        //// but we can get all information from e.Reserved
                        RemovedRecordInfo ri = new RemovedRecordInfo();

                        //// Have to act on parent table control (windowless nested controls could
                        //// be in a totally different context at this time, e.g. editing a different
                        //// childtable).
                        if (SupportsYAmount)
                        {
                            if (removedElement is NestedTable)
                            {
                                ri.yOffset = (int)engineTable.DisplayElements.GetYAmountPositionOf(removedElement);
                            }
                            else
                            {
                                ri.yOffset = (int)engineTable.NestedDisplayElements.GetYAmountPositionOf(removedElement);
                            }

                            ri.yAmount = (int)removedElement.GetYAmountCount();
                        }
                        else
                        {
                            ri.yOffset = (int)engineTable.NestedDisplayElements.IndexOf(removedElement);
                            ri.yAmount = (int)removedElement.GetVisibleCount();
                        }

                        e.Reserved = ri;

                        break;
                    }
            }
        }

        class RemovedRecordInfo
        {
            public int yOffset;
            public int yAmount;
        }

        /*
        private void Table_RecordExpanding(object sender, RecordEventArgs e)
        {
                           if (!e.Record.GetVisibleInHierarchy())
                                              return;

                           Table engineTable = e.Record.EngineTable;
                           GridTableControl tableControl = GetTableControlWindow();

                           //Debug.Assert(tableControl == this);
                           oldYAmount = (int) e.Record.GetYAmountCount() - oldYAmount;
                           //e.RaiseDisplayElementChanged = false;
        }

        private void Table_RecordExpanded(object sender, RecordEventArgs e)
        {
                           if (!e.Record.GetVisibleInHierarchy())
                                              return;

                           Table engineTable = e.Record.EngineTable;
                           GridTableControl tableControl = GetTableControlWindow();

                           //Debug.Assert(tableControl == this);

                           // Have to act on parent table control (windowless nested controls could
                           // be in a totally different context at this time, e.g. editing a different
                           // childtable).
                           int yOffset = (int) engineTable.NestedDisplayElements.GetYAmountPositionOf(e.Record) + oldYAmount;
                           int yAmount = (int) e.Record.GetYAmountCount() - oldYAmount;
                           int yTop = GetTableControlWindow().GetVScrollPixelMinimum();
                           int yBottom = tableControl.Bottom;
                           int yPos = tableControl.GetCurrentVScrollPixelPos();

                           bool scroll = false;

                           if (yOffset < yTop)
                                              tableControl.Invalidate();

                           else if (yOffset + yAmount < yPos)
                           {
                                              tableControl.InternalSetCurrentVScrollPixelPos((int) (yPos + yAmount));
                           }

                           else if (yOffset < yPos && yOffset + yAmount >= yPos)
                           {
                                              yAmount -= yPos - yOffset;
                                              scroll = true;
                           }

                           else if (yOffset > yPos && yOffset + yAmount < yPos + yBottom - yTop)
                           {
                                              scroll = true;
                           }

                           else if (yOffset < yPos + yBottom - yTop)
                                              scroll = true;

                           //                                                                                               if (yOffset < yBottom)
                           //                                                                                               {
                           //                                                                                                                  int y = yOffset - yPos + yTop;
                           //                                                                                                                  Rectangle r = new Rectangle(0, y, tableControl.Bottom-y, tableControl.Right);
                           //                                                                                                                  tableControl.Invalidate(r);
                           //                                                                                               }

                           if (scroll)
                           {
                                              int y = (int) (yOffset - yPos + yTop);// - yAmount;
                                              Rectangle r = new Rectangle(0, y, tableControl.Right, yBottom-y+yAmount);

                                              if (y + yAmount > yBottom)
                                                                 tableControl.Invalidate(r);
                                              else
                                              {
                                                                 tableControl.ScrollWindow(0, (int) yAmount, r, r, true);
                                                                 if (!this.OptimizeInsertRemoveCells)
                                                                                    tableControl.Invalidate(r);
                                              }
                           }

                           tableControl.synchronizeGridShouldnextUpdateScrollBars = true;
                           e.RaiseDisplayElementChanged = false;
        }
        */

        // event GridQueryAllowDragColumnEventHandler QueryAllowDragColumn;

        /// <summary>
        /// Occurs when the user hovers the mouse over a column header or clicks on it.
        /// In your event handler, you can determine if the selected column can be dragged.
        /// </summary>
        /// <remarks>
        /// You can disallow dragging the column when
        /// you assign False to <see cref="GridQueryAllowDragColumnEventArgs.AllowDrag"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowDragColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        public event GridQueryAllowDragColumnEventHandler QueryAllowDragColumn;

        /// <summary>
        /// Raises the <see cref="OnQueryAllowDragColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowDragColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAllowDragColumn(GridQueryAllowDragColumnEventArgs e)
        {
            if (QueryAllowDragColumn != null)
            {
                QueryAllowDragColumn(this, e);
            }
        }

        internal void RaiseQueryAllowDragColumn(GridQueryAllowDragColumnEventArgs e)
        {
            if(GroupingControl != null)
                GroupingControl.RaiseTableControlQueryAllowDragColumn(e);
            OnQueryAllowDragColumn(e);

            if (!e.Handled)
            {
                GridTableDescriptor td = TableDescriptor;
                if (td.GetFrozenColumn() != string.Empty)
                {
                    int col1 = GetColIndex(td, e.Column);
                    int col2 = e.InsertBeforeColumn != null ? GetColIndex(td, e.InsertBeforeColumn) : int.MaxValue;
                    if (Math.Min(col1, col2) <= GetColIndex(td, td.GetFrozenColumn()))
                    {
                        e.AllowDrag = false;
                    }
                }
            }
        }

        /// <summary>
        /// Helper method returns relative column index for column
        /// </summary>
        /// <param name="td">The GridTableDescriptor</param>
        /// <param name="columnName">The columnName</param>
        /// <returns>returns the column index</returns>
        int GetColIndex(GridTableDescriptor td, string columnName)
        {
            string fieldName = td.Columns[columnName].MappingName;
            int row, col;
            td.ColumnToRowColIndex(fieldName, out row, out col);
            return col;
        }

        //// event GridQueryAllowGroupByColumnEventHandler QueryAllowGroupByColumn;

        /// <summary>
        /// Occurs when the user drags a column header over the GroupDropArea.
        /// In your event handler, you can determine if the grid can be grouped by the selected column.
        /// </summary>
        /// <remarks>
        /// You can disallow grouping by the column when
        /// you assign False to <see cref="GridQueryAllowGroupByColumnEventArgs.AllowGroupByColumn"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowGroupByColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        public event GridQueryAllowGroupByColumnEventHandler QueryAllowGroupByColumn;

        /// <summary>
        /// Raises the <see cref="OnQueryAllowGroupByColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowGroupByColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAllowGroupByColumn(GridQueryAllowGroupByColumnEventArgs e)
        {
            if (QueryAllowGroupByColumn != null)
            {
                QueryAllowGroupByColumn(this, e);
            }
        }

        internal void RaiseQueryAllowGroupByColumn(GridQueryAllowGroupByColumnEventArgs e)
        {
            GroupingControl.RaiseTableControlQueryAllowGroupByColumn(e);
            OnQueryAllowGroupByColumn(e);
        }

        // event GridQueryAllowSortColumnEventHandler QueryAllowSortColumn;

        /// <summary>
        /// Occurs when the user hovers the mouse over a column header or clicks on it.
        /// In your event handler you can determine if the selected column can be sorted.
        /// </summary>
        /// <remarks>
        /// You can disallow sorting by the column when
        /// you assign False to <see cref="GridQueryAllowSortColumnEventArgs.AllowSort"/>.
        /// </remarks>
        /// <seealso cref="GridQueryAllowSortColumnEventArgs"/>
        /// <seealso cref="GridTableOptionsStyleInfo"/>
        public event GridQueryAllowSortColumnEventHandler QueryAllowSortColumn;

        /// <summary>
        /// Raises the <see cref="QueryAllowSortColumn"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowSortColumnEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAllowSortColumn(GridQueryAllowSortColumnEventArgs e)
        {
            if (QueryAllowSortColumn != null)
            {
                QueryAllowSortColumn(this, e);
            }
        }

        internal void RaiseQueryAllowSortColumn(GridQueryAllowSortColumnEventArgs e)
        {
            if(GroupingControl != null)
                GroupingControl.RaiseTableControlQueryAllowSortColumn(e);
            OnQueryAllowSortColumn(e);
        }

        // event GridQueryAllowArrowKeyNavigateToEventHandler QueryAllowArrowKeyNavigateTo;

        /// <summary>
        /// Occurs when the user navigates though display elements with arrow keys.
        /// In your event handler you can determine if the specified display element (e.g. a CaptionRow) can be stepped
        /// on or if it should be skipped.
        /// </summary>
        /// <remarks>
        /// You can set AllowNavigateTo if you
        /// want arrow keys to skip over specific display elements (e.g. skip caption rows).
        /// </remarks>
        /// <seealso cref="GridQueryAllowArrowKeyNavigateToEventArgs"/>
        public event GridQueryAllowArrowKeyNavigateToEventHandler QueryAllowArrowKeyNavigateTo;

        /// <summary>
        /// Raises the <see cref="QueryAllowArrowKeyNavigateTo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridQueryAllowArrowKeyNavigateToEventArgs" /> that contains the event data.</param>
        protected virtual void OnQueryAllowArrowKeyNavigateTo(GridQueryAllowArrowKeyNavigateToEventArgs e)
        {
            if (QueryAllowArrowKeyNavigateTo != null)
            {
                QueryAllowArrowKeyNavigateTo(this, e);
            }
        }

        internal void RaiseQueryAllowArrowKeyNavigateTo(GridQueryAllowArrowKeyNavigateToEventArgs e)
        {
            GroupingControl.RaiseTableControlQueryAllowArrowKeyNavigateTo(e);
            OnQueryAllowArrowKeyNavigateTo(e);
        }

        private void synchronizeGridNavigateToBookmark_Disposed(object sender, EventArgs e)
        {
            ResetSynchronizeGridNavigateToBookmark();
        }

        void ResetSynchronizeGridNavigateToBookmark()
        {
            if (synchronizeGridNavigateToBookmark != null)
            {
                synchronizeGridNavigateToBookmark.Disposed -= new EventHandler(synchronizeGridNavigateToBookmark_Disposed);
                this.synchronizeGridNavigateToBookmark = null;
            }
        }

        private void synchronizeGridNavigateTo_Disposed(object sender, EventArgs e)
        {
            ResetSynchronizeGridNavigateTo();
        }

        void ResetSynchronizeGridNavigateTo()
        {
            if (synchronizeGridNavigateTo != null)
            {
                synchronizeGridNavigateTo.Disposed -= new EventHandler(synchronizeGridNavigateToBookmark_Disposed);
                this.synchronizeGridNavigateTo = null;
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers when change occurs in sourcelist.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_SourceListListChanged(object sender, TableListChangedEventArgs e)
        {
            if (Table.TableDirty)
            {
                Invalidate();
                this.MarkResync(false);
            }
        }

        /// <internalonly/>
        /// <summary>
        /// Triggers after the record value changed.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void Table_RecordValueChanged(object sender, RecordValueChangedEventArgs e)
        {
            //// If there are dependent fields, invalidate the whole record.  e.FieldDescriptor.Hide - isObjectReferenceField
            if (e.FieldDescriptor.IsForeignKeyField() || e.FieldDescriptor.Hide || Table.TableDescriptor.ExpressionFields.Count > 0 || Table.TableDescriptor.UnboundFields.Count > 0)
            {
                this.InvalidateRange(Table.GetElementRangeInfo(e.Record));
            }
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns the table</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Table GetTable()
        {
            return Table;
        }

        /// <summary>
        /// Returns the default maximum size for the drop-down part of a StandardValuesCell.
        /// </summary>
        /// <returns>returns the table size</returns>
        /// <override/>
        protected override Size GetDefaultMaxStandardValuesSize()
        {
            return Table.TableOptions.MaxDropDownTableSize;
        }

        /// <summary>
        /// Determines the nested display element that is shown at the specified point.
        /// </summary>
        /// <param name="ptClient">The point in client coordinates</param>
        /// <returns>The display element.</returns>
        /// <remarks>
        /// <seealso cref="GridTableControl.PointToTableCellStyle"/>
        /// <seealso cref="PointToTableCellStyle"/>
        /// </remarks>
        public Element PointToNestedDisplayElement(Point ptClient)
        {
            if (ptClient.X < 0 || ptClient.Y < 0)
            {
                return null;
            }

            GridTableControl tableControl = this.GetTableControlWindow();
            GridTable mainTable = tableControl.Table;

            //// Get nested element that is displayed under cursor
            Element displayElement = null;
            if (SupportsYAmount)
            {
                int yOffset = tableControl.GetVScrollPixelMinimum();
                double yPosition = ptClient.Y;
                if (ptClient.Y > yOffset)
                {
                    yPosition = ptClient.Y - yOffset + tableControl.GetCurrentVScrollPixelPos(); // Take current scroll position if mouse is over scrollable area.
                }

                displayElement = mainTable.NestedDisplayElements.GetItemAtYAmount(yPosition);
            }
            else
            {
                int rowIndex = tableControl.GetRow(tableControl.ViewLayout.PointToClientRow(ptClient, GridCellSizeKind.VisibleSize));
                if (rowIndex >= 0 && rowIndex < mainTable.NestedDisplayElements.Count)
                {
                    displayElement = mainTable.NestedDisplayElements[rowIndex];
                }
            }

            return displayElement;
        }

        /// <summary>
        /// Returns the style information for the cell that is displayed under a mouse position in client coordinates. If cell belongs to a nested
        /// table style information is returned for the cell inside the nested table. Good for hit-testing.
        /// </summary>
        /// <param name="ptClient">The mouse position in client coordinates.</param>
        /// <returns>The style element with identity information for the cell that is displayed at the specified coordinate.</returns>
        /// <remarks>
        /// <example>
        /// This example dumps information about the cell below a mouse cursor while the user is hovering the mouse
        /// over the grid.
        /// <code lang="C#">
        /// private void TableControl_MouseMove(object sender, MouseEventArgs e)
        /// {
        ///     Point ptClient = new Point(e.X, e.Y);
        ///     GridTableControl tableControl = this.groupingGrid1.TableControl;
        /// <para/>
        /// <para/>
        ///     GridTableCellStyleInfo style = tableControl.PointToTableCellStyle(ptClient);
        /// <para/>
        ///     Element displayElement = style.TableCellIdentity.DisplayElement;
        /// <para/>
        ///     string info = string.Empty;
        ///     if (style != null)
        ///     {
        ///         if (style.TableCellIdentity.Column != null)
        ///             info = style.TableCellIdentity.Column.Name;
        ///         else
        ///             info = style.TableCellIdentity.ToString();
        ///     }
        /// <para/>
        ///     Console.WriteLine("{0}: {1},{2},{3} ", ptClient, displayElement.ParentChildTable.CategoriesToString(), displayElement.GetType().Name, info);
        /// }
        /// </code>
        /// <code lang="VB">
        /// Private Sub TableControl_MouseMove(sender As Object, e As MouseEventArgs)
        ///     Dim ptClient As New Point(e.X, e.Y)
        ///     Dim tableControl As GridTableControl = Me.groupingGrid1.TableControl
        /// <para/>
        /// <para/>
        ///     Dim style As GridTableCellStyleInfo = tableControl.PointToTableCellStyle(ptClient)
        /// <para/>
        ///     Dim displayElement As Element = style.TableCellIdentity.DisplayElement
        /// <para/>
        ///     Dim info As String = string.Empty
        ///     If Not (style Is Nothing) Then
        ///         If Not (style.TableCellIdentity.Column Is Nothing) Then
        ///             info = style.TableCellIdentity.Column.Name
        ///         Else
        ///             info = style.TableCellIdentity.ToString()
        ///         End If
        ///     End If
        ///     Console.WriteLine("{0}: {1},{2},{3} ", ptClient, displayElement.ParentChildTable.CategoriesToString(), displayElement.GetType().Name, info)
        /// End Sub 'TableControl_MouseMove
        /// </code>
        /// </example>
        /// <seealso cref="PointToTableCellStyle"/>
        /// <seealso cref="GridTableControl.PointToNestedDisplayElement"/>
        /// </remarks>
        public GridTableCellStyleInfo PointToTableCellStyle(Point ptClient)
        {
            //// Get nested element that is displayed under cursor
            Element displayElement = PointToNestedDisplayElement(ptClient);

            return Table.GetTableCellStyle(displayElement, ptClient.X);
        }

        /// <summary>
        /// Indicates if the grid control should handle tab keys to move between cells. Set this to False if focus
        /// should move to the next control in the form instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public override bool WantTabKey
        {
            get
            {
                return this.GroupingControl.WantTabKey;
            }

            set
            {
                base.WantTabKey = value;
                GroupingControl.WantTabKey = value;
            }
        }

        /// <override/>
        /// <summary>Notifies the grid about the temporary state of the cell.</summary>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        public override void NotifyCellHighlighted(int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (GroupingControl != null)
            {
                GridTableCellStyleInfoIdentity id = ((GridTableCellStyleInfo)style).TableCellIdentity;
                GroupingControl.AddHighlightedElement(id.DisplayElement);
            }
        }
    }
}
