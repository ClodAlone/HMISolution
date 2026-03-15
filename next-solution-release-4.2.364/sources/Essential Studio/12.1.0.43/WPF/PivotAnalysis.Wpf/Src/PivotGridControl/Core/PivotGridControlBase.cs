#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Syncfusion.Windows;
using Syncfusion.Windows.ComponentModel;
using System.Linq.Expressions;
using Syncfusion.Linq;
using System.Diagnostics;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Media;
using System.ComponentModel;
using Syncfusion.Windows.Controls.Scroll;

#if !SILVERLIGHT
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Controls.PivotGrid.Resources;
using System.Windows.Controls;
using System.Threading;
using Syncfusion.Windows.Controls.Cells;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Reflection;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls;
using Syncfusion.Windows.Controls.PivotGrid;
using System.Globalization;
using Syncfusion.Silverlight.Controls.PivotGrid.Resources;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Defines the internal grid control that is placed inside a cell to form a grid control.
    /// It derives from <see cref="GridControlBase"/> and hence share the basic charateristics of the GridControl.
    /// </summary>
#if !SILVERLIGHT
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
#endif

    public class PivotGridControlBase : GridControlBase
    {
        #region [ Initilize /Finalize ]
#if !SILVERLIGHT
        ResourceWrapperKeys rsWrapperKey = new ResourceWrapperKeys();
#endif
        /// <summary>
        /// Initializes the <see cref="PivotGridControlBase"/> class.
        /// </summary>
        public PivotGridControlBase()
        {
            this.HiddenRowGroups = new List<HiddenGroup>();
            this.HiddenColumnGroups = new List<HiddenGroup>();
            this.HiddenSubTotalsRowGroups = new List<HiddenGroup>();
            this.HiddenSubTotalsColumnGroups = new List<HiddenGroup>();
            this.HiddenRowGroupStore = new List<HiddenGroup>();
            this.HiddenColumnGroupStore = new List<HiddenGroup>();
            this.Model.CellModels.Add("ExpanderCell", new PivotGridExpandCellModel());
            this.Model.CellModels.Add("TemplateCell", new PivotGridTemplateCellModel());
            this.Model.CellModels.Add("HyperlinkCell", new PivotGridHyperlinkCellModel());
            this.Model.CellModels.Add("SortableHeaderCell", new PivotGridSortHeaderCellModel()); 
#if !SILVERLIGHT
            this.Model.CellModels.Add("RowGroupingBarCell", new PivotGridRowGroupBarModel());
#endif
            this.Model.Options.WrapCell = true;
            this.Model.TableStyle.CellType = "Static";
            this.Model.Options.AllowExcelLikeResizing = false;

#if !SILVERLIGHT
            this.MouseControllerDispatcher.OverrideMouseCursor = true;
#endif
            this.InitSelectsCellsMouseController();
#if !SILVERLIGHT
            CommandBindings.Add(new System.Windows.Input.CommandBinding(PivotGridCommands.AllowValueSort, AllowValueSortExecuted, AllowValueSortCanExecute));
            CommandBindings.Add(new System.Windows.Input.CommandBinding(PivotGridCommands.ClearValueSorts, ClearValueSortsExecuted, ClearValueSortsCanExecute));
            CommandBindings.Add(new System.Windows.Input.CommandBinding(PivotGridCommands.HideValueColumn, HideValueColumnExecuted, HideValueColumnCanExecute));
            CommandBindings.Add(new System.Windows.Input.CommandBinding(PivotGridCommands.AllowValueFiltering, AllowValueFilteringExecuted, AllowValueFilteringCanExecute));
            CommandBindings.Add(new System.Windows.Input.CommandBinding(PivotGridCommands.ClearValueFilters, ClearValueFiltersExecuted, ClearValueFiltersCanExecute));
#endif

#if SILVERLIGHT
            this.Model.ClipboardCopy += new GridCutPasteEventHandler(Model_ClipboardCopy);
#endif
        }

        #endregion

#if SILVERLIGHT
        void Model_ClipboardCopy(object sender, GridCutPasteEventArgs e)
        {
            PivotModelTextDataExchange textDataExchange = new PivotModelTextDataExchange(this.Model);
            textDataExchange.Engine = this.PivotEngine;
            this.Model.TextDataExchange = textDataExchange;

            if (this.Model.SelectedRanges.Count > 0 && this.Model.SelectedRanges[0].IsTable)
            {
                GridRangeInfoList list = GetExpandedRange(this.Model.SelectedRanges.Clone());
                if (this.Model.GridCopyPaste != null)
                {
                    // Copy the Cell models to Data Object. 
                    GridCellData data = this.Model.CutPaste.CopyCellsToDataObject(list, false);
                    this.Model.GridCopyPaste.Copy(data, list);
                }
                else
                {
                    this.Model.CutPaste.CopyRange(list, false, true);
                }
                e.DataObject = null;
                e.Handled = true;
            }
        }
#endif
#if !SILVERLIGHT

        #region [CommandBinding Methods]
        private void AllowValueSortCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

         private void AllowValueSortExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PivotGridControlBase grid = e.OriginalSource as PivotGridControlBase;
            if (grid != null && ContextMenuTargetColumnIndex > -1)
            {

                if (ContextMenuTargetColumnIndex < grid.PivotEngine.PivotRows.Count)
                {
                   // PivotItem pivotInfo = grid.PivotEngine.PivotRows.Where(i => i.FieldMappingName == clickedCellInfo.FormattedText).FirstOrDefault();
                    int col = grid.PivotEngine.ResolveColumnIndex(ContextMenuTargetColumnIndex);
                    PivotItem pivotInfo = grid.PivotEngine.PivotRows[col];
                    if (pivotInfo != null)
                    {
                        pivotInfo.AllowSort = !pivotInfo.AllowSort;
                        this.InvalidateCell(GridRangeInfo.Cell(0, contextMenuTargetColumnIndex));
                    }
                }
                else
                {
                   // PivotComputationInfo pivotComputationInfo = grid.PivotEngine.PivotCalculations.Where(i => i.FieldName == clickedCellInfo.FormattedText).FirstOrDefault();
                    int col = grid.PivotEngine.ResolveColumnIndex(ContextMenuTargetColumnIndex);
                    PivotComputationInfo pivotComputationInfo = grid.PivotEngine.PivotCalculations[col - grid.PivotEngine.PivotRows.Count];
                    if (pivotComputationInfo != null)
                    {
                        pivotComputationInfo.AllowSort = !pivotComputationInfo.AllowSort;
                        this.InvalidateCell(GridRangeInfo.Cell(0, contextMenuTargetColumnIndex));
                    }
                }
            }
        }

        private void ClearValueSortsCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.PivotEngine.AnyValueColumnsSorted();
            e.Handled = true;
        }
        /// <summary>
        /// Method used to clear the sorting applied on value cells when RowPivotsOnly mode is enabled
        /// </summary>
        public void ClearValueSorts()
        {
            if (GridControl.RowPivotsOnly)
            {
                ClearValueSorts();
            }
        }
        private void ClearValueSortsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ClearSorts();

            this.InvalidateCells();
        }

        private void ClearSorts()
        {
            foreach (int colIndex in sortHeaderList)
            {
                int loc = this.PivotEngine.ResolveColumnIndex(colIndex);
                if (loc > -1)
                {
                    if (this.PivotEngine.GetSortDirection(loc) == ListSortDirection.Descending)
                    {
                        SortColumnWhenRowPivotsOnly(loc, false, ListSortDirection.Descending);
                    }
                }
            }
            this.PivotEngine.ClearSorts();
            this.sortHeaderList.Clear();
        }

        private string GetFieldHeaderOrFieldName(PivotComputationInfo info)
        {
            return info == null ? "" : (info.FieldHeader != null && info.FieldHeader != "" ? info.FieldHeader : info.FieldName);
        }

        private void AllowValueFilteringCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void AllowValueFilteringExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PivotGridControlBase grid = sender as PivotGridControlBase;
            if (grid != null && ContextMenuTargetColumnIndex > -1)
            {

                if (contextMenuTargetColumnIndex < grid.PivotEngine.PivotRows.Count)
                {
                    //PivotItem pivotInfo = grid.PivotEngine.PivotRows.Where(i => i.FieldMappingName == clickedCellInfo.FormattedText).FirstOrDefault();
                    int col = grid.PivotEngine.ResolveColumnIndex(ContextMenuTargetColumnIndex);
                    PivotItem pivotInfo = grid.PivotEngine.PivotRows[col];
                    if (pivotInfo != null)
                    {
                        pivotInfo.AllowFilter = !pivotInfo.AllowFilter;
                        this.InvalidateCell(GridRangeInfo.Cell(0, ContextMenuTargetColumnIndex));
                    }
                }
                else
                {
                  //  PivotComputationInfo pivotComputationInfo = grid.PivotEngine.PivotCalculations.Where(i => i.FieldName == clickedCellInfo.FormattedText).FirstOrDefault();
                    int col = grid.PivotEngine.ResolveColumnIndex(ContextMenuTargetColumnIndex);
                    PivotComputationInfo pivotComputationInfo = grid.PivotEngine.PivotCalculations[col - grid.PivotEngine.PivotRows.Count];
                    if (pivotComputationInfo != null)
                    {
                        pivotComputationInfo.AllowFilter = !pivotComputationInfo.AllowFilter;
                        this.InvalidateCell(GridRangeInfo.Cell(0, ContextMenuTargetColumnIndex));
                    }
                }
            }
        }

        private void ClearValueFiltersCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;// ColumnFilterPopup.lastFilteredColumn.Count > 0;
            e.Handled = true;
        }

        private void ClearValueFiltersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.ClearFilters();
            this.InvalidateCells();
        }

        private void HideValueColumnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void HideValueColumnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PivotComputationInfo pivotComputationInfo = (this.GridControl.PivotCalculations.Where(i => GetFieldHeaderOrFieldName(i) == clickedCellInfo.FormattedText).Select(i => i).FirstOrDefault() as PivotComputationInfo);
            if (pivotComputationInfo != null)
            {
                SetValueColumnVisibility(pivotComputationInfo.FieldName, true);
                if (GridControl.LocalPossibleCalculations != null && GridControl.LocalPossibleCalculations.Count > 0)
                {
                    PivotValueField pv = GridControl.LocalPossibleCalculations.Where(pvf => pvf.FieldName == pivotComputationInfo.FieldName).FirstOrDefault();
                    if (pv != null)
                    {
                        pv.IsSelected = false;
                    }
                }
            }
        }

        #endregion

#endif
        /// <summary>
        /// Expands the Range list when it is Table, Rows, Cols to Cells. 
        /// </summary>
        /// <param name="rangeList">Range List to Expand.</param>
        /// <returns>Expanded Cells Range</returns>
        /// <remarks>When  we Select Entire Table, Column, Rows etc.. Expanded Ranges is must.</remarks>
        private GridRangeInfoList GetExpandedRange(GridRangeInfoList rangeList)
        {
            int headerRowCount = this.Model.HeaderRows;
            int headerColCount = this.Model.HeaderColumns;

            if (rangeList.Count > 0)
            {
                // If Selected Range is Table then expand it as cells.
                if (rangeList[0].IsTable)
                {
                    rangeList = rangeList.ExpandRanges(rangeList[0].Top, rangeList[0].Left, Model.RowCount, Model.ColumnCount);
                }
                return rangeList;
            }

            return null;
        }

        #region [ Private Members ]
        private bool m_AllowSelection;
        private bool showSubTotals = true;
#if SILVERLIGHT
        private ResourceWrapper resourceWrapper = new ResourceWrapper();
#endif
        #endregion

        #region [ Internal Properties ]

        internal List<HiddenGroup> HiddenRowGroupStore { get; set; }
        internal List<HiddenGroup> HiddenColumnGroupStore { get; set; }

        #endregion

        #region [ Dependency property declaration ]

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridControl"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridControl.GridControl"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridControlProperty =
            DependencyProperty.Register("GridControl", typeof(PivotGridControl), typeof(PivotGridControlBase), new PropertyMetadata(null, OnGridControlChanged));

        /// <summary>
        /// Gets or sets the PivotGridControl.
        /// </summary>
        public PivotGridControl GridControl
        {
            get { return (PivotGridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// Gets the PivotEngine wired to PivotGridControl.
        /// </summary>
        public PivotEngine PivotEngine
        {
            get
            {
                if (this.GridControl
                    != null)
                {
                    return this.GridControl.PivotEngine;
                }
                return null;
            }
        }

        
#if SILVERLIGHT
        private bool loadInBackground = false;
        /// <summary>
        /// Gets or Sets the value for the LoadInBackground property
        /// </summary>
        public bool LoadInBackground
        {
            get { return loadInBackground; }
            set
            {
                loadInBackground = value;
                if (PivotEngine != null)
                    PivotEngine.LoadInBackground = loadInBackground;
            }
        }
#endif
        /// <summary>
        /// Gets the sortoption for PivotGrid Control
        /// </summary>
        public PivotSortOption SortOption
        {
            get
            {
                return this.GridControl.SortOption;
            }
        }
        private bool allowFilter;

        /// <summary>
        /// Gets or sets a value whether to enable filtering or not.
        /// </summary>
        public bool AllowFilter
        {
            get { return allowFilter; }
            set { allowFilter = value; }
        }
        
        /// <summary>
        /// Gets the bounded data source.
        /// </summary>
        public object ItemSource
        {
            get
            {
                return this.GridControl.ItemSource;
            }
        }
#if !SILVERLIGHT

        private Pen moveColumnHeaderBorder = new Pen(Brushes.Red, 2);
        /// <summary>
        /// Gets or sets Pen for ColumnHeaderBorder
        /// </summary>
        public Pen MoveColumnHeaderBorder
        {
            get { return moveColumnHeaderBorder; }
            set { moveColumnHeaderBorder = value; }
        }

        private Brush moveColumnHeaderForeground = Brushes.DarkGray;
        /// <summary>
        /// Gets or sets brush for ColumnHeader foreground
        /// </summary>
        public Brush MoveColumnHeaderForeground
        {
            get { return moveColumnHeaderForeground; }
            set { moveColumnHeaderForeground = value; }
        }

        private Brush moveColumnHeaderBackground = Brushes.LightGray;
        /// <summary>
        /// Gets or sets brush for ColumnHeader background
        /// </summary>
        public Brush MoveColumnHeaderBackground
        {
            get { return moveColumnHeaderBackground; }
            set { moveColumnHeaderBackground = value; }
        }


        /// <summary>
        /// Gets or sets whether to enable hyperlinks only while hovering the value cell and disables on leaving it. Used internally.
        /// </summary>
        internal bool EnableHyperlinkOnMouseOver
        {
            get { return this.GridControl.EnableHyperlinkOnMouseOver; }
        }
#endif

        internal List<HiddenGroup> HiddenRowGroups { get; set; }

        internal List<HiddenGroup> HiddenColumnGroups { get; set; }

        internal List<HiddenGroup> HiddenSubTotalsRowGroups { get; set; }

        internal List<HiddenGroup> HiddenSubTotalsColumnGroups { get; set; }
     
      /// <summary>
      /// Returns whether the cell specified is a collapsed expander cell.
      /// </summary>
      /// <param name="info">PivotCellInfo</param>
      /// <param name="index">int</param>
      /// <param name="isRowPivot">bool</param>
      /// <param name="pi">PivotItem</param>
      /// <returns>True, if collapsed;False, otherwise.</returns>
        public bool IsGroupCollapsed(PivotCellInfo info, int index, bool isRowPivot, PivotItem pi)
        {
            bool b = false;
            if (isRowPivot)
            {
                if(info.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell) && 
                    ((info.CellRange.Bottom > info.CellRange.Top && Model.RowHeights[index] == 0) || pi.ShowSubTotal))
                {
                    HiddenGroup g = HiddenRowGroups.Where(h => h.From == info.CellRange.Top && h.To == info.CellRange.Bottom).FirstOrDefault();
                    if (g != null)
                    {
                        b = true;
                    }
                }
            }
            else
            {
                if (info.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell) && 
                    ((info.CellRange.Right > info.CellRange.Left && Model.ColumnWidths[index] == 0) || pi.ShowSubTotal))
                {
                    HiddenGroup g = HiddenColumnGroups.Where(h => h.From == info.CellRange.Left && h.To == info.CellRange.Right).FirstOrDefault();
                    if (g != null)
                    {
                        b = true;
                    }
                }
            }

            return b;
        }

        /// <summary>
        /// Used to clear hidden subtotals on the row pivots or the column pivots when the passed parameter is true.
        /// </summary>
        /// <param name="isRowPivot">Indicates whether to clear hidden subtotals on the row pivots or the column pivots.</param>
        public void ClearHiddenSubtotals(bool isRowPivot)
        {
            if (isRowPivot)
            {
                HiddenSubTotalsRowGroups.Clear();
            }
            else
            {
                HiddenSubTotalsColumnGroups.Clear();
            }
        }
       
        /// <summary>
        /// Gets or sets a value indicating whether [show subtotals].
        /// </summary>
        /// <value><c>true</c> if [show subtotals]; otherwise, <c>false</c>.</value>
        internal bool ShowSubTotals
        {
            get { return showSubTotals; }
            set
            {
                if (showSubTotals != value)
                {
                    showSubTotals = value;
                    SubTotalsRendering(); 
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow selection].
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        internal bool AllowSelection
        {
            get
            {
                return m_AllowSelection;
            }
            set
            {
                if (!value)
                {
                    //if (this.Model.Selections.Count > 0)
                    {
                        this.Model.Selections.Clear();
                        this.Model.Options.AllowSelection = GridSelectionFlags.None;
                        this.Model.Options.ExcelLikeCurrentCell = false;
                        this.Model.Options.ExcelLikeSelectionFrame = false;
                        m_AllowSelection = value;
                    }
                }
                else
                {
                    this.Model.Options.ExcelLikeCurrentCell = true;
                    this.Model.Options.ExcelLikeSelectionFrame = true;
                    this.Model.Options.AllowSelection = GridSelectionFlags.Any;
                    m_AllowSelection = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        internal SelectedItems SelectedItems { get; set; }

        private bool _statePersistenceEnabled;
        internal bool StatePersistenceEnabled
        {
            get
            {
                return _statePersistenceEnabled;
            }
            set
            {
                _statePersistenceEnabled = value;
                if (!_statePersistenceEnabled)
                {
                    this._listOfCollapsedCells = null;
                }
            }
        }

        #endregion

        #region [ Dependency property changed event ]
        /// <summary>
        /// Calls when the value of the GridControl changes
        /// </summary>
        /// <param name="dependencyObject">The Grid Control</param>
        /// <param name="args">An event argument</param>
        public static void OnGridControlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridControlBase gridControlBase = (PivotGridControlBase)dependencyObject;
            if (gridControlBase.PivotEngine != null && !DesignerProperties.GetIsInDesignMode(gridControlBase))
            {
                gridControlBase.PivotEngine.HiddenPivotRowGroups = new Dictionary<int, List<HiddenGroup>>();
                gridControlBase.PivotEngine.HiddenPivotColumnGroups = new Dictionary<int, List<HiddenGroup>>();
                gridControlBase.PivotEngine.PivotSchemaChanged += new PivotSchemaChangedEventHandler(gridControlBase.PivotEngine_PivotSchemaChanged);
                if(gridControlBase.ItemSource != null)
                    gridControlBase.Refresh(true);
            }
        }

        void PivotEngine_PivotSchemaChanged(object sender, PivotSchemaChangedArgs e)
        {
            SynchronizeGrid(e);
        }

        internal void SynchronizeGrid(PivotSchemaChangedArgs e)
        {
            if (!this.GridControl.DeferLayoutUpdate || e.OverrideDeferLayoutUpdate)
            {
                if (this.PivotEngine.UseIndexedEngine)
                {
                    e.ChangeHints = SchemaChangeHints.None;
                }
#if SILVERLIGHT
                if (!this.GridControl.RefreshFromGroupingBar)
                {
                    if (e.ChangeHints == SchemaChangeHints.GrandTotalVisibility)
                    {
                        this.GridControl.UpdateGridLayout = true;
                        this.Refresh(false);
                    }
                    else
                    {
                        this.Refresh(true);
                    }
                }
                else
                {
                    if (!this.GridControl.IgnoreRefesh)
                    {
                        if (e.ChangeHints != SchemaChangeHints.GrandTotalVisibility)
                        {
                            this.ConditionalRefresh();
                        }
                    }
                    ////TODO: Since pivot grid control in Silverlight doesn't has pivot schema designer support and pivot schema related events are handling through grid grouping bar. 
                    ////      Hence, InvalidateCollapsedCells method called only in RefreshFromGroupingBar condition. In future, this method call will move accordingly (as in WPF).
                    this.InvalidateCollapsedCells();
                    if (this.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                    {
                        foreach (PivotItem item in this.GridControl.PivotColumns)
                        {
                            this.SubTotalVisibilityRenderer(item);
                        }
                        foreach (PivotItem item in this.GridControl.PivotRows)
                        {
                            this.SubTotalVisibilityRenderer(item);
                        }
                    }
                }
                
#else
                if (e.ChangeHints == SchemaChangeHints.GrandTotalVisibility)
                {
                    this.GridControl.UpdateGridLayout = true;
                    this.Refresh(false);
                }
                else if (e.ChangeHints == SchemaChangeHints.CalculationChanged)
                {
                    this.PivotEngine.PopulateValueCells();
                    this.InvalidateCells();
                }
                else if (e.ChangeHints == SchemaChangeHints.HeadersChanged)
                {
                    //this.PivotEngine.Populate();
                    this.PivotEngine.RefreshItemProperties();
                    Populate();
                    this.InvalidateCells();
                }
                else
                {
                    this.Refresh(true);
                }

                this.InvalidateCollapsedCells();
#endif
            }
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Handles the SelectionChanging event of the Model control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridSelectionChangingEventArgs"/> instance containing the event data.</param>
        void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (this.PivotEngine != null)
            {
                if (e.Range != null && e.Range != GridRangeInfo.Empty && (e.Range.Bottom > 0 || e.Range.Left > 0))
                {
                    this.FillSelectedItems(e.Range);
                    this.GridControl.RaiseSelectionEvent(new PivotGridSelectionChangedEventArgs(e.Range, this.SelectedItems, e.Reason));
                }
            }
        }

        #endregion

        #region [ Override ]
#if !SILVERLIGHT

        #region moving columns during RowPivotsOnly

        bool isInColumnMoved = false;
        bool isInLeftMouseDown = false;
        Point pointEmpty = new Point(0, 0);
        Point leftMouseDownPoint = new Point(0, 0);
        int columnUnderMouse = -1;
        int mouseDownColumn = -1;
        int lastColumnDrawn = -1;
        bool addOnRight = false;
       
        /// <summary>
        /// Calls before KeyDown event occurs 
        /// </summary>
        /// <param name="e">An event argument</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (PivotEngine.RowPivotsOnly && isInColumnMoved && e.Key == System.Windows.Input.Key.Escape)
            {
                EndColumnMove();
                this.InvalidateCell(GridRangeInfo.Row(0));
                 this.InvalidateVisual();
                e.Handled = true;
            }
        }
        /// <summary>
        /// Calls before the event MouseleftButtonDown occurs
        /// </summary>
        /// <param name="e">An event argument</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {

            base.OnPreviewMouseLeftButtonDown(e);

            isInColumnMoved = false;
            if (autoScrollTimer != null)
            {
                autoScrollTimer.Stop();
                autoScrollTimer.Tick -= autoScrollTimer_Tick;
                autoScrollTimer = null;
            }
            isInLeftMouseDown = false;
            if (PivotEngine.RowPivotsOnly && !e.Handled)
            {
                Point pt = e.GetPosition(this);
                if (pt.X > 0 && pt.X <= this.ActualWidth && pt.Y >= 0 && pt.Y <= this.Model.RowHeights[0])
                {
                    if (!(e.OriginalSource is Image || e.OriginalSource is Path || e.OriginalSource is GridControlBase || e.OriginalSource is TextBlock))
                    {
                        return;//clicked in filter popup
                    }
                    RowColumnIndex cell = this.PointToCellRowColumnIndex(pt);
                    if (cell.RowIndex == 0 && cell.ColumnIndex >= this.PivotEngine.PivotRows.Count)
                    {
                        leftMouseDownPoint = e.GetPosition(this);
                        isInLeftMouseDown = true;
                        columnUnderMouse = mouseDownColumn = cell.ColumnIndex;
                    }
                }
            }
        }
        double autoScrollIncrement = 100;
        int autoScrollTimerIncrement = 100;
        double lastMoveAmount = 100;
        Point lastMovePoint = new Point();
        Point lastColMovePoint = new Point();
        System.Windows.Threading.DispatcherTimer autoScrollTimer = null;
        /// <summary>
        /// Gets or sets the scroll increment that controls the amount of scroll that occurs
        /// when you try to drag a column header to a non-visible location when RowPivotsOnly is true.
        /// </summary>
        public double AutoScrollIncrement
        {
            get { return autoScrollIncrement; }
            set { autoScrollIncrement = value; }
        }
        /// <summary>
        /// Gets or sets the maximum time increment in milliseconds that occurs between scrolls
        /// when you try to drag a column header to a non-visible location when RowPivotsOnly is true.
        /// </summary>
        public int AutoScrollTimerIncrement
        {
            get { return autoScrollTimerIncrement; }
            set { autoScrollTimerIncrement = value; }
        }
        void autoScrollTimer_Tick(object sender, EventArgs e)
        {
            if (lastMovePoint == lastColMovePoint)
            {
                this.ScrollColumns.ScrollBar.Value += lastMoveAmount;
            }
        }
        /// <summary>
        /// An overridden method  to handle mouse movement in prior
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (!isInLeftMouseDown || !PivotEngine.RowPivotsOnly || Mouse.LeftButton == System.Windows.Input.MouseButtonState.Released)
                return;

            if (!isInColumnMoved)
            {
                Point pt = e.GetPosition(this);
                if (Math.Abs(pt.X - leftMouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance)
                {
                    isInColumnMoved = true; 
                    if (autoScrollTimer == null)
                    {
                        autoScrollTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(AutoScrollTimerIncrement) };
                        autoScrollTimer.Tick += autoScrollTimer_Tick;
                        autoScrollTimer.Start();
                    }
                }
            }
            if (isInColumnMoved)
            {
                Point pt = e.GetPosition(this);
                lastMovePoint = pt;
                RowColumnIndex cell = this.PointToCellRowColumnIndexOutsideCells(pt, false);
                if (cell.RowIndex == 0)
                {
                    int col = cell.ColumnIndex;

                    if (pt.X + 30 > this.ActualWidth && col < this.Model.ColumnCount)
                    {
                        this.ScrollColumns.ScrollBar.Value += autoScrollIncrement;
                        lastColMovePoint = pt;
                        lastMoveAmount = autoScrollIncrement;
                    }
                    else if (col < this.PivotEngine.PivotRows.Count && this.ScrollColumns.ScrollBar.Value >= this.PivotEngine.PivotRows.Count)
                    {
                        this.ScrollColumns.ScrollBar.Value -= autoScrollIncrement;
                        lastColMovePoint = pt;
                        lastMoveAmount = -autoScrollIncrement;
                    }

                    if (col >= this.PivotEngine.PivotRows.Count)
                        columnUnderMouse = col;
                    addOnRight = columnUnderMouse == ScrollColumns.LastBodyVisibleLineIndex && pt.X > this.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, GridRangeInfo.Cell(0, columnUnderMouse), false, false).Right - 30;

                    if (columnUnderMouse != lastColumnDrawn || addOnRight)
                    {
                        this.InvalidateCell(GridRangeInfo.Row(0));
                        this.InvalidateVisual(true);
                        this.InvalidateMeasure();
                        this.UpdateLayout();
                        lastColumnDrawn = columnUnderMouse;
                    }
                }
                
            }
        }
        /// <summary>
        /// Occurs when any mouse button is released. 
        /// </summary>
        /// <param name="e">An event argument</param>
        protected override void OnPreviewMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PivotEngine.RowPivotsOnly)
            {
                if (isInColumnMoved)
                {
                    bool moved = false;
                    if (addOnRight)
                        columnUnderMouse += 1;
                    if (columnUnderMouse != mouseDownColumn)
                    {
                        int targetDrop = mouseDownColumn < columnUnderMouse ? columnUnderMouse - 1 : columnUnderMouse;
                        this.PivotEngine.AdjustColumnIndexes(mouseDownColumn, targetDrop);
                        this.Model.MoveColumns(mouseDownColumn, 1, targetDrop);
                        if (GridControl.LocalPossibleCalculations != null && GridControl.LocalPossibleCalculations.Count > 0)
                        {
                            int from = mouseDownColumn - PivotEngine.PivotRows.Count;
                            int to = columnUnderMouse - PivotEngine.PivotRows.Count;
                            if (from < GridControl.LocalPossibleCalculations.Count && to < GridControl.LocalPossibleCalculations.Count)
                            {
                                if (from >= 0 && to >= 0)
                                {
                                    var v = GridControl.LocalPossibleCalculations[from];
                                    GridControl.LocalPossibleCalculations.Insert(to, v);
                                }
                                if (from > to && GridControl.LocalPossibleCalculations.Count > from + 1)
                                    GridControl.LocalPossibleCalculations.RemoveAt(from + 1);
                                else
                                    GridControl.LocalPossibleCalculations.RemoveAt(from);
                            }
                        }
                        moved = true;
                    }
                    EndColumnMove();
                    if (moved)
                    {
                        this.InvalidateCells();
                    }
                    else
                    {
                        this.InvalidateCell(GridRangeInfo.Row(0));
                        this.InvalidateVisual();
                        e.Handled = true; //avoid sorting if on same cell
                    }
                }
            }
            isInLeftMouseDown = false;
            base.OnPreviewMouseUp(e);
        }

        private void EndColumnMove()
        {
            isInLeftMouseDown = false;
            isInColumnMoved = false;
            if (autoScrollTimer != null)
            {
                autoScrollTimer.Stop();
                autoScrollTimer.Tick -= autoScrollTimer_Tick;
                autoScrollTimer = null;
            }
            columnUnderMouse = -1;
            lastColumnDrawn = -1;
            mouseDownColumn = -1;
            addOnRight = false;
                  
        }

        #endregion
