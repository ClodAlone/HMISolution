#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion


#if !SILVERLIGHT
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Data;
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Syncfusion.Windows.Controls.Grid;
    using System.ComponentModel;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows;

    /// <summary>
    /// OlapGrid Renderer
    /// </summary>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapGridBase : GridControlBase
    {
        #region Private Variables

        private OlapDataManager _DataManager;

        private OlapGridCellStyle _ColumnHeaderStyle;

        private OlapGridCellStyle _RowHeaderStyle;

        private OlapGridCellStyle _SummaryColumnStyle;

        private OlapGridCellStyle _SummaryRowStyle;

        private OlapGridCellStyle _ValueCellStyle;

        private bool _ShowValueCellToolTip;

        private bool _ShowHeaderCellsToolTip;

        private bool _ShowMemberPropertiesToolTip;

        private bool m_AllowSelection;

        private bool m_FreezeHeaders;

        #endregion

        #region Events

        /// <summary>
        /// Event triggers before refreshing the data
        /// </summary>
        public event OlapGridDrillDownEventHander AfterRefresh;

        /// <summary>
        /// Event triggers after refreshing the data
        /// </summary>
        public event OlapGridDrillDownEventHander BeforeRefresh;

        /// <summary>
        /// Event triggers when the hyperlink cell is been clicked
        /// </summary>
        public event LinkLabelClickEventHander LinkClick;
        #endregion

        #region Initlize /Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridBase"/> class.
        /// </summary>
        public OlapGridBase()
        {
           this.SelectionChanging += new GridSelectionChangingEventHandler(OlapGridBase_SelectionChanging);
        }

        /// <summary>
        /// Handles the SelectionChanging event of the OlapGridBase control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridSelectionChangingEventArgs"/> instance containing the event data.</param>
        void OlapGridBase_SelectionChanging(object sender, GridSelectionChangingEventArgs e)
        {
            if (this.Engine != null)
            {
                if (e.Range != Syncfusion.Windows.Controls.Grid.GridRangeInfo.Empty)
                {
                    this.FillSelectedItems(e.Range);
                    this.OlapGrid.RaiseSelectionEvent(new OlapGridSelectionChangedEventArgs(e.Range, this.SelectedItems, e.Reason));
                }
            }
        }

        #endregion

        #region Properties

#if !SILVERLIGHT
        #region Paging Properties

        /// <summary>
        /// Gets or sets the olap paging grid instance.
        /// </summary>
        /// <value>The olap paging grid instance.</value>
        internal OlapPagingGrid OlapPagingGridInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the paging info.
        /// </summary>
        /// <value>The paging info.</value>
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        /// <value>The total number of pages.</value>
        public int TotalNumberOfPages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show headers on each page].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show headers on each page]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHeadersOnEachPage
        {
            private get;
            set;
        }

        #endregion
#endif

        public GridLayout Layout
        {
            get;
            set;
        }



        /// <summary>
        /// Gets or sets a value indicating whether header cells are freezed
        /// </summary>
        /// <remarks>
        /// This property enable you to freeze or unfreeze the header cells
        /// </remarks>
        /// <value>
        /// <see langword="true" /> if ; otherwise, <see langword="false" />.
        /// </value>
        public bool FreezeHeaders
        {
            get
            {
                return m_FreezeHeaders;
            }
            
            set
            {
                m_FreezeHeaders = value;
                if (this.Model != null)
                {
                    if (value)
                    {
                        this.Model.FrozenColumns = this.Model.HeaderColumns;
                        this.Model.FrozenRows = this.Model.HeaderRows;
                    }
                    else
                    {
                        this.Model.FrozenColumns = 0;
                        this.Model.FrozenRows = 0;
                    }
                }
            }            
        }

        /// <summary>
        /// Gets or sets the value cell text alignment.
        /// </summary>
        /// <value>The value cell text alignment.</value>
        public HorizontalAlignment ValueCellTextAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the GridLineStroke.
        /// </summary>
        /// <value>The GridLineStroke.</value>
        [DefaultValue(null)]
        public Brush GridLineStroke
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the border thickness of a control.
        /// </summary>
        /// <value></value>
        /// <returns>A thickness value; the default is a thickness of 0 on all four sides.</returns>
        public double GridLineThickness
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        internal SelectedItems SelectedItems { get; set; }

        /// <summary>
        /// Gets or sets the data manager.
        /// </summary>
        /// <value>The data manager.</value>
        [DefaultValue(null)]
        public OlapDataManager DataManager
        {
            get
            {
                return _DataManager;
            }
            set
            {
                bool isFirstCall = _DataManager == null;
                _DataManager = value;
                if (this._DataManager != null)
                {
#if !SILVERLIGHT
                    this._DataManager.AxisElementChanged -= new AxisElementChangedEventHandler(_DataManager_AxisElementChanged);
                    this._DataManager.AxisElementChanged += new AxisElementChangedEventHandler(_DataManager_AxisElementChanged);
#endif
                    if (this._DataManager.CurrentCellSet != null)
                    {
                        if(this.Model == null)
                            this.Model = new OlapGridModel(this);
                        //Commented following lines because of layout changes in Engine. There fore creating the new Engine, while changing the data manager
                        //if (this.DataManager.PivotEngine == null)
                            this.Engine = this.DataManager.ExecuteOlapTable(this.DataManager.CurrentCellSet, this.Layout);
                        //else
                        //    this.Engine = this.DataManager.PivotEngine;
                    }
                    else
                    {
                        DataBind();
                    }
                }
            }
        }
        
        public PivotEngine Engine
        {
            get { return (PivotEngine)GetValue(EngineProperty); }
            set { SetValue(EngineProperty, value); }
        }

        public static readonly DependencyProperty EngineProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("Engine", typeof(PivotEngine), typeof(OlapGridBase), new UIPropertyMetadata(null, OlapGridBase.OnEngineChanged));
