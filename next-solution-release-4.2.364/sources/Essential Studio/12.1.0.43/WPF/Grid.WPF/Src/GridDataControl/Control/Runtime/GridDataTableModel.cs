#region Copyright
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
#if !SILVERLIGHT
    using System.Data;
    using System.Data.Linq;
#else
    using System.Reflection;
    using System.Windows.Controls.Primitives;
    using System.Windows.Controls;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using Syncfusion.Linq;
    using Syncfusion.Linq.Data;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Diagnostics;
    using System.Collections.Specialized;
    using System.Windows.Data;
    using System.Text;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Shared;
    using System.Xml.Serialization;
    using System.Xml;
    using System.IO;
    using System.Windows.Input;
#if !SILVERLIGHT
    using Syncfusion.Windows.Controls.Grid.GridCellRenderer.DropdownCellRenderers;
    // using System.Windows.Input; warning CS0105
    using System.Windows.Controls;
    using Syncfusion.Windows.GridCommon;
    using System.Reflection;
   
   
#endif

    /// <summary>
    /// Interface that provides the Grid Model to any class that implements this.
    /// </summary>
    public interface IGridModelProvider
    {
        void SetTableModel(GridDataTableModel model);
    }


    /// <summary>
    /// Represents the child tables in groups of PrimaryKey columns.
    /// </summary>
    public partial class GridDataTableModel : GridModel, ISupportInitialize
    {
        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>Specifies the GridControl instance</value>
        private GridControlBase grid = null;

        private IGridDataVisualStyle gridVisualStyle = null;

        /// <summary>
        /// Gets the source list.
        /// </summary>
        /// <value>The source list.</value>
        private IEnumerable sourceList = null;

        /// <summary>
        /// private accessor for GridTable.
        /// </summary>
        protected GridDataTable gridDataTable = null;

        private GridDataTableProperties tableProperties;

        internal Dictionary<int,GridDataRecord> DetailsViewRows;

        public const double ExpandCollapseCellWidth = 24d;
        public const double HeaderRowHeight = 28d;
        public const double DefaultRowHeight = 24d;

        internal bool IsSourceReset = false;
        private static Dictionary<IEnumerable, ICollectionViewAdv> sourceListsKey = new Dictionary<IEnumerable, ICollectionViewAdv>();
        private static GridDataTableModel emptyModel = new GridDataTableModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModel"/> class.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> which is binded with the TableModel</param>
        public GridDataTableModel(GridControlBase grid)
            : this()
        {
            this.Grid = grid;
        }

        public GridDataTableModel()
        {
            this.TextDataExchange = new GridDataModelTextDataExchange(this);
            //this properties used to maintain the filter persistence
            this.RemovedFilterPredicateBeforeLoaded = new List<RecordEntry>();
            this.FilterPredicateBeforeLoaded = new List<RecordEntry>();

            this.Options.ExcelLikeCurrentCell = true;
            this.Options.ExcelLikeSelectionFrame = false;
            this.Options.ShowCurrentCell = true;
            this.Options.ListBoxModeAllowUIElementClick = true;
            this.Options.ListBoxSelectionMode = GridSelectionMode.One;
            this.Options.ActivateCurrentCellBehavior = GridCellActivateAction.DblClickOnCell;            
            this.Options.AllowSelection = GridSelectionFlags.Any;
            this.Options.WrapCell = true;
            this.CellModels.Add("DropDownFilterCell", new GridDataDropDownFilterBarCellModel());

#if !SILVERLIGHT
            this.Options.DrawSelectionOptions = GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor | GridDrawSelectionOptions.ExcelLikeSelectionMarker;
            //this.Options.DrawSelectionOptions = GridDrawSelectionOptions.AlphaBlend | GridDrawSelectionOptions.ExcelLikeSelectionMarker;
            this.CellModels.Add("RowHeaderCell", new GridDataCellRowHeaderModel());
            this.CellModels.Add("SortableHeaderCell", new GridCellModel<GridCellSortHeaderRenderer>());
            ////this.CellModels.Add("ExpandCollapseCell", new GridCellModel<GridDataExpandCollapseContentCellRenderer>());
            this.CellModels.Add("ExpandCollapseCell", new GridCellModel<GridDataExpandCollapseVisualCellRenderer>());

            this.CellModels.Add("GridDataControlCell", new GridDataControlDropDownCellModel());            
           
            this.CellModels.Add("FilterBarCell", new GridDataFilterBarCellModel());
            ////this.CellModels.Add("ExpandCollapseCell", new GridCellModel<GridDataCellExpandRenderer>());
            this.CellModels.Add("NestedGrid", new GridDataCellNestedGridModel(GridNestedAxisLayout.Nested, GridNestedAxisLayout.Normal));
            var headerModel = new GridDataHeaderCellModel();
            headerModel.CanShowFilterButton = true;
            headerModel.CanShowColumnOptionsButton = true;
            this.CellModels.Add("HeaderCell", headerModel);
            ////this.CellModels.Add("ContentCell", new GridCellModel<GridDataBackgroundContentCellRenderer>());
            this.CellModels.Add("AddNewHeaderCell", new GridCellModel<GridDataAddNewRowContentCellRenderer>());
            this.CellModels.Add("GridDataBoundTemplate", new GridDataDataBoundTemplateCellBoundModel());
#else
            this.Options.DrawSelectionOptions = GridDrawSelectionOptions.ReplaceBackground | GridDrawSelectionOptions.ReplaceTextColor;
            //this.CellModels.Add("RowHeaderCell", new GridCellTextBlockModel());
            this.CellModels.Add("FilterBarCell", new GridDataFilterBarCellModel());
            this.CellModels.Add("RowHeaderCell", new GridDataCellRowHeaderModel());
            this.CellModels.Add("SortableHeaderCell", new GridCellTextBlockModel());
            this.CellModels.Add("ExpandCollapseCell", new GridCellModel<GridDataExpandCollapseContentCellRenderer>());
            //this.CellModels.Add("AddNewHeaderCell", new GridCellTextBlockModel());
            this.CellModels.Add("AddNewHeaderCell", new GridCellModel<GridDataBackgroundContentCellRenderer>());
            //this.CellModels.Add("GridDataBoundTemplate", new GridCellModel<GridCellDataTemplateRenderer>());
            this.CellModels.Add("GridDataBoundTemplate", new GridDataDataBoundTemplateCellBoundModel());
            this.CellModels.Add("NestedGrid", new GridDataCellNestedGridModel(GridNestedAxisLayout.Nested, GridNestedAxisLayout.Normal));
            var headerModel = new GridDataHeaderCellModel();
            headerModel.CanShowFilterButton = true;
            headerModel.CanShowColumnOptionsButton = true;
            this.CellModels.Add("HeaderCell", headerModel);
#endif

            this.EmptyColumnStyle = new GridStyleInfo()
            {
                Background = Brushes.Transparent,
                CellType = "Static"
            };
#if SILVERLIGHT
            this.EmptyColumnStyle.Borders.All = new Pen();
#else
            this.EmptyColumnStyle.Borders.All = new Pen();
#endif
            this.TableStyle.VerticalAlignment = VerticalAlignment.Center;
            this.HeaderStyle.VerticalAlignment = VerticalAlignment.Center;
            this.indentColumnStyle.VerticalAlignment = VerticalAlignment.Center;
#if !SILVERLIGHT
            indentColumnStyle.Background = SystemColors.ControlBrush;
#else
            indentColumnStyle.Background = new SolidColorBrush(Colors.LightGray);
#endif
            indentColumnStyle.CellType = "Static";
            //this.RowHeights[0] = GridDataTableModel.HeaderRowHeight;
            //this.RowHeights[0] = this.TableProperties.DefaultHeaderRowHeight;
            this.RowHeights.DefaultLineSize = DefaultRowHeight;
            this.FrozenColumns = 0;
            this.gridDataTable = new GridDataTable(this);
            this.CurrencyManager = new GridDataCurrentRecordManager(this);
            this.ChildTableModelCollection = new ObservableCollection<GridDataChildTableModel>();
            this.DetailsViewRows = new Dictionary<int, GridDataRecord>();
        }

#if SILVERLIGHT
        #region Command
        protected override void OnCellRequestNavigate(CellRequestNavigateEventArgs e)
        {
            var style = this[e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex] as GridDataStyleInfo;
            e.Record = style.CellIdentity.Record;
            base.OnCellRequestNavigate(e);
            GridDataControl grid = this.Grid.FindParentElementOfType<GridDataControl>() as GridDataControl;
            if (grid.CellRequestNavigateCommand != null)
            {
                if (grid.CellRequestNavigateCommand.CanExecute(e))
                {
                    grid.CellRequestNavigateCommand.Execute(e);
                }
            }
        }
        #endregion
#endif

        public event GridDataQueryUnboundCellInfoEventHandler QueryUnboundCellInfo;

        /// <summary>
        /// Gets or sets the currency manager.
        /// </summary>
        /// <value>The currency manager.</value>
        public GridDataCurrentRecordManager CurrencyManager
        {
            get;
            internal set;
        }

        internal GridStyleInfo EmptyColumnStyle
        {
            get;
            set;
        }


        #region QueryVisibleColumnInfo


        public event QueryVisibleColumnInfoEventHandler QueryVisibleColumnInfo;

        protected virtual void OnQueryVisibleColumnInfo(QueryVisibleColumnInfoArgs args)
        {
            if (QueryVisibleColumnInfo != null)
                QueryVisibleColumnInfo(this, args);
        }

        public void RaiseQueryVisibleColumnInfo(QueryVisibleColumnInfoArgs e)
        {
            OnQueryVisibleColumnInfo(e);
        }

        #endregion
        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public GridControlBase Grid
        {
            get
            {
                return this.grid;
            }

            set
            {
                if (this.grid != value)
                {
                    this.grid = value;
                    this.WireGrid();
                    this.CurrencyManager.WireEvents();
                    this.ColumnAutoSizer.WireEvents();
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid visual style.
        /// </summary>
        /// <value>The grid visual style.</value>
        public IGridDataVisualStyle GridVisualStyle
        {
            get
            {
                //#if !SILVERLIGHT
                //                if (this.gridVisualStyle == null || GridDataTableModelHelper.IsInDesignMode)
                //#else
                if (this.gridVisualStyle == null)
                //#endif
                {
                    var defaultStyle = this.TableProperties.GetVisualStyle(VisualStyle.Default);
                    //this.ApplyGridVisualStyle(defaultStyle);
                    this.gridVisualStyle = defaultStyle;
                    //this.InvalidateVisual(true);
                }
                return this.gridVisualStyle;
            }

            set
            {
                if (this.gridVisualStyle != value)
                {
                    this.ApplyGridVisualStyle(gridVisualStyle,value);

                    var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();

                    if (gridDataControl != null && !TableProperties.IsLegacyStyleEnabled && value != null)
                    {
                        gridDataControl.BorderBrush = this.GetGridBorderBrush(value);
                        gridDataControl.BorderThickness = this.GetGridBorderThickness(value);
                    }

                    this.gridVisualStyle = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether IsDisposing property. Specifies if the object is disposing.
        /// </summary>
        private bool IsDisposing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditing
        {
            get
            {
                if (this.CurrencyManager != null)
                {
                    return this.CurrencyManager.IsEditing;
                }

                return false;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether IsInitialized property. Specifies if the object needs to be re-initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            private set;
        }

        protected override bool ShouldHideRow(int rowIndex, bool hide)
        {
            if (!this.Table.HasNestedTables)
            {
                return base.ShouldHideRow(rowIndex, hide);
            }

            if (rowIndex == this.ResolveAddNewPositionInGrid())
            {
                return true;
            }

            if (this.IsInNestedIndex(rowIndex))
            {
                var index = hide == true ? (rowIndex + 1) : (rowIndex - 1);
                if (this.IsInNestedIndex(index))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the source list.
        /// </summary>
        /// <value>Specifies the SourceList.</value>
        public IEnumerable SourceList
        {
            get
            {
                return this.sourceList;
            }

            internal set
            {
                if (this.sourceList != value)
                {
                    this.sourceList = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the abstract layer that bridges the gap between different kinds of data sources and it's underlying operations with the GridDataTableModel.
        /// </summary>
        /// <value>The layer.</value>
        /*public GridDataTableModelBinder Binder
        {
            get;
            private set;
        }*/

        /// <summary>
        /// Gets or sets the ICollectionViewAdv which interacts with the Model.
        /// </summary>
        public ICollectionViewAdv View
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the child table.
        /// </summary>
        /// <value>The child table.</value>
        public GridDataTable Table
        {
            get
            {
                if (!this.IsDisposing && !this.IsInitialized)
                {
                    this.EnsureInitialized();
                }

                return this.gridDataTable;
            }
        }

        /// <summary>
        /// Gets or sets the table descriptor.
        /// </summary>
        /// <value>The table descriptor.</value>
        public GridDataTableProperties TableProperties
        {
            get
            {
                return this.tableProperties;
            }

            set
            {
                if (this.tableProperties != value)
                {
                    this.tableProperties = value;

                    // set the Model value, if TableDescriptor is initialized thru XAML then too this would be assigned
                    this.tableProperties.SetTableModel(this);

                    // To stop applying the default visual style all the time irrelevant of the applied visual style
                    //if (value != null && this.gridVisualStyle == null)
                    //{
                    //    this.gridVisualStyle = new GridDataDefaultGridVisualStyle();
                    //    this.ApplyGridVisualStyle(null, gridVisualStyle);
                    //}
                }
            }
        }


        /// <summary>
        /// Gets or sets the unbound row count for the Model. This would add additional rows
        /// to the top of the Grid after the Column headers.
        /// </summary>
        public int UnboundRowsCount
        {
            get;
            set;
        }

#if !SILVERLIGHT
        private void ApplyFont(GridFontInfo styleFont)
        {
            var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl == null)
            {
                return;
            }

            styleFont.FontFamily = gridDataControl.hasFontFamilyChanged ? gridDataControl.FontFamily : styleFont.FontFamily;
            styleFont.FontSize = gridDataControl.hasFontSizeChanged ? gridDataControl.FontSize : styleFont.FontSize;
            styleFont.FontStretch = gridDataControl.hasFontStretchChanged ? gridDataControl.FontStretch : styleFont.FontStretch;
            styleFont.FontStyle = gridDataControl.hasFontStyleChanged ? gridDataControl.FontStyle : styleFont.FontStyle;
            styleFont.FontWeight = gridDataControl.hasFontWeightChanged ? gridDataControl.FontWeight : styleFont.FontWeight;

            if (gridDataControl.hasFontSizeChanged)
            {
                //SD17508-DefaultHeaderRowHeight in GridDataControl not working while defining Font
                if (this.TableProperties != null && gridDataControl.DefaultHeaderRowHeight == GridDataTableModel.HeaderRowHeight)
                {
                    this.TableProperties.DefaultHeaderRowHeight = gridDataControl.FontSize * 1.8;
                }
                this.RowHeights.DefaultLineSize = gridDataControl.FontSize * 1.8;
            }
        }
#else
        private void ApplyFont(GridFontInfo styleFont)
        {
            var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl == null)
            {
                return;
            }

            //var fontFamilyPropertyMetadata = GridDataControl.FontFamilyProperty.GetMetadata(typeof(GridDataControl));
            //var hasFontFamily = fontFamilyPropertyMetadata.DefaultValue != gridDataControl.FontFamily;
            //styleFont.FontFamily = hasFontFamily ? gridDataControl.FontFamily : styleFont.FontFamily;

            //var fontSizePropertyMetadata = GridDataControl.FontSizeProperty.GetMetadata(typeof(GridDataControl));
            //var hasFontSize = (double)fontSizePropertyMetadata.DefaultValue != gridDataControl.FontSize;
            //styleFont.FontSize = hasFontSize ? gridDataControl.FontSize : styleFont.FontSize;

            //var fontStretchPropertyMetadata = GridDataControl.FontStretchProperty.GetMetadata(typeof(GridDataControl));
            //var hasFontStretch = (FontStretch)fontStretchPropertyMetadata.DefaultValue != gridDataControl.FontStretch;
            //styleFont.FontStretch = hasFontStretch ? gridDataControl.FontStretch : styleFont.FontStretch;

            //var fontStylePropertyMetadata = GridDataControl.FontStyleProperty.GetMetadata(typeof(GridDataControl));
            //var hasFontStyle = (FontStyle)fontStylePropertyMetadata.DefaultValue != gridDataControl.FontStyle;
            //styleFont.FontStyle = hasFontStyle ? gridDataControl.FontStyle : styleFont.FontStyle;

            //var fontWeightPropertyMetadata = GridDataControl.FontWeightProperty.GetMetadata(typeof(GridDataControl));
            //var hasFontWeight = (FontWeight)fontWeightPropertyMetadata.DefaultValue != gridDataControl.FontWeight;
            //styleFont.FontWeight = hasFontWeight ? gridDataControl.FontWeight : styleFont.FontWeight;
        }

#endif

#if !SILVERLIGHT
        private void ApplyColor(IGridDataVisualStyle value)
        {
            var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl == null)
            {
                this.TableStyle.Foreground = this.TableProperties.Model.GetValueForegroundBrush(value);
                this.TableStyle.Background = this.TableProperties.Model.GetValueBackgroundBrush(value);
                return;
            }

            if (!gridDataControl.hasBackgroundChanged)
            {
                /// To set the color on the empty space between the last record and scroll bar.
                this.TableProperties.IsInternalChange = true;
                gridDataControl.Background = this.TableProperties.Model.GetValueBackgroundBrush(value);
                this.TableProperties.IsInternalChange = false;
            }

            //if (!gridDataControl.hasForegroundChanged)
            //{
            //    /// To set the color on the empty space between the last record and scroll bar.
            //    gridDataControl.Foreground = this.TableProperties.Model.GetValueForegroundBrush(value);
            //    gridDataControl.hasForegroundChanged = false;
            //}

            if (!this.HeaderStyle.HasHeaderForeGround)
            {
                this.HeaderStyle.Foreground =  this.TableProperties.Model.GetHeaderForeground(value);
            }
            if (!this.HeaderStyle.HasHeaderBackGround)
            {
                this.HeaderStyle.Background =  this.TableProperties.Model.GetHeaderBackground(value);
            }
            if (!this.IndentColumnStyle.HasHeaderForeGround)
            {
                this.IndentColumnStyle.Foreground = this.TableProperties.Model.GetHeaderForeground(value);
            }
            if (!this.IndentColumnStyle.HasHeaderBackGround)
            {
                this.IndentColumnStyle.Background =  this.TableProperties.Model.GetRowHeaderBackground(value);
            }
            this.TableStyle.Foreground = gridDataControl.hasForegroundChanged ? gridDataControl.Foreground : this.TableProperties.Model.GetValueForegroundBrush(value);
            this.TableStyle.Background = gridDataControl.hasBackgroundChanged ? gridDataControl.Background : this.TableProperties.Model.GetValueBackgroundBrush(value);
        }
#endif

        public void ApplyFont()
        {
            var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
            if (gridDataControl == null)
            {
                return;
            }

            var fontFamilyPropertyMetadata = GridDataControl.FontFamilyProperty.GetMetadata(typeof(GridDataControl));
            var hasFontFamily = fontFamilyPropertyMetadata.DefaultValue != gridDataControl.FontFamily;

            if (hasFontFamily)
            {
                TableStyle.Font.FontFamily = gridDataControl.FontFamily;
                HeaderStyle.Font.FontFamily = gridDataControl.FontFamily;
                IndentColumnStyle.Font.FontFamily = gridDataControl.FontFamily;
            }

            var fontSizePropertyMetadata = GridDataControl.FontSizeProperty.GetMetadata(typeof(GridDataControl));
            var hasFontSize = (double)fontSizePropertyMetadata.DefaultValue != gridDataControl.FontSize;

            if (hasFontSize)
            {
                TableStyle.Font.FontSize = gridDataControl.FontSize;
                HeaderStyle.Font.FontSize = gridDataControl.FontSize;
                IndentColumnStyle.Font.FontSize = gridDataControl.FontSize;
            }

            var fontStretchPropertyMetadata = GridDataControl.FontStretchProperty.GetMetadata(typeof(GridDataControl));
            var hasFontStretch = (FontStretch)fontStretchPropertyMetadata.DefaultValue != gridDataControl.FontStretch;

            if (hasFontStretch)
            {
                TableStyle.Font.FontStretch = gridDataControl.FontStretch;
                HeaderStyle.Font.FontStretch = gridDataControl.FontStretch;
                IndentColumnStyle.Font.FontStretch = gridDataControl.FontStretch;
            }

            var fontStylePropertyMetadata = GridDataControl.FontStyleProperty.GetMetadata(typeof(GridDataControl));
            var hasFontStyle = (FontStyle)fontStylePropertyMetadata.DefaultValue != gridDataControl.FontStyle;

            if (hasFontStyle)
            {
                TableStyle.Font.FontStyle = gridDataControl.FontStyle;
                HeaderStyle.Font.FontStyle = gridDataControl.FontStyle;
                IndentColumnStyle.Font.FontStyle = gridDataControl.FontStyle;
            }

            var fontWeightPropertyMetadata = GridDataControl.FontWeightProperty.GetMetadata(typeof(GridDataControl));
            var hasFontWeight = (FontWeight)fontWeightPropertyMetadata.DefaultValue != gridDataControl.FontWeight;

            if (hasFontWeight)
            {
                TableStyle.Font.FontWeight = gridDataControl.FontWeight;
                HeaderStyle.Font.FontWeight = gridDataControl.FontWeight;
                IndentColumnStyle.Font.FontWeight = gridDataControl.FontWeight;
            }


            if (this.Grid != null)
            {
                //// this.Grid.HighlightBrush = value.HighlightBrush;
                if (this.Table.NeedsInvalidate)
                {
                    ////this.Grid.InvalidateCells();
                    this.InvalidateDisplay();
                }
            }
        }


        /// <summary>
        /// Resizes a range of columns to optimally fit contents of the
        /// specified range of cells and given options.
        /// </summary>
        /// <param name="range">The range of cells to be analyzed.</param>
        /// <param name="options">Specifies whether row or column headers should be included; if size can be reduced and if covered cells should be considered.</param>
        /// <returns>True if any changes were made; False if all sizes were already optimal.</returns>
        public bool ResizeDataColumnsToFit(GridRangeInfo range, GridResizeToFitOptions options)
        {
            if (range.IsEmpty)
                return false;
            //this.Options.ColumnSizer;
            bool includeCellsWithinCoveredRange = (options & GridResizeToFitOptions.IncludeCellsWithinCoveredRange) != 0;
            bool resizeCoveredCells = (options & GridResizeToFitOptions.ResizeCoveredCells) != 0 || includeCellsWithinCoveredRange;
            bool noShrinkSize = (options & GridResizeToFitOptions.NoShrinkSize) != 0;
            bool includeHeaders = (options & GridResizeToFitOptions.IncludeHeaders) != 0;
            bool includeHiddenRows = (options & GridResizeToFitOptions.IncludeHiddenCells) != 0;
            range = range.ExpandRange(0, 0, this.RowCount, this.ColumnCount);
            //Model.FloatingCells.EvaluateFloatingCells(range);

            //switch()
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
                            //  CoveredCells.GetCellSpan
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

                                    GridDataTableStyleInfoIdentity tableCellIdentity = (GridDataTableStyleInfoIdentity)styleInfo.CellIdentity;

                                    if (tableCellIdentity.TableCellType == GridDataTableCellType.RecordCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell)
                                    {

                                        size = cellModel.CalculatePreferredCellSize(coveredRange.Top, coveredRange.Left, styleInfo, GridQueryBounds.Width);

                                        // Subtract col heights of previous cols (only the last col can be resized).
                                        if (colIndex > coveredRange.Left)
                                            size.Width -= (int)LineSizeUtil.GetTotal(this.ColumnWidths, coveredRange.Left, colIndex - 1);

                                        width = size.Width + 2;
                                    }
                                }
                                else
                                {
                                    styleInfo = this[rowIndex, colIndex];
                                    GridCellModelBase cellModel = styleInfo.CellModel;
                                    //cellModel.LoadStyle(colIndex, rowIndex, styleInfo);
                                    GridDataTableStyleInfoIdentity tableCellIdentity = (GridDataTableStyleInfoIdentity)styleInfo.CellIdentity;

                                    if (tableCellIdentity.TableCellType == GridDataTableCellType.RecordCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell
                                        || tableCellIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell)
                                    {

                                        size = cellModel.CalculatePreferredCellSize(rowIndex, colIndex, styleInfo, GridQueryBounds.Width);
                                        width = size.Width + 2;
                                    }
                                }
                            }
                            else
                                width = this.ColumnWidths[colIndex];

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
                    /*for (int i = 0;i < this.TableProperties.GroupedColumns.Count;i++)
                    {
                        newWidths[i] = GridDataTableModel.ExpandCollapseCellWidth;
                    }

                    if (this.Table.HasNestedTables)
                    {
                        newWidths[0] = GridDataTableModel.ExpandCollapseCellWidth;
                    }*/

                    if (!equal)
                    {
                        LineSizeUtil.SetRange(ColumnWidths, range.Left, range.Right, newWidths);
                        for (int colIndex = range.Left; colIndex <= range.Right; colIndex++)
                        {
                            var visiblecolindex = this.ResolvePositionToVisibleColumnIndex(colIndex);
                            if (visiblecolindex >= 0 && visiblecolindex < this.TableProperties.VisibleColumns.Count)
                                this.TableProperties.VisibleColumns[visiblecolindex].Width = new GridDataControlLength(newWidths[colIndex - range.Left]);
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

        internal bool inVisualstylechange = false;
     
        private void ApplyGridVisualStyle(IGridDataVisualStyle previousStyle,IGridDataVisualStyle value)
        {
            inVisualstylechange = true;
            if (value != null)
            {
#if !SILVERLIGHT
                if (!this.HeaderStyle.HasHeaderBackGround)
#endif
                    this.HeaderStyle.Background = GetHeaderBackground(value);
#if !SILVERLIGHT
                if (!this.IndentColumnStyle.HasHeaderBackGround)
#endif
                    this.IndentColumnStyle.Background = GetRowHeaderBackground(value);
#if !SILVERLIGHT
                if (!this.HeaderStyle.HasHeaderForeGround)
#endif
                this.HeaderStyle.Foreground = this.IndentColumnStyle.Foreground = GetHeaderForeground(value);
                this.HeaderStyle.Borders = GetHeaderCellBorders(value);
                this.IndentColumnStyle.Borders.Top = GetHeaderCellBorders(value).Top;
                this.IndentColumnStyle.Borders.Bottom = GetHeaderCellBorders(value).Bottom;
                this.HeaderStyle.Font = this.IndentColumnStyle.Font = GetHeaderFont(value);
                this.ApplyFont(this.HeaderStyle.Font);
                this.HeaderStyle.TextMargins = this.IndentColumnStyle.TextMargins = GetHeaderTextMargins(value);

#if !SILVERLIGHT
                this.ApplyBorders(GetValueCellBorders(value));
                if (this.TableStyle.Borders.Left != null) this.TableStyle.BorderMargins.Left = this.TableStyle.Borders.Left.Thickness;
                if (this.TableStyle.Borders.Top != null) this.TableStyle.BorderMargins.Top = this.TableStyle.Borders.Top.Thickness;
                if (this.TableStyle.Borders.Right != null) this.TableStyle.BorderMargins.Right = this.TableStyle.Borders.Right.Thickness;
                if (this.TableStyle.Borders.Bottom != null) this.TableStyle.BorderMargins.Bottom = this.TableStyle.Borders.Bottom.Thickness; 
#else
                this.TableStyle.Borders = GetValueCellBorders(value);
#endif
                this.TableStyle.Font = GetValueFont(value);
                
                this.ApplyFont(this.TableStyle.Font);
#if !SILVERLIGHT
                this.ApplyColor(value);                
#else
                this.TableStyle.Foreground = this.TableProperties.Model.GetValueForegroundBrush(value);
                this.TableStyle.Background = this.TableProperties.Model.GetValueBackgroundBrush(value);

                var gridDataControl = this.Grid.FindParentElementOfType<GridDataControl>();
                if (gridDataControl != null)
                {
                    gridDataControl.Background = this.TableProperties.Model.GetValueBackgroundBrush(value);
                }
#endif
                    this.TableProperties.IsInternalChange = true;
                    if (!this.TableProperties.IsRowBackgroundChangedExternally)
                        this.TableProperties.RowBackground = GetRowBackground(value);
                    if (!this.TableProperties.IsAlternatingRowBackgroundChangedExternally)
                        this.TableProperties.AlternatingRowBackground = value.AlternateRowBackground;
                    this.TableProperties.IsInternalChange = false;

                this.TableStyle.TextMargins = GetValueTextMargins(value);
                this.Options.CurrentCellBorder = GetCurrentCellBorderBrush(value);
                this.Options.CurrentCellBorderWidth = GetCurrentCellBorderWidth(value);

#if SILVERLIGHT
                this.Options.HighlightSelectionBackground = GetHighlightSelectionBackground(value);
                this.Options.HighlightSelectionForeground = GetHighlightSelectionForeground(value);
#else
                this.Options.HighlightSelectionBackground = (this.Options.highlightSelectionBackgroundChanged) ?this.Options.HighlightSelectionBackground :GetHighlightSelectionBackground(value);
                this.Options.HighlightSelectionForeground = (this.Options.highlightSelectionForegroundChanged)?this.Options.HighlightSelectionForeground:GetHighlightSelectionForeground(value);
#endif
                this.Options.HighlightSelectionAlphaBlend = (this.Options.HighlightSelectionAlphaBlend == GridControlConstants.HighlightSelectionAlphaBlend) ? GetHighlightBrush() : this.Options.HighlightSelectionAlphaBlend;
                this.TableProperties.DragIndicatorInnerBrush = this.TableProperties.DragIndicatorInnerBrush == null ? GetDragDropIndicatorBrush(value) : this.TableProperties.DragIndicatorInnerBrush;
                this.tableProperties.DragIndicatorOuterBrush = this.TableProperties.DragIndicatorOuterBrush == null ? GetDragDropIndicatorOuterBrush(value) : this.TableProperties.DragIndicatorOuterBrush;
            }

            if (this.Grid != null)
            {
                var gdc = this.Grid.FindParentElementOfType<GridDataControl>();
#if !SILVERLIGHT
                if (gdc != null && this.TableProperties.EnableVisualStyleForEditors)
                    GetVisualStyleDictionary(gdc);
#else
                GetVisualStyleDictionary(gdc);
#endif
                this.InvalidateDisplay();
            }
            inVisualstylechange = false;
        }

        internal void ApplyGridVisualStyle()
        {
            this.ApplyGridVisualStyle(null, GridVisualStyle);
        }

#if !SILVERLIGHT
        private void ApplyBorders(CellBordersInfo cellBordersInfo)
        {
            var temp = new CellBordersInfo();
            temp.CopyFrom(this.TableStyle.Borders);

            this.TableStyle.Borders = cellBordersInfo;

            if (this.TableStyle.IsLeftBorderChanged)
            {
                this.TableStyle.Borders.Left = temp.Left;
            }

            if (this.TableStyle.IsTopBorderChanged)
            {
                this.TableStyle.Borders.Top = temp.Top;
            }

            if (this.TableStyle.IsRightBorderChanged)
            {
                this.TableStyle.Borders.Right = temp.Right;
            }

            if (this.TableStyle.IsBottomBorderChanged)
            {
                this.TableStyle.Borders.Bottom = temp.Bottom;
            }
        }
#endif
        public bool IsInSourceListChanged
        {
            get;
            internal set;
        }

        protected override void OnRowsInserted(GridRangeInsertedEventArgs e)
        {
            base.OnRowsInserted(e);

            if (this.IsInSourceListChanged)
            {
                ////this.Table.SuspendRecordsEnumerator();
                ////var list = this.GetUnderlyingList();
                ////if (list != null)
                ////{
                this.RefreshSourceListCount();
                bool flag = true;
                if (this.Table.HasGroups)
                {
                    flag = e.InsertAt < this.Table.Model.View.TopLevelGroup.DisplayElements.Count;
                    flag = flag ? !(this.Table.Model.View.TopLevelGroup.DisplayElements[e.InsertAt] is Group) : flag;//checks the inserted row is group or not.
                }
                if ((this.Table.HasNestedTables||this.Table.HasDetailsView) && flag) //SD17193 New detailsview row is added, it should be set hidden
                {
                    var lineSizeCollection = (LineSizeCollection)this.RowHeights;
                    //WPF-10454 Grid gets collapsed while adding new records
                    lineSizeCollection.SetHidden(e.InsertAt + 1, e.InsertAt +e.Count-1, true);
                }
                //}
                ////var rowIndex = this.ResolveIndexToPosition(e.InsertAt);
                ////this.InsertRowAt(rowIndex, e.InsertAt);
                this.CurrencyManager.InsertRows(e.InsertAt, e.Count, null);
                this.InvalidateCell(GridRangeInfo.Row(e.InsertAt));
                this.InvalidateCell(GridRangeInfo.Row(e.InsertAt - 1));
                //}
                ////}

                ////this.Table.ResumeRecordsEnumerator();
            }
        }

        internal void RefreshSourceListCount()
        {
            this.RefreshSourceListCountMethod();
        }

        protected void RefreshSourceListCountMethod()
        {
            if (this.View != null)
                this.SourceListCount = this.View.Records.Count;
        }

        public class GridDataMoveCellsState : GridMoveCellsState
        {
            private GridDataCurrentRecordMoveState currentRecordMoveState = null;

#if !SILVERLIGHT
            public GridDataMoveCellsState(IEditableLineSizeHost lineSizes, GridVolatileCellStyles volatileCellStyles)
                : base(lineSizes, volatileCellStyles)
#else
            public GridDataMoveCellsState(IEditableLineSizeHost lineSizes)
                : base(lineSizes)
#endif
            {
                this.currentRecordMoveState = new GridDataCurrentRecordMoveState();
            }

            public GridDataCurrentRecordMoveState CurrentRecordMoveState
            {
                get { return this.currentRecordMoveState; }
            }
        }

        protected override GridMoveCellsState CreateGridMoveCellsState(IEditableLineSizeHost lineSizes)
        {
#if !SILVERLIGHT
            return new GridDataMoveCellsState(lineSizes, CreateVolatileCellStyles());
#else
            return new GridDataMoveCellsState(lineSizes);
#endif
        }

        protected override void RemoveRowsCore(int removeAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (this.IsInSourceListChanged)
            {
                GridDataCurrentRecordMoveState currentRecordMoveState = null;
                if (moveCellsState is GridDataMoveCellsState)
                {
                    currentRecordMoveState = ((GridDataMoveCellsState)moveCellsState).CurrentRecordMoveState;
                    this.CurrencyManager.RemoveRows(removeAtRowIndex, count, currentRecordMoveState);
                }
            }

            base.RemoveRowsCore(removeAtRowIndex, count, moveCellsState);
        }

        protected override void InsertRowsCore(int insertAtRowIndex, int count, GridMoveCellsState moveCellsState)
        {
            if (this.IsInSourceListChanged)
            {
                GridDataCurrentRecordMoveState currentRecordMoveState = null;
                if (moveCellsState is GridDataMoveCellsState)
                {
                    currentRecordMoveState = ((GridDataMoveCellsState)moveCellsState).CurrentRecordMoveState;
                    this.CurrencyManager.InsertRows(insertAtRowIndex, count, currentRecordMoveState);
                }

                this.InvalidateCell(GridRangeInfo.Row(insertAtRowIndex));
                this.InvalidateCell(GridRangeInfo.Row(insertAtRowIndex - 1));
            }
          //  if (this.TableProperties != null && !this[insertAtRowIndex, insertAtRowIndex].IsRowHidden)
                base.InsertRowsCore(insertAtRowIndex, count, moveCellsState);                
        }

        protected override void OnRowsRemoved(GridRangeRemovedEventArgs e)
        {
            base.OnRowsRemoved(e);

            if (this.IsInSourceListChanged)
            {
                ////this.Table.SuspendRecordsEnumerator();
                ////var list = this.GetUnderlyingList();
                ////if (list != null)
                ////{
                //// we simply update the underlying source list count
                this.RefreshSourceListCount();
                ////var rowIndex = this.ResolveIndexToPosition(e.RemoveAt);
                ////this.RemoveRowAt(rowIndex);
                this.CurrencyManager.RemoveRows(e.RemoveAt, e.Count, null);
                // this.InvalidateVisual(true);
                ////}
                ////this.Table.ResumeRecordsEnumerator();
            }
        }

        public bool IsSourceListReset
        {
            get;
            protected set;
        }

        private void OnSourceListItemMoved(ListChangedArgs e)
        {
            if (this.View.CurrentEditItem != null && this.View.CurrentEditItem.Equals(e.OldItems[0]))
                this.CancelEdit(false);
            var oldRowIndex = this.ResolvePositionToIndex(e.OldIndex);
            var newRowIndex = this.ResolvePositionToIndex(e.NewIndex);
            for (int i = 0; i < e.NewItems.Count; i++)
            {
                bool isGroup = e.NewItems[i] is Group;
                int itemCount = this.Table.HasNestedTables && !isGroup ? this.TableProperties.Relations.Count + 1 : 1;
                itemCount += this.Table.HasDetailsView && !isGroup ? 1 : 0;
                this.MoveRows(oldRowIndex, itemCount, newRowIndex);
            }
        }
        

        private void OnSourceListItemDeleted(ListChangedArgs e)
        {
            int count = 0;
            if (e.OldIndex < 0)
            {
                return;
            }
            count = e.OldItems.Count;
            this.IsInDeteteRecord = true;
            if (this.View.CurrentEditItem!=null && this.View.CurrentEditItem.Equals(e.OldItems[0]))
                this.CancelEdit(true);
            if (count > 0)
            {
#if SILVERLIGHT
                IList items = e.OldItems[0] as IList;
                if (items != null && items.Count > 0)
                {
                    count = items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        bool isGroup = items[i] is Group;
                        this.RemoveRow(e.OldIndex, isGroup);
                    }
                }
                else
                {
#endif
                    for (int i = 0; i < count; i++)
                    {
                        bool isGroup = e.OldItems[i] is Group;
                        this.RemoveRow(e.OldIndex, isGroup);
                    }
#if SILVERLIGHT
                }
#endif
            }
            else
            {
                this.RemoveRow(e.OldIndex, false);
            }
            this.IsInDeteteRecord = false;
        }

        internal void RemoveRow(int removeAt, bool isGroup)
        {
            var rowIndex = this.ResolvePositionToIndex(removeAt);
            //Adjusting the row count based on the relation count and details view existance to
            //remove the hidden rows.
            int itemCount = this.Table.HasNestedTables && !isGroup ? this.TableProperties.Relations.Count + 1 : 1;
            itemCount += this.Table.HasDetailsView && !isGroup ? 1 : 0;
            this.RemoveRowsProxy(rowIndex, itemCount);
        }

        internal void RemoveRowsProxy(int removeAtRowIndex, int count)
        {
#if !SILVERLIGHT
            var moveCellsState = new GridMoveCellsState(true, null, CreateVolatileCellStyles());
#else
            var moveCellsState = new GridMoveCellsState(true, null);
#endif
            if (this.CurrencyManager.IsInEndEdit)
            {
                moveCellsState.SuspendSelections = true;
            }

            RemoveRowsCore(removeAtRowIndex, count, moveCellsState);
            var e = new GridRangeRemovedEventArgs(removeAtRowIndex, count);
            OnRowsRemoved(e);
        }

        private void CancelEdit(bool isDiscardDeactivate)
        {
            if (this.CurrencyManager.IsEditing && this.CurrencyManager.UpdateMode != UpdateMode.PropertyChanged)
            {
                var currentCell = this.CurrencyManager.CurrentCell;
                if (currentCell != null && !currentCell.IsInConfirmChanges)
                {
                    ////currentCell.RejectChanges();
                    if(!isDiscardDeactivate)
                        currentCell.Deactivate();
                    if (!this.CurrencyManager.IsInEndEdit)
                    {
                        this.CurrencyManager.CancelEdit();
                    }
                }
            }
        }

        private void OnSourceListItemAdded(ListChangedArgs e)
        {
            if (!this.CurrencyManager.IsInDeactivate && !this.CurrencyManager.isLastRowCol
               && this.CurrencyManager.CurrentCell.MoveFromRowIndex != this.ResolveAddNewPositionInGrid() && this.View.CurrentEditItem != null && this.View.CurrentEditItem.Equals(e.NewItems[0]))
            {
                this.CancelEdit(false);
            }

            var rowIndex=0;
            if (this.Table.HasGroups)
            {
                rowIndex = this.ResolveGroupPositionToIndex(e.NewIndex);
            }
            else
                rowIndex = this.ResolvePositionToIndex(e.NewIndex);

#if SILVERLIGHT
            var items = e.NewItems[0] as IList;
            if (items != null && items.Count > 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    bool isGroup = items[i] is Group;
                    int itemCount = this.Table.HasNestedTables && !isGroup
                                        ? this.TableProperties.Relations.Count + 1
                                        : 1;
                    itemCount += this.Table.HasDetailsView && !isGroup ? 1 : 0;
                    this.InsertRowsProxy(rowIndex, itemCount);
                }
            }
            else
            {
#endif
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    bool isGroup = e.NewItems[i] is Group;
                    int itemCount = this.Table.HasNestedTables && !isGroup
                                        ? this.TableProperties.Relations.Count + 1
                                        : 1;
                    itemCount += this.Table.HasDetailsView && !isGroup ? 1 : 0;
                    this.InsertRowsProxy(rowIndex, itemCount);
                }
#if SILVERLIGHT
            }
#endif
            //If we end the editing by pressing any keys then Key action should be after the Sorting, For this purpose this code was added here..
                if (this.Grid.CurrentCell.IsInDeactivated && !this.Grid.CurrentCell.ActivateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
                {
                    if (!this.Grid.CurrentCell.IsInMoveTo)
                        this.Grid.CurrentCell.MoveTo(rowIndex, this.Grid.CurrentCell.MoveToColumnIndex);
                }
        }

        internal void InsertRowsProxy(int insertAtRowIndex, int count)
        {
#if !SILVERLIGHT
            var moveCellsState = new GridMoveCellsState(true, null, CreateVolatileCellStyles());
#else
            var moveCellsState = new GridMoveCellsState(true, null);
#endif
            if (this.CurrencyManager.IsInEndEdit)
            {
                moveCellsState.SuspendSelections = true;
            }

            InsertRowsCore(insertAtRowIndex, count, moveCellsState);
            var e = new GridRangeInsertedEventArgs(insertAtRowIndex, count);
            OnRowsInserted(e);
            var args = new GridSelectionChangedEventArgs(GridRangeInfo.Empty, null, GridSelectionReason.Clear);
            this.RaiseSelectionChanged(args);
        }

        /// <summary>
        /// Creates the volatile cell styles.
        /// </summary>
        /// <returns></returns>
        protected override GridVolatileCellStyles CreateVolatileCellStyles()
        {
            return new GridDataVolatileCellStyles(this);
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.IsDisposing = true;
            this.UnwireGrid();
            if(this.View!=null)
            this.UnwireView(this.View);
            base.Dispose(disposing);
            if (disposing)
            {
                emptyModel = null;
                if (this.Table != null)
                {
                    this.Table.Dispose();
                    this.gridDataTable = null;
                }
                if (this.TableProperties != null)
                {
                    this.tableProperties.Dispose();
                    this.tableProperties = null;
                }
                if (this.ChildTableModelCollection != null)
                {
                    this.ChildTableModelCollection.Clear();
                    this.ChildTableModelCollection = null;
                }
                if (this.DetailsViewRows != null)
                {
                    this.DetailsViewRows.Clear();
                    this.DetailsViewRows = null;
                }
                if (this.View != null)
                    this.View.Dispose();
#if !SILVERLIGHT
                if (this.boundFunc != null)
                {
                    this.boundFunc = null;
                    if (this.CellModels != null)
                    {
                        this.CellModels.Clear();
                        this.CellModels.Dispose();
                    }
                }
#endif
                if (this.ColumnAutoSizer != null)
                {
                    this.ColumnAutoSizer.Dispose();
                   
                }
#if !SILVERLIGHT
                if (this.ContextMenuEventArgs != null)
                {
                    this.contextMenuEventArgs = null;
                }
#endif
                if (this.CurrencyManager != null)
                {
                    this.CurrencyManager.CurrentRecordSelectionChanged -= this.OnCurrentRecordSelectionChanged;
                    this.CurrencyManager.Dispose();
                    this.CurrencyManager = null;
                }               
                this.CurrentCellState.Dispose();
                if (this.EmptyColumnStyle != null)
                {
#if !SILVERLIGHT
                    this.EmptyColumnStyle.Dispose(true);
#else
                    this.EmptyColumnStyle.Dispose();
#endif

                    this.EmptyColumnStyle = null;
                }
                if (this.gridVisualStyle != null)
                    this.gridVisualStyle = null;
                if (this.indentColumnStyle != null)
                {
#if !SILVERLIGHT
                    this.indentColumnStyle.Dispose(true);
#else
                    this.indentColumnStyle.Dispose();
#endif
                    this.indentColumnStyle=null;
                }
                if (this.sourceList != null)
                    this.sourceList = null;
                if (sourceListsKey!=null)
                {
                    sourceListsKey.Clear();
                    //sourceListsKey = null;
                }

                //var collection = this.View as CollectionViewAdv;
                //if (collection != null)
                //{
                //    if (collection.SortDescriptions!=null)
                //    {
                //        collection.SortDescriptions.Clear();
                //    }
                //    if (collection.SummaryRows != null)
                //        collection.SummaryRows.Clear();
                //    if (collection.TableSummaryRows != null)
                //        collection.TableSummaryRows.Clear();
                //    if (collection.Records != null)
                //    {
                //        collection.Records.Clear();
                //        collection.UnwireEvents();
                //    }
                //    collection = null;
                //}

                if (this.grid != null)
                {
                    if (this.grid.Model.Equals(this))
                        this.grid.Model = null;
                    if (canDisposeGrid)
                    {
#if !SILVERLIGHT
                        this.grid.Dispose(true);
#else
                    this.grid.Dispose();
#endif
                    }
                    this.grid = null;
                }
            }
            this.IsDisposing = false;
        }
        
        /// <summary>
        /// Ensures the object are initialized.
        /// </summary>
        protected virtual void EnsureInitialized()
        {
            if (this.IsInitialized)
            {
                return;
            }

            //// set this flag after the SourceList is re-init, since the flag would be reset again
            this.IsInitialized = true;
            if (this.SourceList != null)
            {
                //// init all the related objects here
                this.InitializeTable();
                this.Table.SetDirty();
                this.RefreshDisplay(true);
                this.RefreshColumns(true, true);
                this.IsLoaded = true;
                if (this.Initialized != null)
                {
                    this.Initialized(this, EventArgs.Empty);
                }
            }
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public event EventHandler Initialized;

        private int lastAddNewIndex = -1 ;
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Grid != null)

                if (!this.Grid.CheckAccess())
                {
#if !SILVERLIGHT
                    this.Grid.Dispatcher.Invoke(new NotifyCollectionChangedEventHandler(OnCollectionChanged), new object[] { sender, e });
#else
                this.Grid.Dispatcher.BeginInvoke(new NotifyCollectionChangedEventHandler(OnCollectionChanged), new object[] { sender, e });
#endif
                    return;
                }

            if (this.IsInSuspend && !(this is GridDataChildTableModel))
            {
                return;
            }

            this.IsInSourceListChanged = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Replace:
                    this.RefreshSourceListCount();
                    if (this.CurrencyManager.UpdateMode != UpdateMode.PropertyChanged && this.Grid.CurrentCell.IsEditing)
                    {
                        if (this.Grid.CurrentCell.RowIndex >= this.ResolveStartIndexBasedOnPosition())
                            this.Grid.CurrentCell.EndEdit();

                        this.Data.Clear();
                        // This is commented since data contains the infomation needs to be changed such error icon alignment etc.. 
                    }
#if !SILVERLIGHT
                    if (!this.TableProperties.EnableOptimizations)
#endif
                    {
                        // var recordIndex = e.NewStartingIndex; Unused local variable
                        //Console.WriteLine("Row Index : " + e.NewStartingIndex);
                        this.InvalidateCell(GridRangeInfo.Row(this.ResolvePositionToIndex(e.NewStartingIndex)));
                        //The below if condition is added due to 
                        // When two grids are given with same itemsource, if we update value in the first grid it reflects in the second grid and if we
                        // change the same cell in second grid the values not get updated in the first grid. 
                        if (this.CurrencyManager.IsInEndEdit)
                        {
                            this.Data.Clear();
                        }
                    }
#if SILVERLIGHT
                    this.InvalidateCell(GridRangeInfo.Row(this.ResolvePositionToIndex(e.NewStartingIndex)));
                    this.InvalidateVisual();
#endif
                    break;

                case NotifyCollectionChangedAction.Add:
                    this.lastAddNewIndex = -1;
                    this.OnSourceListItemAdded(new ListChangedArgs()
                        {
                            NewIndex = e.NewStartingIndex,
                            NewItems = e.NewItems
                        });
                    this.RefreshSourceListCount();
                    this.lastAddNewIndex = e.NewStartingIndex;
                    RefreshChildRecord();
                    if (this.Table.HasGroups && !this.CurrencyManager.IsEditing)
                    {
                        RefreshGroups(e.NewItems);
                    }
                    else if (this.Table.HasFilters && this.TableProperties.RowBackground != null ||
                             this.TableProperties.AlternatingRowBackground != null && !this.Table.HasGroups)
                    {
#if !SILVERLIGHT
                        this.Grid.needRenderStyleBackgrounds = true;
                        this.Grid.Dispatcher.BeginInvoke(new Action(() =>
                        {
#endif
                            this.InvalidateCell(GridRangeInfo.Table());
#if !SILVERLIGHT
                        }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
#endif
                    }
                    this.RefreshGroupRowsWhenAlternateBackgroundSet(e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.lastAddNewIndex = -1;
                    this.OnSourceListItemDeleted(new ListChangedArgs()
                        {
                            OldIndex = e.OldStartingIndex,
                            OldItems = e.OldItems
                        });
                    this.RefreshSourceListCount();
                    RefreshChildRecord();
                    if (this.Table.HasGroups)
                    {
                        this.RefreshGroupRowsWhenAlternateBackgroundSet(e.OldItems);
                    }
                        //This will refresh the AlternativeForeground and Background for the VisibleRows when delete the Record
                    else if (this.Table.HasFilters && this.TableProperties.RowBackground != null ||
                             this.TableProperties.AlternatingRowBackground != null && !this.Table.HasGroups)
                    {
                        this.InvalidateCell(GridRangeInfo.Table());
                    }
                    break;

#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    if (this.lastAddNewIndex != e.NewStartingIndex)
                    {
                        this.OnSourceListItemMoved(new ListChangedArgs()
                            {
                                NewIndex = e.NewStartingIndex,
                                OldIndex = e.OldStartingIndex,
                                NewItems = e.NewItems,
                                OldItems = e.OldItems
                            });
                        this.RefreshSourceListCount();
                    }
                    this.lastAddNewIndex = -1;
                    break;

#endif
                case NotifyCollectionChangedAction.Reset:
                    if (this.CurrencyManager.IsEditing)
                    {
                        this.CurrencyManager.CancelEdit();
                    }

                    this.RefreshSourceListCount();
                    if (this.SourceListCount == 0 && !this.IsInFilter)
                    {
                        //Need to clear Selections while Reset the ItemsSource.
                        this.Selections.Clear();
                        var gdc = this.Grid.FindParentElementOfType<GridDataControl>();
                        if (gdc != null && !gdc.Model.Table.HasNestedTables)
                        {
                            gdc.SelectedItems.Clear();
                            gdc.SelectedItem = null;
                        }
                    }

                    this.IsSourceListReset = true;
                    this.WireSourceList(false);
                    if (this.Table.HasGroups)
                    {
                        if (this.Table.HasNestedTables)
                        {
                            // we simply clear the previous hidden states
                            var lineSizeCollection = this.RowHeights as LineSizeCollection;
                            lineSizeCollection.ResetHiddenState();

                            foreach (var g in this.View.TopLevelGroup.DisplayElements)
                            {
                                if (g is Group)
                                {
                                    var group = g as Group;
                                    if (group.IsBottomLevel)
                                    {
                                        var startIdx = this.ResolveStartIndexOfGroup(group) + 1;
                                        var endIdx = startIdx + group.GetRecordCount();
                                        if (group.IsExpanded)
                                        {
                                            lineSizeCollection.SetHiddenIntervalWithState(startIdx, endIdx,
                                                                                          this.Table
                                                                                              .ResolveHiddenPattern());
                                        }
                                        int start = startIdx + 1;
                                        foreach (var rec in group.Records)
                                        {
                                            if (rec.IsExpanded)
                                            {
                                                lineSizeCollection.ResetNestedLines(start);
                                            }
                                            start += 2;
                                        }
                                    }
                                }
                            }
                        }
                        var hasFilters = this.TableProperties.VisibleColumns.FirstOrDefault(v => v.Filters != null) !=
                                         null;
                        // when group + filter is done, we need to invalidate all the columns
                        if (hasFilters && !this.TableProperties.isGroupApplied)
                        {
                            var startIdx = this.ResolveStartIndexBasedOnPosition();
                            var range = GridRangeInfo.Rows(startIdx, this.RowCount);
                            this.InvalidateCell(range);
                            this.VolatileCellStyles.Clear(range.ToCellSpan());
                            //this.Data.Clear();
                        }
                        else
                        {
                            if (this.TableProperties.isGroupApplied)
                            {
                                this.TableProperties.isGroupApplied = false;
                            }
                            this.InvalidateDisplay();
                        }

                        this.IsInSourceListChanged = false;
                        this.RefreshColumns(true, true);
                        this.IsInSourceListChanged = true;
                    }

                    //Invalidate the header cells when refresh is set
                    this.ColumnAutoSizer.InvalidateCells(GridRangeInfo.Rows(0, this.HeaderRows));
                    if (!this.Table.HasGroups)
                    {
#if !SILVERLIGHT
                        // If CurrentCell is in FilterBarCell Filter values are cleared while call InvalidateVisual. So skip InvalidateVisual using IsInDropDownFilterCell
                        if (this.Grid != null && !this.IsInFilter &&
                            !this.CurrencyManager.CurrentCell.IsInDropDownFilterCell)
#else
                        if (!this.IsInFilter)
#endif
                        {
                            //this.RefreshColumns();
                            //While clearing the item source, the header cells are recreated again.
#if !SILVERLIGHT
                            unloadHeader = false;
                            this.InvalidateDisplay(true);
                            unloadHeader = true;
#else
                            this.InvalidateDisplay();
#endif

                        }
                    }

                    this.VolatileCellStyles.Clear();
                    this.Data.Clear();
                    this.IsSourceListReset = false;
                    break;
            }

            if (this.Table.HasTableSummaries)
            {
                this.InvalidateTableSummaryRow();
            }
            if (this.FooterRows > 0)
            {
                this.InvalidateCell(GridRangeInfo.Rows(this.RowCount - this.FooterRows, this.RowCount));
            }

            if (this is GridDataChildTableModel)
            {
                var model = this as GridDataChildTableModel;
                var parentModel = model.ParentTable.Model;
                if (model.SelectedChildModel != null)
                {
                    model.SelectedChildModel = null;
                }
                if (parentModel != null)
                    parentModel.InvalidateVisual(true);
            }
            else
            {
                var gdc = this.Grid.FindParentElementOfType<GridDataControl>();
                if (gdc != null)
                {
                    gdc.SelectedChildModel = null;
                }
            }

            this.IsInSourceListChanged = false;
        }

        private void RefreshChildRecord()
        {
            if (this is GridDataChildTableModel)
            {
                var childModel = this as GridDataChildTableModel;

                if (childModel.ParentRecord != null && childModel.ParentRecord.Table != null && childModel.ParentTable != null)
                {
                    var parentModel = childModel.ParentTable.Model;
                    bool isExpandable = parentModel.Table.ShouldExpand(childModel.ParentRecord);
                    int recordIndex = 0;
                    int ActualRecordIndex = parentModel.ResolvePositionToIndex(parentModel.View.Records.IndexOfRecord(childModel.ParentRecord.Data));                    
                    if (childModel.ParentRecord.IsExpanded)
                    {
                        int offset = childModel.ParentRecord.ChildModels.Keys.Where(i => childModel.ParentRecord.ChildModels[i].Equals(this)).SingleOrDefault();
                        offset = offset >= 0 ? offset + 1 : 0; // offset != null && Since int is never equals to null.
                        recordIndex =childModel.ParentTable.HasDetailsView? ActualRecordIndex + offset+1: ActualRecordIndex + offset;
                        if (recordIndex > 0 && recordIndex < parentModel.RowHeights.LineCount)
                        {
                            parentModel.RowHeights.SetHidden(recordIndex, recordIndex, true);
                            parentModel.RowHeights.SetHidden(recordIndex, recordIndex, false);
                            if (!isExpandable||(childModel.View.Records.Count == 0 && childModel.ParentTable.Model.TableProperties.HideEmptyChildGrid))
                            {
                                parentModel.RowHeights.SetHidden(recordIndex, recordIndex, true);                               
                            }                                                     
                        }
                    }
                    if (parentModel is GridDataChildTableModel)
                        RefreshParent(parentModel as GridDataChildTableModel);
                    else
                        parentModel.InvalidateVisual(true);
                   // Invalidate the ExpandCollapse Cell while ChildModel DataSource Count equal to 0
                    if (childModel.View.Records.Count == 0)
                    {
                        parentModel.InvalidateCell(GridRangeInfo.Cell(ActualRecordIndex, !this.Table.HasGroups ? (parentModel.TableProperties.ShowRowHeader ? 1 : 0) : parentModel.ResolveDefaultColumnOffset() - 1));
                    }                 
                    
                }
            }
        }
       
        private void RefreshParent(GridDataChildTableModel model)
        {
            if (model.ParentTable.Model is GridDataChildTableModel)            
                RefreshParent((GridDataChildTableModel)model.ParentTable.Model);                  
            else
                model.ParentTable.Model.InvalidateVisual(true);            
        }
        //Get called whenever a new row / group is added to the GDC during runtime.
        //This method will make changes in the UI part of the Grid. Changes in collection will be taken care by CollectionViewAdv.
        //If the new row is an Record Entry and its parent is in expanded state, just changing the parent's IsExpanded to false & true and vice versa
        //If the new row is an Group and it is in expanded state, then just calling the CollapseGroup() & ExpandGroup and vise versa.

        private void RefreshGroups(IList Items)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    if (item is RecordEntry)
                    {
                        var parent = ((RecordEntry)item).Parent as Group;
                        if (parent.IsExpanded)
                        {
                            parent.IsExpanded = false;
                            parent.IsExpanded = true;
                        }
                        //else
                        //{
                        //    parent.IsExpanded = true;
                        //    parent.IsExpanded = false;
                        //}
                    }
                    else if (item is Group)
                    {
                        foreach (var g in this.View.TopLevelGroup.Groups)
                        {
                            if (g.IsExpanded)
                            {
                                this.Table.CollapseGroup(g);
                                this.Table.ExpandGroup(g);
                            }
                            //else
                            //{
                            //    this.Table.ExpandGroup(g);
                            //    this.Table.CollapseGroup(g);
                            //}
                        }
                    }
                }
            }
        }


        private void RefreshGroupRowsWhenAlternateBackgroundSet(IList items)
        {
            if (this.Table.HasAlternateRowBackground || this.Table.HasAlternateRowForeground)
            {
                foreach (var entry in items)
                {
                    var recordEntry = entry as RecordEntry;
                    if (recordEntry != null)
                    {
                        var grp = recordEntry.Parent as Group;
                        if (grp != null)
                        {
                            var idx = this.ResolveStartIndexOfGroup(grp);
                            //var range = GridRangeInfo.Rows(idx, grp.Records.Count);
                            //Earlier the entire grid rows are invalidated.
                            //Now only the visible rows are invalidated.
                            var rowcoll = this.Grid.ScrollRows.GetVisibleLines();
                            for (int i = 0; i < rowcoll.Count - this.FooterRows; i++)
                            {
                                if (rowcoll[i].LineIndex >= idx)
                                {
                                    this.InvalidateCell(GridRangeInfo.Row(rowcoll[i].LineIndex));
                                }
                            }
                            //this.InvalidateCell(range);
#if SILVERLIGHT
                            this.InvalidateVisual(true);
#endif
                        }
                    }
                }
            }
        }

        private void ExpandCollapseRecord(int originalRowIdx, int originalColIdx)
        {
            //// Resolve row
            GridDataRecord record = null;
            int recordIndex = 0;
            if (this.Table.HasGroups)
            {
                recordIndex = this.ResolveIndexToGroupPosition(originalRowIdx);
                record = this.View.TopLevelGroup.DisplayElements[recordIndex] as GridDataRecord;
            }
            else
            {
                recordIndex = this.ResolveIndexToRecordPosition(originalRowIdx);
                record = this.View.Records[recordIndex] as GridDataRecord;
            }

            if (record == null)
                return;

            if (!record.IsExpanded)
            {
                //record.SetExpandedUI(recordIndex, originalRowIdx, originalColIdx);
                //this.ExpandedRecordCount++;
                record.IsExpanded = true;
            }
            else
            {
                //record.SetCollapsedUI(recordIndex, originalRowIdx, originalColIdx);
                if (this.ExpandedRecordCount > 0)
                    this.ExpandedRecordCount--;
                record.IsExpanded = false;
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Extracts the relational column.
        /// </summary>
        /// <param name="pd">PropertyDescriptor instance.</param>
        private void ExtractRelationalColumn(PropertyDescriptor pd)
        {
            ////sometimes we get a byte[] array for a column that has images
            var isNestedCollection = pd.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
&& !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive);

            // TODO - ForeignKeyRef has to be worked out for the next release.
            if (isNestedCollection)
            {
                var relation = new GridDataRelation()
                {
                    RelationalColumn = pd.Name,
                    RelationType = RelationType.MasterDetails
                    ////RelationType = isNestedCollection ? GridDataRelationType.MasterDetails : GridDataRelationType.ForeignKeyReference
                };
                this.TableProperties.Relations.Add(relation);
                this.View.Relations.Add((IRelationDefinition)relation);
            }
        }
#else
        private void ExtractRelationalColumn(PropertyInfo pd)
        {
            ////sometimes we get a byte[] array for a column that has images
            var isNestedCollection = pd.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
&& !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive);
            // TODO - ForeignKeyRef has to be worked out for the next release.
            if (isNestedCollection)
            {
                var relation = new GridDataRelation()
                {
                    RelationalColumn = pd.Name,
                    RelationType = RelationType.MasterDetails
                    ////RelationType = isNestedCollection ? GridDataRelationType.MasterDetails : GridDataRelationType.ForeignKeyReference
                };
                this.TableProperties.Relations.Add(relation);
                this.View.Relations.Add(relation);
            }
        }
#endif
        #region Summary

        public Dictionary<string, object> GetTableSummary(ISummaryRow summaryRow, ISummaryColumn summaryColumn)
        {
            var summaryRecordEntry = this.View.Records.TableSummaries.FirstOrDefault(o => o.SummaryRow == summaryRow);
            var index = summaryRecordEntry.SummaryRow.SummaryColumns.IndexOf(summaryColumn);
            return summaryRecordEntry.SummaryValues[index].AggregateValues;
        }



        public Dictionary<string, object> GetGroupSummary(Group group, ISummaryRow summaryRow, ISummaryColumn summaryColumn)
        {
            Dictionary<string, object> summaryValue = new Dictionary<string, object>();
            SummaryRecordEntry summaryRecordEntry;
            if (group.IsBottomLevel)
            {
                var groupRecordEntry = group.Details as GroupRecordEntry;
                summaryRecordEntry = groupRecordEntry.Summaries.FirstOrDefault(s => s.SummaryRow == summaryRow);
            }
            else
            {
                summaryRecordEntry = group.SummaryDetails;
            }
            if (summaryRecordEntry != null)
            {
                var summaryItems = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                summaryValue = summaryItems.AggregateValues;
            }
            return summaryValue;
        }

        public Dictionary<string, object> GetGroupCaptionSummary(Group group, ISummaryColumn summaryColumn)
        {
            Dictionary<string, object> summaryValue = new Dictionary<string, object>();
            SummaryRecordEntry summaryRecordEntry = group.SummaryDetails;
            if (summaryRecordEntry != null)
            {
                var summaryItems = summaryRecordEntry.SummaryValues.FirstOrDefault(s => s.Name == summaryColumn.Name);
                summaryValue = summaryItems.AggregateValues;
            }
            return summaryValue;
        }




        #region Check

        internal string GetFormattedTableSummaryForCaption(SummaryRecordEntry summaryRecordEntry)
        {
            return SummaryCreator.GetSummaryDisplayTextForRow(summaryRecordEntry, this.View);
        }
        #endregion

        #endregion

        public event GridFilterEventHandler FilterChanging;

        protected void OnFilterChanging(GridFilterEventArgs e)
        {
            if (this.FilterChanging != null)
            {
                this.FilterChanging(this, e);
            }
        }

        public event GridFilterEventHandler FilterChanged;

        protected void OnFilterChanged(GridFilterEventArgs e)
        {
            if (this.FilterChanged != null)
            {
                this.FilterChanged(this, e);
            }
        }
        /// <summary>
        /// Clears the filter column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <param name="filterType">Type of the filter.</param>
        /// <param name="predicateType">Type of the predicate.</param>
        /// <param name="isCaseSensitive">if set to <c>true</c> [is case sensitive].</param>
        /// <param name="canApplyFilter">if set to <c>true</c> [can apply filter].</param>
        internal void ClearFilterColumn(GridDataVisibleColumn column, object filterValue, FilterType filterType, PredicateType predicateType, bool isCaseSensitive, bool canApplyFilter)
        {
            if (column == null)
            {
                return;
            }
            try
            {
                this.RefreshFromFilter = true;
                this.IsInFilter = true;
                this.TableProperties.VisibleColumns.SuspendEvents();
                FilterPredicate fpredicate = null;
                if (filterValue != null)//||column.ColumnType==typeof(string))
                {
                    if (column.Filters.Count == 0)
                    {
                        fpredicate = new FilterPredicate() { FilterType = filterType, FilterValue = filterValue, PredicateType = predicateType, FilterBehavior = column.FilterBehavior, IsCaseSensitive = isCaseSensitive };
                        column.Filters.Add(fpredicate);
                    }
                    else
                    {
                        // take the last predicate
                        fpredicate = column.Filters[column.Filters.Count - 1];
                        fpredicate.FilterType = filterType;
                        fpredicate.FilterValue = filterValue;
                        fpredicate.PredicateType = predicateType;
                        fpredicate.IsCaseSensitive = isCaseSensitive;
                        fpredicate.FilterBehavior = column.FilterBehavior;
                    }
                }
                else
                {
                    if (column.Filters.Count > 0)
                        column.Filters.Clear();
                }

                var args = new GridFilterEventArgs(column, fpredicate);
                OnFilterChanging(args);
                if (canApplyFilter && !args.Handled)
                {
                    var filters = GetFilters();
                    this.View.FilterPredicates = filters;
                    this.RefreshDisplay(true);
                    var startIdx = this.ResolveStartIndexBasedOnPosition();
                    this.InvalidateCell(GridRangeInfo.Rows(startIdx, this.RowCount));
                }
                if (this.View.PagedSource != null && this.View.EnablePaging && this.View.Groups != null && this.View.Groups.Count > 0)
                {

                    this.View.PagedSource.Refresh();

                }
                this.RefreshFromFilter = false;
                this.RefreshSourceListCount();
                this.IsInFilter = false;
                this.UpdateSelectedRanges();
                this.TableProperties.VisibleColumns.ResumeEvents();
                OnFilterChanged(args);
            }
            catch { }
        }
        internal void FilterColumn(GridDataVisibleColumn column, object filterValue, FilterType filterType, PredicateType predicateType, bool isCaseSensitive, bool canApplyFilter)
        {
            this.FilterColumnMethod(column, filterValue, filterType, predicateType, isCaseSensitive, canApplyFilter);

        }

        protected virtual void FilterColumnMethod(GridDataVisibleColumn column, object filterValue, FilterType filterType, PredicateType predicateType, bool isCaseSensitive, bool canApplyFilter)
        {
            if (column == null)
            {
                return;
            }
            this.RefreshFromFilter = true;
            this.IsInFilter = true;
            this.TableProperties.VisibleColumns.SuspendEvents();
            FilterPredicate fpredicate = null;
            //if (column.Filters == null)
            //{
            //    column.Filters = new ObservableCollection<FilterPredicate>();
            //}
            if (filterValue != null)//||column.ColumnType==typeof(string))
            {
                //GridDataFilterPredicate predicate = null;
                if (column.Filters.Count == 0)
                {
                    //predicate = new GridDataFilterPredicate() { FilterValue = filterValue, FilterType = filterType, PredicateType = predicateType, IsCaseSensitive = isCaseSensitive };
                    fpredicate = new FilterPredicate() { FilterType = filterType, FilterValue = filterValue, PredicateType = predicateType, FilterBehavior = column.FilterBehavior, IsCaseSensitive = isCaseSensitive };
                    column.Filters.Add(fpredicate);
                }
                else
                {
                    // take the last predicate
                    fpredicate = column.Filters[column.Filters.Count - 1];
                    fpredicate.FilterType = filterType;
                    fpredicate.FilterValue = filterValue;
                    fpredicate.PredicateType = predicateType;
                    fpredicate.IsCaseSensitive = isCaseSensitive;
                    fpredicate.FilterBehavior = column.FilterBehavior;
                    //predicate = new GridDataFilterPredicate() { FilterType = fpredicate.FilterType, FilterValue = fpredicate.FilterValue, IsCaseSensitive = fpredicate.IsCaseSensitive, PredicateType = fpredicate.PredicateType };
                }
            }
            else
            {
                var excelfilter = (this.TableProperties.VisibleColumns.TableModel.View as IExcelLikeFilterExt);
                if (excelfilter.IsExcelLikeFilter)
                {
                    //GridDataFilterPredicate predicate = null;
                    if (column.Filters.Count == 0)
                    {
                        //predicate = new GridDataFilterPredicate() { FilterValue = filterValue, FilterType = filterType, PredicateType = predicateType, IsCaseSensitive = isCaseSensitive };
                        fpredicate = new FilterPredicate()
                            {
                                FilterType = filterType,
                                FilterValue = filterValue,
                                PredicateType = predicateType,
                                FilterBehavior = column.FilterBehavior,
                                IsCaseSensitive = isCaseSensitive
                            };
                        column.Filters.Add(fpredicate);
                    }
                    else
                    {
                        // take the last predicate
                        fpredicate = column.Filters[column.Filters.Count - 1];
                        fpredicate.FilterType = filterType;
                        fpredicate.FilterValue = filterValue;
                        fpredicate.PredicateType = predicateType;
                        fpredicate.IsCaseSensitive = isCaseSensitive;
                        fpredicate.FilterBehavior = column.FilterBehavior;
                        //predicate = new GridDataFilterPredicate() { FilterType = fpredicate.FilterType, FilterValue = fpredicate.FilterValue, IsCaseSensitive = fpredicate.IsCaseSensitive, PredicateType = fpredicate.PredicateType };
                    }
                }
                else
                {
                    if (column.Filters.Count > 0)
                        column.Filters.Clear();
                }
            }

            var args = new GridFilterEventArgs(column, fpredicate);
            OnFilterChanging(args);
            if (canApplyFilter && !args.Handled)
            {
                var filters = GetFilters();
                this.View.FilterPredicates = filters;
                this.RefreshDisplay(true);
                var startIdx = this.ResolveStartIndexBasedOnPosition();
                this.InvalidateCell(GridRangeInfo.Rows(startIdx, this.RowCount));
            }

            if (this.View.PagedSource != null && this.View.EnablePaging && this.View.Groups != null && this.View.Groups.Count > 0)
            {
                this.View.PagedSource.Refresh();
            }

            this.RefreshFromFilter = false;
            this.RefreshSourceListCount();
            this.IsInFilter = false;
            this.UpdateSelectedRanges();
            this.TableProperties.VisibleColumns.ResumeEvents();
            OnFilterChanged(args);
        }

        internal bool RefreshFromFilter
        {
            get;
            set;
        }

        internal  void FilterColumn(GridDataVisibleColumn column, List<FilterPredicate> filterPredicates, bool canApplyFilter)
        {
            FilterColumn(column, filterPredicates, canApplyFilter,false);
        }

        protected virtual void FilterColumn(GridDataVisibleColumn column, List<FilterPredicate> filterPredicates, bool canApplyFilter,bool isDomaingrid)        
        {
            if (column == null)
            {
                return;
            }

            if (filterPredicates == null)
            {
                this.View.FilterPredicates = GetFilters();
                return;
            }
            this.RefreshFromFilter = true;
            this.IsInFilter = true;
            this.TableProperties.VisibleColumns.SuspendEvents();
             var args = new GridFilterEventArgs(column, filterPredicates.LastOrDefault());
            OnFilterChanging(args);
            if (!args.Handled)
            {
                if (filterPredicates.Count() > 0)
                {
                    column.Filters.Clear();
                    foreach (var item in filterPredicates)
                    {
                        if (item != null)
                        {
                           
                                column.Filters.Add(item);
                        }
                    }
                }
                else
                {
                    if (column.Filters.Count > 0)
                    {
                        column.Filters.Clear();
                    }
                }
            }
            if (canApplyFilter)
            {
                var filters = GetFilters();
                this.View.FilterPredicates = filters;
               
                //if (this.View.TopLevelGroup != null)
                //    this.View.RefreshFiltering();
                this.RefreshDisplay(true);
                var startIdx = this.ResolveStartIndexBasedOnPosition();
                this.InvalidateCell(GridRangeInfo.Rows(startIdx, this.RowCount));
            }

            this.RefreshSourceListCount();
            this.IsInFilter = false;
            this.RefreshFromFilter = false;
            this.TableProperties.VisibleColumns.ResumeEvents();
            OnFilterChanged(args);
        }

        protected int GetResolveStartIndexBasedOnPosition()
        {
            return this.ResolveStartIndexBasedOnPosition();
        }

        protected ObservableCollection<IFilterDefinition> GetFilters()
        {
            return this.TableProperties.VisibleColumns.OfType<IFilterDefinition, GridDataVisibleColumn>();
        }

        internal void FilterColumn(GridDataVisibleColumn column, object filterValue, bool show, FilterType filterType, bool canApplyFilter)
        {
            GridFilterEventArgs args = null;
            if (column != null)
            {
                FilterPredicate predicate = null;
                if (!show)
                {
                    predicate = new FilterPredicate()
                    {
                        FilterValue = filterValue,
                        FilterType = filterType,
                        PredicateType = PredicateType.And,
                        IsCaseSensitive = true
                    };
                }
                else
                {
                    if (filterValue == null || filterValue == DBNull.Value)
                        predicate = null;
                    else
                        predicate = column.Filters.FirstOrDefault(fp => (fp.FilterValue == null ? "" : fp.FilterValue.ToString()) == filterValue.ToString());
                }
                args = new GridFilterEventArgs(column, predicate);
                OnFilterChanging(args);
                if (!args.Handled)
                {
                    this.TableProperties.VisibleColumns.SuspendEvents();
                    if (!this.IsInFilter && !this.isInitSuspended)
                    {
                        this.IsInFilter = true;
                    }

                    if (!show)
                    {
                        column.Filters.Add(predicate);
                    }
                    else
                    {
                        if (predicate != null)
                        {
                            column.Filters.Remove(predicate);
                        }
                    }
                    this.TableProperties.VisibleColumns.ResumeEvents();
                }
            }

            if (canApplyFilter)
            {
                ForceFilterRefresh();
            }

            this.RefreshSourceListCount();

            if (args != null)
                OnFilterChanged(args);

            if (!this.isInitSuspended)
            {
                this.IsInFilter = false;
            }
        }

        internal void ForceFilterRefresh()
        {
            var filters = this.TableProperties.VisibleColumns.OfType<IFilterDefinition, GridDataVisibleColumn>();
            this.View.FilterPredicates = filters;
            this.RefreshDisplay(true);
            var startIdx = this.ResolveStartIndexBasedOnPosition();
            this.InvalidateCell(GridRangeInfo.Rows(startIdx, this.RowCount));
        }

#if !SILVERLIGHT
        private DataTable GetDataTable(object itemsSource)
        {
            if (itemsSource is DataTable)
            {
                return itemsSource as DataTable;
            }
            else if (itemsSource is DataView)
            {
                return ((DataView)itemsSource).Table;
            }

            return null;
        }

        internal DataTable GetDataTable()
        {
            var itemsSource = this.SourceList;
            return this.GetDataTable(itemsSource);
        }
#endif

        private int sourceListCount = 0;
        public int SourceListCount
        {
            get
            {
                if (this.SourceList == null)
                {
                    return 0;
                }

                return this.sourceListCount;
            }

            private set
            {
                if (this.sourceListCount != value)
                {
                    this.sourceListCount = value;
                }
            }
        }
        internal double extendWidthValue;
        internal int ExpandedRecordCount
        {
            get;
            set;
        }

#if SyncfusionFramework4_0
#if !SILVERLIGHT
        internal void GetDisplayAttribute(PropertyDescriptor pd, ref bool canAddColumn, ref bool hasDisplayAttribute, out System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
        {
            var attr = pd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
#else
        private void GetDisplayAttribute(PropertyInfo pd, ref bool canAddColumn, ref bool hasDisplayAttribute, out System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
        {
            var attr = (System.ComponentModel.DataAnnotations.DisplayAttribute)pd.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute)); //xPd.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
#endif

            displayAttribute = null;
            if (attr != null)
            {
                displayAttribute = attr as System.ComponentModel.DataAnnotations.DisplayAttribute;
                if (!hasDisplayAttribute)
                {
                    hasDisplayAttribute = true;
                }

                if (displayAttribute.GetAutoGenerateField() != null)
                {
                    canAddColumn = displayAttribute.AutoGenerateField;
                }
            }

        }
#endif

#if SyncfusionFramework4_0
#if !SILVERLIGHT
        internal void GetBindableAttribute(PropertyDescriptor pd, ref bool canAddColumn, ref bool hasBindableAttribute, out System.ComponentModel.BindableAttribute bindableAttribute)
        {
            var attr = pd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.BindableAttribute));
#else
        private void GetBindableAttribute(PropertyInfo pd, ref bool canAddColumn, ref bool hasBindableAttribute, out System.ComponentModel.BindableAttribute bindableAttribute)
        {
            var attr = (System.ComponentModel.BindableAttribute)pd.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.BindableAttribute)); //xPd.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
#endif

            bindableAttribute = null;
            if (attr != null)
            {
                bindableAttribute = attr as System.ComponentModel.BindableAttribute;
                if (!hasBindableAttribute)
                {
                    hasBindableAttribute = true;
                }

                if (bindableAttribute != null)
                {
                    canAddColumn = bindableAttribute.Bindable;
                }
            }

        }


#endif

#if SyncfusionFramework4_0
        private void ProcessDisplayAttribute(System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute, GridDataVisibleColumn column)
        {
            if (displayAttribute != null)
            {
                if (displayAttribute.ShortName != null)
                {
                    column.HeaderText = displayAttribute.ShortName;
                }

                if (displayAttribute.Name != null && column.MappingName == null)
                {
                    column.MappingName = displayAttribute.Name;
                }

                if (displayAttribute.GetAutoGenerateFilter() != null)
                {
                    column.AllowFilter = displayAttribute.GetAutoGenerateFilter().Value;
                }


                if (displayAttribute.Description != null)
                {
                    GridDataColumnStyle columnStyle = column.ColumnStyle;
                    if (columnStyle == null)
                    {
                        columnStyle = new GridDataColumnStyle();
                    }

                    columnStyle.ToolTip = new ToolTip() { Content = displayAttribute.Description };
                }


                // TODO : If DisplayAttribute.GroupName is set, Do we need to group that column?
                ////if (displayAttribute.GroupName != null)
                ////{
                ////}
            }
        }
#endif
#if SyncfusionFramework4_0
        internal bool hasDisplayAttribute = false;
        internal bool hasBindableAttribute = false;
#endif

        public GridDataVisibleColumns GetVisibleColumns()
        {
            var visibleColumns = new GridDataVisibleColumns();
            var canGenerateColumns = true;
            if (this.View == null)
                return visibleColumns;
            var properties = this.View.GetItemProperties();
#if SyncfusionFramework4_0
            canGenerateColumns = !this.TableProperties.IsDynamicItemsSource;
            //var hasDisplayAttribute = false;
#endif
#if !SILVERLIGHT
            if (canGenerateColumns)
            {
                foreach (PropertyDescriptor pd in properties)
                {
                    var canAddColumn = !ListUtil.IsComplexType(pd) || typeof(byte[]).IsAssignableFrom(pd.PropertyType);
#if SyncfusionFramework4_0
                    System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                    this.GetDisplayAttribute(pd, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);

                    System.ComponentModel.BindableAttribute bindableAttribute = null;
                    this.GetBindableAttribute(pd, ref canAddColumn, ref hasBindableAttribute, out bindableAttribute);
                    if (hasBindableAttribute)
                    {
                        hasDisplayAttribute = false;
                    }
#endif
                    if (canAddColumn)
                    {
                        var column = new GridDataVisibleColumn()
                        {
                            MappingName = pd.Name,
                            //Width = new GridLength(this.TableProperties.DefaultColumnWidth, GridUnitType.Pixel),
                            Width = this.ResolveColumnSizer(properties.IndexOf(pd) + 1, properties.Count),
                            IsReadOnly = pd.IsReadOnly,
                            AllowDrag = this.TableProperties.AllowDragColumns,
                            AllowFilter = this.TableProperties.ShowFilters,
                            AllowResize = this.TableProperties.AllowResizeColumns,
                            AllowGroup = this.TableProperties.AllowGroup,
                            AllowSort = this.TableProperties.AllowSort,
                            ShowColumnOptions = this.TableProperties.ShowColumnOptions,
                            HeaderCellTemplate = this.TableProperties.HeaderCellTemplate,
                            UpdateMode = this.TableProperties.UpdateMode
                        };

                        if (this.TableProperties.AutoGenerateColumnsInfo)
                        {
                            AutoGenerateColumnsInfo(column, pd);
                        }
#if SyncfusionFramework4_0
                        if (hasDisplayAttribute)
                        {
                            this.ProcessDisplayAttribute(displayAttribute, column);
                        }
#endif
                        if (column!=null && string.IsNullOrEmpty(column.HeaderText))
                            column.HeaderText = column.MappingName;

                        visibleColumns.Add(column);
                    }
                }
            }
#else
                    if (canGenerateColumns)
                    {
                        var propList = properties.Keys.ToList();
                        foreach (var pd in properties)
                        {
                            var canAddColumn = !ListUtil.IsComplexType(pd.Value) || typeof(byte[]).IsAssignableFrom(pd.Value.PropertyType);
                            System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                            this.GetDisplayAttribute(pd.Value, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);

                            System.ComponentModel.BindableAttribute bindableAttribute = null;
                            this.GetBindableAttribute(pd.Value, ref canAddColumn, ref hasBindableAttribute, out bindableAttribute);
                            if (hasBindableAttribute)
                            {
                                hasDisplayAttribute = false;
                            }

                            if (canAddColumn)
                            {
                                var column = new GridDataVisibleColumn()
                                {
                                    MappingName = pd.Key,
                                    Width = this.ResolveColumnSizer(propList.IndexOf(pd.Key) + 1, properties.Count),
                                    IsReadOnly = !pd.Value.CanWrite,
                                    AllowDrag = this.TableProperties.AllowDragColumns,
                                    AllowFilter = this.TableProperties.ShowFilters,
                                    AllowResize = this.TableProperties.AllowResizeColumns,
                                    AllowGroup = this.TableProperties.AllowGroup,
                                    AllowSort = this.TableProperties.AllowSort,
                                    ShowColumnOptions = this.TableProperties.ShowColumnOptions,
                                    HeaderCellTemplate = this.TableProperties.HeaderCellTemplate,
                                    UpdateMode = this.TableProperties.UpdateMode   
                                };
                                if (hasDisplayAttribute)
                                {
                                    this.ProcessDisplayAttribute(displayAttribute, column);
                                }
                                if (this.TableProperties.AutoGenerateColumnsInfo)
                                    this.AutoGenerateColumnsInfo(column, pd.Value);
                                if (column.HeaderText == null || column.HeaderText == string.Empty)
                                    column.HeaderText = column.MappingName;
                                visibleColumns.Add(column);
                            }
                        }
                    }
#endif
            return visibleColumns;
        }
#if SyncfusionFramework4_0
        public GridDataVisibleColumns GetDynamicVisibleColumns()
        {
            var visiblecolumns = new GridDataVisibleColumns();
            var dynObj = this.View.Records[0].Data as System.Dynamic.IDynamicMetaObjectProvider;
            if (dynObj != null)
            {
                this.TableProperties.VisibleColumns.Clear();
                var metaType = dynObj.GetType();
                var metaData = dynObj.GetMetaObject(System.Linq.Expressions.Expression.Parameter(metaType, metaType.Name));
                foreach (var prop in metaData.GetDynamicMemberNames())
                {
                    if (Syncfusion.Dynamic.DynamicHelper.IsPythonType(prop) || Syncfusion.Dynamic.DynamicHelper.IsComplexCollection(dynObj, prop))
                    {
                        continue;
                    }

                    var column = new GridDataVisibleColumn()
                    {
                        MappingName = prop,
                        Width = new GridDataControlLength(this.TableProperties.DefaultColumnWidth),
                        IsReadOnly = false,
                        AllowDrag = this.TableProperties.AllowDragColumns,
                        AllowFilter = this.TableProperties.ShowFilters,
                        AllowResize = this.TableProperties.AllowResizeColumns,
                        AllowGroup = this.TableProperties.AllowGroup,
                        AllowSort = this.TableProperties.AllowSort,
                        ShowColumnOptions = this.TableProperties.ShowColumnOptions,
                        HeaderCellTemplate = this.TableProperties.HeaderCellTemplate,
                        UpdateMode = this.TableProperties.UpdateMode
                    };

                    if (string.IsNullOrEmpty(column.HeaderText))
                        column.HeaderText = column.MappingName;

                    visiblecolumns.Add(column);
                }
            }
            return visiblecolumns;
        }
#endif
        
#if !SILVERLIGHT
        private void AutoGenerateColumnsInfo(GridDataVisibleColumn column, PropertyDescriptor pd)
#else
        private void AutoGenerateColumnsInfo(GridDataVisibleColumn column, PropertyInfo pd)
#endif
        {
            if (column.ColumnStyle == null)
                column.ColumnStyle = new GridDataColumnStyle();

            if (pd.PropertyType.IsEnum)
            {
#if !SILVERLIGHT
                Array collection = Enum.GetValues(pd.PropertyType.UnderlyingSystemType);
#else
                Array collection = GridDataTableModelHelper.GetValues(pd.PropertyType.UnderlyingSystemType);
#endif
                column.ColumnStyle.CellType = "ComboBox";
                column.ColumnStyle.ItemsSource = collection;
                column.ColumnStyle.DropDownStyle = GridDropDownStyle.Editable;
            }
            else if (typeof(string).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "TextBox";
            }
            else if (typeof(bool).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "CheckBox";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Center;
                column.ColumnStyle.IsThreeState = false;
            }
            else if (typeof(bool?).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "CheckBox";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Center;
                column.ColumnStyle.IsThreeState = true;
            }
            else if (typeof(Uri).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "Hyperlink";
            }
            else if (typeof(int).IsAssignableFrom(pd.PropertyType) || typeof(Int32).IsAssignableFrom(pd.PropertyType)
                    || typeof(Int64).IsAssignableFrom(pd.PropertyType) || typeof(Int16).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "IntegerEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
                column.ColumnStyle.IntegerEdit.UseNullOption = false;
            }
            else if (typeof(int?).IsAssignableFrom(pd.PropertyType) || typeof(Int32?).IsAssignableFrom(pd.PropertyType)
                || typeof(Int64?).IsAssignableFrom(pd.PropertyType) || typeof(Int16?).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "IntegerEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
                column.ColumnStyle.IntegerEdit.UseNullOption = true;
            }
            else if (typeof(double).IsAssignableFrom(pd.PropertyType) || typeof(decimal).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "DoubleEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
                column.ColumnStyle.DoubleEdit.UseNullOption = false;
            }
            else if (typeof(double?).IsAssignableFrom(pd.PropertyType) || typeof(decimal?).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "DoubleEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
                column.ColumnStyle.DoubleEdit.UseNullOption = true;
            }
            else if (typeof(DateTime).IsAssignableFrom(pd.PropertyType) || typeof(DateTime?).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "DateTimeEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
            }
            else if (typeof(TimeSpan).IsAssignableFrom(pd.PropertyType))
            {
                column.ColumnStyle.CellType = "TimeSpanEdit";
                column.ColumnStyle.HorizontalAlignment = HorizontalAlignment.Right;
            }
        }

        /// <summary>
        /// Initializes the table.
        /// </summary>
        private void InitializeTable()
        {
            this.TableProperties.UnwireEvents();

            var properties = this.View.GetItemProperties();
            if (properties != null)
            {
                ////populate columns
                if (this.TableProperties.AutoPopulateColumns)
                {
                    //this.TableProperties.VisibleColumns.Clear();
                    this.TableProperties.VisibleColumns.Dispose();
                    this.TableProperties.VisibleColumns = this.GetVisibleColumns();
                }
#if SyncfusionFramework4_0
                else
                {
#if !SILVERLIGHT
                    foreach (PropertyDescriptor pd in properties)
                    {
                        var canAddColumn = !ListUtil.IsComplexType(pd) || typeof(byte[]).IsAssignableFrom(pd.PropertyType);
                        System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                        this.GetDisplayAttribute(pd, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);

                        System.ComponentModel.BindableAttribute bindableAttribute = null;
                        this.GetBindableAttribute(pd, ref canAddColumn, ref hasBindableAttribute, out bindableAttribute);
                        if (hasBindableAttribute)
                        {
                            hasDisplayAttribute = false;
                        }

                        if (canAddColumn)
                        {
                            var column = this.tableProperties.VisibleColumns.FirstOrDefault((col) =>
                            {
                                return pd.Name.Equals(col.MappingName);
                            });

                            if (column != null && string.IsNullOrEmpty(column.HeaderText) && hasDisplayAttribute && displayAttribute != null)
                            {
                                string ShortName=  displayAttribute.GetShortName();
                                if (!string.IsNullOrEmpty(ShortName))
                                {
                                    column.HeaderText = ShortName;
                                }
                            }
                            if (column != null && string.IsNullOrEmpty(column.HeaderText))
                                column.HeaderText = column.MappingName;
                        }
                        
                    }
#else
                    {
                        foreach (var pd in properties)
                        {
                            var canAddColumn = !ListUtil.IsComplexType(pd.Value) || typeof(byte[]).IsAssignableFrom(pd.Value.PropertyType);
                            System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                            this.GetDisplayAttribute(pd.Value, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);

                            System.ComponentModel.BindableAttribute bindableAttribute = null;
                            this.GetBindableAttribute(pd.Value, ref canAddColumn, ref hasBindableAttribute, out bindableAttribute);
                            if (hasBindableAttribute)
                            {
                                hasDisplayAttribute = false;
                            }

                            if (canAddColumn)
                            {
                                var column = this.tableProperties.VisibleColumns.FirstOrDefault((col) =>
                                {
                                    return pd.Key.Equals(col.MappingName);
                                });

                                if (column != null && string.IsNullOrEmpty(column.HeaderText) && hasDisplayAttribute && displayAttribute != null)
                                {
                                    if (!string.IsNullOrEmpty(displayAttribute.ShortName))
                                    {
                                        column.HeaderText = displayAttribute.ShortName;
                                    }
                                }
                                if (column!=null && string.IsNullOrEmpty(column.HeaderText))
                                    column.HeaderText = column.MappingName;
                            }

                        }
                    }
#endif
                }
                if (this.TableProperties.IsDynamicItemsSource && this.TableProperties.AutoPopulateColumns && /*properties.Count == 0 &&*/ this.View.Records.Count > 0)
                {
                    this.TableProperties.VisibleColumns = this.GetDynamicVisibleColumns();
                }
#endif
                if (this.TableProperties.AutoPopulateRelations)
                {
                    this.TableProperties.Relations.Clear();
                    if (this.View.Relations != null)
                        this.View.Relations.Clear();
#if !SILVERLIGHT
                    foreach (PropertyDescriptor pd in properties)
                    {
                        if (!this.IsLegacyDataTable)
                        {
                            ////load the relational column
                            var isRelationalType = ListUtil.IsComplexType(pd.PropertyType) && !typeof(byte[]).IsAssignableFrom(pd.PropertyType);
                            if (isRelationalType)
                            {
#if SyncfusionFramework4_0
                                var canGenerateRelation = true;
                                var attr = pd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
                                System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                                if (attr != null)
                                {
                                    displayAttribute = attr as System.ComponentModel.DataAnnotations.DisplayAttribute;
                                    if (displayAttribute.GetAutoGenerateField() != null)
                                    {
                                        canGenerateRelation = displayAttribute.AutoGenerateField;
                                    }
                                }
                                if (!canGenerateRelation)
                                {
                                    continue;
                                }
#endif
                                ExtractRelationalColumn(pd);
                            }
                        }
                        else
                        {
                            var datarelation = ListUtil.GetDataRelation(pd);
                            if (datarelation != null)
                            {
                                var relation = new GridDataRelation
                                {
                                    RelationType = RelationType.MasterDetails,
                                    RelationalColumn = datarelation.RelationName
                                };
                                this.TableProperties.Relations.Add(relation);
                                this.View.Relations.Add((IRelationDefinition)relation);
                            }
                        }

                    }
#else
                    foreach (var pd in properties)
                    {
                        if (ListUtil.IsComplexType(pd.Value.PropertyType) && !typeof(byte[]).IsAssignableFrom(pd.Value.PropertyType))
                        {
                            ExtractRelationalColumn(pd.Value);
                        }
                    }
#endif
                }
                else
                {
                    foreach (var relation in this.TableProperties.Relations)
                    {
                        this.View.Relations.Add((IRelationDefinition)relation);
                    }
                }
            }
            this.TableProperties.WireEvents();
        }

        #region visual styles

        internal Brush GetRowHeaderSelectionBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.RowHeaderSelectionBackground != null)
                return TableProperties.StyleManager.RowAppearence.RowHeaderSelectionBackground;

            return GridVisualStyle.RowHeaderSelectionBackground;
        }

        internal Brush GetPlusMinusButtonBackgroundBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonBackground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonBackground;

            return GridVisualStyle.PlusMinusButtonBackground;
        }

        internal Brush GetPlusMinusButtonBorderBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonBorderBrush != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonBorderBrush;

            return GridVisualStyle.PlusMinusButtonBorderBrush;
        }

        internal Brush GetPlusMinusButtonForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonForeground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusButtonForeground;

            return GridVisualStyle.PlusMinusButtonForeground;
        }

        internal Brush GetDragDropIndicatorBrush()
        {
            return GetDragDropIndicatorBrush(GridVisualStyle);
        }

        internal Brush GetDragDropIndicatorOuterBrush()
        {
            return GetDragDropIndicatorOuterBrush(GridVisualStyle);
        }

        internal Brush GetDragDropIndicatorBrush(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.DragDropIndicatorBrush != null)
                return TableProperties.StyleManager.GroupAreaAppearence.DragDropIndicatorBrush;

            return value.DragDropIndicatorBrush;
        }

        internal Brush GetDragDropIndicatorOuterBrush(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.DragDropIndicatorBrush != null)
                return TableProperties.StyleManager.GroupAreaAppearence.DragDropIndicatorBrush;

            return value.DragDropIndicatorOuterBrush;
        }

        internal Brush GetRowHeaderForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.RowHeaderForeground != null)
                return TableProperties.StyleManager.RowAppearence.RowHeaderForeground;

            return GridVisualStyle.RowHeaderForeground;
        }

        internal Brush GetRowBackground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.RowBackground != null)
                return TableProperties.StyleManager.RowAppearence.RowBackground;

            return value.RowBackground;
        }

        internal Brush GetGroupAreaBackgroundBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.GroupAreaBackgroundBrush != null)
                return TableProperties.StyleManager.GroupAreaAppearence.GroupAreaBackgroundBrush;

            return GridVisualStyle.GroupAreaBackgroundBrush;
        }

        internal Brush GetGroupAreaForegroundBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.GroupAreaForegroundBrush != null)
                return TableProperties.StyleManager.GroupAreaAppearence.GroupAreaForegroundBrush;

            return GridVisualStyle.GroupAreaForegroundBrush;
        }

        internal GridFontInfo GetGroupHeaderFont()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.GroupHeaderFont != null)
                return TableProperties.StyleManager.GroupAreaAppearence.GroupHeaderFont;

            return GridVisualStyle.GroupHeaderFont;
        }

        internal CellBordersInfo GetGroupCellBorders()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.GroupCellBorders != null)
                return TableProperties.StyleManager.GroupAreaAppearence.GroupCellBorders;

            return GridVisualStyle.GroupCellBorders;
        }

        internal CellMarginsInfo GetGroupCellBorderMargins()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.GroupAreaAppearence != null && TableProperties.StyleManager.GroupAreaAppearence.GroupCellBorderMargins != null)
                return TableProperties.StyleManager.GroupAreaAppearence.GroupCellBorderMargins;

            return GridVisualStyle.GroupCellBorderMargins;
        }

        internal Brush GetHighlightSelectionBackground()
        {
            return GetHighlightSelectionBackground(GridVisualStyle);
        }

        internal Brush GetHighlightSelectionBackground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HighlightSelectionBackground != null)
                return TableProperties.StyleManager.RowAppearence.HighlightSelectionBackground;
            return value.HighlightSelectionBackground;
        }

        internal Brush GetHighlightSelectionForeground()
        {
            return GetHighlightSelectionForeground(GridVisualStyle); 
        }

        internal Brush GetHighlightSelectionForeground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HighlightSelectionForeground != null)
                return TableProperties.StyleManager.RowAppearence.HighlightSelectionForeground;

            return value.HighlightSelectionForeground;
        }

        internal GridFontInfo GetSummaryCaptionFont()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryCaptionFont != null)
                return TableProperties.StyleManager.RowAppearence.SummaryCaptionFont;

            return GridVisualStyle.SummaryCaptionFont;
        }

        internal GridFontInfo GetSummaryRowFont()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryRowFont != null)
                return TableProperties.StyleManager.RowAppearence.SummaryRowFont;

            return GridVisualStyle.SummaryRowFont;
        }

        internal Brush GetSummaryRowBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryRowBackground != null)
                return TableProperties.StyleManager.RowAppearence.SummaryRowBackground;

            return GridVisualStyle.SummaryRowBackground;
        }

        internal Brush GetSummaryRowForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryRowForeground != null)
                return TableProperties.StyleManager.RowAppearence.SummaryRowForeground;

            return GridVisualStyle.SummaryRowForeground;
        }

        internal Brush GetSummaryCaptionForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryCaptionForeground != null)
                return TableProperties.StyleManager.RowAppearence.SummaryCaptionForeground;

            return GridVisualStyle.SummaryCaptionForeground;
        }

        internal Brush GetSummaryCaptionBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.SummaryCaptionBackground != null)
                return TableProperties.StyleManager.RowAppearence.SummaryCaptionBackground;

            return GridVisualStyle.SummaryCaptionBackground;
        }

        internal Brush GetHeaderBackground()
        {
            return GetHeaderBackground(GridVisualStyle);            
        }

        internal Brush GetHeaderForeground()
        {
            return GetHeaderForeground(GridVisualStyle);
        }

        internal Brush GetRowHeaderBackground()
        {
            return GetRowHeaderBackground(GridVisualStyle);
        }

        internal Brush GetRowHeaderBackground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.RowHeaderBackground != null)
                return TableProperties.StyleManager.RowAppearence.RowHeaderBackground;

            return value.RowHeaderBackground;
        }

        internal Brush GetHeaderBackground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderBackgroundBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderBackgroundBrush;

            return value.HeaderBackgroundBrush;
        }

        internal Brush GetHeaderForeground(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderForegroundBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderForegroundBrush;

            return value.HeaderForegroundBrush;
        }

        internal Brush GetHeaderHoverBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderHoverBackgroundBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderHoverBackgroundBrush;

            return GridVisualStyle.HeaderHoverBackgroundBrush;
        }

        internal Brush GetHeaderHoverForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderHoverForegroundBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderHoverForegroundBrush;

            return GridVisualStyle.HeaderHoverForegroundBrush;
        }
       
        internal CellBordersInfo GetHeaderCellBorders()
        {
            return GetHeaderCellBorders(GridVisualStyle);
        }

        internal CellBordersInfo GetHeaderCellBorders(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderCellBorders != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderCellBorders;

            return value.HeaderCellBorders;
        }

        internal Brush GetHeaderInnerBorder()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderInnerBorder != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderInnerBorder;

            return GridVisualStyle.HeaderInnerBorder;
        }

        internal Brush GetHeaderOptionsHoverBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderInnerBorder != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderOptionsHoverBackground;

            return GridVisualStyle.HeaderOptionsHoverBackground;
        }

        internal Brush GetHeaderOptionsBorderBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderOptionsBorderBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderOptionsBorderBrush;

            return GridVisualStyle.HeaderOptionsBorderBrush;
        }

        internal Brush GetHeaderOptionsCheckedBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderOptionsCheckedBackground != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderOptionsCheckedBackground;

            return GridVisualStyle.HeaderOptionsCheckedBackground;
        }

        internal Thickness GetHeaderInnerBorderThickness()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderInnerBorderThickness != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderInnerBorderThickness;

            return GridVisualStyle.HeaderInnerBorderThickness;
        }

        internal GridFontInfo GetHeaderFont()
        {
            return GetHeaderFont(GridVisualStyle);
        }

        internal GridFontInfo GetHeaderFont(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderFont != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderFont; 

            return value.HeaderFont;
        }

        internal CellMarginsInfo GetHeaderTextMargins()
        {
            return GetHeaderTextMargins(GridVisualStyle);
        }

        internal CellMarginsInfo GetHeaderTextMargins(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.HeaderTextMargins != null)
                return TableProperties.StyleManager.HeaderAppearence.HeaderTextMargins; 

            return value.HeaderTextMargins;
        }

        internal Brush GetSortWidgetBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.SortWidgetBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.SortWidgetBrush; 

            return GridVisualStyle.SortWidgetBrush;
        }

        internal Brush GetFilterButtonInnerBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.FilterButtonInnerBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.FilterButtonInnerBrush; 

            return GridVisualStyle.FilterButtonInnerBrush;
        }

        internal Brush GetFilterButtonOuterBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.FilterButtonOuterBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.FilterButtonOuterBrush; 

            return GridVisualStyle.FilterButtonOuterBrush;
        }

        internal Brush GetFilterButtonHoverInnerBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.FilterButtonHoverInnerBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.FilterButtonHoverInnerBrush; 

            return GridVisualStyle.FilterButtonHoverInnerBrush;
        }

        internal Brush GetFilterButtonHoverOuterBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.FilterButtonHoverOuterBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.FilterButtonHoverOuterBrush; 

            return GridVisualStyle.FilterButtonHoverOuterBrush;
        }

        internal Brush GetFilterButtonAppliedBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.FilterButtonAppliedBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.FilterButtonAppliedBrush; 

            return GridVisualStyle.FilterButtonAppliedBrush;
        }

        internal Brush GetColumnOptionsPopupBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.ColumnOptionsPopupBackground != null)
                return TableProperties.StyleManager.HeaderAppearence.ColumnOptionsPopupBackground; 

            return GridVisualStyle.ColumnOptionsPopupBackground;
        }

        internal Brush GetColumnOptionsPopupForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.ColumnOptionsPopupForeground != null)
                return TableProperties.StyleManager.HeaderAppearence.ColumnOptionsPopupForeground; 

            return GridVisualStyle.ColumnOptionsPopupForeground;
        }

        internal Brush GetColumnOptionsButtonBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.ColumnOptionsButtonBackground != null)
                return TableProperties.StyleManager.HeaderAppearence.ColumnOptionsButtonBackground; 

            return GridVisualStyle.ColumnOptionsButtonBackground;
        }