#endif
#if !SILVERLIGHT
        
        private bool loadInBackground = false;
        /// <summary>
        /// Gets or Sets the value for the LoadInBackground property
        /// </summary>
        internal bool LoadInBackground
        {
            get { return loadInBackground; }
            set
            {
                loadInBackground = value;
                if (PivotEngine != null)
                    PivotEngine.LoadInBackground = loadInBackground;
            }
        }
#endif

        
#if !SILVERLIGHT
        /// <summary>
        /// Calls when the cells are rendering
        /// </summary>
        /// <param name="dc">DrawingContext</param>
        protected override void OnRender(DrawingContext dc)
        {
            if (!PivotEngine.LoadInBackground)
                base.OnRender(dc);
            if (PivotEngine.LoadInBackground)
            {
                if (PivotEngine.PopulationStatus > 0)
                {
                    GridControl.BusyIndicator.IsBusy = true;
                }
            }
        }


#endif
        #region [ QueryCellInfo ]
        /// <summary>
        /// Raises the <see cref="E:QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventArgs"/> instance containing the event data.</param>
     
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (PivotEngine.LoadInBackground)
            {
                e.Style.Background = new SolidColorBrush(Colors.DarkGray);
                e.Style.CellType = "Static";
                return;
            }
            base.OnQueryCellInfo(e);
            if (e.Handled)
                return;
            if (this.GridControl != null && this.PivotEngine != null &&
        e.Cell.RowIndex < this.PivotEngine.RowCount && e.Cell.ColumnIndex < this.PivotEngine.ColumnCount && (this.PivotEngine.ColumnCount > 1 && this.PivotEngine.RowCount > 1))
            {
                var styleInfo = e.Style;
                PivotCellInfo cellInfo = this.PivotEngine[e.Cell.RowIndex, e.Cell.ColumnIndex];
                bool hideValue = false;
                if (cellInfo != null)
                {
#if !SILVERLIGHT
                    string name = GetNameAt(e.Cell.ColumnIndex);
#endif
                    if (e.Cell.RowIndex == 0)
                    {
                     //   if (sortedValueColumns.ContainsKey(name))
                        {
#if !SILVERLIGHT
                            if (this.GridControl.RowPivotsOnly)
                            {
                                if (e.Cell.ColumnIndex < this.PivotEngine.PivotRows.Count)
                                {
                                    if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.Style != null)
                                    {
                                        e.Style.CellType = "TemplateCell";
                                        this.SetCellValue(styleInfo, cellInfo, this.GridControl.ColumnHeaderCellStyle.Style);
                                    }
                                    else
                                    {
                                        cellInfo.CellType = PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell;
                                        cellInfo.FormattedText = this.GridControl.PivotRows[e.Cell.ColumnIndex].FieldHeader;
                                        this.SetCellValue(styleInfo, cellInfo, null);
                                    }
                                }
                                e.Style.CellType = "SortableHeaderCell";
                                this.SetCellValue(styleInfo, cellInfo, null);
                                this.ApplyStyle(e.Style, this.PivotEngine[styleInfo.RowIndex, styleInfo.ColumnIndex], e.Cell.ColumnIndex);


                                if (isInColumnMoved && e.Style.ColumnIndex == columnUnderMouse)
                                {
                                    if (addOnRight)
                                    {
                                        e.Style.Borders.Right = MoveColumnHeaderBorder;
                                    }
                                    else
                                    {
                                        e.Style.Borders.Left = MoveColumnHeaderBorder;
                                    }
                                }
                                if (isInColumnMoved && e.Style.ColumnIndex == mouseDownColumn)
                                {
                                    e.Style.Foreground = MoveColumnHeaderForeground;
                                    e.Style.Background = MoveColumnHeaderBackground;

                                }
                            }
#else
                            if (cellInfo.CellType != PivotCellType.TopLeftCell)
                            {
                                e.Style.CellType = "SortableHeaderCell";
                                this.SetCellValue(styleInfo, cellInfo, null);
                                this.ApplyStyle(e.Style, this.PivotEngine[styleInfo.RowIndex, styleInfo.ColumnIndex], e.Cell.ColumnIndex);
                            }
         
#endif
                         }
#if !SILVERLIGHT
                        if (isInColumnMoved)
                        {
                            return;
                        }
#endif
                    }
#if !SILVERLIGHT
                    if (cellInfo.CellType == PivotCellType.TopLeftCell && !this.GridControl.RowPivotsOnly)
                  
#else
                    if (cellInfo.CellType == PivotCellType.TopLeftCell)
#endif
                    {

#if !SILVERLIGHT
                        e.Style.Background = Brushes.Transparent;
#else

                        e.Style.Background = new SolidColorBrush(Colors.Transparent);
#endif
                        if (this.GridControl.ShowGroupingBar)
                        {
#if !SILVERLIGHT
                            e.Style.CellType = "RowGroupingBarCell";
                            e.Style.Borders.All = new Pen(this.GridControl.GridLineStroke, 0);
#endif
                        }
                        else
                        {
#if SILVERLIGHT
                            e.Style.Borders.Left = new Pen(this.GridControl.GridLineStroke, 1);
                            e.Style.Borders.Top = new Pen(this.GridControl.GridLineStroke, 1);
                            e.Style.Borders.Right = new Pen(this.GridControl.GridLineStroke, 0);
                            e.Style.Borders.Bottom = new Pen(this.GridControl.GridLineStroke, 0);


#else
                            e.Style.Borders.Left = new Pen(this.GridControl.GridLineStroke, 0.5);
                            e.Style.Borders.Top = new Pen(this.GridControl.GridLineStroke, 0.5);
                            e.Style.Borders.Right = new Pen(this.GridControl.GridLineStroke, 0);
                            e.Style.Borders.Bottom = new Pen(this.GridControl.GridLineStroke, 0);
                            //e.Style.Borders.All = new Pen(this.GridControl.GridLineStroke, 1);
#endif
                        }
                        return;
                    }
#if !SILVERLIGHT

                    if (this.GridControl.RowPivotsOnly && e.Cell.RowIndex == 0 && e.Cell.ColumnIndex < this.GridControl.PivotRows.Count)
                    {
                        if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.Style != null)
                        {
                            e.Style.CellType = "TemplateCell";
                            this.SetCellValue(styleInfo, cellInfo, this.GridControl.ColumnHeaderCellStyle.Style);
                        }
                        else
                        {
                            cellInfo.CellType = PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell;
                            cellInfo.FormattedText = this.GridControl.PivotRows[e.Cell.ColumnIndex].FieldHeader;
                            this.SetCellValue(styleInfo, cellInfo, null);
                        }
                        this.ApplyStyle(styleInfo, cellInfo, e.Cell.ColumnIndex);
                        return;
                    }


                    else if (this.GridControl.RowPivotsOnly && e.Cell.RowIndex > 0)
                    {
                        PivotItem item = this.PivotEngine.PivotRows.Find(p => p.FieldMappingName == name);
                        if (item != null)
                        {
                            if (item.EnableHyperlinks && cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell))
                                e.Style.CellType = "HyperlinkCell";
                        }

                        PivotComputationInfo info = this.PivotEngine.PivotCalculations.Find(s => s.FieldName == name);
                        if (info != null)
                        {
                            if (info.EnableHyperlinks && cellInfo.CellType == PivotCellType.ValueCell)
                                e.Style.CellType = "HyperlinkCell";
                        }
                    }
