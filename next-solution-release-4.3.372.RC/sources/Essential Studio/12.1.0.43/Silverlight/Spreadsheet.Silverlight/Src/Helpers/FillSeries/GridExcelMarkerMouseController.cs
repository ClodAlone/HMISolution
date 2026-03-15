#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Scroll;
using Syncfusion.Windows.Diagnostics;
using Syncfusion.Windows.ComponentModel;
using System.Windows.Interop;
using Syncfusion.Windows.Controls.Grid;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Globalization;
using System.Collections.Generic;
using Syncfusion.Windows.Tools.Controls;
#if SILVERLIGHT
using System.Windows.Browser;
#endif
using Syncfusion.Windows.Shared;
using System.Windows.Media.Imaging;
using Syncfusion.XlsIO;
using Syncfusion.Windows.Styles;
using System.Collections;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public delegate void ExcelRangeExtendedEventHandler(object sender, ExcelRangeExtendedEventArgs e);

    public class GridExcelMarkerMouseController : IMouseController, IDisposable, IFillOptionChanged
    {
        GridControlBase grid;
        IWorksheet Worksheet;
        Popup codePopup = new Popup();
        string filltype = "FillSeries";
        public GridExcelMarkerMouseController(GridControlBase grid, IWorksheet worksheet)
        {
            this.grid = grid;
            this.Worksheet = worksheet;
            grid.SelectionChanged += new GridSelectionChangedEventHandler(grid_SelectionChanged);
            this.ExcelRangeExtended += new ExcelRangeExtendedEventHandler(GridExcelMarkerMouseController_ExcelRangeExtended);
        }

        void GridExcelMarkerMouseController_ExcelRangeExtended(object sender, ExcelRangeExtendedEventArgs e)
        {
        }

        void grid_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            //grid.InvalidateCells();
            if (codePopup != null)
            {
                codePopup.IsOpen = false;
            }
            filltype = "FillSeries";
        }


        #region IMouseController Members

        public string Name
        {
            get { return "ExcelMarkerMouseController"; }
        }

        Cursor cursorCross = null;


        public Cursor Cursor
        {
            get
            {
                if (cursorCross == null)
                {
                    try
                    {
                        Type type = typeof(GridExcelMarkerMouseController);
                        string cursorstring = "Syncfusion.Windows.Controls.Spreadsheet.Helpers.FillSeries.Cross.cur";
                        Stream stream = type.Module.Assembly.GetManifestResourceStream(cursorstring);
#if !SILVERLIGHT
                        cursorCross = new Cursor(stream);
#else
                        cursorCross = Cursors.Stylus;
#endif
                    }
                    catch (System.Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        throw exception;
                    }
                }
                return cursorCross;
            }
        }

        public void MouseHoverEnter(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void MouseHover(MouseControllerEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void MouseHoverLeave(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        bool isMouseDown = false;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        Point mouseDownPoint = new Point(0, 0);
        MovingDirection moveDir = MovingDirection.None;
        GridRangeInfo currentbaserange = null;

        public void MouseDown(MouseControllerEventArgs e)
        {
            isMouseDown = true;
            mouseDownRange = grid.Model.SelectedCells;
            mouseDownPoint = e.Location;
            moveDir = MovingDirection.None;
            flag = 0;
            grid.CurrentCell.EndEdit();
            this.currentbaserange = this.grid.Model.SelectedCells;
            buttonside = e.Button;
        }
        int flag = 0;
        int top = 0;
        int right = 0;
        int bottom = 0;
        int left = 0;
        public void MouseMove(MouseControllerEventArgs e)
        {
            RowColumnIndex cell = grid.PointToCellRowColumnIndex(e.Location);
            if (isMouseDown && cell.RowIndex != 0 && cell.ColumnIndex != 0)
            {

                if (cell.RowIndex < grid.TopRowIndex && grid.TopRowIndex > grid.Model.FrozenRows)
                {
                    grid.TopRowIndex = grid.TopRowIndex - 1;
                }
                else if (cell.ColumnIndex < grid.LeftColumnIndex && grid.LeftColumnIndex > grid.Model.FrozenColumns)
                {
                    grid.LeftColumnIndex = grid.LeftColumnIndex - 1;
                }

                Rect r = grid.CellSpanToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, new CellSpanInfo(cell.RowIndex, cell.ColumnIndex, cell.RowIndex, cell.ColumnIndex));


                if (flag == 0)
                {
                    top = mouseDownRange.Top;
                    bottom = mouseDownRange.Bottom;
                    left = mouseDownRange.Left;
                    right = mouseDownRange.Right;
                    // MessageBox.Show(mouseDownRange.Left.ToString());
                    flag = 1;
                }


                if (mouseDownRange.Top == top && mouseDownRange.Bottom == bottom && mouseDownRange.Right == right && mouseDownRange.Left == left && flag == 1)
                {
                    moveDir = MovingDirection.None;
                    //flag = 0;
                }

                if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Down) &&
                   (cell.RowIndex > mouseDownRange.Bottom || flag == 1) && cell.RowIndex > mouseDownRange.Bottom)
                {
                    moveDir = MovingDirection.Down;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, mouseDownRange.Right)));
                    grid.ScrollInView(new RowColumnIndex(Math.Min(grid.Model.RowCount, cell.RowIndex), cell.ColumnIndex));
                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Up) &&
                    cell.RowIndex < mouseDownRange.Top)
                {


                    moveDir = MovingDirection.Up;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, mouseDownRange.Right)));
                    grid.ScrollInView(new RowColumnIndex(Math.Max(grid.Model.FrozenRows + 1, cell.RowIndex - 1), cell.ColumnIndex));
                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Right) &&
                    cell.ColumnIndex > mouseDownRange.Right)
                {
                    if (cell.RowIndex >= mouseDownRange.Top && cell.RowIndex <= mouseDownRange.Bottom)
                    {
                        moveDir = MovingDirection.Right;
                        grid.Model.Selections.Clear();
                        grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex)));
                        grid.ScrollInView(new RowColumnIndex(cell.RowIndex, Math.Min(grid.Model.ColumnCount, cell.ColumnIndex + 1)));
                    }
                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Left) &&
                    cell.ColumnIndex < mouseDownRange.Left)
                {
                    moveDir = MovingDirection.Left;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(mouseDownRange.Top, cell.ColumnIndex)));
                    grid.ScrollInView(new RowColumnIndex(cell.RowIndex, Math.Max(grid.Model.FrozenColumns + 1, cell.ColumnIndex - 1)));
                }

                if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Down) &&
                    cell.RowIndex > mouseDownRange.Bottom)
                {

                    moveDir = MovingDirection.Down;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, mouseDownRange.Right)));
                    grid.ScrollInView(new RowColumnIndex(Math.Min(grid.Model.RowCount, cell.RowIndex + 1), cell.ColumnIndex));
                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Up) &&
                    cell.RowIndex < mouseDownRange.Top)
                {

                    moveDir = MovingDirection.Up;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, mouseDownRange.Right)));
                    grid.ScrollInView(new RowColumnIndex(Math.Max(grid.Model.FrozenRows + 1, cell.RowIndex - 1), cell.ColumnIndex));
                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Right) &&
                    cell.ColumnIndex > mouseDownRange.Right)
                {

                    moveDir = MovingDirection.Right;
                    grid.Model.Selections.Clear();
                    grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex)));
                    grid.ScrollInView(new RowColumnIndex(cell.RowIndex, Math.Min(grid.Model.ColumnCount, cell.ColumnIndex + 1)));

                }
                else if ((moveDir == MovingDirection.None || moveDir == MovingDirection.Left) &&
                    cell.ColumnIndex < mouseDownRange.Left)
                {
                    if (cell.RowIndex >= mouseDownRange.Top && cell.RowIndex <= mouseDownRange.Bottom)
                    {
                        moveDir = MovingDirection.Left;
                        grid.Model.Selections.Clear();
                        grid.Model.Selections.Add(mouseDownRange.UnionRange(GridRangeInfo.Cell(cell.RowIndex, cell.ColumnIndex)));
                        grid.ScrollInView(new RowColumnIndex(cell.RowIndex, Math.Max(grid.Model.FrozenColumns + 1, cell.ColumnIndex - 1)));
                    }
                }
            }
        }

        enum MovingDirection
        {
            None,
            Left,
            Down,
            Right,
            Up
        }

        private Dictionary<RowColumnIndex, GridStyleInfoStore> backUpFillStyles = new Dictionary<RowColumnIndex, GridStyleInfoStore>();
        GridCellData gcd;
        private Dictionary<RowColumnIndex, object> BackUpCellValue = new Dictionary<RowColumnIndex, object>();
        public event ExcelRangeExtendedEventHandler ExcelRangeExtended;