#if!SILVERLIGHT
        internal Brush GetColumnOptionsButtonBorderBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.ColumnOptionsButtonBorderBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.ColumnOptionsButtonBorderBrush; 

            return GridVisualStyle.ColumnOptionsButtonBorderBrush;
        }
#endif

        internal Brush GetGroupCaptionSelectionBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.GroupCaptionSelectionBackground != null)
                return TableProperties.StyleManager.RowAppearence.GroupCaptionSelectionBackground; 

            return GridVisualStyle.GroupCaptionSelectionBackground;
        }

        internal Brush GetCurrentCellSelectionBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.CurrentCellSelectionBackground != null)
                return TableProperties.StyleManager.RowAppearence.CurrentCellSelectionBackground; 

            return GridVisualStyle.CurrentCellSelectionBackground;
        }

        internal Brush GetGroupCaptionSelectionForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.GroupCaptionSelectionForeground != null)
                return TableProperties.StyleManager.RowAppearence.GroupCaptionSelectionForeground; 

            return GridVisualStyle.GroupCaptionSelectionForeground;
        }

#if !SILVERLIGHT

        internal Brush GetHoveringGroupCaptionCellBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HoveringGroupCaptionCellBackground != null)
                return TableProperties.StyleManager.RowAppearence.HoveringGroupCaptionCellBackground;

            return GridVisualStyle.HoveringGroupCaptionCellBackground;
        }

        internal Brush GetHoveringRecordCellForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HoveringRecordCellForeground != null)
                return TableProperties.StyleManager.RowAppearence.HoveringRecordCellForeground;

            return GridVisualStyle.HoveringRecordCellForeground;
        }