#endif
                    if (cellInfo.Tag == null)
                    {
                        switch (cellInfo.CellType)
                        {
                            case (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell):
                            case (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell | PivotCellType.GrandTotalCell):
                                if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.Style != null)
                                {
                                    e.Style.CellType = "TemplateCell";
                                    this.SetCellValue(styleInfo, cellInfo, this.GridControl.RowHeaderCellStyle.Style);
                                }
                                else
                                {
                                    this.SetCellValue(styleInfo, cellInfo, null);
                                }
                                break;

                            case (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell):
                            case (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell):
                            case (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell | PivotCellType.GrandTotalCell):
                                if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.Style != null)
                                {
                                    e.Style.CellType = "TemplateCell";
                                    this.SetCellValue(styleInfo, cellInfo, this.GridControl.ColumnHeaderCellStyle.Style);
                                }
                                else
                                {
                                    this.SetCellValue(styleInfo, cellInfo, null);
                                }
                                break;

                            case (PivotCellType.TotalCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                            case (PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):

                                if (this.GridControl.SummaryHeaderStyle != null && this.GridControl.SummaryHeaderStyle.Style != null)
                                {
                                    e.Style.CellType = "TemplateCell";
                                    this.SetCellValue(styleInfo, cellInfo, this.GridControl.SummaryHeaderStyle.Style);
                                }
                                else
                                {
                                    this.SetCellValue(styleInfo, cellInfo, null);
                                }
                                break;
                            case (PivotCellType.ValueCell|PivotCellType.TotalCell|PivotCellType.GrandTotalCell):
                            case (PivotCellType.ValueCell | PivotCellType.TotalCell):
                            case (PivotCellType.ValueCell | PivotCellType.GrandTotalCell):

                                hideValue = CheckForInnerMostOnly(PivotEngine.ResolveColumnIndex(e.Style.ColumnIndex));

                                if (this.GridControl.SummaryCellStyle != null && this.GridControl.SummaryCellStyle.Style != null)
                                {
                                    e.Style.CellType = "TemplateCell";
                                    this.SetCellValue(styleInfo, cellInfo, this.GridControl.SummaryCellStyle.Style);
                                }
                                else
                                {
                                    this.SetCellValue(styleInfo, cellInfo, null);
                                }
                                break;

                            case PivotCellType.ValueCell:
                                if (this.GridControl.ValueCellStyle != null && this.GridControl.ValueCellStyle.Style != null)
                                {
                                    e.Style.CellType = "TemplateCell";
                                    this.SetCellValue(styleInfo, cellInfo, this.GridControl.ValueCellStyle.Style);
                                }
                                else
                                {
                                    this.SetCellValue(styleInfo, cellInfo, null);

                                }
                                break;
                        }

                        this.ApplyStyle(styleInfo, cellInfo, e.Cell.ColumnIndex);
                        if (hideValue)
                        {
                            styleInfo.CellValue = "";
                        }
                    }
                    else
                    {
                        //// Applying collapsed cell data
                        styleInfo = cellInfo.Tag as GridStyleInfo;

                        if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()))
                        {
                            if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.Style != null)
                            {
                                e.Style.CellType = "TemplateCell";
                                e.Style.CellIdentity = new PivotGridStyleInfoIdentity(e.Style.CellIdentity)
                                {
                                    IsExpanded = false,
                                    PivotCellInfo = cellInfo,
                                    Style = this.GridControl.RowHeaderCellStyle.Style
                                };
                                e.Style.CellValue = cellInfo.FormattedText;
                            }
                            else
                            {
                                e.Style.CellType = "ExpanderCell";
                                e.Style.CellIdentity = new PivotGridStyleInfoIdentity(e.Style.CellIdentity)
                                {
                                    IsExpanded = false,
                                    PivotCellInfo = cellInfo,
                                    IsHyperlinkCell = this.GridControl.RowHeaderCellStyle == null ? false : this.GridControl.RowHeaderCellStyle.IsHyperlinkCell,
                                    EnableContextMenu = this.GridControl.RowHeaderCellStyle == null ? false : this.GridControl.RowHeaderCellStyle.EnableContextMenu,
                                    ToolTipEnabled = this.GridControl.RowHeaderCellStyle == null ? false: this.GridControl.RowHeaderCellStyle.ToolTipEnabled
                                };
#if !SILVERLIGHT

                                if (this.GridControl.RowPivotsOnly)
                                {
                                    if (cellInfo.CellRange.Left < this.GridControl.PivotRows.Count)
                                    {
                                        e.Style.CellIdentity = new PivotGridStyleInfoIdentity(e.Style.CellIdentity)
                                        {
                                            IsExpanded = false,
                                            PivotCellInfo = cellInfo,
                                            IsHyperlinkCell = this.GridControl.PivotRows[cellInfo.CellRange.Left].EnableHyperlinks,
                                            EnableContextMenu = this.GridControl.RowHeaderCellStyle == null ? false : this.GridControl.RowHeaderCellStyle.EnableContextMenu,
                                            ToolTipEnabled = this.GridControl.RowHeaderCellStyle == null ? false : this.GridControl.RowHeaderCellStyle.ToolTipEnabled
                                        };
                                    }
                                }
#endif
                                e.Style.CellValue = cellInfo.FormattedText;
                            }
                        }
                        else if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()))
                        {
                            if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.Style != null)
                            {
                                e.Style.CellType = "TemplateCell";
                                this.SetCellValue(e.Style, cellInfo, this.GridControl.ColumnHeaderCellStyle.Style);
                            }
                            else
                            {
                                e.Style.CellType = "ExpanderCell";
                                e.Style.CellIdentity = new PivotGridStyleInfoIdentity(e.Style.CellIdentity)
                                {
                                    IsExpanded = false,
                                    PivotCellInfo = cellInfo,
                                    IsHyperlinkCell = this.GridControl.ColumnHeaderCellStyle == null ? false : this.GridControl.ColumnHeaderCellStyle.IsHyperlinkCell,
                                    EnableContextMenu = this.GridControl.ColumnHeaderCellStyle == null ? false : this.GridControl.ColumnHeaderCellStyle.EnableContextMenu,
                                    ToolTipEnabled = this.GridControl.ColumnHeaderCellStyle == null ? false : this.GridControl.ColumnHeaderCellStyle.ToolTipEnabled
                                };
                                e.Style.CellValue = cellInfo.FormattedText;
                            }

                        }
                        else
                        {
                            e.Style.CellType = "ExpanderCell";
                            e.Style.CellIdentity = new PivotGridStyleInfoIdentity(e.Style.CellIdentity)
                            {
                                IsExpanded = false,
                                PivotCellInfo = cellInfo,
                                IsHyperlinkCell = GetValue(cellInfo),
                                EnableContextMenu =  GetEnableContextMenuValue(cellInfo),
                                ToolTipEnabled = GetToolTipEnabledValue(cellInfo, styleInfo)
                            };
                            e.Style.CellValue = cellInfo.FormattedText;
                        }

                        this.ApplyStyle(e.Style, this.PivotEngine[styleInfo.RowIndex, styleInfo.ColumnIndex], e.Cell.ColumnIndex);
                    }

                    //if (cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ValueCell)
                    //    || cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell))
                    //{
                        if (cellInfo.Value != null && cellInfo.ParentCell == null)
                        {
                            bool canApplyConditionalFormat = false;
                            foreach (var condition in this.GridControl.ConditionalFormats)
                            {
                                switch (condition.ValueCellType)
                                {
                                    case PivotGridValueCellType.ValueCell:
                                        if (cellInfo.CellType == PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.SummaryCell:
                                        if (cellInfo.CellType == (PivotCellType.ValueCell| PivotCellType.TotalCell))
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.GrandTotalCell:
                                        if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.SummaryValueCell:
                                        if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType== PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.GrandTotalValueCell:
                                        if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell) || cellInfo.CellType == PivotCellType.ValueCell)
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.GrandTotalSummaryValueCell:
                                        if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                            canApplyConditionalFormat = true;
                                        break;
                                    case PivotGridValueCellType.All:
                                        if (cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ValueCell) || cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell)
                                         || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell | PivotCellType.TotalCell))
                                            canApplyConditionalFormat = true;
                                        break;
                                    default:
                                        canApplyConditionalFormat = false;
                                        break;
                                }
                                if (canApplyConditionalFormat)
                                {
                                    string associatedMeasure = GetAssociatedMeasure(cellInfo, e.Cell.RowIndex, e.Cell.ColumnIndex);
                                    if (associatedMeasure != null)
                                    {
                                        bool applyFormat = condition.ApplyFormat(cellInfo, associatedMeasure);
                                        if (applyFormat)
                                        {
                                            this.ApplyStyle(e.Style, condition.CellStyle);
                                            //code used to apply CellTemplate for conditional formats.
                                            if (condition.CellStyle != null && condition.CellStyle.Style != null)
                                            {
                                                e.Style.CellType = "TemplateCell";
                                                this.SetCellValue(e.Style, cellInfo, condition.CellStyle.Style);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    //}
                }
            }
            
#if SILVERLIGHT
            //this.GridControl.IsProcessing = false;
#endif
            if (!this.GridControl.ShowExpanderForSinglePivot)
            {
                if (e.Cell.ColumnIndex < this.GridControl.PivotRows.Count - 1 && PivotEngine.ColumnCount > 0 && !this.GridControl.PivotRows[e.Cell.ColumnIndex].ShowSubTotal && e.Cell.RowIndex + 1 < this.PivotEngine.RowCount)
                {
                    var coveredcellsInfo = this.CoveredCells.GetCellSpan(e.Cell.RowIndex+1, e.Cell.ColumnIndex);
                    PivotCellInfo cellInfo = this.PivotEngine[e.Cell.RowIndex+1, e.Cell.ColumnIndex];
                    if (coveredcellsInfo != null && 
                        this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, this.GridControl.PivotRows[e.Cell.ColumnIndex].TotalHeader)))
                    {
                        e.Style.CellType = "Static";
                        e.Style.TextMargins.Left = 20;
                    }
                   
                }
                else if (e.Cell.RowIndex < this.GridControl.PivotColumns.Count - 1 && !this.GridControl.PivotColumns[e.Cell.RowIndex].ShowSubTotal
                    && (e.Cell.ColumnIndex + (PivotEngine.PivotColumns.Count - (e.Cell.RowIndex + 1))) < PivotEngine.ColumnCount)
                {
                    var coveredcellsInfo = this.CoveredCells.GetCellSpan(e.Cell.RowIndex, e.Cell.ColumnIndex + (PivotEngine.PivotColumns.Count - (e.Cell.RowIndex + 1)));
                    PivotCellInfo cellInfo = this.PivotEngine[e.Cell.RowIndex, e.Cell.ColumnIndex + (PivotEngine.PivotColumns.Count - (e.Cell.RowIndex + 1))];
                    if (coveredcellsInfo != null && 
                        this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, this.GridControl.PivotColumns[e.Cell.RowIndex].TotalHeader)))
                    {
                        e.Style.CellType = "Static";
                        e.Style.TextMargins.Left = 20;
                    }
                }
            }
        }

        private bool CheckForInnerMostOnly(int col)
        {
            bool b = false;
            if (this.PivotEngine.PivotCalculations.Count > 0 && col >= this.PivotEngine.PivotRows.Count)
            {
                PivotComputationInfo info = this.PivotEngine.PivotCalculations[(col - this.PivotEngine.PivotRows.Count) % this.PivotEngine.PivotCalculations.Count];

                b = info.InnerMostComputationsOnly == SummaryDisplayLevel.InnerMostOnly;
            }
            return b;
        }

        #endregion

        /// <summary>
        /// Hiding the cell border for header cells
        /// </summary>
        /// <returns></returns>
        protected override bool ShouldRenderCurrentCellBorder()
        {
            var currentCell = this.CurrentCell;
            if (currentCell.Renderer != null && currentCell.Renderer.HasCurrentCellState)
            {
                //// identifying the value cells and applying the selections
                var identity = this.CurrentCell.Renderer.CurrentStyle.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
                if (identity != null &&
                    identity.PivotCellInfo.CellType == PivotCellType.ValueCell)
                {
                    return base.ShouldRenderCurrentCellBorder();
                }
                return false;
            }

            return base.ShouldRenderCurrentCellBorder();            
        }

        /// <summary>
        /// Raises the <see cref="E:ResizingColumns"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridResizingColumnsEventArgs"/> instance containing the event data.</param>
        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            if (this.GridControl.AllowResizeColumns)
            {
                base.OnResizingColumns(args);
                VisibleLineInfo lineInfo = this.ScrollRows.GetVisibleLineAtPoint(args.Point.Y);
                if (this.Model != null && lineInfo != null && lineInfo.LineIndex < this.Model.HeaderRows)
                {
#if !SILVERLIGHT
                    this.MouseControllerDispatcher.OverrideMouseCursor = false;
#endif
                    args.AllowResize = true;
                }
#if !SILVERLIGHT
                else if (this.Model != null && args.Columns.Right < this.Model.HeaderColumns && !PivotEngine.RowPivotsOnly)
                {

                    this.MouseControllerDispatcher.OverrideMouseCursor = false;

                    args.AllowResize = true;
                }
#endif
                else
                {
#if !SILVERLIGHT
                    this.MouseControllerDispatcher.OverrideMouseCursor = false;
#endif
                }
            }
            else
            {
#if !SILVERLIGHT
                this.MouseControllerDispatcher.OverrideMouseCursor = true;
#endif
                args.AllowResize = false;
            }
            if (args.Reason != GridResizeCellsReason.HitTest && !args.Columns.IsEmpty)
            {
                if (args.Reason == GridResizeCellsReason.DoubleClick)
                {
                    VisibleLineInfo rowlineInfo = this.ScrollRows.GetVisibleLineAtPoint(args.Point.Y);
                    var rowindex = rowlineInfo.VisibleIndex;
                    var columnindex = args.Columns.Left;
                    PivotCellInfo cellInfo = this.PivotEngine[rowindex, columnindex];
                    int maxLength = this.Model.RowCount;
                    if (this.Model.Options.MaxLength > 0 && this.Model.Options.MaxLength <= this.Model.RowCount)
                    {
                        maxLength = this.Model.Options.MaxLength;
                    }
                    var selectedrowindex = rowindex;
                    var columnindexNextRow = columnindex;
                    var leftvaluelimit = args.Columns.Left;
                    var rightvaluelimit = args.Columns.Right;
                    int columnindexcount = 0;
                    if (!this.GridControl.ShowCalculationsAsColumns && this.GridControl.PivotCalculations != null)
                    {
                        columnindexcount = this.GridControl.PivotColumns.Count+1 ;
                    }
                    else
                    {
                        columnindexcount = this.GridControl.PivotColumns.Count;
                    }
                    int left = args.Columns.Left;
                    int right = args.Columns.Right;
                    if (cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)
                        ||(cellInfo.CellType==(PivotCellType.TotalCell|PivotCellType.CalculationHeaderCell|PivotCellType.ColumnHeaderCell))
                        || (cellInfo.CellType==(PivotCellType.ColumnHeaderCell|PivotCellType.GrandTotalCell) && !this.GridControl.ShowCalculationsAsColumns))
                    {
                        if (left > 1 && this.Model.ColumnWidths[left] > 0)
                        {
                            var range = GridRangeInfo.Cells(0, left, maxLength, right);
                            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                        }
                    }
                    else if ((cellInfo.CellType == (PivotCellType.HeaderCell|PivotCellType.ColumnHeaderCell|PivotCellType.GrandTotalCell)  && this.GridControl.ShowCalculationsAsColumns))
                    {
                        for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                        {
                            if (left > 1 && this.Model.ColumnWidths[left] > 0 )
                            {
                                var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                            }
                            left = left - 1;
                            right = right - 1;
                        }
                    }
                    else if ((cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) || cellInfo.CellType == (PivotCellType.ValueCell)))
                    {
                        if (this.GridControl.ShowSubTotals && this.GridControl.ShowCalculationsAsColumns && this.GridControl.PivotCalculations != null)
                        {
                            for (int p = 0; p < this.GridControl.PivotColumns.Count; p++)
                            {
                                left = leftvaluelimit;
                                right = rightvaluelimit;
                                columnindex = columnindexNextRow;
                                var uniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex - 1].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.PivotValues.Count; q++)
                                {
                                    if (rowindex >= 0 && rowindex < this.GridControl.PivotColumns.Count && columnindex > columnindexcount)
                                    {
                                        var currentuniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex - 1].Value;
                                        if (uniquetext == currentuniquetext && currentuniquetext != null)
                                        {
                                            if (this.GridControl.ShowCalculationsAsColumns && rowindex == this.GridControl.PivotColumns.Count - 1)
                                            {
                                                for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                                                {
                                                    if (left > 1 && this.Model.ColumnWidths[left] > 0 && left > ColumnIndexLimit)
                                                    {
                                                        var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                        this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                        left = left - 1;
                                                        right = right - 1;
                                                    }
                                                    
                                                    else 
                                                    {
                                                        break;
                                                    }
                                                }
                                                if (rowindex == selectedrowindex)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (left > 1 && this.Model.ColumnWidths[left] > 0 && left > ColumnIndexLimit)
                                                {
                                                    var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                    this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                }
                                            }

                                            if (currentuniquetext != uniquetext)
                                            {
                                                if (rowindex == selectedrowindex)
                                                {
                                                    ColumnIndexLimit = columnindex;
                                                    break;
                                                }
                                                else
                                                {
                                                    columnindex = columnindex - 1;
                                                    uniquetext = this.GridControl.PivotEngine[rowindex, columnindex - 1].Value;
                                                    columnindex = columnindex + 1;
                                                    if (ColumnIndexLimit < columnindex)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            columnindex = columnindex - 1;
                                            left = left - 1;
                                            right = right - 1;
                                        }
                                        if (columnindex == columnindexcount)
                                        {
                                            ColumnIndexLimit = columnindex;
                                            if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                            {
                                                var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                            }
                                            break;
                                        }
                                    }
                                }
                                rowindex = rowindex + 1;
                            }
                        }
                        else if (cellInfo.CellType == (PivotCellType.ValueCell))
                        {
                            for (int p = 0; p < this.GridControl.PivotColumns.Count; p++)
                            {
                                left = leftvaluelimit;
                                right = rightvaluelimit;
                                columnindex = columnindexNextRow;
                                var uniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.PivotValues.Count; q++)
                                {
                                    if (rowindex >= 0 && rowindex < this.GridControl.PivotColumns.Count && columnindex > columnindexcount)
                                    {
                                        var currentuniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex].Value;
                                        if (uniquetext == currentuniquetext)
                                        {
                                            if (this.GridControl.ShowCalculationsAsColumns && rowindex == this.GridControl.PivotColumns.Count - 1)
                                            {
                                                for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                                                {
                                                    if (left > 1 && this.Model.ColumnWidths[left] > 0 && left > ColumnIndexLimit)
                                                    {
                                                        var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                        this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                        left = left - 1;
                                                        right = right - 1;
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }

                                                if (rowindex == selectedrowindex)
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                                {
                                                    var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                    this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (rowindex == selectedrowindex)
                                            {
                                                if (left > 1 && this.Model.ColumnWidths[left] > 0 && left > ColumnIndexLimit)
                                                {
                                                    var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                    this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                }
                                                ColumnIndexLimit = columnindex;
                                                break;
                                            }
                                            else
                                            {
                                                columnindex = columnindex - 1;
                                                uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                columnindex = columnindex + 1;
                                                if (ColumnIndexLimit < columnindex)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        columnindex = columnindex - 1;
                                        left = left - 1;
                                        right = right - 1;
                                    }
                                    if (columnindex == columnindexcount)
                                    {
                                        ColumnIndexLimit = columnindex;
                                        if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                        {
                                            var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                        }
                                        break;
                                    }
                                }
                                rowindex = rowindex + 1;
                            }
                        }
                        else
                        {
                            for (int p = 0; p < this.GridControl.PivotColumns.Count; p++)
                            {
                                left = leftvaluelimit;
                                right = rightvaluelimit;
                                columnindex = columnindexNextRow;
                                var uniquetext = this.GridControl.PivotEngine[rowindex, columnindex - 1].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.ColumnCount; q++)
                                {
                                    if (rowindex >= 0 && rowindex < this.GridControl.PivotColumns.Count && columnindex > columnindexcount)
                                    {
                                        var currentuniquetext = this.GridControl.PivotEngine[rowindex, columnindex - 1].Value;
                                        if (uniquetext == currentuniquetext)
                                        {
                                            if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                            {
                                                var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                            }
                                        }
                                        if (currentuniquetext != null)
                                        {
                                            if (rowindex == selectedrowindex)
                                            {
                                                if (left > 1 && this.Model.ColumnWidths[left] > 0 )
                                                {
                                                    var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                    this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                }
                                                ColumnIndexLimit = columnindex;
                                                break;
                                            }
                                            else
                                            {
                                                columnindex = columnindex - 1;
                                                uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                columnindex = columnindex + 1;
                                                if (ColumnIndexLimit < columnindex)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        columnindex = columnindex - 1;
                                        left = left - 1;
                                        right = right - 1;
                                    }
                                    if (columnindex == columnindexcount)
                                    {
                                        ColumnIndexLimit = columnindex;
                                        if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                        {
                                            var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                        }
                                        break;
                                    }
                                }
                                rowindex = rowindex + 1;
                            }
                        }

                    }
                    else if (cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell))
                    {
                        if ((this.GridControl.ShowSubTotals && this.GridControl.ShowCalculationsAsColumns)
                            || (!this.GridControl.ShowSubTotals && this.GridControl.ShowCalculationsAsColumns))
                        {
                            if (this.GridControl.PivotCalculations != null)
                            {
                                for (int p = 0; p < this.GridControl.PivotColumns.Count; p++)
                                {
                                    left = leftvaluelimit;
                                    right = rightvaluelimit;
                                    columnindex = columnindexNextRow;
                                    var uniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex - 1].Value;
                                    for (int q = 0; q < this.GridControl.PivotEngine.PivotValues.Count; q++)
                                    {
                                        if (rowindex >= 0 && rowindex < this.GridControl.PivotColumns.Count && columnindex > columnindexcount)
                                        {
                                            var currentuniquetext = this.GridControl.PivotEngine.PivotValues[rowindex, columnindex - 1].Value;

                                            if (uniquetext == currentuniquetext && currentuniquetext != null)
                                            {
                                                if (this.GridControl.ShowCalculationsAsColumns && rowindex == this.GridControl.PivotColumns.Count - 1)
                                                {
                                                    for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                                                    {
                                                        if (left > 1 && this.Model.ColumnWidths[left] > 0 && left > ColumnIndexLimit)
                                                        {
                                                            var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                            left = left - 1;
                                                            right = right - 1;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    if (rowindex == selectedrowindex)
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                                    {
                                                        var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                        this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (rowindex == selectedrowindex)
                                                {
                                                    if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                                    {
                                                        var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                        this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                    }
                                                    ColumnIndexLimit = columnindex;
                                                    break;
                                                }
                                                else
                                                {
                                                    columnindex = columnindex - 1;
                                                    uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                    columnindex = columnindex + 1;
                                                    if (ColumnIndexLimit < columnindex)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            columnindex = columnindex - 1;
                                            left = left - 1;
                                            right = right - 1;
                                        }
                                        if (columnindex == columnindexcount)
                                        {
                                            ColumnIndexLimit = columnindex;
                                            if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                            {
                                                var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                            }
                                            break;
                                        }
                                    }
                                    rowindex = rowindex + 1;
                                }
                            }
                        }


                        else if ((this.GridControl.ShowSubTotals && !this.GridControl.ShowCalculationsAsColumns)
                            || (!this.GridControl.ShowSubTotals && !this.GridControl.ShowCalculationsAsColumns))
                        {
                            for (int p = 0; p < this.GridControl.PivotColumns.Count; p++)
                            {
                                left = leftvaluelimit;
                                right = rightvaluelimit;
                                columnindex = columnindexNextRow;
                                var uniquetext = this.GridControl.PivotEngine[rowindex, columnindex - 1].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.ColumnCount; q++)
                                {
                                    if (rowindex >= 0 && rowindex < this.GridControl.PivotColumns.Count && columnindex > columnindexcount)
                                    {
                                        var currentuniquetext = this.GridControl.PivotEngine[rowindex, columnindex - 1].Value;
                                        if (uniquetext == currentuniquetext)
                                        {
                                            if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                            {
                                                var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                            }
                                        }
                                        if (currentuniquetext != null)
                                        {
                                            if (rowindex == selectedrowindex)
                                            {
                                                if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                                {
                                                    var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                                    this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                                }
                                                ColumnIndexLimit = columnindex;
                                                break;
                                            }
                                            else
                                            {
                                                columnindex = columnindex - 1;
                                                uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                columnindex = columnindex + 1;
                                                if (ColumnIndexLimit < columnindex)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        columnindex = columnindex - 1;
                                        left = left - 1;
                                        right = right - 1;
                                    }
                                    if (columnindex == columnindexcount)
                                    {
                                        ColumnIndexLimit = columnindex;
                                        if (left > 1 && this.Model.ColumnWidths[left] > 0)
                                        {
                                            var range = GridRangeInfo.Cells(0, left, maxLength, right);
                                            this.Model.ResizeColumnsToFit(range, GridResizeToFitOptions.None);
                                        }
                                        break;
                                    }
                                }
                                rowindex = rowindex + 1;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Raises the <see cref="E:ResizingRows"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridResizingRowsEventArgs"/> instance containing the event data.</param>
        protected override void OnResizingRows(GridResizingRowsEventArgs args)
        {
            if (this.GridControl.AllowResizeRows)
            {
                base.OnResizingRows(args);
                VisibleLineInfo lineInfo = this.ScrollColumns.GetVisibleLineAtPoint(args.Point.X);
                if (lineInfo.LineIndex < this.Model.HeaderColumns)
                {
#if !SILVERLIGHT
                    this.MouseControllerDispatcher.OverrideMouseCursor = false;
#endif
                    args.AllowResize = true;
                }
                else if (args.Rows.Top < this.Model.HeaderRows)
                {
#if !SILVERLIGHT
                    this.MouseControllerDispatcher.OverrideMouseCursor = false;
#endif
                    args.AllowResize = true;
                }
                else
                {
#if !SILVERLIGHT
                    this.MouseControllerDispatcher.OverrideMouseCursor = false;
#endif
                }
            }
            else
            {
#if !SILVERLIGHT
                this.MouseControllerDispatcher.OverrideMouseCursor = true;
#endif
                args.AllowResize = false;
            }
            if (args.Reason != GridResizeCellsReason.HitTest && !args.Rows.IsEmpty && args.AllowResize)
            {
                if (args.Reason == GridResizeCellsReason.DoubleClick)
                    
                {
                    VisibleLineInfo columnlineInfo = this.ScrollColumns.GetVisibleLineAtPoint(args.Point.X);
                    var rowindex = args.Rows.Top+1;
                    var rowindexNextColumn = rowindex;                   
                    var columnindex = columnlineInfo.VisibleIndex;
                    var selectedcolumnindex = columnindex;
                    PivotCellInfo cellInfo = this.PivotEngine[rowindex, columnindex];
                    var top = args.Rows.Top;
                    var topvaluenextcolumn=top;
                    var rowheight = args.Height;
                    int rowindexcount = 0;
                    var rowlimit = 0;
                    if (this.GridControl.ShowCalculationsAsColumns && this.GridControl.PivotCalculations != null)
                    {
                        rowindexcount = this.GridControl.PivotRows.Count + 1;
                    }
                    else
                    {
                        rowindexcount = this.GridControl.PivotRows.Count;
                    }
                    if (cellInfo == null || cellInfo.CellType==(PivotCellType.ExpanderCell|PivotCellType.RowHeaderCell))
                    {
                        if (this.GridControl.ShowCalculationsAsColumns)
                        {
                            if (this.Model.RowHeights[top] > 0)
                            {
                                this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                            }
                        }
                        else
                        {
                            for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                            {
                                if (this.Model.RowHeights[top] > 0)
                                {
                                    this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                }
                                top = top - 1;
                            }
                        }

                    }
                   
                    else if ( cellInfo.CellType==(PivotCellType.RowHeaderCell|PivotCellType.GrandTotalCell)|| cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell)
                        || cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell) && (this.GridControl.ShowCalculationsAsColumns))
                    {
                            if (this.Model.RowHeights[top] > 0)
                            {
                                this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                            }
                        
                    }
                    else if ((cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) && !this.GridControl.ShowCalculationsAsColumns) || cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell) && columnindex == this.GridControl.PivotRows.Count - 1)
                    {
                        for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                        {
                            if (this.Model.RowHeights[top] > 0)
                            {
                                this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                            }
                            top = top - 1;
                        }
                    }
                    else if ((((cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) && this.GridControl.ShowCalculationsAsColumns)
                        && top == this.Model.RowCount - 1)) || ((cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell)
                        && top == this.Model.RowCount - 1)))
                    {
                        if (this.Model.RowHeights[top] > 0)
                        {
                            this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                        }
                    }
                    
                    else if ((cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell) && !this.GridControl.ShowCalculationsAsColumns && this.GridControl.PivotCalculations != null || cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell))
                        || (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) && (cellInfo.CellType != (PivotCellType.GrandTotalCell | PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)))
                        || (cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)))
                    {
                        if (this.GridControl.PivotCalculations != null && !this.GridControl.ShowCalculationsAsColumns)
                        {
                            if (columnindex >= 0 && columnindex <= this.GridControl.PivotRows.Count)
                            {
                                for (int p = 0; p < this.GridControl.PivotRows.Count; p++)
                                {
                                    top = topvaluenextcolumn;
                                    rowindex = rowindexNextColumn;
                                    var uniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;
                                    for (int q = 0; q < this.GridControl.PivotEngine.RowCount; q++)
                                    {
                                        if (columnindex >= 0 && columnindex < this.GridControl.PivotRows.Count)
                                        {
                                            if (rowindex >= rowindexcount && rowindex > rowlimit && top>0)
                                            {
                                                var currentuniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;

                                                if (uniquetext == currentuniquetext && currentuniquetext != null)
                                                {
                                                    if (!this.GridControl.ShowCalculationsAsColumns && selectedcolumnindex < this.GridControl.PivotRows.Count)
                                                    {
                                                        for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                                                        {
                                                            if (this.Model.RowHeights[top] > 0)
                                                            {
                                                                this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                            }
                                                            top = top - 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.Model.RowHeights[top] > 0)
                                                        {
                                                            this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                        }
                                                    }
                                                }
                                                if (currentuniquetext != uniquetext)
                                                {
                                                    if (columnindex == selectedcolumnindex)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        rowindex = rowindex - 1;
                                                        uniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;
                                                        rowindex = rowindex + 1;
                                                    }
                                                }
                                                rowindex = rowindex - 1;
                                                top = top - 1;
                                            }
                                        }
                                    }
                                    rowlimit = rowindex;
                                    columnindex = columnindex + 1;
                                }
                            }
                        }
                        else
                        {
                            for (int p = 0; p < this.GridControl.PivotRows.Count; p++)
                            {
                                top = topvaluenextcolumn;
                                rowindex = rowindexNextColumn;
                                var uniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.RowCount; q++)
                                {
                                    if (columnindex >= 0 && columnindex < this.GridControl.PivotRows.Count)
                                    {
                                        if (rowindex >= rowindexcount && rowindex > rowlimit && top > 0)
                                        {
                                            var currentuniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;
                                            if (uniquetext == currentuniquetext)
                                            {
                                                if (this.Model.RowHeights[top] > 0)
                                                {
                                                    this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                }
                                            }
                                            if (currentuniquetext != null)
                                            {
                                                if (columnindex == selectedcolumnindex)
                                                {
                                                    if (this.Model.RowHeights[top] > 0)
                                                    {
                                                        this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    rowindex = rowindex - 1;
                                                    uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                    rowindex = rowindex + 1;
                                                }
                                            }
                                            rowindex = rowindex - 1;
                                            top = top - 1;
                                        }
                                    }
                                }
                                rowlimit = rowindex;
                                columnindex = columnindex + 1;
                            }
                        }

                    }
                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                    {
                        if (this.GridControl.PivotCalculations != null && !this.GridControl.ShowCalculationsAsColumns)
                        {
                            if (columnindex >= 0 && columnindex < this.GridControl.PivotRows.Count)
                            {
                                for (int p = 0; p < this.GridControl.PivotRows.Count; p++)
                                {
                                    top = topvaluenextcolumn;
                                    rowindex = rowindexNextColumn;
                                    var uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                    for (int q = 0; q < this.GridControl.PivotEngine.RowCount; q++)
                                    {
                                        if (columnindex >= 0 && columnindex < this.GridControl.PivotRows.Count)
                                        {
                                            if (rowindex >= rowindexcount && rowindex > rowlimit && top > 0)
                                            {
                                                var currentuniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;

                                                if (uniquetext == currentuniquetext && currentuniquetext != null)
                                                {
                                                    if (!this.GridControl.ShowCalculationsAsColumns && selectedcolumnindex < this.GridControl.PivotRows.Count)
                                                    {
                                                        for (int s = 0; s < this.GridControl.PivotCalculations.Count; s++)
                                                        {
                                                            if (this.Model.RowHeights[top] > 0)
                                                            {
                                                                this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                            }
                                                            top = top - 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (this.Model.RowHeights[top] > 0)
                                                        {
                                                            this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                        }
                                                    }
                                                }
                                                if (currentuniquetext != uniquetext)
                                                {
                                                    if (columnindex == selectedcolumnindex)
                                                    {
                                                        if (this.Model.RowHeights[top] > 0)
                                                        {
                                                            this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                        }
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        rowindex = rowindex - 1;
                                                        uniquetext = this.GridControl.PivotEngine[rowindex - 1, columnindex].Value;
                                                        rowindex = rowindex + 1;
                                                    }
                                                    rowindex = rowindex - 1;
                                                    top = top - 1;
                                                }
                                            }
                                        }
                                        rowlimit = rowindex;
                                        columnindex = columnindex + 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int p = 0; p < this.GridControl.PivotRows.Count; p++)
                            {
                                top = topvaluenextcolumn;
                                rowindex = rowindexNextColumn;
                                var uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                for (int q = 0; q < this.GridControl.PivotEngine.RowCount; q++)
                                {
                                    if (columnindex >= 0 && columnindex < this.GridControl.PivotRows.Count)
                                    {
                                        if (rowindex >= rowindexcount && rowindex > rowlimit && top > 0)
                                        {
                                            var currentuniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                            if (uniquetext == currentuniquetext)
                                            {
                                                if (this.Model.RowHeights[top] > 0)
                                                {
                                                    this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                }
                                            }
                                            if (currentuniquetext != null)
                                            {
                                                if (columnindex == selectedcolumnindex)
                                                {
                                                    if (this.Model.RowHeights[top] > 0)
                                                    {
                                                        this.Model.RowHeights[top] = this.Model.RowHeights.DefaultLineSize;
                                                    }
                                                    break;
                                                }
                                                else
                                                {
                                                    rowindex = rowindex - 1;
                                                    uniquetext = this.GridControl.PivotEngine[rowindex, columnindex].Value;
                                                    rowindex = rowindex + 1;
                                                }
                                            }
                                            rowindex = rowindex - 1;
                                            top = top - 1;
                                        }
                                    }
                                }
                                rowlimit = rowindex;
                                columnindex = columnindex + 1;
                            }
                        }
                    }                   
                }
            }
        }                      
#if !SILVERLIGHT

        /// <summary>
        /// Returns whether a given column has been hidden using the SetValueColumnVisibility API.
        /// </summary>
        /// <param name="fieldName">The field name of the column.</param>
        /// <returns>True if the column is visible and false otherwise.</returns>
        public bool IsColumnVisible(string fieldName)
        {
            int columnIndex = GetColumnIndexFromName(fieldName);
            int dummy = 0;
            return !Model.ColumnWidths.GetHidden(columnIndex, out dummy);
        }

        /// <summary>
        /// When RowPivotOnly is true, this method either hides or shows a column at a given column index. The index is the absolute position
        /// in the PivotGridControl and may include previously hidden columns in its count.
        /// </summary>
        /// <param name="columnIndex">The column index in the grid (taking into account possible hidden columns).</param>
        /// <param name="isHidden">True if you want to hide this column, and false if you want to show it.</param>
        public void SetValueColumnVisibility(int columnIndex, bool isHidden)
        {
            if (!GridControl.RowPivotsOnly || columnIndex < GridControl.PivotRows.Count)
                return;
            
            Model.ColumnWidths.SetHidden(columnIndex, columnIndex, isHidden);
        }

        /// <summary>
        /// When RowPivotOnly is true, this method either hides or shows a column specified by the given mapping name.
        /// </summary>
        /// <param name="fieldName">The mapping name associated with the column.</param>
        /// <param name="isHidden">True if you want to hide this column, and false if you want to show it.</param>
        public void SetValueColumnVisibility(string fieldName, bool isHidden)
        {
            int columnIndex = GetColumnIndexFromName(fieldName);
            
            SetValueColumnVisibility(columnIndex, isHidden);
        }       

        /// <summary>
        /// When RowPivotsOnly is true, use this method to move a calculation column without repopulating
        /// the PivotEngine. The underlying grid will be redrawn during the execution of this method.
        /// </summary>
        /// <param name="from">The index of the old column position.</param>
        /// <param name="to">The index of the new column position.</param>
        public void MoveValueColumn(int from, int to)
        {
            MoveValueColumn(from, to, true);
        }

        /// <summary>
        /// When RowPivotsOnly is true, use this method to move a calculation column without repopulating
        /// the PivotEngine.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="refresh">True if you want the underlying grid to redraw itself. If you are calling
        /// MoveValueColumn multiple times to move several columns, then only set refresh to true for the
        /// last call.
        /// </param>
        public void MoveValueColumn(int from, int to, bool refresh)
        {
            if (!GridControl.RowPivotsOnly || from < GridControl.PivotRows.Count || to < GridControl.PivotRows.Count)
                return;
            int target = from < to ? to - 1 : to;
            PivotEngine.AdjustColumnIndexes(from, target);
            Model.MoveColumns(from, 1, to);
            if (refresh)
            {
                InvalidateCells();
            }
        }
        /// <summary>
        /// When RowPivotsOnly is true, this method sorts the pivot based on the information furnished through the arguments.
        /// </summary>
        /// <param name="sortedColumns">The FieldNames for the columns to sort.</param>
        /// <param name="listSortDirections">The correponding sort orders for the sorted columns.</param>
        public void ApplySavedValueSorts(List<string> sortedColumns, List<ListSortDirection> listSortDirections)
        {
            if (!GridControl.RowPivotsOnly)
                return;

            ClearSorts();
            sortedValueColumns.Clear();

            if (sortedColumns == null || listSortDirections == null || sortedColumns.Count != listSortDirections.Count)
                return;

            for (int i = 0; i < sortedColumns.Count; ++i)
            {
                string name = sortedColumns[i];
                int colIndex = GetColumnIndexFromName(name);
                if (colIndex > -1)
                {
                    ListSortDirection dir = listSortDirections[i];
                    dir = dir == ListSortDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending;

                    SortColumnWhenRowPivotsOnly(colIndex, i > 0, dir);

                    int resolvedColIndex = PivotEngine.ResolveColumnIndex(GetColumnIndexFromName(name));
                    if (sortedValueColumns.ContainsKey(name))
                    {
                        sortedValueColumns[name] = dir;
                        sortHeaderList.Remove(resolvedColIndex);
                        sortHeaderList.Add(resolvedColIndex);
                    }
                    else
                    {
                        sortedValueColumns.Add(name, dir);
                        sortHeaderList.Add(resolvedColIndex);
                    }
                }
            }
            ReapplyFiltersAfterSort();
            this.InvalidateCells();	  //removed because it caused flashing
            this.Model.ResizeColumnsToFit(GridRangeInfo.Row(0), GridResizeToFitOptions.NoShrinkSize);

        }

      
        /// <summary>
        /// When RowPivotsOnly is true, this method filters the value computation columns using the information in the passed-in dictionary.
        /// </summary>
        /// <param name="exclusions">A dictionary whose keys represent value column mapping names and whose values are
        /// HashSets containing strings that should be filtered out of the corresponding column.</param>
        public void ApplySavedValueFilter(Dictionary<string, HashSet<string>> exclusions)
        {
            if (!PivotEngine.RowPivotsOnly)
                return;
             
            ColumnFilterPopup.FilteredColumnlist.Clear();
            ColumnFilterPopup.Exclusions.Clear();

            foreach (string col in exclusions.Keys)
            {
                List<FilterChoice> popupList = PopulateFilterPopUp(col);

                ColumnFilterPopup.FilterPopUpCollection.Add(col, popupList);

                ColumnFilterPopup.FilteredColumnlist.Add(col);
                ColumnFilterPopup.Exclusions.Add(col, exclusions[col]);

                int colIndex = this.GetColumnIndexFromName(col);
                int startRow = (this.PivotEngine.PivotColumns.Count != 0 ? this.PivotEngine.PivotColumns.Count : 0) + (this.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
                int rowCount = this.PivotEngine.ShowGrandTotals ? this.PivotEngine.RowCount - 1 : this.PivotEngine.RowCount;
                int colIndexKeyLoc = this.PivotEngine.GetHiddenRowKeyValueColumnIndex();

                for (int i = startRow; i < rowCount; i++)
                {
                    PivotCellInfo pci = this.PivotEngine[i, colIndex];
                    PivotCellInfo pciKey = this.PivotEngine[i, colIndexKeyLoc];
                    bool isValue = pci.CellType == PivotCellType.ValueCell;
                    bool isRowPivot = pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell);
                    if (pci != null && (isValue || isRowPivot) && exclusions[col].Contains(pci.FormattedText))
                    {
                        PivotEngine.HiddenRowIndexes.Add(pciKey);
                    }

                }
            }
            ApplyFilters(true);

        }

        List<FilterChoice> PopulateFilterPopUp( string colName)
        {
            HashSet<Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItem> temp = new HashSet<Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItem>();
           
             Dictionary<string, int> colIndexes = new Dictionary<string, int>();
            foreach (string col in ColumnFilterPopup.Exclusions.Keys)
            {
                colIndexes.Add(col, PivotEngine.ResolveColumnIndex(GetColumnIndexFromName(col)));
            }

            int startRow = (PivotEngine.PivotColumns.Count != 0 ? PivotEngine.PivotColumns.Count : 0) + (PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
            int rowCount = PivotEngine.ShowGrandTotals ? PivotEngine.RowCount - 1 : PivotEngine.RowCount;

            int colLookupLoc = GetColumnIndexFromName(colName);
            int loc = colLookupLoc - PivotEngine.PivotRows.Count;
            bool isRowPivot = loc < 0 || (loc >= 0 && PivotEngine.PivotCalculations[loc].SummaryType == SummaryType.DisplayIfDiscreteValuesEqual);
            for (int i = startRow; i < rowCount; i++)
            {
                PivotCellInfo pci = PivotEngine.PivotValues[i, colLookupLoc];
                if (pci.UniqueText == "x")
                    continue;

                if (pci != null && (pci.CellType == PivotCellType.ValueCell || pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)))
                {
                    bool? b = true;
                    foreach (string col in ColumnFilterPopup.FilteredColumnlist)
                    {
                        int c = colIndexes[col];
                        PivotCellInfo pci1 = PivotEngine.PivotValues[i, colIndexes[col]];
                        if (pci1 != null && ColumnFilterPopup.Exclusions[col].Contains(pci1.FormattedText))
                        {
                            if (col == colName && b == true)
                            {
                                b = false;
                            }
                            else
                            {
                                b = null;
                            }
                        }
                    }
                    if (b != null)
                    {
                        temp.Add(new Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItem() { IsChecked = b, Text = pci.FormattedText, SortKey = isRowPivot ? pci.FormattedText as IComparable : pci.DoubleValue });
                    }
                 }
            }

            List<FilterChoice> tempFilterChoices = new List<FilterChoice>();
            List<Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItem> filterElements = new List<Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItem>(temp);

            FilterChoice filterChoice = null;
            filterElements.Sort(new Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterItemSorter());
            bool nullString = false;
            for (int i = 0; i < filterElements.Count; i++)
            {
                if (filterElements[i].Text != null)
                {
                    tempFilterChoices.Add(filterChoice = new FilterChoice() { Text = filterElements[i].Text, IsChecked = filterElements[i].IsChecked });
                }
                else
                {
                    nullString = true;
                }
            }

            if (nullString)
            {
                tempFilterChoices.Add(filterChoice = new FilterChoice() { Text = "Null", IsChecked = true });
            }
            return tempFilterChoices;
        }



        //This method is used for drag and drop in pivot value chooser
        internal void MoveValueColumn(int from, int to,int target)
        {
            if (!GridControl.RowPivotsOnly || from < GridControl.PivotRows.Count || to < GridControl.PivotRows.Count)
                return;
            PivotEngine.AdjustColumnIndexes(from, target);
            Model.MoveColumns(from, 1, to);
                InvalidateCells();
        }

        /// <summary>
        /// An overridden method to get clicked mouse button to handle mouse right click at computation header cell
        /// </summary>
        /// <param name="args">The event data.</param>
        protected override void OnCellMouseUp(GridCellMouseControllerEventArgs args)
        {
            base.OnCellMouseUp(args);
            if (((System.Windows.Input.MouseButtonEventArgs)(args.MouseControllerEventArgs.SourceEventArgs)).ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                isLeftButtonUp = false;
            }
            else if (((System.Windows.Input.MouseButtonEventArgs)(args.MouseControllerEventArgs.SourceEventArgs)).ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                isLeftButtonUp = true;
            }
        }
        /// <summary>
        /// Calls before Mouse right button up event occur
        /// </summary>
        /// <param name="e">An event argument</param>
        protected override void OnPreviewMouseRightButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            PivotExpanderCell expanderCell = null;
            RowColumnIndex cell = this.PointToCellRowColumnIndex(System.Windows.Input.Mouse.GetPosition(this));
            expanderCell = e.Source as PivotExpanderCell;
            if (expanderCell == null && e.Source is PivotGridControlBase)
            {
                PivotGridControlBase controlBase = e.Source as PivotGridControlBase;
                if (controlBase != null)
                {
                    controlBase.CurrentCell.Activate(cell.RowIndex, cell.ColumnIndex);
                    PivotGridExpandCellRenderer renderer = controlBase.CurrentCell.Renderer as PivotGridExpandCellRenderer;
                    if (renderer != null)
                    {
                        expanderCell = renderer.CurrentCellUIElement;
                        expanderCell.ContextMenu.Items.Clear();
                        expanderCell.ContextMenu.Items.Add(expanderCell.GenerateContextMenuItems());
                        foreach (MenuItem mainObj in expanderCell.ContextMenu.Items)
                        {
                            expanderCell.SetItemProperties(mainObj);
                        }

                        RowPivotsOnlyContextMenuShowingArgs args = new RowPivotsOnlyContextMenuShowingArgs() { Cancel = false, ColumnIndex = cell.ColumnIndex, ContextMenu = expanderCell.ContextMenu, RowIndex = cell.RowIndex };
                        OnRowPivotsOnlyContextMenuShowing(args);
                        if (!args.Cancel)
                        {
                            expanderCell.ContextMenu.IsOpen = true;
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event raised prior to a ContextMenu opening when RowPivotsOnly is true.
        /// </summary>
        public event RowPivotsOnlyContextMenuShowingEvent RowPivotsOnlyContextMenuShowing;
        /// <summary>
        /// Raises the <see cref="E:RowPivotsOnlyContextMenuShowingArgs"/> event.
        /// </summary>
        /// <param name="e">The <see cref="RowPivotsOnlyContextMenuShowingArgs"/> instance containing the event data.</param>
     
      
        protected void OnRowPivotsOnlyContextMenuShowing(RowPivotsOnlyContextMenuShowingArgs e)
        {
            if (PivotEngine.RowPivotsOnly && RowPivotsOnlyContextMenuShowing != null)
            {
                RowPivotsOnlyContextMenuShowing(this, e);
            }
        }

        //used to adjust menu text
        private int contextMenuTargetColumnIndex = -1;
        /// <summary>
        /// Gets or sets the column index of the column that was clicked when this contextmenu was opened.
        /// </summary>
        public int ContextMenuTargetColumnIndex
        {
            get { return contextMenuTargetColumnIndex; }
            set { contextMenuTargetColumnIndex = value; }
        }

        /// <summary>
        /// An overridden method to handle the visibility of items in the context menu being opened.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            Point contextMenuPoint = Mouse.GetPosition(this);
            bool manuallyShowMenu = false;
            ContextMenu cm = null;
            PivotSortHeaderCell sh = null;
            RowColumnIndex cell = this.PointToCellRowColumnIndex(contextMenuPoint);
            if (PivotEngine.RowPivotsOnly)
            {
                if (cell.RowIndex == 0 && this.GridControl.ColumnHeaderCellStyle != null && !this.GridControl.ColumnHeaderCellStyle.EnableContextMenu)
                {
                    e.Handled = true;
                    return;
                }
            }

            contextMenuTargetColumnIndex = cell.ColumnIndex;

            string name = PivotEngine.GetFieldNameAtIndex(cell.ColumnIndex);
            if (GridControl.RowPivotsOnly)
            {
                if (cell.RowIndex == 0 && cell.ColumnIndex >= PivotEngine.PivotRows.Count)
                {
                    PivotComputationInfo info = (this.GridControl.PivotCalculations.Where(i => i.FieldName == name).FirstOrDefault() as PivotComputationInfo);
                    if (info != null)
                    {
                        sh = e.Source as PivotSortHeaderCell;
                        if (sh == null && e.Source is PivotGridControlBase)
                        {//this happens when the click is not on the text...
                            PivotGridControlBase g = e.Source as PivotGridControlBase;
                            if (g != null)
                            {
                                g.CurrentCell.Activate(cell.RowIndex, cell.ColumnIndex);
                                PivotGridSortCellRenderer cr = g.CurrentCell.Renderer as PivotGridSortCellRenderer;
                                if (cr != null)
                                {
                                    sh = cr.CurrentCellUIElement;
                                    manuallyShowMenu = true;
                                }
                            }
                        }
                        if (sh != null)
                        {
                            cm = ((PivotSortHeaderCell)sh).GetContextMenu();
                           AdjustContextMenu(cm, info.AllowSort, info.AllowFilter);
                        }
                    }
                }
                else
                {
                    PivotItem info = (this.GridControl.PivotRows.Where(i => i.FieldMappingName == name).FirstOrDefault() as PivotItem);
                    if (info != null)
                    {
                        sh = e.Source as PivotSortHeaderCell;
                        if (sh == null && e.Source is PivotGridControlBase)
                        {//this happens when the click is not on the text...
                            PivotGridControlBase g = e.Source as PivotGridControlBase;
                            if (g != null)
                            {
                                g.CurrentCell.Activate(cell.RowIndex, cell.ColumnIndex);
                                PivotGridSortCellRenderer cr = g.CurrentCell.Renderer as PivotGridSortCellRenderer;
                                if (cr != null)
                                {
                                    sh = cr.CurrentCellUIElement;
                                    manuallyShowMenu = true;
                                }
                            }
                        }
                        if (sh != null)
                        {
                            cm = ((PivotSortHeaderCell)sh).GetContextMenu();
                            if (this.GridControl.PivotRows.Any(i => i.FieldMappingName == name))
                            {
                                foreach (object obj in cm.Items)
                                {
                                    MenuItem item = obj as MenuItem;
                                    if (item == null && obj is Separator)
                                        continue;
                                    if (item.Tag.ToString() == "Hide Column")
                                    {
                                        item.Visibility = System.Windows.Visibility.Collapsed;
                                    }
                                    if (item.Tag.ToString() == "Allow Sorting" && cell.ColumnIndex < this.GridControl.PivotRows.Count - 1)
                                    {
                                        item.IsEnabled = false;
                                    }
                                    if (item.Tag.ToString() == "Allow Filtering" && cell.ColumnIndex < this.GridControl.PivotRows.Count - 1)
                                    {
                                        item.IsEnabled = false;
                                    }
                                }
                            }
                            AdjustContextMenu(cm, info.AllowSort, info.AllowFilter);
                        }
                    }
                }
            }
            RowPivotsOnlyContextMenuShowingArgs args = new RowPivotsOnlyContextMenuShowingArgs() { Cancel = false, ColumnIndex = cell.ColumnIndex, ContextMenu = cm, RowIndex = cell.RowIndex };
            OnRowPivotsOnlyContextMenuShowing(args);
            if (!args.Cancel)
            {
                base.OnContextMenuOpening(e);
                if (manuallyShowMenu && cm != null)
                {
                    e.Handled = true;
                    cm.PlacementTarget = sh;
                    cm.IsOpen = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void AdjustContextMenu(ContextMenu cm, bool allowSort, bool allowFilter)
        {
            foreach (object o in cm.Items)
            {
                MenuItem mi = o as MenuItem;
                if (mi != null && mi.Tag != null)
                {
                    switch (mi.Tag.ToString())
                    {
                        case "Disable Sorting":
                        case "Allow Sorting":
                            {
                                mi.Tag = allowSort ? "Disable Sorting" : "Allow Sorting";
                                mi.Header = allowSort ? SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "ContextMenu_DisableSorting") : SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "ContextMenu_AllowSorting");
                            }
                            break;
                        case "Clear Sorts":
                            if (!PivotEngine.AnyValueColumnsSorted())
                            {
                                mi.IsEnabled = false;
                            }
                            break;
                        case "Disable Filtering":
                        case "Allow Filtering":
                            {
                                mi.Tag = allowFilter ? "Disable Filtering" : "Allow Filtering";
                                mi.Header = allowFilter ? SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "ContextMenu_DisableFiltering") : SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "ContextMenu_AllowFiltering");
                            }
                            break;
                        case "Clear Filters":
                            if (ColumnFilterPopup.FilterPopUpCollection == null || ColumnFilterPopup.FilterPopUpCollection.Count == 0)
                            {
                                mi.IsEnabled = false;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private bool isLeftButtonUp;
        private PivotCellInfo clickedCellInfo;
        bool isSortingRowPivots = false;
        private Dictionary<string, ListSortDirection> sortedValueColumns = new Dictionary<string, ListSortDirection>();
       
#endif
        static int previousSortRow, previousSortColumn;
        bool flag = false;
        internal List<int> sortHeaderList = new List<int>(); //used to draw sortnumber

        /// <summary>
        /// An overridden method to handle sorting on click over header cells.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnCellClick(GridCellClickEventArgs e)
        {
            base.OnCellClick(e);
#if !SILVERLIGHT
            if (!e.Handled && PivotEngine.RowPivotsOnly)
            {
                if (this.GridControl.PivotCalculations.Count == 0 || !this.GridControl.RowPivotsOnly || e.RowIndex != 0)
                    return;

                PivotCellInfo cellInfo = clickedCellInfo = this.PivotEngine[e.RowIndex, e.ColumnIndex];

                if (!isLeftButtonUp)
                {
                    return;
                }
                bool allowSort = false;
                if (e.ColumnIndex < PivotEngine.PivotRows.Count)
                {
                    PivotItem pi = PivotEngine.PivotRows[e.ColumnIndex];
                    allowSort = pi != null && pi.AllowSort;
                }
                else
                {
                    PivotComputationInfo pivotComputationInfo = (this.GridControl.PivotCalculations.Where(i => GetFieldHeaderOrFieldName(i) == cellInfo.FormattedText).FirstOrDefault() as PivotComputationInfo);
                    allowSort = pivotComputationInfo != null && pivotComputationInfo.AllowSort;
                }
                if (!allowSort)
                    return;
            if (cellInfo != null &&
                           (!cellInfo.CellType.ToString().Contains("ValueCell")) && (!cellInfo.CellType.ToString().Contains("ExpanderCell")) &&
                           (SortOption != PivotSortOption.None) &&
                           (((cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell) && PivotEngine.PivotCalculations.Count == 1) || cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)) && (SortOption == PivotSortOption.ColumnSorting || SortOption == PivotSortOption.All)) ||
                               ((((cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && PivotEngine.PivotCalculations.Count == 1) || cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)) && (SortOption == PivotSortOption.TotalSorting || SortOption == PivotSortOption.All)) ||
                                (((cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) && (PivotEngine.PivotCalculations.Count == 1 || GridControl.RowPivotsOnly)) || (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.HeaderCell) &&( (PivotEngine.PivotColumns.Count == 1 && PivotEngine.PivotCalculations.Count == 1) || GridControl.RowPivotsOnly))
                                || cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell)) && (SortOption == PivotSortOption.GrandTotalSorting || SortOption == PivotSortOption.All))))
            
                {
                    this.Cursor = System.Windows.Input.Cursors.Wait;

                    bool multiColumn = System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl) ||
                                       System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl);
                    if (!multiColumn)
                    {
                        sortedValueColumns.Clear();
                        sortHeaderList.Clear();
                    }

                    SortColumnWhenRowPivotsOnly(e.ColumnIndex, multiColumn, null);
                    string name = GetNameAt(e.ColumnIndex);
                    ListSortDirection dir = PivotEngine.GetSortDirection(e.ColumnIndex);
                    
                    int resolvedColIndex = PivotEngine.ResolveColumnIndex(e.ColumnIndex);
                    if (sortedValueColumns.ContainsKey(name))
                    {
                        sortedValueColumns[name] = dir;
                        sortHeaderList.Remove(resolvedColIndex);
                        sortHeaderList.Add(resolvedColIndex);
                    }
                    else
                    {
                        sortedValueColumns.Add(name, dir);
                        sortHeaderList.Add(resolvedColIndex);
                    }  
                    ReapplyFiltersAfterSort();

                    this.InvalidateCells();	  
                    this.Model.ResizeColumnsToFit(GridRangeInfo.Row(0), GridResizeToFitOptions.NoShrinkSize);
                    this.Cursor = System.Windows.Input.Cursors.Arrow;
                }
            }
            else
#endif
            {
                if (flag)
                {
                    this.Model[previousSortRow, previousSortColumn].CellType = "HeaderCell";
                }
                if (!e.Handled && e.ColumnIndex > 0 || e.RowIndex > 0)
                {
                    PivotCellInfo cellInfo = this.PivotEngine[e.RowIndex, e.ColumnIndex];
                    if (cellInfo != null && !cellInfo.CellType.ToString().Contains("ValueCell") && (!cellInfo.CellType.ToString().Contains("ExpanderCell")) &&
                          (SortOption != PivotSortOption.None))
                    {

                        if ((((cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell) && PivotEngine.PivotCalculations.Count == 1) || cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)) && (SortOption == PivotSortOption.ColumnSorting || SortOption == PivotSortOption.All)) ||
                             ((((cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) && PivotEngine.PivotCalculations.Count == 1) || cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)) && (SortOption == PivotSortOption.TotalSorting || SortOption == PivotSortOption.All)) ||
                              (((cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) && PivotEngine.PivotCalculations.Count == 1) || (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.HeaderCell) && PivotEngine.PivotColumns.Count == 1 && PivotEngine.PivotCalculations.Count == 1) || cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell)) && (SortOption == PivotSortOption.GrandTotalSorting || SortOption == PivotSortOption.All))))
                        {
                            PivotEngine.SortByCalculation(e.ColumnIndex);
                            previousSortRow = e.RowIndex;
                            previousSortColumn = e.ColumnIndex;
                            flag = true;

                            this.Model[e.RowIndex, e.ColumnIndex].CellType = "SortableHeaderCell";
                            this.InvalidateCells();
                        }
                    }
                }
            }

        }

