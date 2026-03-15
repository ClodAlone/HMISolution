#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Chart;
using System.Drawing;
using Syncfusion.Windows.Forms;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Grouping;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Chart helper class holds methods that draws chart for the concerned grid 
    /// </summary>
    public class GroupingChartHelper
    {
        #region Variable Declaration
        private GridGroupingControl GroupingGrid;
        private ChartControl chartControl1;
        private bool enableCustomChart = false;
        private ArrayList yAxisName;
        private string xColName = string.Empty;        
        int i = 1;
        GroupsInDetailsCollection gdc = null;
        private ViewType viewType = ViewType.ActualView;
        private bool realTimeUpdate = true;
        private bool enableAlertMessageBox = true;
        #endregion 

        #region Property
        /// <summary>
        /// Used to enable/disable the message alert box
        /// </summary>
        public bool EnableAlertMessageBox
        {
            get { return enableAlertMessageBox; }
            set { enableAlertMessageBox = value; }
        }

        /// <summary>
        /// Used to set the view type of the input values
        /// </summary>
        public bool RealTimeUpdate
        {
            get { return realTimeUpdate; }
            set { realTimeUpdate = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ViewType ViewType
        {
            get
            {
                return viewType;
            }
            set
            {
                viewType = value;
            }
        }
        #endregion

        #region Constractor
        /// <summary>
        /// Initialize
        /// </summary>
        public GroupingChartHelper()
        {
            yAxisName = new ArrayList();
        }
        #endregion

        #region Method
        /// <summary>
        /// Wires the chartcell model to cellmodels of grid.
        /// </summary>
        /// <param name="Grouping">GridGroupingControl</param>
        /// <param name="chart">ChartControl</param>
        public void Wire(GridGroupingControl Grouping, ChartControl chart)
        {
            enableCustomChart = false;
            this.GroupingGrid = Grouping;
            this.chartControl1 = chart;
            ChartAppearanceSettings();
            this.GroupingGrid.TableDescriptor.GroupedColumns.Changed += new Collections.ListPropertyChangedEventHandler(GroupedColumns_Changed);            
            this.GroupingGrid.SourceListListChanged += new TableListChangedEventHandler(GroupingGrid_SourceListListChanged);
            this.GroupingGrid.TableModel.SelectedRecordsChanged += new SelectedRecordsChangedEventHandler(TableModel_SelectedRecordsChanged);
            this.GroupingGrid.TableModel.SelectionChanged += new GridSelectionChangedEventHandler(TableModel_SelectionChanged);
            BindGroupingChart();
        }

        /// <summary>
        /// Inserts the chart based on input values
        /// </summary>
        /// <param name="range">SelectedRanges</param>
        private void BindSelectedRange(GridRangeInfo range)
        {
            this.chartControl1.Series.Clear();
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
            for (int j = range.Left; j <= range.Right; j++)
            {
                for (int i = range.Top; i <= range.Bottom; i++)
                {
                    if (GroupingGrid.GetTableModel(desc.Name).SelectedRanges.ActiveRange.IntersectsWith(GridRangeInfo.Cell(i, j)))
                    {
                        GridTableCellStyleInfo style = this.GroupingGrid.GetTableControl(desc.Name).GetTableViewStyleInfo(i, j);
                        if (style.TableCellIdentity.Column != null && style.CellType != GridCellTypeName.Header)
                        {
                            string colName = style.TableCellIdentity.Column.Name;
                            CreateSeries(style.Text);
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Used to bind a chart
        /// </summary>
        private void BindGroupingChart()
        {
            this.chartControl1.Series.Clear();
            Group grp = this.GroupingGrid.Table.TopLevelGroup;
            DetailsSection ds = grp.Details;
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
            for (int i = 0; i < GroupingGrid.GetTableDescriptor(desc.Name).Columns.Count; i++)
            {
                if (this.GroupingGrid.GetTable(desc.Name).SelectedRecords.Count > 0 && ViewType == ViewType.SelectionView)
                {
                    for (int j = 0; j < this.GroupingGrid.GetTable(desc.Name).SelectedRecords.Count; j++)
                    {
                        string colName = GroupingGrid.GetTableDescriptor(desc.Name).Columns[i].ToString();
                        CreateSeries(GroupingGrid.GetTable(desc.Name).SelectedRecords[j].Record.GetValue(colName).ToString());
                    }
                }
                else if (ds is GridGroupsDetails)
                {
                    foreach (Group g in grp.Groups)
                    {
                        for (int col = 0; col < GroupingGrid.GetTableDescriptor(desc.Name).Columns.Count; col++)
                        {
                            for (int rec = 0; rec < g.Records.Count; rec++)
                            {
                                string colName = GroupingGrid.GetTableDescriptor(desc.Name).Columns[col].ToString();
                                CreateSeries(g.Records[rec].GetValue(colName).ToString());
                            }
                        }
                    }
                }
                else
                {
                    for (int rec = 0; rec < this.GroupingGrid.GetTable(desc.Name).Records.Count; rec++)
                    {
                        string colName = GroupingGrid.GetTableDescriptor(desc.Name).Columns[i].ToString();
                        CreateSeries(GroupingGrid.GetTable(desc.Name).Records[rec].GetValue(colName).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// used to create the chart series 
        /// </summary>
        /// <param name="value">Value</param>
        private void CreateSeries(string value)
        {
            double NewValue;
            if (double.TryParse(value.ToString(), out NewValue))
            {
                ChartSeries series = new ChartSeries(value);
                series.Points.Add(NewValue, NewValue);
                this.chartControl1.Series.Add(series);
            }
        }
        #endregion

        #region Event Handler
        void TableModel_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.Reason == Syncfusion.Windows.Forms.Grid.GridSelectionReason.MouseUp && ViewType== ViewType.SelectionView)
            {
                if (!enableCustomChart)
                    BindSelectedRange(e.Range);
                else
                    BindSelectedCustomRange(e.Range);
            }
            if (e.Range != null && e.Range.Info.Equals(string.Empty))
            {
                if (!enableCustomChart)
                    BindGroupingChart();
                else
                    CreateGroupSeries();
            }
        }

        void TableModel_SelectedRecordsChanged(object sender, SelectedRecordsChangedEventArgs e)
        {
            if (e.SelectedRecord != null)
            {
                if (!enableCustomChart)
                    BindGroupingChart();
                else
                    CreateGroupSeries();
            }
        }

        void GroupingGrid_SourceListListChanged(object sender, TableListChangedEventArgs e)
        {
            if (realTimeUpdate)
            {
                if (!enableCustomChart)
                    BindGroupingChart();
                else
                    CreateGroupSeries();
            }
        }

        void GroupedColumns_Changed(object sender, Collections.ListPropertyChangedEventArgs e)
        {
            if (!enableCustomChart)
                BindGroupingChart();
            else
                CreateGroupSeries();           
        }
        #endregion

        #region CustomChart Method
        /// <summary>
        /// Wires the chartcell model to cellmodels of grid.
        /// </summary>
        /// <param name="Grouping">GridGroupingControl</param>
        /// <param name="chart">ChartControl</param>
        /// <param name="xAxisColName">X-AxisLabel</param>
        /// <param name="yAxisColName">Y-AxisLabel</param>
        public void Wire(GridGroupingControl Grouping, ChartControl chart, string xAxisColName, ArrayList yAxisColName)
        {
            enableCustomChart = true;
            yAxisName = yAxisColName;
            this.GroupingGrid = Grouping;
            this.chartControl1 = chart;
            ChartAppearanceSettings();
            this.GroupingGrid.TableDescriptor.GroupedColumns.Changed += new Collections.ListPropertyChangedEventHandler(GroupedColumns_Changed);
            this.GroupingGrid.SourceListListChanged += new TableListChangedEventHandler(GroupingGrid_SourceListListChanged);
            this.GroupingGrid.TableModel.SelectedRecordsChanged += new SelectedRecordsChangedEventHandler(TableModel_SelectedRecordsChanged);
            this.GroupingGrid.TableModel.SelectionChanged += new GridSelectionChangedEventHandler(TableModel_SelectionChanged);
            this.chartControl1.ChartRegionClick += new ChartRegionMouseEventHandler(chartControl1_ChartRegionClick);
            this.chartControl1.ChartRegionMouseMove += new ChartRegionMouseEventHandler(chartControl1_ChartRegionMouseMove);            
            this.chartControl1.ShowToolTips = true;
            xColName = xAxisColName;
            CreateGroupSeries();                    
        }

        /// <summary>
        /// Used to create a drilldown chart
        /// </summary>
        /// <param name="seriesIndex">SeriesIndex</param>
        protected void InitializeDrillDownChart(int seriesIndex)
        {            
            int startRow = 0;            
            int groupCount = 1;
            string groupLabelName = this.chartControl1.PrimaryXAxis.GroupingLabels[seriesIndex].Text;
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;           
            Group grp = this.GroupingGrid.Table.TopLevelGroup;
            DetailsSection ds = grp.Details;
            GridGroupsDetails ggd = ds as GridGroupsDetails;
            gdc = null;
            GroupsInDetailsCollection gd = IterateGroup(ggd.Groups, groupLabelName);
            if (gd != null)
            {
                foreach (Group g in gd)
                {
                    bool inc = false;
                    for (int col = 0; col < yAxisName.Count; col++)
                    {
                        for (int row = 0; row < g.Records.Count; row++)
                        {
                            inc = true;
                            CreateSeries(g.Records[row].GetValue(xColName).ToString(), g.Records[row].GetValue(yAxisName[col].ToString()).ToString(), groupCount);
                            CreateGroupLable(startRow, groupCount, g.Category.ToString());
                        }
                    }

                    if (inc)
                    {
                        startRow = groupCount;
                        groupCount++;
                    }
                }
            }
            else
            {                
                BindChart();
            }
            ChartAppearanceSettings();
        }

        /// <summary>
        /// used to iterate a group
        /// </summary>
        /// <param name="g">GroupsInDetailsCollection</param>
        /// <param name="category">Category</param>
        /// <returns></returns>
        private GroupsInDetailsCollection IterateGroup(GroupsInDetailsCollection g, string category)
        {
            while (this.GroupingGrid.TableDescriptor.GroupedColumns.Count > i)
            {
                foreach (Group gg in g)
                {
                    gdc = gg.Groups;
                    if (gg.Category.Equals(category))
                        break;
                }
                i++;
                IterateGroup(gdc, category);
            }
            return gdc;
        }

        /// <summary>
        /// Used to create a custom chart witout group label.
        /// </summary>        
        private void BindCustomChart()
        {
            this.chartControl1.Series.Clear();
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
            if (this.GroupingGrid.GetTable(desc.Name).SelectedRecords.Count > 0 && ViewType == ViewType.SelectionView)
            {
                for (int col = 0; col < yAxisName.Count; col++)
                {
                    foreach (SelectedRecord selRecords in this.GroupingGrid.GetTable(desc.Name).SelectedRecords)
                    {
                        CreateSeries(selRecords.Record.GetValue(xColName).ToString(), selRecords.Record.GetValue(yAxisName[col].ToString()).ToString(), col);
                    }
                }
            }
            else
            {
                int incRow = 0;
                for (int col = 0; col < yAxisName.Count; col++)
                {
                    int findItemColIndex = GroupingGrid.GetTableModel(desc.Name).NameToColIndex(xColName);
                    int findXColIndex = GroupingGrid.GetTableModel(desc.Name).NameToColIndex(yAxisName[col].ToString());
                    for (int row = 0; row <= this.GroupingGrid.GetTableModel(desc.Name).RowCount; row++)
                    {
                        GridTableCellStyleInfo itemStyle = this.GroupingGrid.GetTableControl(desc.Name).GetTableViewStyleInfo(row, findItemColIndex);
                        GridTableCellStyleInfo style = this.GroupingGrid.GetTableControl(desc.Name).GetTableViewStyleInfo(row, findXColIndex + GroupingGrid.GetTableDescriptor(desc.Name).GroupedColumns.Count);
                        if ((style.CellType != GridCellTypeName.Header
                    && style.CellType != "ColumnHeaderCell") || (style.TableCellIdentity.TableCellType == GridTableCellType.RecordFieldCell
                    && style.TableCellIdentity.TableCellType == GridTableCellType.AlternateRecordFieldCell))
                        {
                            CreateSeries(itemStyle.Text, style.Text, incRow);
                        }
                    }
                    incRow++;
                }
            }
        }

        /// <summary>
        /// Inserts the chart based on input values
        /// </summary>
        /// <param name="range">SelectedRange</param>
        private void BindSelectedCustomRange(GridRangeInfo range)
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
            Group grp = this.GroupingGrid.Table.TopLevelGroup;
            int startRow = 0;
            int groupCount = 1;
            DetailsSection ds = grp.Details;
            if (ds is GridGroupsDetails
                && this.GroupingGrid.GetTableDescriptor(desc.Name).GroupedColumns.Count.Equals(1))
            {
                GridGroupsDetails ggd = ds as GridGroupsDetails;
                foreach (Group g in ggd.Groups)
                {
                    bool inc = false;
                    for (int col = 0; col < yAxisName.Count; col++)
                    {
                        for (int row = 0; row < g.Records.Count; row++)
                        {
                            if (range.IntersectsWith(GridRangeInfo.Row(g.Records[row].GetRowIndex())))
                            {
                                inc = true;
                                CreateSeries(g.Records[row].GetValue(xColName).ToString(), g.Records[row].GetValue(yAxisName[col].ToString()).ToString(), groupCount);
                                CreateGroupLable(startRow, groupCount, g.Category.ToString());
                            }
                        }
                    }
                    if (inc)
                    {
                        startRow = groupCount;
                        groupCount++;
                    }
                }
            }
            else
            {
                int incRow = 0;
                for (int col = 0; col < yAxisName.Count; col++)
                {
                    int findXColIndex = GroupingGrid.GetTableModel(desc.Name).NameToColIndex(yAxisName[col].ToString());
                    int findItemColIndex = GroupingGrid.GetTableModel(desc.Name).NameToColIndex(xColName);
                    for (int row = range.Top; row <= range.Bottom; row++)
                    {
                        if (GroupingGrid.GetTableModel(desc.Name).SelectedRanges.ActiveRange.IntersectsWith(GridRangeInfo.Cell(row, findXColIndex)))
                        {
                            GridTableCellStyleInfo itemStyle = this.GroupingGrid.GetTableControl(desc.Name).GetTableViewStyleInfo(row, findItemColIndex);
                            GridTableCellStyleInfo style = this.GroupingGrid.GetTableControl(desc.Name).GetTableViewStyleInfo(row, findXColIndex);
                            if (style.CellType != GridCellTypeName.Header
                        && style.CellType != "ColumnHeaderCell")
                            {
                                CreateSeries(itemStyle.Text, style.Text, incRow);
                            }
                        }
                    }

                }
                incRow++;
            }
            ChartAppearanceSettings();
        }

        /// <summary>
        /// Used to create a custom chart with group label.
        /// </summary>
        private void CreateGroupSeries()
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            if (this.GroupingGrid.TableDescriptor.Relations.Count.Equals(0))
            {
                GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
                Group grp = this.GroupingGrid.GetTable(desc.Name).TopLevelGroup;
                DetailsSection ds = grp.Details;
                if (ds is GridGroupsDetails
                    && this.GroupingGrid.GetTableDescriptor(desc.Name).GroupedColumns.Count.Equals(1))
                {
                    int startRow = 0;
                    GridGroupsDetails ggd = ds as GridGroupsDetails;
                    int groupCount = 1;
                    foreach (Group g in ggd.Groups)
                    {
                        bool inc = false;
                        for (int col = 0; col < yAxisName.Count; col++)
                        {
                            if (this.GroupingGrid.GetTable(desc.Name).SelectedRecords.Count > 0 && ViewType == ViewType.SelectionView)
                            {
                                for (int rec = 0; rec < g.Records.Count; rec++)
                                {
                                    if (g.Records[rec].IsSelected())
                                    {
                                        inc = true;
                                        CreateSeries(g.Records[rec].GetValue(xColName).ToString(), g.Records[rec].GetValue(yAxisName[col].ToString()).ToString(), groupCount);
                                        CreateGroupLable(startRow, groupCount, g.Category.ToString());
                                    }
                                }
                            }
                            else
                            {
                                for (int rec = 0; rec < g.Records.Count; rec++)
                                {
                                    inc = true;
                                    CreateSeries(g.Records[rec].GetValue(xColName).ToString(), g.Records[rec].GetValue(yAxisName[col].ToString()).ToString(), groupCount);
                                }
                                if (inc)
                                    CreateGroupLable(startRow, groupCount, g.Category.ToString());
                            }
                        }
                        if (inc)
                        {
                            startRow = groupCount;
                            groupCount++;
                        }
                    }
                }
                else if (this.GroupingGrid.GetTableDescriptor(desc.Name).GroupedColumns.Count > 1)
                {
                    BindChart();
                }
                else
                {
                    BindCustomChart();
                    ChartAppearanceSettings();
                }
            }
            else
                if (EnableAlertMessageBox)
                    MessageBoxAdv.Show("Chart does not support for relational tables");
        }

        /// <summary>
        ///Used to bind a chart based on the input values.
        /// </summary>
        private void BindChart()
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            GridGroupingControl ggc = new GridGroupingControl();
            ggc.DataSource = GroupingGrid.DataSource;
            GridTableDescriptor desc = this.GroupingGrid.TableDescriptor;
            ggc.TableDescriptor.GroupedColumns.Add(this.GroupingGrid.GetTableDescriptor(desc.Name).GroupedColumns[0].Name);
            i = 1;
            Group grp = this.GroupingGrid.GetTable(desc.Name).TopLevelGroup;
            DetailsSection ds = grp.Details;
            Group internalGrp = ggc.Table.TopLevelGroup;
            DetailsSection internalDs = grp.Details;
            int startRow = 0;
            GridGroupsDetails ggd = ds as GridGroupsDetails;
            int groupCount = 1;
            foreach (Group internalg in internalGrp.Groups)
            {
                bool inc = false;
                for (int col = 0; col < yAxisName.Count; col++)
                {
                    for (int rec = 0; rec < internalg.Records.Count; rec++)
                    {
                        inc = true;
                        CreateSeries(internalg.Records[rec].GetValue(xColName).ToString(), internalg.Records[rec].GetValue(yAxisName[col].ToString()).ToString(), groupCount);
                    }
                    if (inc)
                        CreateGroupLable(startRow, groupCount, internalg.Category.ToString());
                }
                if (inc)
                {
                    startRow = groupCount;
                    groupCount++;
                }
            }
        }

        /// <summary>
        /// Used to create a chart series
        /// </summary>
        /// <param name="seriesName">Series Name</param>
        /// <param name="value">Values</param>
        /// <param name="seriesNo">Series Count</param>
        private void CreateSeries(string seriesName, string value, int seriesNo)
        {
            double NewValue;
            if (double.TryParse(value, out NewValue))
            {
                ChartSeries barSeries = new ChartSeries(seriesName, ChartSeriesType.Column);
                barSeries.Points.Add(seriesNo, NewValue);
                barSeries.Style.Text = seriesName;
                barSeries.Style.TextOrientation = ChartTextOrientation.Smart;
                barSeries.SmartLabels = true;
                this.chartControl1.Series.Add(barSeries);
            }
        }

        /// <summary>
        /// Used to create custom chart groping Label
        /// </summary>
        /// <param name="startRow">StartRowIndex</param>
        /// <param name="endRow">EndRowIndex</param>        
        /// <param name="g">GroupName</param>
        private void CreateGroupLable(int startRow, int endRow, string g)
        {
            if (enableCustomChart)
            {
                ChartAxisGroupingLabel Q2 = new ChartAxisGroupingLabel(new DoubleRange(startRow, endRow), g);
                Q2.BorderStyle = ChartAxisGroupingLabelBorderStyle.Rectangle;
                Q2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                Q2.Range = new DoubleRange(startRow - 0.5, endRow - 0.5);
                this.chartControl1.PrimaryXAxis.GroupingLabels.Add(Q2);
                this.chartControl1.PrimaryXAxis.DrawTickLabelGrid = false;
            }
        }

        /// <summary>
        /// To apply styles of the chart
        /// </summary>
        private void ChartAppearanceSettings()
        {
            this.chartControl1.AddRandomSeries = false;
            this.chartControl1.Legend.ShowSymbol = true;
            this.chartControl1.Indexed = true;
            this.chartControl1.Legend.Visible = true;
            this.chartControl1.SpacingBetweenSeries = 30;
            this.chartControl1.ColumnDrawMode = ChartColumnDrawMode.PlaneMode;
            this.chartControl1.LegendPosition = ChartDock.Right;
            this.chartControl1.LegendAlignment = ChartAlignment.Center;
            this.chartControl1.LegendsPlacement = ChartPlacement.Outside;
            this.chartControl1.Legend.Spacing = 3;
            this.chartControl1.ChartArea.BackInterior = new BrushInfo(GradientStyle.Vertical, Color.Transparent, Color.Transparent);
            this.chartControl1.ChartInterior = new BrushInfo(GradientStyle.Vertical, Color.Transparent, Color.Transparent);
            this.chartControl1.BorderAppearance.SkinStyle = Syncfusion.Windows.Forms.Chart.ChartBorderSkinStyle.Frame;
            this.chartControl1.BorderAppearance.BaseColor = Color.SkyBlue;
            this.chartControl1.BorderAppearance.FrameThickness = new ChartThickness(-2, -2, 2, 2);
            this.chartControl1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.chartControl1.ChartArea.PrimaryXAxis.HidePartialLabels = true;
            this.chartControl1.ElementsSpacing = 5;
            this.chartControl1.PrimaryXAxis.LabelIntersectAction = ChartLabelIntersectAction.Rotate;
            this.chartControl1.PrimaryXAxis.TickLabelsDrawingMode = ChartAxisTickLabelDrawingMode.UserMode;
        }
        #endregion

        #region ChartControl EventHandler
        private double pointindex = -1;
        private int seriesindex = -1;
        void chartControl1_ChartRegionMouseMove(object sender, ChartRegionMouseEventArgs e)
        {
            if (this.GroupingGrid!=null && this.GroupingGrid.TableDescriptor.GroupedColumns.Count > 1)
            {
                if (e.Region.SeriesIndex != seriesindex)
                {
                    if (pointindex > -1 && seriesindex > -1)
                    {
                        foreach (ChartSeries series in this.chartControl1.Series)
                        {
                            if ((series.Summary.FindValue(pointindex, "X") != null))
                            {
                                series.Style.ResetInterior();
                                this.chartControl1.Cursor = System.Windows.Forms.Cursors.Default;
                            }
                        }
                    }
                    pointindex = -1;
                    seriesindex = -1;
                }
                if (e.Region.SeriesIndex > -1
                    && this.chartControl1.Series.Count > e.Region.SeriesIndex)
                {
                    double xvl = this.chartControl1.Series[e.Region.SeriesIndex].Points[e.Region.PointIndex].X;
                    foreach (ChartSeries series in this.chartControl1.Series)
                    {
                        if ((series.Summary.FindValue(xvl, "X") != null))
                        {
                            series.Style.Interior = new BrushInfo(Color.Red);
                            this.chartControl1.Cursor = System.Windows.Forms.Cursors.Hand;
                        }
                    }
                    pointindex = xvl;
                    seriesindex = e.Region.SeriesIndex;
                }
            }
        }
       
        void chartControl1_ChartRegionClick(object sender, ChartRegionMouseEventArgs e)
        {
            if (e.Region.SeriesIndex > -1 && this.GroupingGrid!=null && this.GroupingGrid.TableDescriptor.GroupedColumns.Count > 1)
            {
                int xvl;
                if (int.TryParse(this.chartControl1.Series[e.Region.SeriesIndex].Points[e.Region.PointIndex].X.ToString(), out xvl))
                {
                    if (xvl > 0)
                        InitializeDrillDownChart(xvl - 1);                   
                }
            }
        }
        #endregion       

        #region Unwire
        /// <summary>
        /// Unhook the grouping grid from the chart.
        /// </summary>       
        public void Unwire()
        {
            if (GroupingGrid != null)
            {
                this.GroupingGrid.TableDescriptor.GroupedColumns.Changed -= new Collections.ListPropertyChangedEventHandler(GroupedColumns_Changed);
                this.GroupingGrid.SourceListListChanged -= new TableListChangedEventHandler(GroupingGrid_SourceListListChanged);
                this.GroupingGrid.TableModel.SelectedRecordsChanged -= new SelectedRecordsChangedEventHandler(TableModel_SelectedRecordsChanged);
                this.GroupingGrid.TableModel.SelectionChanged -= new GridSelectionChangedEventHandler(TableModel_SelectionChanged);
                this.GroupingGrid = null;
                this.chartControl1.Series.Clear();
                this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
                this.chartControl1.PrimaryXAxis.Labels.Clear();                
                this.chartControl1 = null;               
            }
        }
        #endregion
    }   
}

namespace Syncfusion.Windows.Forms
{
    using System;
    /// <summary>
    /// Set the type to be viewed for the Grid
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// Selects the entire grid in view
        /// </summary>
        ActualView = 0,
        /// <summary>
        /// Considers the selected area in grid
        /// </summary>
        SelectionView = 1
    }
}