#endif

        internal Brush GetHoveringRecordCellBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HoveringRecordCellBackground != null)
                return TableProperties.StyleManager.RowAppearence.HoveringRecordCellBackground;

            return GridVisualStyle.HoveringRecordCellBackground;
        }

        internal Brush GetHighlightBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.HighlightBrush != null)
                return TableProperties.StyleManager.RowAppearence.HighlightBrush;

            return GridVisualStyle.HighlightBrush;
        }

        internal Brush GetCurrentCellSelectionForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.RowAppearence != null && TableProperties.StyleManager.RowAppearence.CurrentCellSelectionForeground != null)
                return TableProperties.StyleManager.RowAppearence.CurrentCellSelectionForeground; 

            return GridVisualStyle.CurrentCellSelectionForeground;
        }

        internal Brush GetColumnOptionsCloseButtonBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.HeaderAppearence != null && TableProperties.StyleManager.HeaderAppearence.ColumnOptionsCloseButtonBrush != null)
                return TableProperties.StyleManager.HeaderAppearence.ColumnOptionsCloseButtonBrush; 

            return GridVisualStyle.ColumnOptionsCloseButtonBrush;
        }

        internal Brush GetPlusMinusExpandedButtonBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonBackground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonBackground; 

            return GridVisualStyle.PlusMinusExpandedButtonBackground;
        }

        internal Brush GetPlusMinusExpandedButtonBorderBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonBorderBrush != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonBorderBrush; 

            return GridVisualStyle.PlusMinusExpandedButtonBorderBrush;
        }

        internal Brush GetPlusMinusExpandedButtonForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonForeground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusExpandedButtonForeground; 

            return GridVisualStyle.PlusMinusExpandedButtonForeground;
        }

        internal Brush GetPlusMinusHoverButtonBackground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonBackground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonBackground; 

            return GridVisualStyle.PlusMinusHoverButtonBackground;
        }

        internal Brush GetPlusMinusHoverButtonBorderBrush()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonBorderBrush != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonBorderBrush; 

            return GridVisualStyle.PlusMinusHoverButtonBorderBrush;
        }

        internal Brush GetPlusMinusHoverButtonForeground()
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ExpanderAppearence != null && TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonForeground != null)
                return TableProperties.StyleManager.ExpanderAppearence.PlusMinusHoverButtonForeground; 

            return GridVisualStyle.PlusMinusHoverButtonForeground;
        }     

        internal CellBordersInfo GetValueCellBorders(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.ValueCellBorders != null)
                return TableProperties.StyleManager.ValueCellAppearance.ValueCellBorders; 

            return value.ValueCellBorders;
        }

        internal CellBordersInfo GetValueCellBorders()
        {
            return GetValueCellBorders(GridVisualStyle);
        }

        internal CellMarginsInfo GetValueTextMargins()
        {
            return GetValueTextMargins(GridVisualStyle);
        }

        internal CellMarginsInfo GetValueTextMargins(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.ValueTextMargins != null)
                return TableProperties.StyleManager.ValueCellAppearance.ValueTextMargins; 

            return value.ValueTextMargins;
        }

        internal GridFontInfo GetValueFont()
        {
            return GetValueFont(GridVisualStyle);
        }

        internal GridFontInfo GetValueFont(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.ValueFont != null)
                return TableProperties.StyleManager.ValueCellAppearance.ValueFont; 

            return value.ValueFont;
        }
                       
        internal Brush GetValueBackgroundBrush(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.ValueBackgroundBrush != null)
                return TableProperties.StyleManager.ValueCellAppearance.ValueBackgroundBrush; 

            return value.ValueBackgroundBrush;
        }

        internal Brush GetValueForegroundBrush(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.ValueForegroundBrush != null)
                return TableProperties.StyleManager.ValueCellAppearance.ValueForegroundBrush; 

            return value.ValueForegroundBrush;
        }

        internal Brush GetValueBackgroundBrush()
        {
            return GetValueBackgroundBrush(GridVisualStyle);
        }

        internal Brush GetValueForegroundBrush()
        {
            return GetValueForegroundBrush(GridVisualStyle);
        }

        internal Brush GetCurrentCellBorderBrush()
        {
            return GetCurrentCellBorderBrush(GridVisualStyle);
        }

        internal double GetCurrentCellBorderWidth()
        {
            return GetCurrentCellBorderWidth(GridVisualStyle);
        }

        internal Brush GetCurrentCellBorderBrush(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.CurrentCellBorderBrush != null)
                return TableProperties.StyleManager.ValueCellAppearance.CurrentCellBorderBrush; 

            return value.CurrentCellBorderBrush;
        }

        internal double GetCurrentCellBorderWidth(IGridDataVisualStyle value)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null) //  warning CS0472 && TableProperties.StyleManager.ValueCellAppearance.CurrentCellBorderWidth != null
                return TableProperties.StyleManager.ValueCellAppearance.CurrentCellBorderWidth; 

            return value.CurrentCellBorderWidth;
        }



        internal Brush GetGridBorderBrush()
        {
            return GetGridBorderBrush(GridVisualStyle);
        }

        internal Thickness GetGridBorderThickness()
        {
            return GetGridBorderThickness(GridVisualStyle);
        }

        internal Brush GetGridBorderBrush(IGridDataVisualStyle visualStyle)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.GridBorderBrush != null)
                return TableProperties.StyleManager.ValueCellAppearance.GridBorderBrush;

            return visualStyle.GridBorderBrush;
        }

        internal Thickness GetGridBorderThickness(IGridDataVisualStyle visualStyle)
        {
            if (TableProperties != null && TableProperties.StyleManager != null && TableProperties.StyleManager.ValueCellAppearance != null && TableProperties.StyleManager.ValueCellAppearance.GridBorderThickness != null)
                return TableProperties.StyleManager.ValueCellAppearance.GridBorderThickness;

            return visualStyle.GridBorderThickness;
        }