#if !SILVERLIGHT

        private void SortColumnWhenRowPivotsOnly(int columnIndex, bool multiColumn, ListSortDirection? dir)
        {
            bool needToReset = columnIndex < PivotEngine.PivotRows.Count;
            List<int> saveColIndexes = null;
            List<bool> hiddenState = null;
            if (needToReset)
            {
                isSortingRowPivots = true;
                saveColIndexes = new List<int>(PivotEngine.columnIndexes);
                hiddenState = new List<bool>(PivotEngine.columnIndexes.Count);
                int dummy = 0;
                for (int k = 0; k < PivotEngine.columnIndexes.Count; ++k)
                {
                    hiddenState.Add(this.Model.ColumnWidths.GetHidden(k, out dummy));
                }
            }
            PivotEngine.RowPivotsOnly = this.GridControl.RowPivotsOnly; //should be set to allow sorting of non-innermost row pivots
            if (dir == null)
            {
                PivotEngine.SortByCalculation(columnIndex, multiColumn);
            }
            else
            {
                PivotEngine.SortByCalculation(columnIndex, multiColumn, dir.Value);
            }
            if (needToReset)
            {
                isSortingRowPivots = false;
                ResetHiddenIndexes();
                PivotEngine.columnIndexes = saveColIndexes;
                for (int k = 0; k < PivotEngine.columnIndexes.Count; ++k)
                {
                    this.Model.ColumnWidths.SetHidden(k, k, hiddenState[k]);
                }
            }
        }


        private void ResetHiddenIndexes()
        {
            PivotEngine.HiddenRowIndexes.Clear();
            int keyCol = PivotEngine.GetHiddenRowKeyValueColumnIndex();
            for (int i = 1; i < PivotEngine.RowCount; ++i)
            {
                for (int k = PivotEngine.PivotRows.Count - 1; k < PivotEngine.ColumnCount; ++k)
                {
                    if (PivotEngine.PivotValues[i, k] != null)
                    {
                        string key = k >= PivotEngine.PivotRows.Count ? PivotEngine.PivotCalculations[k - PivotEngine.PivotRows.Count].FieldName : PivotEngine.PivotRows[k].FieldMappingName;
                        string val = PivotEngine.PivotValues[i, k].FormattedText;
                        if (ColumnFilterPopup.Exclusions.ContainsKey(key))
                        {
                            if (ColumnFilterPopup.Exclusions[key].Contains(val))
                            {
                                PivotEngine.HiddenRowIndexes.Add(PivotEngine.PivotValues[i, keyCol]);
                                break;
                            }
                        }
                    }
                }
            }
        }
        internal int GetColumnIndexFromRowHeaderName(string rowHeaderName)
        {
            int loc = -1;
            for (int i = 0; i < PivotEngine.PivotRows.Count; i++)
                if ((PivotEngine.PivotRows[i] as PivotItem).FieldMappingName == rowHeaderName)
                {
                    loc = i;
                    break;
                }

            return loc;
        }

        /// <summary>
        /// A method to get the column index in pivot engine from field name.
        /// </summary>
        /// <param name="fieldName">Name of the pivot item</param>
        /// <returns>column index</returns>
        public int GetColumnIndexFromName(string fieldName)
        {
            int loc = -1;
            if (PivotEngine.PivotRows.Count > 0 && PivotEngine.PivotRows[PivotEngine.PivotRows.Count - 1].FieldMappingName == fieldName)
            {
                return PivotEngine.PivotRows.Count - 1;
            }
            if(PivotEngine.PivotCalculations.Any(i=>i.FieldName == fieldName))
            {
                for (int i = PivotEngine.PivotRows.Count; i < PivotEngine.ColumnCount; ++i)
                {
                    if (PivotEngine.columnIndexes != null && i >= PivotEngine.columnIndexes.Count)
                        continue;
                    int ii = PivotEngine.columnIndexes != null ? PivotEngine.columnIndexes[i] : i;
                    ii -= PivotEngine.PivotRows.Count;
                    if (PivotEngine.PivotCalculations[ii].FieldName == fieldName)
                    {
                        loc = i;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < PivotEngine.PivotRows.Count; i++)
                    if ((PivotEngine.PivotRows[i] as PivotItem).FieldMappingName == fieldName)
                    {
                        loc = i;
                    }
            }

            return loc;
        }

        internal string GetNameAt(int colIndex)
        {
            if (GridControl.RowPivotsOnly)
            {
                if (colIndex < GridControl.PivotRows.Count)
                    return GridControl.PivotRows[colIndex].FieldMappingName;
                else if (colIndex < PivotEngine.ColumnCount)
                {
                    int loc = (PivotEngine.columnIndexes != null && colIndex < PivotEngine.columnIndexes.Count) ? PivotEngine.columnIndexes[colIndex] : colIndex;
                    if((loc - GridControl.PivotRows.Count) < GridControl.PivotCalculations.Count)
                        return GridControl.PivotCalculations[loc - GridControl.PivotRows.Count].FieldName;
                }
            }
            return null; //need to think about this alternative
        }

#endif
        #endregion

        #region [ Cell Styler ]

        /// <summary>
        /// Applying CellValue for current style
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="CellStyleInfo">Holds cell style information</param>
        private void SetCellValue(GridStyleInfo styleInfo, PivotCellInfo cellInfo, Style CellStyleInfo)
        {
            if (cellInfo != null)
            {
                if (cellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell) ||
                    cellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell))
                {
                    if (CellStyleInfo == null)
                    {
                        styleInfo.CellType = "ExpanderCell";
                    }

                    PivotGridStyleInfoIdentity pivotGridStyleInfoIdentity = new PivotGridStyleInfoIdentity(styleInfo.CellIdentity);
                    pivotGridStyleInfoIdentity.IsExpanded = true;
                    pivotGridStyleInfoIdentity.PivotCellInfo = cellInfo;
                    pivotGridStyleInfoIdentity.IsHyperlinkCell = GetValue(cellInfo);
                    pivotGridStyleInfoIdentity.EnableContextMenu = GetEnableContextMenuValue(cellInfo);
                    pivotGridStyleInfoIdentity.ToolTipEnabled = GetToolTipEnabledValue(cellInfo, styleInfo);

                    if (CellStyleInfo != null)
                    {
                        pivotGridStyleInfoIdentity.Style = CellStyleInfo;
                    }
                    styleInfo.CellIdentity = pivotGridStyleInfoIdentity;
                    styleInfo.CellValue = cellInfo.FormattedText;
                }
                else
                {

                    PivotGridStyleInfoIdentity pivotGridStyleInfoIdentity = new PivotGridStyleInfoIdentity(styleInfo.CellIdentity);
                    pivotGridStyleInfoIdentity.IsExpanded = false;
                    pivotGridStyleInfoIdentity.PivotCellInfo = cellInfo;

                    pivotGridStyleInfoIdentity.IsHyperlinkCell = GetValue(cellInfo);
                    pivotGridStyleInfoIdentity.EnableContextMenu = GetEnableContextMenuValue(cellInfo);
                    pivotGridStyleInfoIdentity.ToolTipEnabled = GetToolTipEnabledValue(cellInfo, styleInfo);

                    if (CellStyleInfo != null)
                    {
                        pivotGridStyleInfoIdentity.Style = CellStyleInfo;
                    }

                    if (pivotGridStyleInfoIdentity.IsHyperlinkCell)
                    {
                        styleInfo.CellType = "HyperlinkCell";
                    }

                    styleInfo.CellIdentity = pivotGridStyleInfoIdentity;
#if !SILVERLIGHT
                    if (cellInfo.FormattedText != null)
                    {
                        if (cellInfo.FormattedText.Contains("Total"))
                        {
                            if (cellInfo.FormattedText.Equals("Grand Total"))
                            {
                                //replaces the Grand Total with the given localized string
                                styleInfo.CellValue = rsWrapperKey.pivotGridHeaderGrandTotal;
                            }
                            else
                            {
                                // replaces the Total with the given localized string
                                string subTotalstring = cellInfo.FormattedText.Replace("Total", rsWrapperKey.pivotGridHeaderSubTotal);
                                styleInfo.CellValue = subTotalstring;
                            }
                        }
                        else
                        {
                            styleInfo.CellValue = cellInfo.FormattedText;
                        }
                    }
#else
                    if (cellInfo.FormattedText != null)
                    {
                        if (cellInfo.FormattedText.Contains("Total"))
                        {
                            if (cellInfo.FormattedText.Equals("Grand Total") || cellInfo.FormattedText.Equals("GrandTotal"))
                            {
                                styleInfo.CellValue = resourceWrapper.GrandTotal; 
                            }
                            else
                            {
                                string subTotalString = cellInfo.FormattedText.Replace("Total",resourceWrapper.SubTotal);
                                styleInfo.CellValue = subTotalString;
                            }
                        }
                        else if (cellInfo.FormattedText.Contains("Grandtotal"))
                        {
                            string subTotalString = cellInfo.FormattedText.Replace("Total", resourceWrapper.SubTotal);
                            styleInfo.CellValue = subTotalString;
                        }
                        else
                            styleInfo.CellValue = cellInfo.FormattedText;
                    }
#endif
                }

                if (styleInfo.TooltipTemplateKey == null) styleInfo.TooltipTemplateKey = "pivotTooltipTemplate";
                styleInfo.ShowTooltip = GetToolTipEnabledValue(cellInfo, styleInfo);
                if (GridControl.CustomToolTipTemplateKey != null) styleInfo.TooltipTemplateKey = GridControl.CustomToolTipTemplateKey;

                if (styleInfo.ShowTooltip)
                {
                    if (cellInfo.UniqueText != null)
                    {
#if SILVERLIGHT
                        SetHeaderCellTag(styleInfo, cellInfo, resourceWrapper,cellInfo.UniqueText);
#else
                        SetHeaderCellTag(styleInfo, cellInfo, rsWrapperKey, cellInfo.UniqueText);
#endif
                    }
                    if (cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()) && !cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()))
                    {
                        if (cellInfo.FormattedText == null && this.GridControl.SummaryHeaderStyle.ToolTipEnabled)
                        {
                            Syncfusion.Windows.Controls.Cells.CoveredCellInfo coveredCellCol = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                            if (coveredCellCol != null)
                            {
                                GridStyleInfo style = this.Model[coveredCellCol.Top, coveredCellCol.Left];
                                if (style != null && style.Tag != null)
                                {
                                    styleInfo.Tag = Convert.ToString(style.Tag);
                                    return;
                                }
                            }
                        }
#if SILVERLIGHT
                        SetHeaderCellTag(styleInfo, cellInfo, resourceWrapper,cellInfo.FormattedText);
#else
                        SetHeaderCellTag(styleInfo, cellInfo, rsWrapperKey, cellInfo.FormattedText);
#endif
                    }
                    if ((cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()) && GridControl.ValueCellStyle.ToolTipEnabled && !(cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) || cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString())))
                        || ((cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) || cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString())) && GridControl.SummaryCellStyle.ToolTipEnabled)
                        || ((cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) || cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString())) && GridControl.SummaryHeaderStyle.ToolTipEnabled))
                    {
                        string colText = string.Empty;
                        string rowText = string.Empty;

                        GetRowColHeaderText(styleInfo, cellInfo, out colText, out rowText);
#if SILVERLIGHT
                        string tooltipText = resourceWrapper.ToolTip_Value+": " + (String.IsNullOrEmpty(cellInfo.FormattedText)? "("+resourceWrapper.ToolTip_Empty+")": cellInfo.FormattedText)
                            + Environment.NewLine + ((rowText != String.Empty) ? resourceWrapper.ToolTip_Row+": " + rowText + Environment.NewLine : string.Empty)
                            + ((colText != String.Empty) ? resourceWrapper.ToolTip_Column+": " + colText : string.Empty);
#else
                        string tooltipText = rsWrapperKey.PivotGridToolTipValue + (String.IsNullOrEmpty(cellInfo.FormattedText) ? rsWrapperKey.PivotGridToolTipEmpty : cellInfo.FormattedText)
                            + Environment.NewLine + ((rowText != String.Empty) ? rsWrapperKey.PivotGridToolTipRow + rowText + Environment.NewLine : string.Empty)
                            + ((colText != String.Empty) ? rsWrapperKey.PivotGridToolTipColumn + colText : string.Empty);
#endif
                        if (tooltipText.EndsWith(Environment.NewLine))
                        {
                            tooltipText = tooltipText.Remove(tooltipText.LastIndexOf(Environment.NewLine));
                        }
                        styleInfo.Tag = tooltipText;
                    }
                    if (styleInfo.Tag == null)
                    {
                        if (cellInfo.FormattedText == null)
                        {
                            Syncfusion.Windows.Controls.Cells.CoveredCellInfo coveredCellCol = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                            if (coveredCellCol != null)
                            {
                                GridStyleInfo style = this.Model[coveredCellCol.Top, coveredCellCol.Left];
                                if (style != null && style.Tag != null)
                                {
                                    styleInfo.Tag = Convert.ToString(style.Tag);
                                    return;
                                }
                            }
                        }
#if SILVERLIGHT
                        SetHeaderCellTag(styleInfo, cellInfo, resourceWrapper,cellInfo.FormattedText);

                        if (string.IsNullOrEmpty(cellInfo.FormattedText) && (styleInfo.Tag.ToString().Contains("Row:") || styleInfo.Tag.ToString().Contains("Column:")))
#else
                        SetHeaderCellTag(styleInfo, cellInfo, rsWrapperKey, cellInfo.FormattedText);

                        if (string.IsNullOrEmpty(cellInfo.FormattedText) && (styleInfo.Tag.ToString().Contains(rsWrapperKey.PivotGridToolTipRow) || styleInfo.Tag.ToString().Contains(rsWrapperKey.PivotGridToolTipColumn)))
#endif
                        {
                            string text = string.Empty;
                            Syncfusion.Windows.Controls.Cells.CoveredCellInfo coveredCellCol = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                            if (coveredCellCol != null)
                            {
                                GridStyleInfo style = this.Model[coveredCellCol.Top, coveredCellCol.Left];
                                if (style != null && style.CellValue != null)
                                {
                                    text = Convert.ToString(style.CellValue);
                                }
                            }
                            if ((cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString())))
                            {
#if SILVERLIGHT
                                styleInfo.Tag = "Row: " + text;
#else
                                styleInfo.Tag = rsWrapperKey.PivotGridToolTipRow + text;
#endif
                            }
                            if ((cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString())))
                            {
#if SILVERLIGHT
                                styleInfo.Tag = "Column: " + text;
#else
                                styleInfo.Tag = rsWrapperKey.PivotGridToolTipColumn + text;
#endif
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Header cell Style's Tag property
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="rsWrapperKey">ResourceWrapperKeys</param>
        /// <param name="text">string</param>
#if SILVERLIGHT
        private void SetHeaderCellTag(GridStyleInfo styleInfo, PivotCellInfo cellInfo,ResourceWrapper resourceWrapper,string text)
#else
        private void SetHeaderCellTag(GridStyleInfo styleInfo, PivotCellInfo cellInfo, ResourceWrapperKeys rsWrapperKey, string text)
#endif
        {
            if ((cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString())) && this.GridControl.RowHeaderCellStyle.ToolTipEnabled)
            {
#if SILVERLIGHT
                styleInfo.Tag = resourceWrapper.ToolTip_Row+": " + text;
#else
                styleInfo.Tag = rsWrapperKey.PivotGridToolTipRow + text;
#endif
            }
            if ((cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString())) && GridControl.ColumnHeaderCellStyle.ToolTipEnabled)
            {
#if SILVERLIGHT
                styleInfo.Tag =resourceWrapper.ToolTip_Column+ ": " + text;
#else
                styleInfo.Tag = rsWrapperKey.PivotGridToolTipColumn + text;
#endif
            }
        }

        /// <summary>
        /// Gets the HeaderText to set in ToolTip
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="colText">An output parameter</param>
        /// <param name="rowText">An output parameter</param>
        private void GetRowColHeaderText(GridStyleInfo styleInfo, PivotCellInfo cellInfo, out string colText, out string rowText)
        {
            colText = string.Empty;
            rowText = string.Empty;
            List<string> temp2 = new List<string>();
            for (int j = 0; j < this.GridControl.PivotColumns.Count; j++)
            {
                Syncfusion.Windows.Controls.Cells.CoveredCellInfo coveredCellCol = this.CoveredCells.GetCellSpan(j, styleInfo.ColumnIndex);
                if (colText.Length > 0) colText += " - ";
                if (coveredCellCol != null)
                    colText += this.PivotEngine[coveredCellCol.Top, coveredCellCol.Left].FormattedText;
                else
                    colText += this.PivotEngine[j, styleInfo.ColumnIndex].FormattedText;
            }

            for (int i = 0; i < this.GridControl.PivotRows.Count; i++)
            {
                Syncfusion.Windows.Controls.Cells.CoveredCellInfo coveredCellRow = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, i);
                if (rowText.Length > 0) rowText += " - ";
                if (coveredCellRow != null)
                    rowText += this.PivotEngine[coveredCellRow.Top, coveredCellRow.Left].FormattedText;
                else
                    rowText += this.PivotEngine[styleInfo.RowIndex, i].FormattedText;
            }

            colText = RemoveRepeatedText(colText);
            rowText = RemoveRepeatedText(rowText);
        }

        /// <summary>
        /// Helper method to remove repeated text from ToolTip text
        /// </summary>
        /// <param name="text">A string argument</param>
        /// <returns>The string without repeated values.</returns>
        private static string RemoveRepeatedText(string text)
        {
            string[] temp = text.Split(new string[] { " - " }, StringSplitOptions.None);
            List<string> temp2 = new List<string>();
            foreach (string word in temp)
            {
                if (!temp2.Contains(word))
                    temp2.Add(word);
            }
            text = string.Empty;
            foreach (string word in temp2)
            {
                if (text.Length > 0) text += " - ";
                text += word;
            }
            return text;
        }

        internal DataTemplate tooltipTemplate = null;

#if SILVERLIGHT
        /// <summary>
        /// Gets the Template from the themes files
        /// </summary>
        /// <param name="vs">VisualStyle</param>
        /// <returns>The ToolTip DataTemplate for the provided VisualStyle.</returns>
        internal DataTemplate GetToolTipTemplate(Syncfusion.Windows.Controls.Theming.VisualStyle vs)
        {
            var rd = new ResourceDictionary();
            string source = string.Empty;
            switch (vs)
            {
                case Windows.Controls.Theming.VisualStyle.Office2007Black:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Office2007Blue:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Office2007Silver:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Metro:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Metro.xaml";
                    break;
                case Windows.Controls.Theming.VisualStyle.Blend:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml";
                    break;
                default:
                    source = "/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml";
                    break;
            }
            rd = new ResourceDictionary()
            {
                Source = new Uri(source, UriKind.RelativeOrAbsolute)
            };
            this.tooltipTemplate = rd["pivotTooltipTemplate"] as DataTemplate;
            return this.tooltipTemplate;
        }
#else
        /// <summary>
        /// Gets the Template from the themes files
        /// </summary>
        /// <param name="vs">A string argument</param>
        /// <returns>The ToolTip DataTemplate for the provided VisualStyle string.</returns>
        internal DataTemplate GetToolTipTemplate(string vs)
        {
            var rd = new ResourceDictionary();
            string source = string.Empty;
            switch (vs)
            {
                case "Office2007Black":
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Black.xaml";
                    break;
                case "Office2007Blue":
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Blue.xaml";
                    break;
                case "Office2007Silver":
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Office2007Silver.xaml";
                    break;
                case "Metro":
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Metro.xaml";
                    break;
                case "Blend":
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.Brushes.Blend.xaml";
                    break;
                default:
                    source = "/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml";
                    break;
            }
            rd = new ResourceDictionary()
            {
                Source = new Uri(source, UriKind.RelativeOrAbsolute)
            };
            this.tooltipTemplate = rd["pivotTooltipTemplate"] as DataTemplate;
            return this.tooltipTemplate;
        }
#endif
        /// <summary>
        /// Applying style for supplied GridStyleInfo
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellInfo">PivotCellInfo</param>
        /// <param name="colIndex">column index</param>
        private void ApplyStyle(GridStyleInfo styleInfo, PivotCellInfo cellInfo, int colIndex)
        {
            styleInfo.TextMargins.Left = 5;
            styleInfo.TextMargins.Right = 5;
            styleInfo.TextMargins.Top = 2;
            styleInfo.TextMargins.Bottom = 1;         

            if (colIndex == 0)
            {
#if SILVERLIGHT
                //styleInfo.Borders.Left = new Windows.Controls.Cells.CellBorder(new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0 }, Windows.Controls.Cells.BorderStyle.Standard);
                //styleInfo.Borders.Top = new Windows.Controls.Cells.CellBorder(new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 }, Windows.Controls.Cells.BorderStyle.Standard);
                //styleInfo.Borders.Right = new Windows.Controls.Cells.CellBorder(new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 }, Windows.Controls.Cells.BorderStyle.Standard);
                //styleInfo.Borders.Bottom = new Windows.Controls.Cells.CellBorder(new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 }, Windows.Controls.Cells.BorderStyle.Standard);
                styleInfo.Borders.Left = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0 };
                styleInfo.Borders.Top = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 };
                styleInfo.Borders.Right = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 };
                styleInfo.Borders.Bottom = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 };
#else
                styleInfo.Borders.Left = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0 };
                styleInfo.Borders.Top = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.1 };
                styleInfo.Borders.Right = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.1 };
                styleInfo.Borders.Bottom = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.1 };
#endif
            }
            else
#if SILVERLIGHT
             
                //styleInfo.Borders.All = new Windows.Controls.Cells.CellBorder(new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 }, Windows.Controls.Cells.BorderStyle.Standard);
                styleInfo.Borders.All = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.2 };
#else
                styleInfo.Borders.All = new Pen() { Brush = this.GridControl.GridLineStroke, Thickness = 0.1 };

