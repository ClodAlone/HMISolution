#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Reports;
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Syncfusion.Windows.Controls.Grid;    
    using System.Collections.Generic;
    using Syncfusion.Windows;
#if !SILVERLIGHT
    using Syncfusion.Olap.Manager;
#endif
    
    /// <summary>
    ///  The GridModel class for OlapGrid
    /// </summary>
    public class OlapGridModel : GridModel
    {
        #region Variables

        private PivotEngine _Engine;

        #region Paging Members
#if !SILVERLIGHT

        private PivotEngine _PagingEngine = null;

        private int _StartIndex = -1;

        private int _EndIndex = -1;
#endif

        #endregion

        #endregion

        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridModel"/> class.
        /// </summary>
        public OlapGridModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridModel"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        public OlapGridModel(OlapGridBase grid)
        {
            this.Grid = grid;
            this.CellModels.Add("GridExpandHyperlinkCell", new OlapGridExpandHyperlinkCellModel());
            this.CellModels.Add("GridHyperlinkCell", new OlapGridHyperlinkCellModel());
            this.CellModels.Add("GridKpiCell", new OlapGridKpiCellModel());
            this.CellModels.Add("GridTemplateCell", new OlapGridTemplateCellModel());
            this.TableStyle.CellType = "Static";
            this.HeaderStyle.CellType = "Static";
#if !SILVERLIGHT
            this.HeaderStyle.FlowDirection = grid.FlowDirection;
            this.TableStyle.FlowDirection = grid.FlowDirection;
#endif
            if (this.Grid.ShowValueCellToolTip || this.Grid.ShowHeaderCellsToolTip)
            {
                /// Enabling tooltip for cells
                GridTooltipService.SetShowTooltips(this.Grid, true);                      
            }
            InvalidateSelection();
        }

        #endregion

        #region Properties

        #region Paging Properties

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the start index. Indicates the starting row of the paging grid.
        /// </summary>
        /// <value>The start index.</value>
        public int StartIndex
        {
            get
            {
                return _StartIndex;
            }
            set
            {
                _StartIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the end index. Indicats the ending row of the paging grid.
        /// </summary>
        /// <value>The end index.</value>
        public int EndIndex
        {
            get
            {
                return _EndIndex;
            }
            set
            {
                _EndIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the paging engine. Backup engine of the paging grid.
        /// </summary>
        /// <value>The paging engine.</value>
        [DefaultValue(null)]
        public PivotEngine PagingEngine
        {
            get
            {
                return _PagingEngine;
            }
            set
            {
                _PagingEngine = value;
            }
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

        /// <summary>
        /// Gets or sets the length of the horizontal header. Assigned from Olap grid base.
        /// </summary>
        /// <value>The length of the horizontal header.</value>
        public int HorizontalHeaderLength
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the vertical header.
        /// </summary>
        /// <value>The length of the vertical header.</value>
        public int VerticalHeaderLength
        {
            private get;
            set;
        }

#endif
        #endregion

        /// <summary>
        /// Gets or sets the engine.
        /// </summary>
        /// <value>The engine.</value>
        [DefaultValue(null)]
        public PivotEngine Engine
        {
            get
            {
                return _Engine;
            }
            set
            {

                if (value != this._Engine)
                {
                    _Engine = value;
                    if (_Engine != null)
                    {
                        this.Refresh();
                    }
                    else
                    {
                        this.InvalidateDisplay();
                        this.ResetVisibleRowColCount();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public OlapGridBase Grid
        {
            get;
            private set;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the RunWorkerCompleted event of the bgWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Grid.IsProcessing = false;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Does the events.
        /// </summary>
        private void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            },
            f);
            Dispatcher.PushFrame(f);
        }
#endif

        /// <summary>
        /// Handles the Completed event of the op control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void op_Completed(object sender, EventArgs e)
        {
            this.Grid.IsProcessing = false;
#if !SILVERLIGHT
            this.Grid.RaiseGridCellCursor();
#endif
            //this.Grid.RaiseAfterDrillDown(cellDescriptor, e);
        }

        /// <summary>
        /// Processes the cell click.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridBase">The grid base.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        private void ProcessCellClick(PivotCellDescriptor cellDescriptor, OlapGridBase gridBase, OlapGridDrillDownEventArgs e)
        {
#if Debug
            var sw = new Stopwatch();
            sw.Start();
#endif
            try
            {
                if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader ||
                    cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                {
#if !SILVERLIGHT
                    if (gridBase.DataManager.CurrentReport.DrillType != DrillType.DrillPosition && !gridBase.DataManager.UseSharedDataManager)
                    {
                        gridBase.DataManager.ToggleExpandableState(cellDescriptor, this.Grid.Layout);
                    }
                    else if (gridBase.DataManager.ActiveReport != null && !string.IsNullOrEmpty(gridBase.OlapGrid.ReportName))
                    {
                        gridBase.DataManager.ActiveReport = gridBase.DataManager.Reports[gridBase.OlapGrid.ReportName];
                        gridBase.DataManager.ToggleExpandableState(cellDescriptor, this.Grid.Layout);
                        CellSet cellSet = gridBase.DataManager.ExecuteCellSet();
                        PivotEngine engine = gridBase.DataManager.ExecuteOlapTable(cellSet,this.Grid.Layout);
                        gridBase.OlapGrid.InternalGrid.Engine = engine;
                        gridBase.DataManager.NotifyActiveReportChanged();
                    }
                    else
                        gridBase.DataManager.ToggleExpandableStateOnDrillPosition(cellDescriptor);

                    gridBase.CurrentCell.MoveTo(-1, -1);
#else
                    if ((gridBase.DataManager.CurrentReport.DrillType != DrillType.DrillPosition && gridBase.DataManager.ToggleExpandableState(cellDescriptor.CellType, (Member)e.CellDescriptor.Tag))
                        || (gridBase.DataManager.CurrentReport.DrillType == DrillType.DrillPosition && gridBase.DataManager.ToggleExpandableStateOnDrillPosition(cellDescriptor)))
                    {
                        gridBase.DataManager.ExecuteCellSet();
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                this.Grid.IsProcessing = false;
                throw ex;
            }
#if Debug
            sw.Stop();
            Console.WriteLine("Time {0}", sw.Elapsed);
#endif
        }

#if !SILVERLIGHT
        /// <summary>
        /// Processes the cell click.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="gridBase">The grid base.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        private void ProcessCellClick(PivotCellDescriptor cellDescriptor, OlapGrid gridBase, OlapGridDrillDownEventArgs e)
        {
#if Debug
            var sw = new Stopwatch();
            sw.Start();
#endif
            try
            {
                if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader ||
                    cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                {
                    if (gridBase.OlapDataManager.CurrentReport.DrillType != DrillType.DrillPosition && !(gridBase.OlapDataManager as OlapDataManager).UseSharedDataManager)
                    {
                        gridBase.OlapDataManager.ToggleExpandableState(cellDescriptor, this.Grid.Layout);
                    }
                    else if ((gridBase.OlapDataManager as OlapDataManager).ActiveReport != null && !string.IsNullOrEmpty(gridBase.ReportName))
                    {
                        (gridBase.OlapDataManager as OlapDataManager).ActiveReport = gridBase.OlapDataManager.Reports[gridBase.ReportName];
                        gridBase.OlapDataManager.ToggleExpandableState(cellDescriptor, this.Grid.Layout);
                        CellSet cellSet = gridBase.OlapDataManager.ExecuteCellSet();
                        PivotEngine engine = gridBase.OlapDataManager.ExecuteOlapTable(cellSet, this.Grid.Layout);
                        gridBase.InternalGrid.Engine = engine;
                        (gridBase.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                    }
                    else
                        (gridBase.OlapDataManager as OlapDataManager).ToggleExpandableStateOnDrillPosition(cellDescriptor);

                    gridBase.InternalGrid.CurrentCell.MoveTo(-1, -1);
                }
            }
            catch (Exception ex)
            {
                this.Grid.IsProcessing = false;
                throw ex;
            }
#if Debug
            sw.Stop();
            Console.WriteLine("Time {0}", sw.Elapsed);
#endif
        }
#endif

        /// <summary>
        /// Resets the visible row col count.
        /// </summary>
        private void ResetVisibleRowColCount()
        {
            this.RowCount = 1;
            this.Reset();
            this.ColumnCount = 1;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        private void Reset()
        {
            this.VolatileCellStyles.Clear();
            this.Data.Clear();
            this.CoveredCells.Clear();
            this.CellSpanBackgrounds.Clear();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets the grid cell style info.
        /// </summary>
        /// <param name="gridCellStyle">The grid cell style.</param>
        /// <returns></returns>
        internal GridStyleInfo GetGridCellStyleInfo(OlapGridCellStyle gridCellStyle)
        {
            GridStyleInfo gridStyleInfo = new GridStyleInfo();
            gridStyleInfo.Background = gridCellStyle.Background;
            gridStyleInfo.Font.FontFamily = gridCellStyle.FontFamily;
            gridStyleInfo.Font.FontSize = gridCellStyle.FontSize;
            gridStyleInfo.Font.FontWeight = gridCellStyle.FontWeight;
            gridStyleInfo.Foreground = gridCellStyle.Foreground;            
            return gridStyleInfo;
        }

        /// <summary>
        /// Grids the expander click.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        internal void GridExpanderClick(PivotCellDescriptor cellDescriptor, OlapGridDrillDownEventArgs e)
        {
            try
            {
                this.Grid.IsProcessing = true;
#if !SILVERLIGHT
                this.Grid.RaiseBeforeDrillDown(cellDescriptor, e);
                //ThreadStart start = delegate()
                //{
                //    DispatcherOperation op = this.Grid.Dispatcher.BeginInvoke(
                //        DispatcherPriority.Background,
                //        new Action<PivotCellDescriptor, OlapGridBase, OlapGridDrillDownEventArgs>
                //            (ProcessCellClick), cellDescriptor, this.Grid, e);
                //    op.Completed += new EventHandler(op_Completed);
                //};
                //new Thread(start).Start();
                this.DoEvents();
#endif
                if (this.Grid.OlapGrid != null && this.Grid.OlapGrid.OlapDataManager != null)
                {
#if !SILVERLIGHT
                    if ((this.Grid.OlapGrid.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        ProcessCellClick(cellDescriptor, this.Grid.OlapGrid, e);
                    else
#endif
                        ProcessCellClick(cellDescriptor, this.Grid, e);
                }
#if !SILVERLIGHT
                this.DoEvents();

                this.Grid.RaiseAfterDrillDown(cellDescriptor, e);
#endif
                this.Grid.IsProcessing = false;

            }
            catch (Exception ex)
            {
                this.Grid.IsProcessing = false;
                throw ex;
            }
        }

        private void InvalidateSelection()
        {
            if (this.Grid.AllowSelection)
            {
                this.Options.ExcelLikeCurrentCell = true;
                this.Options.ExcelLikeSelectionFrame = true;
                this.Options.AllowSelection = GridSelectionFlags.Cell;
            }
            else
            {
                this.Options.AllowSelection = GridSelectionFlags.None;
                this.Options.ShowCurrentCell = false;
                this.Options.ExcelLikeCurrentCell = false;
                this.Options.ListBoxSelectionMode = GridSelectionMode.None;
            }
        }

        /// <summary>
        /// Invalidates the display.
        /// </summary>
        internal void InvalidateDisplay()
        {
            //// Invalidating the allow selection settings
            InvalidateSelection();

            if (this.Grid != null)
            {
                this.Grid.RenderStyles.Clear();
                this.Grid.ArrangedCellUIElements.UnloadAll();
#if !SILVERLIGHT
                this.Grid.RenderedCellVisuals.Invalidate();
#endif
            }

            this.CoveredCells.Clear();
            this.VolatileCellStyles.Clear();
            this.Data.Clear();
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the <see cref="E:QueryCellInfo"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventArgs"/> instance containing the event data.</param>
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            base.OnQueryCellInfo(e);
            if (e.Handled)
                return;
            OlapGridCellStyleInfoIdentity cellIdentity = e.Style.Tag as OlapGridCellStyleInfoIdentity;
            
            if (cellIdentity != null)
            {
                switch (cellIdentity.CellDescriptor.CellType)
                {
                    case PivotCellDescriptorType.ColumnHeader:
                        {
                            this.UpdateCellStyle(e.Style, this.Grid.ColumnHeaderStyle, cellIdentity);
                            if (this.Grid.ColumnHeaderStyle != null && this.Grid.ColumnHeaderStyle.Style != null)
                            {
                                e.Style.CellType = "GridTemplateCell";
                            }

                            //// Header cell's tooltip settings
                            if (this.Grid.ShowHeaderCellsToolTip)
                            {
                                Member member = cellIdentity.CellDescriptor.Tag as Member;
                                if (member != null && member.Type != MemberTypeEnum.Measure && cellIdentity.CellDescriptor.HasChildren)
                                {
                                    //// Template defined in Generic.xaml
                                    e.Style.TooltipTemplateKey = "toolTipHeaderCells";
                                    e.Style.ShowTooltip = true;
                                }
                            }
                            else if (this.Grid.ShowMemberPropertiesToolTip)
                            {
                                Member member = cellIdentity.CellDescriptor.Tag as Member;
                                if (member != null && member.Type != MemberTypeEnum.Measure)
                                {
                                    //// Template defined in Generic.xaml
                                    e.Style.TooltipTemplateKey = "MemberPropertytoolTipHeaderCells";
                                    e.Style.ShowTooltip = true;
                                }
                            }
                            else
                            {
                                e.Style.TooltipTemplateKey = string.Empty;
                                e.Style.ShowTooltip = false;
                            }
                            break;
                        }
                    case PivotCellDescriptorType.RowHeader:
                        {
                            this.UpdateCellStyle(e.Style, this.Grid.RowHeaderStyle, cellIdentity);

                            if (this.Grid.RowHeaderStyle!=null && this.Grid.RowHeaderStyle.Style != null)
                            {
                                e.Style.CellType = "GridTemplateCell";
                            }

                            //// Header cell's tooltip settings
                            if (this.Grid.ShowHeaderCellsToolTip && ((this.Grid.OlapGrid.OlapDataManager != null && this.Grid.OlapGrid.OlapDataManager.ItemSource == null) ||(this.Grid.DataManager !=null &&  this.Grid.DataManager.ItemSource == null)))
                            {
                                Member member = cellIdentity.CellDescriptor.Tag as Member;
                                if (member != null && member.Type != MemberTypeEnum.Measure && cellIdentity.CellDescriptor.HasChildren)
                                {
                                    //// Template defined in Generic.xaml
                                    e.Style.TooltipTemplateKey = "toolTipHeaderCells";
                                   
                                   

                                    e.Style.ShowTooltip = true;
                                }
                            }
                            else if (this.Grid.ShowMemberPropertiesToolTip)
                            {
                                Member member = cellIdentity.CellDescriptor.Tag as Member;
                                if (member != null && member.Type != MemberTypeEnum.Measure)
                                {
                                    //// Template defined in Generic.xaml
                                    e.Style.TooltipTemplateKey = "MemberPropertytoolTipHeaderCells";
                                    e.Style.ShowTooltip = true;
                                }
                            }
                            else
                            {
                                e.Style.TooltipTemplateKey = string.Empty;
                                e.Style.ShowTooltip = false;
                            }
                            break;
                        }
                    case PivotCellDescriptorType.SummaryColumn:
                        {
                            if (this.Grid.SummaryColumnStyle != null)
                            {
                                if (this.Grid.SummaryColumnStyle.Style != null && ((this.Grid.OlapGrid.OlapDataManager != null && this.Grid.OlapGrid.OlapDataManager.ItemSource == null) ||(this.Grid.DataManager != null && this.Grid.DataManager.ItemSource == null)))
                                {
                                    e.Style.CellType = "GridTemplateCell";
                                    cellIdentity.Style = this.Grid.SummaryColumnStyle.Style;
                                }
                                this.UpdateCellStyle(e.Style, this.Grid.SummaryColumnStyle, cellIdentity);
                            }
                            break;
                        }
                    case PivotCellDescriptorType.SummaryRow:
                        {
                            if (this.Grid.SummaryRowStyle != null)
                            {
                                if (this.Grid.SummaryRowStyle.Style != null)
                                {
                                    e.Style.CellType = "GridTemplateCell";
                                    cellIdentity.Style = this.Grid.SummaryRowStyle.Style;
                                }
                                this.UpdateCellStyle(e.Style, this.Grid.SummaryRowStyle, cellIdentity);
                            }
                            break;
                        }
                    case PivotCellDescriptorType.Value:
                        {

                            UpdateCellStyle(e.Style, this.Grid.ValueCellStyle, cellIdentity);
                            //// Value cell Tooltip settings
                            if (this.Grid.ShowValueCellToolTip)
                            {
                                /// Template defined in Generic.xaml
                                if(this.Grid.FlowDirection== System.Windows.FlowDirection.RightToLeft)
                                
                                {
                                   
                                    e.Style.TooltipTemplateKey = "RTLtoolTipValueTemplate";
                                }
                                else

                                e.Style.TooltipTemplateKey = "toolTipValueTemplate";
                                
                                e.Style.ShowTooltip = true;                                
                            }
                            else
                            {
                                e.Style.TooltipTemplateKey = string.Empty;
                                e.Style.ShowTooltip = false;
                            }
                            switch (this.Grid.ValueCellTextAlignment)
                            {
                                case System.Windows.HorizontalAlignment.Left:
                                    {
                                        e.Style.TextMargins.Left = 5d;
                                        e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                                        break;
                                    }
                                case System.Windows.HorizontalAlignment.Right:
                                    {
                                        e.Style.TextMargins.Right = 5d;
                                        e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                                        break;
                                    }
                                default:
                                    {
                                        e.Style.HorizontalAlignment = this.Grid.ValueCellTextAlignment;

                                        break;
                                    }
                            }
#if !SILVERLIGHT

                            if (cellIdentity.CellDescriptor.KpiType == Syncfusion.Olap.Data.KpiTypeEnum.Kpi_Status ||
                                cellIdentity.CellDescriptor.KpiType == Syncfusion.Olap.Data.KpiTypeEnum.Kpi_Trend)
#else
                            if (cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Status ||
                                cellIdentity.CellDescriptor.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Trend)
#endif
                            {
                                e.Style.CellType = "GridKpiCell";
                                e.Style.CellValue = cellIdentity.CellDescriptor;
                            }
                            if (this.Grid.SummaryRowStyle != null)
                            {
                                if (cellIdentity.CellDescriptor.CellExTypes.Contains("SummaryRow"))
                                {
                                    if (this.Grid.SummaryRowStyle.Style != null)
                                    {
                                        e.Style.CellType = "GridTemplateCell";
                                        cellIdentity.Style = this.Grid.SummaryRowStyle.Style;
                                    }
                                    this.UpdateCellStyle(e.Style, this.Grid.SummaryRowStyle, cellIdentity);
                                }
                            }
                            if (this.Grid.SummaryColumnStyle != null)
                            {
                                if (cellIdentity.CellDescriptor.CellExTypes.Contains("SummaryColumn"))
                                {
                                    if (this.Grid.SummaryColumnStyle.Style != null)
                                    {
                                        e.Style.CellType = "GridTemplateCell";
                                        cellIdentity.Style = this.Grid.SummaryColumnStyle.Style;
                                    }
                                    this.UpdateCellStyle(e.Style, this.Grid.SummaryColumnStyle, cellIdentity);
                                }
                            }
                            break;
                        }
                }

                if (cellIdentity.CellDescriptor.CellType == PivotCellDescriptorType.Value)
                {
                    if (cellIdentity.CellDescriptor.Value != null && cellIdentity.CellDescriptor.Value != string.Empty && this.Grid.OlapGrid != null)
                    {
                        foreach (var condition in this.Grid.OlapGrid.ConditionalFormats)
                        {
                            var cellData = this.Engine.GetCellData(cellIdentity.CellDescriptor);
                            string associatedMeasure = string.Empty;
                            if (cellData != null)
                            {
                                if (cellData.MeasureInfo != null)
                                    associatedMeasure = cellData.MeasureInfo.UniqueName.Replace("[", "").Replace("]", "").Replace("Measures.", "");
                                else
                                    associatedMeasure = cellData.Measure;
                            }
                            bool applyFormat = condition.ApplyFormat(cellIdentity.CellDescriptor, associatedMeasure);
                            if (applyFormat)
                            {
                                this.UpdateCellStyle(e.Style, condition.CellStyle, cellIdentity);
                            }
                        }
                    }
                }
            }         
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        internal void Refresh()
        {
            this.InvalidateDisplay();
            this.ResetVisibleRowColCount();

            #region Paging Logic
#if !SILVERLIGHT

            bool isInternalOlap = true;
            if (StartIndex > -1 && EndIndex > -1)
            {
                if (this._PagingEngine == null)
                {
                    this._PagingEngine = this._Engine;
                }

                PivotEngine pe = new PivotEngine();
                pe = this.GetRowsFromPagingEngine(StartIndex, EndIndex, this._PagingEngine);
                this._Engine = pe;
                isInternalOlap = false;
            }
#endif
            #endregion

            this.RowCount = this.Engine.RowsCount;
            this.ColumnCount = this.Engine.TableColumns.Count;            
            this.HeaderRows = this.Engine.HeaderSection.Height;
            this.RowHeights.DefaultLineSize = 25;
            this.HeaderColumns = this.Engine.RowHeaderSection.Width;
#if !SILVERLIGHT
            this[0, 0].Background = Brushes.Transparent;
#else
            this[0, 0].Background = this.Grid.Background;
#endif
            
            if (this.Grid.FreezeHeaders)
            {
                this.FrozenRows = this.Engine.HeaderSection.Height;
#if SILVERLIGHT
                if (this.Engine.RowHeaderSection == Syncfusion.OlapSilverlight.Engine.GridRangeInfo.Empty)
#else
                if (this.Engine.RowHeaderSection == Syncfusion.Olap.Engine.GridRangeInfo.Empty)
#endif
                {
                    this.FrozenColumns = 0;
                }
                else
                    this.FrozenColumns = this.Engine.RowHeaderSection.Width;

#if SILVERLIGHT
                if (this.Engine.HeaderSection == Syncfusion.OlapSilverlight.Engine.GridRangeInfo.Empty)
#else
                if (this.Engine.HeaderSection == Syncfusion.Olap.Engine.GridRangeInfo.Empty)
#endif

                {
                    this.FrozenRows = 0;
                }
                else
                    this.FrozenRows = this.Engine.HeaderSection.Height;
            }
            else
            {
                this.FrozenRows = 0;
                this.FrozenColumns = 0;
            }

            for (int column = 0; column < ColumnCount; column++)
            {
                for (int row = 0; row < RowCount; row++)
                {
                    var pgvCell = this.Engine.TableColumns[column].Cells[row];
                    if (pgvCell.SpanCell == null)
                    {
                        var style = this[row, column];
                        var styleIdentity = new OlapGridCellStyleInfoIdentity(style.CellIdentity.Data, row, column)
                        {
                            CellDescriptor = pgvCell,
                        };

                        
                        style.CellValue = pgvCell.CellValue;
                        style.VerticalAlignment = System.Windows.VerticalAlignment.Center;

#if !SILVERLIGHT
                        if (this.Engine.ItemSource != null || this._PagingEngine != null)
#else
                        if (this.Engine.ItemSource != null)
#endif
                        {
                            string[] range = pgvCell.Range.Info.Split('-');
                            if (range.Length != 2)
                            {
#if !SILVERLIGHT
                                pgvCell.Range = Syncfusion.Olap.Engine.GridRangeInfo.Empty;
#else
                                pgvCell.Range = Syncfusion.OlapSilverlight.Engine.GridRangeInfo.Empty;
#endif
                            }

                            if (!pgvCell.Range.IsEmpty)
                            {
#if !SILVERLIGHT
                                if (isInternalOlap)
                                {
                                    this.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(
                                       row,
                                       column,
                                       pgvCell.Range.Bottom,
                                       pgvCell.Range.Right));
                                }
#else
                                this.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(
                                      row,
                                      column,
                                      pgvCell.Range.Bottom,
                                      pgvCell.Range.Right));
#endif
                            }
                        }
                        else
                        {
                            this.CoveredCells.Add(new Syncfusion.Windows.Controls.Cells.CoveredCellInfo(
                               row,
                               column,
                               row + pgvCell.Range.Bottom,
                               column + pgvCell.Range.Right));

                        }

                        switch (pgvCell.CellType)
                        {
                            case PivotCellDescriptorType.Any:
                                break;
                            case PivotCellDescriptorType.ColumnHeader:
                                {
                                    style.CellType = "GridExpandHyperlinkCell";
                                    style.CellValue = pgvCell;
                                    if (this.Grid.ColumnHeaderStyle != null && this.Grid.ColumnHeaderStyle.Style != null)
                                        styleIdentity.Style = this.Grid.ColumnHeaderStyle.Style;
                                    break;
                                }
                            case PivotCellDescriptorType.RowHeader:
                                {
                                    if (this.Grid.Layout == GridLayout.ExcelLikeLayout || this.Grid.Layout == GridLayout.ExcelLikeLayoutWithMemberProperties)
                                    {
                                        if (pgvCell.IsLastLevel)
                                        {
                                            if (this.Grid.DataManager.ItemSource == null)
                                            {
                                                style.TextMargins.Left = (pgvCell.Level - 1) * 14;
                                            }
                                            else
                                                style.TextMargins.Left = (pgvCell.Level - 1) * 10;
                                        }
                                        else
                                            style.TextMargins.Left = (pgvCell.Level - 1) * 8;
                                    }
                                    style.CellType = "GridExpandHyperlinkCell";
                                    style.CellValue = pgvCell;
                                    if (this.Grid.RowHeaderStyle != null && this.Grid.RowHeaderStyle.Style != null)
                                        styleIdentity.Style = this.Grid.RowHeaderStyle.Style;
                                    break;
                                }
                            case PivotCellDescriptorType.Value:
                                {
#if !SILVERLIGHT
                                    if (pgvCell.KpiType == Syncfusion.Olap.Data.KpiTypeEnum.Kpi_Status ||
                                        pgvCell.KpiType == Syncfusion.Olap.Data.KpiTypeEnum.Kpi_Trend)
#else
                                    if (pgvCell.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Status ||
                                        pgvCell.KpiType == Syncfusion.OlapSilverlight.Data.KpiTypeEnum.Kpi_Trend)
#endif
                                    {
                                        style.CellType = "GridKpiCell";
                                        style.CellValue = pgvCell;
                                    }
                                    style.TextMargins.Left = 5d;
                                    if (this.Grid.ValueCellStyle != null && this.Grid.ValueCellStyle.Style != null)
                                    {
                                        style.CellType = "GridTemplateCell";
                                        styleIdentity.Style = this.Grid.ValueCellStyle.Style;
                                    }
                                    break;
                                }
                            case PivotCellDescriptorType.SummaryRow:
                                style.TextMargins.Left = 5d;
                                if (this.Grid.DataManager != null && this.Grid.DataManager.ItemSource != null)
                                {
                                    style.TextMargins.Left = 5d;
                                    if (this.Grid.Layout == GridLayout.ExcelLikeLayout && this.Grid.DataManager.ItemSource != null)
                                    {
                                        style.TextMargins.Left = 5d;
                                        if (pgvCell.IsLastLevel)
                                        {
                                            style.TextMargins.Left = (pgvCell.Level) * 14;
                                        }
                                        else
                                            style.TextMargins.Left = (pgvCell.Level) * 10;
                                    }
                                }

                                break;           
                            case PivotCellDescriptorType.SummaryColumn:
                                {
                                    style.TextMargins.Left = 5d;
                                    break;
                                }
                            default:
                                break;
                        }
                        if (this.Grid.GridLineStroke != null)
                        {
//#if SILVERLIGHT
//                            style.Borders.All = new Windows.Controls.Cells.CellBorder(new Pen(this.Grid.GridLineStroke, this.Grid.GridLineThickness), Windows.Controls.Cells.BorderStyle.Standard);
//#else
                            style.Borders.All = new Pen(this.Grid.GridLineStroke, this.Grid.GridLineThickness);
//#endif
                        }
                        style.CellIdentity = styleIdentity;
                        style.Tag = styleIdentity;
                    }
                }
            }

            if (this.Grid.CurrentCell != null)
            {
                this.Grid.CurrentCell.Deactivate();
            }
            this.Grid.InvalidateCells();

            if (this.Grid.ResizeColumnsToFit)
            {
                this.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
            }

            if (this.Grid.ResizeRowsToFit)
            {
                this.ResizeRowsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Table(), GridResizeToFitOptions.None);
            }
#if SILVERLIGHT
            if (this.Grid.Layout == GridLayout.ExcelLikeLayout || this.Grid.Layout == GridLayout.ExcelLikeLayoutWithMemberProperties)
            {
                int level = 1;
                int min = 1;
                var m_lengthiestWord = string.Empty;
                double wordLength = this.Grid.Model.ColumnWidths.DefaultLineSize;
                for (int row = 0; row < RowCount; row++)
                {
                    var pgvCell = this.Engine.TableColumns[0].Cells[row];
                    m_lengthiestWord = m_lengthiestWord.Length > pgvCell.CellValue.Length ? m_lengthiestWord : pgvCell.CellValue;
                    if (pgvCell.Level > min && pgvCell.Level > level)
                    {
                        level = int.Parse(pgvCell.Level.ToString());
                    }
                }
                wordLength = this.Grid.OlapGrid.GetRowHeaderTextWidth(m_lengthiestWord);
                if (min < level)
                    this.Grid.Model.ColumnWidths[0] = wordLength + ((level - 1) * 30);
                else
                    this.Grid.Model.ColumnWidths[0] = wordLength + 20;

            }
#endif
        }

        /// <summary>
        /// Updates the cell style.
        /// </summary>
        /// <param name="gridStyleInfo">The grid style info.</param>
        /// <param name="gridCellStyle">The grid cell style.</param>
        /// <param name="cellIdentity">The cell identity.</param>
        internal void UpdateCellStyle(GridStyleInfo gridStyleInfo, OlapGridCellStyle gridCellStyle, OlapGridCellStyleInfoIdentity cellIdentity)
        {
            if (gridCellStyle != null)
            {
                gridStyleInfo.Background = gridCellStyle.Background;
                gridStyleInfo.Font.FontFamily = gridCellStyle.FontFamily;
                gridStyleInfo.Font.FontSize = gridCellStyle.FontSize;
                gridStyleInfo.Font.FontWeight = gridCellStyle.FontWeight;
                gridStyleInfo.Foreground = gridCellStyle.Foreground;

           #if SILVERLIGHT
                if ((this.Engine != null && (this.Engine.TableColumns.Count == 1 && this.Engine.RowsCount == 1)) || 
                    this.Engine.TableColumns[0].Cells[0].CellType == PivotCellDescriptorType.Any)
                    this[0, 0].Background = this.Grid.Background;
                gridStyleInfo.Borders.All = new Pen(this.Grid.GridLineStroke, this.Grid.GridLineThickness);
           #endif

                if (gridCellStyle.IsHyperlinkCell)
                {
                    if (cellIdentity.CellDescriptor.CellType == PivotCellDescriptorType.Value || cellIdentity.CellDescriptor.CellType== PivotCellDescriptorType.SummaryRow
                        || cellIdentity.CellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        switch (cellIdentity.CellDescriptor.CellType)
                        {
                            case PivotCellDescriptorType.Value:
                                if (this.Grid.OlapGrid != null && this.Grid.OlapGrid.ValueCellStyle.IsHyperlinkCell && cellIdentity.CellDescriptor.CellExTypes.Count == 0)
                                {
                                    gridStyleInfo.CellType = "GridHyperlinkCell";
                                    gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                                    cellIdentity.IsHyperlinkCell = true;
                                }
                                else if(cellIdentity.CellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()))
                                {
                                    if (this.Grid.OlapGrid.SummaryRowStyle.IsHyperlinkCell)
                                    {
                                        gridStyleInfo.CellType = "GridHyperlinkCell";
                                        gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                                        cellIdentity.IsHyperlinkCell = true;
                                    }
                                }
                                else if(cellIdentity.CellDescriptor.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()))
                                {
                                    if (this.Grid.OlapGrid.SummaryColumnStyle.IsHyperlinkCell)
                                    {
                                        gridStyleInfo.CellType = "GridHyperlinkCell";
                                        gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                                        cellIdentity.IsHyperlinkCell = true;
                                    }
                                }                       
                                break;
                            case PivotCellDescriptorType.SummaryColumn:
                                if (this.Grid.OlapGrid.SummaryColumnStyle.IsHyperlinkCell)
                                {
                                    gridStyleInfo.CellType = "GridHyperlinkCell";
                                    gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                                    cellIdentity.IsHyperlinkCell = true;
                                }
                                break;
                            case PivotCellDescriptorType.SummaryRow:
                                if (this.Grid.OlapGrid.SummaryRowStyle.IsHyperlinkCell)
                                {
                                    gridStyleInfo.CellType = "GridHyperlinkCell";
                                    gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                                    cellIdentity.IsHyperlinkCell = true;
                                }
                                break;
                        }                      
                    }
                    else
                    {
                        gridStyleInfo.CellType = "GridExpandHyperlinkCell";
                        gridStyleInfo.CellValue = cellIdentity.CellDescriptor;
                        cellIdentity.IsHyperlinkCell = true;
                    }
                }
                else
                {
                    cellIdentity.IsHyperlinkCell = false;
                }
            }
        }
        
        #region Paging Helper Methods
#if !SILVERLIGHT

        /// <summary>
        /// Gets the rows from paging engine.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        /// <param name="pagingPivotEngine">The paging pivot engine.</param>
        /// <returns>The paged pivot engine.</returns>
        private PivotEngine GetRowsFromPagingEngine(int startRow, int endRow, PivotEngine pagingPivotEngine)
        {
            PivotEngine engine = new PivotEngine();
            List<List<PivotCellDescriptor>> cols = null;

            ////if (!this.ShowHeadersOnEachPage)
            //{
            //    cols = new List<List<PivotCellDescriptor>>();

            //    //// Selects the header rows.
            //    cols.AddRange(this.SelectRows(startRow, HorizontalHeaderLength, pagingPivotEngine));

            //    if (startRow == 0)
            //    {
            //        //// Selects the rows other then header rows.
            //        //cols.AddRange(this.SelectRows(HorizontalHeaderLength, endRow, pagingPivotEngine));
            //        //ColumnCount merger. cols[0].AddRange(cols[0].AddRange)
            //        List<List<PivotCellDescriptor>> tempCols = new List<List<PivotCellDescriptor>>();
            //        tempCols = this.SelectRows(HorizontalHeaderLength, endRow, pagingPivotEngine);

            //        //// Mergin rows.
            //        int i = 0;
            //        foreach (List<PivotCellDescriptor> lpcd in tempCols)
            //        {
            //            cols[i].AddRange(lpcd);
            //            i++;
            //        }
            //    }
            //    else
            //    {
            //        //// Selects the rows other then header rows.
            //        //cols.AddRange(this.SelectRows(HorizontalHeaderLength + startRow, endRow, pagingPivotEngine));                    
            //        List<List<PivotCellDescriptor>> tempCols = new List<List<PivotCellDescriptor>>();
            //        tempCols = this.SelectRows(HorizontalHeaderLength + startRow, endRow, pagingPivotEngine);

            //        //// Mergin rows.
            //        int i = 0;
            //        foreach (List<PivotCellDescriptor> lpcd in tempCols)
            //        {
            //            cols[i].AddRange(lpcd);
            //            i++;
            //        }
            //    }
            //}
            //else
            //{
            cols = new List<List<PivotCellDescriptor>>();

            //// Selects the given number of rows without having consideration about column headers.
            cols = this.SelectRows(startRow, endRow, pagingPivotEngine);
            //}

            PivotColumnDescriptor pcdTemp = null;

            //// Creating engine with the specified number of rows.
            for (int colIndex = 0; colIndex < cols.Count; colIndex++)
            {
                pcdTemp = new PivotColumnDescriptor();

                foreach (PivotCellDescriptor pcd in cols[colIndex])
                {
                    pcdTemp.Cells.Add(pcd);
                }

                engine.TableColumns.Add(pcdTemp);
            }

            //this.UpdateNewSpanSettings(ref engine);

            return engine;
        }

        /// <summary>
        /// Selects the rows.
        /// </summary>
        /// <param name="startRow">The start row. Zero base indexed.</param>
        /// <param name="endRow">The end row. Zero base indexed.</param>
        /// <param name="pagingPivotEngine">The paging pivot engine. The backup paging pivot engine.</param>
        /// <returns>Engine with selected rows.</returns>
        private List<List<PivotCellDescriptor>> SelectRows(int startRow, int endRow, PivotEngine pagingPivotEngine)
        {
            List<PivotCellDescriptor> cellDescriptorCol = null;
            List<List<PivotCellDescriptor>> cols = new List<List<PivotCellDescriptor>>();

            //// Takes cols upto specified number of rows.
            for (int col = 0; col < pagingPivotEngine.TableColumns.Count; col++)
            {
                cellDescriptorCol = new List<PivotCellDescriptor>();
                for (int row = startRow; row <= endRow; row++)
                {
                    cellDescriptorCol.Add(pagingPivotEngine[row, col]);
                }

                cols.Add(cellDescriptorCol);
            }

            return cols;
        }


        /// <summary>
        /// Updates the new span settings.
        /// </summary>
        /// <param name="engine">The engine.</param>
        private void UpdateNewSpanSettings(ref PivotEngine engine)
        {
            PivotCellDescriptor oldSpanCell = new PivotCellDescriptor();
            PivotCellDescriptor currentSpanCell = new PivotCellDescriptor();
            //// Cols
            for (int col = 0; col < this.VerticalHeaderLength; col++)
            {
                //// Rows
                for (int row = 0; row < engine.RowsCount; row++)
                {
                    var currentCell = engine[row, col];
                    if (row == 0)
                    {
                        if (currentCell.SpanCell == null)
                        {
                            //// Indicates the cell is not a span cell. It is not s
                            break;
                        }
                        else
                        {
                            oldSpanCell = currentCell.SpanCell;
                            currentCell.SpanCell = null;
                            currentSpanCell = currentCell.SpanCell;
                        }
                    }
                    else
                    {
                        if (currentCell.SpanCell == oldSpanCell)
                        {
                            currentCell.SpanCell = currentSpanCell;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

#endif
        #endregion

        #endregion
    }
}