//#if !SILVERLIGHT
        /// <summary>
        /// Returns the VisualStyle ResourceDictionary.
        /// </summary>
        /// 

        public ResourceDictionary GetVisualStyleDictionary(FrameworkElement element)
        {
            if (this.TableProperties.VisualStyle == VisualStyle.BureauBlue || this.TableProperties.VisualStyle == VisualStyle.GlassyGreen || this.TableProperties.VisualStyle == VisualStyle.TwilightBlue || this.TableProperties.VisualStyle == VisualStyle.SyncfusionTheme || this.TableProperties.VisualStyle == VisualStyle.Windows7
                    || this.TableProperties.VisualStyle == VisualStyle.SunBlack || this.TableProperties.VisualStyle == VisualStyle.Office2007Blue || this.TableProperties.VisualStyle == VisualStyle.Office2007Black || this.TableProperties.VisualStyle == VisualStyle.Office2007Silver || this.TableProperties.VisualStyle == VisualStyle.Blend || this.TableProperties.VisualStyle == VisualStyle.Office14Blue ||
                    this.TableProperties.VisualStyle == VisualStyle.Office14Black || this.TableProperties.VisualStyle == VisualStyle.Office14Silver || this.TableProperties.VisualStyle == VisualStyle.ShinyRed || this.TableProperties.VisualStyle == VisualStyle.ShinyBlue || this.TableProperties.VisualStyle == VisualStyle.VS2010 || this.TableProperties.VisualStyle == VisualStyle.Default || this.TableProperties.VisualStyle == VisualStyle.Metro)
            {
                string str = this.TableProperties.VisualStyle.ToString();
                if (this.TableProperties.VisualStyle == VisualStyle.Default)
                    str = "Windows7";
#if !SILVERLIGHT
                str = "/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/" + str + "Style.xaml";
#else
                str = "/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/" + str + "Style.xaml";
#endif
                if (RemoveDictionaryIfExist(element, str))
                {
                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri(str, UriKind.RelativeOrAbsolute);
                    element.Resources.MergedDictionaries.Add(dictionary);
                }
            }
            else
                RemoveDictionaryIfExist(element, this.TableProperties.VisualStyle.ToString());
            return element.Resources;
        }

        private static bool RemoveDictionaryIfExist(FrameworkElement element, string dictionary)
        {
            if (element != null)
            {
                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source.ToString() == dictionary)
                    {
                        return false;
                    }
#if SILVERLIGHT
                        else if (rdic.Source.ToString().Contains("/Syncfusion.Grid.Silverlight;component/GridDataControl/Control/Themes/"))
#else
                        else if (rdic.Source.ToString().Contains("/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/"))
#endif
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }

                return true;
            }

            return false;
        }
//#endif
        #endregion

        //To be used only when AutoPopulateRelations is True
        private GridDataControlLength ResolveColumnSizer(int index, int totalCount)
        {
            GridDataControlLength columnLength = null;
            switch (this.TableProperties.ColumnSizer)
            {
                case GridControlLengthUnitType.Auto:
                case GridControlLengthUnitType.AutoOnLoad:
                    columnLength = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
                    break;
                case GridControlLengthUnitType.AutoOnLoadWithLastColumnFill:
                case GridControlLengthUnitType.AutoWithLastColumnFill:
                    if (totalCount == index)
                    {
                        columnLength = new GridDataControlLength(1d, GridControlLengthUnitType.Star);
                    }
                    else
                    {
                        columnLength = new GridDataControlLength(1d, GridControlLengthUnitType.Auto);
                    }

                    break;
                case GridControlLengthUnitType.SizeToCells:
                case GridControlLengthUnitType.SizeToHeader:
                case GridControlLengthUnitType.Star:
                    columnLength = new GridDataControlLength(1d, this.TableProperties.ColumnSizer);
                    break;
                case GridControlLengthUnitType.None:
                    columnLength = new GridDataControlLength(this.TableProperties.DefaultColumnWidth);
                    break;
            }

            if (columnLength == null)
            {
                columnLength = new GridDataControlLength(this.TableProperties.DefaultColumnWidth);
            }

            return columnLength;
        }

        protected virtual void InternalGrid_CellClick(object sender, GridCellClickEventArgs e)
        {
#if !SILVERLIGHT
            if (Mouse.GetPosition(this.Grid).X > this.Grid.ScrollColumns.ViewSize
                || Mouse.GetPosition(this.Grid).Y > this.Grid.ScrollRows.ViewSize)
                return;
#endif

            if (this is GridDataChildTableModel)
            {
                var nestedEditor = sender as GridDataCellNestedGridEditor;
                //// since we listen to the same grid cell renderer, other Models are also firing the events, check if the Model is same here for proceeding
                if (!Object.Equals(this, nestedEditor.TableModel))
                {
                    return;
                }
            }

            if (e.RowIndex > -1 && e.ColumnIndex > -1)
            {
                var style = this[e.RowIndex, e.ColumnIndex] as GridDataStyleInfo;
                var tableStyleInfoIdentity = style.CellIdentity;
                if (tableStyleInfoIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell || tableStyleInfoIdentity.TableCellType == GridDataTableCellType.UnboundColumnHeaderCell)
                {
                    var column = tableStyleInfoIdentity.Column;
                    /*if (column.IsUnbound)
                    {
                        return;
                    }*/

                    if (this.View == null)
                    {
                        return;
                    }

#if !SILVERLIGHT
                    if (this.TableProperties.SortClickAction == SortClickAction.DoubleClick && e.ClickCount != 2)
                    {
                        return;
                    }
#endif

                    if (this.View.CanSort)
                    {
                        var isInAddNewRow = this.CurrencyManager.IsInAddNewRow;
                        if (!isInAddNewRow && this.CurrencyManager.IsEditing)
                        {
                            //// if the currency manager is in edit, we first need to confirm the changes and then apply the sort
                            this.CurrencyManager.ConfirmChanges();
                            this.CurrencyManager.CurrentCell.Deactivate();
                        }

                        //// handle sorting
                        this.SortColumn(column);
                        this.InvalidateCell(GridRangeInfo.Cell(e.RowIndex, e.ColumnIndex));
                        //The below condition is added to set the top row index as current cell when the HeaderCell is clicked
                        if (!this.Table.HasGroups && !isInAddNewRow && this.Options.ListBoxSelectionMode == GridSelectionMode.None && !this.TableProperties.ShowFilterBar)
                            this.Grid.CurrentCell.MoveTo(Grid.TopRowIndex, e.ColumnIndex);
                        //// if we are in addnew row, we simply have to call BeginEdit to listen to editing changes
                        if (isInAddNewRow)
                        {
                            var currentRecordIndex = this.ResolveIndexToRecordPosition(e.RowIndex);
                            if (currentRecordIndex > -1) this.CurrencyManager.BeginEdit(currentRecordIndex);
                        }
                    }
                    else
                    {
                        //MessageBox.Show(GridDataResources.CanntPerformSortMessage);
                    }
                }
                else if (tableStyleInfoIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell)
                {
                    if (this.Table.HasNestedTables || this.Table.HasDetailsView)
                    {
                        int columnIndex = this.ResolveDefaultColumnOffset();

                        //Click expander may be a nested table expander or details veiw expander, hence resolving the cell
                        if (!this.Table.HasDetailsView || (columnIndex - e.ColumnIndex) > 1)
                        {
                            //Processing nested table expander click
                            this.ExpandCollapseRecord(e.RowIndex, e.ColumnIndex);
                        }
                        else
                        {
                            //Processing the details view expander click
                            if (this.Table.HasGroups)
                            {
                                int recordIndex = this.ResolveIndexToGroupPosition(e.RowIndex);
                                var record = this.View.TopLevelGroup.DisplayElements[recordIndex] as GridDataRecord;
                                record.IsDetailsViewExpanded = !record.IsDetailsViewExpanded;
                                this.Grid.InvalidateArrange();
                            }
                            else
                            {
                                int recordIndex = this.ResolveIndexToRecordPosition(e.RowIndex);
                                var record = this.View.Records[recordIndex] as GridDataRecord;
                                record.IsDetailsViewExpanded = !record.IsDetailsViewExpanded;
                            }
                        }

                        //Record collapsed does not affect parent grid size.
                        if (this is GridDataChildTableModel)
                        {
                            this.RefreshParentGrids();
                        }
                        this.ColumnAutoSizer.SetStarWidth();
                    }
                }
                else if (tableStyleInfoIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell
#if !SILVERLIGHT
 || (tableStyleInfoIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell && e.ClickCount == 2)
#endif
)
                {
                    var rowIndex = this.ResolveIndexToGroupPosition(e.RowIndex);
                    if (rowIndex > -1)
                    {
                        this.View.TopLevelGroup.ResetCache = true;
                        var group = this.Table.GroupModel.DisplayElements[rowIndex];
                        var isGroupKey = group is Group;
                        if (isGroupKey)
                        {
                            var groupKey = group as Group;
                            if (!groupKey.IsExpanded)
                            {
                                this.Table.ExpandGroup(groupKey);
                            }
                            else
                            {
                                this.Table.CollapseGroup(groupKey);
                            }
                        }
                        this.InvalidateCell(GridRangeInfo.Cell(e.RowIndex, e.ColumnIndex));
                        this.RefreshSourceListCount();
                    }
                }
            }
        }


        bool unloadHeader = true;
        internal void InvalidateDisplay()
        {
            InvalidateDisplay(true);
        }

        internal void InvalidateDisplay(bool needsToUnloadUiElment)
        {
            if (this.Grid != null)
            {
                if (this.Grid.CurrentCell != null)
                {
                    this.Grid.CurrentCell.Deactivate();
                }

                this.Grid.RenderStyles.Clear();
                if (needsToUnloadUiElment)
                {
                    if (this.Grid.ArrangedCellUIElements != null)
                    {
                        if (unloadHeader)
                            this.Grid.ArrangedCellUIElements.UnloadAll();
                        else
                            this.Grid.ArrangedCellUIElements.Unload(GridRangeInfo.Rows(this.HeaderRows, this.RowCount).ToCellSpan(this));
                    }
                }
#if !SILVERLIGHT
                if (this.Grid.RenderedCellVisuals != null)
                    this.Grid.RenderedCellVisuals.Invalidate();
                if (this.Grid.CellRenderers != null)
                    this.Grid.CellRenderers.EmptyRecycleBin();
#endif
            }
            if (this.VolatileCellStyles != null)
                this.VolatileCellStyles.Clear();
            if (this.Data != null)
                this.Data.Clear();
            this.InvalidateVisual(true);
        }

        private GridStyleInfo indentColumnStyle = new GridStyleInfo();

        public GridStyleInfo IndentColumnStyle
        {
            get
            {
                return this.indentColumnStyle;
            }
        }
# if !SILVERLIGHT
        protected override ContextMenu OnContextMenuCreating()
        {
            if (this.Grid != null && this.Grid.Model != null && Grid.Model.Views.Count() > 0)
            {
                if (this.Grid.ContextMenu == null)
                {
                    var menu = new ContextMenu {DataContext = this.Grid.DataContext};
                    return menu;
                }
            }
            return base.OnContextMenuCreating();
        }