#endif
            if (cellInfo != null)
            {
                if (cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()) && !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString())
                    && !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                {
                    styleInfo.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    if (this.GridControl.ValueCellStyle != null)
                        this.ApplyStyle(styleInfo, this.GridControl.ValueCellStyle);
                }

                else if ((cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()) ||
                    (cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))) &&
                    !cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString())
                    && this.GridControl.SummaryHeaderStyle != null)
                {

                    this.ApplyStyle(styleInfo, this.GridControl.SummaryHeaderStyle);
                }

                else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ValueCell) ||
                     cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell)||cellInfo.CellType==(PivotCellType.GrandTotalCell|PivotCellType.ValueCell|PivotCellType.TotalCell))
                {
                    styleInfo.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    if (this.GridControl.SummaryCellStyle != null)
                        this.ApplyStyle(styleInfo, this.GridControl.SummaryCellStyle);
                }

                else if ((cellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell) ||
                            cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell) ||
                            cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)) &&
                            this.GridControl.ColumnHeaderCellStyle != null)
                {
                    this.ApplyStyle(styleInfo, this.GridControl.ColumnHeaderCellStyle);
                }

                else if ((cellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell) ||
                            cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell) ||
                            cellInfo.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell)) &&
                            this.GridControl.RowHeaderCellStyle != null)
                {
                    this.ApplyStyle(styleInfo, this.GridControl.RowHeaderCellStyle);
                }
            }

        }

        /// <summary>
        /// Applying style for supplied GridStyleInfo
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
        /// <param name="cellStyle">PivotGridCellStyle</param>
        private void ApplyStyle(GridStyleInfo styleInfo, PivotGridCellStyle cellStyle)
        {
            styleInfo.Font.FontFamily = cellStyle.FontFamily;
            styleInfo.Font.FontSize = cellStyle.FontSize;
            styleInfo.Font.FontWeight = cellStyle.FontWeight;
            styleInfo.Background = cellStyle.Background;
            styleInfo.Foreground = cellStyle.Foreground;

#if !SILVERLIGHT
            styleInfo.FlowDirection = this.GridControl.FlowDirection; 
#endif
        }

        #endregion

        #region [ Expand / Collapse ]

        /// <summary>
        /// A method to expand all pivot item groups.
        /// </summary>
        public void ExpandAllGroup()
        {
            if (this.PivotEngine != null)
            {
                for (int row = 0; row < this.PivotEngine.RowCount; row++)
                {
                    for (int column = 0; column < this.PivotEngine.ColumnCount; column++)
                    {
                        PivotCellInfo cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null)
                        {
                            // TODO : CellTypes are in-correct implementing a work around
                            if (cellInfo.Tag != null)
                            //if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell) ||
                            //    cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                            {
                                //GridStyleInfo styleInfo = this.Model[row, column];
                                this.ExpandGroup(cellInfo, cellInfo.Tag as GridStyleInfo);
                            }
                        }
                    }
                }
                if (this._listOfCollapsedCells != null) this._listOfCollapsedCells.Clear();
                this.InvalidateCells();
            }
            else
            {
                throw new PivotGridException("Invalid operation, PivotEngine is null");
            }
        }

        /// <summary>
        /// A method to collapse all pivot item groups.
        /// </summary>
        public void CollapseAllGroup()
        {
            if (this.PivotEngine != null)
            {
                //for (int row = 0; row < this.PivotEngine.RowCount; row++)
                //{
                //    for (int column = 0; column < this.PivotEngine.ColumnCount; column++)
                //    {
                //        PivotCellInfo cellInfo = this.PivotEngine[row, column];
                //        if (cellInfo != null)
                //        {
                //            if ((cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) ||
                //                cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell)) && cellInfo.ParentCell==null)
                //            {
                //                GridStyleInfo styleInfo = this.Model[row, column];
                //                this.CollapseGroup(cellInfo, styleInfo);
                //            }
                //        }
                //    }
                //}
                this.CollapseGroupRow();
                this.CollapseGroupColumn();
                this.InvalidateCells();
            }
            else
            {
                throw new PivotGridException("Invalid operation, PivotEngine is null");
            }
        }

        /// <summary>
        /// Collapses all the rows in the PivotGrid starting from the inner group.
        /// </summary>
        internal void CollapseGroupRow()
        {
            for (int column = this.PivotEngine.PivotRows.Count - 1; column >= 0; column--)
            {
                for (int row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    CollapseGroup(column, row);
                }
            }
        }

        /// <summary>
        /// Collapse all the rows of a specified group(column) of the PivotGrid.
        /// </summary>
        /// <param name="columnGroupIndex">int</param>
        /// <param name="CollopseGroup">string</param>
        internal void CollapseGroupRow(int columnGroupIndex, string CollopseGroup)
        {
            for (int row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
            {
                CollapseGroup(columnGroupIndex, row, CollopseGroup);
            }
        }

        /// <summary>
        /// Collapses all the columns in the PivotGrid starting from the inner group.
        /// </summary>
        internal void CollapseGroupColumn()
        {
            for (int row = this.PivotEngine.PivotColumns.Count - 1; row >= 0; row--)
            {
                for (int column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    CollapseGroup(column, row);
                }
            }
        }

        internal void CollapseGroupColumn(int rowGroupIndex, string CollopseGroup)
        {
            for (int column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
            {
                CollapseGroup(column, rowGroupIndex,CollopseGroup);
            }
        }
        internal void CollapseGroup(int column, int row, string CollopseGroup )
        {
            PivotCellInfo cellInfo = this.PivotEngine[row, column];
            if (cellInfo != null && cellInfo.Key == CollopseGroup)
            {
                if ((cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) || cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell)) && cellInfo.ParentCell == null)
                {
                    GridStyleInfo styleInfo = this.Model[row, column];
                    this.CollapseGroup(cellInfo, styleInfo);
                }
            }
        }

        internal void CollapseGroup(int column, int row)
        {
            PivotCellInfo cellInfo = this.PivotEngine[row, column];
            if (cellInfo != null)
            {
                if ((cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) || cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell)) && cellInfo.ParentCell == null)
                {
                    GridStyleInfo styleInfo = this.Model[row, column];
                    this.CollapseGroup(cellInfo, styleInfo);
                }
            }
        }

        internal void ExpandGroup(int row, int column, string CellKey)
        {
            PivotCellInfo cellInfo = this.GridControl.PivotEngine[row, column];
            if (cellInfo != null && cellInfo.Key == CellKey)
            {
                if (cellInfo.Tag != null)
                {
                    this.ExpandGroup(cellInfo, cellInfo.Tag as GridStyleInfo);
                }
            }
        }

        internal void ExpandGroup(int row, int column)
        {
            PivotCellInfo cellInfo = this.GridControl.PivotEngine[row, column];
            if (cellInfo != null)
            {
                if (cellInfo.Tag != null)
                {
                    this.ExpandGroup(cellInfo, cellInfo.Tag as GridStyleInfo);
                }
            }
        }

        /// <summary>
        /// Expands the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        internal void ExpandRow(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText))
                        {
                            styleInfo = this.Model[row, column];
                            cellInfo.Tag = styleInfo;
                            this.GridControl.RaiseOnExpanding(new ExpandingEventArgs(cellInfo, uniqueText, styleInfo.CellRowColumnIndex), this.GridControl.PivotEngine[row, column]);
                        }
                    }
                }
                this.GridControl.InvalidateCells();
            }
        }

        /// <summary>
        /// Expand the rows for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        internal void ExpandRow(List<string> list)
        {
            if (list == null)
                return;
          
            foreach (string str in list)
            {
                this.ExpandRow(str);
            }
        }

        /// <summary>
        /// Collapses the row for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        internal void CollapseRow(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText))
                        {
                            styleInfo = this.Model[row, column];
                             this.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(cellInfo),styleInfo);
                        }
                    }
                }
                this.GridControl.InvalidateCells();
            }
        }

        /// <summary>
        /// Collapse the rows for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        internal void CollapseRow(List<string> list)
        {
            if (list == null)
                return;

            foreach (string str in list)
            {
                this.CollapseRow(str);
            }
        }

        /// <summary>
        /// Expands the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        internal void ExpandColumn(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
               for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                    {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText))
                        {
                            styleInfo = this.Model[row, column];
                            cellInfo.Tag = styleInfo;
                            this.GridControl.RaiseOnExpanding(new ExpandingEventArgs(cellInfo, uniqueText, styleInfo.CellRowColumnIndex), this.GridControl.PivotEngine[row, column]);
                        }
                    }
                }
                this.GridControl.InvalidateCells();
            }
        }

        /// <summary>
        /// Expand the Columns for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        internal void ExpandColumn(List<string> list)
        {
            if (list == null)
                return;

            foreach (string str in list)
            {
                this.ExpandColumn(str);
            }
        }

        /// <summary>
        /// Collapses the column for the specified unique text.
        /// </summary>
        /// <param name="uniqueText">Unique text of the Expander header.</param>
        internal void CollapseColumn(string uniqueText)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(uniqueText))
                        {
                            styleInfo = this.Model[row, column];
                            this.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(cellInfo), styleInfo);
                        }
                    }
                }
                this.GridControl.InvalidateCells();
            }
        }

        internal void HideCalculations(string fieldName)
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            GridStyleInfo styleInfo = null;
            if (this.PivotEngine != null)
            {
                for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount-2; column++)
                {
                    for (row = 0; row < this.PivotEngine.RowCount; row++)
                    {
                        cellInfo = this.PivotEngine[row, column];
                        if (cellInfo != null && cellInfo.UniqueText != null && cellInfo.UniqueText.Equals(fieldName))
                        {
                            styleInfo = this.Model[row, column];
                            this.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(cellInfo), styleInfo);
                        }
                    }
                }
                this.GridControl.InvalidateCells();
            }
        }

        /// <summary>
        /// Collapse the Columns for provided array of unique text.
        /// </summary>
        /// <param name="list">Array of Unique text of the Expander headers.</param>
        internal void CollapseColumn(List<string> list)
        {
            if (list == null)
                return;

            foreach (string str in list)
            {
                this.CollapseColumn(str);
            }
        }

        
        internal void SubTotalVisibilityRenderer(PivotItem item)
        {
            int row = 0, column = 0;
            PivotCellInfo cellInfo = null;
            if (this.PivotEngine.PivotRows.Any(x => x.FieldMappingName == item.FieldMappingName))
            {
                column = this.PivotEngine.PivotRows.IndexOf(this.PivotEngine.PivotRows.FirstOrDefault(x => x.FieldMappingName == item.FieldMappingName));
                for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                {
                    cellInfo = this.PivotEngine[row, column];
                    if (cellInfo != null)
                    {
                        if (cellInfo.FormattedText != null && cellInfo.FormattedText != "" && cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.RowHeaderCell | PivotCellType.TotalCell)))
                        {
                            var coveredcellsInfo = this.CoveredCells.GetCellSpan(row, column);
                            if (coveredcellsInfo != null)
                            {
                                if (this.GridControl.ShowSubTotals && item.ShowSubTotal == true && !PivotEngine.HiddenPivotRowGroups.Values.Any(x => x.Any(n => row >= n.From && row <= n.To)))
                                {
                                    this.Model.RowHeights.SetHidden(coveredcellsInfo.Top, coveredcellsInfo.Bottom, false);
                                    int count = this.PivotEngine.PivotValues[coveredcellsInfo.Top].Where(i => i != null && i.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) && i.FormattedText == null).ToList().Count;
                                    this.HiddenSubTotalsRowGroups.Remove(this.HiddenRowGroups.Where(i => i.From == coveredcellsInfo.Top && i.To == coveredcellsInfo.Bottom).FirstOrDefault());
                                }
                                else
                                {
                                    if (!this.HiddenRowGroups.Any(x => x.To == coveredcellsInfo.Bottom -1 ) ||  PivotEngine.HiddenPivotRowGroups.Values.Any(x => x.Any(n => n.To == coveredcellsInfo.Bottom -1)))
                                    {
                                        this.Model.RowHeights.SetHidden(coveredcellsInfo.Top, coveredcellsInfo.Bottom, true);
                                        if (!this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, item.TotalHeader)))
                                        {
                                            this.HiddenSubTotalsRowGroups.Add(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, item.TotalHeader));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            else if (this.PivotEngine.PivotColumns.Any(x => x.FieldMappingName == item.FieldMappingName))
            {
                row = PivotEngine.PivotColumns.IndexOf(this.PivotEngine.PivotColumns.FirstOrDefault(x => x.FieldMappingName == item.FieldMappingName));
                 for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                 {
                     cellInfo = this.PivotEngine[row, column];
                     if (cellInfo != null)
                     {
                         if (cellInfo.FormattedText != null && cellInfo.FormattedText != "" && cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell)))
                         {
                             var coveredcellsInfo = this.CoveredCells.GetCellSpan(row, column);
                             if (coveredcellsInfo != null)
                             {
                                 if (this.GridControl.ShowSubTotals && item.ShowSubTotal == true && !PivotEngine.HiddenPivotColumnGroups.Values.Any(x => x.Any(n => row >= n.From && row <= n.To)))
                                 {
                                     this.Model.ColumnWidths.SetHidden(coveredcellsInfo.Left, coveredcellsInfo.Right, false);
                                     this.HiddenSubTotalsColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredcellsInfo.Left && i.To == coveredcellsInfo.Right).FirstOrDefault());
                                 }
                                 else
                                 {
                                     if (!this.HiddenColumnGroups.Any(x => x.To == coveredcellsInfo.Bottom - 1) || PivotEngine.HiddenPivotColumnGroups.Values.Any(x => x.Any(n => n.To == coveredcellsInfo.Right - 1)))
                                     {
                                         this.Model.ColumnWidths.SetHidden(coveredcellsInfo.Left, coveredcellsInfo.Right, true);
                                         if (!this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, item.TotalHeader)))
                                         {
                                             this.HiddenSubTotalsColumnGroups.Add(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, item.TotalHeader));
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }
            }
        }
        /// <summary>
        ///Show or Hide the PivotGrid's SubTotal based on the ShowSubTotals Property
        /// </summary>
        public void SubTotalsRendering()
        {
            int row, column;
            PivotCellInfo cellInfo = null;
            //Shows the pivotGrid SubTotals if ShowSubTotals is true
            if (ShowSubTotals)
            {
                if (this.PivotEngine != null)
                {
                    for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                    {
                        for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                        {
                            cellInfo = this.PivotEngine[row, column];
                            if (cellInfo != null)
                            {
                                if (cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.RowHeaderCell | PivotCellType.TotalCell)))
                                {
                                    var coveredcellInfo = this.CoveredCells.GetCellSpan(row, column);
                                    if (coveredcellInfo != null)
                                    {
                                        this.Model.RowHeights.SetHidden(coveredcellInfo.Top, coveredcellInfo.Bottom, false);
                                        this.HiddenSubTotalsRowGroups.Remove(this.HiddenRowGroups.Where(i => i.From == coveredcellInfo.Top && i.To == coveredcellInfo.Bottom).FirstOrDefault());
                                    }
                                }
                            }
                        }
                    }

                    for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                    {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {                      
                            cellInfo = this.PivotEngine[row, column];
                            if (cellInfo != null)
                            {
                                if (cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell)))
                                {
                                    var coveredcellInfo = this.CoveredCells.GetCellSpan(row,column);
                                    if (coveredcellInfo != null)
                                    {                                        
                                        this.Model.ColumnWidths.SetHidden(coveredcellInfo.Left, coveredcellInfo.Right, false);
                                        this.HiddenSubTotalsColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredcellInfo.Left && i.To == coveredcellInfo.Right).FirstOrDefault());
                                    }
                                }
                            }
                        }
                    }

                    this.InvalidateCells();
                }
                else
                {
                    throw new PivotGridException("Invalid operation, PivotEngine is null");
                }
            }

            //Hides the PivotGrid SubTotals Field when ShowSubTotals is false
            else
            {
                if (this.PivotEngine != null)
                {
                    PivotItem pivotItem = null;
                    for (row = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.PivotEngine.RowCount; row++)
                    {
                        for (column = 0; column < this.PivotEngine.PivotRows.Count - 1; column++)
                        {
                            cellInfo = this.PivotEngine[row, column];
                            if (cellInfo != null)
                            {
                                if (cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.RowHeaderCell | PivotCellType.TotalCell)))
                                {
                                    var coveredcellsInfo = this.CoveredCells.GetCellSpan(row,column);
                                    pivotItem = this.PivotEngine.PivotRows[column];
                                    if (coveredcellsInfo != null)
                                    {
                                        this.Model.RowHeights.SetHidden(coveredcellsInfo.Top, coveredcellsInfo.Bottom, true);
                                        if (!this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, pivotItem.TotalHeader)))
                                        {
                                            this.HiddenSubTotalsRowGroups.Add(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom, coveredcellsInfo.Left, cellInfo.FormattedText, pivotItem.TotalHeader));
                                        }
                                    }
                                }
                            }
                        }

                    }
                    for (column = this.PivotEngine.PivotRows.Count; column < this.PivotEngine.ColumnCount; column++)
                    {
                    for (row = 0; row < this.PivotEngine.PivotColumns.Count - 1; row++)
                    {                        
                            cellInfo = this.PivotEngine[row, column];
                            if (cellInfo != null)
                            {
                                if (cellInfo.CellType == ((this.GridControl.GridLayout == GridLayout.TopSummary) ? (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell) : (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell)))
                                {
                                    var coveredcellsInfo = this.CoveredCells.GetCellSpan(row,column);
                                    pivotItem = this.PivotEngine.PivotColumns[row];
                                    if (coveredcellsInfo != null)
                                    {
                                        this.Model.ColumnWidths.SetHidden(coveredcellsInfo.Left, coveredcellsInfo.Right, true);
                                        if (!this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, pivotItem.TotalHeader)))
                                        {
                                            this.HiddenSubTotalsColumnGroups.Add(new HiddenGroup(coveredcellsInfo.Left, coveredcellsInfo.Right, coveredcellsInfo.Top, cellInfo.FormattedText, pivotItem.TotalHeader));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    this.InvalidateCells();
                }
                else
                {
                    throw new PivotGridException("Invalid operation, PivotEngine is null");
                }
            }
            EnsureHiddenGroupsShown();
        }

      
        /// <summary>
        ///Ensuring whether all the collapsed(hidden) groups are shown when ShowSubTotals is set as false
        /// </summary>
        internal void EnsureHiddenGroupsShown()
        {
            int ToField, Index = 0, cnt = -1;
            GridStyleInfo styleInfo1 = null;
            
            //For row groups
            foreach (var item in this.HiddenRowGroups)
            {
                cnt = 0;
                for (int i = this.PivotEngine.PivotColumns.Count + (!this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); i < this.PivotEngine.RowCount; i++)
                {
                    for (int j = 0; j < this.PivotEngine.PivotRows.Count - 1; j++)
                    {
                        if (Convert.ToString(this.Model[i, j].CellValue).Equals(item.GroupName))
                        {

                            if (cnt == 0)
                            {
                                //To check whether inner group of the particular row group is in collpased state
                                rowGroupChecking(item.From, item.To);
                                if (checkInnergroup)
                                {
                                    styleInfo1 = this.Model[i, j];
                                    var coveredcell = this.CoveredCells.GetCellSpan(styleInfo1.RowIndex, styleInfo1.ColumnIndex);
                                    this.Model.RowHeights.SetHidden(coveredcell.Top, coveredcell.Bottom, true);
                                    ToField = !this.PivotEngine.ShowCalculationsAsColumns && this.PivotEngine.PivotCalculations.Count > 1 ? this.PivotEngine.PivotCalculations.Count - 1 : 0;
                                    this.Model.RowHeights.SetHidden(item.To + 1, item.To + ToField + 1, false);
                                }
                            }

                            Index = j;
                            cnt = -1;
                        }
                    }
                }
                ToField = !this.PivotEngine.ShowCalculationsAsColumns && this.PivotEngine.PivotCalculations.Count > 1 ? this.PivotEngine.PivotCalculations.Count - 1 : 0;
                if (!checkInnergroup)
                {
                    this.Model.RowHeights.SetHidden(item.To + 1, item.To + ToField + 1, false);
                    GridStyleInfo styleInfo = this.Model[item.From, Index];
                    var summaryCell = this.Model[item.To + 1, Index];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = styleInfo;
                    this.InvalidateCell(GridRangeInfo.Cell(item.To + 1, Index));
                }
            }

            //For column groups 
            foreach (var item in this.HiddenColumnGroups)
            {
                cnt = 0;
                for (int i = 0; i < this.PivotEngine.PivotColumns.Count - 1; i++)
                    for (int j = this.PivotEngine.PivotRows.Count; j < this.PivotEngine.ColumnCount; j++)
                    {
                        if (Convert.ToString(this.Model[i, j].CellValue).Equals(item.GroupName))
                        {
                            if (cnt == 0)
                            {
                                //To check whether inner group of the particular column group is in collpased state
                                columnGroupChecking(item.From, item.To);
                                if (checkInnergroup)
                                {
                                    styleInfo1 = this.Model[i, j];
                                    var coveredcell = this.CoveredCells.GetCellSpan(styleInfo1.RowIndex, styleInfo1.ColumnIndex);
                                    this.Model.ColumnWidths.SetHidden(coveredcell.Left, coveredcell.Right, true);
                                    ToField = !this.PivotEngine.ShowCalculationsAsColumns && this.PivotEngine.PivotCalculations.Count > 1 ? this.PivotEngine.PivotCalculations.Count - 1 : 0;
                                    this.Model.ColumnWidths.SetHidden(item.To + 1, item.To + ToField + 1, false);
                                }
                            }
                            Index = i;
                            cnt = -1;
                        }

                    }
                ToField = this.PivotEngine.ShowCalculationsAsColumns && this.PivotEngine.PivotCalculations.Count > 1 ? this.PivotEngine.PivotCalculations.Count - 1 : 0;
                if (!checkInnergroup)
                {
                    this.Model.ColumnWidths.SetHidden(item.To + 1, item.To + ToField + 1, false);
                    GridStyleInfo styleInfo = this.Model[Index, item.From];
                    var summaryCell = this.Model[Index, item.To + 1];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = styleInfo;
                    this.InvalidateCell(GridRangeInfo.Cell(Index, item.To + 1));
                }
            }
        }       


        private bool checkInnergroup = false;

        /// <summary>
        ///To check whether inner group of the particular row group is in collapsed state(i.e., in HiddenRowGroups). If so, then that outer group remains in collapsed state.
        /// </summary>
        /// <param name="from">integer</param>
        /// <param name="to">integer</param>
        internal void rowGroupChecking(int from, int to)
        {
            foreach (var item in this.HiddenRowGroups)
            {
                if ( item.From >=from  &&  item.To <to)
                {
                    checkInnergroup = true;
                    break;
                }
            }
        }

        /// <summary>
        ///To check whether inner group of the particular column group is in collapsed state(i.e., in HiddenColumnGroups). If so, then that outer group remains in collapsed state.
        /// </summary>
        /// <param name="from">integer</param>
        /// <param name="to">integer</param>
        internal void columnGroupChecking(int from, int to)
        {
            foreach (var item in this.HiddenColumnGroups)
            {
                if (item.From >= from && item.To < to)
                {
                    checkInnergroup = true;
                    break;
                }
            }
        } 
        /// <param name="pivotCellInfo">PivotCellInfo</param>
        /// <param name="styleInfo">GridStyleInfo</param>
        internal void ExpandGroup(PivotCellInfo pivotCellInfo, GridStyleInfo styleInfo)
        {
            //if (pivotCellInfo.Tag != null)
            //    Debugger.Break();
            // TODO : CellTypes are in-correct implementing a workaroud
            PivotItem pivotItem = null;
            int toField;
           
            if (pivotCellInfo.CellType.ToString().Contains("RowHeaderCell"))
            //if (pivotCellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
            {
                CoveredCellInfo coveredCellInfo=null;
                if ((this.GridControl.GridLayout == GridLayout.TopSummary))
                    coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex + 1, styleInfo.ColumnIndex);
                else
                    coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                pivotItem = this.PivotEngine.PivotRows[styleInfo.ColumnIndex];                
                if (coveredCellInfo != null)
                {
                    this.Model.RowHeights.SetHidden(coveredCellInfo.Top, coveredCellInfo.Bottom, false);
                    this.HiddenRowGroups.Remove(this.HiddenRowGroups.Where(i => i.From == coveredCellInfo.Top && i.To == coveredCellInfo.Bottom).FirstOrDefault());
                    pivotCellInfo.Tag = null;
                    var summaryCell = this.Model[coveredCellInfo.Bottom + 1, styleInfo.ColumnIndex];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = null;
                }
                toField = (this.PivotEngine.ShowCalculationsAsColumns) ? coveredCellInfo.Bottom + 1 : ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellInfo.Bottom + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellInfo.Bottom + 1);
                string groupName = pivotItem.TotalHeader != null ? styleInfo.FormattedText + " " + pivotItem.TotalHeader : styleInfo.FormattedText;
                if (this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredCellInfo.Bottom + 1, toField, styleInfo.ColumnIndex, groupName , pivotItem.TotalHeader)) && (!ShowSubTotals || !pivotItem.ShowSubTotal))
                {
                    this.Model.RowHeights.SetHidden(coveredCellInfo.Bottom + 1, toField, true);
                }
                else if (!ShowSubTotals || !pivotItem.ShowSubTotal)
                {
                    this.HiddenSubTotalsRowGroups.Add(new HiddenGroup(coveredCellInfo.Bottom + 1, toField, styleInfo.ColumnIndex, groupName, pivotItem.TotalHeader));
                    this.Model.RowHeights.SetHidden(coveredCellInfo.Bottom + 1, toField, true);
                }
                EnsureInnerHiddenRowGroups(styleInfo);
            }
            else if (pivotCellInfo.CellType.ToString().Contains("ColumnHeaderCell"))
            //else if (pivotCellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
            {
                CoveredCellInfo coveredCellInfo = null;
                GridStyleInfo summaryCell = null;
                if ((this.GridControl.GridLayout == GridLayout.TopSummary))
                    coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex + this.GridControl.PivotEngine.PivotCalculations.Count);
                else
                    coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                pivotItem = this.PivotEngine.PivotColumns[styleInfo.RowIndex];
                if (coveredCellInfo != null)// && !(this.GridControl.GridLayout == GridLayout.TopSummary))
                {
                    this.Model.ColumnWidths.SetHidden(coveredCellInfo.Left, coveredCellInfo.Right, false);
                    this.HiddenColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredCellInfo.Left && i.To == coveredCellInfo.Right).FirstOrDefault());
                    pivotCellInfo.Tag = null;
                    summaryCell = this.Model[styleInfo.RowIndex, coveredCellInfo.Right + 1];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = null;
                }
                else
                {
                    this.Model.ColumnWidths.SetHidden(coveredCellInfo.Left, coveredCellInfo.Right, false);
                    this.HiddenColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredCellInfo.Left && i.To == coveredCellInfo.Right).FirstOrDefault());
                    pivotCellInfo.Tag = null;
                    summaryCell = this.Model[styleInfo.RowIndex, coveredCellInfo.Left];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = null;
                }
                if (!(this.GridControl.GridLayout == GridLayout.TopSummary))
                    toField = (this.PivotEngine.ShowCalculationsAsColumns) ? ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellInfo.Right + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellInfo.Right + 1) : coveredCellInfo.Right + 1;
                else
                    toField = (this.PivotEngine.ShowCalculationsAsColumns) ? ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellInfo.Left - 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellInfo.Left - 1) : coveredCellInfo.Left - 1;
                string groupName = pivotItem.TotalHeader != null ? styleInfo.FormattedText + " " + pivotItem.TotalHeader : styleInfo.FormattedText;
                if (this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredCellInfo.Right + 1, toField, styleInfo.RowIndex, groupName, pivotItem.TotalHeader)) && (!ShowSubTotals || !pivotItem.ShowSubTotal))
                {
                    this.Model.ColumnWidths.SetHidden(coveredCellInfo.Right + 1, toField, true);
                }
                else if (!ShowSubTotals || !pivotItem.ShowSubTotal)
                {
                    this.HiddenSubTotalsColumnGroups.Add(new HiddenGroup(coveredCellInfo.Right + 1, toField, styleInfo.RowIndex, groupName, pivotItem.TotalHeader));
                    this.Model.ColumnWidths.SetHidden(coveredCellInfo.Right + 1, toField, true);
                }
               EnsureInnerHiddenColumnGroups(styleInfo);
            }
            //// Ensuring all hidden groups are collapsed.
            this.Model.Selections.Clear();

            if (this.StatePersistenceEnabled && this._listOfCollapsedCells != null && pivotCellInfo.UniqueText != null && pivotCellInfo.UniqueText.LastIndexOf(delimiter) >= 0)
            {
                string formattedText = pivotCellInfo.UniqueText.Remove(pivotCellInfo.UniqueText.LastIndexOf(delimiter));
                if (_listOfCollapsedCells.Contains(formattedText))
                {
                    _listOfCollapsedCells.Remove(formattedText);
                }
            }
            //if (this.GridControl.IsDynamicData)
            //{
            //    this.Refresh(true);
            //}
        }


        private bool checkInnergroupforExpand = false;

        /// <summary>
        /// Ensure whether the collapsed inner row group retains its state
        /// </summary>
        /// <param name="styleInfo">GridStyleInfo</param>
       internal void EnsureInnerHiddenRowGroups(GridStyleInfo styleInfo)
        {
            var coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
            if (this.HiddenSubTotalsRowGroups.Count != 0)
            {
                foreach (var item in this.HiddenSubTotalsRowGroups)
                {
                    if ((item.From >= coveredCellInfo.Top && item.To <= coveredCellInfo.Bottom))
                    {
                         InnerRowGroupChecking(item.From, item.To);
                        if (checkInnergroupforExpand)
                            this.Model.RowHeights.SetHidden(item.From, item.To, false);
                        else
                        {
                            PivotItem pivotItem = this.PivotEngine.PivotRows[item.Level];
                            if (!ShowSubTotals || !pivotItem.ShowSubTotal)
                                this.Model.RowHeights.SetHidden(item.From, item.To, true);
                            else
                                this.Model.RowHeights.SetHidden(item.From, item.To, false);
                        }
                        checkInnergroupforExpand = false;
                    }
                }
            }


            if (this.HiddenRowGroups.Count != 0)
            {
                foreach (var item in this.HiddenRowGroups)
                {
                    this.Model.RowHeights.SetHidden(item.From, item.To, true);
                }
            }
        }

       /// <summary>
       /// To Check whether inner group of the particular expanded row group retains its state
       /// </summary>
       /// <param name="from">integer</param>
       /// <param name="to">integer</param>
       internal void InnerRowGroupChecking(int from,int to)
       {
           foreach (var item in this.HiddenRowGroups)
           {
               if (from == item.To + 1)
               {
                   this.Model.RowHeights.SetHidden(item.From, item.To, true);
                   checkInnergroupforExpand = true;
               }
           }
       }

        /// <summary>
       /// Ensure whether the collapsed inner column group retains its state
        /// </summary>
         /// <param name="styleInfo">GridStyleInfo</param>
       internal void EnsureInnerHiddenColumnGroups(GridStyleInfo styleInfo)
       {
           var coveredCellInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
           if (this.HiddenSubTotalsColumnGroups.Count != 0)
           {
               foreach (var item in this.HiddenSubTotalsColumnGroups)
               {
                   if (item.From >= coveredCellInfo.Left && item.To <= coveredCellInfo.Right)
                   {
                       InnerColumnGroupChecking(item.From, item.To);
                       if (checkInnergroupforExpand)
                           this.Model.ColumnWidths.SetHidden(item.From, item.To, false);
                       else
                       {
                           PivotItem pivotItem = this.PivotEngine.PivotColumns[item.Level];
                           if (!ShowSubTotals || !pivotItem.ShowSubTotal)
                               this.Model.ColumnWidths.SetHidden(item.From, item.To, true);
                           else
                               this.Model.ColumnWidths.SetHidden(item.From, item.To, false);

                       }
                       checkInnergroupforExpand = false;
                   }
               }
           }

           if (this.HiddenColumnGroups.Count != 0)
           {
               foreach (var item in this.HiddenColumnGroups)
               {
                   this.Model.ColumnWidths.SetHidden(item.From, item.To, true);
               }
           }
       }

       /// <summary>
       /// To Check whether inner group of the particular expanded column group retains its state
       /// </summary>
       /// <param name="from">integer</param>
       /// <param name="to">integer</param>
       internal void InnerColumnGroupChecking(int from, int to)
       {
           foreach (var item in this.HiddenColumnGroups)
           {
               if (from == item.To + 1)
               {
                   this.Model.ColumnWidths.SetHidden(item.From, item.To, true);
                   checkInnergroupforExpand = true;
               }
           }
       }

        internal List<string> _listOfCollapsedCells;
        string delimiter = new string((char)131, 1);
        /// <summary>
        /// Collapse the group of supplied item
        /// </summary>
        /// <param name="pivotCellInfo">PivotCellInfo</param>
        /// <param name="styleInfo">GridStyleInfo</param>
        internal void CollapseGroup(PivotCellInfo pivotCellInfo, GridStyleInfo styleInfo)
        {
            PivotItem pivotItem = null;
            int toField;
            if (pivotCellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.ExpanderCell))
            {
                CoveredCellInfo coveredCellsInfo = null;
                GridStyleInfo summaryCell = null;
                if (this.GridControl.GridLayout == GridLayout.TopSummary)
                    coveredCellsInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex + 1, styleInfo.ColumnIndex);
                else
                    coveredCellsInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                pivotItem = this.PivotEngine.PivotRows[styleInfo.ColumnIndex];
                if (coveredCellsInfo != null)
                {
                    this.Model.RowHeights.SetHidden(coveredCellsInfo.Top, coveredCellsInfo.Bottom, true);

                    if (!this.HiddenRowGroups.Has(new HiddenGroup(coveredCellsInfo.Top, coveredCellsInfo.Bottom, coveredCellsInfo.Left, pivotCellInfo.FormattedText, pivotItem.TotalHeader)))
                    {
                        this.HiddenRowGroups.Add(new HiddenGroup(coveredCellsInfo.Top, coveredCellsInfo.Bottom, coveredCellsInfo.Left, pivotCellInfo.FormattedText, pivotItem.TotalHeader));
                    }

                    //// Getting summary cell and masking it as expander parent
                    if (this.GridControl.GridLayout == GridLayout.TopSummary)
                        toField = (this.PivotEngine.ShowCalculationsAsColumns) ? coveredCellsInfo.Top - 1 : ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellsInfo.Bottom + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellsInfo.Bottom + 1);
                    else
                        toField = (this.PivotEngine.ShowCalculationsAsColumns) ? coveredCellsInfo.Bottom + 1 : ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellsInfo.Bottom + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellsInfo.Bottom + 1);
                    string groupName = pivotItem.TotalHeader != null ? styleInfo.FormattedText + " " + pivotItem.TotalHeader : styleInfo.FormattedText;
              
                    if (this.HiddenSubTotalsRowGroups.Has(new HiddenGroup(coveredCellsInfo.Bottom + 1, toField, styleInfo.ColumnIndex,groupName , pivotItem.TotalHeader)))
                    {
                        this.Model.RowHeights.SetHidden(coveredCellsInfo.Bottom + 1, toField, false);
                        //this.HiddenSubTotalsRowGroups.Remove(this.HiddenRowGroups.Where(i => i.From == coveredCellsInfo.Bottom + 1 && i.To == toField).FirstOrDefault());
                        this.HiddenSubTotalsRowGroups.Remove(this.HiddenSubTotalsRowGroups.Where(i => i.From == coveredCellsInfo.Bottom + 1 && i.To == toField).FirstOrDefault());
                    }
                    if (this.GridControl.GridLayout == GridLayout.TopSummary)
                        summaryCell = this.Model[coveredCellsInfo.Top - 1, styleInfo.ColumnIndex];
                    else
                        summaryCell = this.Model[coveredCellsInfo.Bottom + 1, styleInfo.ColumnIndex];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = styleInfo;
                }
            }
            else if (pivotCellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.ExpanderCell))
            {
                CoveredCellInfo coveredCellsInfo = null;
                GridStyleInfo summaryCell = null;
                if ((this.GridControl.GridLayout == GridLayout.TopSummary))
                    coveredCellsInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex + this.GridControl.PivotEngine.PivotCalculations.Count);
                else
                    coveredCellsInfo = this.CoveredCells.GetCellSpan(styleInfo.RowIndex, styleInfo.ColumnIndex);
                pivotItem = this.PivotEngine.PivotColumns[styleInfo.RowIndex];
                if (coveredCellsInfo != null)
                {
                    this.Model.ColumnWidths.SetHidden(coveredCellsInfo.Left, coveredCellsInfo.Right, true);

                    if (!this.HiddenColumnGroups.Has(new HiddenGroup(coveredCellsInfo.Left, coveredCellsInfo.Right, coveredCellsInfo.Top, pivotCellInfo.FormattedText, pivotItem.TotalHeader)))
                    {
                        this.HiddenColumnGroups.Add(new HiddenGroup(coveredCellsInfo.Left, coveredCellsInfo.Right, coveredCellsInfo.Top, pivotCellInfo.FormattedText, pivotItem.TotalHeader));
                    }

                    //// Getting summary cell and masking it as expander parent
                    if ((this.GridControl.GridLayout == GridLayout.TopSummary))
                    {
                        toField = (this.PivotEngine.ShowCalculationsAsColumns) ? ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellsInfo.Right + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellsInfo.Right + 1) : coveredCellsInfo.Left - 1;
                        string groupName = pivotItem.TotalHeader != null ? styleInfo.FormattedText + " " + pivotItem.TotalHeader : styleInfo.FormattedText;
                        if (this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredCellsInfo.Left - 1, toField, styleInfo.RowIndex, groupName, pivotItem.TotalHeader)))
                        {
                            this.Model.ColumnWidths.SetHidden(coveredCellsInfo.Left - 1, toField, false);
                            // this.HiddenSubTotalsColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredCellsInfo.Right + 1 && i.To == toField).FirstOrDefault());
                            this.HiddenSubTotalsColumnGroups.Remove(this.HiddenSubTotalsColumnGroups.Where(i => i.From == coveredCellsInfo.Right + 1 && i.To == toField).FirstOrDefault());
                        }
                    }
                    else
                    {
                        toField = (this.PivotEngine.ShowCalculationsAsColumns) ? ((this.GridControl.PivotEngine.PivotCalculations.Count > 1) ? coveredCellsInfo.Right + 1 + this.GridControl.PivotEngine.PivotCalculations.Count - 1 : coveredCellsInfo.Right + 1) : coveredCellsInfo.Right + 1;
                        string groupName = pivotItem.TotalHeader != null ? styleInfo.FormattedText + " " + pivotItem.TotalHeader : styleInfo.FormattedText;
                        if (this.HiddenSubTotalsColumnGroups.Has(new HiddenGroup(coveredCellsInfo.Right + 1, toField, styleInfo.RowIndex, groupName, pivotItem.TotalHeader)))
                        {
                            this.Model.ColumnWidths.SetHidden(coveredCellsInfo.Right + 1, toField, false);
                            // this.HiddenSubTotalsColumnGroups.Remove(this.HiddenColumnGroups.Where(i => i.From == coveredCellsInfo.Right + 1 && i.To == toField).FirstOrDefault());
                            this.HiddenSubTotalsColumnGroups.Remove(this.HiddenSubTotalsColumnGroups.Where(i => i.From == coveredCellsInfo.Right + 1 && i.To == toField).FirstOrDefault());
                        }
                    }
                    if ((this.GridControl.GridLayout == GridLayout.TopSummary))
                        summaryCell = this.Model[styleInfo.RowIndex, coveredCellsInfo.Left - this.PivotEngine.PivotCalculations.Count];
                    else
                        summaryCell = this.Model[styleInfo.RowIndex, coveredCellsInfo.Right + 1];
                    var summaryIdentity = (PivotGridStyleInfoIdentity)summaryCell.CellIdentity;
                    summaryIdentity.PivotCellInfo.Tag = styleInfo;
                }
            }

            if (this.StatePersistenceEnabled)
            {
                _listOfCollapsedCells = _listOfCollapsedCells ?? new List<string>();
                if (!_listOfCollapsedCells.Contains(pivotCellInfo.UniqueText))
                {
                    _listOfCollapsedCells.Add(pivotCellInfo.UniqueText);
                }
            }

            this.Model.Selections.Clear();          
        }
       
        #endregion

        /// <summary>
        /// This method calls PivotEngine.Populate to construct the undlerying pivot. It does it 
        /// in a manner such that if both PivotEngine.EnableOnDemandCalculations and 
        /// PivotEngine.EnableLazyLoadOnDemandCalculations are true, then the summaries are
        /// calculated in the background during Application Idle cycles.
        /// </summary>
        public void Populate()
        {
            Populate(this.PivotEngine, true);
        }

        internal void Populate(PivotEngine pe)
        {
            Populate(pe, true);
        }

        internal void Populate(PivotEngine pe, bool checkLazyLoading)
        {
            pe.Populate();
            if (checkLazyLoading && pe.EnableOnDemandCalculations && pe.EnableLazyLoadOnDemandCalculations)
            {
                this.GridControl.DoLazyCalculations();
            }
        }

        #region [ Methods ]