#if SILVERLIGHT
        private MouseButtons buttonside;
#else
        private MouseButton? buttonside;
#endif
        MovingDirection internalmovdir = MovingDirection.None;
        public void MouseUp(MouseControllerEventArgs e)
        {
            internalmovdir = moveDir;
            //throw new NotImplementedException();
            if (mouseDownRange != grid.Model.SelectedCells && ExcelRangeExtended != null)
            {
                gcd = new GridCellData();
                backUpFillStyles.Clear();
                BackUpCellValue.Clear();
                for (int row = grid.Model.SelectedCells.Top; row <= grid.Model.SelectedCells.Bottom; row++)
                {
                    for (int col = grid.Model.SelectedCells.Left; col <= grid.Model.SelectedCells.Right; col++)
                    {
                        object val = Worksheet.Range[row, col].Cells.Clone();
                        BackUpCellValue.Add(new RowColumnIndex(row, col), this.grid.Model[row, col].CellValue);
                    }
                }
                if (!FillDraggedRanges())
                {
                    moveDir = MovingDirection.None;
                    return;
                }
                ExcelRangeExtendedEventArgs arg = new ExcelRangeExtendedEventArgs(mouseDownRange, grid.Model.SelectedCells, internalmovdir.ToString(), filltype);
                ExcelRangeExtended(grid, arg);
                this.grid.InvalidateCell(this.grid.Model.SelectedCells);
            }
            moveDir = MovingDirection.None;
            Rect rangerect = grid.RangeToRect(ScrollAxisRegion.Body, ScrollAxisRegion.Body, grid.Model.SelectedCells, true, true);
#if !SILVERLIGHT
            if (buttonside == MouseButton.Left)
#else
            if (buttonside == MouseButtons.Left)
#endif
            {
                codePopup.Child = new FillDropDownItem(this);//new Border() { Child = splitButton };
#if SILVERLIGHT
                codePopup.HorizontalOffset = rangerect.Right + this.grid.PointFromRootVisual().X + 1;
                codePopup.VerticalOffset = rangerect.Bottom + this.grid.PointFromRootVisual().Y + 1;
#else
                codePopup.PlacementTarget = this.grid;
                codePopup.Placement = PlacementMode.Bottom;
                codePopup.VerticalOffset = 2;
                codePopup.HorizontalOffset = rangerect.Width + 1;
                codePopup.PlacementRectangle = rangerect;
                codePopup.MaxWidth = 40;
#endif
                codePopup.IsOpen = true;
            }
        }

        private bool FillDraggedRanges()
        {
            if (this.grid.Model.CommandStack.Enabled)
                this.grid.Model.CommandStack.BeginTrans("Fill Series -");

            if (!copyformat())
                return false;
            switch (filltype)
            {
                case "FillSeries":
                    fillseries();
                    break;
                case "CopySeries":
                    copySeries();
                    break;
                case "FillFormatOnly":
                    fillformatonly();
                    break;
                case "FillWithoutFormat":
                    fillwithoutformat();
                    break;
            }
            if (this.grid.Model.CommandStack.InTransaction)
                this.grid.Model.CommandStack.CommitTrans();

            grid.InvalidateCell(this.grid.Model.SelectedCells);
            grid.InvalidateVisual(true);
            return true;
        }

        private void fillseries()
        {
            var nv = 0d;
            DateTime dv;
            GridRangeInfo range = this.grid.Model.SelectedCells;

            if (internalmovdir == MovingDirection.Down)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var isNumberDateString = "";
                    bool isTop = true; double diff = 0;
                    int format = 0;
                    for (int top = currentbaserange.Top; top <= currentbaserange.Bottom; top++)
                    {
                        if (double.TryParse(this.grid.Model[top, leftrange].Text, out nv) && (isNumberDateString == string.Empty || isNumberDateString == "number"))
                        {
                            isNumberDateString = "number";
                            if (currentbaserange.Height == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isTop)
                            {
                                diff = diff + nv - double.Parse(this.grid.Model[top - 1, leftrange].Text);
                            }
                            isTop = false;
                        }
                        else if (DateTime.TryParse(this.grid.Model[top, leftrange].Text, out dv) && (isNumberDateString == string.Empty || isNumberDateString == "date"))
                        {
                            isNumberDateString = "date";
                            if (currentbaserange.Height == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isTop)
                            {
                                diff = diff + nv - DateTime.Parse(this.grid.Model[top - 1, leftrange].Text).Day;
                            }
                            isTop = false;
                        }
                        else
                        {
                            isNumberDateString = string.Empty;
                            break;
                        }
                    }
                    if (isNumberDateString == "number")
                    {
                        diff = currentbaserange.Height <= 1 ? 1 : diff / (currentbaserange.Height - 1);
                        FillNumber(leftrange, diff);
                    }
                    else if (isNumberDateString == "date")
                    {
                        diff = currentbaserange.Height <= 1 ? 1 : diff / (currentbaserange.Height - 1);
                        FillDate(leftrange, diff, format);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var isNumberDateString = "";
                    bool isBottom = true; double diff = 0;
                    int format = 0;
                    for (int top = currentbaserange.Bottom; top >= currentbaserange.Top; top--)
                    {
                        if (double.TryParse(this.grid.Model[top, leftrange].Text, out nv) && (isNumberDateString == string.Empty || isNumberDateString == "number"))
                        {
                            isNumberDateString = "number";
                            if (currentbaserange.Height == 1)
                            {
                                diff = -1;
                                break;
                            }
                            if (!isBottom)
                            {
                                diff = diff + nv - double.Parse(this.grid.Model[top + 1, leftrange].Text);
                            }
                            isBottom = false;
                        }
                        else if (DateTime.TryParse(this.grid.Model[top, leftrange].Text, out dv) && (isNumberDateString == string.Empty || isNumberDateString == "date"))
                        {
                            isNumberDateString = "date";
                            if (currentbaserange.Height == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isBottom)
                            {
                                diff = diff + nv - DateTime.Parse(this.grid.Model[top + 1, leftrange].Text).Day;
                            }
                            isBottom = false;
                        }
                        else
                        {
                            isNumberDateString = string.Empty;
                            break;
                        }
                    }
                    if (isNumberDateString == "number")
                    {
                        diff = currentbaserange.Height <= 1 ? -1 : diff / (currentbaserange.Height - 1);
                        FillNumber(leftrange, diff);
                    }
                    else if (isNumberDateString == "date")
                    {
                        diff = currentbaserange.Height <= 1 ? 1 : diff / (currentbaserange.Height - 1);
                        FillDate(leftrange, diff, format);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var isNumberDateString = "";
                    bool isLeft = true; double diff = 0;
                    int format = 0;
                    for (int left = currentbaserange.Left; left <= currentbaserange.Right; left++)
                    {
                        if (double.TryParse(this.grid.Model[toprange, left].Text, out nv) && (isNumberDateString == string.Empty || isNumberDateString == "number"))
                        {
                            isNumberDateString = "number";
                            if (currentbaserange.Width == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isLeft)
                            {
                                diff = diff + nv - double.Parse(this.grid.Model[toprange, left - 1].Text);
                            }
                            isLeft = false;
                        }
                        else if (DateTime.TryParse(this.grid.Model[toprange, left].Text, out dv) && (isNumberDateString == string.Empty || isNumberDateString == "date"))
                        {
                            isNumberDateString = "date";
                            if (currentbaserange.Width == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isLeft)
                            {
                                diff = diff - DateTime.Parse(this.grid.Model[toprange, left - 1].Text).Day;
                            }
                            isLeft = false;
                        }
                        else
                        {
                            isNumberDateString = string.Empty;
                            break;
                        }
                    }
                    if (isNumberDateString == "number")
                    {
                        diff = currentbaserange.Width <= 1 ? 1 : diff / (currentbaserange.Width - 1);
                        FillNumber(toprange, diff);
                    }
                    else if (isNumberDateString == "date")
                    {
                        diff = currentbaserange.Width <= 1 ? 1 : diff / (currentbaserange.Width - 1);
                        FillDate(toprange, diff, format);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var isNumberDateString = "";
                    bool isLeft = true; double diff = 0;
                    int format = 0;
                    for (int left = currentbaserange.Right; left >= currentbaserange.Left; left--)
                    {
                        if (double.TryParse(this.grid.Model[toprange, left].Text, out nv) && (isNumberDateString == string.Empty || isNumberDateString == "number"))
                        {
                            isNumberDateString = "number";
                            if (currentbaserange.Width == 1)
                            {
                                diff = -1;
                                break;
                            }
                            if (!isLeft)
                            {
                                diff = diff + nv - double.Parse(this.grid.Model[toprange, left + 1].Text);
                            }
                            isLeft = false;
                        }
                        else if (DateTime.TryParse(this.grid.Model[toprange, left].Text, out dv) && (isNumberDateString == string.Empty || isNumberDateString == "date"))
                        {
                            isNumberDateString = "date";
                            if (currentbaserange.Width == 1)
                            {
                                diff = 1;
                                break;
                            }
                            if (!isLeft)
                            {
                                diff = diff - DateTime.Parse(this.grid.Model[toprange, left + 1].Text).Day;
                            }
                            isLeft = false;
                        }
                        else
                        {
                            isNumberDateString = string.Empty;
                            break;
                        }
                    }
                    if (isNumberDateString == "number")
                    {
                        diff = currentbaserange.Width <= 1 ? -1 : diff / (currentbaserange.Width - 1);
                        FillNumber(toprange, diff);
                    }
                    else if (isNumberDateString == "date")
                    {
                        diff = currentbaserange.Width <= 1 ? 1 : diff / (currentbaserange.Width - 1);
                        FillDate(toprange, diff, format);
                    }
                }
            }
        }

        private void copySeries()
        {
            if (internalmovdir == MovingDirection.Down)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    object[] baseRangeArray = new object[currentbaserange.Height];
                    int baseindex = 0;
                    for (int top = currentbaserange.Top; top <= currentbaserange.Bottom; top++)
                    {
                        baseRangeArray[baseindex] = this.grid.Model[top, leftrange];
                        baseindex++;
                    }
                    var range = this.grid.Model.SelectedCells;
                    baseindex = 0;
                    for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                    {
                        string cellvalue = (baseRangeArray[baseindex % baseRangeArray.Length] as SpreadsheetGridStyleInfo).Text;
                        var style = this.grid.Model[top, leftrange];
                        style.CellValue = cellvalue;
                        if (cellvalue.StartsWith("="))
                            style.FormulaTag = null;
                        baseindex++;
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    object[] baseRangeArray = new object[currentbaserange.Height];
                    int baseindex = 0;
                    for (int top = currentbaserange.Top; top <= currentbaserange.Bottom; top++)
                    {
                        baseRangeArray[baseindex] = this.grid.Model[top, leftrange];
                        baseindex++;
                    }
                    var range = this.grid.Model.SelectedCells;
                    baseindex = 0;
                    for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                    {
                        string cellvalue = (baseRangeArray[baseindex % baseRangeArray.Length] as SpreadsheetGridStyleInfo).Text;
                        var style = this.grid.Model[top, leftrange];
                        style.CellValue = cellvalue;
                        if (cellvalue.StartsWith("="))
                            style.FormulaTag = null;
                        baseindex++;
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    object[] baseRangeArray = new object[currentbaserange.Width];
                    int baseindex = 0;
                    for (int left = currentbaserange.Left; left <= currentbaserange.Right; left++)
                    {
                        baseRangeArray[baseindex] = this.grid.Model[toprange, left];
                        baseindex++;
                    }
                    var range = this.grid.Model.SelectedCells;
                    baseindex = 0;
                    for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(toprange, left);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            string cellvalue = (baseRangeArray[baseindex % baseRangeArray.Length] as SpreadsheetGridStyleInfo).Text;
                            var style = this.grid.Model[toprange, left];
                            style.CellValue = cellvalue;
                            if (cellvalue.StartsWith("="))
                                style.FormulaTag = null;
                            baseindex++;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    object[] baseRangeArray = new object[currentbaserange.Width];
                    int baseindex = 0;
                    for (int left = currentbaserange.Left; left <= currentbaserange.Right; left++)
                    {
                        baseRangeArray[baseindex] = this.grid.Model[toprange, left];
                        baseindex++;
                    }
                    var range = this.grid.Model.SelectedCells;
                    baseindex = 0;
                    for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(toprange, left);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            string cellvalue = (baseRangeArray[baseindex % baseRangeArray.Length] as SpreadsheetGridStyleInfo).Text;
                            var style = this.grid.Model[toprange, left];
                            style.CellValue = cellvalue;
                            if (cellvalue.StartsWith("="))
                                style.FormulaTag = null;
                            baseindex++;
                        }
                    }
                }
            }
        }

        private void fillformatonly()
        {
            if (internalmovdir == MovingDirection.Down)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(top, leftrange);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            object cellvalue = BackUpCellValue[rowcol];
                            this.grid.Model[top,leftrange].CellValue = cellvalue;
                        }
                    }
                }
            }
            else if(internalmovdir== MovingDirection.Up)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(top, leftrange);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            object cellvalue = BackUpCellValue[rowcol];
                            this.grid.Model[top, leftrange].CellValue = cellvalue;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(toprange, left);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            object cellvalue = BackUpCellValue[rowcol];
                            this.grid.Model[toprange, left].CellValue = cellvalue;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                    {
                        RowColumnIndex rowcol = new RowColumnIndex(toprange, left);
                        if (BackUpCellValue.ContainsKey(rowcol))
                        {
                            object cellvalue = BackUpCellValue[rowcol];
                            this.grid.Model[toprange, left].CellValue = cellvalue;
                        }
                    }
                }
            }
            //copyformat();
        }

        private void fillwithoutformat()
        {
            if (internalmovdir == MovingDirection.Down)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                    {
                        Worksheet.Range[top, leftrange].Clear(ExcelClearOptions.ClearFormat);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                    {
                        Worksheet.Range[top, leftrange].Clear(ExcelClearOptions.ClearFormat);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                    {
                        Worksheet.Range[toprange, left].Clear(ExcelClearOptions.ClearFormat);
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    var range = this.grid.Model.SelectedCells;
                    for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                    {
                        Worksheet.Range[toprange, left].Clear(ExcelClearOptions.ClearFormat);
                    }
                }
            }
            fillseries();
        }

        object BaseExcelRange;
        object SelectedExcelRange;
        private bool copyformat()
        {
            var range = this.grid.Model.SelectedCells;
            BaseExcelRange = Worksheet.Range[currentbaserange.Top, currentbaserange.Left, currentbaserange.Bottom, currentbaserange.Right].Cells.Clone();
            SelectedExcelRange = Worksheet.Range[range.Top, range.Left, range.Bottom, range.Right].Cells.Clone();
            GridStyleInfo[] cellsInfo = null;
            if (internalmovdir == MovingDirection.Down)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    IRange ExcelSourceRange;
                    IEnumerator SourceRangeEnumerator = null;
                    IRange SourceRange = Worksheet.Range[currentbaserange.Top, leftrange, currentbaserange.Bottom, leftrange];
                    if (SourceRange.Cells.Length > 1)
                    {
                        SourceRangeEnumerator = SourceRange.Cells.GetEnumerator();
                        SourceRangeEnumerator.Reset();
                        SourceRangeEnumerator.MoveNext();
                        ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                    }
                    else
                        ExcelSourceRange = SourceRange;
                    for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                    {
                        IRange ExcelTargetRange = Worksheet.Range[top, leftrange];
                        if (Worksheet.IsPasswordProtected && ExcelTargetRange.CellStyle.Locked)
                            return false;
                        ExcelSourceRange.CopyTo(ExcelTargetRange, ExcelCopyRangeOptions.All | ExcelCopyRangeOptions.UpdateFormulas);

                        cellsInfo = this.grid.Model.GetCellsInfo(GridRangeInfo.Cell(top, leftrange));
                        this.grid.Model.ChangeCells(GridRangeInfo.Cell(top, leftrange), cellsInfo, StyleModifyType.Copy);

                        if (SourceRange.Cells.Length > 1 && SourceRangeEnumerator != null)
                        {
                            if (!SourceRangeEnumerator.MoveNext())
                            {
                                SourceRangeEnumerator.Reset();
                                SourceRangeEnumerator.MoveNext();
                            }
                            ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int leftrange = currentbaserange.Left; leftrange <= currentbaserange.Right; leftrange++)
                {
                    IRange ExcelSourceRange;
                    IEnumerator SourceRangeEnumerator = null;
                    IRange SourceRange = Worksheet.Range[currentbaserange.Top, leftrange, currentbaserange.Bottom, leftrange];
                    if (SourceRange.Cells.Length > 1)
                    {
                        SourceRangeEnumerator = SourceRange.Cells.GetEnumerator();
                        SourceRangeEnumerator.Reset();
                        SourceRangeEnumerator.MoveNext();
                        ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                    }
                    else
                        ExcelSourceRange = SourceRange;
                    for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                    {
                        IRange ExcelTargetRange = Worksheet.Range[top, leftrange];
                        if (Worksheet.IsPasswordProtected && ExcelTargetRange.CellStyle.Locked)
                            return false;
                        ExcelSourceRange.CopyTo(ExcelTargetRange, ExcelCopyRangeOptions.All | ExcelCopyRangeOptions.UpdateFormulas);
                        cellsInfo = this.grid.Model.GetCellsInfo(GridRangeInfo.Cell(top, leftrange));
                        this.grid.Model.ChangeCells(GridRangeInfo.Cell(top, leftrange), cellsInfo, StyleModifyType.Copy);
                        if (SourceRange.Cells.Length > 1 && SourceRangeEnumerator != null)
                        {
                            if (!SourceRangeEnumerator.MoveNext())
                            {
                                SourceRangeEnumerator.Reset();
                                SourceRangeEnumerator.MoveNext();
                            }
                            ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    IRange ExcelSourceRange;
                    IEnumerator SourceRangeEnumerator = null;
                    IRange SourceRange = Worksheet.Range[toprange, currentbaserange.Left, toprange, currentbaserange.Right];
                    if (SourceRange.Cells.Length > 1)
                    {
                        SourceRangeEnumerator = SourceRange.Cells.GetEnumerator();
                        SourceRangeEnumerator.Reset();
                        SourceRangeEnumerator.MoveNext();
                        ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                    }
                    else
                        ExcelSourceRange = SourceRange;
                    for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                    {
                        IRange ExcelTargetRange = Worksheet.Range[toprange, left];
                        if (Worksheet.IsPasswordProtected && ExcelTargetRange.CellStyle.Locked)
                            return false;
                        ExcelSourceRange.CopyTo(ExcelTargetRange, ExcelCopyRangeOptions.All | ExcelCopyRangeOptions.UpdateFormulas);
                        cellsInfo = this.grid.Model.GetCellsInfo(GridRangeInfo.Cell(toprange, left));
                        this.grid.Model.ChangeCells(GridRangeInfo.Cell(toprange, left), cellsInfo, StyleModifyType.Copy);
                        if (SourceRange.Cells.Length > 1 && SourceRangeEnumerator != null)
                        {
                            if (!SourceRangeEnumerator.MoveNext())
                            {
                                SourceRangeEnumerator.Reset();
                                SourceRangeEnumerator.MoveNext();
                            }
                            ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                        }
                    }
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int toprange = currentbaserange.Top; toprange <= currentbaserange.Bottom; toprange++)
                {
                    IRange ExcelSourceRange;
                    IEnumerator SourceRangeEnumerator = null;
                    IRange SourceRange = Worksheet.Range[toprange, currentbaserange.Left, toprange, currentbaserange.Right];
                    if (SourceRange.Cells.Length > 1)
                    {
                        SourceRangeEnumerator = SourceRange.Cells.GetEnumerator();
                        SourceRangeEnumerator.Reset();
                        SourceRangeEnumerator.MoveNext();
                        ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                    }
                    else
                        ExcelSourceRange = SourceRange;
                    for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                    {
                        IRange ExcelTargetRange = Worksheet.Range[toprange, left];
                        if (Worksheet.IsPasswordProtected && ExcelTargetRange.CellStyle.Locked)
                            return false;
                        ExcelSourceRange.CopyTo(ExcelTargetRange, ExcelCopyRangeOptions.All);
                        cellsInfo = this.grid.Model.GetCellsInfo(GridRangeInfo.Cell(toprange, left));
                        this.grid.Model.ChangeCells(GridRangeInfo.Cell(toprange, left), cellsInfo, StyleModifyType.Copy);
                        if (SourceRange.Cells.Length > 1 && SourceRangeEnumerator != null)
                        {
                            if (!SourceRangeEnumerator.MoveNext())
                            {
                                SourceRangeEnumerator.Reset();
                                SourceRangeEnumerator.MoveNext();
                            }
                            ExcelSourceRange = SourceRangeEnumerator.Current as IRange;
                        }
                    }
                }
            }
            return true;
        }

        private void FillNumber(int lefttoprange, double diff)
        {
            var range = this.grid.Model.SelectedCells;

            if (internalmovdir == MovingDirection.Down)
            {
                for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                {
                    this.grid.Model[top, lefttoprange].CellValue = double.Parse(this.grid.Model[top - 1, lefttoprange].Text) + diff;
                    if (this.grid.Model[top, lefttoprange].CellType == "FormulaCell")
                        this.grid.Model[top, lefttoprange].FormulaTag = null;
                    this.grid.Model[top, lefttoprange].HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                {
                    this.grid.Model[top, lefttoprange].CellValue = double.Parse(this.grid.Model[top + 1, lefttoprange].Text) + diff;
                    if (this.grid.Model[top, lefttoprange].CellType == "FormulaCell")
                        this.grid.Model[top, lefttoprange].FormulaTag = null;
                    this.grid.Model[top, lefttoprange].HorizontalAlignment = HorizontalAlignment.Right;
                }

            }
            else if (internalmovdir == MovingDirection.Right)
            {
                for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                {
                    this.grid.Model[lefttoprange, left].CellValue = double.Parse(this.grid.Model[lefttoprange, left - 1].Text) + diff;
                    if (this.grid.Model[top, lefttoprange].CellType == "FormulaCell")
                        this.grid.Model[top, lefttoprange].FormulaTag = null;
                    this.grid.Model[lefttoprange, left].HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                {
                    this.grid.Model[lefttoprange, left].CellValue = double.Parse(this.grid.Model[lefttoprange, left + 1].Text) + diff;
                    if (this.grid.Model[top, lefttoprange].CellType == "FormulaCell")
                        this.grid.Model[top, lefttoprange].FormulaTag = null;
                    this.grid.Model[lefttoprange, left].HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
        }

        private void FillDate(int lefttoprange, double diff, int format)
        {
            var range = this.grid.Model.SelectedCells;
            if (internalmovdir == MovingDirection.Down)
            {
                for (int top = currentbaserange.Bottom + 1; top <= range.Bottom; top++)
                {
#if !SILVERLIGHT
                    this.grid.Model[top, lefttoprange].CellValue = (DateTime.Parse(this.grid.Model[top - 1, lefttoprange].Text).AddDays(diff)).GetDateTimeFormats()[format];
#else
                    this.grid.Model[top, lefttoprange].CellValue = (DateTime.Parse(this.grid.Model[top - 1, lefttoprange].Text).AddDays(diff)).ToShortDateString();
#endif
                }
            }
            else if (internalmovdir == MovingDirection.Up)
            {
                for (int top = currentbaserange.Top - 1; top >= range.Top; top--)
                {
                    this.grid.Model[top, lefttoprange].CellValue = (DateTime.Parse(this.grid.Model[top + 1, lefttoprange].Text).AddDays(diff)).ToShortDateString();
                }
            }
            if (internalmovdir == MovingDirection.Right)
            {
                for (int left = currentbaserange.Right + 1; left <= range.Right; left++)
                {
#if !SILVERLIGHT
                    this.grid.Model[lefttoprange, left].CellValue = (DateTime.Parse(this.grid.Model[lefttoprange, left - 1].Text).AddDays(diff)).GetDateTimeFormats()[format];
#else
                    this.grid.Model[lefttoprange, left].CellValue = (DateTime.Parse(this.grid.Model[lefttoprange, left - 1].Text).AddDays(diff)).ToShortDateString();
#endif
                }
            }
            else if (internalmovdir == MovingDirection.Left)
            {
                for (int left = currentbaserange.Left - 1; left >= range.Left; left--)
                {
                    this.grid.Model[lefttoprange, left].CellValue = (DateTime.Parse(this.grid.Model[lefttoprange, left + 1].Text).AddDays(diff)).ToShortDateString();
                }
            }
        }

        public void CancelMode()
        {
            //throw new NotImplementedException();
        }

        public void RestoreMode()
        {
            //throw new NotImplementedException();
        }

        private bool IsNotNested(VisibleLineInfo dragLine)
        {
            var lineSizeCollection = this.grid.Model.RowHeights as LineSizeCollection;
            var result = lineSizeCollection.GetNestedLines(dragLine.LineIndex) != null;
            return result;
        }

        double hitTestPrecision = 4;
        private VisibleLineInfo HitRowTest(Point point)
        {
            return grid.ScrollRows.GetLineNearCorner(point.Y, hitTestPrecision);

        }

        private VisibleLineInfo HitColTest(Point point)
        {
            return grid.ScrollColumns.GetLineNearCorner(point.X, hitTestPrecision);
        }

        public int HitTest(MouseControllerEventArgs mouseEventArgs, IMouseController controller)
        {
            Point point = mouseEventArgs.Location;

            GridRangeInfo range = grid.Model.SelectedCells;
            if (range == null || range.IsEmpty)
            {
                return 0;
            }

            RowColumnIndex pos = grid.PointToCellRowColumnIndex(point, true);
            if (pos != RowColumnIndex.Empty && moveDir != MovingDirection.None)
            {
                return 1;
            }

            VisibleLineInfo hitRow = HitRowTest(point);
            if (hitRow != null && !this.IsNotNested(hitRow) && range.Bottom == hitRow.LineIndex)
            {
                VisibleLineInfo column = HitColTest(point);//Grid.ScrollColumns.GetVisibleLineAtPoint(point.X);
                if (column != null && range.Right == column.LineIndex)
                {
                    //  Grid.InvalidateCells();
                    return 1;
                }
            }

            return 0;
        }


        public bool SupportsCancelMouseCapture
        {
            get { return false; }// throw new NotImplementedException(); }
        }

        public bool SupportsMouseTracking
        {
            get { return false; }// throw new NotImplementedException(); }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            grid.SelectionChanged -= new GridSelectionChangedEventHandler(grid_SelectionChanged);
            this.ExcelRangeExtended -= new ExcelRangeExtendedEventHandler(GridExcelMarkerMouseController_ExcelRangeExtended);
            codePopup = null;
        }

        #endregion

        public void FillOptionChanged(string Option)
        {
            this.filltype = Option;
            this.FillDraggedRanges();
            this.grid.InvalidateCell(this.grid.Model.SelectedCells);
        }
    }

    public class ExcelRangeExtendedEventArgs : EventArgs
    {
        public ExcelRangeExtendedEventArgs(GridRangeInfo originalRange, GridRangeInfo newRange, string movingDirection,string filltype)
        {
            this.originalRange = originalRange;
            this.newRange = newRange;
            this.movingDirection = movingDirection;
            this.filltype = filltype;
        }
        GridRangeInfo originalRange;

        public GridRangeInfo OriginalRange
        {
            get { return originalRange; }
            set { originalRange = value; }
        }

        GridRangeInfo newRange;

        public GridRangeInfo NewRange
        {
            get { return newRange; }
            set { newRange = value; }
        }

        string movingDirection;

        public string MovingDirection
        {
            get { return movingDirection; }
            set { movingDirection = value; }
        }

        string filltype;

        public string FillType
        {
            get { return filltype; }
            set { filltype = value; }
        }
    }

    public class ExcelGridCellData : RowColumnIndexValueArray<SpreadsheetGridStyleInfoStore>
    {
    }
}