#endif
        protected override void OnClipboardCanPaste(GridCutPasteEventArgs e)
        {
            base.OnClipboardCanPaste(e);
            //if it's not editable, force the CanPaste to return false by setting "e.Handled=true";if it's editable,should follow the base class, in which code snippet mentioned before will decide CanPaste .
            if (!this.TableProperties.AllowEdit)
            {
                e.Handled = true;
            }
        }

        //Earlier RowBackgroud, AlternateRowBackgroud and RowForeGround/AlternateRowForeground are set separetely and styles are calculated newly.
        //So to overcome this, both are set in same place
        protected override void OnQueryBaseStyles(GridQueryBaseStylesEventArgs e)
        {
            base.OnQueryBaseStyles(e);
            if (e.Handled)
            {
                return;
            }

            var colIdx = this.TableProperties.ShowRowHeader ? 1 : 0;
            colIdx = (this.Table.HasNestedTables ? colIdx + 1 : colIdx) + this.TableProperties.HeaderColumns;
            colIdx += this.Table.HasDetailsView ? 1 : 0;

            if (this.TableProperties.RowBackground != null || this.TableProperties.AlternatingRowBackground != null || this.TableProperties.RowForeground != null || this.TableProperties.AlternatingRowForeground != null)
            {
                if ((e.Cell.ColumnIndex >= colIdx) && e.Cell.RowIndex >= this.ResolveStartIndexBasedOnPosition())
                {
                    var rowIndex = -1;
                    var canContinue = false;
                    if (this.Table.HasGroups)
                    {
                        if (this.Table.HasDetailsView && this.DetailsViewRows.ContainsKey(e.Cell.RowIndex))
                        {
                            //Here we dont have background for details view row. so we retrun here.
                            return;
                        }
                        rowIndex = this.ResolveIndexToGroupPosition(e.Cell.RowIndex);
                        var displayEl = rowIndex > -1 && rowIndex < this.Table.GroupModel.DisplayElements.Count ? this.Table.GroupModel.DisplayElements[rowIndex] : null;
                        if (displayEl != null)
                        {
                            canContinue = displayEl is RecordEntry && !(displayEl is SummaryRecordEntry);
                        }
                    }
                    else
                    {
                        rowIndex = this.ResolveIndexToRecordPosition(e.Cell.RowIndex);
                        if (this.View != null && this.View.Records != null)
                        {
                            canContinue = rowIndex > -1 && rowIndex < this.View.Records.Count ? true : false;
                        }
                    }

                    if (!(e.Cell.ColumnIndex < this.ResolveDefaultColumnOffset()))
                    {
                        var colIndex = this.ResolvePositionToVisibleColumnIndex(e.Cell.ColumnIndex);
                        canContinue = colIndex < this.TableProperties.VisibleColumns.Count ? true : false;
                    }
                    else
                    {
                        canContinue = false;
                    }
                    var rowStyles = new GridStyleInfo();
                    var colStyles = new GridStyleInfo();
                    if (canContinue && e.Cell.RowIndex != this.ResolveAddNewPositionInGrid())
                    {
                        this.RowStyles.TryGetValue(e.Cell.RowIndex, out rowStyles);
                        this.ColStyles.TryGetValue(e.Cell.ColumnIndex, out colStyles);
                        if (this.Grid != null && (this.TableProperties.RowBackground != null && this.TableProperties.AlternatingRowBackground != null) || (this.TableProperties.RowForeground != null && this.TableProperties.AlternatingRowForeground != null))
                        {
                            if (rowIndex % this.TableProperties.AlternatingRowCount == 0)
                            {
                                if (rowStyles == null && colStyles == null)
                                {
                                    // if even then apply row style
                                    if((this.TableProperties.RowBackground != null && this.TableProperties.AlternatingRowBackground != null))
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Background = this.TableProperties.RowBackground});
                                    }
                                    if (this.TableProperties.RowForeground != null && this.TableProperties.AlternatingRowForeground != null)
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() {Foreground = this.TableProperties.RowForeground });
                                    }
                                }
                                else
                                {
                                    if((this.TableProperties.RowBackground != null && this.TableProperties.AlternatingRowBackground != null))
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Background = rowStyles == null ? colStyles.Background : rowStyles.Background });
                                    }
                                    if (this.TableProperties.RowForeground != null && this.TableProperties.AlternatingRowForeground != null)
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Foreground = rowStyles == null ? colStyles.Foreground : rowStyles.Foreground });
                                    }                                    
                                }
                               
                            }
                            else
                            {
                                if (rowStyles == null && colStyles == null)
                                {
                                    // if odd then apply alternating row style
                                    if((this.TableProperties.RowBackground != null && this.TableProperties.AlternatingRowBackground != null))
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Background = this.TableProperties.AlternatingRowBackground});
                                    }
                                    if (this.TableProperties.RowForeground != null && this.TableProperties.AlternatingRowForeground != null)
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Foreground = this.TableProperties.AlternatingRowForeground });
                                    }
                                    
                                }
                                else
                                {
                                    if((this.TableProperties.RowBackground != null && this.TableProperties.AlternatingRowBackground != null))
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Background = rowStyles == null ? colStyles.Background : rowStyles.Background});
                                    }
                                    if (this.TableProperties.RowForeground != null && this.TableProperties.AlternatingRowForeground != null)
                                    {
                                        e.BaseStyles.Add(new GridStyleInfo() { Foreground = rowStyles == null ? colStyles.Foreground : rowStyles.Foreground });
                                    }
                                    
                                }
                            }
                        }
                        else if (this.Grid != null && (this.TableProperties.RowBackground != null && this.tableProperties.AlternatingRowBackground == null) || (this.TableProperties.RowForeground != null && this.tableProperties.AlternatingRowForeground == null))
                        {
                            if(this.TableProperties.RowBackground != null && this.tableProperties.AlternatingRowBackground == null)
                            {
                                e.BaseStyles.Add(new GridStyleInfo() { Background = this.TableProperties.RowBackground });
                            }
                            if(this.TableProperties.RowForeground != null && this.tableProperties.AlternatingRowForeground == null)
                            {
                                e.BaseStyles.Add(new GridStyleInfo() { Foreground = this.TableProperties.RowForeground });
                            }
                            
                        }
                        else if (this.Grid != null && (this.tableProperties.AlternatingRowBackground != null && this.TableProperties.RowBackground == null)||(this.tableProperties.AlternatingRowForeground != null && this.TableProperties.RowForeground == null))
                        {                           
                            if (rowIndex % this.TableProperties.AlternatingRowCount != 0)
                            {
                                if(this.TableProperties.AlternatingRowBackground != null && this.TableProperties.RowBackground == null)
                                {
                                    e.BaseStyles.Add(new GridStyleInfo() { Background = this.TableProperties.AlternatingRowBackground});
                                }
                                if(this.TableProperties.AlternatingRowForeground != null && this.TableProperties.RowForeground == null)
                                {
                                    e.BaseStyles.Add(new GridStyleInfo() { Foreground = this.TableProperties.AlternatingRowForeground });
                                }
                                
                            }
                        }
                    }
                }
            }
            // if we have nested tables, then it does have the last column of the parent to, finally just check if the column index is not 0
            if (e.Cell.ColumnIndex > 0 && e.Cell.ColumnIndex == this.ColumnCount - 1)
            {
                if (this.Table.HasNestedTables || this.Table.HasDetailsView)
                {
                   e.BaseStyles.Add(this.EmptyColumnStyle);
                }
            }

            bool isRowHeader = false;
            if (this.TableProperties.ShowRowHeader)
            {
                isRowHeader = e.Cell.ColumnIndex == 0 ? true : false;
            }

            var visibleColIndex = this.ResolvePositionToVisibleColumnIndex(e.Cell.ColumnIndex);
            if (!isRowHeader)
            {
                var headerRow = this.HeaderRows - 1;
                if (visibleColIndex < 0 && e.Cell.RowIndex < headerRow)
                {
                    //Base style will be added for HeaderRow in QueryBaseStyles method itself. So we dont have to add once again here
                    //e.BaseStyles.Add(this.HeaderStyle);
                }
                else if (visibleColIndex == -1 && e.Cell.RowIndex > headerRow)
                {
                    e.BaseStyles.Add(new GridStyleInfo()
                    {
                        Borders = new CellBordersInfo()
                        {
                            Right = this.GetValueCellBorders().Right,
                            Top = new Pen(),
                            Bottom = new Pen()
                        }
                    });
                }
                else if (visibleColIndex < 0 && e.Cell.RowIndex > headerRow)
                {
                    e.BaseStyles.Add(new GridStyleInfo()
                    {
                        Borders = new CellBordersInfo()
                        {
                            Top = new Pen(),
                            Bottom = new Pen()
                        }
                    });
                }
            }
            else
            {
                //Adding BaseStyle for RowHeaderRow
                if (e.Cell.RowIndex > this.HeaderRows - 1 && e.Cell.ColumnIndex == 0 && visibleColIndex < 0)
                    e.BaseStyles.Add(this.IndentColumnStyle);
            }
        }

        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {

            var style = e.Style as GridDataStyleInfo;
            var tableCellIdentity = style.CellIdentity;

            if (this.Table.HasIDataErrorInfo)
            {
                if (tableCellIdentity.TableCellType == GridDataTableCellType.RecordCell && tableCellIdentity.Record != null)
                {
                    var record = tableCellIdentity.Record as IDataErrorInfo;
                    var result = record[tableCellIdentity.Column.MappingName];
#if !SILVERLIGHT
                    if (this.IsLegacyDataTable)
                    {
                        var rowinfo = ((tableCellIdentity.Record as DataRowView).Row) as IDataErrorInfo;
                        if (rowinfo != null)
                            result = rowinfo[tableCellIdentity.Column.MappingName];
                    }
#endif

                    if (result != null && result != string.Empty)
                    {
                        style.ErrorInfo = new GridErrorStyleInfo()
                        {
                            //If we set the ErrorContentAlignment in QueryCellInfo means the value not set
                            //So i have added this code
                            ErrorContentAlignment = style.ErrorInfo.ErrorContentAlignment,
                            ErrorMessage = result,
                            ErrorType = style.ErrorInfo.ErrorType,
                            ImageHeight = new GridLength(0.74d, GridUnitType.Star),
                            ImageMargins = new CellMarginsInfo(0, 3d, 0, 0)
                        };
                    }
                }
            }


            base.OnQueryCellInfo(e);
            if (e.Handled)
            {
                return;
            }
            if (this.TableProperties.QueryCellInfoCommand != null && this.TableProperties.QueryCellInfoCommand.CanExecute(e))
            {
                this.TableProperties.QueryCellInfoCommand.Execute(e);
            }

            if (tableCellIdentity.TableCellType == GridDataTableCellType.RecordCell || tableCellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell)
            {
                if (this.Table.HasConditionalFormats)
                {
                    var record = tableCellIdentity.Record;
                    foreach (var condition in this.TableProperties.ConditionalFormats)
                    {
                        bool result = false;
                        try
                        {
#if !SILVERLIGHT
                            if (this.IsLegacyDataTable)
                            {
                                var recordEntry = tableCellIdentity.RecordEntry;
                                var dataRow = ((DataRowView)recordEntry.Data).Row;
                                var dataRowDelg = condition.GetCompiledDelegateForDataRow();
                                result = (bool)dataRowDelg.DynamicInvoke(dataRow);
                            }
                            else
                            {
#endif
                                try
                                {
                                    string columnName = condition.ApplyStyleToColumn;
                                    if (!string.IsNullOrEmpty(columnName) && columnName.Contains('.'))
                                    {

                                        var delg = condition.GetCompiledDelegate();
                                        result = (bool)delg.DynamicInvoke(new object[] { record });
                                    }

                                    else
                                    {
                                        var delg = condition.GetCompiledDelegate();
                                        result = (bool)delg.DynamicInvoke(new object[] { record });
                                    }
                                }
                                catch (Exception)
                                {
                                    result = false;
                                }
  
#if !SILVERLIGHT
                            }
#endif
                        }
                        catch (Exception ex)
                        {
                            result = false;
                            throw new InvalidOperationException(ex.Message);
                        }

                        if (result)
                        {
                            var styleColumn = condition.ApplyStyleToColumn;

                            //bool hiddenProperty = condition.Style.IsRowHidden;

                            if (styleColumn == string.Empty)
                            {
                                style.ModifyStyle(condition.Style, Syncfusion.Windows.Styles.StyleModifyType.Override);
                            }
                            else if (tableCellIdentity.Column.MappingName == styleColumn)
                            {
                                style.ModifyStyle(condition.Style, Syncfusion.Windows.Styles.StyleModifyType.Override);
                            }
                            
                        }
                    }

                    e.Handled = true;
                }
               
                GridDataRecord gridRecord = tableCellIdentity.RecordEntry;
                if (gridRecord != null)
                {
                    if (gridRecord.HasErrors)
                    {
                        if (gridRecord.ErrorList.ContainsKey(tableCellIdentity.Column.MappingName))
                        {
                            var errorData = gridRecord.ErrorList[tableCellIdentity.Column.MappingName];
                            style.ErrorInfo = new GridErrorStyleInfo()
                            {
                                ErrorMessage = errorData.ErrorMessage,
                                ErrorType = ErrorType.ErrorMessage,
                                ImageHeight = new GridLength(0.74d, GridUnitType.Star),
                                ImageMargins = new CellMarginsInfo(0, 3d, 0, 0)
                            };
                            style.CellValue = errorData.Value;
                        }
                    }
                }
            }
        }

        protected override void OnQueryCoveredRange(GridQueryCoveredRangeEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            if (e.CellRowColumnIndex.RowIndex > -1 && e.CellRowColumnIndex.ColumnIndex > -1)
            {
                GridDataStyleInfo style = null;
#if !SILVERLIGHT
                if (this.Grid != null && !(this.Grid is GridDataCellNestedGridEditor))
                {
                    var renderstyle = this.Grid.GetRenderStyleInfo(e.CellRowColumnIndex);
                    style = renderstyle.ModelStyle as GridDataStyleInfo;
                }
                else
                    style = this[e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex] as GridDataStyleInfo;
#else
                style = this[e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex] as GridDataStyleInfo;
#endif

                var tableStyleIdentity = style.CellIdentity;
                //Processing NestedTable cell and Details view cell 
                if (tableStyleIdentity.TableCellType == GridDataTableCellType.NestedTableCell || tableStyleIdentity.TableCellType == GridDataTableCellType.DetailsViewCell)
                {
                    int nestedcolIndex = 0;
                    if (!this.Table.HasGroups)
                    {
                        nestedcolIndex = this.TableProperties.ShowRowHeader ? 1 : 0;
                        nestedcolIndex = this.TableProperties.ShowRecordPlusMinus ? nestedcolIndex + 1 : nestedcolIndex;
                        nestedcolIndex += this.Table.HasDetailsView && (this.Table.HasNestedTables || !this.TableProperties.ShowRecordPlusMinus) ? 1 : 0;
                    }
                    else
                    {
                        nestedcolIndex = this.ResolveDefaultColumnOffset();
                    }
                    ////var rowIdx = this.ResolveIndexToPosition(e.CellRowColumnIndex.RowIndex);
                    ////rowIdx < this.SourceListCount ? this.Table.Records[rowIdx] : null;
                    var record = style.CellIdentity.RecordEntry;
                    if (record != null && (tableStyleIdentity.TableCellType == GridDataTableCellType.NestedTableCell && record.IsExpanded) ||
                        (tableStyleIdentity.TableCellType == GridDataTableCellType.DetailsViewCell && record.IsDetailsViewExpanded))
                    {
                        //adjusting the span column count based on existance of details view
                        int extraColCount = this.Table.HasNestedTables ? 1 : 1;
                        e.Range = new CoveredCellInfo(
                            e.CellRowColumnIndex.RowIndex,
                            nestedcolIndex,
                            e.CellRowColumnIndex.RowIndex,
                            this.ColumnCount - extraColCount - 1)
                        {
                            SpanWholeRow = true
                        };
                        e.Handled = true;
                    }
                }
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell)
                {
                    var groupLevel = this.TableProperties.ShowRowHeader ? tableStyleIdentity.Group.Level : tableStyleIdentity.Group.Level - 1;
                    //// if we have a caption cell, then create a spanned cell for showing captions from the Group
                    var groupColIdx = this.TableProperties.ShowGroupCaptionPlusMinus ? groupLevel + 1 : groupLevel;
                    var endPointValue = this.Table.HasNestedTables ? 1 : 0;
                    endPointValue += this.Table.HasDetailsView ? 1 : 0;
                    if (groupColIdx <= e.CellRowColumnIndex.ColumnIndex)
                    {
                        e.Range = new CoveredCellInfo(
                            e.CellRowColumnIndex.RowIndex,
                            groupColIdx,
                            e.CellRowColumnIndex.RowIndex,
                            this.ColumnCount - endPointValue - 1)
                        {
                            SpanWholeRow = true
                        };
                        e.Handled = true;
                    }
                }

                else if (tableStyleIdentity.SummaryRow != null && (tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryCoveredCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell))
                {
                    /* tableStyleIdentity.SummaryRow != null && */
                    var endPointValue = this.Table.HasNestedTables ? 2 : 1;
                    endPointValue += this.Table.HasDetailsView ? 1 : 0;
                    e.Range = new CoveredCellInfo(e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex, e.CellRowColumnIndex.RowIndex, this.ColumnCount - endPointValue - 1) { SpanWholeRow = true };

                    if (tableStyleIdentity.SummaryRow.TitleColumnCount > 1 && e.CellRowColumnIndex.ColumnIndex <= tableStyleIdentity.SummaryRow.TitleColumnCount && this.FrozenColumns > 0)
                    {
                        int endrowindex = 0;
                        if (this.TableProperties.ShowRowHeader)
                        {
                            endrowindex = endrowindex + 1;
                        }
                        var grid = this.Grid.FindParentElementOfType<GridDataControl>() as GridDataControl;
                        if (grid != null)
                        {
                            endrowindex = endrowindex + grid.GroupedColumns.Count;
                        }
                        e.Range = new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(e.CellRowColumnIndex.RowIndex, endrowindex, e.CellRowColumnIndex.RowIndex, this.ColumnCount - 1);
                        Console.WriteLine(e.Range.ToString());
                    }

                    e.Handled = true;
                }
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.SummaryTitleCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell)
                {
                    var summaryRow = tableStyleIdentity.SummaryRow;
                    var endPointValue = summaryRow.TitleColumnCount;
                    e.Range = new CoveredCellInfo(e.CellRowColumnIndex.RowIndex, e.CellRowColumnIndex.ColumnIndex, e.CellRowColumnIndex.RowIndex, (e.CellRowColumnIndex.ColumnIndex - 1) + endPointValue);
                    e.Handled = true;
                }
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.StackedColumnHeaderCell)
                {
                    var stackedHeaderCol = tableStyleIdentity.StackHeaderColumn;
                    var stackedHeaderRow = tableStyleIdentity.StackHeaderRow;
                    var startIndex = this.ResolveStartIndexOfStackedHeaderColumn(stackedHeaderRow, stackedHeaderCol, e.CellRowColumnIndex.ColumnIndex);
                    startIndex = this.TableProperties.ShowRowHeader ? startIndex + 1 : startIndex;
                    var endIndex = this.ResolveEndIndexOfStackedHeaderColumn(stackedHeaderRow, stackedHeaderCol);
                    endIndex = this.TableProperties.ShowRowHeader ? endIndex : endIndex - 1;
                    startIndex = this.Table.HasNestedTables ? startIndex + 1 : startIndex;
                    endIndex = this.Table.HasNestedTables ? endIndex + 1 : endIndex;

                    startIndex = this.Table.HasDetailsView ? startIndex + 1 : startIndex;
                    endIndex = this.Table.HasDetailsView ? endIndex + 1 : endIndex;

                    if (this.Table.HasGroups && this.TableProperties.ShowGroupCaptionPlusMinus)
                    {
                        var maxLevel = this.Table.GroupModel.GetMaxLevel();
                        startIndex += maxLevel;
                        endIndex += maxLevel;
                    }

                    e.Range = new CoveredCellInfo(e.CellRowColumnIndex.RowIndex, startIndex, e.CellRowColumnIndex.RowIndex, endIndex) { ClipColumns = true };
                    e.Handled = true;
                }
            }

            base.OnQueryCoveredRange(e);
        }

        protected override void RemoveColumnsCore(int removeAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            base.RemoveColumnsCore(removeAtColumnIndex, count, moveCellsState);
            if (this.IsInSourceListChanged)
            {
                this.TableProperties.SuspendEvents();
                GridDataCurrentRecordMoveState currentRecordMoveState = null;
                var removeAt = this.ResolvePositionToVisibleColumnIndex(removeAtColumnIndex);
                if (removeAt > -1 && removeAt < this.TableProperties.VisibleColumns.Count)
                {
                    if (moveCellsState is GridDataMoveCellsState)
                    {
                        currentRecordMoveState = ((GridDataMoveCellsState)moveCellsState).CurrentRecordMoveState;
                        {
                            currentRecordMoveState.Column = this.TableProperties.VisibleColumns[removeAt];
                        }
                    }
                    this.TableProperties.VisibleColumns.RemoveAt(removeAt);
                    this.InvalidateCell(GridRangeInfo.Col(removeAtColumnIndex));
                }
                this.TableProperties.ResumeEvents();
            }
        }

        protected override void InsertColumnsCore(int insertAtColumnIndex, int count, GridMoveCellsState moveCellsState)
        {
            base.InsertColumnsCore(insertAtColumnIndex, count, moveCellsState);
            if (this.IsInSourceListChanged)
            {
                this.TableProperties.SuspendEvents();
                GridDataCurrentRecordMoveState currentRecordMoveState = null;
                if (moveCellsState is GridDataMoveCellsState)
                {
                    currentRecordMoveState = ((GridDataMoveCellsState)moveCellsState).CurrentRecordMoveState;
                    var insertAt = this.ResolvePositionToVisibleColumnIndex(insertAtColumnIndex);
                    if (insertAt > -1)
                    {
                        this.TableProperties.VisibleColumns.Insert(insertAt, currentRecordMoveState.Column);
                        this.InvalidateCell(GridRangeInfo.Col(insertAtColumnIndex));
                        if (this.Table.HasStackedHeaders)
                        {
                            var range = GridRangeInfo.Rows(0, this.TableProperties.StackedHeaderRows.Count);
                            this.InvalidateCell(range);
                        }
                    }
                }
                this.TableProperties.ResumeEvents();
            }
        }

        internal void RaiseQueryUnboundCellInfo(GridQueryCellInfoEventArgs args)
        {
            if (this.QueryUnboundCellInfo != null)
            {
                this.QueryUnboundCellInfo(this, args);
            }
        }

        internal Type GetUnboundType(string propertyName)
        {
            var source = this.View.SourceCollection;
            var enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }
            var unboundExpressionFunc = this.View as IUnboundExpressionFunc;
            if (unboundExpressionFunc == null)
            {
                return null;
            }

            var typeFunc = this.GetUnboundTypeExpressionFunc(propertyName);
            var checkDelg = typeFunc.Compile();
            // invoking this delegate will return a value, that determines the type of method to be called in the Queryable class.
            var bodyType = (Type)checkDelg.DynamicInvoke(new object[] { propertyName, enumerator.Current });           
            return bodyType;
        }

        #region IValueConverter Func<>
        internal Func<string, object, object> GetConverterFunc(string propertyName)
        {
            var column = this.TableProperties.VisibleColumns.FirstOrDefault(v => v.MappingName == propertyName);
            if (column == null || column.ValueConverter == null)
                return null;

            if(column.ValueConverterFunc == null)
            {
                column.ValueConverterFunc =(columnName,record)=>
                {
                    var result = column.ValueConverter.Convert(record, null, column.ValueConverterParameter ?? column.MappingName, System.Globalization.CultureInfo.CurrentCulture);
                    return result;
                };
            }
            return column.ValueConverterFunc;
        }

        internal Expression<Func<string, object, object>> GetConverterExpressionFunc(string propertyName)
        {
            var recordFunc = this.GetConverterFunc(propertyName);
            if (recordFunc != null)
            {
                return (columnName, record) => recordFunc(columnName, record);
            }
            return null;
        }

        #endregion

        #region UnboundColumns Func<>
        internal Func<string, object, object> GetUnboundFunc(string propertyName)
        {
            var column = this.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
            if (column == null)
            {
                return null;
            }

            if(this.Table.HasUnboundColumns && column.UnboundFunc == null)
            {
                column.UnboundFunc = (columnName, record) =>
                {
                    var result = this.Table.GetUnboundValue(record, columnName);
                    return result ?? string.Empty;
                };
            }
            return column.UnboundFunc;
        }

        private Func<string, object, object> UnboundTypeFunc = null;
        internal Func<string, object, object> GetUnboundTypeFunc(string propertyName)
        {
            var column = this.TableProperties.VisibleColumns.OfType<GridDataVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
            if (column == null)
                return null;
            var itemProperties = ((CollectionViewAdv)this.View).GetItemProperties();
            if (this.Table.HasUnboundColumns && this.UnboundTypeFunc == null)
            {
                this.UnboundTypeFunc = (columnName, record) =>
                   {
                       if (itemProperties != null)
                       {
                           var value = this.GetUnboundFunc(columnName).Invoke(columnName, record);
                           return value.GetType();
                       }
                       return null;
                   };
            }
            return this.UnboundTypeFunc;
        }

        internal Expression<Func<string, object, object>> GetUnboundExpressionFunc(string propertyName)
        {
            return this.GetUnboundExpressionFuncMethod(propertyName);
        }

        protected virtual Expression<Func<string, object, object>> GetUnboundExpressionFuncMethod(string propertyName)
        {
            var recordFunc = this.GetUnboundFunc(propertyName);
            if (recordFunc != null)
            {
                return (columnName, record) => recordFunc(columnName, record);
            }

            return null;
        }

        internal virtual Expression<Func<string, object, object>> GetUnboundTypeExpressionFunc(string propertyName)
        {
            var recordFunc = this.GetUnboundTypeFunc(propertyName);
            if (recordFunc != null)
            {
                return (columnName, record) => recordFunc(columnName, record);
            }

            return null;
        }

        #endregion

        #region Binding Func<>
        /// <summary>
        /// This function used when we use the Binding property in GridDataVisibleColumn
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal Expression<Func<string, object, object>> GetboundExpressionFunc(string propertyName)
        {
            return this.GetboundExpressionFuncMethod(propertyName);
        }

        protected virtual Expression<Func<string, object, object>> GetboundExpressionFuncMethod(string propertyName)
        {
            var recordFunc = this.GetboundFunc(propertyName);
            if (recordFunc != null)
            {
                return (columnName, record) => recordFunc(columnName, record);
            }
            return null;
        }

        internal virtual Expression<Func<string, object, object>> GetboundTypeExpressionFunc(string propertyName)
        {
            var recordFunc = this.GetboundTypeFunc(propertyName);
            if (recordFunc != null)
            {
                return (columnName, record) => recordFunc(columnName, record);
            }
            return null;
        }

        private Func<string, object, object> boundFunc = null;
        internal Func<string, object, object> GetboundFunc(string propertyName)
        {
            var column = this.TableProperties.VisibleColumns.OfType<GridDataVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
            if (column == null)
                return null;

            if (this.boundFunc == null)
            {
                this.boundFunc = (columnName, record) =>
                {
                    var result = ((CollectionViewAdv)this.View).GetPropertyAccessProvider().GetValue(record, columnName);
                    return result ?? string.Empty;
                };
            }
            return this.boundFunc;
        }

        private Func<string, object, object> boundTypeFunc = null;
        internal Func<string, object, object> GetboundTypeFunc(string propertyName)
        {
            var column = this.TableProperties.VisibleColumns.OfType<GridDataVisibleColumn>().FirstOrDefault(v => v.MappingName == propertyName);
            if (column == null)
                return null;

            var itemProperties = ((CollectionViewAdv)this.View).GetItemProperties();
            if (this.boundTypeFunc == null)
            {
                this.boundTypeFunc = (columnName, record) =>
                {
                    if (itemProperties != null)
                    {
                        var value = this.GetboundFunc(columnName).Invoke(columnName, record);
                        if (value != null)
                            return value.GetType(); 

                        var pd = itemProperties.GetPropertyDescriptor(columnName);
                        if (pd != null)
                        {
                            return pd.PropertyType;
                        }
                        
                    }
                    return null;
                };
            }
            return this.boundTypeFunc;
        }

        #endregion

        public event GridDataQueryUnboundColumnCellEventHandler QueryUnboundColumnValue;

        internal void RaiseQueryUnboundValue(RowColumnIndex cell, GridDataStyleInfo style, int recordIndex, GridDataUnboundVisibleColumn unboundColumn, bool setValue)
        {
            GridDataRecord record = null;

            if (recordIndex >= 0)
            {
                if (!this.Table.HasGroups)
                    record = recordIndex < this.View.Records.Count ? (GridDataRecord)this.View.Records[recordIndex] : null;
                else
                    record = recordIndex < this.View.TopLevelGroup.DisplayElements.Count ? (GridDataRecord)this.View.TopLevelGroup.DisplayElements[recordIndex] : null;
            }

            var args = new GridDataQueryUnboundColumnCellEventArgs()
            {
                Cell = cell,
                Style = style,
                RecordEntry = record,
                RecordIndex = recordIndex,
                UnboundColumn = unboundColumn
            };
            var handler = this.QueryUnboundColumnValue;
            if (handler != null)
            {
                handler(this, args);
            }

            if (!args.Handled)
            {
                if (record != null && unboundColumn != null && setValue)
                    style.CellValue = this.Table.GetUnboundValueForEvent(record.Data, unboundColumn);
            }
        }

        internal bool ValidateColumns()
        {
            var count = this.TableProperties.ShowRowHeader ? this.TableProperties.VisibleColumns.Count + 1 : this.TableProperties.VisibleColumns.Count;
            count = this.Table.HasNestedTables ? count + 1 : count;
            count = this.Table.HasDetailsView ? count + 1 : count;
            if (this.Table.HasNestedTables || this.Table.HasDetailsView)
            {
                // this is the last invisible column
                count = count + 1;
            }

            if (this.Table.HasGroups)
            {
                int maxLevel = 0;
                if (this.TableProperties.ShowGroupCaptionPlusMinus)
                {
                    maxLevel = this.Table.GroupModel.GetMaxLevel();
                    count = count + maxLevel;
                }
            }

            return this.ColumnCount == count;
        }

        public GridDataControlColumnSizer ColumnAutoSizer
        {
            get
            {
                var sizer = this.Sizer as GridDataControlColumnSizer;
                return sizer;
            }
        }

        protected override GridColumnAutoSizer CreateAutoSizer()
        {
            return new GridDataControlColumnSizer(this);
        }

        protected override void OnUpdateAutoSizer(bool applySizes, bool columnSizerChanged)
        {
            if (columnSizerChanged)
            {
                if (this.Options.ColumnSizer != GridControlLengthUnitType.None && this.Options.ColumnSizer != GridControlLengthUnitType.AutoWithLastColumnFill)
                {
                    this.TableProperties.VisibleColumns.ForEach(d =>
                       {
                           d.Width = new GridDataControlLength(1.0d, this.Options.ColumnSizer);
                       });
                }
            }

            base.OnUpdateAutoSizer(false, false);
        }

        public bool IsInColumnRefresh
        {
            get;
            private set;
        }

        internal void RefreshColumns(bool setColumnCount, bool shouldReset)
        {
            this.IsInColumnRefresh = true;
            if (this.TableProperties.VisibleColumns.Count == 0)
            {
                this.Reset();               
                this.ColumnCount = this.TableProperties.ShowRowHeader ? 1 : 0;
                return;
            }

            if (shouldReset)
            {
                this.Reset();
            }

            var count = this.TableProperties.ShowRowHeader ? this.TableProperties.VisibleColumns.Count + 1 : this.TableProperties.VisibleColumns.Count;
            count += this.Table.HasNestedTables ? 1 : 0;
            count += this.Table.HasDetailsView ? 1 : 0;

            if (this.TableProperties.ShowRowHeader)
            {
                this.ColumnWidths[0] = this.TableProperties.RowHeaderWidth;
                this.HeaderColumns = this.TableProperties.HeaderColumns + 1;
            }
            else
            {
                this.HeaderColumns = this.TableProperties.HeaderColumns;
            }

            // if we have nested tables, then we just add an extra delta column to adjust the width 
            if (this.Table.HasNestedTables || this.Table.HasDetailsView)
            {
               count = count + 1;
            }

            if (!this.Table.HasGroups)
            {
                if (setColumnCount)
                {
                    if (this.ColumnCount > count && this.CurrencyManager.CurrentCell.ColumnIndex == count)
                        this.CurrencyManager.CurrentCell.MoveTo(CurrencyManager.CurrentCell.RowIndex, count - 1);

                    this.ColumnCount = count;
                }
                for (int i = 0; i < this.TableProperties.VisibleColumns.Count; i++)
                {
                    var column = this.TableProperties.VisibleColumns[i];
                    if (column.IsHidden)
                    {
                        continue;
                    }

                    var colIdx = i + this.ResolveDefaultColumnOffset();
                    //Below condition is added for the performance issue in loading.
                    if (this.ColumnWidths[colIdx] != column.ActualWidth)
                    {
                        this.ColumnWidths[colIdx] = column.ActualWidth;
                    }
                }

                if (this.Table.HasNestedTables)
                {
                    var colIdx = this.TableProperties.ShowRowHeader ? 1 : 0;
                    this.ColumnWidths[colIdx] = GridDataTableModel.ExpandCollapseCellWidth;
                }
                if (this.Table.HasDetailsView)
                {
                    var colIdx = this.TableProperties.ShowRowHeader ? 1 : 0;
                    colIdx += this.Table.HasNestedTables ? 1 : 0;
                    this.ColumnWidths[colIdx] = GridDataTableModel.ExpandCollapseCellWidth;
                }
            }
            else
            {
                int maxLevel = this.TableProperties.ShowGroupCaptionPlusMinus ? this.Table.GroupModel.GetMaxLevel() : 0;
                count = count + maxLevel;
                if (setColumnCount)
                {
                    this.ColumnCount = count;
                }
                if (this.TableProperties.ShowGroupCaptionPlusMinus)
                {
                    maxLevel += this.Table.HasNestedTables ? 1 : 0;
                    maxLevel += this.Table.HasDetailsView ? 1 : 0;
                    var startIdx = this.TableProperties.ShowRowHeader ? 1 : 0;
                    for (int i = startIdx; i <= maxLevel; i++)
                    {
                        this.ColumnWidths[i] = GridDataTableModel.ExpandCollapseCellWidth;
                    }
                }

                for (int i = 0; i < this.TableProperties.VisibleColumns.Count; i++)
                {
                    var column = this.TableProperties.VisibleColumns[i];
                    if (column.IsHidden)
                    {
                        continue;
                    }

                    var colIdx = i + this.ResolveDefaultColumnOffset();
                    this.ColumnWidths[colIdx] = column.ActualWidth;//((GridDataControlBaseImpl)this.Grid).GridDataColumnSizer.ApplyColumnSizer(column);
                }
            }

            if (this.Table.HasNestedTables || this.Table.HasDetailsView)
            {
               this.ColumnWidths[this.ColumnCount - 1] = 0;
            }

            this.Table.RefreshHiddenColumns();
            this.IsInColumnRefresh = false;
            this.ColumnAutoSizer.SetStarWidth();

            var handler = this.ColumnsReset;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raised whenever the columns is reset. This occurs when data is grouped / ungrouped.
        /// </summary>
        public event EventHandler ColumnsReset;

        internal void Reset()
        {
            if (this.VolatileCellStyles != null)
                this.VolatileCellStyles.Clear();
            if (this.Data != null)
                this.Data.Clear();
            if (this.CoveredCells != null)
                this.CoveredCells.Clear();
#if Silverlight
            this.ImageCells.Clear();
#endif
            this.CellSpanBackgrounds.Clear();
        }

        public void SuspendEvents()
        {
            if (!this.IsInSuspend)
            {
                this.IsInSuspend = true;
            }
        }

        public bool IsInSuspend
        {
            get;
            private set;
        }

        public void ResumeEvents()
        {
            if (this.IsInSuspend)
            {
                this.IsInSuspend = false;
            }
        }

        /// <summary>
        /// Refreshes the display for the grid clearing Cached Cell Styles, Recalculating Scrollbars and Repainting the 
        /// whole control.
        /// </summary>
        internal void RefreshDisplay(bool needsRefreshTable)
        {
            this.RefreshDisplayMethod(needsRefreshTable);
        }

        protected void RefreshDisplayMethod(bool needsRefreshTable)
        {
            this.RefreshSourceListCount();
            if (needsRefreshTable && this.Table != null)
            {
                this.Table.Refresh();
                this.InvalidateVisual(true);
            }
        }

        internal void SetDirty()
        {
            if (this.IsInitialized)
            {
                this.SetTableDirty();
                if (this.Grid != null)
                {
                    this.Grid.InvalidateCells();
                }
            }
        }

        internal void SetSourceList(object itemsSource, bool changeOriginalSource, bool needsWholeGridRefresh, ICollectionViewAdv collectionView)
        {
            if (itemsSource != null && this.SourceList != null && !this.TableProperties.OneTimePopulateRelations)
            {
                ResetItemsSource();
            }

            if (itemsSource != null)
            {
                var source = this.GetSourceList(itemsSource);
                if (changeOriginalSource && source != null)
                {
                    this.SourceList = source;
                    if (collectionView == null)
                    {
                        this.View = this.CreateCollectionViewAdv(source);
                        if (this.View is CollectionViewAdv)
                        {
#if !SILVERLIGHT
                            ((CollectionViewAdv)this.View).DispatchOwner = this.Grid != null ? this.Grid.Dispatcher : null;
#endif
                            ((CollectionViewAdv)this.View).SortingOptions = this.TableProperties.SortingOptions;
                        }
                    }
                    else
                    {
                        this.View = collectionView;
                    }

                    //This is for paging support                    
                    this.View.EnablePaging = this.TableProperties.EnablePaging;
                    this.View.IsViewLevelPaging = this.TableProperties.IsViewLevelPaging;
                    this.View.PagedSource = itemsSource as PagedCollectionView;
                   
#if SILVERLIGHT
                    this.TableProperties.VisibleColumns.SetTableModel(this);
#endif
#if !SILVERLIGHT
                    if (this.TableProperties.IsSynchronizedWithCurrentItem)
                    {
                        var externalView = CollectionViewSource.GetDefaultView(this.View.SourceCollection);
                        this.View.AddListener(externalView);
                        var cView = this.View as CollectionViewAdv;
                        cView.ExternalCollectionView.MoveCurrentTo(this.View.CurrentItem);
                    }
#endif

                    this.View.PersistGroupsExpandedState = this.TableProperties.PersistGroupsExpandState;
                    this.View.IsGroupsExpanded = this.TableProperties.IsGroupsExpanded;
                    this.View.GroupComparer = this.TableProperties.CustomGroupComparer;
                    
                    var view = this.View;
                    using (view.DeferRefresh())
                    {
                        // if there is a custom expression set it before the actions
                        if (this.TableProperties.ExpressionFunc != null)
                        {
                            view.SetCustomExpressionFunc(this.TableProperties.ExpressionFunc);
                        }

                        this.TableProperties.GroupedColumns.ForEach<GridDataGroupColumn>(g =>
                            {
                                //this.View.CustomGroupComparers.Add(g.ColumnName, g.Comparer);
                                this.View.GroupDescriptions.Add(new PropertyGroupDescription() { PropertyName = g.ColumnName, Converter = g.Converter });
                                if (this.TableProperties.HideColumnsWhenGrouped)
                                {
                                    var visibleColumn = this.TableProperties.VisibleColumns.FirstOrDefault(v => v.MappingName == g.ColumnName);
                                    if (visibleColumn != null)
                                    {
                                        visibleColumn.IsInSuspend = true;
                                        visibleColumn.IsHidden = true;
                                        visibleColumn.IsInSuspend = false;
                                    }
                                }

                                if (this.TableProperties.SortWhenGrouped)
                                {
                                    var sortColumn = this.TableProperties.GetSortColumnForGroup(g);
                                    if (sortColumn == null)
                                    {
                                        this.View.SortDescriptions.Add(new SortDescription() { PropertyName = g.ColumnName, Direction = ListSortDirection.Ascending });
                                        this.TableProperties.SortColumns.Add(new GridDataSortColumn() { ColumnName = g.ColumnName, SortDirection = ListSortDirection.Ascending });
                                    }
                                    else
                                    {
                                        this.View.SortDescriptions.Add(new SortDescription() { PropertyName = sortColumn.ColumnName, Direction = sortColumn.SortDirection });
                                    }
                                }
                            });
                        foreach (var s in this.TableProperties.SortColumns)
                        {
                            var sortColumn = this.View.SortDescriptions.FirstOrDefault(d => d.PropertyName == s.ColumnName);
                            var notFound = !(sortColumn != null && sortColumn != default(SortDescription));

                            if (notFound)
                            {
                                this.View.SortDescriptions.Add(new SortDescription() { PropertyName = s.ColumnName, Direction = s.SortDirection });
                                if (s.CustomComparer != null)
                                {
                                    this.View.SortComparers.Add(s.ColumnName, s.CustomComparer);
                                }
                            }
                        }
                        //var filters = this.TableProperties.VisibleColumns.OfType<IFilterDefinition, GridDataVisibleColumn>();
                        //if (filters.Count > 0)
                        //{
                        //    //This is for paging Support
                        //    if(!this.TableProperties.EnablePaging||(this.TableProperties.EnablePaging&&this.TableProperties.IsViewLevelPaging))
                        //         this.View.FilterPredicates = filters;
                        //}
                    }

                    this.CurrencyManager.CurrentRecordSelectionChanged -= this.OnCurrentRecordSelectionChanged;
                    this.WireView(this.View);
                    this.CurrencyManager.CurrentRecordSelectionChanged += this.OnCurrentRecordSelectionChanged;
                    this.WireSourceList(needsWholeGridRefresh);
                    var allowFiltersVisibleColumn = this.TableProperties.VisibleColumns.FirstOrDefault(column => column.AllowFilter == true);
                    var filterpaneColumn = this.TableProperties.VisibleColumns.FirstOrDefault(column => column.FilterPane != null);
                    if (allowFiltersVisibleColumn != null && filterpaneColumn == null && !allowFiltersVisibleColumn.IsAdvancedFilteringMode && !this.TableProperties.ShowFilterBar)
                        (this.View as IExcelLikeFilterExt).IsExcelLikeFilter = true;
                    else
                        (this.View as IExcelLikeFilterExt).IsExcelLikeFilter = false;
                    
                    if (this.ColumnAutoSizer != null)
                    {
                        if (changeOriginalSource)
                        {
                            this.ColumnAutoSizer.IsAutoOnLoad = false;
                        }
                        this.ColumnAutoSizer.RefreshAll();
                    }
                }
                else
                {
                    // sometimes the sourcelist would be null, we don't display anything here
                    this.RowCount = 0;
                }
            }
            else if (this.SourceList != null)
            {
                // reset 
                if (this.TableProperties.AutoPopulateColumns)
                    this.TableProperties.VisibleColumns.Dispose();
                ResetItemsSource();
                this.FrozenRows = 0;
                this.FrozenColumns = 0;
                this.FooterRows = 0;
                this.FooterColumns = 0;
                this.RowCount = 1;
                if (this.TableProperties.AutoPopulateColumns)
                    this.ColumnCount = 1;
            }
        }

        private void ResetItemsSource()
        {
#if !SILVERLIGHT
            if (this.Table.HasNestedTables)
            {
                //resets the nestedlines heights when changing the itemsource.
                LineSizeCollection lines = this.RowHeights as LineSizeCollection;
                lines.ResetNestedLines();
            }
#endif
            this.SourceList = null;
            this.SourceListCount = 0;
            sourceListsKey.Clear();
            this.Table.Refresh();
            this.RefreshColumns(true, true);

            if (this.TableProperties.ConditionalFormats != null)
            {
                foreach (var conditionalFormat in this.TableProperties.ConditionalFormats)
                    conditionalFormat.ResetCompiledDelegate();
            }

            this.IsSourceReset = true;
            var disposableView =this.View as IDisposable;
            if (disposableView != null)
            {
                this.UnwireView(this.View);
                disposableView.Dispose(); // clearing source throws exception while reseting the item source. 
                this.View = null;
            }
            if (this.ChildTableModelCollection != null)
            {
                foreach (var childmodel in this.ChildTableModelCollection)
                {
                    //var disposableChildView = childmodel.View as IDisposable;
                    //if (disposableChildView != null)
                    //{
                    //    childmodel.UnwireView(childmodel.View);
                    //    disposableChildView.Dispose(); // clearing source throws exception while reseting the item source. 
                    //    childmodel.View = null;
                    //}
                }
                ChildTableModelCollection.Clear();
            }
            this.IsSourceReset = false;
            this.CurrencyManager.ResetCache();
            this.CurrencyManager.Reset();
        }

        private void WireView(ICollectionViewAdv view)
        {
            view.CollectionChanged += this.OnCollectionChanged;
            view.CurrentChanged += this.OnViewCurrentChanged;
            var viewPropertyNotifyChanged = view as INotifyPropertyChanged;
            this.SetNotifyPropertyChanged(this.TableProperties.NotifyPropertyChanges);
            if (viewPropertyNotifyChanged != null)
            {
                viewPropertyNotifyChanged.PropertyChanged += new PropertyChangedEventHandler(viewPropertyNotifyChanged_PropertyChanged);
            }

            ((INotifyCollectionChanged)view.SortDescriptions).CollectionChanged += new NotifyCollectionChangedEventHandler(ViewSortDescriptionsChanged);
            view.GroupDescriptions.CollectionChanged += new NotifyCollectionChangedEventHandler(ViewGroupDescriptionsCollectionChanged);
        }

        private void ViewSortDescriptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInSort || this.IsInSuspend || this.TableProperties.inSortColumnsChanged)
            {
                return;
            }

            this.TableProperties.SuspendEvents();
            var view = this.View;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SortDescription sortItem in e.NewItems)
                    {
                        var hasSortItem = this.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == sortItem.PropertyName) != null;
                        if (!hasSortItem)
                        {
                            this.TableProperties.SortColumns.Insert(view.SortDescriptions.IndexOf(sortItem), new GridDataSortColumn() { ColumnName = sortItem.PropertyName, SortDirection = sortItem.Direction });
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SortDescription sortItem in e.OldItems)
                    {
                        var modelSortItem = this.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == sortItem.PropertyName);
                        if (modelSortItem != null)
                        {
                            this.TableProperties.SortColumns.Remove(modelSortItem);
                        }
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.TableProperties.SortColumns.Clear();
                    break;
            }

            this.InvalidateCell(GridRangeInfo.Row(this.TableProperties.StackedHeaderRows.Count));
            this.TableProperties.ResumeEvents();
        }

        private void ViewGroupDescriptionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.IsInGroup || this.IsInSuspend || this.TableProperties.inGroupColumnsChanged)
            {
                return;
            }

            this.TableProperties.SuspendEvents();
            var view = this.View;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (PropertyGroupDescription groupItem in e.NewItems)
                    {
                        var hasSortItem = this.TableProperties.GroupedColumns.FirstOrDefault(s => s.ColumnName == groupItem.PropertyName) != null;
                        if (!hasSortItem)
                        {
                            this.TableProperties.GroupedColumns.Insert(view.GroupDescriptions.IndexOf(groupItem), new GridDataGroupColumn() { ColumnName = groupItem.PropertyName });
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (PropertyGroupDescription sortItem in e.OldItems)
                    {
                        var modelGroupItem = this.TableProperties.GroupedColumns.FirstOrDefault(s => s.ColumnName == sortItem.PropertyName);
                        if (modelGroupItem != null)
                        {
                            this.TableProperties.GroupedColumns.Remove(modelGroupItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.TableProperties.GroupedColumns.Clear();
                    break;
            }

            this.RefreshColumns(true, true);
            this.InvalidateCell(GridRangeInfo.Row(this.TableProperties.StackedHeaderRows.Count));
            this.TableProperties.ResumeEvents();
        }

        private void UnwireView(ICollectionViewAdv view)
        {
            view.CollectionChanged -= this.OnCollectionChanged;
            view.CurrentChanged -= this.OnViewCurrentChanged;
            var viewPropertyNotifyChanged = view as INotifyPropertyChanged;
            this.SetNotifyPropertyChanged(false);
            if (viewPropertyNotifyChanged != null)
            {
                viewPropertyNotifyChanged.PropertyChanged -= new PropertyChangedEventHandler(viewPropertyNotifyChanged_PropertyChanged);
            }

            ((INotifyCollectionChanged)view.SortDescriptions).CollectionChanged -= new NotifyCollectionChangedEventHandler(ViewSortDescriptionsChanged);
            view.GroupDescriptions.CollectionChanged -= new NotifyCollectionChangedEventHandler(ViewGroupDescriptionsCollectionChanged);
        }

        private bool isNotifyPropertyChangedWired = false;

        internal void SetNotifyPropertyChanged(bool wire)
        {
            if (this.View == null)
            {
                return;
            }

            if (wire)
            {
                if (!isNotifyPropertyChangedWired)
                    this.View.RecordPropertyChanged += OnPropertyChanged;
            }
            else
                this.View.RecordPropertyChanged -= OnPropertyChanged;
            this.isNotifyPropertyChangedWired = wire;
        }

        void viewPropertyNotifyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.TableProperties.AutoPopulateColumns && e.PropertyName == "ItemProperties")
            {
                this.WireSourceList(true);
            }
        }

        public virtual IEnumerable GetSourceList(object source)
        {
            IEnumerable result = null;
            if (source != null)
            {
#if !SILVERLIGHT
                if (source is DataTable)
                {
                    result = ((DataTable)source).DefaultView;
                }
                else if (source is DataView)
                {
                    result = source as DataView;
                }
                else if (source is CollectionViewSource)
                {
#else
                if (source is CollectionViewSource)
                {
#endif
                    var cvs = source as CollectionViewSource;
                    if (cvs.View != null)
                    {
                        result = GetSourceList(cvs.View.SourceCollection);
                    }
                }

                else if (source is PagedCollectionView)
                {
                    if (source is INotifyCollectionChanged)
                    {
                        var notifyPagedCollection = source as INotifyCollectionChanged;
                        notifyPagedCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(notifyPagedCollection_CollectionChanged);
                    }

                    result = ((PagedCollectionView)source).SourceCollection;
                }


                else if (source is ICollectionView)
                {
                    var sourceList = ((ICollectionView)source).SourceCollection;
                    result = GetSourceList(sourceList);
                }
#if !SILVERLIGHT
                else
                {
                    result = ((IEnumerable)source).ToTypedSource(this.TableProperties.SourceType);
                    // sometimes the non-generic source would be null initially, we simply check for IBindingList / INotifyCollectionChanged based interfaces
                    if (result == null)
                    {
                        if (source is IBindingList)
                        {
                            var bindingList = source as IBindingList;
                            bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
                        }
                        else if (source is INotifyCollectionChanged)
                        {
                            var notifyCollection = source as INotifyCollectionChanged;
                            notifyCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(notifyCollection_CollectionChanged);
                        }
                    }
                }
#else
                else
                {
                    result = source as IEnumerable;
                }
#endif
            }

            return result;
        }

        void notifyPagedCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            var notifyCollection = sender as INotifyCollectionChanged;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                notifyCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(notifyPagedCollection_CollectionChanged);
                this.SetSourceList(sender,true,true,null);//  (sender, true, true, null);

            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (!this.IsInGroup)
                {
                    notifyCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(notifyPagedCollection_CollectionChanged);
                    using (this.View.DeferRefresh())
                    {
                        this.SourceList = this.GetSourceList(sender);
                        (this.View as GridDataQueryableCollectionViewWrapper).SetViewSource(this.SourceList);
                    }
                }  
            }

            if (!this.RefreshFromFilter)
                this.InvalidateDisplay();
        }