#if !SILVERLIGHT
        #region code to handle filtering calculation values

        /// <summary>
        /// Contains the row indexes that are hidden through filtering value columns when RowPivotsOnly is true.
        /// </summary>
        public HashSet<PivotCellInfo> HiddenRowIndexes
        {
            get { return PivotEngine.HiddenRowIndexes; }
        }

        /// <summary>
        /// Uses the contents of HiddenRowIndexes to display pivot results filtered using calculation values. Any previous
        /// filters are first cleared. This method only affects the display when RowPivotsOnly is set to true.
        /// </summary>
        public void ApplyFilters()
        {
            ApplyFilters(true);
        }
        /// <summary>
        /// Uses the contents of HiddenRowIndexes to display pivot results filtered using calculation values. Any previous
        /// filters optionally cleared depending upon clearExisitingFilter passed i.
        /// This method only affects the display when RowPivotsOnly is set to true.
        /// </summary>
        /// <param name="clearExistingFilter"></param>
        public void ApplyFilters(bool clearExistingFilter)
        {
            if (!GridControl.RowPivotsOnly)
            {
                return;
            }

            if(clearExistingFilter)
                ClearFilters(false);

            int colIndexKeyLoc = this.PivotEngine.GetHiddenRowKeyValueColumnIndex();
            bool needToCheckRowPivot = this.PivotEngine.PivotRows.Count > 1;
            for (int i = 0; i < this.PivotEngine.RowCount - this.GridControl.PivotRows.Count; ++i)
            {
                if (HiddenRowIndexes.Contains(this.PivotEngine[i, colIndexKeyLoc]))
                {
                    this.Model.RowHeights[i] = 0;
                }
            }
            
           // HideAllEmptyGroups();
            PivotEngine.UpdateAllSummariesRespectingHiddenRowIndexes();
            InvalidateCells();  
        }

        private void HideAllEmptyGroups()
        {
            //this code cuases the loss of groups if they collapse
            bool hasVisibleItem = false;
            int colIndexKeyLoc = this.PivotEngine.GetHiddenRowKeyValueColumnIndex();
            for (int i = 1; i < this.PivotEngine.RowCount - this.GridControl.PivotRows.Count; ++i)
            {
                bool isInnerMost = this.PivotEngine[i, colIndexKeyLoc].CellType == PivotCellType.ValueCell;
                if (isInnerMost)
                {
                    hasVisibleItem |= this.Model.RowHeights[i] > 0;
                    continue;
                }
                else
                if (!hasVisibleItem)
                {
                    this.Model.RowHeights[i] = 0;
                }
                hasVisibleItem = false;
            }
        }

        
        /// <summary>
        /// This method clears the current filtered state and redraws the pivot display.
        /// </summary>
        public void ClearFilters()
        {
            ClearFilters(true);
        }

        /// <summary>
        /// This method clears the current filtered state and optionally, redraws the pivot display.
        /// </summary>
        /// <param name="refresh">True if the pivot display should be redrawn and false otherwise.</param>
        public void ClearFilters(bool refresh)
        {
            if (!GridControl.RowPivotsOnly)
                return;

            this.Model.RowHeights.SetRange(1, this.Model.RowCount - 1, this.Model.RowHeights.DefaultLineSize);
            if (refresh)
            {
                PivotEngine.HiddenRowIndexes.Clear();
                
                PivotEngine.UpdateAllSummariesRespectingHiddenRowIndexes();
                ColumnFilterPopup.FilterPopUpCollection.Clear(); 
                ColumnFilterPopup.Exclusions.Clear();
                GridControl.RaiseFilterActionCompleted(new FilterActionCompletedEventArgs() { Choices = null, FieldName = null });
                InvalidateCells();
            }
        }
        /// <summary>
        /// Used to reapply the filtering in PivotGrid after the sorting operation done at PivotGrid when EnableOnDemandCalculations is true
        /// </summary>
        public void ReapplyFiltersAfterSort()
        {
            if (!GridControl.RowPivotsOnly)
                return;

            this.Model.RowHeights.SetRange(1, this.Model.RowCount - 1, this.Model.RowHeights.DefaultLineSize);
            this.ApplyFilters();
        }

        #endregion
#endif
        internal void ReApplyFilters(bool shouldCalculateTotal)
        {
            PivotEngine.HiddenPivotColumnGroups.Clear();
            PivotEngine.HiddenPivotRowGroups.Clear();
            for (int i = 0; this.GridControl.PivotEngine.EnableOnDemandCalculations && i < this.GridControl.Filters.Count; i++)
            {
                if (this.GridControl.GroupingBar != null && !this.GridControl.GroupingBar.Filters.Any(x => x.Name == this.GridControl.Filters[i].Name) && this.GridControl.Filters[i].Expression == "")
                    this.GridControl.InternalGrid.ApplyGroupingBarFilters(this.GridControl.Filters[i].Tag as FilterItemsCollection, shouldCalculateTotal);
            }
        }
  
        internal void ApplyGroupingBarFilters(FilterItemsCollection filterList, bool shouldCaluculateTotal)
        {
            if (filterList != null && this.GridControl.PivotRows.Any(x => x.FieldMappingName == filterList.Name))
            {
                int startRow = (this.GridControl.PivotEngine.PivotColumns.Count != 0 ? this.GridControl.PivotEngine.PivotColumns.Count : 0) + (this.GridControl.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
                int rowCount = this.GridControl.PivotEngine.ShowGrandTotals ? this.GridControl.PivotEngine.RowCount - 1 : this.GridControl.PivotEngine.RowCount;
                int level = this.GridControl.PivotRows.IndexOf(this.GridControl.PivotRows.FirstOrDefault(x => x.FieldMappingName == filterList.Name));
                PivotItem pivotItem = this.GridControl.PivotRows[level];
                for (int i = startRow; i < rowCount; i++)
                {
                    PivotCellInfo pci = this.GridControl.PivotEngine[i, level];
                    var coveredcellsInfo = this.GridControl.InternalGrid.CoveredCells.GetCellSpan(i, level);
                    if (pci != null && pci.FormattedText != null && filterList.Any(x => x.Key == pci.FormattedText && x.IsSelected == false))
                    {
                        if (pci.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell))
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom + (this.PivotEngine.ShowCalculationsAsColumns && pivotItem.ShowSubTotal ? 1 : (this.PivotEngine.ShowCalculationsAsColumns == false ? PivotEngine.PivotCalculations.Count : 0)), coveredcellsInfo.Left, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Add(level, hiddenGroup);
                            }
                            else
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Add(new HiddenGroup(coveredcellsInfo.Top, coveredcellsInfo.Bottom + (this.PivotEngine.ShowCalculationsAsColumns && pivotItem.ShowSubTotal ? 1 : (this.PivotEngine.ShowCalculationsAsColumns == false ? PivotEngine.PivotCalculations.Count : 0)), coveredcellsInfo.Left, pci.FormattedText, pivotItem.TotalHeader));
                        }
                        else if (this.GridControl.GridLayout == GridLayout.TopSummary && pci.CellType == (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell) && pci.CellRange != null)
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(coveredcellsInfo.Top - (this.PivotEngine.ShowCalculationsAsColumns && pivotItem.ShowSubTotal ? 1 : (this.PivotEngine.ShowCalculationsAsColumns == false ? PivotEngine.PivotCalculations.Count : 0)), coveredcellsInfo.Bottom, coveredcellsInfo.Left, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Add(level, hiddenGroup);
                            }
                            else
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Add(new HiddenGroup(coveredcellsInfo.Top - +(this.PivotEngine.ShowCalculationsAsColumns && pivotItem.ShowSubTotal ? 1 : (this.PivotEngine.ShowCalculationsAsColumns == false ? PivotEngine.PivotCalculations.Count : 0)), coveredcellsInfo.Bottom, coveredcellsInfo.Left, pci.FormattedText, pivotItem.TotalHeader));

                        }
                        else
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Add(level, hiddenGroup);
                            }
                            else if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Has(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader)))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Add(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader));
                        }
                    }
                    else if (pci != null && filterList.Any(x => x.Key == pci.FormattedText && x.IsSelected == true))
                    {
                        if (pci.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell))
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Where(n => n.From == coveredcellsInfo.Top && n.To == coveredcellsInfo.Bottom + (pivotItem.ShowSubTotal ? 1 : 0)).FirstOrDefault());
                            this.GridControl.InternalGrid.Model.RowHeights.SetHidden(pci.CellRange.Top, pci.CellRange.Bottom + (pivotItem.ShowSubTotal ? 1 : 0), false);
                        }
                        else if (this.GridControl.GridLayout == GridLayout.TopSummary && pci.CellType == (PivotCellType.RowHeaderCell | PivotCellType.HeaderCell) && pci.CellRange != null)
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                            {
                                if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                                    this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Where(n => n.From == coveredcellsInfo.Top - (pivotItem.ShowSubTotal ? 1 : 0) && n.To == coveredcellsInfo.Bottom).FirstOrDefault());
                                this.GridControl.InternalGrid.Model.RowHeights.SetHidden(pci.CellRange.Top - (pivotItem.ShowSubTotal ? 1 : 0), pci.CellRange.Bottom, false);
                            }
                        }
                        else
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups.Any(x => x.Key == level))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotRowGroups[level].Where(n => n.From == i && n.To == i).FirstOrDefault());
                            this.GridControl.InternalGrid.Model.RowHeights.SetHidden(i, i, false);
                        }
                    }
                }
                this.GridControl.InternalGrid.ApplyPivotRowFilters();
                if (GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || !GridControl.ShowSubTotals)
                {
                    foreach (PivotItem item in this.GridControl.PivotRows)
                    {
                        if (!item.ShowSubTotal)
                            this.SubTotalVisibilityRenderer(item);
                    }
                }
                if (shouldCaluculateTotal)
                {
                    if (PivotEngine.EnableOnDemandCalculations)
                    {
                        //this call ensures summaries fully computed to avoid issue with summaries displaying empty after a sort
                        PivotEngine.EnsureCalculationsLoaded();
                    };
                    if (this.GridControl.GridLayout != GridLayout.TopSummary)
                        UpdateAllSummariesRowsRespectingHiddenRowIndexes();
                }
                if (HiddenRowGroups.Count > 0)
                    foreach (HiddenGroup g in HiddenRowGroups)
                        this.GridControl.InternalGrid.Model.RowHeights.SetHidden(g.From, g.To, true);
            }
            else if (filterList != null && this.GridControl.PivotColumns.Any(x => x.FieldMappingName == filterList.Name))
            {
                int level = this.GridControl.PivotColumns.IndexOf(this.GridControl.PivotColumns.FirstOrDefault(x => x.FieldMappingName == filterList.Name));
                int startColumn = this.GridControl.PivotRows.Count;
                PivotItem pivotItem = this.GridControl.PivotColumns[level];

                for (int i = startColumn; i < this.GridControl.PivotEngine.ColumnCount; i++)
                {
                    PivotCellInfo pci = this.GridControl.PivotEngine[level, i];
                    if (pci != null && filterList.Any(x => x.Key == pci.FormattedText && x.IsSelected == false))
                    {
                        if (this.GridControl.GridLayout != GridLayout.TopSummary && pci.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell))
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(pci.CellRange.Left, pci.CellRange.Right + (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), level, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Add(level, hiddenGroup);
                                i = pci.CellRange.Right;
                            }
                            else
                            {
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Add(new HiddenGroup(pci.CellRange.Left, pci.CellRange.Right + (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), level, pci.FormattedText, pivotItem.TotalHeader));
                                i = pci.CellRange.Right;
                            }
                        }
                        else if (this.GridControl.GridLayout == GridLayout.TopSummary && pci.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell) && pci.CellRange != null && !((this.PivotEngine.PivotColumns.Count - 1) == level))
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level) && pci.CellRange != null)
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(pci.CellRange.Left - (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), pci.CellRange.Right, level, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Add(level, hiddenGroup);
                                i = pci.CellRange.Right;
                            }
                            else if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level) && pci.CellRange != null && ((this.PivotEngine.PivotColumns.Count - 1) == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Add(level, hiddenGroup);
                            }
                            else
                            {
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Add(new HiddenGroup(pci.CellRange.Left - (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), pci.CellRange.Right, level, pci.FormattedText, pivotItem.TotalHeader));
                                i = pci.CellRange.Right;
                            }
                        }
                        else
                        {
                            if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level))
                            {
                                List<HiddenGroup> hiddenGroup = new List<HiddenGroup>();
                                hiddenGroup.Add(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader));
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Add(level, hiddenGroup);
                            }
                            else if (!this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Has(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader)))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Add(new HiddenGroup(i, i, level, pci.FormattedText, pivotItem.TotalHeader));
                        }
                    }
                    else if (pci != null && filterList.Any(x => x.Key == pci.FormattedText && x.IsSelected == true))
                    {
                        if (pci.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell))
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Where(n => n.From == pci.CellRange.Left && n.To == pci.CellRange.Right + (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0)).FirstOrDefault());
                            this.GridControl.InternalGrid.Model.ColumnWidths.SetHidden(pci.CellRange.Left, pci.CellRange.Right + (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), false);
                        }
                        else if (this.GridControl.GridLayout == GridLayout.TopSummary && pci.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell) && pci.CellRange != null && !((this.PivotEngine.PivotColumns.Count - 1) == level))
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Where(n => n.From == pci.CellRange.Left - (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0) && n.To == pci.CellRange.Right).FirstOrDefault());
                            this.GridControl.InternalGrid.Model.ColumnWidths.SetHidden(pci.CellRange.Left - (pivotItem.ShowSubTotal ? PivotEngine.PivotCalculations.Count : 0), pci.CellRange.Right, false);
                        }
                        else
                        {
                            if (this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups.Any(x => x.Key == level))
                                this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Remove(this.GridControl.InternalGrid.PivotEngine.HiddenPivotColumnGroups[level].Where(n => n.From == i && n.To == i).FirstOrDefault());
                            this.GridControl.InternalGrid.Model.ColumnWidths.SetHidden(i, i, false);
                        }
                    }
                }
                this.GridControl.InternalGrid.ApplyPivotColumnFilters();
                if (this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false) || !this.GridControl.ShowSubTotals)
                {
                    foreach (PivotItem item in this.GridControl.PivotColumns)
                    {
                        if (!item.ShowSubTotal)
                            this.SubTotalVisibilityRenderer(item);
                    }
                }
                if (shouldCaluculateTotal)
                {
                    if (PivotEngine.EnableOnDemandCalculations)
                    {
                        //this call ensures summaries fully computed to avoid issue with summaries displaying empty after a sort
                        PivotEngine.EnsureCalculationsLoaded();
                    };
                    if (this.GridControl.GridLayout != GridLayout.TopSummary)
                        UpdateAllSummariesColumnsRespectingHiddenRowIndexes();
                }
                if (HiddenColumnGroups.Count > 0)
                    foreach (HiddenGroup g in HiddenColumnGroups)
                        this.GridControl.InternalGrid.Model.ColumnWidths.SetHidden(g.From, g.To, true);
            }
            InvalidateCells();
        }


        /// <summary>
        /// This method will recompute all summaries ignoring any summary row whose index is
        /// in HiddenRowIndexes.
        /// </summary>
        private void UpdateAllSummariesRowsRespectingHiddenRowIndexes()
        {
            int rowCount = PivotEngine.RowCount;
            int colCount = PivotEngine.ColumnCount;

            int valueColStart = PivotEngine.PivotRows.Count;
            int valueRowStart = PivotEngine.PivotColumns.Count + (PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);

            List<SummaryBase> values = new List<SummaryBase>();
            int kRow = 0;
            SummaryBase tempSB;

            int count2 = PivotEngine.PivotRows.Count < 2 ? PivotEngine.PivotRows.Count + 1 : PivotEngine.PivotRows.Count;
            if (rowCount > 0)
                for (int columnPos = valueColStart; columnPos < colCount - 1; ++columnPos)
                {
                    string format = PivotEngine.PivotCalculations[(columnPos - PivotEngine.PivotRows.Count) % PivotEngine.PivotCalculations.Count].Format;
                    values.Clear();
                    for (int i1 = 0; i1 < count2; ++i1)
                    {
                        values.Add(PivotEngine.PivotCalculations[(columnPos - PivotEngine.PivotRows.Count) % PivotEngine.PivotCalculations.Count].Summary.GetInstance());
                    }
                    for (int rowPos = valueRowStart; rowPos < rowCount - 1; ++rowPos)
                    {
                        if (PivotEngine.HiddenPivotRowGroups.Values.Any(x => x.Any(n => rowPos >= n.From && rowPos <= n.To)) || PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false) == null)
                            continue;
                        if (PivotEngine.IsSummaryRowWhileOnDemand(rowPos, ref kRow))
                        {
                            if (this.PivotEngine.ShowNullAsBlank && this.PivotEngine[rowPos, columnPos] != null && values[kRow].GetResult() == null)
                            {
                                tempSB = values[kRow].GetInstance();
                                tempSB.ShowNullAsBlank = this.PivotEngine.ShowNullAsBlank;
                                PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Summary = tempSB;
                                PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).FormattedText = null;
                            }
                            else if (this.PivotEngine[rowPos, columnPos] != null)
                            {
                                PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Summary = values[kRow];
                                if (values[kRow] != null && values[kRow].GetResult() != null)
                                {
                                    PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).FormattedText = string.Format("{0:" + format + "}", values[kRow].GetResult());
                                    PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Value = values[kRow].GetResult();
                                }
                                values[kRow] = values[kRow].GetInstance();
                            }
                        }
                        else if (PivotEngine[rowPos, columnPos] != null && PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Summary is SummaryBase)
                        {
                            foreach (SummaryBase sum in values)
                            {
                                sum.ShowNullAsBlank = this.PivotEngine.ShowNullAsBlank;
                                sum.CombineSummary(PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Summary);
                            }
                        }
                    }
                    if (PivotEngine[PivotEngine.PivotValues.GetLength(0) - 2, columnPos] != null)
                    {
                        PivotEngine.GetPivotEngineValueFor(PivotEngine.PivotValues.GetLength(0) - 2, columnPos, false).Summary = values[values.Count - 1];
                        if (values[values.Count - 1] != null && values[values.Count - 1].GetResult() != null)
                        {
                            PivotEngine.GetPivotEngineValueFor(PivotEngine.PivotValues.GetLength(0) - 2, columnPos, false).FormattedText = string.Format("{0:" + format + "}", values[values.Count - 1].GetResult());
                            PivotEngine.GetPivotEngineValueFor(PivotEngine.PivotValues.GetLength(0) - 2, columnPos, false).Value = values[values.Count - 1].GetResult();
                        }
                        else
                            PivotEngine.GetPivotEngineValueFor(PivotEngine.PivotValues.GetLength(0) - 2, columnPos, false).FormattedText = null;
                    }
                }
        }

        /// <summary>
        /// This method will recompute all summaries ignoring any summary column whose index is
        /// in HiddenColumnIndexes.
        /// </summary>
        private void UpdateAllSummariesColumnsRespectingHiddenRowIndexes()
        {
            int rowCount = PivotEngine.RowCount;
            int colCount = PivotEngine.ColumnCount;

            int valueColStart = PivotEngine.PivotRows.Count;
            int valueRowStart = PivotEngine.PivotColumns.Count + (PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);

            List<SummaryBase> values = new List<SummaryBase>();
            int kCol = 0;
            SummaryBase tempSB;

            int j2 = 0;
            int count2 = PivotEngine.PivotColumns.Count < 2 ? PivotEngine.PivotColumns.Count + 1 : PivotEngine.PivotColumns.Count;
            for (int rowPos = valueRowStart; rowPos < rowCount - 1; ++rowPos)
            {
                values.Clear();
                for (int i1 = 0; i1 < count2; ++i1)
                {
                    for (int i2 = 0; i2 < PivotEngine.PivotCalculations.Count; ++i2)
                        values.Add(PivotEngine.PivotCalculations[i2].Summary.GetInstance());
                    values[i1].ShowNullAsBlank = this.PivotEngine.ShowNullAsBlank;
                }

                for (int columnPos = valueColStart; columnPos <= colCount - 1; columnPos++)
                {
                    if (PivotEngine.HiddenPivotColumnGroups.Values.Any(x => x.Any(n => columnPos >= n.From && columnPos <= n.To)) || PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false) == null)
                        continue;
                    if (PivotEngine.IsSummaryColumnWhileOnDemand(columnPos, ref kCol))
                    {
                        for (int i2 = 0; i2 < PivotEngine.PivotCalculations.Count; ++i2)
                        {
                           if (this.PivotEngine.ShowNullAsBlank && this.PivotEngine[rowPos,columnPos]!=null && values[kCol * PivotEngine.PivotCalculations.Count + i2].GetResult() == null)
                            {
                                PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).FormattedText = null;
                                tempSB = values[kCol * PivotEngine.PivotCalculations.Count + i2].GetInstance();
                                tempSB.ShowNullAsBlank = this.PivotEngine.ShowNullAsBlank;
                                PivotEngine.GetPivotEngineValueFor(rowPos, columnPos++, false).Summary = tempSB;
                            }
                            else
                            {
                                string format = PivotEngine.PivotCalculations[(columnPos - PivotEngine.PivotRows.Count) % PivotEngine.PivotCalculations.Count].Format;
                                if (PivotEngine[rowPos, columnPos] != null && values[kCol * PivotEngine.PivotCalculations.Count + i2] != null && values[kCol * PivotEngine.PivotCalculations.Count + i2].GetResult() != null)
                                {
                                    PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).FormattedText = string.Format("{0:" + format + "}", values[kCol * PivotEngine.PivotCalculations.Count + i2].GetResult());
                                    PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Value = values[kCol * PivotEngine.PivotCalculations.Count + i2].GetResult();
                                }
                                if (columnPos + 1 < colCount && PivotEngine[rowPos, columnPos + 1] != null)
                                    PivotEngine.GetPivotEngineValueFor(rowPos, columnPos++, false).Summary = values[kCol * PivotEngine.PivotCalculations.Count + i2];
                                values[kCol * PivotEngine.PivotCalculations.Count + i2] = values[kCol * PivotEngine.PivotCalculations.Count + i2].GetInstance();
                            }
                        }
                        columnPos--;
                    }

                    else if (PivotEngine.GetPivotEngineValueFor(rowPos, columnPos, false).Summary is SummaryBase)
                    {
                        j2 = (columnPos - PivotEngine.PivotRows.Count) % PivotEngine.PivotCalculations.Count;
                        while (j2 < values.Count)
                        {
                            values[j2].CombineSummary(PivotEngine[rowPos, columnPos].Summary);
                            j2 += PivotEngine.PivotCalculations.Count;
                        }
                    }

                }
                for (int i2 = 0; i2 < PivotEngine.PivotCalculations.Count; ++i2)
                {
                    string format = PivotEngine.PivotCalculations[(((PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1) - PivotEngine.PivotRows.Count) % PivotEngine.PivotCalculations.Count].Format;

                    if (PivotEngine.GetPivotEngineValueFor(rowPos, (PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1, false) != null)
                        PivotEngine.GetPivotEngineValueFor(rowPos, (PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1, false).Summary = values[values.Count - PivotEngine.PivotCalculations.Count + i2];

                    if (values[values.Count - PivotEngine.PivotCalculations.Count + i2] != null && values[values.Count - PivotEngine.PivotCalculations.Count + i2].GetResult() != null)
                    {
                        PivotEngine.GetPivotEngineValueFor(rowPos, (PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1, false).FormattedText = string.Format("{0:" + format + "}", values[values.Count - PivotEngine.PivotCalculations.Count + i2].GetResult());
                        PivotEngine.GetPivotEngineValueFor(rowPos, (PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1, false).Value = values[values.Count - PivotEngine.PivotCalculations.Count + i2].GetResult();
                    }
                    else
                        PivotEngine.GetPivotEngineValueFor(rowPos, (PivotEngine.PivotValues.GetLength(1) - PivotEngine.PivotCalculations.Count + i2) - 1, false).FormattedText = null;
                }

            }
        }
        internal void ApplyPivotRowFilters()
        {
            foreach (KeyValuePair<int, List<HiddenGroup>> item in PivotEngine.HiddenPivotRowGroups.OrderByDescending(x => x.Key))
            {
                foreach (HiddenGroup hiddenGroup in item.Value)
                    if (hiddenGroup.From < this.GridControl.InternalGrid.Model.RowCount)
                        this.GridControl.InternalGrid.Model.RowHeights.SetHidden(hiddenGroup.From, hiddenGroup.To, true);
            }
        }

        internal void ApplyPivotColumnFilters()
        {
            foreach (KeyValuePair<int, List<HiddenGroup>> item in PivotEngine.HiddenPivotColumnGroups.OrderByDescending(x => x.Key))
            {
                foreach (HiddenGroup hiddenGroup in item.Value)
                    if (hiddenGroup.From < this.GridControl.InternalGrid.Model.ColumnCount)
                        this.GridControl.InternalGrid.Model.ColumnWidths.SetHidden(hiddenGroup.From, hiddenGroup.To, true);
            }
        }
        /// <summary>
        /// Gets the associated measure.
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        private string GetAssociatedMeasure(PivotCellInfo cellInfo, int rowIndex, int columnIndex)
        {
            if (this.GridControl.PivotCalculations.Count > 0)
            {
                if (this.GridControl.PivotCalculations.Count > 1)
                {
                    if (this.GridControl.ShowCalculationsAsColumns)
                    {
                        return this.GridControl.PivotEngine[this.Model.HeaderRows - 1, columnIndex].FormattedText;
                        //return string.Empty;                       
                    }
                    else
                    {
                        return this.GridControl.PivotEngine[rowIndex, this.Model.HeaderColumns - 1].FormattedText;
                    }
                }
                else
                    return this.GridControl.PivotCalculations[0].FieldName;
            }
            else
                return string.Empty;
        }

#if SILVERLIGHT

        internal void ConditionalRefresh()
        {
            DataRefreshingArgs args = new DataRefreshingArgs();
            this.GridControl.RaiseDataRefreshing(args);
            if (!args.Cancel)
            {
                this.InvalidateDisplay();
                this.ResetVisibleRowColCount();

                if (this.GridControl.IsDynamicData || this.GridControl.UpdateGridLayout)
                {
                    this.HiddenRowGroupStore.Clear();
                    this.HiddenColumnGroupStore.Clear();

                    foreach (var item in this.HiddenRowGroups)
                    {
                        HiddenRowGroupStore.Add(item.Clone(item));
                    }

                    foreach (var item in this.HiddenColumnGroups)
                    {
                        HiddenColumnGroupStore.Add(item.Clone(item));
                    }
                }

                this.HiddenColumnGroups.Clear();
                this.HiddenRowGroups.Clear();
                this.HiddenSubTotalsColumnGroups.Clear();
                this.HiddenSubTotalsRowGroups.Clear();

                if (this.PivotEngine.PivotCalculations.Count > 0)
                {
                    //if (shouldPopulateEngine)
                    //{
                        this.PivotEngine.CoveredRanges.Clear();
#if DEBUG               
                        Debug.WriteLine("Engine population start time is : " + DateTime.Now);
#endif
#if SILVERLIGHT
                        ////Synchronize the Pivot Engine's Allowed Fields using Pivot Grid's Allowed Fields before updating the Pivot Table.
                        if (this.GridControl.AllowedFields.Count > 0)
                        {
                            foreach (FieldInfo item in this.GridControl.AllowedFields)
                            {
                                this.PivotEngine.AddAllowedField(item, false);
                            }
                        }
#endif
                        if (loadInBackground)
                        {
                            PivotEngine pe = this.PivotEngine;
                            pe.LoadInBackground = true;
                            if (pe.LoadInBackground)
                            {
                                this.GridControl.BusyIndicator.IsBusy = true;
                            }
                            System.Windows.Threading.DispatcherTimer t =
                                          new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
                            t.Tick += (s, e) =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    InvalidateCells();
                                }));
                            };
                            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                            {

                                //pe.Populate();
                                Populate(pe);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    pe.LoadInBackground = false;
                                    this.GridControl.BusyIndicator.IsBusy = false;
                                    Refresh(false);
                                    this.PivotEngine.HiddenPivotColumnGroups.Clear();
                                    this.PivotEngine.HiddenPivotRowGroups.Clear();
                                    if (this.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                                    {
                                        foreach (PivotItem item in this.GridControl.PivotColumns)
                                        {
                                            this.SubTotalVisibilityRenderer(item);
                                        }
                                        foreach (PivotItem item in this.GridControl.PivotRows)
                                        {
                                            this.SubTotalVisibilityRenderer(item);
                                        }
                                    }
                                }
                            ));
                                return;
                            });
                            return;
                        }
                        else
                        {
                            //this.PivotEngine.Populate();
                           Populate();
                        }
                        