#else
            DependencyProperty.Register("Engine", typeof(PivotEngine), typeof(OlapGridBase), new PropertyMetadata(null, OlapGridBase.OnEngineChanged));
#endif

        public static void OnEngineChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapGridBase olapGridBase = dependencyObject as OlapGridBase;
            if (olapGridBase != null )
            {
                PivotEngine _engine = e.NewValue as PivotEngine;
                olapGridBase.Model.Engine = _engine;
                olapGridBase.IsResizeColumnsUpdated = false;
            }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public new OlapGridModel Model
        {
            get
            {
                OlapGridModel model = base.Model as OlapGridModel;
                if (model == null)
                {
                    this.Model = new OlapGridModel(this);
                    return this.Model;
                }
                return model;
            }

            set
            {
                base.Model = value;
            }
        }

        /// <summary>
        /// Gets or sets the column header style.
        /// </summary>
        /// <value>The column header style.</value>
        [DefaultValue(null)]
        public OlapGridCellStyle ColumnHeaderStyle
        {
            get
            {
                return _ColumnHeaderStyle;
            }
            set
            {
                _ColumnHeaderStyle = value;
                UnwireAndWireEvents(_ColumnHeaderStyle);
            }
        }

        /// <summary>
        /// Gets or sets the row header style.
        /// </summary>
        /// <value>The row header style.</value>
        [DefaultValue(null)]
        public OlapGridCellStyle RowHeaderStyle
        {
            get
            {
                return _RowHeaderStyle;
            }
            set
            {
                _RowHeaderStyle = value;
                UnwireAndWireEvents(_RowHeaderStyle);
            }
        }

        /// <summary>
        /// Gets or sets the summary column style.
        /// </summary>
        /// <value>The summary column style.</value>
        [DefaultValue(null)]
        public OlapGridCellStyle SummaryColumnStyle
        {
            get
            {
                return _SummaryColumnStyle;
            }
            set
            {
                _SummaryColumnStyle = value;
                UnwireAndWireEvents(_SummaryColumnStyle);
            }
        }

        /// <summary>
        /// Gets or sets the summary row style.
        /// </summary>
        /// <value>The summary row style.</value>
        [DefaultValue(null)]
        public OlapGridCellStyle SummaryRowStyle
        {
            get
            {
                return _SummaryRowStyle;
            }
            set
            {
                _SummaryRowStyle = value;
                UnwireAndWireEvents(_SummaryRowStyle);
            }
        }

        /// <summary>
        /// Gets or sets the value cells style.
        /// </summary>
        /// <value>The value cells style.</value>
        [DefaultValue(null)]
        public OlapGridCellStyle ValueCellStyle
        {
            get
            {
                return _ValueCellStyle;
            }
            set
            {
                _ValueCellStyle = value;
                UnwireAndWireEvents(_ValueCellStyle);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show value cells tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show value cells tool tip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValueCellToolTip
        {
            get
            {
                return _ShowValueCellToolTip;
            }
            set
            {
                _ShowValueCellToolTip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show header cells tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show header cells tool tip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowHeaderCellsToolTip
        {
            get
            {
                return _ShowHeaderCellsToolTip;
            }
            set
            {
                _ShowHeaderCellsToolTip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show member properties tool tip].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show member properties tool tip]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowMemberPropertiesToolTip
        {
            get
            {
                return _ShowMemberPropertiesToolTip;
            }
            set
            {
                _ShowMemberPropertiesToolTip = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow selection].
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        public bool AllowSelection 
        {
            get
            {
                return m_AllowSelection;
            }
            set
            {
                if (!value)
                {
                    if (this.Model.Selections.Count > 0)
                    {
                        this.Model.Selections.Clear();
                    }
                    this.Model.Options.AllowSelection = GridSelectionFlags.None;
                    this.Model.Options.ExcelLikeCurrentCell = false;
                    this.Model.Options.ExcelLikeSelectionFrame = false;
                    m_AllowSelection = value;
                }
                else
                {
                    this.Model.Options.ExcelLikeCurrentCell = true;
                    this.Model.Options.ExcelLikeSelectionFrame = true;
                    this.Model.Options.AllowSelection = GridSelectionFlags.Cell;
                    m_AllowSelection = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to Resize columns to fit.
        /// </summary>
        /// <value><c>true</c> if [resize columns to fit]; otherwise, <c>false</c>.</value>
        public bool ResizeColumnsToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to Resize rows to fit.
        /// </summary>
        /// <value><c>true</c> if [resize rows to fit]; otherwise, <c>false</c>.</value>
        public bool ResizeRowsToFit { get; set; }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets a value indicating whether this drill processing is on progress
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance ; otherwise, <see langword="false" />.
        /// </value>
        internal bool IsProcessing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is resize columns updated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is resize columns updated; otherwise, <c>false</c>.
        /// </value>
        internal bool IsResizeColumnsUpdated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the OlapGridControl
        /// </summary>
        internal OlapGrid OlapGrid
        {
            get;
            set;
        }

        internal PivotCellDescriptor CurrentCellDescriptor { get; set; }

        #endregion

        #region private Methods

        /// <summary>
        /// Handles the PropertyChanged event of the Style control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Style_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.InvalidateCells();
        }

        /// <summary>
        /// Unwires the and wire events.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        private void UnwireAndWireEvents(OlapGridCellStyle cellStyle)
        {
            if (cellStyle != null)
            {
                cellStyle.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(Style_PropertyChanged);
                cellStyle.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Style_PropertyChanged);
            }
            this.InvalidateCells();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the AxisElementChanged event of the _DataManager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Olap.Model.AxisElementChangedEventArgs"/> instance containing the event data.</param>
        private void _DataManager_AxisElementChanged(object sender, AxisElementChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new System.Action(delegate()
                    {
                        PivotEngine _engine = null;
                        if (this.DataManager != null)
                        {
                            if (this.DataManager.ItemSource == null)
                            {
                                _engine = this.DataManager.ExecuteOlapTable(this.DataManager.CurrentCellSet, this.Layout);
                            }
                            else
                                _engine = this.DataManager.PivotEngine;
                            if (this.Model == null)
                            {
                                this.Model = new OlapGridModel(this);
                            }
                            this.Engine = _engine;
                        }
                    }));
        }
#endif
        #endregion

        #region Internal Methods


#if !SILVERLIGHT
        /// <summary>
        /// Raises the grid cell click.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="colIndex">Index of the col.</param>
        internal void RaiseGridCellClick(int rowIndex, int colIndex)
        {
            GridCellClickEventArgs e = new GridCellClickEventArgs(rowIndex, colIndex, 1, GridControlBase.CellClickEvent, this);
            base.OnCellClick(e);
        }

        /// <summary>
        /// Raises the grid cell cursor.
        /// </summary>
        /// <returns></returns>
        internal Cursor RaiseGridCellCursor()
        {
            GridCellCursorEventArgs args = new GridCellCursorEventArgs(GridControlBase.CellCursorEvent, this);
            if (args.Handled)
                return args.Cursor;
            return Cursors.Arrow;
        }

        /// <summary>
        /// Raises the after drill down.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="drillDownEventArgs">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseAfterDrillDown(PivotCellDescriptor cellDescriptor, OlapGridDrillDownEventArgs drillDownEventArgs)
        {
            drillDownEventArgs.CellDescriptor = cellDescriptor;
            if (this.AfterRefresh != null)
            {
                if (drillDownEventArgs.ShowDefaultIndicator)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                //// Triggering after refresh method
                this.AfterRefresh(this, drillDownEventArgs);
            }
            else
            {
                this.Cursor = drillDownEventArgs.CurrentCursor;
            }
        }

        /// <summary>
        /// Raises the before drill down.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="drillDownEventArgs">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseBeforeDrillDown(PivotCellDescriptor cellDescriptor, OlapGridDrillDownEventArgs drillDownEventArgs)
        {
            drillDownEventArgs.CellDescriptor = cellDescriptor;
            if (this.BeforeRefresh != null)
            {
                //// Triggering the before refresh method
                this.BeforeRefresh(this, drillDownEventArgs);
                if (drillDownEventArgs.ShowDefaultIndicator)
                {
                    drillDownEventArgs.CurrentCursor = this.Cursor;
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                }
            }
            else
            {
                drillDownEventArgs.CurrentCursor = this.Cursor;
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
            }
        }
#endif

        /// <summary>
        /// Raises the label click.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Grid.Olap.LinkLabelEventArgs"/> instance containing the event data.</param>
        internal void RaiseLabelClick(LinkLabelEventArgs args)
        {
            if (this.LinkClick != null)
            {
                this.LinkClick(this, args);
            }
        }

        /// <summary>
        /// Raises the before refresh.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Silverlight.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseBeforeRefresh(OlapGridDrillDownEventArgs args)
        {
            if (this.BeforeRefresh != null)
            {
                this.CurrentCellDescriptor = args.CellDescriptor;
                this.BeforeRefresh(this, args);
            }
        }

        /// <summary>
        /// Raises the after refresh.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Silverlight.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void RaiseAfterRefresh(OlapGridDrillDownEventArgs args)
        {
            if (this.AfterRefresh != null)
            {
                this.AfterRefresh(this, args);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Raises the <see cref="E:ResizingColumns"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridResizingColumnsEventArgs"/> instance containing the event data.</param>
        protected override void OnResizingColumns(GridResizingColumnsEventArgs args)
        {
            if (this.OlapGrid != null)
            {
                if (this.OlapGrid.AllowResizeColumns)
                {
                    base.OnResizingColumns(args);
                    VisibleLineInfo lineInfo = this.ScrollRows.GetVisibleLineAtPoint(args.Point.Y);
                    if (lineInfo != null)
                    {
                        if (lineInfo.LineIndex < this.Model.HeaderRows)
                        {
                            args.AllowResize = true;
                        }
                        else
                        {
                            args.AllowResize = false;
                        }
                    }
                    else
                    {
                        args.AllowResize = false;
                    }
                }
                else
                {
                    args.AllowResize = false;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ResizingRows"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridResizingRowsEventArgs"/> instance containing the event data.</param>
        protected override void OnResizingRows(GridResizingRowsEventArgs args)
        {
            if (this.OlapGrid != null)
            {
                if (this.OlapGrid.AllowResizeRows)
                {
                    base.OnResizingRows(args);
                    VisibleLineInfo lineInfo = this.ScrollColumns.GetVisibleLineAtPoint(args.Point.X);
                    if (lineInfo != null)
                    {
                        if (lineInfo.LineIndex < this.Model.HeaderColumns)
                        {
                            args.AllowResize = true;
                        }
                        else
                        {
                            args.AllowResize = false;
                        }
                    }
                    else
                    {
                        args.AllowResize = false;
                    }
                }
                else
                {
                    args.AllowResize = false;
                }
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Binds the data and renders the OlapGrid
        /// </summary>
        public void DataBind()
        {
            PivotEngine _engine = null;
#if !SILVERLIGHT
            if (this.DataManager != null && this.DataManager.CurrentReport != null)
            {
                if (this.DataManager.ItemSource == null)
                {
                    Syncfusion.Olap.Data.CellSet cellSet = this.DataManager.ExecuteCellSet();
                    _engine = this.DataManager.ExecuteOlapTable(cellSet, this.Layout);
#else       
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.DataManager != null && this.DataManager.CurrentCubeSchema == null && (this.OlapGrid.EnableColumnHeaderContextMenu || this.OlapGrid.EnableRowHeaderContextMenu))
                    this.DataManager.GetCubeSchema(this.DataManager.CurrentReport.CurrentCubeName);
            }
            if (this.DataManager != null && !this.DataManager.IsProcessing)
            {
                if (this.DataManager.ItemSource == null)
                {
                    this.DataManager.ExecuteCellSet();
                    //_engine = this.DataManager.ExecuteOlapTable(this.DataManager.CurrentCellSet, this.Layout);
#endif

                }
                else
                {
                    _engine = this.DataManager.ExecuteOlapTable(this.Layout);
                }
            }

            this.Model = new OlapGridModel(this);
            this.Engine = _engine;
            
        }

        public void DataBind(PivotEngine engine)
        {
            this.Model = new OlapGridModel(this);
            this.Engine = engine;
        }

        /// <summary>
        /// Fills the selected items with appropriate Column, Row and Value based on the Specified Range
        /// </summary>
        /// <param name="gridRangeInfo">The grid range info.</param>
        private void FillSelectedItems(Syncfusion.Windows.Controls.Grid.GridRangeInfo gridRangeInfo)
        {
            bool MultipleCells = gridRangeInfo.ToString().Contains(":");

            if (!MultipleCells)
            {
                this.SelectedItems = new SelectedItems();

                this.SelectedItems.Add(new SelectedItem
                {
                    ColumnList = GetColumnList(gridRangeInfo.Left).ColumnList,
                    RowList = GetRowList(gridRangeInfo.Bottom).RowList,
                    FormattedValue = this.Engine[gridRangeInfo.Bottom, gridRangeInfo.Left].CellValue,
                    Value = this.Engine[gridRangeInfo.Bottom, gridRangeInfo.Left].Value == null ? null : this.Engine[gridRangeInfo.Bottom, gridRangeInfo.Left].Value.ToString()
                });
            }

            else
            {
                this.SelectedItems = new SelectedItems();
                SelectedItems selectedRowLists = new SelectedItems();

                for (int col = gridRangeInfo.Left; col <= gridRangeInfo.Right; col++)
                {
                    SelectedItem selectedColList = GetColumnList(col);

                    for (int row = gridRangeInfo.Top; row <= gridRangeInfo.Bottom; row++)
                    {
                        if (col == gridRangeInfo.Left)
                        {
                            selectedRowLists.Add(GetRowList(row));
                        }

                        this.SelectedItems.Add(new SelectedItem
                        {
                            ColumnList = selectedColList.ColumnList,
                            RowList = selectedRowLists[row - gridRangeInfo.Top].RowList,
                            FormattedValue = this.Engine[gridRangeInfo.Bottom, gridRangeInfo.Left].CellValue,
                            Value = this.Engine[row, col].Value == null ? null : this.Engine[row, col].Value.ToString()
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Gets the column list.
        /// </summary>
        /// <param name="gridRangeInfo">The grid range info.</param>
        /// <returns></returns>
        private SelectedItem GetColumnList(int col)
        {
            SelectedItem m_SelectedItem = new SelectedItem();

            for (int row = 0; row < this.Engine.HeaderSection.Height; row++)
            {
                PivotCellDescriptor cellDesc = GetParentCell(this.Engine[row, col]);
                m_SelectedItem.ColumnList.Add(cellDesc.CellValue);
            }

            return m_SelectedItem;
        }

        /// <summary>
        /// Gets the row list.
        /// </summary>
        /// <param name="gridRangeInfo">The grid range info.</param>
        /// <returns></returns>
        private SelectedItem GetRowList(int row)
        {
            SelectedItem m_SelectedItem = new SelectedItem();

            for (int col = 0; col < this.Engine.RowHeaderSection.Width; col++)
            {
                PivotCellDescriptor cellDesc = GetParentCell(this.Engine[row, col]);
                m_SelectedItem.RowList.Add(cellDesc.CellValue);
            }
        
            return m_SelectedItem;
        }


        /// <summary>
        /// Gets the parent cell.
        /// </summary>
        /// <param name="pivotCellDescriptor">The pivot cell descriptor.</param>
        /// <returns></returns>
        private PivotCellDescriptor GetParentCell(PivotCellDescriptor pivotCellDescriptor)
        {
            while (pivotCellDescriptor.SpanCell != null)
            {
                pivotCellDescriptor = pivotCellDescriptor.SpanCell;
            }
            return pivotCellDescriptor;
        }     

        #region Paging Methods

#if !SILVERLIGHT
        /// <summary>
        /// Moves to the specific page.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        public void MoveTo(int pageNumber)
        {
            if (this.OlapPagingGridInstance != null)
            {
                if (this.OlapPagingGridInstance.TotalPageNumbers > 1)
                {
                    this.OlapPagingGridInstance.MoveTo(pageNumber);
                    var indices = this.OlapPagingGridInstance.GetIndices(pageNumber);
                    this.Model.StartIndex = indices.StartIndex;
                    this.Model.EndIndex = indices.EndIndex;
                    this.Model.HorizontalHeaderLength = this.OlapPagingGridInstance.HorizontalHeaderLength;
                    this.Model.VerticalHeaderLength = this.OlapPagingGridInstance.VerticalHeaderLength;
                    this.Model.ShowHeadersOnEachPage = this.ShowHeadersOnEachPage;
                    this.Model.Refresh();
                }
            }
        }
#endif

#endregion

        #endregion
    }

}