#if !SILVERLIGHT
        void notifyCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var notifyCollection = sender as INotifyCollectionChanged;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                notifyCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(notifyCollection_CollectionChanged);
                this.SetSourceList(sender, true, true, null);
            }
        }

        void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            var bindingList = sender as IBindingList;
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
                this.SetSourceList(sender, true, true, null);
            }
        }
#endif

        public static ICollectionViewAdv GetDefaultView(IEnumerable source)
        {
            if (!sourceListsKey.ContainsKey(source))
            {
                var view = emptyModel.CreateCollectionViewAdv(source);
                sourceListsKey.Add(source, view);
            }


            return sourceListsKey[source];
        }

        internal ObservableCollection<GridDataChildTableModel> ChildTableModelCollection { get; set; }

        //Following for to maintain filter persistence in excelLikeFiltering.
        
        /// <summary>
        /// Gets or sets the removed filter predicate before loaded. If FilterPredicate applied in the sampe it should load in the Excellike DropDown. If Same record filtered by many column then it should prefer first column, other column filter should remove. To check that we have maintaing this.
        /// </summary>
        /// <value>The removed filter predicate before loaded.</value>
        internal List<RecordEntry> RemovedFilterPredicateBeforeLoaded
        {
            get;
            set;
        }

        /// <summary>
        /// To maintain the Filtered record which are applid in the sample.
        /// </summary>
        internal List<RecordEntry> FilterPredicateBeforeLoaded
        {
            get;
            set;
        }

    

        public virtual ICollectionViewAdv CreateCollectionViewAdv(IEnumerable source)
        {
            ICollectionViewAdv view = null;
            if (source != null)
            {
#if !SILVERLIGHT
                if (!this.CheckIsLegacyDataTable(source))
                {
#endif
                    {
                        view = new GridDataQueryableCollectionViewWrapper(source, this, (rec, parentView) =>
                        {
                           var record = new GridDataRecord(rec, this.gridDataTable);
#if !SILVERLIGHT
                           if (this.grid != null && this.grid.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                           {
                               this.grid.Dispatcher.Invoke(new Action(() =>
                                {
                                    CreateNestedCollection(record, parentView);
                                }));
                               return record;
                           }
                           else if (this is GridDataChildTableModel)
                           {
                               var childmodel = this as GridDataChildTableModel;
                               if (childmodel.ParentRecord != null && childmodel.ParentRecord.Table != null && childmodel.ParentRecord.Table.Model.grid != null
                                   && childmodel.ParentTable.Model.Grid.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                               {
                                   childmodel.ParentTable.Model.Grid.Dispatcher.Invoke(new Action(() =>
                                   {
                                       CreateNestedCollection(record, parentView);
                                   }));
                                   return record;
                               }
                           }
#endif
                            CreateNestedCollection(record, parentView);
                            return record;
                        });
#if !SILVERLIGHT && SyncfusionFramework4_0
                        ((GridDataQueryableCollectionViewWrapper)view).UsePLINQ = this.TableProperties.UsePLINQ;
#endif
                        ((GridDataQueryableCollectionViewWrapper)view).InternalSetSourceType(this.TableProperties.SourceType);
                        ((GridDataQueryableCollectionViewWrapper)view).NotifyComplexPropertyChanges = this.TableProperties.NotifyComplexPropertyChanges;
                    }
#if !SILVERLIGHT
                }
                else
                {
                    view = new GridDataTableCollectionViewWrapper(source, this, (rec, parentView) =>
                    {
                        var record = new GridDataRecord(rec, this.gridDataTable);
#if !SILVERLIGHT
                        if (this.grid != null && this.grid.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                        {
                            this.grid.Dispatcher.Invoke(new Action(() =>
                            {

                                CreateDataTableNestedCollection(record, parentView);
                            }));
                            return record;
                        }
                        else if (this is GridDataChildTableModel)
                        {
                            var childmodel = this as GridDataChildTableModel;
                            if (childmodel.ParentRecord != null && childmodel.ParentRecord.Table != null && childmodel.ParentRecord.Table.Model.grid != null
                                && childmodel.ParentTable.Model.Grid.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                            {
                                childmodel.ParentTable.Model.Grid.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    CreateDataTableNestedCollection(record, parentView);
                                }));
                                return record;
                            }
                        }
#endif
                        CreateDataTableNestedCollection(record, parentView);
                        return record;
                    });

                    ((GridDataTableCollectionViewWrapper)view).InternalSetSourceType(this.TableProperties.SourceType);
                    ((GridDataTableCollectionViewWrapper)view).NotifyComplexPropertyChanges = this.TableProperties.NotifyComplexPropertyChanges;
                }
#endif


                if (this.TableProperties.CaptionSummaryRow != null)
                {
                    //Reset the SummaryColumns for CaptionSummaryRow when changing the itemsource at runtime.
                    if (this.TableProperties.SummaryColumns != null && this.TableProperties.SummaryColumns.Count > 0)
                    {
                        foreach (ISummaryColumn col in this.TableProperties.SummaryColumns)
                        {
                            if (!(this.TableProperties.CaptionSummaryRow.SummaryColumns.Contains(col)))
                            {
                                this.TableProperties.CaptionSummaryRow.SummaryColumns.Add(col);
                            }
                        }
                    }
                    view.CaptionSummaryRow = this.TableProperties.CaptionSummaryRow;
                }
            }

            if (!sourceListsKey.ContainsKey(source))
            {
                sourceListsKey.Add(source, view);
            }

            return view != null ? view : GridDataTableModel.GetDefaultView(source);
        }

        
        private void CreateNestedCollection(object record, ICollectionViewAdv parentView)
        {
            if (this.TableProperties.OneTimePopulateRelations && this.TableProperties.Relations.Count > 0)
            {
                int key = 0;
                foreach (var rd in this.TableProperties.Relations)
                {
                    key++;
                    var gridRecord = record as GridDataRecord;
                    gridRecord.ChildViews = new Dictionary<string, NestedRecordEntry>();
                    var childModel = new GridDataChildTableModel(rd);

                    // set the sync properties
                    childModel.ParentRecord = gridRecord;
                    childModel.TableProperties = new GridDataTableProperties();
                    childModel.TableProperties.SuspendEvents();
                    childModel.TableProperties.InitializeFrom(rd.TableProperties);
                    childModel.TableProperties.ResumeEvents();
                    // add the key based on the order. This would be easy to calculate when QueryCellInfo is called for rendering
#if !SILVERLIGHT
                    childModel.TableStyle.FlowDirection = this.TableStyle.FlowDirection;
                    var childSource = gridRecord.GetChildSource(rd.RelationalColumn, this.IsLegacyDataTable, parentView);
#else
                    var childSource = gridRecord.GetChildSource(rd.RelationalColumn, false, parentView);
#endif
                    if (childSource != null)
                    {
                        var collectionview = childModel.CreateCollectionViewAdv(childSource);
                        gridRecord.PopulateChildView(collectionview, key, rd.RelationalColumn);
                        childModel.View = collectionview;
                        childModel.TableProperties.VisualStyle = this.TableProperties.VisualStyle;
                        gridRecord.ChildModels.Add(key - 1, childModel);
                    }
                }
            }
        }

#if !SILVERLIGHT

        private void CreateDataTableNestedCollection(object record, ICollectionViewAdv parentView)
        {
            if (this.TableProperties.OneTimePopulateRelations && this.TableProperties.Relations.Count > 0)
            {
                int key = 0;
                foreach (var rd in this.TableProperties.Relations)
                {
                    key++;
                    var gridRecord = record as GridDataRecord;
                    gridRecord.ChildViews = new Dictionary<string, NestedRecordEntry>();
                    var childModel = new GridDataChildTableModel(rd);

                    // set the sync properties
                    childModel.ParentRecord = gridRecord;
                    childModel.TableProperties = new GridDataTableProperties();
                    childModel.TableProperties.SuspendEvents();
                    childModel.TableProperties.InitializeFrom(rd.TableProperties);
                    childModel.TableProperties.ResumeEvents();

                    // add the key based on the order. This would be easy to calculate when QueryCellInfo is called for rendering
                    var childSource = gridRecord.GetChildSource(rd.RelationalColumn, this.IsLegacyDataTable, parentView);
                    var collectionview = childModel.CreateCollectionViewAdv(childSource);
#if !SILVERLIGHT
                    if (this.View is CollectionViewAdv)
                    {
                        ((CollectionViewAdv)collectionview).DispatchOwner = this.Grid != null ? this.Grid.Dispatcher : null;
                    }
#endif
                    gridRecord.PopulateChildView(collectionview, key, rd.RelationalColumn);
                    childModel.View = collectionview;
                    childModel.TableProperties.VisualStyle = this.TableProperties.VisualStyle;
                    gridRecord.ChildModels.Add(key - 1, childModel);
                }
            }
        }

        internal bool CheckIsLegacyDataTable(IEnumerable source)
        {
            return source is DataTable || source is DataView;
        }

        internal bool IsLegacyDataTable
        {
            get
            {
                return this.SourceList is DataView || this.SourceList is DataTable;
            }
        }
#endif
        public void SetTableDirty()
        {
            this.IsInitialized = false;
        }

       private int _sortColumnVisibleIndexPosition = -1;
        
         internal int SortColumnVisibleIndexPosition
         {

            get { return _sortColumnVisibleIndexPosition; }
            set { _sortColumnVisibleIndexPosition = value; }
         }

        internal bool isSortingApplied;
        protected virtual void SortColumnMethod(GridDataVisibleColumn column)
        {
            if (this.View.IsEditingItem)
            {
                this.View.CancelEdit();
            }

            if (this.SourceListCount > 0 && column != null && column.AllowSort)
            {
                this.Grid.Focus();
                this.SortColumnVisibleIndexPosition = this.TableProperties.VisibleColumns.IndexOf(column);
                LineSizeCollection rowHeights = (LineSizeCollection)this.RowHeights;
                rowHeights.SuspendUpdates();
                
                this.IsInSort = true;
                this.Table.HideAllUIRows();
                //this.TableProperties.UnwireEvents();
                this.View.BeginInit();
                var sortColumName = column.SortMemberPath ?? column.MappingName;
                //if (!this.SortInGroupedColumn(column))
                //{
                // Only if you have the Ctrl key pressed, allow multi sort
#if !SILVERLIGHT
                bool allowMultiSort = Keyboard.Modifiers.Equals(System.Windows.Input.ModifierKeys.Control) | Keyboard.Modifiers.Equals(System.Windows.Input.ModifierKeys.Shift);
#else
                bool allowMultiSort = Keyboard.Modifiers.Equals(System.Windows.Input.ModifierKeys.Control);
#endif
                if (this.TableProperties.SortColumns.Count >= 1 && allowMultiSort)
                {

                    var sortedColumn = this.TableProperties.SortColumns.Where(s => s.ColumnName == sortColumName).FirstOrDefault();

                    if (sortedColumn == null)
                    {
                        // we dint find any sort column
                        var newSortColumn = new GridDataSortColumn() { ColumnName = sortColumName, SortDirection = ListSortDirection.Ascending };
                        if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { newSortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add)) // adding new column
                        {
                            this.TableProperties.SortColumns.Add(newSortColumn);
                            this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { newSortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add);
                        }
                    }
                    else
                    {
                        if (sortedColumn.SortDirection == ListSortDirection.Descending && this.TableProperties.EnableTriStateSorting)
                        {
                            GridDataSortColumn removedSortColumn = this.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == sortedColumn.ColumnName);
                            if (removedSortColumn != null)
                            {
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>(), new List<GridDataSortColumn>() { removedSortColumn }, NotifyCollectionChangedAction.Remove)) // removing column in tristate
                                {
                                    this.TableProperties.SortColumns.Remove(removedSortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>(), new List<GridDataSortColumn>() { removedSortColumn }, NotifyCollectionChangedAction.Remove);
                                }
                            }
                        }
                        else
                        {
                            sortedColumn.SortDirection = sortedColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;                           
                            if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortedColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace)) // Replacing the same column
                            {
                                this.TableProperties.SortColumns.Remove(sortedColumn);
                                this.TableProperties.SortColumns.Add(sortedColumn);
                                this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortedColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace);
                            }
                        }
                    }
                }
                else
                {
                    var currentSortColumn = this.TableProperties.SortColumns.Where(s => s.ColumnName == sortColumName).FirstOrDefault();
                    if (this.TableProperties.SortColumns.Count > 0 && currentSortColumn != null && currentSortColumn.ColumnName == sortColumName)
                    {
                        if (currentSortColumn.SortDirection == ListSortDirection.Descending && this.TableProperties.EnableTriStateSorting)
                        {
                            if (!this.Table.HasGroups)
                            {
                                var sortColumnsClone = this.TableProperties.SortColumns.ToList();
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>(), sortColumnsClone, NotifyCollectionChangedAction.Remove))//removing sort in tristate
                                {
                                    this.TableProperties.SortColumns.Clear();
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>(), sortColumnsClone, NotifyCollectionChangedAction.Remove);
                                }
                            }
                            else
                            {
                                GridDataSortColumn removedSortColumn = this.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == currentSortColumn.ColumnName);
                                if (removedSortColumn != null)
                                {
                                    if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>(), new List<GridDataSortColumn>() { removedSortColumn }, NotifyCollectionChangedAction.Remove)) // Removing group in tristate
                                    {
                                        this.TableProperties.SortColumns.Remove(removedSortColumn);
                                        this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>(), new List<GridDataSortColumn>() { removedSortColumn }, NotifyCollectionChangedAction.Remove);
                                    }
                                }
                            }
                        }
                        else
                        {
                            currentSortColumn.SortDirection = currentSortColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

                            // clear it before adding the current sort column
                            if (!this.Table.HasGroups)
                            {
                                var sortColumnsClone = this.TableProperties.SortColumns.ToList();
                                if (sortColumnsClone.Contains(currentSortColumn))
                                    sortColumnsClone.Remove(currentSortColumn);
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { currentSortColumn },sortColumnsClone, NotifyCollectionChangedAction.Replace)) // Resetting the same column
                                {
                                    this.TableProperties.SortColumns.Clear();
                                    this.TableProperties.SortColumns.Add(currentSortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { currentSortColumn }, sortColumnsClone, NotifyCollectionChangedAction.Replace);
                                }
                            }
                            else
                            {
                                GridDataSortColumn removedSortColumn = this.TableProperties.SortColumns.FirstOrDefault(s => s.ColumnName == currentSortColumn.ColumnName);                                
                                if (removedSortColumn != null)
                                {
                                    if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { currentSortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace)) // Reseting the same column in group
                                    {
                                        this.TableProperties.SortColumns.Remove(removedSortColumn);
                                        this.TableProperties.SortColumns.Add(currentSortColumn);
                                        this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { currentSortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Replace);
                                    }
                                }
                            }
                           // this.TableProperties.SortColumns.Add(currentSortColumn);
                        }
                    }

                    else
                    {
                        var sortColumn = new GridDataSortColumn()
                        {
                            ColumnName = sortColumName,
                            SortDirection = ListSortDirection.Ascending
                        };
                        if (!this.Table.HasGroups)
                        {
                            if (this.TableProperties.SortColumns.Count > 0)
                            {                                
                                var sortColumnsClone = this.TableProperties.SortColumns.ToList();
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, sortColumnsClone, NotifyCollectionChangedAction.Add)) // removing previous column and adding new column
                                {
                                    this.TableProperties.SortColumns.Clear();
                                    this.tableProperties.SortColumns.Add(sortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, sortColumnsClone, NotifyCollectionChangedAction.Add);
                                }
                            }
                            else
                            {
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add)) // Adding new column
                                {
                                    this.TableProperties.SortColumns.Add(sortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add);
                                }
                            }
                        }
                        else
                        {
                            var sortColumnClone = new List<GridDataSortColumn>();
                            //if(this.tableProperties.SortColumns.Count>0)
                            this.GetSortColumnsNotInGroup().ForEach<GridDataSortColumn>(g =>
                            {
                                 sortColumnClone.Add(g);
                               // this.TableProperties.SortColumns.Remove(g);
                                //this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>() { g }, NotifyCollectionChangedAction.Remove);
                            });
                            if (sortColumnClone!=null)
                            {
                                if(this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, sortColumnClone, NotifyCollectionChangedAction.Add)) // removing previous column and adding new column
                                {
                                    foreach (var removecolumns in sortColumnClone)
                                    {
                                         this.tableProperties.SortColumns.Remove(removecolumns);
                                    }
                                    this.tableProperties.SortColumns.Add(sortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, sortColumnClone, NotifyCollectionChangedAction.Add);
                                }
                            }
                            else
                            {
                                if (this.Table.RaiseSortColumnsChanging(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add)) // Adding new column in group
                                {
                                    this.TableProperties.SortColumns.Add(sortColumn);
                                    this.Table.RaiseSortColumnsChanged(new List<GridDataSortColumn>() { sortColumn }, new List<GridDataSortColumn>(), NotifyCollectionChangedAction.Add);
                                }
                            }
                        }
                       
                    }
                }
                //}
                isSortingApplied = true;
                //Cancelling the AddNew row when we do sorting
                if (this.View.IsAddingNew)
                    this.View.CancelNew();
                this.View.EndInit();
                
                this.UpdateSelectedRanges();

                this.IsInSort = false;
#if !SILVERLIGHT
                if (this.FrozenColumns - this.HeaderColumns > 0)
                    this.grid.InvalidateCell(GridRangeInfo.Row(0));
                else
                    this.Grid.InvalidateCell(GridRangeInfo.Row(0).ToCellSpan(this), true);
#else
                this.InvalidateCell(GridRangeInfo.Row(0));