#if DEBUG
                        Debug.WriteLine("Engine population end time is : " + DateTime.Now);
#endif                    
                    //}

                    int headerColumns = 0, headerRows = 0;
                    headerColumns = this.PivotEngine.PivotRows.Count;
                    headerRows = this.PivotEngine.PivotColumns.Count;

                    if (this.PivotEngine.PivotCalculations.Count > 1 || this.PivotEngine.ShowSingleCalculationHeader)
                    {
                        if (!this.PivotEngine.ShowCalculationsAsColumns)
                            headerColumns++;
                        else
                            headerRows++;
                    }
                 ////freezing headers  
                    if (this.GridControl.FreezeHeaders)
                    {
                        if (headerColumns == 0)
                        {
                            this.Model.FrozenRows = 0;
                            this.Model.FrozenColumns = 1;
                        }
                        else
                        {
                            this.Model.FrozenRows = headerRows;
                            this.Model.FrozenColumns = headerColumns;
                            
                        }
                        this.Model.HeaderColumns = headerColumns;
                        this.Model.HeaderRows = headerRows;
                    }
                    else
                    {
                        this.Model.FrozenRows = 0;
                        this.Model.FrozenColumns = 0;
                    }

                    //// Specifying header rows, columns
                    this.Model.HeaderRows = Math.Max(0, headerRows);
                    this.Model.HeaderColumns = Math.Max(0, headerColumns);

                    //// Specifying grid total row, column count
                    if (this.PivotEngine.RowCount > 0)
                    {
                        this.Model.RowCount = this.PivotEngine.RowCount - 1;

                    }
                    if (this.PivotEngine.ColumnCount > 0)
                        this.Model.ColumnCount = this.PivotEngine.ColumnCount - 1;

                    // Defining the covered range
                    foreach (var range in this.PivotEngine.CoveredRanges)
                    {
                        this.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(range.Top, range.Left, range.Bottom, range.Right));
                    }

                    if (this.PivotEngine.PivotCalculations.Count > 0)
                    {
                        //if (this.GridControl.AutoSizeOption == GridAutoSizeOption.All)
                        //{
                        //    this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                        //}
                        //else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.FixedCount)
                        //{
                        //    if (this.GridControl.AutoSizeColumnCount < this.PivotEngine.ColumnCount && this.GridControl.AutoSizeColumnCount > 0)
                        //    {
                        //        this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Cols(0, this.GridControl.AutoSizeColumnCount), GridResizeToFitOptions.None);
                        //    }
                        //    else
                        //    {
                        //        this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                        //    }
                        //}

                        if (this.GridControl.AutoSizeOption != GridAutoSizeOption.None)
                            this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Cols(this.GridControl.PivotRows.Count - 1, this.Model.ColumnCount - 1), GridResizeToFitOptions.IncludeHiddenCells);
                    }

                    if (this.GridControl.IsDynamicData || this.GridControl.UpdateGridLayout)
                    {
                        if (this.HiddenRowGroupStore.Count > 0)
                        {
                            foreach (var item in this.HiddenRowGroupStore)
                            {
                                int i = 0;
                                for (; i < this.PivotEngine.RowCount; i++)
                                {
                                    if (this.PivotEngine[i, item.Level] != null)
                                    {
                                        if (this.PivotEngine[i, item.Level].FormattedText == item.GroupName)
                                            break;
                                    }
                                }

                                //// TODO: (If) condition here is a workaround for Groupbar Filtering, have to implement in a better way
                                if (i < this.PivotEngine.RowCount)
                                    this.CollapseGroup(this.PivotEngine[i, item.Level], this.Model[i, item.Level]);
                            }
                        }
                        if (this.HiddenColumnGroupStore.Count > 0)
                        {
                            foreach (var item in this.HiddenColumnGroupStore)
                            {
                                int i = 0;
                                for (; i < this.PivotEngine.ColumnCount; i++)
                                {
                                    if (this.PivotEngine[item.Level, i] != null)
                                    {
                                        if (this.PivotEngine[item.Level, i].FormattedText == item.GroupName)
                                            break;
                                    }
                                }

                                //// TODO: (If) condition here is a workaround for Groupbar Filtering, have to implement in a better way
                                if (i < this.PivotEngine.ColumnCount)
                                    this.CollapseGroup(this.PivotEngine[item.Level, i], this.Model[item.Level, i]);
                            }
                        }
                        this.InvalidateCells();
                    }
                }
                this.GridControl.RaiseDataRefreshed(new DataRefreshedArgs());
                //this.FreezeHeaders();
            }
        }

#endif
        /// <summary>
        /// Refreshes this PivotGrid instance.
        /// if the parameter value is true,then the refreshing inclues repopulation of the PivotEngine 
        /// </summary>
        /// <param name="shouldPopulateEngine">bool</param>
        public void Refresh(bool shouldPopulateEngine)
        {
#if DEBUG            
            Debug.WriteLine("Grid Refresh method start time is : " + DateTime.Now);
#endif            
            if (!this.GridControl.IgnoreRefesh && (this.PivotEngine.DataSourceList != null && this.PivotEngine.DataSourceList.ToList<object>().Count() >= 0))
            {
                DataRefreshingArgs args = new DataRefreshingArgs();
                this.GridControl.RaiseDataRefreshing(args);
                if (!args.Cancel)
                {
                    this.InvalidateDisplay();
                    this.ResetVisibleRowColCount();

                    if (this.GridControl.IsDynamicData || this.GridControl.UpdateGridLayout)
                    {
                        this.HiddenRowGroupStore.Clear();
                        this.HiddenColumnGroupStore.Clear();

                        foreach (var item in this.HiddenRowGroups)
                        {
                            HiddenRowGroupStore.Add(item.Clone(item));
                        }

                        foreach (var item in this.HiddenColumnGroups)
                        {
                            HiddenColumnGroupStore.Add(item.Clone(item));
                        }
                    }

                    this.HiddenColumnGroups.Clear();
                    this.HiddenRowGroups.Clear();
                    this.HiddenSubTotalsColumnGroups.Clear();
                    this.HiddenSubTotalsRowGroups.Clear();

                    if (shouldPopulateEngine)
                    {
                        this.PivotEngine.CoveredRanges.Clear();
#if !SILVERLIGHT

                        HiddenRowIndexes.Clear();
#endif
#if DEBUG
                        Debug.WriteLine("Engine population start time is : " + DateTime.Now);
#endif                        
#if SILVERLIGHT
                        ////Synchronize the Pivot Engine's Allowed Fields using Pivot Grid's Allowed Fields before updating the Pivot Table.
                        if (this.GridControl.AllowedFields.Count > 0)
                        {
                            foreach (FieldInfo item in this.GridControl.AllowedFields)
                            {
                                this.PivotEngine.AddAllowedField(item, false);
                            }
                        }
#endif
#if !SILVERLIGHT
                        if (loadInBackground)
                        {
                            LoadPivotInBackground();
                            return;
                        }
                        else if(!this.GridControl.LoadInBackground)
                        {
                            //this.PivotEngine.Populate();
                            if (!this.PivotEngine.LockComputations || this.PivotEngine.UseIndexedEngine)
                            {
                                this.PivotEngine.RefreshItemProperties();
                                Populate();
                                Refresh(false);
                                this.PivotEngine.HiddenPivotColumnGroups.Clear();
                                this.PivotEngine.HiddenPivotRowGroups.Clear();
                                if (this.GridControl.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                                {
                                    foreach (PivotItem item in this.GridControl.PivotColumns)
                                    {
                                        this.SubTotalVisibilityRenderer(item);
                                    }
                                    foreach (PivotItem item in this.GridControl.PivotRows)
                                    {
                                        this.SubTotalVisibilityRenderer(item);
                                    }
                                }
                                if (this.GridControl.Filters.Count > 0)
                                    this.ReApplyFilters(true);
                            }
                            return;
                        }
                       
#else

                        if (this.GridControl.LoadInBackground)
                        {
                            PivotEngine pe = this.PivotEngine;
                            pe.LoadInBackground = true;
                            if (pe.LoadInBackground)
                            {
                                this.GridControl.BusyIndicator.IsBusy = true;
                            }
                             System.Windows.Threading.DispatcherTimer t =
                                           new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
                             t.Tick += (s, e) =>
                             {
                                 this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        InvalidateCells();
                                    }));
                             };
                            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                            {

                                //pe.Populate();
                                Populate(pe);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    pe.LoadInBackground = false;
                                    this.GridControl.BusyIndicator.IsBusy = false;
                                    Refresh(false);
                                    this.PivotEngine.HiddenPivotColumnGroups.Clear();
                                    this.PivotEngine.HiddenPivotRowGroups.Clear();
                                    if (this.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                                    {
                                        foreach (PivotItem item in this.GridControl.PivotColumns)
                                        {
                                            this.SubTotalVisibilityRenderer(item);
                                        }
                                        foreach (PivotItem item in this.GridControl.PivotRows)
                                        {
                                            this.SubTotalVisibilityRenderer(item);
                                        }
                                    }
                                }
                            ));
                                return;
                            });
                            return;
                        }
                        else
                        {
                            //this.PivotEngine.Populate();
                         
                            Populate();
                            Refresh(false);
                            this.PivotEngine.HiddenPivotColumnGroups.Clear();
                            this.PivotEngine.HiddenPivotRowGroups.Clear();
                            if (this.GridControl.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                            {
                                foreach (PivotItem item in this.GridControl.PivotColumns)
                                {
                                    this.SubTotalVisibilityRenderer(item);
                                }
                                foreach (PivotItem item in this.GridControl.PivotRows)
                                {
                                    this.SubTotalVisibilityRenderer(item);
                                }
                            }
                            return;
                        }
#endif

                    }
#if DEBUG
                    Debug.WriteLine("Engine population end time is : " + DateTime.Now);
#endif                    
       
                    int headerColumns = 0, headerRows = 0;
                    headerColumns = this.PivotEngine.PivotRows.Count;
                    headerRows = this.PivotEngine.PivotColumns.Count;

                    if (this.PivotEngine.PivotCalculations.Count > 1 || this.PivotEngine.ShowSingleCalculationHeader)
                    {
                        if (!this.PivotEngine.ShowCalculationsAsColumns)
                            headerColumns++;
                        else
                            headerRows++;
                    }

                    if (headerRows == 0 && this.PivotEngine.PivotCalculations.Count == 1)
                    {
                        headerRows += 1;
                    }

                    //// freezing headers   
                    if (this.GridControl.FreezeHeaders)
                    {
                        this.Model.FrozenRows = headerRows;
                        this.Model.FrozenColumns = headerColumns;
                        this.Model.HeaderColumns = headerColumns;
                        this.Model.HeaderRows = headerRows;
                    }
                    else
                    {
                        this.Model.FrozenRows = 0;
                        this.Model.FrozenColumns = 0;
                    }


                    //// Specifying header rows, columns
                    this.Model.HeaderRows = Math.Max(0, headerRows);
                    this.Model.HeaderColumns = Math.Max(0, headerColumns);

                    //// Specifying grid total row, column count
                    int extra = 1;
                    if (this.PivotEngine.UseIndexedEngine)
                    {
                        extra = 0;
                        this.CoveredCells.Clear();
                    }
                    if (this.PivotEngine.RowCount > 0)
                    {
                        this.Model.RowCount = this.PivotEngine.RowCount - extra;
                        if (this.GridControl.GrandTotalRowAlwaysVisible && this.GridControl.PivotEngine.PivotRows.Count > 0)
                        {
                            this.Model.FooterRows = 1;
                        }
                        else
                            this.Model.FooterRows = 0;
                    }
                    if (this.PivotEngine.ColumnCount > 0)
                        this.Model.ColumnCount = this.PivotEngine.ColumnCount - extra;

                    // Defining the covered range
#if !SILVERLIGHT

                    if (this.GridControl.RowPivotsOnly && this.PivotEngine.CoveredRanges.Count > 0)
                    {
                        this.PivotEngine.CoveredRanges.RemoveAt(0);
                    }
#endif
                    foreach (var range in this.PivotEngine.CoveredRanges)
                    {
                        if (range.Top <= range.Bottom && range.Left <= range.Right)
                            this.CoveredCells.Add(new CoveredCellInfo(range.Top, range.Left, range.Bottom, range.Right));
                    }
#if !SILVERLIGHT

                    if (this.GridControl.RowPivotsOnly)
                    {
                        foreach (var item in this.GridControl.PossiblePivotCalculations)
                        {
                            PivotComputationInfo pci = GridControl.PivotCalculations.Where(c => c.FieldName == item.FieldName).FirstOrDefault();
                            if (pci != null)
                            {
                                this.SetValueColumnVisibility(item.FieldName, false);
                            }
                            else
                            {
                                GridControl.PivotCalculations.Add(item);
                                this.SetValueColumnVisibility(item.FieldName, true);
                            }
                        }
                    }
#endif
                    ResizeRowColumnsToFit();
                    if (this.GridControl.IsDynamicData || this.GridControl.UpdateGridLayout)
                    {
                        if (this.HiddenRowGroupStore.Count > 0)
                        {
                            foreach (var item in this.HiddenRowGroupStore)
                            {
                                int i = 0;
                                for (; i < this.PivotEngine.RowCount; i++)
                                {
                                    if (this.PivotEngine[i, item.Level] != null)
                                    {
                                        if (this.PivotEngine[i, item.Level].FormattedText == item.GroupName)
                                            break;
                                    }
                                }

                                //// TODO: (If) condition here is a workaround for Groupbar Filtering, have to implement in a better way
                                if (i < this.PivotEngine.RowCount)
                                    this.CollapseGroup(this.PivotEngine[i, item.Level], this.Model[i, item.Level]);
                            }
                        }
                        if (this.HiddenColumnGroupStore.Count > 0)
                        {
                            foreach (var item in this.HiddenColumnGroupStore)
                            {
                                int i = 0;
                                for (; i < this.PivotEngine.ColumnCount; i++)
                                {
                                    if (this.PivotEngine[item.Level, i] != null)
                                    {
                                        if (this.PivotEngine[item.Level, i].FormattedText == item.GroupName)
                                            break;
                                    }
                                }

                                //// TODO: (If) condition here is a workaround for Groupbar Filtering, have to implement in a better way
                                if (i < this.PivotEngine.ColumnCount)
                                    this.CollapseGroup(this.PivotEngine[item.Level, i], this.Model[item.Level, i]);
                            }
                        }
                        this.Model.Options.CurrentCellBorder = this.GridControl.CurrentCellBorder;                        
                        this.InvalidateCells();
                    }
                    this.GridControl.RaiseDataRefreshed(new DataRefreshedArgs());
#if !SILVERLIGHT

                    if (this.GridControl.RowPivotsOnly && !isSortingRowPivots && ColumnFilterPopup.FilterPopUpCollection != null && ColumnFilterPopup.FilterPopUpCollection.Count > 0)
                    {
                        ColumnFilterPopup.Exclusions.Clear();
                        ColumnFilterPopup.FilterPopUpCollection.Clear();
                    }
#endif
                }
            }
#if DEBUG            
            Debug.WriteLine("Grid Refresh method end time is : " + DateTime.Now);
#endif        
        }

        /// <summary>
        /// Builds a pivot using the current schema on a background thread so the UI thread is responsive.
        /// </summary>
        public void LoadPivotInBackground()
        {
            PivotEngine pe = this.PivotEngine;
            pe.NotPopulated = true;
            pe.LoadInBackground = true;
#if !SILVERLIGHT
            System.Windows.Threading.DispatcherTimer t =
                           new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000), IsEnabled = true };
            t.Tick += (s, e) => { this.Dispatcher.Invoke(new Action(() => { InvalidateCells(); })); };
#else
               System.Windows.Threading.DispatcherTimer t =
                                          new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000) };
                            t.Tick += (s, e) =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    InvalidateCells();
                                }));
                            };
#endif
#if !SILVERLIGHT
            this.PivotEngine.RefreshItemProperties(); 
#endif
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                Populate(pe, false);
                //pe.Populate();
#if !SILVERLIGHT
                this.Dispatcher.Invoke(new Action(() =>
                {
                    t.IsEnabled = false;
                    pe.LoadInBackground = false;
                    pe.NotPopulated = false;
                    this.GridControl.BusyIndicator.IsBusy = false;
                    if (!this.PivotEngine.LockComputations && !this.PivotEngine.UseIndexedEngine)
                    {
                        Refresh(false);
                        this.GridControl.RaiseLoadInBackgroundCompleted();
                        this.PivotEngine.HiddenPivotColumnGroups.Clear();
                        this.PivotEngine.HiddenPivotRowGroups.Clear();
                        if (this.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                        {
                            foreach (PivotItem item in this.GridControl.PivotColumns)
                            {
                                this.SubTotalVisibilityRenderer(item);
                            }
                            foreach (PivotItem item in this.GridControl.PivotRows)
                            {
                                this.SubTotalVisibilityRenderer(item);
                            }
                        }
                        if (this.GridControl.Filters.Count > 0)
                            this.ReApplyFilters(true);
                        if (pe.EnableOnDemandCalculations && pe.EnableLazyLoadOnDemandCalculations)
                        {
                            this.GridControl.DoLazyCalculations();
                        }
                    }
                }

            ));
#else
                this.Dispatcher.BeginInvoke(new Action(() =>
                               {
                                   pe.LoadInBackground = false;
                                   this.GridControl.BusyIndicator.IsBusy = false;
                                   Refresh(false);
                                   this.GridControl.RaiseLoadInBackgroundCompleted();
                                   this.PivotEngine.HiddenPivotColumnGroups.Clear();
                                   this.PivotEngine.HiddenPivotRowGroups.Clear();
                                   if (this.ShowSubTotals && this.GridControl.PivotRows.Any(x => x.ShowSubTotal == false) || this.GridControl.PivotColumns.Any(x => x.ShowSubTotal == false))
                                   {
                                       foreach (PivotItem item in this.GridControl.PivotColumns)
                                       {
                                           this.SubTotalVisibilityRenderer(item);
                                       }
                                       foreach (PivotItem item in this.GridControl.PivotRows)
                                       {
                                           this.SubTotalVisibilityRenderer(item);
                                       }
                                   }
                                   if (this.GridControl.Filters.Count > 0)
                                       this.ReApplyFilters(true);
                                   if (pe.EnableOnDemandCalculations && pe.EnableLazyLoadOnDemandCalculations)
                                   {
                                       this.GridControl.DoLazyCalculations();
                                   }


                               }
                ));
#endif

                return;
            });
        }
        /// <summary>
        /// Handles the ValueChanged of the horizontal scrollbar. 
        /// </summary>
        /// <param name="sender">The internal grid </param>
        /// <param name="e">An event argument</param>
        protected override void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("HSC Value Changed");
            base.OnHScrollBarValueChanged(sender, e);
            if (this.GridControl.AutoSizeOption == GridAutoSizeOption.VisibleRange)
            {
                 ResizeRowColumnsToFit();
            }
        }
        /// <summary>
        /// Handles the ValueChanged of the vertical scrollbar. 
        /// </summary>
        /// <param name="sender">The internal grid </param>
        /// <param name="e">An event argument</param>
        protected override void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("VSC Value Changed");
            base.OnVScrollBarValueChanged(sender, e);
            if (this.GridControl.AutoSizeOption == GridAutoSizeOption.VisibleRange)
            {
                ResizeRowColumnsToFit();
            }
        }

        private void ResizeRowColumnsToFit()
        {
            if (this.PivotEngine.PivotCalculations.Count > 0)
            {
                ////Fit the columns
                if (this.GridControl.AutoSizeOption == GridAutoSizeOption.TotalRows)
                {
                    this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Rows(Math.Max(0, this.PivotEngine.RowCount - 2), Math.Max(0, this.PivotEngine.RowCount - 2)), GridResizeToFitOptions.NoShrinkSize);
                }
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.All)
                {
                    this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                }
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.None && this.GridControl.InternalGrid != null)
                {
                    this.GridControl.InternalGrid.Model.ColumnWidths.DefaultLineSize = this.GridControl.DefaultComputationColumnSize;
                }
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.VisibleRange)
                {
                    //// Resize columns to fit for the visible range body.
                    int firstBodyVisibleLineIndexInRows = -1;
                    VisibleLinesCollection visibleRows = this.ScrollRows.GetVisibleLines();
                    var firstItem = visibleRows.FirstOrDefault(i => i.Region == ScrollAxisRegion.Body);
                    if (firstItem != null)
                        firstBodyVisibleLineIndexInRows = firstItem.LineIndex;

                    VisibleLinesCollection visibleCols = this.ScrollColumns.GetVisibleLines();
                    

                    if (firstBodyVisibleLineIndexInRows > -1 && this.ScrollColumns.LastBodyVisibleLineIndex > -1
                        && this.ScrollRows.LastBodyVisibleLineIndex > -1 && this.ScrollColumns.LastBodyVisibleLineIndex > -1)
                    {
                        int top = firstBodyVisibleLineIndexInRows;
                        int left = this.GridControl.GridScrollViewer.HorizontalOffset == 0.0 ? (visibleCols == null ? 0 : visibleCols.FirstBodyVisibleIndex) : this.ScrollColumns.LastBodyVisibleLineIndex;
                        int bottom = this.ScrollRows.LastBodyVisibleLineIndex;
                        int right = this.ScrollColumns.LastBodyVisibleLineIndex;
                        this.Model.ResizeColumnsToFit(GridRangeInfo.Cells(top, left, bottom, right), GridResizeToFitOptions.None);
                    }
                }
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.FixedCount)
                {
                    if (this.GridControl.AutoSizeColumnCount < this.PivotEngine.ColumnCount && this.GridControl.AutoSizeColumnCount > 0)
                    {
                        this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Cols(0, this.GridControl.AutoSizeColumnCount), GridResizeToFitOptions.None);
                    }
                    else
                    {
                        this.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                    }
                }

                if (this.GridControl.AutoSizeOption == GridAutoSizeOption.All)
                {
                    this.Model.ResizeRowsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                }
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.VisibleRange)
                {
                    int firstBodyVisibleLineIndexInColumns = -1;
                    VisibleLinesCollection visibleRows = this.ScrollColumns.GetVisibleLines();
                    var firstItem = visibleRows.FirstOrDefault(i => i.Region == ScrollAxisRegion.Body);
                    if (firstItem != null)
                        firstBodyVisibleLineIndexInColumns = firstItem.LineIndex;
                    VisibleLinesCollection visibleColumns = this.ScrollRows.GetVisibleLines();

                    if (this.ScrollRows.LastBodyVisibleLineIndex > -1 && firstBodyVisibleLineIndexInColumns > -1
                        && this.ScrollRows.LastBodyVisibleLineIndex > -1 && this.ScrollColumns.LastBodyVisibleLineIndex > -1)
                    {
                        int top = this.GridControl.GridScrollViewer.VerticalOffset == 0.0 ? (visibleColumns == null ? 0 : visibleColumns.FirstBodyVisibleIndex) : this.ScrollRows.LastBodyVisibleLineIndex;
                        int left = firstBodyVisibleLineIndexInColumns;
                        int bottom = this.ScrollRows.LastBodyVisibleLineIndex;
                        int right = this.ScrollColumns.LastBodyVisibleLineIndex;
                        this.Model.ResizeRowsToFit(GridRangeInfo.Cells(top, left, bottom, right), GridResizeToFitOptions.None);
                    }
                }
