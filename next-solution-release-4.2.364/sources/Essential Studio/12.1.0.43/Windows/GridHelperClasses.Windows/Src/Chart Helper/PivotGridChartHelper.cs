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
using Syncfusion.Windows.Forms.PivotAnalysis;
using System.Collections;
using System.Data;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Drawing;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// An helper class to embed chartcontrol in grid cells
    /// </summary>
    public class PivotGridChartHelper
    {
        private PivotGridControl pivotGrid1;
        private ChartControl chartControl1;
        private ArrayList colCollection, rowCollection, items;
        private ChartAxis secXAxis = new ChartAxis();
        private ChartAxis secYAxis = new ChartAxis();
        private int count = 0;

        #region Constructor
        /// <summary>
        /// Initializes GridChartHelper class
        /// </summary>
        public PivotGridChartHelper()
        {
            colCollection = new ArrayList();
            rowCollection = new ArrayList();
            items = new ArrayList();
        }
        #endregion

        #region Selection Handlers
        private GridRangeInfo selRange;
        /// <summary>
        /// Handles the selection to get the value cells
        /// </summary>
        private void TableModel_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (e.Reason == Syncfusion.Windows.Forms.Grid.GridSelectionReason.MouseUp)
            {
                selRange = GridRangeInfo.Empty;
                for (int i = e.Range.Top; i <= e.Range.Bottom; i++)
                    for (int j = e.Range.Left; j <= e.Range.Right; j++)
                        selRange = selRange.UnionRange(GridRangeInfo.Cell(i, j));
                if (selRange.Left == 0 && selRange.Right == 0 && selRange.Top == 0 && selRange.Bottom == 0)
                {
                    MessageBox.Show("Selection not valid : Select Value Cells");
                    this.pivotGrid1.TableModel.Selections.Clear();
                }
                else if (selRange.Left <= this.pivotGrid1.PivotRows.Count || selRange.Top <= (this.pivotGrid1.PivotCalculations.Count > 1 ? this.pivotGrid1.PivotColumns.Count + 1 : this.pivotGrid1.PivotColumns.Count))
                {
                    MessageBox.Show("Selection not valid : Exclude Headers");
                    this.pivotGrid1.TableModel.Selections.Clear();
                }
                else
                    BindSelectedRange(selRange);
                this.chartControl1.ChartFormatAxisLabel += new ChartFormatAxisLabelEventHandler(chartControl1_ChartFormatAxisLabel);
            }
        }
        #endregion

        #region Chart Setting Handlers
        /// <summary>
        /// Wires the chartcell model to cellmodels of grid.
        /// </summary>
        /// <param name="grid">A grid to which wiring to be made.</param>
        /// <param name="chart">parameter setting chart for grid </param>
        public void WireGrid(PivotGridControl grid, ChartControl chart)
        {
            if (grid != null && grid.ItemSource != null && chart != null)
            {
                this.pivotGrid1 = grid;
                this.chartControl1 = chart;
                if (this.chartControl1.Text == "chartControl1")
                    this.chartControl1.Text = "";
                this.chartControl1.Series3D = true;
                this.chartControl1.Legend.ShowSymbol = true;
                this.chartControl1.Indexed = true;
                this.chartControl1.Legend.Visible = true;
                this.pivotGrid1.TableModel.SelectionChanged += new GridSelectionChangedEventHandler(TableModel_SelectionChanged);
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
            }
        }

        /// <summary>
        /// Applies text to the x-y axes
        /// </summary>
        private void chartControl1_ChartFormatAxisLabel(object sender, ChartFormatAxisLabelEventArgs e)
        {
            int firstRow = selRange.Top;
            int column = this.pivotGrid1.PivotRows.Count;
            int maxColumn = column - (column - 1);
            int max = (selRange.Bottom - selRange.Top) + 1;
            string Value = string.Empty;
            if (e.AxisOrientation == ChartOrientation.Horizontal)
            {
                if (e.Value == -1 || e.Value == selRange.Height)
                {
                    e.Label = string.Empty;
                }
                if (e.Value != -1)
                {
                    while (maxColumn <= column)
                    {
                        if (this.pivotGrid1.TableModel[(int)e.Value + firstRow, maxColumn].Text != string.Empty)
                        {
                            if (maxColumn != column && !this.pivotGrid1.TableModel[(int)e.Value + firstRow, maxColumn].Text.Contains("Total"))
                                Value += this.pivotGrid1.TableModel[(int)e.Value + firstRow, maxColumn].Text + "-";
                            else
                                Value += this.pivotGrid1.TableModel[(int)e.Value + firstRow, maxColumn].Text;
                        }
                        else
                        {
                            EnsureCoveredRanges();
                            GridRangeInfo covRange = this.pivotGrid1.TableModel.CoveredRanges.FindRange((int)e.Value + firstRow, maxColumn);
                            if (maxColumn != column)
                                Value += this.pivotGrid1.TableModel[covRange.Top, maxColumn].Text + "-";
                            else
                                Value += this.pivotGrid1.TableModel[covRange.Top, maxColumn].Text;
                        }
                        maxColumn++;
                    }
                }
                e.Label = Value;
                e.Handled = true;
            }
        }

        /// <summary>
        /// To ensure the covered ranges in PivotGrid
        /// </summary>
        private void EnsureCoveredRanges()
        {
            this.pivotGrid1.TableModel.BeginUpdate();
            this.pivotGrid1.TableModel.CoveredRanges.Clear();
            this.pivotGrid1.TableModel.CoveredRanges.UpdateList();
            this.pivotGrid1.TableModel.CoveredRanges.ResetCache();
            foreach (var range in this.pivotGrid1.PivotEngine.CoveredRanges)
            {
                this.pivotGrid1.TableModel.CoveredRanges.Add(GridRangeInfo.Cells(range.Top + 1, range.Left + 1, range.Bottom + 1, range.Right + 1));
            }
            this.pivotGrid1.TableModel.EndUpdate();
        }
        #endregion

        #region Chart Generation

        private ArrayList grpValues = new ArrayList(); 
        /// <summary>
        /// Inserts the chart based on input values
        /// </summary>
        /// <param name="range">A range whose values to be considered as input.</param>
        public void BindSelectedRange(GridRangeInfo range)
        {
            this.chartControl1.Series.Clear();
            this.chartControl1.PrimaryXAxis.GroupingLabels.Clear();
            int startRow = range.Top;
            int startCol = range.Left;
            int endRow = range.Bottom;
            int endCol = range.Right;
            int headerCount = this.pivotGrid1.PivotCalculations.Count > 1 ? this.pivotGrid1.PivotColumns.Count + 1 : this.pivotGrid1.PivotColumns.Count;
            int colHeaderIndex = this.pivotGrid1.PivotColumns.Count;
            int rowHeaderIndex = this.pivotGrid1.PivotRows.Count;

            if (endCol % this.pivotGrid1.PivotCalculations.Count == 0)
            {
                count = endCol - this.pivotGrid1.PivotCalculations.Count;
            }
            else
                count = endCol - (this.pivotGrid1.PivotRows.Count);

            int GrpIndex = 0;
            ArrayList parentGroupList = new ArrayList();
            if (this.pivotGrid1.PivotCalculations.Count > 1)
                GrpIndex = startRow - 2;
            else
                GrpIndex = startRow - 1;
            for (int colStart = startCol; colStart <= endCol; colStart++)
            {
                parentGroupList.Add(this.pivotGrid1.TableModel[GrpIndex, colStart].CellValue);
                if (this.pivotGrid1.PivotCalculations.Count > 1)
                    colStart++;
            }
            ChartSeries[] series = new ChartSeries[count];
            int maxCol = startRow - 1;
            for (int i = 0; i < range.Width; i++)
            {
                series[i] = new ChartSeries();
                series[i].Style.TextOffset = 20;
                series[i].SmartLabelsBorderColor = Color.Red;
                series[i].SmartLabelsBorderWidth = 1;
                this.chartControl1.Series.Add(series[i]);
            }
            DataTable table = new DataTable();
            for (int i = 0; i < this.pivotGrid1.PivotRows.Count; i++)
            {
                table.Columns.Add(this.pivotGrid1.PivotRows[i].FieldMappingName);
            }
            for (int i = 0; i < this.pivotGrid1.PivotColumns.Count; i++)
            {
                if (!table.Columns.Contains(this.pivotGrid1.PivotColumns[i].FieldMappingName))
                    table.Columns.Add(this.pivotGrid1.PivotColumns[i].FieldMappingName);
            }
            for (int i = 0; i < this.pivotGrid1.PivotCalculations.Count; i++)
            {
                table.Columns.Add(this.pivotGrid1.PivotCalculations[i].FieldName);
            }
            for (int row = startRow, startRowIndex = 0; row <= endRow; row++, startRowIndex++)
            {
                DataRow dr = table.NewRow();
                for (int col = startCol, startIndex = 0; col <= endCol; col++, startIndex++)
                {
                    for (int i = 0; i < this.pivotGrid1.PivotRows.Count; i++)
                    {
                        if (this.pivotGrid1.PivotEngine[row - 1, i].Value!=null && this.pivotGrid1.PivotEngine[row - 1, i].Value.ToString() != "x")
                            dr[this.pivotGrid1.PivotRows[i].FieldMappingName] = this.pivotGrid1.PivotEngine[row - 1, i].Value;
                    }
                    for (int j = 0; j < this.pivotGrid1.PivotColumns.Count; j++)
                    {
                        if (this.pivotGrid1.PivotEngine[j, col - 1].Value!=null && this.pivotGrid1.PivotEngine[j, col - 1].Value.ToString() != "x")
                            dr[this.pivotGrid1.PivotColumns[j].FieldMappingName] = this.pivotGrid1.PivotEngine[j, col - 1].Value;
                    }
                    for (int k = 0; k < this.pivotGrid1.PivotCalculations.Count; k++)
                    {
                        dr[this.pivotGrid1.PivotCalculations[k].FieldName] = this.pivotGrid1.PivotEngine[row - 1, col - 1].Value;
                    }
                    string val = string.Empty;
                    if (this.pivotGrid1.TableModel[row, this.pivotGrid1.PivotRows.Count].CellValue != null)
                        val = this.pivotGrid1.TableModel[row, this.pivotGrid1.PivotRows.Count].CellValue.ToString();
                    else if (this.pivotGrid1.TableModel[row, this.pivotGrid1.PivotRows.Count - 1].CellValue != null)
                        val = this.pivotGrid1.TableModel[row, this.pivotGrid1.PivotRows.Count - 1].CellValue.ToString();
                    else
                        val = this.pivotGrid1.TableModel[row, this.pivotGrid1.PivotRows.Count - this.pivotGrid1.PivotRows.Count + 1].CellValue.ToString();
                    string name = this.pivotGrid1.TableModel[colHeaderIndex + 1, col].CellValue.ToString();
                    if (val != "x")
                    {
                        if (startIndex < count)
                        {
                            if (this.pivotGrid1.TableModel[row, col].CellValue != null)
                            {
                                this.chartControl1.Series[startIndex].Points.Add(startRowIndex, double.Parse(this.pivotGrid1.TableModel[row, col].CellValue.ToString()));
                            }
                            else if (this.pivotGrid1.TableModel[row, col].CellValue == null && this.pivotGrid1.TableModel[row, col].Text == string.Empty)
                                this.chartControl1.Series[startIndex].Points.Add(startRowIndex, 0);
                            if (this.chartControl1.Series[startIndex].Name == string.Empty)
                            {
                                for (int i = 1; i <= headerCount; i++)
                                {
                                    if (this.pivotGrid1.TableModel[i, col].Text != string.Empty)
                                    {
                                        if (i != headerCount)
                                            this.chartControl1.Series[startIndex].Name += this.pivotGrid1.TableModel[i, col].Text + "-";
                                        else
                                            this.chartControl1.Series[startIndex].Name += this.pivotGrid1.TableModel[i, col].Text;
                                    }
                                    else
                                    {
                                        GridRangeInfo GroupHeaderRange = this.pivotGrid1.TableModel.CoveredRanges.FindRange(i, col);
                                        if (i != headerCount)
                                            this.chartControl1.Series[startIndex].Name += this.pivotGrid1.TableModel[i, GroupHeaderRange.Left].Text + "-";
                                        else
                                            this.chartControl1.Series[startIndex].Name += this.pivotGrid1.TableModel[i, GroupHeaderRange.Left].Text;
                                    }
                                }
                                this.chartControl1.Series[startIndex].Text = this.chartControl1.Series[startIndex].Name;
                            }
                        }
                    }
                }
                table.Rows.Add(dr);
            }
            secYAxis.DrawGrid = true;
            secYAxis.HidePartialLabels = true;
            secYAxis.LabelIntersectAction = ChartLabelIntersectAction.Rotate;
            secYAxis.LineType.ForeColor = Color.FromArgb(213, 219, 204);
            secYAxis.ValueType = ChartValueType.Double;
            secYAxis.Orientation = ChartOrientation.Vertical;
            secYAxis.GridLineType.BackColor = Color.FromArgb(250, 209, 150, 150);
            secYAxis.GridLineType.ForeColor = Color.FromArgb(250, 230, 193, 193);
            secYAxis.GridLineType.PenType = System.Drawing.Drawing2D.PenType.LinearGradient;
            secYAxis.GridLineType.Width = 0;
            secYAxis.LineType.ForeColor = Color.FromArgb(213, 219, 204);
            secYAxis.Font = new Font("Verdana", 8f);
            this.chartControl1.Axes.Add(secYAxis);
            for (int seriesCount = 0; seriesCount < range.Width; seriesCount++)
            {
                if (seriesCount % 2 != 0)
                    this.chartControl1.Series[seriesCount].YAxis = secYAxis;
            }
            this.chartControl1.ChartArea.YAxesLayoutMode = ChartAxesLayoutMode.Stacking;
            this.chartControl1.Series[0].XAxis.DesiredIntervals = range.Bottom - range.Top + 1;
        }
        #endregion

        #region Smart Labels
        /// <summary>
        /// To apply styles to series text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GridChartHelper_PrepareStyle(object sender, ChartPrepareStyleInfoEventArgs args)
        {
            ChartSeries series = sender as ChartSeries;
            args.Style.DisplayText = true;
            args.Style.TextOrientation = ChartTextOrientation.Smart;
            args.Style.Text = series.Name;
            series.Style.Font.Facename = "Arial";
            series.Style.Font.Size = 7;
        }
        #endregion
    }
}