#endif                
                //this.TableProperties.WireEvents();
                rowHeights.ResumeUpdates();
                /*if (this.Grid != null)
                {
                    var colIndex = this.ResolveVisibleColumnIndexToPosition(this.TableProperties.VisibleColumns.IndexOf(column));
                    this.Grid.CurrentCell.MoveTo(this.CurrencyManager.CurrentRowIndex, colIndex);
                }*/
            }
        }

        //Updates the selectedranges based on SelectedItems. 
        public virtual void UpdateSelectedRanges()
        {
            var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();

            //Skipped clear selection when header cell is cliked for sorting if ListBoxSelectionMode is None
            if (this.IsInSort && (this.Options.ListBoxSelectionMode == GridSelectionMode.None &&
                    this.Options.AllowSelection == GridSelectionFlags.Column |
                    this.Options.AllowSelection == GridSelectionFlags.Any))
            {
                return;
            }

            this.SelectedRanges.Clear();
            var removeItems = new List<object>();
            var addedItems = new List<object>();
            // Only in nested grid we have cleared the selection when sorting, so we have set the selection from current cell.
            if (dataGrid.Model.Table.HasNestedTables && dataGrid.SelectedItems.Count == 0 && dataGrid.Model != null && dataGrid.Model.Grid != null && dataGrid.Model.Grid.CurrentCell != null && dataGrid.Model.Grid.CurrentCell.HasCurrentCell && dataGrid.Model.Grid.CurrentCell.RowIndex != 0)
            {
                var recordIndex = this.ResolveIndexToRecordPosition(dataGrid.Model.Grid.CurrentCell.RowIndex);
                if (recordIndex != -1)
                {
                    var record = this.View.Records[recordIndex];
                    dataGrid.SelectedItems.Add(record.Data);
                }
            }
            foreach (var record in dataGrid.SelectedItems)
            {
                if (this.View.Contains(record))
                {
                    int rowIndex = -1;
                    int recordIndex = -1;
                    if (!this.Table.HasGroups)
                    {
                        recordIndex = this.View.Records.IndexOfRecord(record);
                        rowIndex = this.ResolvePositionToIndex(recordIndex);
                    }
                    else
                    {
                        recordIndex = this.View.TopLevelGroup.IndexOf(record);
                        rowIndex = this.ResolvePositionToIndex(recordIndex);
                    }

                    if (rowIndex > -1)
                    {
                        GridRangeInfo selectRange = null;
                        if (this.Options.ListBoxSelectionMode == GridSelectionMode.None &&
                            this.Options.AllowSelection == GridSelectionFlags.Cell |
                            this.Options.AllowSelection == GridSelectionFlags.Any)
                            selectRange = GridRangeInfo.Cell(rowIndex, grid.CurrentCell.ColumnIndex);
                        else
                            selectRange = GridRangeInfo.Row(rowIndex);
                        this.SelectedRanges.Add(selectRange);
                        this.Grid.InvalidateCell(selectRange);
                    }
                    else
                    {
                        removeItems.Add(record);
                    }
                }
                else
                {
                    removeItems.Add(record);
                }
            }

            foreach (var record in removeItems)
                dataGrid.SelectedItems.Remove(record);

            if (!dataGrid.SelectedItems.Contains(dataGrid.SelectedItem) && dataGrid.SelectedItem != null)
                dataGrid.SelectedItem = null;

            this.Table.RaiseRecordsSelectionChanged(new GridDataRecordsSelectionChangedEventArgs(removeItems, addedItems));

            //The below code was added to select the first row while grouping, if selectedRanges.count is 0.
            if (this.SelectedRanges.Count == 0 && this.IsInGroup)
            {
                this.Grid.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.CurrencyManager.CurrentCell.MoveTo(this.ResolveStartIndexBasedOnPosition(), 0);
                    }));
            }

            //Below code was added to maintain the selection in AddNewRow and FilterBarRow.
            if ((this.CurrencyManager.IsInAddNewRow || this.CurrencyManager.IsInFilterBarRow) &&
                this.SelectedRanges.Count == 0)
            {
                int startIndex = this.CurrencyManager.CurrentRowIndex;

                if (CurrencyManager.CurrentCell.HasCurrentCellAt(startIndex,
                                                                 this.CurrencyManager.CurrentCell.ColumnIndex))
                {
                    if (this.Options.ListBoxSelectionMode == GridSelectionMode.None &&
                        this.Options.AllowSelection == GridSelectionFlags.Cell |
                        this.Options.AllowSelection == GridSelectionFlags.Any)
                        this.SelectedRanges.Add(GridRangeInfo.Cell(startIndex, grid.CurrentCell.ColumnIndex));
                    else
                        this.SelectedRanges.Add(GridRangeInfo.Row(startIndex));
                }
                else
                    this.CurrencyManager.CurrentCell.MoveTo(startIndex, this.CurrencyManager.CurrentCell.ColumnIndex);
            }
        }

        internal void SortColumn(GridDataVisibleColumn column)
        {
            this.SortColumnMethod(column);
        }

        internal List<GridDataSortColumn> GetSortColumnsNotInGroup()
        {
            var clearColumns = new List<GridDataSortColumn>();
            foreach (var sortColumn in this.TableProperties.SortColumns)
            {
                var groupCol = this.TableProperties.GroupedColumns.FirstOrDefault(g => g.ColumnName == sortColumn.ColumnName);
                if (groupCol == null)
                {
                    clearColumns.Add(sortColumn);
                }
            }

            return clearColumns;
        }

        /*private bool SortInGroupedColumn(GridDataVisibleColumn column)
        {
            var result = false;
            if (this.Table.HasGroups)
            {
                var groupColumn = this.TableProperties.GroupedColumns.Where(g => g.ColumnName == column.MappingName).FirstOrDefault();
                var groupIndex = this.TableProperties.GroupedColumns.IndexOf(groupColumn);
                if (groupColumn != null)
                {
                    this.GetSortColumnsNotInGroup().ForEach<GridDataSortColumn>(g =>
                    {
                        this.TableProperties.SortColumns.Remove(g);
                    });
                    groupColumn.SortDirection = groupColumn.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    this.TableProperties.GroupedColumns[groupIndex] = groupColumn;
                    result = true;
                }
            }

            return result;
        }*/

        public bool IsInSort
        {
            get;
            internal set;
        }

        public bool IsInFilter
        {
            get;
            set;
        }

        /// <summary>
        /// This property set true if Record removes from the collection.
        /// </summary>
        internal bool IsInDeteteRecord
        {
            get;
            set;
        }


        protected bool IsInFilterOverride
        {
            get
            {
                return this.IsInFilter;
            }
            set
            {
                this.IsInFilter = value;
            }
        }


        public bool IsInGroup
        {
            get;
            internal set;
        }

        private void UnwireGrid()
        {
            if (this.Grid != null)
            {
                this.Grid.CellClick -= this.InternalGrid_CellClick;
            }
        }

        private void WireGrid()
        {
            if (this.Grid != null)
            {
                this.Grid.CellClick += this.InternalGrid_CellClick;
            }

            if (this.TableProperties != null)
            {
                this.Grid.AllowDragColumns = this.TableProperties.AllowDragColumns;
            }
        }

        protected bool InCurrencyManager
        {
            get;
            set;
        }

        protected bool InCollectionViewCurrency
        {
            get;
            set;
        }

        private void OnCurrentRecordSelectionChanged(object sender, GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            if (this.InCollectionViewCurrency || this.IsInSort || this.IsInFilter || !this.TableProperties.AutoFocusCurrentItem || this.IsInSuspend)
            {
                return;
            }

#if !SILVERLIGHT
            var editableView = this.View as IEditableCollectionView;
#else
            var editableView = this.View as Syncfusion.Windows.Data.IEditableCollectionView;
#endif
            if (editableView != null)
            {
                ///adding this code to check whether the currentCell is in Edit mode when deleting the record.
                ///If the CurrentCell is in edit mode then it forces the EndEdit.
                if (!editableView.IsAddingNew && this.CurrencyManager.CurrentCell.IsEditing)
                {
                    this.CurrencyManager.CurrentCell.EndEdit();
                }
            }

            this.InCurrencyManager = true;
            //Adding recordscount condition to make sure the Index range in case of UnBoundRow in the bottom of the Grid
            var recordscount = this.View != null && this.View.Records != null ? this.View.Records.Count : -1;
            if (args.NewIndex > -1 && args.NewIndex < recordscount)
            {
                this.View.MoveCurrentToPosition(args.NewIndex);
            }
            this.InCurrencyManager = false;
        }

        private bool IsAddNewRowSelected
        {
            get
            {
                if (this.SelectedRanges != null && this.SelectedRanges.Count == 1)
                {
                    if (this.SelectedRanges[0].Top == this.ResolveAddNewPositionInGrid())
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void OnViewCurrentChanged(object sender, EventArgs e)
        {
            if (this.Grid != null)
                if (this.InCurrencyManager || this.IsAddNewRowSelected || this.CurrencyManager.IsInAddNewRow || this.IsInFilter || (this.View.CurrentPosition == -1 && this.CurrencyManager.CurrentCell.RowIndex==-1) || this.CurrencyManager.IsInDelete || this.CurrencyManager.IsInDeactivate || !this.TableProperties.AutoFocusCurrentItem || this.IsInSuspend || this.Grid.CurrentCell.IsInDeactivated)
                {
                    return;
                }

#if !SILVERLIGHT
            var editableView = this.View as IEditableCollectionView;
#else
            var editableView = this.View as Syncfusion.Windows.Data.IEditableCollectionView;
#endif
            if (editableView != null)
            {
                if (editableView.IsEditingItem)
                {
                    editableView.CommitEdit();
                }
                else if (editableView.IsAddingNew)
                {
                    editableView.CommitNew();
                }
            }

            this.InCollectionViewCurrency = true;
#if !SILVERLIGHT

            if (this.Grid != null && !this.Grid.CurrentCell.IsInDropDownFilterCell)
#endif
            {
                if (this.View.CurrentPosition >= 0)
                    this.CurrencyManager.MoveTo(this.View.CurrentPosition);
                else if (this.View.CurrentPosition == -1 && this.CurrencyManager.CurrentCell.RowIndex == 0 && !this.Grid.IsCellVisible(new RowColumnIndex(this.Grid.CurrentCell.RowIndex, this.grid.CurrentCell.ColumnIndex)))//If CurrentCell is the headercell then, MoveTo Method shouldnot call.
                    this.Grid.ScrollCellInView(this.CurrencyManager.CurrentCell.RowIndex, this.CurrencyManager.CurrentCell.ColumnIndex, GridScrollCurrentCellReason.Click);
            }
            this.InCollectionViewCurrency = false;
        }

        /// <summary>
        /// Wires the source list. This also will only refresh the internal collections without refreshing the whole grid itself.
        /// </summary>
        /// <param name="needsWholeGridRefresh">if set to <c>true</c> [needs whole grid refresh].</param>
        public void WireSourceList(bool needsWholeGridRefresh)
        {
            this.RefreshSourceListCount();

            if (needsWholeGridRefresh)
            {
                this.SetTableDirty();
                this.EnsureInitialized();
            }
            else
            {
                // refresh the rows only
                this.Table.SetDirty();
                this.Table.Refresh();
            }
        }

        #region ISupportInitialize Members
        private bool isInitSuspended = false;
        public void BeginInit()
        {
            if (!this.isInitSuspended)
            {
                this.TableProperties.SuspendEvents();
                if (this.View != null)
                {
                    this.View.BeginInit();
                }
                this.isInitSuspended = true;
            }
        }

        public void EndInit()
        {
            if (this.isInitSuspended)
            {
                this.TableProperties.ResumeEvents();
                if (this.View != null)
                {
                    this.View.EndInit();
                }
                this.isInitSuspended = false;
            }
        }

        #endregion

        private Group GetRecordParent(Group group, object data)
        {
            Group parentGroup = null;
            if (group.IsBottomLevel)
            {
                foreach (var record in group.Records)
                {
                    if (record.Data == data)
                    {
                        parentGroup = group;
                        break;
                    }
                }
            }
            else
            {
                foreach (var childGroup in group.Groups)
                {
                    parentGroup = GetRecordParent(childGroup, data);
                    if (parentGroup != null)
                        break;
                }
            }
            return parentGroup;

        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // check to see if the incoming update is from the same thread
            if (this.Grid != null && !this.Grid.CheckAccess())
            {
#if !SILVERLIGHT
                this.Grid.Dispatcher.Invoke(new PropertyChangedEventHandler(OnPropertyChanged), new object[] { sender, e });
#else
                this.Grid.Dispatcher.BeginInvoke(new PropertyChangedEventHandler(OnPropertyChanged), new object[] { sender, e });
#endif
                return;
            }

            if (this.IsInSuspend)
            {
                return;
            }

            var record = sender;
            this.InvalidateRecord(e.PropertyName, record);
            if (this.CurrencyManager.IsInEndEdit)
            {
                this.Data.Clear();
            }
        }

        bool refreshWholeRow = false;

        /// <summary>
        /// Gets or sets the grid should refresh the whole row even if a single cell in the row changes.
        /// </summary>
        /// <remarks>
        /// if you want the grid to change the backcolor of a whole row when a single value changes, then the grid needs to redraw the whole row when the value changes.
        /// </remarks>
        public bool RefreshWholeRow
        {
            get { return refreshWholeRow; }
            set { refreshWholeRow = value; }
        }

        private void InvalidateRecord(string propertyName, object record)
        {
            if (this.Table.HasGroups && !this.IsInSuspend)
            {
                var topLevelGroup = this.View.TopLevelGroup;
                var recordParentGroup = this.GetRecordParent(topLevelGroup, record);
                if (recordParentGroup != null)
                {
                    var groupIndex = topLevelGroup.DisplayElements.IndexOf(recordParentGroup);
                    if (recordParentGroup.IsExpanded)
                    {
                        var groupRecordEntry = recordParentGroup.Details as GroupRecordEntry;
                        if (groupRecordEntry.Summaries.Count > 0)
                        {
                            //topLevelGroup.UpdateSummaries(recordParentGroup);
                            var summaryindex = groupIndex + recordParentGroup.GetRecordCount() + 1;
                            topLevelGroup.Invalidate(summaryindex, groupRecordEntry.Summaries.Count);
                        }
                    }

                    if (this.TableProperties.CaptionSummaryRow != null)
                    {
                        //topLevelGroup.UpdateCaptionSummaries();
                        if (groupIndex > -1)
                            topLevelGroup.Invalidate(groupIndex, 1);
                        //While loop will validate all the CaptionSummaries of the ParticularGroup
                        var group = recordParentGroup.Parent;
                        while (group != null)
                        {
                            groupIndex = topLevelGroup.DisplayElements.IndexOf(group);
                            if (groupIndex > -1)
                                topLevelGroup.Invalidate(groupIndex, 1);
                            group = group.Parent;
                        }
                    }
                }
            }
            var index = this.View.Records.IndexOfRecord(record);
            var rowIndex = !this.Table.HasGroups ? this.ResolvePositionToIndex(index) : this.ResolveGroupRecordPositionToIndex(index);
//#if !SILVERLIGHT
//            if (this.Grid != null)
//            {
//                bool visibleCellsAffected = this.Grid.ScrollRows.AnyVisibleLines(rowIndex, rowIndex); // && ScrollColumns.AnyVisibleLines(span.Left, span.Right);
//                if (!visibleCellsAffected)
//                {
//                    InvalidateTableSummaryRow();
//                    return;
//                }
//            }
//#endif
            IEnumerable<GridDataVisibleColumn> columns;
            if (this.TableProperties.NotifyComplexPropertyChanges)
                columns = this.TableProperties.VisibleColumns.Where(v => (propertyName != null && v.MappingName == propertyName) || (propertyName != null && v.MappingName != null && v.MappingName.Contains(propertyName)));
            else
                columns = this.TableProperties.VisibleColumns.Where(v => v.MappingName == propertyName);
            
            if (index > -1)
            {
                // sometimes the property changed updates dont focus on the current item if the grid is sorted, we force the grid to scrollinview
                if (
#if !SILVERLIGHT
Mouse.LeftButton == MouseButtonState.Released &&
#endif
 this.TableProperties.AutoFocusCurrentItem && this.Grid != null && !this.Grid.IsCellVisible(this.Grid.CurrentCell.CellRowColumnIndex))
                {
                    if (!this.Table.HasFilters && !(this.Grid is GridDataCellNestedGridEditor && !this.IsEditing))
                    {
                        this.Grid.CurrentCell.ScrollInView();
                    }
                }
#if SILVERLIGHT
                if (this.Table.HasDetailsView)
                {
                    var rec = this.View.Records[index] as GridDataRecord;
                    if (rec.IsDetailsViewExpanded)
                    {
                        this.InvalidateCell(GridRangeInfo.Row(rowIndex + 1));
                    }
                }
#endif
                foreach (var col in columns)
                {
                    var colIndex = this.ResolveVisibleColumnIndexToPosition(this.TableProperties.VisibleColumns.IndexOf(col));
#if !SILVERLIGHT
                    if (this.Grid != null)
                    {
                        if (!this.Grid.ScrollRows.AnyVisibleLines(rowIndex, rowIndex))
                        {
                            InvalidateTableSummaryRow();
                            this.VolatileCellStyles.Clear(new RowColumnIndex(rowIndex, colIndex));
                            return;
                        }
                        bool visibleCellsAffected = this.Grid.ScrollColumns.AnyVisibleLines(colIndex, colIndex) || !string.IsNullOrEmpty(col.ReferenceFields);
                        if (!visibleCellsAffected)
                            continue;
                    }
#endif
                    var shouldbreak = this.RefreshRow(rowIndex, colIndex, RefreshWholeRow);
                    if (shouldbreak == -1)
                        break;
                }
                if (columns.Count() <= 0)
                {
                    this.RefreshRow(rowIndex, 0, true);
                }
            }
            this.InvalidateTableSummaryRow();
        }

        private void InvalidateTableSummaryRow()
        {
            if (this.Table.HasTableSummaries)
            {
                if (this.TableProperties.TableSummaryPosition == Position.Top)
                    this.InvalidateCell(GridRangeInfo.Rows(this.HeaderRows, this.FrozenRows - this.HeaderRows));
                else
                    this.InvalidateCell(GridRangeInfo.Rows(this.RowCount - this.FooterRows, this.RowCount));
            }

#if SILVERLIGHT
            this.InvalidateVisual(true);
#endif
        }

        private int RefreshRow(int rowIndex, int colIndex, bool _RefreshWholeRow)
        {
#if !SILVERLIGHT
            if (this.Grid != null && this.Grid.EnableRenderOptimization == EnableRenderOptimization.DisableBackgroundFrameRendering)
                this.Grid.needRenderStyleBackgrounds = false;
#endif
            if(!_RefreshWholeRow)
            {
                var column = this.TableProperties.VisibleColumns.ElementAt(this.ResolvePositionToVisibleColumnIndex(colIndex));
                if (!string.IsNullOrEmpty(column.ReferenceFields))
                {               
                    string[] Fields = column.ReferenceFields.Split(';');
                    foreach (var field in Fields)
                    {
                        var col = this.TableProperties.VisibleColumns[field];//.Where(v => ( v.MappingName == field) );                
                        if (col != null)
                        {
                            int columnIndex = this.ResolveVisibleColumnIndexToPosition(this.TableProperties.VisibleColumns.IndexOf(col));
                            this.InvalidateCell(new RowColumnIndex(rowIndex, columnIndex));
#if !SILVERLIGHT
                            if (this.Grid != null)
                                this.Grid.MarkIndividualCellBackgroundDirty(rowIndex, columnIndex);
#endif
                        }
                    }
                }
            }
            
          
            var range = GridRangeInfo.Cell(rowIndex, colIndex);
            //if (this.CurrencyManager != null
            //    && this.CurrencyManager.IsEditing
            //    && !this.CurrencyManager.CurrentCell.CellRowColumnIndex.IsEmpty)
            //{
            //    this.CurrencyManager.CurrentCell.EndEdit();
            //}

            if (this.Table.HasConditionalFormats || this.Table.HasExpressionColumns || this.Table.HasFilters)
            {
#if !SILVERLIGHT
                if (this.Grid.EnableRenderOptimization != EnableRenderOptimization.DisableBackgroundFrameRendering)
                {
                    foreach (var grid in this.Views)
                    {
                        grid.InvalidateCellBackground(rowIndex, 0);
                    }
                }

                if (rowIndex > -1 && _RefreshWholeRow)
                {
                    if (this.Grid != null)
                        this.Grid.MarkIndividualRowBackgroundDirty(rowIndex);
                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
                }
                else
                {
                    if (this.Grid != null)
                        this.Grid.MarkIndividualCellBackgroundDirty(rowIndex, colIndex);
                    var currentcell = this.CurrencyManager.CurrentCell;
                  
                    if (currentcell.HasCurrentCell && currentcell.RowIndex == range.Top &&
                        currentcell.ColumnIndex == range.Left)
                    {
                        if (!currentcell.IsEditing)
                        {
                            this.InvalidateCell(range);
                            this.InvalidateVisual();
                        }
                    }
                    else
                        this.InvalidateCell(range);
                    AnimateBackground(range.Top, range.Left);
                }
#else
                foreach (var grid in this.Views)
                {
                    grid.InvalidateCellBackground(rowIndex, 0);
                }

                if (rowIndex > -1 && _RefreshWholeRow)
                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
                else
                    this.InvalidateCell(range);

                if (this.Table.HasExpressionColumns && !RefreshWholeRow)
                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
#endif

                //if (this.Table.HasFilters && this.TableProperties.RowBackground != null || this.TableProperties.AlternatingRowBackground != null && !this.Table.HasGroups)
                //{
                //    //Earlier the entire grid rows are invalidated.
                //    //Now only the visible rows are invalidated.
                //    var rowcoll = this.Grid.ScrollRows.GetVisibleLines();
                //    for (int i = 0; i < rowcoll.Count - this.FooterRows; i++)
                //    {
                //        if (rowcoll[i].LineIndex >= rowIndex)
                //        {
                //            this.InvalidateCell(GridRangeInfo.Row(rowcoll[i].LineIndex));
                //        }
                //    }
                //}
                return -1; //don't need to iterate thru columns if whole row has been refreshed
            }
            else if (_RefreshWholeRow)
            {
                if (rowIndex > -1)
                {
#if !SILVERLIGHT
                    if (this.Grid != null)
                        this.Grid.MarkIndividualRowBackgroundDirty(rowIndex);

                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
                    if (this.Grid != null && this.Grid.EnableRenderOptimization != EnableRenderOptimization.DisableBackgroundFrameRendering)
                        this.InvalidateVisual();
#else
                    this.InvalidateCell(GridRangeInfo.Row(rowIndex));
                    this.InvalidateVisual();
#endif

                }
                return -1; //don't need to iterate thru columns if whole row has been refreshed
            }
            else
            {
                if (range.RangeType != GridRangeInfoType.Empty)
                {
#if !SILVERLIGHT
                    if (this.Grid != null)
                        this.Grid.MarkIndividualCellBackgroundDirty(rowIndex, colIndex);
#endif
                    var currentcell = this.CurrencyManager.CurrentCell;
                    if (currentcell.HasCurrentCell && currentcell.RowIndex == range.Top &&
                        currentcell.ColumnIndex == range.Left)
                    {
                        if (!currentcell.IsEditing)
                        {
                            this.InvalidateCell(range);
                            this.InvalidateVisual();
                        }
                    }
                    else
                        this.InvalidateCell(range);
                }
            }
#if !SILVERLIGHT
            if (!_RefreshWholeRow)
                AnimateBackground(range.Top, range.Left);
#endif
            return 1;
        }

#if !SILVERLIGHT
        private void AnimateBackground(int rowindex,int colindex)
        {
            if (this.TableStyle.IsBackGroundAnimationEnabled == true)
            {
                SolidColorBrush newBrush = new SolidColorBrush();

                ColorAnimation cellBackGroundAnimation = new ColorAnimation();
                cellBackGroundAnimation.From = this.TableStyle.BackgroundAnimationFromColor;
                cellBackGroundAnimation.To = this.TableStyle.BackgroundAnimationToColor;
                cellBackGroundAnimation.AutoReverse = this.TableStyle.IsAutoReverseEnabled;
                cellBackGroundAnimation.Duration = this.TableStyle.AnimationDuration;
                cellBackGroundAnimation.FillBehavior = FillBehavior.Stop;
                newBrush.BeginAnimation(SolidColorBrush.ColorProperty, cellBackGroundAnimation);
                this.Table.Model[rowindex, colindex].Background = newBrush;
                NameScope.SetNameScope(this.Grid, new NameScope());
                this.Table.Model[rowindex, colindex].Background = newBrush;
                this.Grid.RegisterName("BackgroundBrush", newBrush);
                DoubleAnimation glowEffectOpacity = new DoubleAnimation();
                glowEffectOpacity.From = 0.5;
                glowEffectOpacity.To = 0.0;
                //glowEffectOpacity.Duration = TimeSpan.FromMilliseconds(500);
                glowEffectOpacity.Duration = this.TableStyle.AnimationDuration;
                glowEffectOpacity.AutoReverse = false;
                glowEffectOpacity.FillBehavior = FillBehavior.HoldEnd;
                Storyboard.SetTargetName(glowEffectOpacity, "BackgroundBrush");
                Storyboard.SetTargetProperty(glowEffectOpacity, new PropertyPath(SolidColorBrush.OpacityProperty));
                Storyboard opacityAnimation = new Storyboard();
                opacityAnimation.Children.Add(glowEffectOpacity);
                opacityAnimation.Begin(this.Grid);
            }
        }
#endif

#if SILVERLIGHT
        class ListUtil
        {
            /// <summary>
            /// Indicates whether the specified PropertyDescriptor has nested properties.
            /// </summary>
            /// <param name="pd">The PropertyDescriptor to be checked.</param>
            /// <returns>True if nested properties are found; False otherwise.</returns>
            public static bool IsComplexType(PropertyInfo pd)
            {
                Type t = pd.PropertyType;
                return IsComplexType(t);
            }

            /// <summary>
            /// Indicates whether the specified Type has nested properties.
            /// </summary>
            /// <param name="t">The Type to be checked.</param>
            /// <returns>True if nested properties are found; False otherwise.</returns>
            public static bool IsComplexType(Type t)
            {
                Type underlyingType = NullableHelper.GetUnderlyingType(t);
                if (underlyingType != null)
                    t = underlyingType;

                if (t != typeof(object)
                    && t != typeof(Decimal)
                    && t != typeof(DateTime)
                    && t != typeof(Type)
                    //&& t != typeof(System.Drawing.Color)
                    && t != typeof(string)
                    && t != typeof(Guid)
                    && t.BaseType != typeof(Enum)
                    && !t.IsPrimitive)
                    return true;

                return false;
            }
        }
#endif

#if !SILVERLIGHT
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/>.
        /// </summary>
        /// <param name="model">The file name.</param>
        public override void Serialize(string fileName)
#else
        /// <summary>
        /// Serializes the specified properties in the <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/>.
        /// Opens up a SaveFileDialog and saves the serialized data in XML.
        /// </summary>
        public override void Serialize()
#endif
        {
            try
            {
                XmlSerializer xs = null;
#if !SILVERLIGHT
                Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn),
                               typeof(GridDataTimeSpanEditVisibleColumn)};
                xs = new XmlSerializer(typeof(GridDataTableProperties),types);
                using (var sw = new XmlTextWriter(fileName, Encoding.Default))
                {
                    xs.Serialize(sw.BaseStream, this.TableProperties);
                }
#else
                xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn),typeof(GridDataDoubleEditVisibleColumn)});
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };
                if (sfd.ShowDialog() == true)
                {
                    var stream = sfd.OpenFile();
                    if (stream != null)
                    {
                        using (var sw = new StreamWriter(stream))
                        {
                            xs.Serialize(sw.BaseStream, this.TableProperties);
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
        /// <param name="model">The model.</param>
        /// <param name="textWriter">The text writer.</param>
#if !SILVERLIGHT
        public override void SerializeToStream(TextWriter textWriter)
#else
        public override void SerializeToStream(Stream stream)
#endif
        {
            try
            {

                XmlSerializer xs = null;
#if !SILVERLIGHT
                Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn),
                               typeof(GridDataTimeSpanEditVisibleColumn)};
                xs = new XmlSerializer(typeof(GridDataTableProperties), types);
                using (var sw = new XmlTextWriter(textWriter))
                {
                    xs.Serialize(sw.BaseStream, this.TableProperties);
                }
#else
                xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
                using (var sw = new StreamWriter(stream))
                {
                    xs.Serialize(sw.BaseStream, this.TableProperties);
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Serializes the <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/> properties as string.
        /// </summary>
        /// <returns></returns>
        public override string SerializeAsString()
        {
            var result = string.Empty;
            try
            {
                 XmlSerializer xs = null;
                using (var sWriter = new StringWriter())
                {
#if !SILVERLIGHT
                    Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn),
                               typeof(GridDataTimeSpanEditVisibleColumn)};
                    xs = new XmlSerializer(typeof(GridDataTableProperties), types);
                    using (var sw = new XmlTextWriter(sWriter))
                    {
                        xs.Serialize(sw, this.TableProperties);
                    }
#else
                    xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
                    using (var sw = XmlWriter.Create(sWriter))
                    {
                        xs.Serialize(sw, this.TableProperties);
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
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/>.
        /// </summary>
        public override void Deserialize(string fileName)
#else
        /// <summary>
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/> 
        /// from an XML file read with an OpenFileDialog.
        /// </summary>
        public override void Deserialize()
#endif
        {
            try
            {

                XmlSerializer xs = null;
#if !SILVERLIGHT
                Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn)
                               ,typeof(GridDataTimeSpanEditVisibleColumn)};
                xs = new XmlSerializer(typeof(GridDataTableProperties), types);
                using (var sr = new XmlTextReader(fileName))
                {
                    var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);

                    // clearing the selection
                    this.TableProperties.Model.SelectedRanges.Clear();
                    dataGrid.Model.CurrencyManager.Reset();
                    // Reselecting the selected items.
                    dataGrid.SelectedItems.Clear();
                    dataGrid.SelectedItem = null;
                }
#else
                xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
                OpenFileDialog ofd = new OpenFileDialog() { Filter = "XML Files (*.xml)|*.xml", FilterIndex = 1 };

                if (ofd.ShowDialog() == true)
                {
                    var stream = ofd.File.OpenRead();
                    using (var sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                        Apply(this, tableProperties);
                    }
                }
#endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SelectionChanged_serialization()
        {
            this.TableProperties.Model.SelectedRanges.Clear();
        }

        /// <summary>
        /// Deserializes from stream.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
#if !SILVERLIGHT
        public override void DeserializeFromStream(TextReader textReader)
#else
        public override void DeserializeFromStream(Stream stream)
#endif
        {
            try
            {
                XmlSerializer xs = null;
#if !SILVERLIGHT
                Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn)
                               ,typeof(GridDataTimeSpanEditVisibleColumn)};
                 xs = new XmlSerializer(typeof(GridDataTableProperties), types);
                using (var sr = new XmlTextReader(textReader))
                {
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);
                }
#else
                xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
                using (var sr = new StreamReader(stream, Encoding.UTF8))
                {
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);
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
        /// Deserializes the specified <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/> from a FileStream. 
        /// The File can be read from an Isolated Storage with the <see cref="System.IO.FileStream"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="System.IO.FileStream"/> has to be closed or used inside a closure when calling this method.
        /// </remarks>
        /// <param name="fileStream">The file stream.</param>
        public override void Deserialize(FileStream fileStream)
        {
            var xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
            try
            {
                using (var sr = new StreamReader(fileStream))
                {
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#endif

        /// <summary>
        /// Deserializes <see cref="Syncfusion.Windows.Controls.Grid.GridDataTableModel"/> properties from string.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="content">The content.</param>
        public override void DeserializeFromString(string content)
        {
            XmlSerializer xs = null;
            using (var sReader = new StringReader(content))
            {
#if !SILVERLIGHT
                Type[] types = { typeof(GridDataDateTimeVisibleColumn), typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataCheckBoxVisibleColumn), typeof(GridDataDoubleEditVisibleColumn),
                                   typeof(GridDataUpDownEditVisibleColumn),typeof(GridDataCurrencyEditVisibleColumn),typeof(GridDataMaskEditVisibleColumn),typeof(GridDataPercentEditVisibleColumn)
                               ,typeof(GridDataTimeSpanEditVisibleColumn)};
                xs = new XmlSerializer(typeof(GridDataTableProperties), types);
                using (var sr = new XmlTextReader(sReader))
                {
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);
                }
#else
                xs = new XmlSerializer(typeof(GridDataTableProperties), new Type[] { typeof(GridDataIntegerEditVisibleColumn), typeof(GridDataDoubleEditVisibleColumn) });
                using (var sr = XmlReader.Create(sReader))
                {
                    var tableProperties = xs.Deserialize(sr) as GridDataTableProperties;
                    Apply(this, tableProperties);
                }
#endif
            }
        }

        private void Apply(GridDataTableModel model, GridDataTableProperties tableProperties)
        {
            model.SuspendEvents();
            model.TableProperties.SuspendEvents();
             var dataGrid = model.Grid.FindParentElementOfType<GridDataControl>();
#if !SILVERLIGHT
            var summaryrows = new FreezableCollection<GridDataSummaryRow>();
#else
             var summaryrows = new ObservableCollection<GridDataSummaryRow>();
#endif

             if (dataGrid.SummaryRows.Count > 0)
                 {
                  summaryrows = dataGrid.SummaryRows;
                 }
           
            model.TableProperties.InitializeFrom(tableProperties, true);

            if (dataGrid.SummaryRows.Count > 0)
                {
                for (int rowindex=0 ; rowindex < dataGrid.SummaryRows.Count; rowindex++)// summaryrow in dataGrid.SummaryRows)
                    {
                    var summaryrow = dataGrid.SummaryRows[rowindex];
                    for (int colindex = 0; colindex < summaryrow.SummaryColumns.Count; colindex++ )
                        {
                        var summarycol = summaryrow.SummaryColumns[colindex];
                        if (summarycol.SummaryType == SummaryType.Custom)
                            {
                            var oldsummarycol = summaryrows[rowindex].SummaryColumns[colindex];
                            summarycol.CustomAggregate = oldsummarycol.CustomAggregate;
                            }
                        }
                    }
                }

            model.TableProperties.ResumeEvents();
            if (model.View != null)
            {
                model.View.BeginInit();
                model.View.EndInit();
            }

            var canPopulateColumn = tableProperties.VisibleColumns.Count == 0 && tableProperties.AutoPopulateColumns;

            //Previously passed true to WireSourceList, In that case columns repopulated once again when AutoPopulateColumns set to True
            model.WireSourceList(canPopulateColumn);
            //Previous code
            //model.WireSourceList(true);
            model.RefreshColumns(true, true);
            model.ResumeEvents();
          
           
            if (dataGrid.GroupDropAreaGrid != null)
            {
                dataGrid.GroupDropAreaGrid.Refresh();
            }
            model.InvalidateDisplay();
        }
#if !SILVERLIGHT
        internal IEnumerable DefaultHeaderContextMenuItems
        {
            get
            {
                return GetDefaultContextMenuItems();
            }
        }

        private IEnumerable GetDefaultContextMenuItems()
        {
            List<object> defaultHeaderContextMenuItems = new List<object>();

            MenuItem SortAscendingItem = new MenuItem();
            SortAscendingItem.Header = "Sort Ascending";
            SortAscendingItem.Command = ContextMenuCommands.SortAscending;
            SortAscendingItem.CommandParameter = this.tableProperties;
            ResourceDictionary resources = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };
            Image icon1 = resources["SortAscending"] as Image;
            SortAscendingItem.Icon = icon1;

            MenuItem SortDescendingItem = new MenuItem();
            SortDescendingItem.Header = "Sort Descending";
            SortDescendingItem.Command = ContextMenuCommands.SortDescending;
            SortDescendingItem.CommandParameter = this.tableProperties;
            Image icon2 = resources["SortDescending"] as Image;
            SortDescendingItem.Icon = icon2;

            MenuItem ClearSortItem = new MenuItem();
            ClearSortItem.Header = "Clear Sort";
            ClearSortItem.Command = ContextMenuCommands.ClearSort;
            ClearSortItem.CommandParameter = this.tableProperties;
            ClearSortItem.IsEnabled = false;
            Image icon3 = resources["ClearSort"] as Image;
            ClearSortItem.Icon = icon3;

            MenuItem ClearFilterItem = new MenuItem();
            ClearFilterItem.Header = "Clear Filter";
            ClearFilterItem.Command = ContextMenuCommands.ClearFilter;
            ClearFilterItem.CommandParameter = this.tableProperties;
            Image icon4 = resources["ClearFilter"] as Image;
            ClearFilterItem.Icon = icon4;

            MenuItem HideColumnItem = new MenuItem();
            HideColumnItem.Header = "Hide Column";
            HideColumnItem.Command = ContextMenuCommands.HideColumn;
            HideColumnItem.CommandParameter = this.tableProperties;

            MenuItem GroupColumnItem = new MenuItem();
            GroupColumnItem.Header = "GroupBy Column";
            GroupColumnItem.Command = ContextMenuCommands.GroupBy;
            GroupColumnItem.CommandParameter = this.tableProperties;
            Image icon6 = resources["GroupBy"] as Image;
            GroupColumnItem.Icon = icon6;
            GroupColumnItem.IsEnabled = true;

            MenuItem GroupDropAreaItem = new MenuItem();
            GroupDropAreaItem.Header = "GroupDropArea";
            GroupDropAreaItem.Command = ContextMenuCommands.HideGroupDropArea;
            GroupDropAreaItem.CommandParameter = this.tableProperties;
            //menuItem7.IsCheckable = true;   
            //Image icon7 = resources["GroupDropArea"] as Image;
            //menuItem7.Icon = icon7;

            MenuItem BestFitItem = new MenuItem();
            BestFitItem.Header = "Best Fit";
            BestFitItem.Command = ContextMenuCommands.BestFit;
            BestFitItem.CommandParameter = this.tableProperties;
            Image icon8 = resources["BestFit"] as Image;
            BestFitItem.Icon = icon8;

            var style = new GridDataStyleInfo(contextMenuEventArgs.Style.CellIdentity);
            var tableStyleInfoIdentity = style.CellIdentity;
            var visibleColumn = tableStyleInfoIdentity.Column;

            if (this.TableProperties.SortColumns.Count != 0)
            {                
                foreach (GridDataSortColumn column in this.TableProperties.SortColumns)
                {
                    if (column.ColumnName == visibleColumn.MappingName)
                    {
                        if (visibleColumn.AllowSort)
                        {
                            if (column.SortDirection == ListSortDirection.Descending)
                            {
                                SortAscendingItem.IsEnabled = true;
                                SortDescendingItem.IsEnabled = false;
                                ClearSortItem.IsEnabled = true;
                            }
                            else
                            {
                                SortAscendingItem.IsEnabled = false;
                                SortDescendingItem.IsEnabled = true;
                                ClearSortItem.IsEnabled = true;
                            }
                        }
                        else
                        {
                            SortAscendingItem.IsEnabled = false;
                            SortDescendingItem.IsEnabled = false;
                            ClearSortItem.IsEnabled = false;
                        }

                        break;
                    }
                }
            }
            else
            {
                SortAscendingItem.IsEnabled = true;
                SortDescendingItem.IsEnabled = true;
                ClearSortItem.IsEnabled = false;
            }

            if (this.TableProperties.GroupedColumns.Count != 0)
            {
                foreach (var groupColumn in this.TableProperties.GroupedColumns)
                {
                    if (groupColumn.ColumnName == visibleColumn.MappingName)
                    {
                        GroupColumnItem.IsChecked = true;
                        GroupColumnItem.IsEnabled = true;
                    }
                }
            }

            if (!visibleColumn.AllowGroup)
            {
                //menuItem6.IsChecked = false;
                GroupColumnItem.IsEnabled = false;
            }

            if (!visibleColumn.AllowSort)
            {
                SortAscendingItem.IsEnabled = false;
                SortDescendingItem.IsEnabled = false;
                ClearSortItem.IsEnabled = false;
            }
            if (this.TableProperties.ShowGroupDropArea)
            {
                GroupDropAreaItem.IsChecked = true;
            }
            else
            {
                GroupDropAreaItem.IsChecked = false;
            }
            if (this.TableProperties.ItemsSource == null && this.SourceListCount == 0)
            {
                SortAscendingItem.IsEnabled = false;
                SortDescendingItem.IsEnabled = false;
                ClearSortItem.IsEnabled = false;
                ClearFilterItem.IsEnabled = false;
                HideColumnItem.IsEnabled = false;
                GroupColumnItem.IsEnabled = false;
                GroupDropAreaItem.IsEnabled = false;
                BestFitItem.IsEnabled = false;
            }
            defaultHeaderContextMenuItems.Add(SortAscendingItem);
            defaultHeaderContextMenuItems.Add(SortDescendingItem);
            defaultHeaderContextMenuItems.Add(new Separator());
            defaultHeaderContextMenuItems.Add(ClearSortItem);
            defaultHeaderContextMenuItems.Add(ClearFilterItem);
            defaultHeaderContextMenuItems.Add(new Separator());
            defaultHeaderContextMenuItems.Add(HideColumnItem);
            defaultHeaderContextMenuItems.Add(GroupColumnItem);
            defaultHeaderContextMenuItems.Add(GroupDropAreaItem);
            defaultHeaderContextMenuItems.Add(new Separator());
            defaultHeaderContextMenuItems.Add(BestFitItem);

            return defaultHeaderContextMenuItems;
        }

        internal IEnumerable DefaultRecordContextMenuItems
        {
            get
            {
                return GetDefaultRecordContextMenuItems();
            }
        }

        private IEnumerable GetDefaultRecordContextMenuItems()
        {
            List<object> defaultRecordContextMenuItems = new List<object>();

            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "Delete";
            menuItem1.Command = ContextMenuCommands.Delete;
            menuItem1.CommandParameter = this.tableProperties;
            ResourceDictionary resources = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Grid.Wpf;component/GridDataControl/Control/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            Image icon1 = resources["Delete"] as Image;
            menuItem1.Icon = icon1;

            defaultRecordContextMenuItems.Add(menuItem1);
            if (!this.TableProperties.AllowDelete)
                menuItem1.IsEnabled = false;
            else
                menuItem1.IsEnabled = true;
            return defaultRecordContextMenuItems;
        }

        internal IEnumerable DefaultGroupHeaderContextMenuItems
        {
            get
            {
                return GetDefaultGroupHeaderContextMenuItems();
            }
        }

        private IEnumerable GetDefaultGroupHeaderContextMenuItems()
        {
            List<object> defaultGroupHeaderContextMenuItems = new List<object>();

            MenuItem menuItem1 = new MenuItem();
            menuItem1.Header = "Expand";
            menuItem1.Command = ContextMenuCommands.ExpandGroup;
            menuItem1.CommandParameter = this.tableProperties;

            MenuItem menuItem2 = new MenuItem();
            menuItem2.Header = "Collapse";
            menuItem2.Command = ContextMenuCommands.CollapseGroup;
            menuItem2.CommandParameter = this.tableProperties;

            var rowIndex = this.ResolveIndexToGroupPosition(this.contextMenuEventArgs.Cell.RowIndex);
            var group = this.Table.GroupModel.DisplayElements[rowIndex];
            var groupKey = group as Group;
            if (!groupKey.IsExpanded)
            {
                menuItem1.IsEnabled = true;
                menuItem2.IsEnabled = false;
            }
            else
            {
                menuItem1.IsEnabled = false;
                menuItem2.IsEnabled = true;
            }

            defaultGroupHeaderContextMenuItems.Add(menuItem1);
            defaultGroupHeaderContextMenuItems.Add(menuItem2);
            return defaultGroupHeaderContextMenuItems;
        }

        protected override void OnQueryContextMenuInfo(GridQueryContextMenuInfoEventArgs e)
        {
            this.contextMenuEventArgs = e;

            var dataStyleInfo = new GridDataStyleInfo(e.Style.CellIdentity);
            var tableStyleIdentity = dataStyleInfo.CellIdentity;
            if (tableProperties.ContextMenuOptions == ContextMenuOptions.Default)
            {
                if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell || tableStyleIdentity.TableCellType==GridDataTableCellType.GroupCaptionSummaryCoveredCell
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell)
                    e.Style.ContextMenuItems = DefaultGroupHeaderContextMenuItems;
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell)
                    e.Style.ContextMenuItems = DefaultHeaderContextMenuItems;
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell)
                    e.Style.ContextMenuItems = DefaultRecordContextMenuItems;
            }
            else if (tableProperties.ContextMenuOptions == ContextMenuOptions.CustomWithDefault)
            {
                if (tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell || tableStyleIdentity.TableCellType==GridDataTableCellType.GroupCaptionSummaryCoveredCell
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell)
                {
                    List<object> contextMenuItems = DefaultGroupHeaderContextMenuItems as List<object>;
                    if (this.TableProperties.GroupHeaderContextMenuItems.ToList<object>().Count() > 0)
                        contextMenuItems.Add(new Separator());
                    foreach (var item in this.TableProperties.GroupHeaderContextMenuItems)
                        contextMenuItems.Add(item);
                    e.Style.ContextMenuItems = contextMenuItems;
                }
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell)
                {
                    List<object> contextMenuItems = DefaultHeaderContextMenuItems as List<object>;
                    if (this.TableProperties.HeaderContextMenuItems.ToList<object>().Count() > 0)
                        contextMenuItems.Add(new Separator());
                    foreach (var item in this.TableProperties.HeaderContextMenuItems)
                        contextMenuItems.Add(item);
                    e.Style.ContextMenuItems = contextMenuItems;
                }
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell)
                {
                    List<object> contextMenuItems = DefaultRecordContextMenuItems as List<object>;
                    if (this.TableProperties.RecordContextMenuItems.ToList<object>().Count() > 0)
                        contextMenuItems.Add(new Separator());
                    foreach (var item in this.TableProperties.RecordContextMenuItems)
                        contextMenuItems.Add(item);
                    e.Style.ContextMenuItems = contextMenuItems;
                }
            }
            else
            {
                if ((tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryTitleCell || tableStyleIdentity.TableCellType==GridDataTableCellType.GroupCaptionSummaryCoveredCell&& TableProperties.GroupHeaderContextMenuItems.ToList<object>().Count() > 0)
                    || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell || tableStyleIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryRecordCell && this.TableProperties.GroupHeaderContextMenuItems != null)
                    e.Style.ContextMenuItems = this.TableProperties.GroupHeaderContextMenuItems;
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.ColumnHeaderCell && this.TableProperties.HeaderContextMenuItems != null && TableProperties.HeaderContextMenuItems.ToList<object>().Count() > 0)
                    e.Style.ContextMenuItems = this.TableProperties.HeaderContextMenuItems;
                else if (tableStyleIdentity.TableCellType == GridDataTableCellType.RecordCell && this.TableProperties.RecordContextMenuItems != null && TableProperties.RecordContextMenuItems.ToList<object>().Count() > 0)
                    e.Style.ContextMenuItems = this.TableProperties.RecordContextMenuItems;
            }
            base.OnQueryContextMenuInfo(e);
        }

        GridQueryContextMenuInfoEventArgs contextMenuEventArgs;
        internal GridQueryContextMenuInfoEventArgs ContextMenuEventArgs
        {
            get { return contextMenuEventArgs; }
            set { contextMenuEventArgs = value; }
        }
