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
using System.Drawing;
using System.Collections;
using Syncfusion.GridExcelConverter;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    /// Types of advanced border
    /// </summary>
    public enum BorderType
    {
        /// <summary>
        /// Doubl border for the cells
        /// </summary>
        Double
    }
    /// <summary>
    /// The advanced border for the cells
    /// </summary>
    public class GridBorderAdv
    {
        GridControl grid;
        BorderType type;
        GridRangeInfoList range = new GridRangeInfoList();
        GridRangeInfoList toprange = new GridRangeInfoList();
        GridRangeInfoList bottomrange = new GridRangeInfoList();
        GridRangeInfoList leftrange = new GridRangeInfoList();
        GridRangeInfoList rightrange = new GridRangeInfoList();
        Dictionary<string, GridBorderSide> cellcoll = new Dictionary<string, GridBorderSide>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GridBorderAdv"/> class.
        /// </summary>
        /// <param name="grid">The grid.</param>
        /// <param name="type">The type.</param>
        public GridBorderAdv(GridControl grid, BorderType type)
        {
            this.grid = grid;
            this.type = type;
            grid.DrawCellFrameAppearance += new Syncfusion.Windows.Forms.Grid.GridDrawCellBackgroundEventHandler(OnDrawCellFrameAppearance);
            grid.RowsInserted += new GridRangeInsertedEventHandler(grid_RowsInserted);
            grid.ColsInserted += new GridRangeInsertedEventHandler(grid_ColsInserted);
            grid.RowsRemoved += new GridRangeRemovedEventHandler(grid_RowsRemoved);
            grid.ColsRemoved += new GridRangeRemovedEventHandler(grid_ColsRemoved);
            grid.SelectionChanged += new GridSelectionChangedEventHandler(grid_SelectionChanged);
            grid.PrepareViewStyleInfo += new GridPrepareViewStyleInfoEventHandler(grid_PrepareViewStyleInfo);
        }

        void grid_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e)
        {
            if ((cellcoll.ContainsKey(grid[e.RowIndex, e.ColIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[e.RowIndex, e.ColIndex].CellIdentity.ToString()] == GridBorderSide.Right ||
                cellcoll[grid[e.RowIndex, e.ColIndex].CellIdentity.ToString()] == GridBorderSide.All)) ||
                (cellcoll.ContainsKey(grid[e.RowIndex, e.ColIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[e.RowIndex, e.ColIndex + 1].CellIdentity.ToString()] == GridBorderSide.Left ||
                cellcoll[grid[e.RowIndex, e.ColIndex + 1].CellIdentity.ToString()] == GridBorderSide.All)))
            {
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.None);
            }
        }

        void grid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            grid.InvalidateRange(e.Range);
        }

        private void grid_RowsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            bool valid = false;
            Dictionary<string, GridBorderSide> tempadd = new Dictionary<string, GridBorderSide>();
            ArrayList temprem = new ArrayList();
            foreach (string keys in cellcoll.Keys)
            {
                int rowindex = int.Parse(keys.Substring(keys.IndexOf("= ") + 2, keys.IndexOf(",") - keys.IndexOf("= ") - 2));
                if (rowindex + 1 > e.InsertAt)
                {
                    temprem.Add(keys);
                    string tempstr = keys.Replace("rowIndex = " + rowindex, "rowIndex = " + (rowindex + e.Count));
                    tempadd.Add(tempstr, cellcoll[keys]);
                    valid = true;
                }
            }
            foreach (string keys in temprem)
            {
                cellcoll.Remove(keys);
            }
            foreach (string keys in tempadd.Keys)
            {
                cellcoll.Add(keys, tempadd[keys]);
            }
            ////grid.Refresh();
            if (valid)
                new MethodInvoker(ResetBorders).Invoke();
        }

        private void grid_ColsInserted(object sender, GridRangeInsertedEventArgs e)
        {
            bool valid = false;
            Dictionary<string, GridBorderSide> tempadd = new Dictionary<string, GridBorderSide>();
            ArrayList temprem = new ArrayList();
            foreach (string keys in cellcoll.Keys)
            {
                int colindex = int.Parse(keys.Substring(keys.IndexOf("colIndex = ") + 11, keys.IndexOf(" }") - keys.IndexOf("colIndex = ") - 11));
                if (colindex + 1 > e.InsertAt)
                {
                    temprem.Add(keys);
                    string tempstr = keys.Replace("colIndex = " + colindex, "colIndex = " + (colindex + e.Count));
                    tempadd.Add(tempstr, cellcoll[keys]);
                    valid = true;
                }
            }
            foreach (string keys in temprem)
            {
                cellcoll.Remove(keys);
            }
            foreach (string keys in tempadd.Keys)
            {
                cellcoll.Add(keys, tempadd[keys]);
            }
            ////grid.Refresh();
            if (valid)
                new MethodInvoker(ResetBorders).Invoke();
        }

        private void grid_RowsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            Dictionary<string, GridBorderSide> tempadd = new Dictionary<string, GridBorderSide>();
            ArrayList temprem = new ArrayList();
            foreach (string keys in cellcoll.Keys)
            {
                int rowindex = int.Parse(keys.Substring(keys.IndexOf("= ") + 2, keys.IndexOf(",") - keys.IndexOf("= ") - 2));
                if (rowindex >= e.From)
                {
                    temprem.Add(keys);
                }
                if (rowindex > e.To)
                {
                    string tempstr = keys.Replace("rowIndex = " + rowindex, "rowIndex = " + (rowindex - (e.To - e.From + 1)));
                    tempadd.Add(tempstr, cellcoll[keys]);
                }
            }
            foreach (string keys in temprem)
            {
                cellcoll.Remove(keys);
            }
            foreach (string keys in tempadd.Keys)
            {
                cellcoll.Add(keys, tempadd[keys]);
            }
            //grid.Refresh();
            ResetBorders();
        }

        private void grid_ColsRemoved(object sender, GridRangeRemovedEventArgs e)
        {
            Dictionary<string, GridBorderSide> tempadd = new Dictionary<string, GridBorderSide>();
            ArrayList temprem = new ArrayList();
            foreach (string keys in cellcoll.Keys)
            {
                int colindex = int.Parse(keys.Substring(keys.IndexOf("colIndex = ") + 11, keys.IndexOf(" }") - keys.IndexOf("colIndex = ") - 11));
                if (colindex >= e.From)
                {
                    temprem.Add(keys);
                }
                if (colindex > e.To)
                {
                    string tempstr = keys.Replace("colIndex = " + colindex, "colIndex = " + (colindex - (e.To - e.From + 1)));
                    tempadd.Add(tempstr, cellcoll[keys]);
                }
            }
            foreach (string keys in temprem)
            {
                cellcoll.Remove(keys);
            }
            foreach (string keys in tempadd.Keys)
            {
                cellcoll.Add(keys, tempadd[keys]);
            }
            //grid.Refresh();
            ResetBorders();
        }

        /// <summary>
        /// To reset the double border applied in cells.
        /// </summary>
        public void ResetAll()
        {
            if (this.toprange.Count > 0)
            {
                foreach (GridRangeInfo range in toprange)
                {
                    this.grid[range.Top - 1, range.Left].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                }
            }

            if (this.bottomrange.Count > 0)
            {
                foreach (GridRangeInfo range in bottomrange)
                {
                    this.grid[range.Bottom, range.Right].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                }
            }

            if (this.leftrange.Count > 0)
            {
                foreach (GridRangeInfo range in leftrange)
                {
                    this.grid[range.Top, range.Left - 1].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                }
            }

            if (this.rightrange.Count > 0)
            {
                foreach (GridRangeInfo range in rightrange)
                {
                    this.grid[range.Bottom, range.Right].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                }
            }
            this.cellcoll.Clear();
            this.grid.Refresh();
        }

        /// <summary>
        /// To remove double borders applied in specific range.
        /// </summary>
        /// <param name="range">GridRangeInfo</param>
        /// <param name="side">GridBorderSide</param>
        public void RemoveRange(GridRangeInfo range, GridBorderSide side)
        {
            if (range.RangeType == GridRangeInfoType.Cells)
            {
                for (int row = range.Top; row <= range.Bottom; row++)
                {
                    for (int col = range.Left; col <= range.Right; col++)
                    {
                        if (this.cellcoll.ContainsKey(this.grid[row, col].CellIdentity.ToString()))
                        {
                            this.grid[row - 1, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col - 1].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.cellcoll[this.grid[row, col].CellIdentity.ToString()] &= ~side;
                            if (this.cellcoll[this.grid[row, col].CellIdentity.ToString()] == 0)
                                this.cellcoll.Remove(this.grid[row, col].CellIdentity.ToString());
                        }
                    }
                }
            }
            else if (range.RangeType == GridRangeInfoType.Cols)
            {
                for (int row = this.grid.Rows.HeaderCount + 1; row < this.grid.RowCount + this.grid.Rows.HeaderCount; row++)
                {
                    for (int col = range.Left; col <= range.Right; col++)
                    {
                        if (this.cellcoll.ContainsKey(this.grid[row, range.Left].CellIdentity.ToString()))
                        {
                            this.grid[row - 1, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col - 1].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.cellcoll[this.grid[row, col].CellIdentity.ToString()] &= ~side;
                            if (this.cellcoll[this.grid[row, col].CellIdentity.ToString()] == 0)
                                this.cellcoll.Remove(this.grid[row, col].CellIdentity.ToString());
                        }
                    }
                }
            }
            else if (range.RangeType == GridRangeInfoType.Rows)
            {
                for (int row = range.Top; row <= range.Bottom; row++)
                {
                    for (int col = this.grid.Cols.HeaderCount; col < this.grid.ColCount + this.grid.Cols.HeaderCount; col++)
                    {
                        if (this.cellcoll.ContainsKey(this.grid[row, range.Left].CellIdentity.ToString()))
                        {
                            this.grid[row - 1, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col - 1].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);
                            this.grid[row, col].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                            this.cellcoll[this.grid[row, col].CellIdentity.ToString()] &= ~side;
                            if (this.cellcoll[this.grid[row, col].CellIdentity.ToString()] == 0)
                                this.cellcoll.Remove(this.grid[row, col].CellIdentity.ToString());
                        }
                    }
                }
            }
            else if (range.RangeType == GridRangeInfoType.Table)
            {
                this.ResetAll();
            }
            ResetBorders();
            this.grid.Refresh();
        }

        private void ResetBorders()
        {
            toprange.Clear();
            bottomrange.Clear();
            leftrange.Clear();
            rightrange.Clear();
            for (int i = 1; i < grid.RowCount; i++)
            {
                for (int j = 0; j < grid.ColCount; j++)
                {
                    if (grid[i, j].Borders.Left == new GridBorder(GridBorderStyle.None))
                        grid[i, j].Borders.Left = new GridBorder(GridBorderStyle.Standard);
                    if (grid[i, j].Borders.Right == new GridBorder(GridBorderStyle.None))
                        grid[i, j].Borders.Right = new GridBorder(GridBorderStyle.Standard);
                    if (grid[i, j].Borders.Top == new GridBorder(GridBorderStyle.None))
                        grid[i, j].Borders.Top = new GridBorder(GridBorderStyle.Standard);
                    if (grid[i, j].Borders.Bottom == new GridBorder(GridBorderStyle.None))
                        grid[i, j].Borders.Bottom = new GridBorder(GridBorderStyle.Standard);

                    if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                    {
                        if (cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("All") ||
                            cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("Bottom"))
                        {
                            grid[i, j].Borders.Bottom = new GridBorder(GridBorderStyle.None);
                            bottomrange.Add(GridRangeInfo.Cell(i, j));
                        }
                    }
                    if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                    {
                        if (cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("All") ||
                            cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("Right"))
                        {
                            grid[i, j].Borders.Right = new GridBorder(GridBorderStyle.None);
                            rightrange.Add(GridRangeInfo.Cell(i, j));
                        }
                    }
                    if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                    {
                        if (cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("All") ||
                            cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("Left"))
                        {
                            grid[i, j - 1].Borders.Right = new GridBorder(GridBorderStyle.None);
                            leftrange.Add(GridRangeInfo.Cell(i, j));
                        }
                    }
                    if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                    {
                        if (cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("All") ||
                            cellcoll[grid[i, j].CellIdentity.ToString()].ToString().Contains("Top"))
                        {
                            grid[i - 1, j].Borders.Bottom = new GridBorder(GridBorderStyle.None);
                            toprange.Add(GridRangeInfo.Cell(i, j));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Adds the ranges for the cells to have double border.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="side">The side.</param>
        public void AddRanges(GridRangeInfo range, GridBorderSide side)
        {
            this.range.Add(range);
            for (int i = range.Top; i <= range.Bottom; i++)
            {
                for (int j = range.Left; j <= range.Right; j++)
                {
                    if (side == GridBorderSide.Top || side == GridBorderSide.All)
                    {
                        if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                            cellcoll[grid[i, j].CellIdentity.ToString()] |= GridBorderSide.Top;
                        else
                            cellcoll.Add(grid[i, j].CellIdentity.ToString(), GridBorderSide.Top);
                    }
                    if (side == GridBorderSide.Bottom || side == GridBorderSide.All)
                    {
                        if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                            cellcoll[grid[i, j].CellIdentity.ToString()] |= GridBorderSide.Bottom;
                        else
                            cellcoll.Add(grid[i, j].CellIdentity.ToString(), GridBorderSide.Bottom);
                    }
                    if (side == GridBorderSide.Right || side == GridBorderSide.All)
                    {
                        if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                            cellcoll[grid[i, j].CellIdentity.ToString()] |= GridBorderSide.Right;
                        else
                            cellcoll.Add(grid[i, j].CellIdentity.ToString(), GridBorderSide.Right);
                    }
                    if (side == GridBorderSide.Left || side == GridBorderSide.All)
                    {
                        if (cellcoll.ContainsKey(grid[i, j].CellIdentity.ToString()))
                            cellcoll[grid[i, j].CellIdentity.ToString()] |= GridBorderSide.Left;
                        else
                            cellcoll.Add(grid[i, j].CellIdentity.ToString(), GridBorderSide.Left);
                    }
                }
            }
            if (side == GridBorderSide.Top || side == GridBorderSide.All)
            {
                toprange.Add(range);
            }
            if (side == GridBorderSide.Bottom || side == GridBorderSide.All)
            {
                bottomrange.Add(range);
            }
            if (side == GridBorderSide.Right || side == GridBorderSide.All)
            {
                rightrange.Add(range);
            }
            if (side == GridBorderSide.Left || side == GridBorderSide.All)
            {
                leftrange.Add(range);
            }
        }

        Dictionary<string, string> colorcell = new Dictionary<string, string>();
        /// <summary>
        /// Provides a range of colors to be applied for the cell.
        /// </summary>
        /// <param name="range">GridRangeInfo</param>
        /// <param name="side">GridBorderSide</param>
        /// <param name="color">Color</param>
        public void ColorRange(GridRangeInfo range, GridBorderSide side, Color color)
        {
            for (int i = range.Top; i <= range.Bottom; i++)
            {
                for (int j = range.Left; j <= range.Right; j++)
                {
                    if (!colorcell.ContainsKey(grid[i, j].CellIdentity.ToString()))
                        colorcell.Add(grid[i, j].CellIdentity.ToString(), side + ":" + color);
                    else
                        colorcell[grid[i, j].CellIdentity.ToString()] = side + ":" + color;
                }
            }
        }

        /// <summary>
        /// Exports the border adv to excel.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public void ExportBorderAdv(GridExcelConverterControl converter)
        {
            converter.QueryImportExportCellInfo += new GridImportExportCellInfoEventHandler(OnQueryImportExportCellInfo);
        }

        private void OnQueryImportExportCellInfo(object sender, GridImportExportCellInfoEventArgs e)
        {
            if (toprange.AnyRangeContains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
            {
                e.ExcelCell[e.RowIndex, e.ColIndex].CellStyle.Borders[Syncfusion.XlsIO.ExcelBordersIndex.EdgeTop].LineStyle = Syncfusion.XlsIO.ExcelLineStyle.Double;
            }
            if (bottomrange.AnyRangeContains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
            {
                e.ExcelCell[e.RowIndex + 1, e.ColIndex].CellStyle.Borders[Syncfusion.XlsIO.ExcelBordersIndex.EdgeTop].LineStyle = Syncfusion.XlsIO.ExcelLineStyle.Double;
            }
            if (leftrange.AnyRangeContains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
            {
                e.ExcelCell[e.RowIndex, e.ColIndex].CellStyle.Borders[Syncfusion.XlsIO.ExcelBordersIndex.EdgeLeft].LineStyle = Syncfusion.XlsIO.ExcelLineStyle.Double;
            }
            if (rightrange.AnyRangeContains(GridRangeInfo.Cell(e.RowIndex, e.ColIndex)))
            {
                e.ExcelCell[e.RowIndex, e.ColIndex + 1].CellStyle.Borders[Syncfusion.XlsIO.ExcelBordersIndex.EdgeLeft].LineStyle = Syncfusion.XlsIO.ExcelLineStyle.Double;
            }
        }

        void OnDrawCellFrameAppearance(object sender, Syncfusion.Windows.Forms.Grid.GridDrawCellBackgroundEventArgs e)
        {

            int rowIndex = e.Style.CellIdentity.RowIndex;
            int colIndex = e.Style.CellIdentity.ColIndex;

            Brush brush;
            Rectangle rect = e.TargetBounds;
            Graphics g = e.Graphics;
            brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.Black, Color.Black, 45f);
            String colorstring = "Black";
            //top
            if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
            {
                if (colorcell.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("Top") ||
                    colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("All")))
                {
                    colorstring = colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()];
                    int start = colorstring.IndexOf("Color [") + 7;
                    int end = colorstring.IndexOf("]");
                    colorstring = colorstring.Substring(start,end-start);
                    brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.FromName(colorstring), Color.FromName(colorstring), 45f);
                }
                //top-bottom
                if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 1, 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, rect.Width, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, rect.Width + 2, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, rect.Width + 1, 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y + 1, rect.Width - 1, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y + 1, rect.Width + 1, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y + 1, rect.Width, 1));
                    }
                }


                //top-top
                if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, rect.Width - 2, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, rect.Width, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, rect.Width - 1, 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 1, 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y - 1, rect.Width - 1, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y - 1, rect.Width + 1, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Y - 1, rect.Width, 1));
                    }
                }
                brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.Black, Color.Black, 45f);
            }

            //bottom
            if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
            {
                if (colorcell.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("Bottom")||
                    colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("All")))
                {
                    colorstring = colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()];
                    int start = colorstring.IndexOf("Color [") + 7;
                    int end = colorstring.IndexOf("]");
                    colorstring = colorstring.Substring(start, end - start);
                    brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.FromName(colorstring), Color.FromName(colorstring), 45f);
                }
                //bottom-top
                if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom - 1, rect.Width - 2, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString())
                        && (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                            cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom - 1, rect.Width, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom - 1, rect.Width - 1, 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                            cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom - 1, rect.Width, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString())
                        && (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                            cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom - 1, rect.Width + 2, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom - 1, rect.Width + 1, 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom - 1, rect.Width - 1, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString())
                        && (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                            cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom - 1, rect.Width + 1, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
                    }
                }
                //bottom-bottom
                if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom + 1, rect.Width - 2, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom + 1, rect.Width, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Bottom + 1, rect.Width - 1, 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom + 1, rect.Width, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom + 1, rect.Width + 2, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Bottom + 1, rect.Width + 1, 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom + 1, rect.Width - 1, 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                    cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                    (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                    (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                    cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom + 1, rect.Width + 1, 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X, rect.Bottom + 1, rect.Width, 1));
                    }
                }
                brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.Black, Color.Black, 45f);
            }

            //left
            if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
            {
                if (colorcell.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("Left")||
                    colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("All")))
                {
                    colorstring = colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()];
                    int start = colorstring.IndexOf("Color [") + 7;
                    int end = colorstring.IndexOf("]");
                    colorstring = colorstring.Substring(start, end - start);
                    brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.FromName(colorstring), Color.FromName(colorstring), 45f);
                }
                //left-right
                if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height - 2));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height - 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, 1, rect.Height));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, 1, rect.Height + 2));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y - 1, 1, rect.Height));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y, 1, rect.Height - 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y, 1, rect.Height + 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X + 1, rect.Y, 1, rect.Height));
                    }
                }

                //left-left
                if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, 1, rect.Height - 2));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, 1, rect.Height));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y + 1, 1, rect.Height - 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, 1, rect.Height));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, 1, rect.Height + 2));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y - 1, 1, rect.Height + 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex - 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y, 1, rect.Height - 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y, 1, rect.Height + 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.X - 1, rect.Y, 1, rect.Height));
                    }
                }
                brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.Black, Color.Black, 45f);
            }

            //right
            if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Right") ||
                cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Left") ||
                cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
            {
                if (colorcell.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                    (colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("Right")||
                    colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()].Contains("All")))
                {
                    colorstring = colorcell[grid[rowIndex, colIndex].CellIdentity.ToString()];
                    int start = colorstring.IndexOf("Color [") + 7;
                    int end = colorstring.IndexOf("]");
                    colorstring = colorstring.Substring(start, end - start);
                    brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.FromName(colorstring), Color.FromName(colorstring), 45f);
                }
                //right-left
                if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y + 1, 1, rect.Height - 2));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y + 1, 1, rect.Height));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y + 1, 1, rect.Height - 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y - 1, 1, rect.Height));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y - 1, 1, rect.Height + 2));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y - 1, 1, rect.Height + 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height - 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height + 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height));
                    }
                }

                //right-right
                if ((cellcoll.ContainsKey(grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y + 1, 1, rect.Height - 2));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y + 1, 1, rect.Height));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y + 1, 1, rect.Height - 1));
                    }
                }
                else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex - 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex - 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y - 1, 1, rect.Height));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y - 1, 1, rect.Height + 2));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y - 1, 1, rect.Height + 1));
                    }
                }
                else
                {
                    if ((cellcoll.ContainsKey(grid[rowIndex, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex + 1].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y, 1, rect.Height - 1));
                    }
                    else if ((cellcoll.ContainsKey(grid[rowIndex, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("Bottom") ||
                        cellcoll[grid[rowIndex, colIndex].CellIdentity.ToString()].ToString().Contains("All"))) ||
                        (cellcoll.ContainsKey(grid[rowIndex + 1, colIndex].CellIdentity.ToString()) &&
                        (cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("Top") ||
                        cellcoll[grid[rowIndex + 1, colIndex].CellIdentity.ToString()].ToString().Contains("All"))))
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y, 1, rect.Height + 1));
                    }
                    else
                    {
                        g.FillRectangle(brush, new Rectangle(rect.Right + 1, rect.Y, 1, rect.Height));
                    }
                }
                brush = new System.Drawing.Drawing2D.LinearGradientBrush(e.TargetBounds, Color.Black, Color.Black, 45f);
            }

            brush.Dispose();
            e.Cancel = true;
            grid.NotifyCellHighlighted(rowIndex, colIndex, e.Style);
        }

        /// <summary>
        /// Saves the Double Border state through file stream
        /// </summary>
        public void SaveDoubleBorder(string path)
        {
            FileStream fs = new FileStream
                   (path + ".dat", FileMode.OpenOrCreate, FileAccess.Write);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                //as easy as 1,2,3...we serialize a to a binary 
                //formating using file stream.

                bf.Serialize(fs, this.cellcoll);
            }
            catch
            {
#if DEBUG
                Debug.WriteLine("Double Border serialization Failed");
#endif
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Saves the Double Border state through memory stream
        /// </summary>
        public void SaveDoubleBorder(MemoryStream ms)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                //as easy as 1,2,3...we serialize a to a binary 
                //formating using file stream.

                bf.Serialize(ms, this.cellcoll);
                ms.Seek(0, 0);
            }
            catch
            {
#if DEBUG
                Debug.WriteLine("Double Border serialization Failed");
#endif
            }
            finally
            {
            }
        }

        /// <summary>
        /// Loads the saved double border state if any from file stream.
        /// </summary>
        public void LoadDoubleBorder(string path)
        {
            FileStream fs = new FileStream
                   (path + ".dat", FileMode.OpenOrCreate, FileAccess.Read);

            if (fs != null && fs.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    grid.BeginUpdate();
                    this.cellcoll = bf.Deserialize(fs) as Dictionary<string, GridBorderSide>;
                    ResetBorders();                    
                }
                catch
                {
#if DEBUG
                    Debug.WriteLine("Double border Deserialiization Failed");                    
#endif
                }
                finally
                {
                    grid.EndUpdate();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Loads the saved double border state if any from memory stram.
        /// </summary>
        public void LoadDoubleBorder(MemoryStream ms)
        {           

            if (ms != null && ms.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    grid.BeginUpdate();
                    this.cellcoll = bf.Deserialize(ms) as Dictionary<string, GridBorderSide>;
                    ms.Seek(0, 0);
                    ResetBorders();
                }
                catch
                {
#if DEBUG
                    Debug.WriteLine("Double border Deserialiization Failed");
#endif
                }
                finally
                {
                    grid.EndUpdate();
                }
            }
        }
    }
}
