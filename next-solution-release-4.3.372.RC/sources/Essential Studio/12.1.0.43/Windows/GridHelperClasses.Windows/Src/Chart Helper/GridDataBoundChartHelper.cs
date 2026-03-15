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
using Syncfusion.Windows.Forms.Chart;
using System.Collections;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using Syncfusion.Drawing;
using System.Data;
using Syncfusion.Windows.Forms;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// GridDataBoundChartHelper class
    /// </summary>
    public class GridDataBoundChartHelper
    {
        #region Variable Declaration
        private GridDataBoundGrid Grid;
        private ChartControl chartControl1;
        private DataTable dtable = new DataTable();
        private bool enableCustomChart = false;
        private ArrayList yAxisName;
        private string xColName = string.Empty;
        private List<string> xAxisItems = new List<string>();       
        private Dictionary<int,string> xAxisIndex=new Dictionary<int,string>();
        private ViewType viewType = ViewType.ActualView;
        #endregion

        # region Property
        /// <summary>
        /// Used to set the view type of the input values
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
        /// Initializes GridChartHelper class
        /// </summary>              
        public GridDataBoundChartHelper()
        {

        }
        #endregion          

        #region SelectionEvents
        /// <summary>
        /// Handles the selection to get the value cells
        /// </summary>
        void Model_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.Reason == Syncfusion.Windows.Forms.Grid.GridSelectionReason.MouseUp && ViewType == ViewType.SelectionView)
            {
                if (!enableCustomChart)
                    BindSelectedRange(e.Range);
                else
                    BindSelectedCustomRange(e.Range);
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// Wires the chartcell model to cellmodels of grid.
        /// </summary>      
        /// <param name="grid">GridDataBoundControl</param>
        /// <param name="chart">ChartControl</param>
        public void Wire(GridDataBoundGrid grid, ChartControl chart)
        {
            Grid = grid;
            this.Grid.AllowSelection = GridSelectionFlags.Any;
            this.chartControl1 = chart;
            this.chartControl1.Series3D = true;
            this.chartControl1.Legend.ShowSymbol = true;
            this.chartControl1.Indexed = true;
            this.chartControl1.Legend.Visible = true;
            this.chartControl1.SpacingBetweenSeries = 30;
            this.chartControl1.ColumnDrawMode = ChartColumnDrawMode.PlaneMode;
            this.chartControl1.LegendPosition = ChartDock.Bottom;
            this.chartControl1.LegendAlignment = ChartAlignment.Center;
            this.chartControl1.LegendsPlacement = ChartPlacement.Outside;
            this.chartControl1.Legend.Spacing = 3;
            this.chartControl1.BackInterior = new BrushInfo(GradientStyle.PathRectangle, new Color[] { Color.FromArgb(214, 231, 247), Color.White });
            this.chartControl1.ChartArea.BackInterior = new BrushInfo(GradientStyle.Vertical, Color.Transparent, Color.Transparent);
            this.chartControl1.ChartInterior = new BrushInfo(GradientStyle.Vertical, Color.Transparent, Color.Transparent);
            this.chartControl1.BorderAppearance.SkinStyle = Syncfusion.Windows.Forms.Chart.ChartBorderSkinStyle.Frame;
            this.chartControl1.BorderAppearance.BaseColor = Color.SkyBlue;
            this.chartControl1.BorderAppearance.FrameThickness = new ChartThickness(-2, -2, 2, 2);
            this.chartControl1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.chartControl1.ChartArea.PrimaryXAxis.HidePartialLabels = true;
            this.chartControl1.ElementsSpacing = 5;
            this.chartControl1.Palette = ChartColorPalette.Pastel;
            this.chartControl1.PrimaryXAxis.LabelIntersectAction = ChartLabelIntersectAction.Rotate;
            this.Grid.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
            CreateChart();
        }

        /// <summary>
        /// Used to create a chart based on the bounded values
        /// </summary>
        private void CreateChart()
        {
            this.chartControl1.Series.Clear();
            for (int row = 0; row < Grid.Model.RowCount; row++)
            {
                for (int col = 0; col < Grid.Model.ColCount; col++)
                {
                    GridStyleInfo style = this.Grid.GetViewStyleInfo(row, col);
                    if (style.CellType != GridCellTypeName.Header
                        && style.CellType != "ColumnHeaderCell")
                        CreateSeries(style.Text);
                }
            }
        }

        /// <summary>
        /// Inserts the chart based on input values
        /// </summary>
        /// <param name="range">A range whose values to be considered as input</param>
        private void BindSelectedRange(GridRangeInfo range)
        {
            this.chartControl1.Series.Clear();
            if (Grid.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.None)
            {
                for (int i = range.Top; i <= range.Bottom; i++)
                {
                    for (int j = range.Left; j <= range.Right; j++)
                    {
                        if (Grid.Model.SelectedRanges.ActiveRange.IntersectsWith(GridRangeInfo.Row(i)))
                        {
                            GridStyleInfo style = this.Grid.GetViewStyleInfo(i, j);
                            if (style.CellType != GridCellTypeName.Header
                                && style.CellType != "ColumnHeaderCell")
                                CreateSeries(style.Text);
                        }
                    }

                }
            }
            else
            {
                for (int i = range.Top; i <= range.Bottom; i++)
                {
                    for (int j = 0; j <= Grid.Model.ColCount; j++)
                    {
                        if (Grid.Model.Selections.Ranges.ActiveRange.IntersectsWith(GridRangeInfo.Row(i)))
                        {
                            GridStyleInfo style = this.Grid.GetViewStyleInfo(i, j);
                            if (style.CellType != GridCellTypeName.Header
                                && style.CellType != "ColumnHeaderCell")
                                CreateSeries(style.Text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used to bind the series in chart
        /// </summary>
        /// <param name="value">Value</param>
        private void CreateSeries(string value)
        {
            double NewValue = 0;
            if (double.TryParse(value.ToString(), out NewValue))
            {
                ChartSeries series = new ChartSeries(value);
                series.Points.Add(NewValue, NewValue);
                this.chartControl1.Series.Add(series);
            }
        }
        #endregion       

        #region Appearance Settings
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
            this.chartControl1.LegendPosition = ChartDock.Bottom;
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
            this.chartControl1.Skins = Windows.Forms.Chart.Skins.Metro;           
            this.chartControl1.Palette = ChartColorPalette.Custom;             
        }
        #endregion

        #region CustomChart Method   
        /// <summary>
        /// Wires the chartcell model to cellmodels of grid.
        /// </summary>
        /// <param name="gridDataBound">GridDataBound</param>
        /// <param name="chart">ChartControl</param>
        /// <param name="xAxisColName">X-Axis Value</param>
        /// <param name="yAxisColName">Y-Axix Value</param>
        public void WireGrid(GridDataBoundGrid gridDataBound, ChartControl chart, string xAxisColName, ArrayList yAxisColName)
        {
            enableCustomChart = true;
            yAxisName = yAxisColName;
            this.Grid = gridDataBound;
            dtable = (DataTable)gridDataBound.DataSource;
            this.chartControl1 = chart;
            this.chartControl1.Palette = ChartColorPalette.Custom;
            ChartAppearanceSettings();
            this.chartControl1.ShowToolTips = true;
            xColName = xAxisColName;
            this.Grid.Model.SelectionChanged += new GridSelectionChangedEventHandler(Model_SelectionChanged);
            CreateCustomChart();
        }

        /// <summary>
        /// Used to create a custom chart
        /// </summary>
        private void CreateCustomChart()
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            if (xAxisIndex != null)
                xAxisIndex.Clear();
            int colIndex;
            bool avail = false;
            if (yAxisName.Count > 0)
            {
                for (int col = 0; col < yAxisName.Count; col++)
                {
                    ChartSeries series = new ChartSeries(yAxisName[col].ToString(), ChartSeriesType.Column);
                    colIndex = this.Grid.NameToColIndex(yAxisName[col].ToString());
                    xAxisIndex.Add(colIndex, yAxisName[col].ToString());
                    if (xColName.Equals(yAxisName[col].ToString()))
                        avail = true;
                    for (int row = 0; row < Grid.Model.RowCount; row++)
                    {
                        GridStyleInfo style = this.Grid.GetViewStyleInfo(row, colIndex);
                        if (style.CellType != GridCellTypeName.Header
                            && style.CellType != "ColumnHeaderCell")
                        {
                            double value;
                            if (double.TryParse(style.Text, out value))
                            {
                                series.Points.Add(series.Points.Count, value);
                            }
                            if (avail)
                                this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), row, "", "", ChartValueType.Custom));
                        }
                    }

                    this.chartControl1.Series.Add(series);

                }
                if (!avail)
                    CreateLabel();
                ChartAppearanceSettings();
            }
        }

        /// <summary>
        /// Used to create a custom labels.
        /// </summary>
        private void CreateLabel()
        {
            this.chartControl1.PrimaryXAxis.TickLabelsDrawingMode = ChartAxisTickLabelDrawingMode.UserMode;
            int colIndex = this.Grid.NameToColIndex(xColName);

            for (int row = 0; row < this.Grid.Model.RowCount; row++)
            {
                GridStyleInfo style = this.Grid.GetViewStyleInfo(row, colIndex);
                if (style.CellType != GridCellTypeName.Header
                    && style.CellType != "ColumnHeaderCell")
                {
                    this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), this.chartControl1.PrimaryXAxis.Labels.Count, "", "", ChartValueType.Custom));
                }
            }
        }

        /// <summary>
        /// Used to create a chart based on the customized columns values
        /// </summary>
        private void BindSelectedCustomRange(GridRangeInfo range)
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.Labels.Clear();
            if (yAxisName.Count > 0)
            {
                if (Grid.ListBoxSelectionMode == System.Windows.Forms.SelectionMode.None)
                {
                    bool avail = false;
                    for (int j = range.Left; j <= range.Right; j++)
                    {
                        ChartSeries series = new ChartSeries();
                        string colName = string.Empty;
                        if (xAxisIndex.TryGetValue(j, out colName))
                            series.Text = colName;
                        if (!string.IsNullOrEmpty(colName) && colName.Equals(xColName))
                            avail = true;
                        if (xAxisIndex.ContainsKey(j))
                        {
                            for (int i = range.Top; i <= range.Bottom; i++)
                            {
                                if (Grid.Model.SelectedRanges.ActiveRange.IntersectsWith(GridRangeInfo.Row(i)))
                                {
                                    GridStyleInfo style = this.Grid.GetViewStyleInfo(i, j);
                                    if (style.CellType != GridCellTypeName.Header
                                        && style.CellType != "ColumnHeaderCell")
                                    {
                                        double value;
                                        if (double.TryParse(style.Text, out value))
                                        {
                                            series.Points.Add(series.Points.Count, value);
                                        }
                                    }
                                    if (avail)
                                        this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), this.chartControl1.PrimaryXAxis.Labels.Count, "", "", ChartValueType.Custom));
                                }
                            }
                            this.chartControl1.Series.Add(series);
                        }

                    }

                    if (!avail)
                    {
                        int colIndex = this.Grid.NameToColIndex(xColName);
                        for (int j = range.Left; j <= range.Right; j++)
                        {
                            for (int i = range.Top; i <= range.Bottom; i++)
                            {
                                GridStyleInfo style = this.Grid.GetViewStyleInfo(i, colIndex);
                                if (style.CellType != GridCellTypeName.Header
                                    && style.CellType != "ColumnHeaderCell")
                                {
                                    this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), this.chartControl1.PrimaryXAxis.Labels.Count, "", "", ChartValueType.Custom));
                                }
                            }
                        }
                    }
                }
                else
                {
                    bool avail = false;
                    int colIndex;
                    for (int col = 0; col < yAxisName.Count; col++)
                    {
                        ChartSeries series = new ChartSeries();
                        colIndex = this.Grid.NameToColIndex(yAxisName[col].ToString());
                        series.Text = yAxisName[col].ToString();
                        for (int i = range.Top; i <= range.Bottom; i++)
                        {
                            if (Grid.Model.Selections.Ranges.ActiveRange.IntersectsWith(GridRangeInfo.Row(i)))
                            {
                                GridStyleInfo style = this.Grid.GetViewStyleInfo(i, colIndex);
                                if (style.CellType != GridCellTypeName.Header
                                    && style.CellType != "ColumnHeaderCell")
                                {
                                    double value;
                                    if (double.TryParse(style.Text, out value))
                                    {
                                        series.Points.Add(series.Points.Count, value);
                                    }
                                    if (avail)
                                        this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), this.chartControl1.PrimaryXAxis.Labels.Count, "", "", ChartValueType.Custom));
                                }
                            }
                        }
                        this.chartControl1.Series.Add(series);
                    }
                    if (!avail)
                    {
                        colIndex = this.Grid.NameToColIndex(xColName);
                        for (int j = range.Left; j <= range.Right; j++)
                        {
                            for (int i = range.Top; i <= range.Bottom; i++)
                            {
                                GridStyleInfo style = this.Grid.GetViewStyleInfo(i, colIndex);
                                if (style.CellType != GridCellTypeName.Header
                                    && style.CellType != "ColumnHeaderCell")
                                {
                                    this.chartControl1.PrimaryXAxis.Labels.Add(new ChartAxisLabel(style.Text, Color.OrangeRed, new Font("Arial", 8F, System.Drawing.FontStyle.Bold), this.chartControl1.PrimaryXAxis.Labels.Count, "", "", ChartValueType.Custom));
                                }
                            }
                        }
                    }
                }
            }
            ChartAppearanceSettings();
        }
        #endregion       

        #region UnWire
        /// <summary>
        /// Unhook the grouping grid from the chart.
        /// </summary>       
        public void Unwire()
        {
            if (Grid != null)
            {
                this.Grid.Model.SelectionChanged -= new GridSelectionChangedEventHandler(Model_SelectionChanged);
                this.Grid = null;
                this.chartControl1.Series.Clear();
                this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
                this.chartControl1.PrimaryXAxis.Labels.Clear();                
                this.chartControl1 = null;

            }
        }
        #endregion
    }
}