#endif

        internal Brush GetFilterPopupBackgroundBrush()
        {
            return GridVisualStyle.FilterPopupBackgroundBrush;
        }

        internal Brush GetFilterPopupForeGroundBrush()
        {
            return GridVisualStyle.FilterPopupForegroundBrush;
        }
    }

    public static class GridDataTableModelHelper
    {

#if !SILVERLIGHT
        public static bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }
#else
        internal static Array GetValues(Type enumType)
        {
            List<object> values = new List<object>();

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add(value);
            }
            return values.ToArray();
        }
#endif
        /// <summary>
        /// Determines whether the RowIndex is in group records based on position.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="actualRowIndex">Actual index of the row.</param>
        /// <returns>
        /// <c>true</c> if [is in group index] [the specified model]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInGroupRecordIndex(this GridDataTableModel model, int actualRowIndex)
        {
            var result = false;
            var rowIndex = model.ResolveIndexToGroupPosition(actualRowIndex);
            var groupCache = rowIndex > -1 && rowIndex < model.Table.GroupModel.DisplayElements.Count ? model.Table.GroupModel.DisplayElements[rowIndex] : null;
            var groupModel = model.Table.GroupModel;
            if (groupModel != null && rowIndex > -1 && rowIndex < groupModel.DisplayElements.Count)
            {
                var item = groupModel.DisplayElements[rowIndex];
                if (item is RecordEntry)
                {
                    result = true;
                    ////var parentItem = groupModel.DisplayElements.GetParentAt(rowIndex);
                    ////if (!parentItem.IsExpanded.HasValue || (parentItem.IsExpanded.HasValue && parentItem.IsExpanded.Value))
                    ////{
                    ////    result = true;
                    ////}
                }
            }

            return result;
        }

        /// <summary>
        /// Resolves the index to group position in the GroupModel.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="actualRowIndex">Actual index of the row.</param>
        /// <returns></returns>
        public static int ResolveIndexToGroupPosition(this GridDataTableModel model, int actualRowIndex)
        {
            var counter0 = actualRowIndex - model.ResolveStartIndexBasedOnPosition();

            ////if (model.Table.HasNestedTables) // && model.Table.HasGroupsExpanded)
            ////{
            ////    var maxLevel = model.Table.GroupModel.GetMaxLevel();
            ////    if (counter0 > 0)
            ////    {
            ////        counter0 = (counter0 + maxLevel) / (model.TableProperties.Relations.Count + 1);
            ////    }
            ////}

            return counter0;
        }

        public static int ResolveIndexToStackHeaderRowPosition(this GridDataTableModel model, int actualRowIndex)
        {
            var totalCount = model.HeaderRows;
            if (actualRowIndex < totalCount)
            {
                return actualRowIndex;
            }

            return -1;
        }

        public static int ResolveEndIndexOfStackedHeaderColumn(this GridDataTableModel model, GridDataStackedHeaderRow stackedRow, GridDataStackedHeaderColumn stackedHeaderCol)
        {
            int index = 0;
            var stackedHeaderColIndex = stackedRow.Columns.IndexOf(stackedHeaderCol);
            for (int i = 0; i <= stackedHeaderColIndex; i++)
            {
                index += stackedRow.Columns[i].ColumnSpan;
            }

            return index;
        }

        public static int ResolveStartIndexOfStackedHeaderColumn(this GridDataTableModel model, GridDataStackedHeaderRow stackedRow, GridDataStackedHeaderColumn stackedHeaderCol, int actualColIndex)
        {
            int index = 0;
            var stackedHeaderColIndex = stackedRow.Columns.IndexOf(stackedHeaderCol);
            for (int i = 0; i < stackedHeaderColIndex; i++)
            {
                index += stackedRow.Columns[i].ColumnSpan;
            }

            return index;
        }

        public static GridDataStackedHeaderColumn ResolveIndexToStackHeaderColumn(this GridDataTableModel model, GridDataStackedHeaderRow stackedRow, int actualColIndex)
        {
            var colIndex = actualColIndex - model.ResolveDefaultColumnOffset();
            var counter0 = 0;
            GridDataStackedHeaderColumn stackedHeaderCol = null;
            for (int i = 0; i < model.ColumnCount; )
            {
                if (colIndex == i)
                {
                    stackedHeaderCol = counter0 < stackedRow.Columns.Count ? stackedRow.Columns[counter0] : null;
                    break;
                }
                else
                {
                    if (counter0 < stackedRow.Columns.Count)
                    {
                        var header = stackedRow.Columns[counter0];
                        if (colIndex < i + header.ColumnSpan)
                        {
                            stackedHeaderCol = header;
                            break;
                        }
                        else
                        {
                            i += stackedRow.Columns[counter0].ColumnSpan;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                counter0++;
            }

            return stackedHeaderCol;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Retunr the Range of TableSummary rows </returns>
        /// <remarks></remarks>
        internal static GridRangeInfo GetRangeOfTableSummaryRows(this GridDataTableModel model)
            {
            // 0-Header Rows
            // 1- Stacked Header Rows 
            // 2- UnboundRows - Top
            // 3 - TableSummaryRows - Top
            // 4-  FilterBar Row  -(Only Top)
            // 5- Addnew Row  - Top
            // 6 - Records
            // 7- AddNewRow -- Bottom
            // --  FilterBar Row - Bottom(To do : This is not implemented currently. )
            // 8- TableSummaryRow - Bottom
            // 9 - UnboundRows  - Bottom

            if (model.TableProperties.TableSummaryRows.Count > 0)
                {
                var startindex = 0;
                var endindex = 0;
                if (model.TableProperties.TableSummaryPosition == Position.Top)
                    {
                    startindex += model.TableProperties.HeaderRows + model.TableProperties.StackedHeaderRows.Count;
                    if (model.TableProperties.UnboundRowPosition == Position.Top)
                        {
                        startindex += model.UnboundRowsCount;
                        }
                    endindex = startindex + model.TableProperties.TableSummaryRows.Count;
                    }
                else
                    {
                    endindex = model.RowCount - 1;
                    if (model.TableProperties.UnboundRowPosition == Position.Bottom)
                        {
                        endindex -= model.UnboundRowsCount;
                        }
                    if (model.TableProperties.ShowAddNewRow && model.TableProperties.AddNewRowPosition == Position.Bottom)
                        {
                        endindex = endindex - 1;
                        }
                    startindex = endindex - (model.TableProperties.TableSummaryRows.Count - 1);
                    }

                return new GridRangeInfo(startindex, model.HeaderColumns, endindex, model.ColumnCount - 1);
                }
            else
                return new GridRangeInfo();
            }


        /// <summary>
        /// Returns the summary row index.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actualRowIndex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ResolveIndexToSummaryPosition(this GridDataTableModel model, int actualRowIndex)
        {
            // 0-Header Rows
            // 1- Stacked Header Rows 
            // 2- UnboundRows - Top
            // 3 - TableSummaryRows - Top
            // 4-  FilterBar Row  -(Only Top)
            // 5- Addnew Row  - Top
            // 6 - Records
            // 7- AddNewRow -- Bottom
            // --  FilterBar Row - Bottom(To do : This is not implemented currently. )
            // 8- TableSummaryRow - Bottom
            // 9 - UnboundRows  - Bottom
            if (model.TableProperties.TableSummaryRows.Count > 0)
            {
                var startindex = 0;
                var endindex = 0;
                if (model.TableProperties.TableSummaryPosition == Position.Top)
                {
                    startindex += model.TableProperties.HeaderRows + model.TableProperties.StackedHeaderRows.Count;
                    if (model.TableProperties.UnboundRowPosition == Position.Top)
                    {
                        startindex += model.UnboundRowsCount;
                    }
                    endindex = startindex + model.TableProperties.TableSummaryRows.Count;
                    if (actualRowIndex >= startindex && actualRowIndex < endindex)
                    {
                        return actualRowIndex - startindex;
                    }
                    else
                        return -1;
                }
                else
                {
                    endindex = model.RowCount - 1;
                    if (model.TableProperties.UnboundRowPosition == Position.Bottom)
                    {
                        endindex -= model.UnboundRowsCount;
                    }
                    //No need to consider AddNewRow here
                    //if (model.TableProperties.ShowAddNewRow && model.TableProperties.AddNewRowPosition == Position.Bottom)
                    //{
                    //    endindex = endindex - 1;
                    //}
                    startindex = endindex - (model.TableProperties.TableSummaryRows.Count - 1);
                    if (actualRowIndex >= startindex && actualRowIndex <= endindex)
                    {
                        return actualRowIndex - startindex;
                    }
                    else
                        return -1;
                }
            }
            else
                return -1;

        }

        public static bool IsInFilterBarPosition(this GridDataTableModel model, int actualRowIndex)
        {
            return model.TableProperties.ShowFilterBar && model.ResolveFilterBarPositionInGrid() == actualRowIndex ? true : false;
        }

        public static bool IsInSummaryPosition(this GridDataTableModel model, int actualRowIndex)
        {
            return model.Table.HasTableSummaries && model.ResolveIndexToSummaryPosition(actualRowIndex) >= 0 ? true : false;
        }

        /// <summary>
        /// Resolves the index of the group position to index in the grid.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        public static int ResolveGroupPositionToIndex(this GridDataTableModel model, int rowIndex)
        {
            var counter0 = rowIndex + model.ResolveStartIndexBasedOnPosition();
            return counter0;
        }
        
        public static void AdjustParents(this GridDataTableModel model, GridDataChildTableModel childModel, bool IsInCollaspeState)
        {
            if (childModel != null && !IsInCollaspeState && (model.ExpandedRecordCount == 1
                || (model is GridDataChildTableModel && (model as GridDataChildTableModel).ExpandedRecordCount == 1)))
            {
                var childTotalExtent = childModel.ColumnWidths.TotalExtent;
                var parentTotalExtent = childModel.ParentTable.Model.ColumnWidths.TotalExtent;
                double extraWidth = 0;
                if (model.Grid != null)
                {
                    for (int i = 0; i < model.Grid.NavigateWithArrowKeysCellsRange.Left; i++)
                        extraWidth += model.ColumnWidths[i];
                }
                if ((parentTotalExtent - extraWidth) <= childTotalExtent)
                {
                    var value = childTotalExtent - (parentTotalExtent - extraWidth);
                    if (model.TableProperties.AllowNestedGridPadding)
                        value += 8;//Added the nested grid padding
                    model.extendWidthValue += value;
                    foreach (var vColumn in model.TableProperties.VisibleColumns)
                    {
                        if (vColumn.Width.IsStar)
                        {
                            vColumn.IsLengthUnitTypeChanged = true;
                            model.TableProperties.VisibleColumns[vColumn.MappingName].Width = new GridDataControlLength(vColumn.ActualWidth, GridControlLengthUnitType.None);
                        }
                    }
                    model.ColumnWidths[model.ColumnCount - 2] += value;
                    //synchronise the model.columnWidth and visible column width
                    model.TableProperties.VisibleColumns[model.ResolvePositionToVisibleColumnIndex(model.ColumnCount - 2)].Width =
                        new GridDataControlLength(model.ColumnWidths[model.ColumnCount - 2]);
                    if (model is GridDataChildTableModel)
                    {
                        (model as GridDataChildTableModel).ParentTable.Model.AdjustParents(model as GridDataChildTableModel, IsInCollaspeState);//adjust parents at multiple child level.
                    }
                    else
                        model.AdjustParents(childModel.ParentTable.Model as GridDataChildTableModel, IsInCollaspeState);
                }
            }
            else if (childModel != null && IsInCollaspeState && model.extendWidthValue > 0 && (model.ExpandedRecordCount == 0
                || (model is GridDataChildTableModel && (model as GridDataChildTableModel).ExpandedRecordCount == 0)))
            {
                model.ColumnWidths[model.ColumnCount - 2] -= model.extendWidthValue;
                model.extendWidthValue = 0;
                model.TableProperties.VisibleColumns[model.ResolvePositionToVisibleColumnIndex(model.ColumnCount - 2)].Width =
                    new GridDataControlLength(model.ColumnWidths[model.ColumnCount - 2]);
                foreach (var vColumn in model.TableProperties.VisibleColumns)
                {
                    if (vColumn.IsLengthUnitTypeChanged)
                    {
                        vColumn.IsLengthUnitTypeChanged = false;
                        if (model.TableProperties.ColumnSizer == GridControlLengthUnitType.Star)
                            model.TableProperties.VisibleColumns[vColumn.MappingName].Width = new GridDataControlLength(vColumn.ActualWidth, GridControlLengthUnitType.Star);
                    }
                }
                if (model is GridDataChildTableModel)
                {
                    (model as GridDataChildTableModel).ParentTable.Model.AdjustParents(model as GridDataChildTableModel, IsInCollaspeState);
                }
                else
                    model.AdjustParents(childModel.ParentTable.Model as GridDataChildTableModel, IsInCollaspeState);
            }
        }

        public static void AdjustModelWidth(this GridDataTableModel model, double size)
        {
            if (!double.IsNaN(size) && size != 0)
            {
                var detailsViewTotalExtent = size;
                var gridTotalExtent = model.ColumnWidths.TotalExtent;
                if (gridTotalExtent < detailsViewTotalExtent)
                {
                    var value = detailsViewTotalExtent - gridTotalExtent;

                    // 24 is the default value for rowheader and recordplusminus
                    value += ((model.ResolveDefaultColumnOffset() + 1) * 24);

                    // add a padding value, just to make sure it looks neat
                    model.ColumnWidths[model.ColumnCount - 2] += value;
                    //synchronise the model.columnWidth and visible column width
                    model.TableProperties.VisibleColumns[model.ResolvePositionToVisibleColumnIndex(model.ColumnCount - 2)].Width = new GridDataControlLength(model.ColumnWidths[model.ColumnCount - 2]);
                    if (model is GridDataChildTableModel)
                    {
                        (model as GridDataChildTableModel).ParentTable.Model.AdjustModelWidth(model.ColumnWidths.TotalExtent);
                    }
                }
            }
        }

        public static void GetNestedTableIndentedValue(this GridDataTableModel model, ref int value)
        {
            if (model is GridDataChildTableModel)
            {
                GridDataChildTableModel childModel = model as GridDataChildTableModel;
                if (childModel.ParentTable != null)
                {
                    value++;
                }

                GetNestedTableIndentedValue(childModel.ParentTable.Model, ref value);
            }
        }

        public static int GetOrderForChildTableBasedOnIndex(this GridDataTableModel model, int actualRowIdx)
        {
            // simply find the index which is not in nestedIndex
            var counter0 = 0;
            for (int i = actualRowIdx; i > 0; i--)
            {
                if (!model.IsInNestedIndex(i))
                {
                    break;
                }
                counter0++;
            }
            /// adjusting the key value based on the existance of details view
            return model.Table.HasDetailsView ? counter0 - 1 : counter0;
        }

        /// <summary>
        /// Returns the starting index for records.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ResolveStartIndexBasedOnPosition(this GridDataTableModel model)
        {
            var startIdx = model.TableProperties.HeaderRows > 0 ? model.TableProperties.HeaderRows : 0;

            startIdx = model.TableProperties.ShowFilterBar ? (startIdx + 1) : startIdx;
            // Check the add new row if it is at Top
            if (model.TableProperties.AddNewRowPosition == Position.Top)
            {
                startIdx += model.TableProperties.ShowAddNewRow ? 1 : 0;
            }
            // Checks the stacked headers
            if (model.Table.HasStackedHeaders)
            {
                startIdx += model.TableProperties.StackedHeaderRows.Count;
            }
            // Checks the unbound rows 
            if (model.TableProperties.UnboundRowPosition == Position.Top)
            {
                startIdx += model.UnboundRowsCount;
            }
            // cehck the Table Summary Rows 
            if (model.TableProperties.TableSummaryPosition == Position.Top)
            {
                startIdx += model.TableProperties.TableSummaryRows.Count;
            }

            return startIdx;
        }

        public static bool IsInHiddenState(this GridDataTableModel model, int rowIndex)
        {
            var lineSizeCollection = model.RowHeights as LineSizeCollection;
            int repeatCount = 0;
            return lineSizeCollection.GetHidden(rowIndex, out repeatCount);
        }

        public static bool IsInNestedIndex(this GridDataTableModel model, int rowIdx)
        {
            if (model.IsInHiddenState(rowIdx))
            {
                return true;
            }

            var startIdx = model.ResolveStartIndexBasedOnPosition();
            var counter0 = Math.Max((rowIdx - startIdx), 0);

            if (model.Table.HasGroups)
            {
                var groupIdx = model.ResolveIndexToGroupPosition(rowIdx);
                var displayEl = model.Table.GroupModel.DisplayElements[groupIdx];
                if (displayEl is NestedRecordEntry)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// To resolve the nested index when grid has nested table with details veiw
            ///(this works when any one of this is in expand state)
            if (model.Table.HasNestedTables || model.Table.HasDetailsView)
            {
                return (counter0 % (model.TableProperties.Relations.Count + 1 + (model.Table.HasDetailsView ? 1 : 0))) != 0;
            }

            return false;
        }

        /// <summary>
        /// Returns if the actual row is in Unbound Row.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actualRowIdx"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static bool IsInUnboundRow(this GridDataTableModel model, int actualRowIdx)
        {
        var unboundRowsCount = model.UnboundRowsCount;

        // we might have multiple header rows, and so we would want these values to be considered too.
        // 0 colheader
        // 1 colheader
        // 2 unbound
        // 3 unbound
        // 4 unbound
        if (model.TableProperties.UnboundRowPosition == Position.Top)
            {
            unboundRowsCount += model.HeaderRows;
            if (actualRowIdx >= model.HeaderRows && actualRowIdx < unboundRowsCount)
                return true;
            else
                return false;
            }
        else
            {
            return actualRowIdx > ((model.RowCount - 1) - model.UnboundRowsCount);
            }
        }

        public static void RefreshParentGrids(this GridDataTableModel model)
        {
            if (model is GridDataChildTableModel)
            {
                var childModel = model as GridDataChildTableModel;
                childModel.ParentTable.Model.RefreshParentGrids();
            }

            //// model.Grid.InvalidateCells();
            model.InvalidateDisplay();
            model.InvalidateVisual(true);
        }

        public static int ResolveDefaultColumnOffset(this GridDataTableModel model)
        {
            var value = model.TableProperties.ShowRowHeader ? 1 : 0;
            value += model.Table.HasNestedTables ? 1 : 0;
            value += model.Table.HasDetailsView ? 1 : 0;

            if (model.Table.HasGroups && model.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = model.Table.GroupModel.GetMaxLevel();
                value += maxLevel;
            }

            //// if we have any nested tables, get the indented value
            ////var indentedValue = 0;
            ////model.GetIndentedValue(ref indentedValue);
            ////colIdx = indentedValue > 0 ? colIdx + indentedValue : colIdx;
            return value;
        }

        public static int ResolveDefaultNestedIndex(this GridDataTableModel model)
        {
            return model.Table.HasNestedTables ? 2 : 1;
        }

        /// <summary>
        /// Resolves row index to position in the records collection.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="actualRowIdx"></param>
        public static int ResolveIndexToRecordPosition(this GridDataTableModel model, int actualRowIdx)
        {
            var startIdx = model.ResolveStartIndexBasedOnPosition();
            var counter0 = actualRowIdx - startIdx;
            if (counter0 < 0 )
            {
                return -1;
            }

            if (model.Table.HasGroups)
            {
                //var groupIdx = model.ResolveIndexToGroupPosition(actualRowIdx);
                var indexToReturn = -1;
                if (counter0 > -1 && counter0 < model.Table.GroupModel.DisplayElements.Count)
                {
                    var displayEl = model.Table.GroupModel.DisplayElements[counter0];
                    var isRecord = (displayEl is RecordEntry) && !(displayEl is NestedRecordEntry);
                    if (isRecord)
                    {
                        var record = (displayEl as RecordEntry);
                        indexToReturn = model.View.TopLevelGroup.DisplayElements.IndexOf(record);
                    }
                    else if (displayEl is NestedRecordEntry)
                    {
                        var parent = (displayEl as NestedRecordEntry).Parent as RecordEntry;                        
                        indexToReturn = model.View.TopLevelGroup.DisplayElements.IndexOf(parent);
                    }
                }
                return indexToReturn;
            }

            if (model.Table.HasNestedTables | model.Table.HasDetailsView)
            {
                /// calculating the devisor to resolve the current index
                int divisor = model.TableProperties.Relations.Count + 1 + (model.Table.HasDetailsView ? 1 : 0);

                if (counter0 > 0)
                {
                    counter0 = counter0 / divisor;
                }
                else
                {
                    counter0 = (actualRowIdx - startIdx) / divisor;
                }
            }

            return counter0;
        }

        /// <summary>
        /// Resolves the index in the Grid from the Records collection.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="rowIdx">The row idx.</param>
        /// <returns></returns>
        public static int ResolvePositionToIndex(this GridDataTableModel model, int recordIndex)
        {
            if (recordIndex < 0)
                return -1;
            var startIdx = model.ResolveStartIndexBasedOnPosition();
            if (model.Table.HasGroups)
            {
                //var rec = model.Binder.GetItemAt(recordIndex);
                //var group = model.Table.GroupModel.DisplayElements.Where(g => g.GroupType == GroupType.Item && Object.Equals(g.Item, rec)).FirstOrDefault();
                //var groupPos = model.Table.GroupModel.DisplayElements.IndexOf(group);
                if (recordIndex > -1)
                {
                    var groupPos = recordIndex + model.ResolveStartIndexBasedOnPosition();
                    return groupPos;
                }

                return -1;
            }
            // Adjusting the index based on the relation count and details view
            return (recordIndex * (model.TableProperties.Relations.Count + 1 + (model.Table.HasDetailsView ? 1 : 0))) + startIdx;
        }

        public static int ResolveGroupRecordPositionToIndex(this GridDataTableModel model, int recordIndex)
        {
            if (recordIndex > -1)
            {
                var record = model.View.Records[recordIndex];
                var grpRecordIndex = model.View.TopLevelGroup.DisplayElements.IndexOf(record);
                if (grpRecordIndex > -1)
                {
                    var groupPos = grpRecordIndex + model.ResolveStartIndexBasedOnPosition();
                    return groupPos;
                }
            }

            return -1;
        }

        public static int ResolvePositionToVisibleColumnIndex(this GridDataTableModel model, int colIdx)
        {
            if (model == null || model.Table==null || model.TableProperties == null)
            {
                return -1;
            }

            if (model.Table.HasNestedTables)
            {
                colIdx = model.TableProperties.ShowRecordPlusMinus ? colIdx - 1 : colIdx;
            }
            /// Adjusting the column index based on details view existance
            colIdx -= model.Table.HasDetailsView ? 1 : 0;

            if (model.Table.HasGroups && model.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = model.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx - maxLevel;
            }

            //// adjust the value of colIndex to get the exact column
            colIdx = model.TableProperties.ShowRowHeader ? colIdx - 1 : colIdx;

            return colIdx;
        }

        public static int ResolveVisibleColumnIndexToPosition(this GridDataTableModel model, int visibleColIndex)
        {
            var defaultOffset = model.ResolveDefaultColumnOffset();
            return defaultOffset + visibleColIndex;
        }

        public static GridRangeInfo ExpandRange(this GridDataTableModel model, GridRangeInfo range)
        {
            if (model.Table.HasNestedTables)
            {
                return range.ExpandRange(0, 0, model.RowCount, model.ColumnCount - 1);
            }

            return range.ExpandRange(0, 0, model.RowCount, model.ColumnCount);
        }

        public static int ResolveStartIndexOfGroup(this GridDataTableModel model, Group group)
        {
            if (model.Table.HasGroups)
            {
                var startIndex = model.ResolveStartIndexBasedOnPosition();
                var grpIdx = model.Table.GroupModel.DisplayElements.IndexOf(group);
                return grpIdx + startIndex;
            }
            return -1;
        }

        /// <summary>
        /// Retunrs the Add new Row Position
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ResolveAddNewPositionInGrid(this GridDataTableModel model)
        {
            if (model.TableProperties.ShowAddNewRow)
            {
                var addNewRowIndex = model.TableProperties.AddNewRowPosition == Position.Top ? model.ResolveStartIndexBasedOnPosition() - 1 : model.RowCount - 1;
                if (model.TableProperties.UnboundRowPosition == Position.Bottom && model.TableProperties.AddNewRowPosition == Position.Bottom)
                {
                    addNewRowIndex -= model.UnboundRowsCount;
                }
                //AddNewRowIndex consider with TableSummaryPosition as Bottom
                if (model.Table.HasTableSummaries && model.TableProperties.ShowTableSummaries && model.TableProperties.TableSummaryPosition == Position.Bottom && model.TableProperties.AddNewRowPosition == Position.Bottom)
                {
                    addNewRowIndex -= model.TableProperties.TableSummaryRows.Count;
                }
                return addNewRowIndex;
            }

            return -1;
        }



        /// <summary>
        ///  Returns the Filterbar Position.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int ResolveFilterBarPositionInGrid(this GridDataTableModel model)
        {
            var filterBarIndex = -1;
            if (model.TableProperties.ShowFilterBar)
            {
            if (model.TableProperties.ShowAddNewRow)
            {
                filterBarIndex = model.TableProperties.AddNewRowPosition == Position.Top
                                     ? model.ResolveStartIndexBasedOnPosition() - 2
                                     : model.ResolveStartIndexBasedOnPosition() - 1;
            }
            else
               filterBarIndex = model.ResolveStartIndexBasedOnPosition() - 1;
                return filterBarIndex;
            }

            return -1;
        }
    }

    internal class ListChangedArgs
    {
        public ListChangedArgs()
        {
            this.NewIndex = -1;
            this.OldIndex = -1;
        }

        public int NewIndex
        {
            get;
            set;
        }

        public IList NewItems
        {
            get;
            internal set;
        }

        public int OldIndex
        {
            get;
            set;
        }

        public IList OldItems
        {
            get;
            internal set;
        }
    }

    /*
    public delegate void GridDataModelBinderEventHandler(object sender, GridDataModelBinderEventArgs args);
    public class GridDataModelBinderEventArgs : SyncfusionHandledEventArgs
    {
        public GridDataModelBinderEventArgs()
        {
        }

        public object DataSource
        {
            get;
            internal set;
        }

        public GridDataTableModelBinder Binder
        {
            get;
            set;
        }
    }*/
    //Temp Added
    internal class ExtensionProperties : DependencyObject
    {
        /// <summary>
        /// Tracks whether or not the event handlers of a particular object are currently suspended.
        /// Used by the SetValueNoCallback and AreHandlersSuspended extension methods.
        /// </summary>
        public static readonly DependencyProperty AreHandlersSuspended = DependencyProperty.RegisterAttached(
            "AreHandlersSuspended",
            typeof(Boolean),
            typeof(ExtensionProperties),
            new PropertyMetadata(false)
        );
        public static void SetAreHandlersSuspended(DependencyObject obj, Boolean value)
        {
            obj.SetValue(AreHandlersSuspended, value);
        }
        public static Boolean GetAreHandlersSuspended(DependencyObject obj)
        {
            return (Boolean)obj.GetValue(AreHandlersSuspended);
        }
    }
    //Till This

    internal static class Extensions
    {

        //Temp Added


        public static bool AreHandlersSuspended(this DependencyObject obj)
        {
            return ExtensionProperties.GetAreHandlersSuspended(obj);
        }

        /// <summary>
        /// Walks the visual tree to determine if a particular child is contained within a parent DependencyObject.
        /// </summary>
        /// <param name="element">Parent DependencyObject</param>
        /// <param name="child">Child DependencyObject</param>
        /// <returns>True if the parent element contains the child</returns>
        internal static bool ContainsChild(this DependencyObject element, DependencyObject child)
        {
            if (element != null)
            {
                while (child != null)
                {
                    if (child == element)
                    {
                        return true;
                    }

                    // Walk up the visual tree.  If we hit the root, try using the framework element's
                    // parent.  We do this because Popups behave differently with respect to the visual tree,
                    // and it could have a parent even if the VisualTreeHelper doesn't find it.
                    DependencyObject parent = VisualTreeHelper.GetParent(child);
                    if (parent == null)
                    {
                        FrameworkElement childElement = child as FrameworkElement;
                        if (childElement != null)
                        {
                            parent = childElement.Parent;
                        }
                    }
                    child = parent;
                }
            }
            return false;
        }

        /// <summary>
        /// Walks the visual tree to determine if the currently focused element is contained within
        /// a parent DependencyObject.  The FocusManager's GetFocusedElement method is used to determine
        /// the currently focused element, which is updated synchronously.
        /// </summary>
        /// <param name="element">Parent DependencyObject</param>
        /// <returns>True if the currently focused element is within the visual tree of the parent</returns>
        internal static bool ContainsFocusedElement(this DependencyObject element)
        {
#if !SILVERLIGHT

            return (element == null) ? false : element.ContainsChild(FocusManager.GetFocusedElement(element) as DependencyObject);
#else
             return (element == null) ? false : element.ContainsChild(FocusManager.GetFocusedElement() as DependencyObject);
#endif
        }

        /// <summary>
        /// Checks a MemberInfo object (e.g. a Type or PropertyInfo) for the ReadOnly attribute
        /// and returns the value of IsReadOnly if it exists.
        /// </summary>
        /// <param name="memberInfo">MemberInfo to check</param>
        /// <returns>true if MemberInfo is read-only, false otherwise</returns>
        internal static bool GetIsReadOnly(this MemberInfo memberInfo)
        {
            if (memberInfo != null)
            {
                // Check if ReadOnlyAttribute is defined on the member
                object[] attributes = memberInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    ReadOnlyAttribute readOnlyAttribute = attributes[0] as ReadOnlyAttribute;
                    Debug.Assert(readOnlyAttribute != null);
                    return readOnlyAttribute.IsReadOnly;
                }
            }
            return false;
        }

        internal static Type GetItemType(this IEnumerable list)
        {
            // Type listType = list.GetType();
            Type itemType = null;

            // if it's a generic enumerable, we get the generic type

            // Unfortunately, if data source is fed from a bare IEnumerable, TypeHelper will report an element type of object,
            // which is not particularly interesting.  We deal with it further on.
            //if (listType.IsEnumerableType())// Temp Commented
            //{
            //    itemType = listType.GetEnumerableItemType();
            //}

            // Bare IEnumerables mean that result type will be object.  In that case, we try to get something more interesting
            if (itemType == null || itemType == typeof(object))
            {
                // We haven't located a type yet.. try a different approach.
                // Does the list have anything in it?

                IEnumerator en = list.GetEnumerator();
                if (en.MoveNext() && en.Current != null)
                {
                    return en.Current.GetType();
                }
            }

            // if we're null at this point, give up
            return itemType;
        }

        public static void SetStyleWithType(this FrameworkElement element, Style style)
        {
            if (element.Style != style && (style == null || style.TargetType != null))
            {
                element.Style = style;
            }
        }

        public static void SetValueNoCallback(this DependencyObject obj, DependencyProperty property, object value)
        {
            ExtensionProperties.SetAreHandlersSuspended(obj, true);
            try
            {
                obj.SetValue(property, value);
            }
            finally
            {
                ExtensionProperties.SetAreHandlersSuspended(obj, false);
            }
        }

        internal static Point Translate(this UIElement fromElement, UIElement toElement, Point fromPoint)
        {
            if (fromElement == toElement)
            {
                return fromPoint;
            }
            else
            {
                return fromElement.TransformToVisual(toElement).Transform(fromPoint);
            }
        }

        internal static bool Within(this Point referencePoint, UIElement referenceElement, FrameworkElement targetElement, bool ignoreVertical)
        {
            Point position = referenceElement.Translate(targetElement, referencePoint);

            return position.X > 0 && position.X < targetElement.ActualWidth
                && (ignoreVertical
                    || (position.Y > 0 && position.Y < targetElement.ActualHeight)
                );
        }
    
    
        //Till This
#if !SILVERLIGHT
        public static ObservableCollection<T> OfType<T, K>(this FreezableCollection<K> items)
            where K : DependencyObject, T
        {
#else
        public static ObservableCollection<T> OfType<T, K>(this ObservableCollection<K> items)
           where K : T
        {

#endif
            ObservableCollection<T> newItems = new ObservableCollection<T>();
            foreach (var item in items)
            {
                var typeCastedItem = (T)item;
                if (typeCastedItem != null)
                {
                    newItems.Add(typeCastedItem);
                }
            }

            return newItems;
        }
    }
#if SyncfusionFramework4_0
    internal class GridDataVisibleColumnDisplayAttributeComparer : IComparer<GridDataVisibleColumn>
    {
#if!SILVERLIGHT
        PropertyDescriptorCollection itemProperties;

        internal GridDataVisibleColumnDisplayAttributeComparer(PropertyDescriptorCollection itemProperties)
        {
#else
        PropertyInfoCollection itemProperties;
        internal GridDataVisibleColumnDisplayAttributeComparer(PropertyInfoCollection itemProperties)
        {
#endif

            this.itemProperties = itemProperties;
        }

        #region IComparer<GridDataVisibleColumn> Members

        public int Compare(GridDataVisibleColumn x, GridDataVisibleColumn y)
        {
            var xPd = this.itemProperties.Find(x.MappingName, false);
            var yPd = this.itemProperties.Find(y.MappingName, false);
#if !SILVERLIGHT
            var xDisplayAttribute = (System.ComponentModel.DataAnnotations.DisplayAttribute)xPd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
            var yDisplayAttribute = (System.ComponentModel.DataAnnotations.DisplayAttribute)yPd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
#else
            var xDisplayAttribute = (System.ComponentModel.DataAnnotations.DisplayAttribute)xPd.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute)); //xPd.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
            var yDisplayAttribute = (System.ComponentModel.DataAnnotations.DisplayAttribute)yPd.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
#endif

            bool xIsNull = xDisplayAttribute == null;
            bool yIsNull = yDisplayAttribute == null;

            int cmp = -1;
            if (yIsNull && xIsNull)
            {
                cmp = 1;
            }
            else if (xIsNull)
            {
                cmp = 0;
            }
            else if (yIsNull)
            {
                cmp = -1;
            }
            else if (xDisplayAttribute.GetOrder() != null && yDisplayAttribute.GetOrder() != null)
            {
                cmp = xDisplayAttribute.Order.CompareTo(yDisplayAttribute.Order);
            }

            return cmp;
        }

        #endregion
    }
#endif

}