#if !SILVERLIGHT
                else if (this.GridControl.AutoSizeOption == GridAutoSizeOption.FixedCount)
                {

                    if (this.GridControl.AutoSizeRowCount < this.PivotEngine.RowCount && this.GridControl.AutoSizeRowCount > 0)
                    {
                        this.Model.ResizeRowsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Rows(0, this.GridControl.AutoSizeRowCount), GridResizeToFitOptions.None);
                    }
                    else
                    {
                        this.Model.ResizeRowsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Invalidates the collapsed cells when pivot schema getting changed along with <see cref="StatePersistenceEnabled"/> as true.
        /// </summary>
        private void InvalidateCollapsedCells()
        {
            if (_listOfCollapsedCells != null && this.StatePersistenceEnabled)
            {
                for (int i = 0; i < PivotEngine.RowCount; i++)
                {
                    for (int j = 0; j < PivotEngine.ColumnCount; j++)
                    {
                        PivotCellInfo cellInfo = this.PivotEngine[i, j];
                        if (cellInfo != null && cellInfo.UniqueText != null && _listOfCollapsedCells.Contains(cellInfo.UniqueText))
                        {
                            CollapseGroup(cellInfo, this.Model[i, j]);
                        }
                    }
                }
                this.InvalidateCells();
            }
        }

        private void FreezeHeaders()
        {
            int headerColumns = 0, headerRows = 0;
            headerColumns = this.PivotEngine.PivotRows.Count;
            headerRows = this.PivotEngine.PivotColumns.Count;
            if (this.PivotEngine.PivotCalculations.Count > 1 || this.PivotEngine.ShowSingleCalculationHeader)
            {
                if (!this.PivotEngine.ShowCalculationsAsColumns)
                    headerColumns++;
                else
                    headerRows++;
            }

            //// freezing headers
            if (this.GridControl.FreezeHeaders)
            {
                this.GridControl.InternalGrid.Model.FrozenRows = headerRows;
                this.GridControl.InternalGrid.Model.FrozenColumns = headerColumns;
            }
            else
            {
                this.GridControl.InternalGrid.Model.FrozenRows = 0;
                this.GridControl.InternalGrid.Model.FrozenColumns = 0;
            }
        }

        /// <summary>
        /// Checks whether template is applied for the grid cell style.
        /// </summary>
        /// <param name="IsRow">if set to <c>true</c> [is row].</param>
        /// <returns></returns>
        private bool CheckCellStyle(bool IsRow)
        {
            if (IsRow)
            {
                if (this.GridControl != null && this.GridControl.RowHeaderCellStyle == null)
                {
                    return false;
                }
                if (this.GridControl != null && this.GridControl.RowHeaderCellStyle.Style == null)
                {
                    return false;
                }
                return true;
            }
            else
            {
                if (this.GridControl != null && this.GridControl.ColumnHeaderCellStyle == null)
                {
                    return false;
                }
                if (this.GridControl != null && this.GridControl.ColumnHeaderCellStyle.Style == null)
                {
                    return false;
                }
                return true;
            }
        }

        internal void InvalidateDisplay()
        {
            this.RenderStyles.Clear();
            this.ArrangedCellUIElements.UnloadAll();
            //this.RenderedCellVisuals.Invalidate();
            this.CoveredCells.Clear();
            this.Model.CoveredCells.Clear();
            this.Model.VolatileCellStyles.Clear();
            this.Model.Data.Clear();
            this.CellSpanBackgrounds.Clear();
        }

        private void ResetVisibleRowColCount()
        {
            this.Model.RowCount = 1;
            this.Model.ColumnCount = 1;
            this.Model.FrozenRows = 0;
            this.Model.FrozenColumns = 0;

        }

        private void InitSelectsCellsMouseController()
        {
            //var selectController = this.MouseControllerDispatcher.Find("SelectCellsMouseController") as GridSelectCellsMouseController;
            //if (selectController != null)
            //{
            //    selectController.AdjustedRangeFunc = (type, r, c, selectedRange) =>
            //    {
            //        var style = this.Model[r, c] as GridStyleInfo;
            //        var identity = style.CellIdentity as PivotGridStyleInfoIdentity;

            //        switch (type)
            //        {
            //            case GridSelectCellsMouseController.SelectionType.IsCells:
            //                if (tableStyleIdentity.TableCellType != GridDataTableCellType.RecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.AddNewRecordCell && tableStyleIdentity.TableCellType != GridDataTableCellType.UnboundColumnCell && tableStyleIdentity.TableCellType != GridDataTableCellType.RowHeaderCell && tableStyleIdentity.TableCellType != GridDataTableCellType.UnboundRecordCell)
            //                {
            //                    selectedRange = GridRangeInfo.Empty;
            //                }

            //                break;
            //            case GridSelectCellsMouseController.SelectionType.IsRow:
            //                if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell || tableStyleIdentity.TableCellType == GridDataTableCellType.UnboundRecordCell)
            //                {
            //                    var adjustValue = this.Model.TableModel.Table.HasNestedTables ? 1 : 0;
            //                    var colOffset = this.TableModel.ResolveDefaultColumnOffset();
            //                    selectedRange = selectedRange.ExpandRange(0, colOffset, this.Model.RowCount, this.Model.ColumnCount - adjustValue);
            //                }
            //                else if (tableStyleIdentity.TableCellType != GridDataTableCellType.RowHeaderCell)
            //                {
            //                    selectedRange = GridRangeInfo.Empty;
            //                }

            //                break;
            //        }

            //        return selectedRange;
            //    };
            //}           
        }

        /// <summary>
        /// Fills the selected items with appropriate Column, Row and Value based on the Specified Range
        /// </summary>
        /// <param name="gridRangeInfo">The grid range info.</param>
        private void FillSelectedItems(GridRangeInfo gridRangeInfo)
        {
            bool MultipleCells = gridRangeInfo.ToString().Contains(':');

            if (!MultipleCells)
            {
                this.SelectedItems = new SelectedItems();

                this.SelectedItems.Add(new SelectedItem
                {
                    ColumnList = GetColumnList(gridRangeInfo.Left).ColumnList,
                    RowList = GetRowList(gridRangeInfo.Bottom).RowList,
                    FormattedValue = this.PivotEngine[gridRangeInfo.Bottom, gridRangeInfo.Left].ToString(),
                    Value = this.PivotEngine[gridRangeInfo.Bottom, gridRangeInfo.Left].Value == null ? null : this.PivotEngine[gridRangeInfo.Bottom, gridRangeInfo.Left].Value.ToString()
                });
            }

            else
            {
                this.SelectedItems = new SelectedItems();
                SelectedItems selectedRowLists = new SelectedItems();

                for (int col = gridRangeInfo.Left; col <= gridRangeInfo.Right; col++)
                {
                    if (!ColumnHiddenGroup(col))
                    {
                        SelectedItem selectedColList = GetColumnList(col);

                        for (int row = gridRangeInfo.Top; row <= gridRangeInfo.Bottom; row++)
                        {
                            if (!RowHiddenGroup(row))
                            {
                                if (col == gridRangeInfo.Left)
                                {
                                    selectedRowLists.Add(GetRowList(row));
                                }

                                this.SelectedItems.Add(new SelectedItem
                                {
                                    ColumnList = selectedColList.ColumnList,
                                    RowList = selectedRowLists[row - gridRangeInfo.Top].RowList,
                                    FormattedValue = this.PivotEngine[gridRangeInfo.Bottom, gridRangeInfo.Left].ToString(),
                                    Value = this.PivotEngine[row, col].Value == null ? null : this.PivotEngine[row, col].Value.ToString()
                                });
                            }
                            else
                            {
                                selectedRowLists.Add(new SelectedItem());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the given Row Index is in Column Row Group.
        /// </summary>
        /// <param name="row">The RowIndex.</param>
        /// <returns>True, if row is within hidden row group. False, otherwise.</returns>
        private bool RowHiddenGroup(int row)
        {
            bool IsHidden = false;
            this.HiddenRowGroups.ForEach<HiddenGroup>(hidden =>
            {
                if (row >= hidden.From && row <= hidden.To)
                {
                    IsHidden = true;
                }
            }
           );
            return IsHidden;
        }

        /// <summary>
        /// Checks whether the given Column Index is in Column Hidden Group.
        /// </summary>
        /// <param name="col">The ColumnIndex.</param>
        /// <returns></returns>
        private bool ColumnHiddenGroup(int col)
        {
            bool IsHidden = false;
            this.HiddenColumnGroups.ForEach<HiddenGroup>(hidden =>
            {
                if (col >= hidden.From && col <= hidden.To)
                {
                    IsHidden = true;
                }
            }
           );
            return IsHidden;
        }

        /// <summary>
        /// Gets the column list.
        /// </summary>
        /// <param name="col">position of item to select</param>
        /// <returns>list of columns in selected item</returns>
        private SelectedItem GetColumnList(int col)
        {
            SelectedItem m_SelectedItem = new SelectedItem();
            const char underScoreMarker = (char)129;

            for (int row = 0; row < this.PivotEngine.PivotColumns.Count + (this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row++)
            {
                if (this.PivotEngine[row, col] != null && this.PivotEngine[row, col].ParentCell == null)
                {
                    if (this.PivotEngine[row, col].Tag == null)
                    {
                        if (!string.IsNullOrEmpty(PivotEngine[row, col].UniqueText))
                        {
                            foreach (var item in PivotEngine[row, col].UniqueText.Split(underScoreMarker))
                            {
                                if (!m_SelectedItem.ColumnList.Contains(item))
                                {
                                    m_SelectedItem.ColumnList.Add(item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.PivotEngine[row, col] != null && ((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue != null)
                        {
                            if (!m_SelectedItem.ColumnList.Contains(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString()))
                            {
                                m_SelectedItem.ColumnList.Add(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString());
                            }
                        }
                        break;
                    }
                }
                else
                {
                    if (this.PivotEngine[row, col] !=null && this.PivotEngine[row, col].Tag == null)
                    {
                        if (this.PivotEngine[row, col].Value != null && this.PivotEngine[row,col].ParentCell.Value != null)
                        {
                            if (!m_SelectedItem.ColumnList.Contains(this.PivotEngine[row, col].ParentCell.Value.ToString()))
                            {
                                m_SelectedItem.ColumnList.Add(this.PivotEngine[row, col].ParentCell.Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        if (this.PivotEngine[row, col] !=null && ((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue != null)
                        {
                            if (!m_SelectedItem.ColumnList.Contains(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString()))
                            {
                                m_SelectedItem.ColumnList.Add(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString());
                            }
                        }
                        break;
                    }

                }
            }

            return m_SelectedItem;
        }

        /// <summary>
        /// Gets the row list.
        /// </summary>
        /// <param name="row">position of item to select</param>
        /// <returns>list of rows in selected item</returns>
        private SelectedItem GetRowList(int row)
        {
            SelectedItem m_SelectedItem = new SelectedItem();
            const char underScoreMarker = (char)129;

            for (int col = 0; col< this.PivotEngine.PivotColumns.Count + (this.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); col++)
            {
                if (this.PivotEngine[row, col] != null && this.PivotEngine[row, col].ParentCell == null)
                {
                    if (this.PivotEngine[row, col].Tag == null)
                    {
                        if (!string.IsNullOrEmpty(PivotEngine[row, col].UniqueText))
                        {
                            foreach (var item in PivotEngine[row, col].UniqueText.Split(underScoreMarker))
                            {
                                if (!m_SelectedItem.RowList.Contains(item))
                                {
                                    m_SelectedItem.RowList.Add(item);
                                }
                            }
                        }
                    }

            
                    else
                    {
                        if (this.PivotEngine[row, col] != null && ((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue != null)
                        {
                            if (!m_SelectedItem.RowList.Contains(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString()))
                            {
                                m_SelectedItem.RowList.Add(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString());
                            }
                        }
                        break;
                    }
                }
                else
                {
                    if (this.PivotEngine[row, col] != null && this.PivotEngine[row, col].Tag == null)
                    {
                        if (this.PivotEngine[row, col].Value != null && this.PivotEngine[row,col].ParentCell.Value != null)
                        {
                            if (!m_SelectedItem.RowList.Contains(this.PivotEngine[row, col].ParentCell.Value.ToString()))
                            {
                                m_SelectedItem.RowList.Add(this.PivotEngine[row, col].ParentCell.Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        if (this.PivotEngine[row, col] != null && ((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue != null)
                        {
                            if (!m_SelectedItem.RowList.Contains(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString()))
                            {
                                m_SelectedItem.RowList.Add(((GridStyleInfo)this.PivotEngine[row, col].Tag).CellValue.ToString());
                            }
                        }
                        break;
                    }
                }
            }

            return m_SelectedItem;
        }

        /// <summary>
        /// Raises the Selection changed event.
        /// </summary>
        /// <param name="selectionChangedEventArgs">The <see cref="Syncfusion.Windows.Controls.PivotGrid.PivotGridSelectionChangedEventArgs"/> instance containing the event data.</param>
        internal void RaiseSelectionChangedEvent(PivotGridSelectionChangedEventArgs selectionChangedEventArgs)
        {
            this.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
        }

        /// <summary>
        /// Gets the Hyperlink Value based on Cell Type
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns></returns>
        private bool GetValue(PivotCellInfo cellInfo)
        {
            if((cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()))&&
                !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
            {
                if((this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.IsHyperlinkCell))
                {
                    return true;
                }
            }
            else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
            {
                if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.IsHyperlinkCell)
                {
                    return true;
                }
#if !SILVERLIGHT

                else if (this.GridControl.RowPivotsOnly)
                {
                    if (cellInfo.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell) && cellInfo.CellRange.Right < this.GridControl.PivotRows.Count)
                    {
                        return this.GridControl.PivotRows[cellInfo.CellRange.Right].EnableHyperlinks;
                    }
                }
#endif
            }
            else if (cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                !cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()))
            {
                if (this.GridControl.SummaryHeaderStyle != null && this.GridControl.SummaryHeaderStyle.IsHyperlinkCell)
                {
                    return true;
                }
            }
            else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ValueCell) ||
                cellInfo.CellType == (PivotCellType.GrandTotalCell | PivotCellType.ValueCell)||
                cellInfo.CellType==(PivotCellType.GrandTotalCell|PivotCellType.TotalCell|PivotCellType.ValueCell))
            {
                if (this.GridControl.SummaryCellStyle != null && this.GridControl.SummaryCellStyle.IsHyperlinkCell)
                {
                    return true;
                }
            }
            else if (cellInfo.CellType == PivotCellType.ValueCell)
            {
                if (this.GridControl.ValueCellStyle != null && this.GridControl.ValueCellStyle.IsHyperlinkCell)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the EnableContextMenu Value based on Cell Type and style
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <returns></returns>
        private bool GetEnableContextMenuValue(PivotCellInfo cellInfo)
        {
            if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
            {
                if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.EnableContextMenu)
                {
                    return true;
                }
            }
            else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
            {
                if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.EnableContextMenu)
                {
                    return true;
                }
            }
            else if (cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                !cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()))
            {
                if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.EnableContextMenu && cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()))
                {
                    return true;
                }
                if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.EnableContextMenu && cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the ToolTipEnabled Value based on Cell Type and style
        /// </summary>
        /// <param name="cellInfo">The cell info.</param>
        /// <param name="styleInfo">The style info.</param>
        /// <returns>True if the PivotCellInfo and its Style is set to show ToolTip; else if false.</returns>
        private bool GetToolTipEnabledValue(PivotCellInfo cellInfo, GridStyleInfo styleInfo)
        {
            if (PivotGridTooltipService.GetShowTooltips(this.GridControl))
            {
                if (cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) ||
                    cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                {
                    if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) || cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()))
                    {
                        if (this.GridControl.SummaryHeaderStyle != null && this.GridControl.SummaryHeaderStyle.ToolTipEnabled)
                        {
                            if (GridControl.SummaryHeaderStyle.CustomToolTipTemplateKey != null)
                                styleInfo.TooltipTemplateKey = GridControl.SummaryHeaderStyle.CustomToolTipTemplateKey;
                            return true;
                        }
                        return false;
                    }
                    if (this.GridControl.SummaryCellStyle != null && this.GridControl.SummaryCellStyle.ToolTipEnabled)
                    {
                        if (GridControl.SummaryCellStyle.CustomToolTipTemplateKey != null)
                            styleInfo.TooltipTemplateKey = GridControl.SummaryCellStyle.CustomToolTipTemplateKey;
                        return true;
                    }
                }
                else if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                    !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
                {
                    if (this.GridControl.ColumnHeaderCellStyle != null && this.GridControl.ColumnHeaderCellStyle.ToolTipEnabled)
                    {
                        if (GridControl.ColumnHeaderCellStyle.CustomToolTipTemplateKey != null)
                            styleInfo.TooltipTemplateKey = GridControl.ColumnHeaderCellStyle.CustomToolTipTemplateKey;
                        return true;
                    }
                }
                else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                    !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()))
                {
                    if (this.GridControl.RowHeaderCellStyle != null && this.GridControl.RowHeaderCellStyle.ToolTipEnabled)
                    {
                        if (GridControl.RowHeaderCellStyle.CustomToolTipTemplateKey != null)
                            styleInfo.TooltipTemplateKey = GridControl.RowHeaderCellStyle.CustomToolTipTemplateKey;
                        return true;
                    }
                }
                else if (cellInfo.CellType.ToString().Contains(PivotCellType.ValueCell.ToString()))
                {
                    if (this.GridControl.ValueCellStyle != null && this.GridControl.ValueCellStyle.ToolTipEnabled)
                    {
                        if (GridControl.ValueCellStyle.CustomToolTipTemplateKey != null)
                            styleInfo.TooltipTemplateKey = GridControl.ValueCellStyle.CustomToolTipTemplateKey;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        private int ColumnIndexLimit { get; set; }
    }


    /// <summary>
    /// Specifies the type of Value Cell.
    /// </summary>
    public enum PivotGridValueCellType
    {
        /// <summary>
        /// Performs for only Value Cell.
        /// </summary>
        ValueCell,
        /// <summary>
        /// Performs for only Total Cell.
        /// </summary>
        SummaryCell,
        /// <summary>
        /// Performs for only Grand Total Cell.
        /// </summary>
        GrandTotalCell,
        /// <summary>
        /// Performs for the Combination of Total Cell and Value Cell.
        /// </summary>
        SummaryValueCell,
        /// <summary>
        /// Performs for the combination of Grand Total Cell and Value Cell.
        /// </summary>
        GrandTotalValueCell,
        /// <summary>
        /// Performs for the combination of Total Cell and Grand Total Cell.
        /// </summary>
        GrandTotalSummaryValueCell,
        /// <summary>
        /// Performs for the combination of Total Cell, Grand Total Cell and Value Cell.
        /// </summary>
        All
    }

    /// <summary>
    /// Manages text data exchange for the Pivot Grid table range. Lets you copy cell text to a stream or clipboard and recreate the
    /// cell text at a later time.
    /// </summary>
    public class PivotModelTextDataExchange : GridModelTextDataExchange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.Controls.PivotGrid.PivotModelTextDataExchange">PivotModelTextDataExchange</see> class. 
        /// </summary>
        /// <param name="model">Grid Model</param>
        public PivotModelTextDataExchange(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets the PivotEngine attached to.
        /// </summary>
        /// <value>Pivot Engine</value>
        public PivotEngine Engine
        {
            get;
            internal set;
        }

        /// <summary>
        /// Copies text from list of specific ranges to buffer.
        /// </summary>
        /// <param name="buffer">copy of the text</param>
        /// <param name="rangeList">list of ranges whose text are to be copied.</param>
        /// <param name="nrowsdone">count of rows copied.</param>
        /// <param name="ncolsdone">count of columns copied.</param>
        /// <param name="clear">If set to <see langword="true"/>, then range list is cleared; otherwise, not.</param>
        /// <returns>true, if text copied successfully. false, otherwise.</returns>
        public override bool CopyTextToBuffer(out string buffer, GridRangeInfoList rangeList, out int nrowsdone, out int ncolsdone, bool clear)
        {
            if (!this.Model.SelectedRanges.ActiveRange.IsTable)
                return base.CopyTextToBuffer(out buffer, rangeList, out nrowsdone, out ncolsdone, clear);
            else
            {
                GridRangeInfoList rowRanges = rangeList.GetRowRanges(GridRangeInfoType.Cells | GridRangeInfoType.Rows);
                GridRangeInfoList colRanges = rangeList.GetColRanges(GridRangeInfoType.Cells | GridRangeInfoType.Cols);
                int nrows = rowRanges.Cast<GridRangeInfo>().Sum(range => range.Height);
                int ncols = colRanges.Cast<GridRangeInfo>().Sum(range => range.Width);

                var sb = new StringBuilder();
                nrowsdone = 0;
                ncolsdone = 0;

                string tabDelim = "\t";
                if (this.TabDelimiter != string.Empty)
                {
                    tabDelim = this.TabDelimiter;
                }

                string text = string.Empty;
                bool colFirst = true;

                if (this.Engine.Filters.Count > 0)
                {
                    foreach (FilterExpression expression in this.Engine.Filters)
                    {
                        if (!colFirst)
                        {
                            sb.Append(tabDelim);
                        }
                        text = expression.Name;
                        text = new StringBuilder(text)
                                    .ToString()
                                    .Trim();
                        // Append the Cell value to buffer text
                        sb.Append(text);
                        colFirst = false;
                    }
                    sb.Append(Environment.NewLine);
                }

                colFirst = true;
                if (this.Engine.PivotCalculations.Count > 0)
                {
                    foreach (PivotComputationInfo info in this.Engine.PivotCalculations)
                    {
                        if (!colFirst)
                        {
                            sb.Append(tabDelim);
                        }
                        text = info.FieldHeader;
                        text = new StringBuilder(text)
                                    .ToString()
                                    .Trim();
                        // Append the Cell value to buffer text
                        sb.Append(text);
                        colFirst = false;
                    }
                    sb.Append(Environment.NewLine);
                }

                colFirst = true;
                bool isFirstLoop = true;
                if (this.Engine.PivotColumns.Count > 0)
                {
                    foreach (PivotItem item in this.Engine.PivotColumns)
                    {
                        if (isFirstLoop)
                        {
                            for (int i = 0; i < this.Engine.PivotRows.Count; i++)
                            {
                                sb.Append(tabDelim);
                            }
                            isFirstLoop = false;
                        }
                        if (!colFirst)
                        {
                            sb.Append(tabDelim);
                        }
                        text = item.FieldHeader;
                        text = new StringBuilder(text)
                                    .ToString()
                                    .Trim();
                        // Append the Cell value to buffer text
                        sb.Append(text);
                        colFirst = false;
                    }
                    sb.Append(Environment.NewLine);
                }

                colFirst = true;


                for (int rowindex = 0; rowindex < rowRanges.Count; rowindex++)
                {
                    for (int nrow = rowRanges[rowindex].Top; nrow <= rowRanges[rowindex].Bottom; nrow++)
                    {
                        if (nrowsdone > 0)
                        {
                            sb.Append(Environment.NewLine);
                        }

                        ncolsdone = 0;
                        bool firstCol = true;
                        GridRangeInfo info;

                        for (int colindex = 0; colindex < colRanges.Count; colindex++)
                        {
                            for (int ncol = colRanges[colindex].Left; ncol <= colRanges[colindex].Right; ncol++)
                            {
                                int rowIndex = this.Engine.PivotCalculations.Count > 1 ? this.Engine.PivotColumns.Count : this.Engine.PivotColumns.Count > 0 ? this.Engine.PivotColumns.Count - 1 : 0;
                                if (nrow == rowindex && ncol == colRanges[0].Left)
                                {
                                    foreach (PivotItem item in this.Engine.PivotRows)
                                    {
                                        if (!colFirst)
                                        {
                                            sb.Append(tabDelim);
                                        }
                                        text = item.FieldHeader;
                                        text = new StringBuilder(text)
                                                    .ToString()
                                                    .Trim();
                                        // Append the Cell value to buffer text
                                        sb.Append(text);
                                        colFirst = false;
                                    }
                                    firstCol = false;
                                    ncol += this.Engine.PivotRows.Count > 0 ? this.Engine.PivotRows.Count - 1 : 0;
                                }
                                else
                                {
                                    if (!firstCol)
                                    {
                                        sb.Append(tabDelim);
                                    }

                                    //  Gets the Cell Value(Not Formated Value)
                                    text = this.GetCopyTextRowCol(nrow, ncol);

                                    info = GridRangeInfo.Empty;
                                    if (this.Model.CoveredCells.Find(nrow, ncol, out info))
                                    {
                                        if ((nrow > info.Top && nrow <= info.Bottom && ncol > info.Left && ncol <= info.Right) ||
                                            (info.Top == info.Bottom && nrow == info.Top && ncol > info.Left && ncol <= info.Right) ||
                                            (info.Left == info.Right && ncol == info.Left && nrow > info.Top && nrow <= info.Bottom))
                                            text = string.Empty;
                                    }

                                    if (!rangeList.AnyRangeContains(GridRangeInfo.Cell(nrow, ncol)))
                                    {
                                        ncolsdone++;
                                        continue;
                                    }

                                    text = new StringBuilder(text)
                                        .ToString()
                                        .Trim();
                                    // Append the Cell value to buffer text
                                    sb.Append(text);
                                    firstCol = false;
                                    CutValueFromCell(nrow, ncol, clear);
                                    ncolsdone++;
                                }
                            }
                        }
                        nrowsdone++;
                    }
                }
                foreach (GridRangeInfo range in rangeList)
                {
                    this.Model.InvalidateCell(range);
                }
                this.Model.InvalidateVisual(true);
                buffer = sb.ToString();
                return true;
            }
        }
    }

    #region PivotValueEdited event
    /// <summary>
    /// Event handler for the <see cref="RowPivotsOnlyContextMenuShowingEvent"/> event.
    /// </summary>
    /// <param name="sender">The InternalGrid raising the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void RowPivotsOnlyContextMenuShowingEvent(object sender, RowPivotsOnlyContextMenuShowingArgs e);

    /// <summary>
    /// Event argumment for the RowPivotsOnlyContextMenuShowing event.
    /// </summary>
    public class RowPivotsOnlyContextMenuShowingArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets whether the the menu should be displayed
        /// </summary>
        public bool Cancel { get; set; }
        /// <summary>
        /// Gets the row index in the grid of the cell that was clicked.
        /// </summary>
        public int RowIndex { get; internal set; }
        /// <summary>
        /// Gets the column index in the grid of the cell that was clicked.
        /// </summary>
        public int ColumnIndex { get; internal set; }
        /// <summary>
        /// Gets the ContextMenu.
        /// </summary>
#if !SILVERLIGHT
        public ContextMenu ContextMenu { get; internal set; }
#endif
    }

    #endregion

}